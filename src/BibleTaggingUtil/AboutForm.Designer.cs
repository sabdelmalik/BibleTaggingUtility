namespace BibleTaggingUtil
{
    partial class AboutForm
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
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this.textBoxAbout1 = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            label2.Cursor = System.Windows.Forms.Cursors.Hand;
            label2.ForeColor = System.Drawing.Color.Blue;
            label2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            label2.Location = new System.Drawing.Point(2, 154);
            label2.MaximumSize = new System.Drawing.Size(554, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(534, 23);
            label2.TabIndex = 2;
            label2.Text = "STEPBible.org under CC BY 4.0";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            label3.Cursor = System.Windows.Forms.Cursors.Hand;
            label3.ForeColor = System.Drawing.Color.Blue;
            label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            label3.Location = new System.Drawing.Point(2, 221);
            label3.MaximumSize = new System.Drawing.Size(554, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(534, 23);
            label3.TabIndex = 3;
            label3.Text = "CrossWire Bible Society";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // textBoxAbout1
            // 
            this.textBoxAbout1.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxAbout1.Font = new System.Drawing.Font("Calibri", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxAbout1.Location = new System.Drawing.Point(2, 2);
            this.textBoxAbout1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxAbout1.Multiline = true;
            this.textBoxAbout1.Name = "textBoxAbout1";
            this.textBoxAbout1.ReadOnly = true;
            this.textBoxAbout1.Size = new System.Drawing.Size(534, 107);
            this.textBoxAbout1.TabIndex = 0;
            this.textBoxAbout1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(204, 253);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(94, 29);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(2, 131);
            this.label1.MaximumSize = new System.Drawing.Size(554, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(534, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Hebrew and Greek Strong\'s numbers Obtained from";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(2, 198);
            this.label4.MaximumSize = new System.Drawing.Size(554, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(534, 23);
            this.label4.TabIndex = 4;
            this.label4.Text = "SWORD module generation uses code from";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AboutForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(536, 294);
            this.ControlBox = false;
            this.Controls.Add(label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.textBoxAbout1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "AboutForm";
            this.Text = "About";
            this.Load += new System.EventHandler(this.About_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxAbout1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
    }
}