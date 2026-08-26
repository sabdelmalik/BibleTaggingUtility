using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil
{
    public partial class TaggedFileSelectionForm : Form
    {
        string[] files;
        public TaggedFileSelectionForm()
        {
            InitializeComponent();
        }

        public TaggedFileSelectionForm(string[] files)
        {
            InitializeComponent();

            this.files = files;
            List<string> fileNames = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                fileNames.Add(Path.GetFileName(files[i]));
            }

            listView1.Columns[0].Width = listView1.Width;
            var newRows = new List<ListViewItem>();
            for (int i = 0; i < fileNames.Count; i++)
            {
                newRows.Add(new ListViewItem($"{fileNames[i]}"));
            }
            listView1.Items.AddRange(newRows.ToArray());

            listView1.MouseDoubleClick += ListView1_MouseDoubleClick;
        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 1. Get the item that was located at the double-click coordinates
            ListViewItem clickedItem = listView1.GetItemAt(e.X, e.Y);

            // 2. Verify that the user clicked a real item, not empty space
            if (clickedItem != null)
            {
                string text = clickedItem.Text;
                int selectedIndex = clickedItem.Index;
                SelctedFile = files[selectedIndex];
            }
            this.DialogResult = DialogResult.OK;

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                int selectedIndex = listView1.SelectedIndices[0];
                SelctedFile = files[selectedIndex];
            }

        }

        public string SelctedFile { get; private set; }
    }
}
