
namespace BibleTaggingUtil
{
    partial class VerseSelectionPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerseSelectionPanel));
            splitContainerBook = new System.Windows.Forms.SplitContainer();
            lbBookNames = new System.Windows.Forms.ListBox();
            splitContainerChVs = new System.Windows.Forms.SplitContainer();
            lbChapters = new System.Windows.Forms.ListBox();
            lbVerses = new System.Windows.Forms.ListBox();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripPrevious = new System.Windows.Forms.ToolStripLabel();
            toolStripNext = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)splitContainerBook).BeginInit();
            splitContainerBook.Panel1.SuspendLayout();
            splitContainerBook.Panel2.SuspendLayout();
            splitContainerBook.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerChVs).BeginInit();
            splitContainerChVs.Panel1.SuspendLayout();
            splitContainerChVs.Panel2.SuspendLayout();
            splitContainerChVs.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerBook
            // 
            splitContainerBook.Cursor = System.Windows.Forms.Cursors.VSplit;
            splitContainerBook.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerBook.Location = new System.Drawing.Point(0, 25);
            splitContainerBook.Name = "splitContainerBook";
            // 
            // splitContainerBook.Panel1
            // 
            splitContainerBook.Panel1.Controls.Add(lbBookNames);
            // 
            // splitContainerBook.Panel2
            // 
            splitContainerBook.Panel2.Controls.Add(splitContainerChVs);
            splitContainerBook.Size = new System.Drawing.Size(800, 426);
            splitContainerBook.SplitterDistance = 211;
            splitContainerBook.SplitterWidth = 5;
            splitContainerBook.TabIndex = 2;
            // 
            // lbBookNames
            // 
            lbBookNames.Cursor = System.Windows.Forms.Cursors.Hand;
            lbBookNames.Dock = System.Windows.Forms.DockStyle.Fill;
            lbBookNames.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbBookNames.FormattingEnabled = true;
            lbBookNames.ItemHeight = 23;
            lbBookNames.Location = new System.Drawing.Point(0, 0);
            lbBookNames.Name = "lbBookNames";
            lbBookNames.Size = new System.Drawing.Size(211, 426);
            lbBookNames.TabIndex = 0;
            lbBookNames.SelectedIndexChanged += lbBookNames_SelectedIndexChanged;
            // 
            // splitContainerChVs
            // 
            splitContainerChVs.Cursor = System.Windows.Forms.Cursors.VSplit;
            splitContainerChVs.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerChVs.Location = new System.Drawing.Point(0, 0);
            splitContainerChVs.Name = "splitContainerChVs";
            // 
            // splitContainerChVs.Panel1
            // 
            splitContainerChVs.Panel1.Controls.Add(lbChapters);
            // 
            // splitContainerChVs.Panel2
            // 
            splitContainerChVs.Panel2.Controls.Add(lbVerses);
            splitContainerChVs.Size = new System.Drawing.Size(584, 426);
            splitContainerChVs.SplitterDistance = 259;
            splitContainerChVs.SplitterWidth = 5;
            splitContainerChVs.TabIndex = 0;
            // 
            // lbChapters
            // 
            lbChapters.Cursor = System.Windows.Forms.Cursors.Hand;
            lbChapters.Dock = System.Windows.Forms.DockStyle.Fill;
            lbChapters.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbChapters.FormattingEnabled = true;
            lbChapters.ItemHeight = 23;
            lbChapters.Location = new System.Drawing.Point(0, 0);
            lbChapters.Name = "lbChapters";
            lbChapters.Size = new System.Drawing.Size(259, 426);
            lbChapters.TabIndex = 0;
            lbChapters.SelectedIndexChanged += lbChapters_SelectedIndexChanged;
            // 
            // lbVerses
            // 
            lbVerses.Cursor = System.Windows.Forms.Cursors.Hand;
            lbVerses.Dock = System.Windows.Forms.DockStyle.Fill;
            lbVerses.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbVerses.FormattingEnabled = true;
            lbVerses.ItemHeight = 23;
            lbVerses.Location = new System.Drawing.Point(0, 0);
            lbVerses.Name = "lbVerses";
            lbVerses.Size = new System.Drawing.Size(320, 426);
            lbVerses.TabIndex = 0;
            lbVerses.SelectedIndexChanged += lbVerses_SelectedIndexChanged;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripPrevious, toolStripNext });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(800, 25);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripPrevious
            // 
            toolStripPrevious.Image = (System.Drawing.Image)resources.GetObject("toolStripPrevious.Image");
            toolStripPrevious.Name = "toolStripPrevious";
            toolStripPrevious.Size = new System.Drawing.Size(20, 22);
            toolStripPrevious.Click += toolStripPrevious_Click;
            // 
            // toolStripNext
            // 
            toolStripNext.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            toolStripNext.Image = (System.Drawing.Image)resources.GetObject("toolStripNext.Image");
            toolStripNext.Name = "toolStripNext";
            toolStripNext.Size = new System.Drawing.Size(20, 22);
            toolStripNext.Click += toolStripNext_Click;
            // 
            // VerseSelectionPanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 451);
            Controls.Add(splitContainerBook);
            Controls.Add(toolStrip1);
            Name = "VerseSelectionPanel";
            Text = "VerseSelectionPanel";
            Load += VerseSelectionPanel_Load;
            splitContainerBook.Panel1.ResumeLayout(false);
            splitContainerBook.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerBook).EndInit();
            splitContainerBook.ResumeLayout(false);
            splitContainerChVs.Panel1.ResumeLayout(false);
            splitContainerChVs.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerChVs).EndInit();
            splitContainerChVs.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerBook;
        private System.Windows.Forms.ListBox lbBookNames;
        private System.Windows.Forms.SplitContainer splitContainerChVs;
        private System.Windows.Forms.ListBox lbChapters;
        private System.Windows.Forms.ListBox lbVerses;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripPrevious;
        private System.Windows.Forms.ToolStripLabel toolStripNext;
    }
}