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

namespace Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Username = textBox1.Text;
            string Password = textBox2.Text;
            string Lang = listBox1.SelectedItem != null ? (string)listBox1.SelectedItem : "English";

            string RemoteServer = textBox3.Text;

            string cert = "";

            if(!File.Exists(cert + @"\SiennaCert.pfx"))
                File.Create(cert + @"\SiennaCert.pfx");

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

            Process process = new Process();
            process.StartInfo.FileName = "rift.exe";
            process.StartInfo.Arguments = "-u " + Username + " -k " + cert + @"\SiennaCert.pfx -l " + Lang + " -s " + RemoteServer;
            process.Start();
        }
    }
}
