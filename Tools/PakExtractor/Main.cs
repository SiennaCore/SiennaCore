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

namespace PakExtractor
{
    public partial class Extractor : Form
    {
        static public string FilesFolder = "";
        static public Extractor Instance = null;
        static public DataGridView Data = null;
        static public ComboBox Box = null;
        static public Timer Time = new Timer();
        static public PakFile Decoding = null;
        static public PakFile Extracting = null;
        static public int AllId = -1;
        static public string ToolText = "";

        public Extractor()
        {
            InitializeComponent();

            Instance = this;
            Data = datagrid;
            Box = l_files;

            Time.Enabled = true;
            Time.Interval = 20;
            Time.Tick += new EventHandler(Update);
            Time.Start();

            l_files.SelectedValueChanged+=new EventHandler(l_files_SelectedValueChanged);

            ExtractorMgr.LoadHeaders();
            ExtractorMgr.StartExtractorThread();

            b_allpak.Enabled = false;
            b_extract.Enabled = false;
            l_files.Enabled = false;

            b_bik.CheckedChanged += new EventHandler(Filtre);
            b_wav.CheckedChanged += new EventHandler(Filtre);
            b_unk.CheckedChanged += new EventHandler(Filtre);
            b_nif.CheckedChanged += new EventHandler(Filtre);
            b_dds.CheckedChanged += new EventHandler(Filtre);

            this.FormClosing += new FormClosingEventHandler(OnExit);
        }

        private void b_listing_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Dial;
            if (FilesFolder.Length <= 0)
            {
                Dial = new FolderBrowserDialog();
                Dial.ShowDialog();
                if (Dial.SelectedPath.Length >= 0)
                {
                    try
                    {
                        String[] fichiers = Directory.GetFiles(Dial.SelectedPath, "*.pak", SearchOption.AllDirectories);
                        foreach (string FileLink in fichiers)
                        {
                            FileInfo Info = new FileInfo(FileLink);
                            l_files.Items.Add(Info);
                        }

                        FilesFolder = Dial.SelectedPath;
                        tip_status.Text = fichiers.Length + " Fichiers";
                        b_allpak.Enabled = true;
                        b_extract.Enabled = true;
                        l_files.Enabled = true;
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                        MessageBox.Show("Could not access the selected Path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void l_files_SelectedValueChanged(object sender, EventArgs e)
        {
            if (l_files.SelectedItem == null)
                return;

            if (Decoding != null && Decoding.Decoded == false)
                return;

            l_files.Enabled = false;
            ClearData();
            Decoding = ExtractorMgr.DecodePak((l_files.SelectedItem as FileInfo));
        }

        public void Tool(string Text)
        {
            ToolText = Text;
        }

        public int ProgressValue;
        public void Progress(int value)
        {
            ProgressValue = value;
        }

        public void ClearData()
        {
            datagrid.Rows.Clear();
        }

        public void Filtre(object Sender,EventArgs a)
        {
            if(Decoding != null)
                Decoding.PrintHeaders();
        }

        public bool IsChecked(string Ext)
        {
            switch (Ext)
            {                
                case ".wav":
                    return b_wav.Checked;
                case ".dds":
                    return b_dds.Checked;
                case ".nif":
                    return b_nif.Checked;
                case ".unk":
                    return b_unk.Checked;
                case ".bik":
                    return b_bik.Checked;
            };

            return true;
        }

        public void AddData(params object[] Params)
        {
            if (!IsChecked(Params[1].ToString()))
                return;

            datagrid.Rows.Add(Params);
        }

        public void Update(object sender,EventArgs a)
        {
            tool_progress.Value = ProgressValue;
            tip_status.Text = ToolText;

            if (datagrid.Rows.Count <= 0 && Decoding != null && Decoding.Decoded)
            {
                Decoding.PrintHeaders();
                l_files.Enabled = true;
            }

            if (AllId >= 0 && (Decoding == null || Decoding.Decoded) && ExtractorMgr._ToExtract.Count <= 0)
            {
                if (Decoding != null)
                {
                    foreach (PakElement Element in Decoding._Elements)
                        if (Extractor.Instance.IsChecked(Element.Header.Ext))
                            ExtractorMgr.AddToExtract(Element);
                }

                if (AllId < l_files.Items.Count)
                {
                    FileInfo Info = l_files.Items[AllId] as FileInfo;

                    if (Info != null)
                    {
                        l_files.SelectedIndex = AllId;
                        Decoding = ExtractorMgr.DecodePak(Info);
                        l_files.Enabled = false;
                    }

                    ++AllId;
                }
                else
                    AllId = -1;
            }
        }

        public void OnExit(object sender, FormClosingEventArgs a)
        {
            lock (_ToExtract)
                _ToExtract.Clear();

            Extracting = null;
            Decoding = null;

            Time.Stop();

            ExtractorMgr.IsRunning = false;
        }

        private void b_extract_Click(object sender, EventArgs e)
        {
            ExtractorMgr.ExtractPack();
        }

        public List<FileInfo> _ToExtract = new List<FileInfo>();
        private void b_allpak_Click(object sender, EventArgs e)
        {
            if (ExtractorMgr.ExtractingFolder.Length <= 0)
            {
                FolderBrowserDialog Dial = new FolderBrowserDialog();
                Dial.SelectedPath = Directory.GetCurrentDirectory();
                Dial.ShowDialog();
                ExtractorMgr.ExtractingFolder = Dial.SelectedPath;

                if (ExtractorMgr.ExtractingFolder.Length <= 0)
                    return;
            }

            try
            {
                if (AllId < 0)
                    AllId = 0;
                else
                    AllId =  -1;
            }
            catch (Exception ex)
            {
                Tool(ex.ToString());
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            string windir = Environment.GetEnvironmentVariable("WINDIR");
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = "explorer";
            prc.StartInfo.Arguments = this.toolStripStatusLabel1.Text;
            prc.Start();
        }
    }
}
