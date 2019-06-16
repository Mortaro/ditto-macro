using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ditto.Files
{
    class Updater
    {

        static string Manifest = "/update.txt";

        public static void Execute(Launcher launcher)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    string update = wc.DownloadString(launcher.Domain + Manifest);
                    string[] Lines = update.Trim().Split('\n');
                    launcher.UpdateLink = Lines[2];
                    launcher.UpdateLinkLabel.Show();
                    string hashes = "";
                    for (int i = 3; i < Lines.Length; i++)
                    {
                        string[] arguments = Lines[i].Trim().Split(' ');
                        hashes += Update(launcher, arguments[0], arguments[1]);
                        hashes += Environment.NewLine;
                    }
                    launcher.UpdateTitleLabel.Text = Lines[0];
                    launcher.UpdateDescriptionLabel.Text = Lines[1];
                    launcher.LaunchClientButton.Enabled = true;
                    launcher.LaunchClientButton.Show();
                    File.WriteAllText(Directory.GetCurrentDirectory() + "/kalonline/data/hashes.txt", hashes);
                }
                catch (WebException e)
                {
                    launcher.UpdateTitleLabel.Text = "Something went wrong";
                    launcher.UpdateDescriptionLabel.Text = "Please restart the launcher later";
                }
            }
        }

        static string Update(Launcher launcher, string file, string md5)
        {
            string Base = Directory.GetCurrentDirectory();
            if (!File.Exists(Base+file) || Hash(Base + file) != md5)
            {
                launcher.UpdateDescriptionLabel.Text = "Downloading " + file + "...";
                using (var client = new WebClient())
                {
                    new FileInfo(Base+file).Directory.Create();
                    client.DownloadFile(launcher.Domain + file, Base+file);
                }
            }
            return file + " " + Hash(Base + file);
        }

        static string Hash(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

    }
}
