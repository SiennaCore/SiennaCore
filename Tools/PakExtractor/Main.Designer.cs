namespace PakExtractor
{
    partial class Extractor
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tip_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.tool_progress = new System.Windows.Forms.ToolStripProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.datagrid = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Extention = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ZSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RealSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Extract = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Software = new System.Windows.Forms.DataGridViewLinkColumn();
            this.b_listing = new System.Windows.Forms.Button();
            this.b_extract = new System.Windows.Forms.Button();
            this.l_files = new System.Windows.Forms.ComboBox();
            this.b_allpak = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.b_bik = new System.Windows.Forms.CheckBox();
            this.b_dds = new System.Windows.Forms.CheckBox();
            this.b_wav = new System.Windows.Forms.CheckBox();
            this.b_nif = new System.Windows.Forms.CheckBox();
            this.b_unk = new System.Windows.Forms.CheckBox();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.datagrid)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tip_status,
            this.tool_progress,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(750, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tip_status
            // 
            this.tip_status.Name = "tip_status";
            this.tip_status.Size = new System.Drawing.Size(633, 17);
            this.tip_status.Spring = true;
            this.tip_status.Text = "Starting...";
            // 
            // tool_progress
            // 
            this.tool_progress.Name = "tool_progress";
            this.tool_progress.Size = new System.Drawing.Size(100, 16);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Files";
            // 
            // datagrid
            // 
            this.datagrid.AllowUserToAddRows = false;
            this.datagrid.AllowUserToDeleteRows = false;
            this.datagrid.AllowUserToOrderColumns = true;
            this.datagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Extention,
            this.ZSize,
            this.RealSize,
            this.Extract,
            this.Software});
            this.datagrid.Location = new System.Drawing.Point(0, 112);
            this.datagrid.Name = "datagrid";
            this.datagrid.Size = new System.Drawing.Size(750, 313);
            this.datagrid.TabIndex = 3;
            // 
            // Id
            // 
            this.Id.HeaderText = "Name";
            this.Id.Name = "Id";
            // 
            // Extention
            // 
            this.Extention.HeaderText = "Extention";
            this.Extention.Name = "Extention";
            // 
            // ZSize
            // 
            this.ZSize.HeaderText = "ZSize";
            this.ZSize.Name = "ZSize";
            // 
            // RealSize
            // 
            this.RealSize.HeaderText = "RealSize";
            this.RealSize.Name = "RealSize";
            // 
            // Extract
            // 
            this.Extract.HeaderText = "Extract";
            this.Extract.Name = "Extract";
            // 
            // Software
            // 
            this.Software.HeaderText = "Software";
            this.Software.Name = "Software";
            this.Software.Width = 200;
            // 
            // b_listing
            // 
            this.b_listing.Location = new System.Drawing.Point(0, 16);
            this.b_listing.Name = "b_listing";
            this.b_listing.Size = new System.Drawing.Size(97, 23);
            this.b_listing.TabIndex = 4;
            this.b_listing.Text = "Pak Folder";
            this.b_listing.UseVisualStyleBackColor = true;
            this.b_listing.Click += new System.EventHandler(this.b_listing_Click);
            // 
            // b_extract
            // 
            this.b_extract.Location = new System.Drawing.Point(523, 83);
            this.b_extract.Name = "b_extract";
            this.b_extract.Size = new System.Drawing.Size(92, 23);
            this.b_extract.TabIndex = 5;
            this.b_extract.Text = "Extract this Pak";
            this.b_extract.UseVisualStyleBackColor = true;
            this.b_extract.Click += new System.EventHandler(this.b_extract_Click);
            // 
            // l_files
            // 
            this.l_files.FormattingEnabled = true;
            this.l_files.Location = new System.Drawing.Point(0, 85);
            this.l_files.Name = "l_files";
            this.l_files.Size = new System.Drawing.Size(510, 21);
            this.l_files.TabIndex = 6;
            // 
            // b_allpak
            // 
            this.b_allpak.Location = new System.Drawing.Point(104, 16);
            this.b_allpak.Name = "b_allpak";
            this.b_allpak.Size = new System.Drawing.Size(75, 23);
            this.b_allpak.TabIndex = 7;
            this.b_allpak.Text = "Extract All Pak";
            this.b_allpak.UseVisualStyleBackColor = true;
            this.b_allpak.Click += new System.EventHandler(this.b_allpak_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(185, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Types to Extract";
            // 
            // b_bik
            // 
            this.b_bik.AutoSize = true;
            this.b_bik.Checked = true;
            this.b_bik.CheckState = System.Windows.Forms.CheckState.Checked;
            this.b_bik.Location = new System.Drawing.Point(361, 30);
            this.b_bik.Name = "b_bik";
            this.b_bik.Size = new System.Drawing.Size(43, 17);
            this.b_bik.TabIndex = 10;
            this.b_bik.Text = ".bik";
            this.b_bik.UseVisualStyleBackColor = true;
            // 
            // b_dds
            // 
            this.b_dds.AutoSize = true;
            this.b_dds.Checked = true;
            this.b_dds.CheckState = System.Windows.Forms.CheckState.Checked;
            this.b_dds.Location = new System.Drawing.Point(361, 8);
            this.b_dds.Name = "b_dds";
            this.b_dds.Size = new System.Drawing.Size(46, 17);
            this.b_dds.TabIndex = 11;
            this.b_dds.Text = ".dds";
            this.b_dds.UseVisualStyleBackColor = true;
            // 
            // b_wav
            // 
            this.b_wav.AutoSize = true;
            this.b_wav.Checked = true;
            this.b_wav.CheckState = System.Windows.Forms.CheckState.Checked;
            this.b_wav.Location = new System.Drawing.Point(275, 7);
            this.b_wav.Name = "b_wav";
            this.b_wav.Size = new System.Drawing.Size(49, 17);
            this.b_wav.TabIndex = 12;
            this.b_wav.Text = ".wav";
            this.b_wav.UseVisualStyleBackColor = true;
            // 
            // b_nif
            // 
            this.b_nif.AutoSize = true;
            this.b_nif.Checked = true;
            this.b_nif.CheckState = System.Windows.Forms.CheckState.Checked;
            this.b_nif.Location = new System.Drawing.Point(275, 30);
            this.b_nif.Name = "b_nif";
            this.b_nif.Size = new System.Drawing.Size(40, 17);
            this.b_nif.TabIndex = 13;
            this.b_nif.Text = ".nif";
            this.b_nif.UseVisualStyleBackColor = true;
            // 
            // b_unk
            // 
            this.b_unk.AutoSize = true;
            this.b_unk.Checked = true;
            this.b_unk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.b_unk.Location = new System.Drawing.Point(448, 7);
            this.b_unk.Name = "b_unk";
            this.b_unk.Size = new System.Drawing.Size(47, 17);
            this.b_unk.TabIndex = 14;
            this.b_unk.Text = ".unk";
            this.b_unk.UseVisualStyleBackColor = true;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.IsLink = true;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(151, 17);
            this.toolStripStatusLabel1.Text = "http://siennacore.com";
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // Extractor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 450);
            this.Controls.Add(this.b_unk);
            this.Controls.Add(this.b_nif);
            this.Controls.Add(this.b_wav);
            this.Controls.Add(this.b_dds);
            this.Controls.Add(this.b_bik);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.b_allpak);
            this.Controls.Add(this.l_files);
            this.Controls.Add(this.b_extract);
            this.Controls.Add(this.b_listing);
            this.Controls.Add(this.datagrid);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "Extractor";
            this.Text = "PakExtractor";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.datagrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tip_status;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView datagrid;
        private System.Windows.Forms.Button b_listing;
        private System.Windows.Forms.Button b_extract;
        private System.Windows.Forms.ComboBox l_files;
        private System.Windows.Forms.Button b_allpak;
        private System.Windows.Forms.ToolStripProgressBar tool_progress;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Extention;
        private System.Windows.Forms.DataGridViewTextBoxColumn ZSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn RealSize;
        private System.Windows.Forms.DataGridViewButtonColumn Extract;
        private System.Windows.Forms.DataGridViewLinkColumn Software;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox b_bik;
        private System.Windows.Forms.CheckBox b_dds;
        private System.Windows.Forms.CheckBox b_wav;
        private System.Windows.Forms.CheckBox b_nif;
        private System.Windows.Forms.CheckBox b_unk;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

