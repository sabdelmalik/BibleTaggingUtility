﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace BibleTaggingUtil.Settings
{
    public partial class SettingsForm : DockContent
    {
        private string execFolder = string.Empty;
        private string topReferencesFolder = string.Empty;
        private string translatorsAmalgamatedFolder = string.Empty;

        SettingsFlags changedFlags = new SettingsFlags();
        public SettingsForm()
        {
            InitializeComponent();

            // https://learn.microsoft.com/en-us/dotnet/desktop/winforms/advanced/how-to-read-settings-at-run-time-with-csharp?view=netframeworkdesktop-4.8

            string referenceBiblePath = string.Empty;

            //referenceBiblePath = Properties.Settings.Default.ReferenceBibleFileName;
            //Properties.Settings.Default.ReferenceBibleFileName = "xyz";
            //Properties.Settings.Default.Save();

            Assembly assembly = Assembly.GetExecutingAssembly();
            execFolder = Path.GetDirectoryName(assembly.Location);
            topReferencesFolder = Path.Combine(execFolder, @"ReferenceBibles\TopReferences");
            translatorsAmalgamatedFolder = Path.Combine(execFolder, @"ReferenceBibles\TranslatorsAmalgamated");
        }


        private void SettingsForm_Load(object sender, EventArgs e)
        {
            InitializeReferencesTab();
            InitializeTranslationTagsTab();
            InitializeTargetBiblesTab();
            InitializeOsisGenerationTab();

            this.Width = 1020;
            this.Height = 758;
        }

        internal SettingsFlags ChangedFlags
        {
            get { return changedFlags; }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {

            if (topRefSet && mainOtSet && mainNtSet)
            {
                Properties.ReferenceBibles.Default.Configured = true;
            }

            Properties.MainSettings.Default.Save();

            Properties.OsisFileGeneration.Default.HebrewTagsToExclude = tbHebTags2Exclude.Text;
            Properties.OsisFileGeneration.Default.GreekTagsToExclude = tbGrkTags2Exclude.Text;
            Properties.OsisFileGeneration.Default.Save();

            Properties.ReferenceBibles.Default.Save();
            Properties.TargetBibles.Default.Save();
            Properties.TranslationTags.Default.Save();
            this.Close();
            this.DialogResult = DialogResult.OK;
        }

    }
}
