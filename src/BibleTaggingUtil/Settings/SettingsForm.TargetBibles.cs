
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
            checkBoxAncientLex.Checked = Properties.TargetBibles.Default.ShowAncientWord;
            checkBoxAncientMeaning.Checked = Properties.TargetBibles.Default.ShowAncientMeaning;
            checkBoxAncientMorph.Checked = Properties.TargetBibles.Default.ShowAncientMorphology;
            checkBoxSaveMorphology.Checked = Properties.TargetBibles.Default.SaveMorphology;
            checkBoxIndividual.Checked = Properties.TargetBibles.Default.IndividualBooks;

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
                cbVersification.Text = "KJV";



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
            StartLabel:
              List<string> bibles = new List<string>();

            string targetBiblesPath = tbTargetBiblesFolder.Text;
            if (string.IsNullOrEmpty(targetBiblesPath)) return;

            string[] biblesFolders = Directory.GetDirectories(targetBiblesPath);
            foreach (string biblePath  in biblesFolders)
            {
                if (Directory.Exists(Path.Combine(biblePath, "tagged")) &&
                    Directory.GetFiles(Path.Combine(biblePath, "tagged")).Length == 1 &&
                    File.Exists(Path.Combine(biblePath, "BiblesConfig.txt")))
                {
                    string conf = File.ReadAllText(Path.Combine(biblePath, "BiblesConfig.txt"));
                    if (conf.Contains("[Tagging]"))
                        bibles.Add(Path.GetFileName(biblePath));
                }
                else
                {
                    string temp = biblePath;
                    // Did they selected the tagged folder, in error and did it contain OldTagged
                    if (temp.EndsWith("OldTagged"))
                        temp = Path.GetDirectoryName(temp);
                    if (temp.EndsWith("tagged"))
                        temp = Path.GetDirectoryName(temp);
                    // are we at an actual target folder,
                    if(Directory.Exists(Path.Combine(temp, "tagged")) && 
                        Directory.GetFiles(Path.Combine(temp, "tagged")).Length == 1 &&
                        File.Exists(Path.Combine(temp, "BiblesConfig.txt")))
                    {
                        tbTargetBiblesFolder.Text = Path.GetDirectoryName(temp);
                        goto StartLabel;
                    }
                }
            }

            if (bibles.Count == 0)
            {
                MessageBox.Show(
$@"No valid Bible folder found at:

{targetBiblesPath}

Please Select the main Bibles folder
Bibles folder contains one or more specific Bible folder.
Each specifc Bible folder contains:
a configurations file 'BiblesConfig.txt'
and a sub folder 'tagged'",
                    "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
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
            changedFlags.TargetBibleChanged = true;
        }

        private void checkBoxTR_Byz_CheckedChanged(object sender, EventArgs e)
        {
            Properties.TargetBibles.Default.UseGrkMeaningVar = checkBoxTR_Byz.Checked;
            changedFlags.MainNtChanged= true;
        }

        private void checkBoxAncientLex_CheckedChanged(object sender, EventArgs e)
        {
            Properties.TargetBibles.Default.ShowAncientWord = checkBoxAncientLex.Checked;
            changedFlags.TargetBibleChanged = true;
        }

        private void checkBoxAncientMorph_CheckedChanged(object sender, EventArgs e)
        {
            Properties.TargetBibles.Default.ShowAncientMorphology = checkBoxAncientMorph.Checked;
            changedFlags.TargetBibleChanged = true;
        }
        private void checkBoxSaveMorphology_CheckedChanged(object sender, EventArgs e)
        {
            Properties.TargetBibles.Default.SaveMorphology = checkBoxSaveMorphology.Checked;
            changedFlags.TargetBibleChanged = true;
        }

        private void checkBoxAncientMeaning_CheckedChanged(object sender, EventArgs e)
        {
            Properties.TargetBibles.Default.ShowAncientMeaning = checkBoxAncientMeaning.Checked;
            changedFlags.TargetBibleChanged = true;
        }

        private void checkBoxIndividual_CheckedChanged(object sender, EventArgs e)
        {
            Properties.TargetBibles.Default.IndividualBooks = checkBoxIndividual.Checked;
            changedFlags.TargetBibleChanged = true;
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