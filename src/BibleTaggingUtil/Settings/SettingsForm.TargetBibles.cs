
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BibleTaggingUtil.Settings
{

    partial class SettingsForm
    {
        private string targetBiblesFolder = string.Empty;
        private string currentTarget = string.Empty;
        private bool targetBibleChanged = false;

        public event EventHandler TargetAboutToChange;
        private void InitializeTargetBiblesTab()
        {
            targetBibleChanged = false;
            targetBiblesFolder = Properties.TargetBibles.Default.TargetBiblesFolder;
            checkBoxRTL.Checked = Properties.TargetBibles.Default.RightToLeft;
            checkBoxTR_Byz.Checked = Properties.TargetBibles.Default.UseGrkMeaningVar;
            if (string.IsNullOrEmpty(targetBiblesFolder) )
            {
                tbTargetBiblesFolder.Text = string.Empty;
            }
            else
            {
                tbTargetBiblesFolder.Text = targetBiblesFolder;
                ProcessTargetBibles();
            }

            string[] versifications = { "AraSVD_sb", "KJV", "NRSV" };
            cbVersification.Items.Clear();
            cbVersification.Items.AddRange( versifications );
            if (!string.IsNullOrEmpty(Properties.TargetBibles.Default.Versification))
            {
                 cbVersification.Text = Properties.TargetBibles.Default.Versification;
            }
            else
                cbVersification.SelectedIndex = 0;



        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = targetBiblesFolderDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                tbTargetBiblesFolder.Text = targetBiblesFolderDialog.SelectedPath;
                ProcessTargetBibles();
            }
        }

        private void ProcessTargetBibles()
        {
            List<string> bibles = new List<string>();

            string targetBiblesPath = tbTargetBiblesFolder.Text;
            if (string.IsNullOrEmpty(targetBiblesPath)) return;

            string[] biblesFolders = Directory.GetDirectories(targetBiblesPath);
            foreach (string biblePath in biblesFolders)
            {
                if (Directory.Exists(Path.Combine(biblePath, "tagged")) &&
                    Directory.GetFiles(Path.Combine(biblePath, "tagged")).Length == 1 &&
                    File.Exists(Path.Combine(biblePath, "BiblesConfig.txt")))
                {
                    string conf = File.ReadAllText(Path.Combine(biblePath, "BiblesConfig.txt"));
                    if (conf.Contains("[Tagging]"))
                        bibles.Add(Path.GetFileName(biblePath));
                }
            }

            if (bibles.Count == 0)
            {
                MessageBox.Show("No valid Bible folder found at:\r\n" + targetBiblesPath);
                return;
            }
            Properties.TargetBibles.Default.TargetBiblesFolder = targetBiblesPath;
            cbTargetBibles.Items.Clear();
            cbTargetBibles.Items.AddRange(bibles.ToArray());
            currentTarget = Properties.TargetBibles.Default.TargetBible;
            if (string.IsNullOrEmpty(currentTarget))
                cbTargetBibles.SelectedIndex = 0;
            else
                cbTargetBibles.Text = currentTarget;
        }

        private void cbTargetBibles_SelectedIndexChanged(object sender, EventArgs e)
        {
            changedFlags.TargetBibleChanged = false;
            if (cbTargetBibles.Text != currentTarget)
            {
                TargetAboutToChange?.Invoke(this, EventArgs.Empty);

                currentTarget = cbTargetBibles.Text;
                Properties.TargetBibles.Default.TargetBible = currentTarget;
                changedFlags.TargetBibleChanged = true;
            }
        }

        private void checkBoxRTL_CheckedChanged(object sender, EventArgs e)
        {
            Properties.TargetBibles.Default.RightToLeft = checkBoxRTL.Checked;

        }

        private void checkBoxTR_Byz_CheckedChanged(object sender, EventArgs e)
        {
            Properties.TargetBibles.Default.UseGrkMeaningVar = checkBoxTR_Byz.Checked;
        }

        private void cbVersification_SelectedIndexChanged(object sender, EventArgs e)
        {
            changedFlags.VersificaltionChanged = false;
            string currentVersification = Properties.TargetBibles.Default.Versification;
            if (!string.IsNullOrEmpty(cbVersification.Text))
            {
                if (cbVersification.Text != currentVersification)
                {
                    Properties.TargetBibles.Default.Versification = cbVersification.Text;
                    changedFlags.VersificaltionChanged = true;
                }
            }
        }


    }
}