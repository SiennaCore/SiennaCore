using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;

using System.Security.Cryptography;

using System.Net;
using System.Web;

namespace Launcher
{
    public partial class Form1 : Form
    {
        /// 
        /// CONFIG
        /// 

        private bool UsingCertificateServer = false;
        private string CertificateServerUrl = "http://127.0.0.1/GetCertificate.php";

        //////////////////////////

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Username = textBox1.Text;
            string Password = textBox2.Text;

            bool CanContinue = false;

            SHA1CryptoServiceProvider Crypto = new SHA1CryptoServiceProvider();
            Crypto.Initialize();

            Password = BitConverter.ToString(Crypto.ComputeHash(Encoding.UTF8.GetBytes(Password), 0, Encoding.UTF8.GetByteCount(Password))).Replace("-", "").ToLower();

            string Lang = listBox1.SelectedItem != null ? (string)listBox1.SelectedItem : "English";

            string RemoteServer = textBox3.Text;

            string cert = "";

            if (!UsingCertificateServer)
            {
                using (StreamWriter writer = new StreamWriter(cert + @"\SiennaCert.pfx"))
                {
                    writer.WriteLine("Signature: SiennaAuth");
                    writer.WriteLine("<?xml version=\"1.0\"?>");
                    writer.WriteLine("<ClientAuthCertificate xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
                    writer.WriteLine("<Username>" + Username + "</Username>");
                    writer.WriteLine("<Hash>" + Password + "</Hash>");
                    writer.WriteLine("<Sessionkey></Sessionkey>");
                    writer.WriteLine("</ClientAuthCertificate>");
                }

                CanContinue = true;
            }
            else
            {
                WebClient wc = new WebClient();
                string Certificate = wc.DownloadString(CertificateServerUrl + "?username=" + Username + "&passhash=" + Password);

                switch (Certificate)
                {
                    case "ERR_INVALID_USERNAME":
                        MessageBox.Show("Error, your username is invalid");
                        break;

                    case "ERR_INVALID_PASSWORD":
                        MessageBox.Show("Error, your password is invalid");
                        break;

                    case "ERR_ACTIVE_BAN":
                        MessageBox.Show("Error, you are banned from the server");
                        break;

                    default:
                        CanContinue = true;
                        break;
                }

                if (CanContinue)
                {
                    using (StreamWriter writer = new StreamWriter(cert + @"\SiennaCert.pfx"))
                    {
                        writer.WriteLine("Signature: SiennaAuth");
                        writer.Write(Certificate);
                    }
                }
            }

            if (CanContinue)
            {
                try
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "rift.exe";
                    process.StartInfo.Arguments = "-u " + Username + " -k " + cert + @"\SiennaCert.pfx -l " + Lang + " -s " + RemoteServer;
                    process.Start();
                }
                catch (Exception)
                {
                    MessageBox.Show("Error, the launcher must be placed in the game directory");
                }
            }
        }
    }
}
