using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ditto
{

    public partial class Launcher : Form
    {

        public string Domain = "https://dittokal.com";

        public List<Macro> Macros = new List<Macro>();

        public Launcher()
        {
            InitializeComponent();
        }

        private void Launcher_Load(object sender, EventArgs e)
        {
            Files.Updater.Execute(this);
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

        Timer timer = new Timer();

        private void LaunchClientButton_Click(object sender, EventArgs e)
        {
            LaunchClientButton.Enabled = false;
            LaunchClientButton.Text = "...";
            timer.Interval = 10000;
            timer.Tick += LaunchClientTick;
            timer.Start();
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C set __COMPAT_LAYER=Win8RTM && cd kalonline && engine.exe";
            process.StartInfo = startInfo;
            process.Start();
        }

        private void LaunchClientTick(object sender, System.EventArgs e)
        {
            LaunchClientButton.Enabled = true;
            LaunchClientButton.Text = "Launch Client";
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
            LoadMacroButton.Text = "Save Macro";
            return macro;
        }

        private void LoadMacroButton_Click(object sender, EventArgs e)
        {
            if (Macros.Count == 0) LoadMacros();
            else SaveMacros();
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

        private void SaveMacros()
        {
            if (this.SaveMacroDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = this.SaveMacroDialog.FileName;
                string serialization = "";
                foreach (Macro macro in Macros)
                {
                    serialization += macro.GetCommands() + Environment.NewLine + Environment.NewLine;
                }
                File.WriteAllText(fileName, serialization.TrimEnd(new char[] { '\r', '\n', ' ' }));
            }
        }

        private void UpdateLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Domain + UpdateLink);
        }

        private void WebsiteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Domain);
        }
    }
}
