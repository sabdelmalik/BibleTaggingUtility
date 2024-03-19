namespace BibleTaggingUtil
{
    partial class SearchReportForm
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
            tbOtputPath = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            btnTTFolder = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            checkBoxTarget = new System.Windows.Forms.CheckBox();
            checkBoxTAGNT = new System.Windows.Forms.CheckBox();
            checkBoxTAHOT = new System.Windows.Forms.CheckBox();
            checkBoxTop = new System.Windows.Forms.CheckBox();
            btnCancel = new System.Windows.Forms.Button();
            btnOK = new System.Windows.Forms.Button();
            saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // tbOtputPath
            // 
            tbOtputPath.Location = new System.Drawing.Point(212, 31);
            tbOtputPath.Name = "tbOtputPath";
            tbOtputPath.Size = new System.Drawing.Size(483, 27);
            tbOtputPath.TabIndex = 9;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(22, 33);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(136, 20);
            label6.TabIndex = 8;
            label6.Text = "Output Report Path";
            // 
            // btnTTFolder
            // 
            btnTTFolder.BackColor = System.Drawing.SystemColors.ButtonFace;
            btnTTFolder.BackgroundImage = Properties.Resources.ellipsisTX;
            btnTTFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            btnTTFolder.Font = new System.Drawing.Font("Wingdings", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnTTFolder.Location = new System.Drawing.Point(733, 29);
            btnTTFolder.Name = "btnTTFolder";
            btnTTFolder.Size = new System.Drawing.Size(33, 31);
            btnTTFolder.TabIndex = 10;
            btnTTFolder.UseVisualStyleBackColor = false;
            btnTTFolder.Click += btnTTFolder_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBoxTarget);
            groupBox1.Controls.Add(checkBoxTAGNT);
            groupBox1.Controls.Add(checkBoxTAHOT);
            groupBox1.Controls.Add(checkBoxTop);
            groupBox1.Location = new System.Drawing.Point(22, 225);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(766, 79);
            groupBox1.TabIndex = 11;
            groupBox1.TabStop = false;
            groupBox1.Text = "Bible To Search";
            // 
            // checkBoxTarget
            // 
            checkBoxTarget.AutoSize = true;
            checkBoxTarget.Location = new System.Drawing.Point(641, 31);
            checkBoxTarget.Name = "checkBoxTarget";
            checkBoxTarget.Size = new System.Drawing.Size(72, 24);
            checkBoxTarget.TabIndex = 2;
            checkBoxTarget.Text = "Target";
            checkBoxTarget.UseVisualStyleBackColor = true;
            // 
            // checkBoxTAGNT
            // 
            checkBoxTAGNT.AutoSize = true;
            checkBoxTAGNT.Location = new System.Drawing.Point(432, 31);
            checkBoxTAGNT.Name = "checkBoxTAGNT";
            checkBoxTAGNT.Size = new System.Drawing.Size(48, 24);
            checkBoxTAGNT.TabIndex = 1;
            checkBoxTAGNT.Text = "TA";
            checkBoxTAGNT.UseVisualStyleBackColor = true;
            // 
            // checkBoxTAHOT
            // 
            checkBoxTAHOT.AutoSize = true;
            checkBoxTAHOT.Location = new System.Drawing.Point(223, 31);
            checkBoxTAHOT.Name = "checkBoxTAHOT";
            checkBoxTAHOT.Size = new System.Drawing.Size(48, 24);
            checkBoxTAHOT.TabIndex = 1;
            checkBoxTAHOT.Text = "TA";
            checkBoxTAHOT.UseVisualStyleBackColor = true;
            // 
            // checkBoxTop
            // 
            checkBoxTop.AutoSize = true;
            checkBoxTop.Location = new System.Drawing.Point(6, 31);
            checkBoxTop.Name = "checkBoxTop";
            checkBoxTop.Size = new System.Drawing.Size(56, 24);
            checkBoxTop.TabIndex = 0;
            checkBoxTop.Text = "Top";
            checkBoxTop.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(501, 369);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(94, 35);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.BackColor = System.Drawing.Color.PaleGreen;
            btnOK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnOK.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            btnOK.Location = new System.Drawing.Point(165, 369);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(94, 35);
            btnOK.TabIndex = 13;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = false;
            btnOK.Click += btnOK_Click;
            // 
            // SearchReportForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(groupBox1);
            Controls.Add(tbOtputPath);
            Controls.Add(label6);
            Controls.Add(btnTTFolder);
            Name = "SearchReportForm";
            Text = "SearchReportForm";
            Load += SearchReportForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox tbOtputPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnTTFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxTarget;
        private System.Windows.Forms.CheckBox checkBoxTAHOT;
        private System.Windows.Forms.CheckBox checkBoxTop;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.CheckBox checkBoxTAGNT;
    }
}