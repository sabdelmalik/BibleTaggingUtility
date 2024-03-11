/*********************************************************
*
*Reference Bibles
*
*
*/
using System;
using System.Collections.Generic;
using System.IO;

namespace BibleTaggingUtil.Settings
{
    partial class SettingsForm
    {
        private string topReferencePath = string.Empty;
        private string tagntPath = string.Empty;
        private string tothtPath = string.Empty;

        private bool topRefSet = false;
        private bool mainNtSet = false;
        private bool mainOtSet = false;

        private void InitializeReferencesTab()
        {
                      TopRefChanged = false;
          MainNtChanged = false;
          MainOtChanged = false;

        List<string> folders = new List<string>();
            foreach(string folder in Directory.GetDirectories(topReferencesFolder))
            {
                folders.Add(Path.GetFileName(folder));
            }
            cbTopReference.Items.Clear();
            cbTopReference.Items.AddRange(folders.ToArray());

            folders.Clear();
            foreach (string folder in Directory.GetDirectories(Path.Combine(translatorsAmalgamatedFolder, "OT")))
            {
                folders.Add(Path.GetFileName(folder));
            }
            cbMainOT.Items.Clear();
            cbMainOT.Items.AddRange(folders.ToArray());

            folders.Clear();
            foreach (string folder in Directory.GetDirectories(Path.Combine(translatorsAmalgamatedFolder, "NT")))
            {
                folders.Add(Path.GetFileName(folder));
            }
            cbMainNT.Items.Clear();
            cbMainNT.Items.AddRange(folders.ToArray());
        
            // get current sttings
            if(!string.IsNullOrEmpty(Properties.ReferenceBibles.Default.TopReference))
            {
                cbTopReference.Text = Properties.ReferenceBibles.Default.TopReference;
                topRefSet = true;
            }
            if (!string.IsNullOrEmpty(Properties.ReferenceBibles.Default.TAOTReference))
            {
                cbMainOT.Text = Properties.ReferenceBibles.Default.TAOTReference;
                mainOtSet = true;
            }
            if (!string.IsNullOrEmpty(Properties.ReferenceBibles.Default.TANTReference))
            {
                cbMainNT.Text = Properties.ReferenceBibles.Default.TANTReference;
                mainNtSet = true;
            }

            checkbTopRefSkip.Checked = Properties.ReferenceBibles.Default.TopRefSkip;
            checkbOtRefSkip.Checked = Properties.ReferenceBibles.Default.OtRefSkip;
            checkbNtRefSkip.Checked = Properties.ReferenceBibles.Default.NtRefSkip;
        }

        public bool TopRefChanged { get; private set; }
        public bool MainNtChanged { get; private set; }
        public bool MainOtChanged { get; private set; }

        public string ReferenceTopVersionPath
        {
            get
            {
                string name = Properties.ReferenceBibles.Default.TopReference;
                return Path.Combine(topReferencesFolder, name);
            }
            internal set
            {

            }
        }
        public string ReferenceTAOTPath
        {
            get
            {
                string name = Properties.ReferenceBibles.Default.TAOTReference;
                return Path.Combine(translatorsAmalgamatedFolder, @"OT\" + name);
            }
            internal set
            {

            }
        }

        public string ReferenceTANTPath
        {
            get
            {
                string name = Properties.ReferenceBibles.Default.TANTReference;
                return Path.Combine(translatorsAmalgamatedFolder, @"NT\" + name);
            }
            internal set
            {

            }
        }

        private void cbTopReference_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Properties.ReferenceBibles.Default.TopReference != cbTopReference.Text)
            {
                Properties.ReferenceBibles.Default.TopReference = cbTopReference.Text;
                topRefSet = true;
                TopRefChanged = true;
            }
        }

        private void cbMainOT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Properties.ReferenceBibles.Default.TAOTReference != cbMainOT.Text)
            {
                Properties.ReferenceBibles.Default.TAOTReference = cbMainOT.Text;
                mainOtSet = true;
                MainOtChanged = true;
           }
        }

        private void cbMainNT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Properties.ReferenceBibles.Default.TANTReference != cbMainNT.Text)
            {
                Properties.ReferenceBibles.Default.TANTReference = cbMainNT.Text;
                mainNtSet = true;
                MainNtChanged = true;
            }
        }

        private void CheckbOtRefSkip_CheckedChanged(object sender, System.EventArgs e)
        {
            Properties.ReferenceBibles.Default.OtRefSkip = checkbOtRefSkip.Checked;
        }

        private void CheckbNtRefSkip_CheckedChanged(object sender, System.EventArgs e)
        {
            Properties.ReferenceBibles.Default.NtRefSkip = checkbNtRefSkip.Checked;
        }

        private void CheckbTopRefSkip_CheckedChanged(object sender, System.EventArgs e)
        {
            Properties.ReferenceBibles.Default.TopRefSkip = checkbTopRefSkip.Checked;
        }


    }
}