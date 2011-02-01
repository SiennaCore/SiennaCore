using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

using Sienna.PakLib;

namespace PackageManager
{
    public partial class Form1 : Form
    {
        private delegate void Action();
        private List<Action> Actions = new List<Action>();
        private Package PackOpened = null;
        private bool CanExtract = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (PackOpened != null)
                PackOpened.Close();

            PackOpened = new Package(openFileDialog1.FileName, ReadingEnded, ReadingProgress);

            CanExtract = false;
        }

        private void ReadingProgress(PackedFile pf)
        {
            lock(Actions)
                Actions.Add(() => { listBox1.Items.Add(pf.GetIndex()); });
        }

        private void ReadingEnded()
        {
            CanExtract = true;
        }

        private void openPAKFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (Actions)
            {
                foreach (Action ac in Actions)
                    ac.Invoke();

                Actions.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (PackOpened == null)
                MessageBox.Show("Error, you must first open a pak file");

            if (listBox1.SelectedItem == null)
                MessageBox.Show("Error, your must select a file before trying to extract");

            if (!CanExtract)
                MessageBox.Show("Error, your must wait the end of reading first");

            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            lock (Actions)
            {
                Actions.Add(() =>
                    {
                        FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                        byte[] rawfile = PackOpened.GetFile((int)listBox1.SelectedItem);
                        fs.Write(rawfile, 0, rawfile.Length);
                        fs.Close();
                    }
                );
            }
        }
    }
}
