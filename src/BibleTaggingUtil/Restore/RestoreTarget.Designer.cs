namespace BibleTaggingUtil.Restore
{
    partial class RestoreTarget
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dgvTaggedFiles = new System.Windows.Forms.DataGridView();
            datetime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            file = new System.Windows.Forms.DataGridViewTextBoxColumn();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            panel2 = new System.Windows.Forms.Panel();
            btnCancel = new System.Windows.Forms.Button();
            btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)dgvTaggedFiles).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // dgvTaggedFiles
            // 
            dgvTaggedFiles.BackgroundColor = System.Drawing.Color.White;
            dgvTaggedFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTaggedFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { datetime, file });
            dgvTaggedFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvTaggedFiles.GridColor = System.Drawing.Color.Cyan;
            dgvTaggedFiles.Location = new System.Drawing.Point(3, 3);
            dgvTaggedFiles.MultiSelect = false;
            dgvTaggedFiles.Name = "dgvTaggedFiles";
            dgvTaggedFiles.ReadOnly = true;
            dgvTaggedFiles.RowHeadersWidth = 51;
            dgvTaggedFiles.RowTemplate.Height = 29;
            dgvTaggedFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvTaggedFiles.Size = new System.Drawing.Size(441, 380);
            dgvTaggedFiles.TabIndex = 0;
            // 
            // datetime
            // 
            datetime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            datetime.HeaderText = "Timestamp";
            datetime.MinimumWidth = 150;
            datetime.Name = "datetime";
            datetime.ReadOnly = true;
            datetime.Width = 150;
            // 
            // file
            // 
            file.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            file.HeaderText = "FileName";
            file.MinimumWidth = 6;
            file.Name = "file";
            file.ReadOnly = true;
            file.Width = 101;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(dgvTaggedFiles, 0, 0);
            tableLayoutPanel1.Controls.Add(panel2, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(447, 450);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.BackColor = System.Drawing.Color.White;
            panel2.Controls.Add(btnCancel);
            panel2.Controls.Add(btnOK);
            panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(3, 389);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(441, 58);
            panel2.TabIndex = 2;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(195, 14);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(94, 35);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.BackColor = System.Drawing.Color.PaleGreen;
            btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnOK.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            btnOK.Location = new System.Drawing.Point(24, 14);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(94, 35);
            btnOK.TabIndex = 6;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = false;
            btnOK.Click += btnOK_Click;
            // 
            // RestoreTarget
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(447, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "RestoreTarget";
            Text = "RestoreTarget";
            Load += RestoreTarget_Load;
            ((System.ComponentModel.ISupportInitialize)dgvTaggedFiles).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTaggedFiles;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn datetime;
        private System.Windows.Forms.DataGridViewTextBoxColumn file;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}