using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using WebSocketSharp;

namespace Ditto
{

    public partial class Launcher : Form
    {

        public List<Macro> Macros = new List<Macro>();

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
                if (arguments.Length >= 3 && arguments[0] == Password())
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

        private void NewMacroButton_Click(object sender, EventArgs e)
        {
            NewMacro();
        }

        public Macro NewMacro()
        {
            Macro macro = new Macro(Macros.Count, this);
            Macros.Add(macro);
            macro.Show();
            return macro;
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
                }
                catch (IOException) { }
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
