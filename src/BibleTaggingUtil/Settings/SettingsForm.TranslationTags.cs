

using System;
using System.IO;
using System.Windows.Forms;


namespace BibleTaggingUtil.Settings
{
    partial class SettingsForm
    {

        private void InitializeTranslationTagsTab()
        {
            tbTtFolder.Text = Properties.TranslationTags.Default.TranslationTagsFolder;
            tbForReviewFileName.Text = Properties.TranslationTags.Default.ForReviewFileName;
            tbMissingWordsFileName.Text = Properties.TranslationTags.Default.MissingWordsFileName;
            tbErrorsFileName.Text = Properties.TranslationTags.Default.ErrorsFileName;
            tbOutputFileName.Text = Properties.TranslationTags.Default.OutputFileName;
            tbRepeatedWord.Text = Properties.TranslationTags.Default.RepeatedWordFileName;
            if (string.IsNullOrEmpty(Properties.TranslationTags.Default.Version))
                tbVersion.Text = "0.0.0";
            else
                tbVersion.Text = Properties.TranslationTags.Default.Version;

            checkbAppendTimestamp.Checked = Properties.TranslationTags.Default.AppendTimestamp;
            checkbFilesPerBook.Checked = Properties.TranslationTags.Default.FilesPerBook;
            checkbPublicDomain.Checked = Properties.TranslationTags.Default.PublicDomain;
        }


        private void TbVersion_TextChanged(object sender, System.EventArgs e)
        {
            Properties.TranslationTags.Default.Version = tbVersion.Text;
        }

        private void btnTTFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                tbTtFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void tbTtFolder_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(tbTtFolder.Text))
            {
                Properties.TranslationTags.Default.TranslationTagsFolder = tbTtFolder.Text;
            }
        }

        private void TbForReviewFileName_TextChanged(object sender, System.EventArgs e)
        {
            Properties.TranslationTags.Default.ForReviewFileName = tbForReviewFileName.Text;
        }

        private void TbMissingWordsFileName_TextChanged(object sender, System.EventArgs e)
        {
            Properties.TranslationTags.Default.MissingWordsFileName = tbMissingWordsFileName.Text;
        }

        private void TbErrorsFileName_TextChanged(object sender, System.EventArgs e)
        {
            Properties.TranslationTags.Default.ErrorsFileName = tbErrorsFileName.Text;
        }

        private void TbOutputFileName_TextChanged(object sender, System.EventArgs e)
        {
            Properties.TranslationTags.Default.OutputFileName = tbOutputFileName.Text;
        }

        private void TbRepeatedWord_TextChanged(object sender, System.EventArgs e)
        {
            Properties.TranslationTags.Default.RepeatedWordFileName = tbRepeatedWord.Text;
        }


        private void checkbFilesPerBook_CheckedChanged(object sender, EventArgs e)
        {
            Properties.TranslationTags.Default.FilesPerBook = checkbFilesPerBook.Checked;
        }

        private void checkbAppendTimestamp_CheckedChanged(object sender, EventArgs e)
        {
            Properties.TranslationTags.Default.AppendTimestamp = checkbAppendTimestamp.Checked;
        }

        private void checkbPublicDomain_CheckedChanged(object sender, EventArgs e)
        {
            Properties.TranslationTags.Default.PublicDomain = checkbPublicDomain.Checked;
        }


    }
}