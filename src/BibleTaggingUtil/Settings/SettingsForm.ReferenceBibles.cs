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
            if(string.IsNullOrEmpty(Properties.ReferenceBibles.Default.TopReference))
            {
                cbTopReference.Text = cbTopReference.Items.Count > 0 ? cbTopReference.Items[0].ToString() : string.Empty;
            }
            else
            {
                cbTopReference.Text = Properties.ReferenceBibles.Default.TopReference;
                topRefSet = true;
            }


            if (string.IsNullOrEmpty(Properties.ReferenceBibles.Default.TAOTReference))
            {
                cbMainOT.Text = cbMainOT.Items.Count > 0 ? cbMainOT.Items[0].ToString() : string.Empty;
            }
            else
            {
                cbMainOT.Text = Properties.ReferenceBibles.Default.TAOTReference;
                mainOtSet = true;
            }

            if (string.IsNullOrEmpty(Properties.ReferenceBibles.Default.TANTReference))
            {
                cbMainNT.Text = cbMainNT.Items.Count > 0 ? cbMainNT.Items[0].ToString() : string.Empty;
            }
            else
            {
                cbMainNT.Text = Properties.ReferenceBibles.Default.TANTReference;
                mainNtSet = true;
            }

            checkbTopRefSkip.Checked = Properties.ReferenceBibles.Default.TopRefSkip;
            checkbOtRefSkip.Checked = Properties.ReferenceBibles.Default.OtRefSkip;
            checkbNtRefSkip.Checked = Properties.ReferenceBibles.Default.NtRefSkip;
            checkbNdStrong.Checked = Properties.ReferenceBibles.Default.dStrongs;
            checkBoxTopRTL.Checked = Properties.ReferenceBibles.Default.TopRightToLeft;
            checkBoxExtendedTaggingOT.Checked = Properties.ReferenceBibles.Default.ExtendedTaggingOT;
            ColumnWidthMode columnWidthMode = (ColumnWidthMode)Properties.ReferenceBibles.Default.ColumnWidthMode;
            nudColumnWidth.Value = Properties.ReferenceBibles.Default.FixedColumnWidth;
            switch (columnWidthMode)
            {
                case ColumnWidthMode.Auto:
                    rbColunnWidthAuto.Checked = true;
                    nudColumnWidth.Visible = false;
                    break;
                case ColumnWidthMode.Default:
                    rbColunnWidthDefault.Checked = true;
                    nudColumnWidth.Visible = false;
                    break;
                case ColumnWidthMode.Fixed:
                    rbColunnWidthFixed.Checked = true;
                    nudColumnWidth.Visible = true;
                    break;
            }
        }
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
                changedFlags.TopRefChanged = false;
                Properties.ReferenceBibles.Default.TopReference = cbTopReference.Text;
                topRefSet = true;
                changedFlags.TopRefChanged = true;
            }
        }

        private void cbMainOT_SelectedIndexChanged(object sender, EventArgs e)
        {
            changedFlags.MainOtChanged = false;
            if (Properties.ReferenceBibles.Default.TAOTReference != cbMainOT.Text)
            {
                Properties.ReferenceBibles.Default.TAOTReference = cbMainOT.Text;
                mainOtSet = true;
                changedFlags.MainOtChanged = true;
           }
        }

        private void cbMainNT_SelectedIndexChanged(object sender, EventArgs e)
        {
            changedFlags.MainNtChanged = false;
            if (Properties.ReferenceBibles.Default.TANTReference != cbMainNT.Text)
            {
                Properties.ReferenceBibles.Default.TANTReference = cbMainNT.Text;
                mainNtSet = true;
                changedFlags.MainNtChanged = true;
            }
        }

        private void CheckbOtRefSkip_CheckedChanged(object sender, System.EventArgs e)
        {
            Properties.ReferenceBibles.Default.OtRefSkip = checkbOtRefSkip.Checked;
            changedFlags.MainOtChanged = true;
        }

        private void checkBoxExtendedTaggingOT_CheckedChanged(object sender, EventArgs e)
        {
            Properties.ReferenceBibles.Default.ExtendedTaggingOT = checkBoxExtendedTaggingOT.Checked;
            changedFlags.MainOtChanged = true;
        }

        private void rbColunnWidthDefault_CheckedChanged(object sender, EventArgs e)
        {
            nudColumnWidth.Visible = true;
            Properties.ReferenceBibles.Default.ColumnWidthMode = (int)ColumnWidthMode.Default;
            changedFlags.MainOtChanged = true;
        }


        private void rbColunnWidthAuto_CheckedChanged(object sender, EventArgs e)
        {
            nudColumnWidth.Visible = false;
            Properties.ReferenceBibles.Default.ColumnWidthMode = (int)ColumnWidthMode.Auto;
            changedFlags.MainOtChanged = true;
        }

        private void rbColunnWidthFixed_CheckedChanged(object sender, EventArgs e)
        {
            nudColumnWidth.Visible = true;
            Properties.ReferenceBibles.Default.ColumnWidthMode = (int)ColumnWidthMode.Fixed;
            Properties.ReferenceBibles.Default.FixedColumnWidth = (int)nudColumnWidth.Value;
            changedFlags.MainOtChanged = true;
        }

        private void nudColumnWidth_ValueChanged(object sender, EventArgs e)
        {
            Properties.ReferenceBibles.Default.FixedColumnWidth = (int)nudColumnWidth.Value;
            changedFlags.MainOtChanged = true;
        }

        private void CheckbNtRefSkip_CheckedChanged(object sender, System.EventArgs e)
        {
            Properties.ReferenceBibles.Default.NtRefSkip = checkbNtRefSkip.Checked;
            changedFlags.MainNtChanged = true;
        }

        private void CheckbNdStrongs_CheckedChanged(object sender, System.EventArgs e)
        {
            Properties.ReferenceBibles.Default.dStrongs = checkbNdStrong.Checked;
            changedFlags.MainNtChanged = true;
        }

        private void CheckbTopRefSkip_CheckedChanged(object sender, System.EventArgs e)
        {
            Properties.ReferenceBibles.Default.TopRefSkip = checkbTopRefSkip.Checked;
            changedFlags.TopRefChanged = true;
        }

        private void checkBoxTopRTL_CheckedChanged(object sender, EventArgs e)
        {
            Properties.ReferenceBibles.Default.TopRightToLeft = checkBoxTopRTL.Checked;
            changedFlags.TopRefChanged = true;
        }

    }
}