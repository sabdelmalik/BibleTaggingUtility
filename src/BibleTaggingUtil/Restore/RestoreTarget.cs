using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BibleTaggingUtil.Restore
{
    public partial class RestoreTarget : Form
    {
        private string taggedFolder = string.Empty;
        private string oldTaggedFolder = string.Empty;
        public RestoreTarget()
        {
            InitializeComponent();
        }

        private void RestoreTarget_Load(object sender, EventArgs e)
        {
            string bibleName = Properties.TargetBibles.Default.TargetBible;

            this.Text = string.Format("{0}: Restore from old tagged files", bibleName);
            string targetBibleFolder = Path.Combine(Properties.TargetBibles.Default.TargetBiblesFolder,
                            bibleName);
            taggedFolder = Path.Combine(targetBibleFolder, "tagged");
            oldTaggedFolder = Path.Combine(taggedFolder, "OldTagged");

            string[] files = Directory.GetFiles(oldTaggedFolder);
            List<string[]> fileEntries = new List<string[]>();
            foreach (string file in files)
            {
                //niv-th_osis_2023_08_30_23_05.txt
                string fileName = Path.GetFileName(file);
                string datetime = string.Empty;
                Match m = Regex.Match(fileName, @"[_]([0-9]{4})[_]([0-9]{2})[_]([0-9]{2})[_]([0-9]{2})[_]([0-9]{2})");
                if (m.Success)
                {
                    datetime = string.Format("{0}-{1}-{2} {3}:{4}",
                                m.Groups[1].Value,
                                m.Groups[2].Value,
                                m.Groups[3].Value,
                                m.Groups[4].Value,
                                m.Groups[5].Value);
                }
                if (string.IsNullOrEmpty(datetime))
                    datetime = "No Timestamp";

                ListViewItem fileItem = new ListViewItem(datetime);
                fileItem.SubItems.Add(file);

                fileEntries.Add(new string[] { datetime, fileName });
            }
            foreach (string[] line in fileEntries)
                dgvTaggedFiles.Rows.Add(line);

            dgvTaggedFiles.Sort(dgvTaggedFiles.Columns[0], ListSortDirection.Descending);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string selectedFile = (string)dgvTaggedFiles.SelectedRows[0].Cells[1].Value;
            string currentFilePath = Directory.GetFiles(taggedFolder)[0];
            string currentFile = Path.GetFileName(currentFilePath);
            if (selectedFile == currentFile)
                return;

            string msg = string.Format("Are you sure?\r\nIf you click Yes,\r\n" +
                "CurrentFile '{0}' will be moved to Old Tagged,\r\n" +
                "'{1}' will become the current tagged file.", currentFile, selectedFile);

            DialogResult result = MessageBox.Show(msg, "Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            if (result == DialogResult.Yes)
            {
                DoRestore(selectedFile);
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
        
        private void DoRestore(string selectedFile)
        {
            // move existing tagged files to the old folder
            String[] existingTagged = Directory.GetFiles(taggedFolder, "*.*");
            foreach (String existingTaggedItem in existingTagged)
            {
                string fName = Path.GetFileName(existingTaggedItem);
                string src = Path.Combine(taggedFolder, fName);
                string dst = Path.Combine(oldTaggedFolder, fName);
                if (System.IO.File.Exists(dst))
                    System.IO.File.Delete(src);
                else
                    System.IO.File.Move(src, dst);
            }

            string src1 = Path.Combine(oldTaggedFolder, selectedFile);
            string dst1 = Path.Combine(taggedFolder, selectedFile);
            System.IO.File.Move(src1, dst1);
         }

    }
}
