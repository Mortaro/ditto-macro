using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using WebSocketSharp;

namespace Ditto
{

    public partial class Launcher : Form
    {

        public List<Macro> Macros = new List<Macro>();

        public Dictionary<string, List<IntPtr>> CapturesByCharacter = new Dictionary<string, List<IntPtr>>();

        public void StoreCapture(string character, IntPtr window)
        {
            if (string.IsNullOrEmpty(character)) return;
            List<IntPtr> captures;
            if (!CapturesByCharacter.TryGetValue(character, out captures))
            {
                captures = new List<IntPtr>();
                CapturesByCharacter[character] = captures;
            }
            if (!captures.Contains(window)) captures.Add(window);
        }

        public WebSocket Socket;

        public Launcher()
        {
            InitializeComponent();
        }

        private void Launcher_Load(object sender, EventArgs e)
        {
            HostInput.Text = Properties.Settings.Default.Host;
            PasswordInput.Text = Properties.Settings.Default.Password;
        }

        public string UpdateLink = "";

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Launcher_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void ConnectSocket()
        {
            if (this.Socket == null && this.HostInput.Text != "")
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    this.Socket = new WebSocket("ws://" + this.HostInput.Text);
                    Socket.EmitOnPing = true;
                    Socket.OnOpen += (ss, ee) =>
                    {
                        ConnectionStatusLabel.Text = "Mortaro's Tool";
                    };
                    Socket.OnMessage += (ss, ee) =>
                    {
                        if (!ee.IsPing) ReceiveCommand(ee.Data);
                    };
                    Socket.OnClose += (ss, ee) =>
                    {
                        if (this.NetworkMode.Checked)
                        {
                            BeginInvoke(new MethodInvoker(delegate ()
                            {
                                ConnectionStatusLabel.Text = "Reconnecting to the server...";
                            }));
                            for (int i = 0; i < 100; i++)
                            {
                                Thread.Sleep(100);
                            }
                            ConnectSocket();
                        }
                        else
                        {
                            ConnectionStatusLabel.Text = "Mortaro's Tool";
                        }
                    };
                    Socket.Connect();
                }));
            }
        }

        private void DisconnectSocket()
        {
            if (this.Socket == null) return;
            this.Socket.Close();
            this.Socket = null;
        }

        public string Password()
        {
            return this.PasswordInput.Text;
        }

        private void ReceiveCommand(string command)
        {
            BeginInvoke(new MethodInvoker(delegate ()
            { 
                string[] arguments = command.Split(' ');
                if (arguments.Length < 2 || arguments[0] != Password()) return;

                if (arguments[1] == "tool" && arguments.Length >= 4)
                {
                    int index;
                    if (Int32.TryParse(arguments[2], out index)
                        && index >= 0 && index < this.Macros.Count)
                    {
                        try
                        {
                            string script = Encoding.UTF8.GetString(Convert.FromBase64String(arguments[3]));
                            var target = this.Macros[index];
                            bool wasRunning = target.Running;
                            if (wasRunning) target.Stop();
                            target.SetCommands(script);
                            if (wasRunning) target.Start();
                            ConnectionStatusLabel.Text = "Tool " + index + " updated";
                        }
                        catch (FormatException)
                        {
                            ConnectionStatusLabel.Text = "Tool " + index + " not updated: invalid script";
                        }
                    }
                    return;
                }

                if (arguments[1] == "load" && arguments.Length >= 3)
                {
                    string path = string.Join(" ", arguments, 2, arguments.Length - 2);
                    CarregarArquivo(path);
                    return;
                }

                if (arguments.Length >= 3)
                {
                    BeginInvoke(new MethodInvoker(delegate ()
                    {
                        foreach(var macro in this.Macros)
                        {
                            if (
                                (arguments.Length == 3 && (macro.Channel == arguments[2] || macro.Character == arguments[2]))
                                || (arguments.Length == 4 && macro.Channel == arguments[2] && macro.Character == arguments[3])
                            )
                            {
                                if (macro.Visible)
                                {
                                    if (arguments[1] == "start")
                                    {
                                        macro.Start();
                                    }
                                    else if (arguments[1] == "stop")
                                    {
                                        macro.Stop();
                                    }
                                }
                            }
                        }
                    }));
                }
            }));
        }

        public void CarregarArquivo(string path)
        {
            if (!File.Exists(path)) return;
            try
            {
                foreach (var macro in this.Macros.ToArray())
                {
                    macro.Stop();
                    macro.Close();
                }
                this.Macros.Clear();
                foreach (string instructions in File.ReadAllText(path).Split(new string[]
                {
                    Environment.NewLine + Environment.NewLine
                }, StringSplitOptions.None))
                {
                    NewMacro().SetCommands(instructions);
                }
                ArrangeMacros();
                ConnectionStatusLabel.Text = "Macro reloaded";
            }
            catch (IOException)
            {
                ConnectionStatusLabel.Text = "Could not read the macro file";
            }
        }

        private void NewMacroButton_Click(object sender, EventArgs e)
        {
            NewMacro();
            ArrangeMacros();
        }

        public Macro NewMacro()
        {
            Macro macro = new Macro(Macros.Count, this);
            Macros.Add(macro);
            macro.Show();
            return macro;
        }

        private const int MACRO_WIDTH = 253;
        private const int MACRO_HEIGHT = 293;

        private static List<System.Drawing.Rectangle> ScreensLeftToRight()
        {
            var areas = new List<System.Drawing.Rectangle>();
            foreach (Screen screen in Screen.AllScreens) areas.Add(screen.WorkingArea);
            areas.Sort(delegate (System.Drawing.Rectangle a, System.Drawing.Rectangle b)
            {
                return a.X.CompareTo(b.X);
            });
            return areas;
        }

        public void ArrangeMacros()
        {
            var areas = ScreensLeftToRight();
            if (areas.Count == 0 || Macros.Count == 0) return;

            int placed = 0;
            for (int t = 0; t < areas.Count && placed < Macros.Count; t++)
            {
                System.Drawing.Rectangle area = areas[t];
                int columns = Math.Max(1, area.Width / MACRO_WIDTH);
                int remainingScreens = areas.Count - t;
                int howMany = (Macros.Count - placed + remainingScreens - 1) / remainingScreens;
                int rows = (howMany + columns - 1) / columns;

                int stepY = MACRO_HEIGHT;
                if (rows > 1)
                    stepY = Math.Min(MACRO_HEIGHT, (area.Height - MACRO_HEIGHT) / (rows - 1));
                stepY = Math.Max(24, stepY);

                for (int k = 0; k < howMany && placed < Macros.Count; k++, placed++)
                {
                    int x = area.X + (k % columns) * MACRO_WIDTH;
                    int y = area.Y + (k / columns) * stepY;
                    if (x + MACRO_WIDTH > area.Right) x = area.Right - MACRO_WIDTH;
                    if (y + MACRO_HEIGHT > area.Bottom) y = area.Bottom - MACRO_HEIGHT;
                    Macro macro = Macros[placed];
                    macro.StartPosition = FormStartPosition.Manual;
                    macro.Location = new System.Drawing.Point(x, y);
                }
            }
        }

        private void OrganizarButton_Click(object sender, EventArgs e)
        {
            ArrangeMacros();
        }

        private void LoadMacroButton_Click(object sender, EventArgs e)
        {
            LoadMacros();
        }

        private void LoadMacros()
        {
            if (this.LoadMacroDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = this.LoadMacroDialog.FileName;
                try
                {
                    foreach (string instructions in File.ReadAllText(fileName).Split(new string[]
                    {
                        Environment.NewLine + Environment.NewLine
                    }, StringSplitOptions.None))
                    {
                        NewMacro().SetCommands(instructions);
                    }
                    ArrangeMacros();
                }
                catch (IOException)
                {
                    ConnectionStatusLabel.Text = "Could not read the macro file";
                }
            }
        }
        private void SaveMacroButton_Click(object sender, EventArgs e)
        {
            SaveMacros();
        }

        private void SaveMacros()
        {
            if (this.SaveMacroDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = this.SaveMacroDialog.FileName;
                string serialization = "";
                foreach (Macro macro in Macros)
                {
                    if (macro.Visible)
                    {
                        serialization += macro.GetCommands() + Environment.NewLine + Environment.NewLine;
                    }
                }
                File.WriteAllText(fileName, serialization.TrimEnd(new char[] { '\r', '\n', ' ' }));
            }
        }

        private void HostInput_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Host = HostInput.Text;
            Properties.Settings.Default.Save();
        }

        private void PasswordInput_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Password = PasswordInput.Text;
            Properties.Settings.Default.Save();
        }

        private void NetworkMode_CheckedChanged(object sender, EventArgs e)
        {
            if(NetworkMode.Checked)
            {
                ConnectSocket();
            }
            else
            {
                DisconnectSocket();
            }
        }

        private void PerformanceMode_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var macro in this.Macros)
            {
                macro.CommandsDisplay.Clear();
            }
        }

        private void CreditsLabel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://dittokal.com");
        }
    }
}
