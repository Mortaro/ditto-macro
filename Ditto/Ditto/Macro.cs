using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ditto
{
    public partial class Macro : Form
    {

        public int Index = 0;
        private Thread Worker;
        public List<IntPtr> Windows = new List<IntPtr>();
        public bool Running = false;
        public Launcher Launcher;
        public string Channel = "";
        public string Character = "";
        public List<string> Commands = new List<string>();

        public Macro(int Index, Launcher launcher)
        {
            InitializeComponent();
            this.Index = Index;
            this.Launcher = launcher;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                (260 * (Index / 2)) + 30,
                (Index % 2 == 0) ? 30 : 330
            );
        }

        private void Macro_Load(object sender, EventArgs e)
        {
            ParseCommands();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Stop();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Macro_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private void TargetButton_Click(object sender, EventArgs e)
        {
            RegisterHotKey(base.Handle, 1, 0, 119);
            TargetButton.Text = "...";
        }

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 786)
            {
                if (m.WParam.ToInt32() == 1)
                {
                    IntPtr window = GetForegroundWindow();
                    if(this.Handle == window)
                    {
                        this.Windows.Clear();
                    }
                    else if (!this.Windows.Contains(window))
                    {
                        this.Windows.Add(window);
                        if(this.Character != "")
                        {
                            foreach (var macro in this.Launcher.Macros)
                            {
                                if(macro.Character == this.Character && !macro.Windows.Contains(window))
                                {
                                    macro.Windows.Add(window);
                                    macro.SetWindowTitle();
                                }
                            }
                        }
                    }
                    SetWindowTitle();
                    UnregisterHotKey(base.Handle, 1);
                    TargetButton.Text = "F8";
                }
                else if (m.WParam.ToInt32() == 2)
                {
                    IntPtr window = GetForegroundWindow();
                    Rect windowPosition = new Rect();
                    GetWindowRect(window, ref windowPosition);
                    int X = Cursor.Position.X - windowPosition.Left;
                    int Y = Cursor.Position.Y - windowPosition.Top - 28;
                    Color pixel = Ditto.Commands.Pixel.GetPixel(window, X, Y);
                    CommandsInput.Text = CommandsInput.Text.Insert(CommandsInput.SelectionStart, X.ToString() + " " + Y.ToString() + " " + pixel.R.ToString() + "." + pixel.G.ToString() + "." + pixel.B.ToString());
                    UnregisterHotKey(base.Handle, 2);
                    CoordinatesButton.Text = "F7";
                }
            }
            base.WndProc(ref m);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (Running) Stop();
            else Start();
        }

        public void Start()
        {
            if (CommandsInput.TextLength > 1)
            {
                Running = true;
                SetStartButtonText("Stop");
                this.Worker = new Thread(new ThreadStart(this.ExecuteCommands));
                this.Worker.IsBackground = true;
                this.Worker.Start();
                CommandsInput.Hide();
                CommandsDisplay.Show();
            }
        }

        public void Stop()
        {
            Running = false;
            SetStartButtonText("Start");
            if (this.Worker != null)
            {
                this.Worker.Join();
                this.Worker = null;
            }
            CommandsDisplay.Hide();
            CommandsInput.Show();
        }

        public void SetStartButtonText(string text)
        {
            StartButton.Text = text;
        }

        private void ExecuteCommands()
        {
            while (Running)
            {
                int currentline = 0;
                foreach (string Line in Commands)
                {
                    if (!this.Launcher.PerformanceMode.Checked)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate ()
                        {
                            CommandsDisplay.Clear();
                            currentline++;
                            int currentInstruction = 0;
                            foreach (string instruction in Commands)
                            {
                                currentInstruction++;
                                if (currentline == currentInstruction)
                                {
                                    int from = this.CommandsDisplay.TextLength;
                                    this.CommandsDisplay.AppendText(instruction);
                                    int to = this.CommandsDisplay.TextLength;
                                    this.CommandsDisplay.Select(from, to);
                                    this.CommandsDisplay.SelectionColor = Color.Red;
                                    this.CommandsDisplay.ScrollToCaret();
                                    this.CommandsDisplay.Select(0, 0);
                                }
                                else
                                {
                                    this.CommandsDisplay.AppendText(instruction);
                                }
                                this.CommandsDisplay.AppendText("\n");
                            }
                        }));
                    }
                    string[] Arguments = Line.Trim().Split(' ');
                    if (Arguments.Length > 0)
                    {
                        if (Arguments[0] == "keypress")
                        {
                            Ditto.Commands.Keypress.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "rightclick")
                        {
                            Ditto.Commands.Rightclick.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "leftclick")
                        {
                            Ditto.Commands.Leftclick.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "wait")
                        {
                            Ditto.Commands.Wait.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "stop")
                        {
                            Ditto.Commands.Stop.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "start")
                        {
                            Ditto.Commands.Start.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "cursor")
                        {
                            Ditto.Commands.Cursor.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "pixel")
                        {
                            Ditto.Commands.Pixel.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "leftclickmonster")
                        {
                            Ditto.Commands.LeftClickMonster.Execute(this, Arguments);
                        }
                    }
                    Thread.Sleep(100);
                }
            }
        }

        public void SetCommands(string commands)
        {
            CommandsInput.Text = commands;
        }

        public string GetCommands()
        {
            return CommandsInput.Text;
        }

        private void CoordinatesButton_Click(object sender, EventArgs e)
        {
            RegisterHotKey(base.Handle, 2, 0, 118);
            CoordinatesButton.Text = "...";
        }

        private void ParseCommands()
        {
            string[] Lines = this.CommandsInput.Text.Trim().Split('\n');
            this.Channel = "";
            this.Character = "";
            this.Commands.Clear();
            foreach (string Line in Lines)
            {
                if (!Line.StartsWith("//"))
                {
                    if (Line.StartsWith("#"))
                    {
                        this.Channel = Line.Trim();
                    }
                    else if (Line.StartsWith("@"))
                    {
                        this.Character = Line.Trim();
                    }
                    else
                    {
                        string[] instructions = Line.Trim().Split(' ');
                        if (instructions.Length < 4)
                        {
                            this.Commands.Add(string.Join(" ", instructions));
                        }
                        else
                        {
                            int times = 1;
                            string interval = "";
                            string possibleMultiplier = instructions[instructions.Length - 2];
                            if (possibleMultiplier.Contains('x'))
                            {
                                if (Int32.TryParse(possibleMultiplier.Remove(possibleMultiplier.Length - 1), out int value))
                                {
                                    times = value;
                                }
                                interval = instructions[instructions.Length - 1];
                                string line = string.Join(" ", instructions.Reverse().Skip(2).Reverse());
                                for (int i = 0; i < times; i++)
                                {
                                    this.Commands.Add(line);
                                    this.Commands.Add("wait " + interval);
                                }
                            }
                            else
                            {
                                this.Commands.Add(string.Join(" ", instructions));
                            }
                        }
                    }
                }
            }
            SetWindowTitle();
        }

        private void SetWindowTitle()
        {
            List<string> Titles = new List<string>();
            if (this.Character != "")
            {
                Titles.Add(this.Character);
            }
            if (this.Channel != "")
            {
                Titles.Add(this.Channel);
            }
            Titles.Add("Tool " + this.Index);
            if (this.Windows.Count > 0)
            {
                Titles.Add("✓");
            }
            string Title = string.Join(" - ", Titles);
            this.Text = Title;
            TitleLabel.Text = Title;
        }

        private void CommandsInput_TextChanged(object sender, EventArgs e)
        {
            ParseCommands();
        }

    }
}
