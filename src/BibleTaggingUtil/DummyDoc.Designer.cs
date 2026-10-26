namespace BibleTaggingUtil
{
    partial class DummyDoc
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DummyDoc));
            mainMenu = new System.Windows.Forms.MenuStrip();
            menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            menuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            menuItemCheckTest = new System.Windows.Forms.ToolStripMenuItem();
            contextMenuTabPage = new System.Windows.Forms.ContextMenuStrip(components);
            menuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            menuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            menuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            toolTip = new System.Windows.Forms.ToolTip(components);
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            mainMenu.SuspendLayout();
            contextMenuTabPage.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenu
            // 
            mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { menuItem1 });
            mainMenu.Location = new System.Drawing.Point(0, 4);
            mainMenu.Name = "mainMenu";
            mainMenu.Size = new System.Drawing.Size(448, 24);
            mainMenu.TabIndex = 1;
            mainMenu.Visible = false;
            // 
            // menuItem1
            // 
            menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuItem2, menuItemCheckTest });
            menuItem1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            menuItem1.MergeIndex = 1;
            menuItem1.Name = "menuItem1";
            menuItem1.Size = new System.Drawing.Size(150, 20);
            menuItem1.Text = "&MDI Document";
            // 
            // menuItem2
            // 
            menuItem2.Name = "menuItem2";
            menuItem2.Size = new System.Drawing.Size(196, 34);
            menuItem2.Text = "Test";
            menuItem2.Click += menuItem2_Click;
            // 
            // menuItemCheckTest
            // 
            menuItemCheckTest.Name = "menuItemCheckTest";
            menuItemCheckTest.Size = new System.Drawing.Size(196, 34);
            menuItemCheckTest.Text = "Check Test";
            menuItemCheckTest.Click += menuItemCheckTest_Click;
            // 
            // contextMenuTabPage
            // 
            contextMenuTabPage.ImageScalingSize = new System.Drawing.Size(24, 24);
            contextMenuTabPage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { menuItem3, menuItem4, menuItem5 });
            contextMenuTabPage.Name = "contextMenuTabPage";
            contextMenuTabPage.Size = new System.Drawing.Size(241, 133);
            // 
            // menuItem3
            // 
            menuItem3.Name = "menuItem3";
            menuItem3.Size = new System.Drawing.Size(240, 32);
            menuItem3.Text = "Option &1";
            // 
            // menuItem4
            // 
            menuItem4.Name = "menuItem4";
            menuItem4.Size = new System.Drawing.Size(240, 32);
            menuItem4.Text = "Option &2";
            // 
            // menuItem5
            // 
            menuItem5.Name = "menuItem5";
            menuItem5.Size = new System.Drawing.Size(240, 32);
            menuItem5.Text = "Option &3";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new System.Drawing.Point(0, 89);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new System.Drawing.Size(448, 304);
            richTextBox1.TabIndex = 2;
            richTextBox1.Text = "";
            // 
            // DummyDoc
            // 
            ClientSize = new System.Drawing.Size(448, 393);
            Controls.Add(richTextBox1);
            Controls.Add(mainMenu);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = mainMenu;
            Name = "DummyDoc";
            Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            TabPageContextMenuStrip = contextMenuTabPage;
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            contextMenuTabPage.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }
        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuItem2;
        private System.Windows.Forms.ContextMenuStrip contextMenuTabPage;
        private System.Windows.Forms.ToolStripMenuItem menuItem3;
        private System.Windows.Forms.ToolStripMenuItem menuItem4;
        private System.Windows.Forms.ToolStripMenuItem menuItem5;
        private System.Windows.Forms.ToolStripMenuItem menuItemCheckTest;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}