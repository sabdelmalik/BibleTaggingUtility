namespace BibleTaggingUtil
{
    partial class ProgressForm
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
            progressBar = new System.Windows.Forms.ProgressBar();
            label = new System.Windows.Forms.Label();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            progressBar.Location = new System.Drawing.Point(0, 0);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(209, 32);
            progressBar.TabIndex = 0;
            // 
            // label
            // 
            label.AutoSize = true;
            label.Dock = System.Windows.Forms.DockStyle.Bottom;
            label.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label.Location = new System.Drawing.Point(0, 11);
            label.Name = "label";
            label.Size = new System.Drawing.Size(55, 23);
            label.TabIndex = 1;
            label.Text = "label1";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(label);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(progressBar);
            splitContainer1.Size = new System.Drawing.Size(209, 70);
            splitContainer1.SplitterDistance = 34;
            splitContainer1.TabIndex = 2;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(209, 70);
            ControlBox = false;
            Controls.Add(splitContainer1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "ProgressForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "ProgressForm";
            TopMost = true;
            Load += ProgressForm_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}