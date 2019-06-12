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
        public IntPtr Window;
        public bool Running = false;
        public Launcher Launcher;

        public Macro(int Index, Launcher launcher)
        {
            InitializeComponent();
            this.Index = Index;
            SetWindowTitle("Ditto " + Index);
            this.Launcher = launcher;
        }

        private void Macro_Load(object sender, EventArgs e)
        {

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
                    IntPtr zero = IntPtr.Zero;
                    this.Window = GetForegroundWindow();
                    SetWindowTitle("Ditto " + this.Index + " - Handler " + Window.ToString());
                    UnregisterHotKey(base.Handle, 1);
                    TargetButton.Text = "F8";
                }
                else if (m.WParam.ToInt32() == 2)
                {

                    IntPtr zero = IntPtr.Zero;
                    this.Window = GetForegroundWindow();
                    Rect windowPosition = new Rect();
                    GetWindowRect(this.Window, ref windowPosition);
                    int X = Cursor.Position.X - windowPosition.Left;
                    int Y = Cursor.Position.Y - windowPosition.Top - 28;
                    CommandsInput.Text = CommandsInput.Text.Insert(CommandsInput.SelectionStart, X + " " + Y);
                    UnregisterHotKey(base.Handle, 2);
                    CoordinatesButton.Text = "F7";
                }
            }
            base.WndProc(ref m);
        }

        public void SetWindowTitle(string text)
        {
            this.Text = text;
            TitleLabel.Text = text;
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
            string[] Lines = this.CommandsInput.Text.Trim().Split('\n');
            while (Running)
            {
                int currentline = 0;
                foreach (string Line in Lines)
                {
                    this.BeginInvoke(new MethodInvoker(delegate ()
                    {
                        CommandsDisplay.Clear();
                        currentline++;
                        int currentInstruction = 0;
                        foreach (string instruction in Lines)
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
                        }
                    }));
                    System.Diagnostics.Debug.WriteLine(Line);
                    string[] Arguments = Line.Trim().Split(' ');
                    if (Arguments.Length > 0)
                    {
                        if (Arguments[0] == "keypress")
                        {
                            Commands.Keypress.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "rightclick")
                        {
                            Commands.Rightclick.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "leftclick")
                        {
                            Commands.Leftclick.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "wait")
                        {
                            Commands.Wait.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "stop")
                        {
                            Commands.Stop.Execute(this, Arguments);
                        }
                        else if (Arguments[0] == "start")
                        {
                            Commands.Start.Execute(this, Arguments);
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
    }
}
