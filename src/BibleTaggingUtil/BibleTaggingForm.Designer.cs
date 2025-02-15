﻿
namespace BibleTaggingUtil
{
    partial class BibleTaggingForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BibleTaggingForm));
            dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            vS2013LightTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2013LightTheme();
            vS2013BlueTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2013BlueTheme();
            vS2013DarkTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2013DarkTheme();
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            reloadTargetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveUpdatedTartgetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            serachReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tAHOTEnglishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            generateSWORDFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            usfmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            generateUSFMFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            convertUSFMToOSISToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            generateSWORDFilesUsfmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            oSISToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            generateOSISToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            generateSWORDFilesOsisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            translatorsTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exportTranslatorTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            folderBrowserDialog3 = new System.Windows.Forms.FolderBrowserDialog();
            waitCursorAnimation = new System.Windows.Forms.PictureBox();
            restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)waitCursorAnimation).BeginInit();
            SuspendLayout();
            // 
            // dockPanel
            // 
            dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            dockPanel.DockBackColor = System.Drawing.Color.FromArgb(41, 57, 85);
            dockPanel.DockBottomPortion = 150D;
            dockPanel.DockLeftPortion = 200D;
            dockPanel.DockRightPortion = 200D;
            dockPanel.DockTopPortion = 150D;
            dockPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            dockPanel.Location = new System.Drawing.Point(0, 30);
            dockPanel.Name = "dockPanel";
            dockPanel.RightToLeftLayout = true;
            dockPanel.ShowAutoHideContentOnHover = false;
            dockPanel.Size = new System.Drawing.Size(1061, 519);
            dockPanel.TabIndex = 0;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, generateSWORDFilesToolStripMenuItem, usfmToolStripMenuItem, oSISToolStripMenuItem, translatorsTagsToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(6, 3, 0, 3);
            menuStrip1.Size = new System.Drawing.Size(1061, 30);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { reloadTargetToolStripMenuItem, saveUpdatedTartgetToolStripMenuItem, restoreToolStripMenuItem, settingsToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // reloadTargetToolStripMenuItem
            // 
            reloadTargetToolStripMenuItem.Name = "reloadTargetToolStripMenuItem";
            reloadTargetToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            reloadTargetToolStripMenuItem.Text = "Reload Target";
            reloadTargetToolStripMenuItem.Click += reloadTargetToolStripMenuItem_Click;
            // 
            // saveUpdatedTartgetToolStripMenuItem
            // 
            saveUpdatedTartgetToolStripMenuItem.Name = "saveUpdatedTartgetToolStripMenuItem";
            saveUpdatedTartgetToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            saveUpdatedTartgetToolStripMenuItem.Text = "Save Updatest";
            saveUpdatedTartgetToolStripMenuItem.Click += saveUpdatedTartgetToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { serachReportToolStripMenuItem, tAHOTEnglishToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new System.Drawing.Size(49, 24);
            editToolStripMenuItem.Text = "Edit";
            // 
            // serachReportToolStripMenuItem
            // 
            serachReportToolStripMenuItem.Name = "serachReportToolStripMenuItem";
            serachReportToolStripMenuItem.Size = new System.Drawing.Size(286, 26);
            serachReportToolStripMenuItem.Text = "Serach Report (Experimental)";
            serachReportToolStripMenuItem.Click += serachReportToolStripMenuItem_Click;
            // 
            // tAHOTEnglishToolStripMenuItem
            // 
            tAHOTEnglishToolStripMenuItem.Name = "tAHOTEnglishToolStripMenuItem";
            tAHOTEnglishToolStripMenuItem.Size = new System.Drawing.Size(286, 26);
            tAHOTEnglishToolStripMenuItem.Text = "TAHOT English";
            tAHOTEnglishToolStripMenuItem.Click += tAHOTEnglishToolStripMenuItem_Click;
            // 
            // generateSWORDFilesToolStripMenuItem
            // 
            generateSWORDFilesToolStripMenuItem.Name = "generateSWORDFilesToolStripMenuItem";
            generateSWORDFilesToolStripMenuItem.Size = new System.Drawing.Size(173, 24);
            generateSWORDFilesToolStripMenuItem.Text = "Generate SWORD Files";
            generateSWORDFilesToolStripMenuItem.Click += generateSWORDFilesToolStripMenuItem_Click;
            // 
            // usfmToolStripMenuItem
            // 
            usfmToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { generateUSFMFilesToolStripMenuItem, convertUSFMToOSISToolStripMenuItem, generateSWORDFilesUsfmToolStripMenuItem });
            usfmToolStripMenuItem.Name = "usfmToolStripMenuItem";
            usfmToolStripMenuItem.Size = new System.Drawing.Size(61, 24);
            usfmToolStripMenuItem.Text = "USFM";
            // 
            // generateUSFMFilesToolStripMenuItem
            // 
            generateUSFMFilesToolStripMenuItem.Name = "generateUSFMFilesToolStripMenuItem";
            generateUSFMFilesToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            generateUSFMFilesToolStripMenuItem.Text = "Generate USFM Files";
            generateUSFMFilesToolStripMenuItem.Click += generateUSFMFilesToolStripMenuItem_Click;
            // 
            // convertUSFMToOSISToolStripMenuItem
            // 
            convertUSFMToOSISToolStripMenuItem.Name = "convertUSFMToOSISToolStripMenuItem";
            convertUSFMToOSISToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            convertUSFMToOSISToolStripMenuItem.Text = "Convert USFM to OSIS";
            convertUSFMToOSISToolStripMenuItem.Click += convertUSFMToOSISToolStripMenuItem_Click;
            // 
            // generateSWORDFilesUsfmToolStripMenuItem
            // 
            generateSWORDFilesUsfmToolStripMenuItem.Name = "generateSWORDFilesUsfmToolStripMenuItem";
            generateSWORDFilesUsfmToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            generateSWORDFilesUsfmToolStripMenuItem.Text = "Generate SWORD Files";
            generateSWORDFilesUsfmToolStripMenuItem.Click += generateSWORDFilesUsfmToolStripMenuItem_Click;
            // 
            // oSISToolStripMenuItem
            // 
            oSISToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { generateOSISToolStripMenuItem, generateSWORDFilesOsisToolStripMenuItem });
            oSISToolStripMenuItem.Name = "oSISToolStripMenuItem";
            oSISToolStripMenuItem.Size = new System.Drawing.Size(54, 24);
            oSISToolStripMenuItem.Text = "OSIS";
            // 
            // generateOSISToolStripMenuItem
            // 
            generateOSISToolStripMenuItem.Name = "generateOSISToolStripMenuItem";
            generateOSISToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            generateOSISToolStripMenuItem.Text = "Generate OSIS";
            generateOSISToolStripMenuItem.Click += generateOSISToolStripMenuItem_Click;
            // 
            // generateSWORDFilesOsisToolStripMenuItem
            // 
            generateSWORDFilesOsisToolStripMenuItem.Name = "generateSWORDFilesOsisToolStripMenuItem";
            generateSWORDFilesOsisToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            generateSWORDFilesOsisToolStripMenuItem.Text = "Generate SWORD Files";
            generateSWORDFilesOsisToolStripMenuItem.Click += generateSWORDFilesOsisToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(64, 24);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // translatorsTagsToolStripMenuItem
            // 
            translatorsTagsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { exportTranslatorTagsToolStripMenuItem });
            translatorsTagsToolStripMenuItem.Name = "translatorsTagsToolStripMenuItem";
            translatorsTagsToolStripMenuItem.Size = new System.Drawing.Size(131, 24);
            translatorsTagsToolStripMenuItem.Text = "Translators Tags ";
            // 
            // exportTranslatorTagsToolStripMenuItem
            // 
            exportTranslatorTagsToolStripMenuItem.Name = "exportTranslatorTagsToolStripMenuItem";
            exportTranslatorTagsToolStripMenuItem.Size = new System.Drawing.Size(155, 26);
            exportTranslatorTagsToolStripMenuItem.Text = "Export TT";
            exportTranslatorTagsToolStripMenuItem.Click += exportTranslatorTagsToolStripMenuItem_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog1";
            // 
            // waitCursorAnimation
            // 
            waitCursorAnimation.Image = (System.Drawing.Image)resources.GetObject("waitCursorAnimation.Image");
            waitCursorAnimation.Location = new System.Drawing.Point(414, 203);
            waitCursorAnimation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            waitCursorAnimation.Name = "waitCursorAnimation";
            waitCursorAnimation.Size = new System.Drawing.Size(111, 111);
            waitCursorAnimation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            waitCursorAnimation.TabIndex = 4;
            waitCursorAnimation.TabStop = false;
            waitCursorAnimation.Visible = false;
            // 
            // restoreToolStripMenuItem
            // 
            restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            restoreToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            restoreToolStripMenuItem.Text = "Restore";
            restoreToolStripMenuItem.Click += restoreToolStripMenuItem_Click;
            // 
            // BibleTaggingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1061, 549);
            Controls.Add(waitCursorAnimation);
            Controls.Add(dockPanel);
            Controls.Add(menuStrip1);
            IsMdiContainer = true;
            MainMenuStrip = menuStrip1;
            Name = "BibleTaggingForm";
            Load += BibleTaggingForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)waitCursorAnimation).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion


        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private WeifenLuo.WinFormsUI.Docking.VS2013LightTheme vS2013LightTheme1;
        private WeifenLuo.WinFormsUI.Docking.VS2013BlueTheme vS2013BlueTheme1;
        private WeifenLuo.WinFormsUI.Docking.VS2013DarkTheme vS2013DarkTheme1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveUpdatedTartgetToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog3;
        private System.Windows.Forms.ToolStripMenuItem generateSWORDFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usfmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateUSFMFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertUSFMToOSISToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateSWORDFilesUsfmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oSISToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateOSISToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateSWORDFilesOsisToolStripMenuItem;
        private System.Windows.Forms.PictureBox waitCursorAnimation;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadTargetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem translatorsTagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportTranslatorTagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serachReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tAHOTEnglishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
    }
}

