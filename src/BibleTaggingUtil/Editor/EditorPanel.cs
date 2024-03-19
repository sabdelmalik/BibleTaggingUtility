using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using static System.Net.Mime.MediaTypeNames;

using BibleTaggingUtil.BibleVersions;
using System.Security.Cryptography.Xml;
using System.Reflection;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using BibleTaggingUtil.Strongs;
using BibleTaggingUtil.Versification;

namespace BibleTaggingUtil.Editor
{
    public partial class EditorPanel : DockContent
    {
        private BibleTaggingForm container;
        private BrowserPanel browser;
        private VerseSelectionPanel verse;

        private string strongsPrefix = string.Empty;
        private TestamentEnum testament = TestamentEnum.NEW;

        private System.Timers.Timer tempTimer = null;
        private bool osis = false;

        private FixedSizeStack<string> navStackBack = new FixedSizeStack<string>(20);
        private FixedSizeStack<string> navStackForward = new FixedSizeStack<string>(20);
        public bool TargetDirty { get; set; }


        public ReferenceVersionTAGNT TAGNT
        {
            get { return dgvTOTHT.BibleNT; }
            set { dgvTOTHT.BibleNT = value; }
        }
        public ReferenceVersionTAHOT TAHOT
        {
            get { return dgvTOTHT.BibleOT; }
            set { dgvTOTHT.BibleOT = value; }
        }
        public ReferenceTopVersion TopVersion
        {
            get { return dgvTopVersion.Bible; }
            set { dgvTopVersion.Bible = value; }
        }


        public TargetVersion TargetVersion
        {
            get { return dgvTarget.Bible; }
            set { dgvTarget.Bible = value; }
        }

        public TargetOsisVersion TargetOsisVersion
        {
            get { return dgvTarget.OsisBible; }
            set { dgvTarget.OsisBible = value; }
        }

        class VerseDetails
        {
            public VerseDetails()
            {
                OldTestament = false;
            }
            public bool OldTestament { get; set; }
            public string[] VerseWords { get; set; }
            public string[] VerseTags { get; set; }
        }

        #region Constructors
        public EditorPanel()
        {
            InitializeComponent();
            this.ControlBox = false;
            this.CloseButtonVisible = false;
            this.CloseButton = false;
        }

        public EditorPanel(BibleTaggingForm container, BrowserPanel browser, VerseSelectionPanel verse)
        {
            InitializeComponent();

            this.ControlBox = false;
            this.CloseButtonVisible = false;
            this.CloseButton = false;

            System.Drawing.Image img = picRedo.Image;
            img.RotateFlip(RotateFlipType.Rotate180FlipY);
            picRedo.Image = img;
            //picRedo.Invalidate();

            System.Drawing.Image imgPrev = picPrevVerse.Image;
            imgPrev.RotateFlip(RotateFlipType.Rotate180FlipX);
            picPrevVerse.Image = imgPrev;


            this.container = container;
            this.browser = browser;
            this.verse = verse;

        }
        #endregion Constructors


        private void EditorPanel_Load(object sender, EventArgs e)
        {
            int tbTopVersionHeight = tbTopVersion.Height;
            int dgvTopVersionHeight = dgvTopVersion.Height;

            int tbTHHeight = tbTH.Height;
            int dgvTOTHTHeight = dgvTOTHT.Height;

            int tbTargetHeight = tbTarget.Height;
            int dgvTargetHeight = dgvTarget.Height;

            int bottomPanel = 0;
            int editorHeight = this.Size.Height;

            foreach (Control c in this.Controls)
            {
                if (c is Panel)
                {
                    bottomPanel = c.Height;
                    break;
                }
            }

            tbTH.Click += TbTH_Click;
            tbTH_Previous.Click += TbTH_Previous_Click;
            tbTH_Next.Click += TbTH_Next_Click;

            dgvTopVersion.CellContentDoubleClick += Dgv_CellContentDoubleClick;
            dgvTarget.CellContentDoubleClick += Dgv_CellContentDoubleClick;
            dgvTOTHT.CellContentDoubleClick += Dgv_CellContentDoubleClick;
            //dgvTOTHT.CellContentDoubleClick += DgvTOTHTView_CellContentDoubleClick;

            dgvTarget.VerseViewChanged += DgvTarget_VerseViewChanged;
            dgvTarget.RefernceHighlightRequest += DgvTarget_RefernceHighlightRequest;
            dgvTarget.GotoVerseRequest += DgvTarget_GotoVerseRequest;

            dgvTarget.KeyDown += DgvTarget_KeyDown;
            dgvTarget.KeyUp += DgvTarget_KeyUp;
            dgvTopVersion.KeyUp += DgvTarget_KeyUp;
            dgvTOTHT.KeyUp += DgvTarget_KeyUp;

            dgvTarget.SearchTag = cbTagToFind.Text;

            verse.VerseChanged += Verse_VerseChanged;

        }

        private void DgvTarget_GotoVerseRequest(object sender, string reference)
        {
            container.VerseSelectionPanel.GotoVerse(reference);
        }

        private void DgvTarget_VerseViewChanged(object sender, EventArgs e)
        {
            TargetDirty = true;
        }


        public string CurrentVerseRef
        {
            get { return tbCurrentReference.Text; }
        }

        public void ClearCurrentVerse()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => { ClearCurrentVerse(); }));
            }
            else
            {
                tbCurrentReference.Text = string.Empty;
                dgvTarget.Rows.Clear();
                dgvTarget.ColumnCount = 0;
                dgvTopVersion.Rows.Clear();
                dgvTopVersion.ColumnCount = 0;
                dgvTOTHT.Rows.Clear();
                dgvTOTHT.ColumnCount = 0;
            }
        }

        public void SaveCurrentVerse()
        {
            if (!string.IsNullOrEmpty(tbCurrentReference.Text) && dgvTarget.Columns.Count > 0)
            {
                // save current target verse
                dgvTarget.SaveVerse(tbCurrentReference.Text);
            }
        }

        private bool firstEvent = true;
        private void Verse_VerseChanged(object sender, VerseChangedEventArgs e)
        {
            osis = Properties.MainSettings.Default.Osis;
            if (osis && firstEvent)
            {
                firstEvent = false;
                picEnableEdit.Hide();

            }

            strongsPrefix = e.StrongsPrefix;
            testament = e.Testament;

            string oldReference = tbCurrentReference.Text;
            navStackBack.Push(oldReference);
            tbCurrentReference.Text = e.VerseReference;
            dgvTarget.CurrentVerseReferece = e.VerseReference;
            string bookName = e.VerseReference.Substring(0, 3);

            string actualBookName = string.Empty;
            try
            {
                // Top Version view
                actualBookName = container.TopVersion[bookName];
                if (!string.IsNullOrEmpty(actualBookName))
                {
                    string reference = e.VerseReference.Replace(bookName, actualBookName);
                    if (container.TopVersion.Bible.ContainsKey(reference))
                        dgvTopVersion.Update(container.TopVersion.Bible[reference]);
                    else
                        dgvTopVersion.Update(null);
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }

            Verse verseWords = null;
            try
            {
                UpdateTOTHT(e.VerseReference);
                /*
                // TOTHT view
                if (testament == TestamentEnum.OLD) //  && container.TOTHT.Bible.ContainsKey(e.VerseReferenceAlt))
                {
                    actualBookName = container.TOTHT[bookName];
                    if (!string.IsNullOrEmpty(actualBookName))
                    {
                        string reference = e.VerseReference.Replace(bookName, actualBookName);
                        if (container.TOTHT.Bible.ContainsKey(reference))
                        {
                            verseWords = container.TOTHT.Bible[reference];
                            UpdateTOTHT(verseWords, BibleTestament.OT);
                        }
                        else
                            UpdateTOTHT(null, BibleTestament.OT);
                    }
                }
                else if (testament == TestamentEnum.NEW)// && container.TAGNT.Bible.ContainsKey(e.VerseReferenceUBS))
                {
                    actualBookName = container.TAGNT[bookName];
                    if (!string.IsNullOrEmpty(actualBookName))
                    {
                        string reference = e.VerseReference.Replace(bookName, actualBookName);
                        if (container.TAGNT.Bible.ContainsKey(reference))
                        {
                            verseWords = container.TAGNT.Bible[reference];
                            UpdateTOTHT(verseWords, BibleTestament.NT);
                        }
                        else
                            UpdateTOTHT(null, BibleTestament.NT);
                    }
                }
                else
                {
                    if (tbCurrentReference.Text.Contains("Not"))
                        tbCurrentReference.Text += ", " + tbTH.Text;
                    else
                        tbCurrentReference.Text += " Not in " + tbTarget.Text;
                }
                */
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }

            try
            {
                // Target view
                if (!string.IsNullOrEmpty(oldReference) && dgvTarget.Columns.Count > 0)
                {
                    if (osis)
                    {
                        // TODO handle osis save
                    }
                    else
                        dgvTarget.SaveVerse(oldReference);
                }


                actualBookName = osis ? container.OsisTarget[bookName] : container.Target[bookName];
                if (!string.IsNullOrEmpty(actualBookName))
                {
                    string reference = e.VerseReference.Replace(bookName, actualBookName);
                    //targetUpdatedVerse = Utils.GetVerseText(container.Target.Bible[targetRef], true);
                    if (tbTarget.Text.ToLower().Contains("arabic"))
                        DoSepecialHandling(reference);
                    Verse v = null;
                    if(osis)
                    {
                        if (container.OsisTarget.Bible.ContainsKey(reference))
                            v = container.OsisTarget.Bible[reference];
                    }
                    else
                    {
                        if (container.Target.Bible.ContainsKey(reference))
                            v = container.Target.Bible[reference];
                    }

                    if (dgvTarget.IsCurrentTextAramaic)
                    {
                        for (int i = 0; i < v.Count; i++)
                        {
                            VerseWord vw = v[i];
                            if (vw.Strong.Count == 1 && !(vw.Strong[0].IsEmpty) && verseWords != null)
                            {
                                string st = vw.Strong[0].ToString();
                                for (int j = 0; j < verseWords.Count; j++)
                                {
                                    if (verseWords[j].Strong.ToString().Contains(st))
                                    {
                                        if (verseWords[j].Strong.Count == 2)
                                        {
                                            vw.Strong[0] = verseWords[j].Strong[1];
                                        }
                                        break;
                                    }
                                }
                            }
                        }

                    }

                    dgvTarget.Update(v);
                }
                else
                {
                    //targetVerse = e.VerseReferenceAlt + " NotFound";
                    if (tbCurrentReference.Text.Contains("Not"))
                        tbCurrentReference.Text += ", " + tbTarget.Text;
                    else
                        tbCurrentReference.Text += " Not in " + tbTarget.Text;
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }

        }

        private void UpdateTOTHT(string verseReference) //Verse verseWords)//, BibleTestament testament)
        {
            string bk = string.Empty;
            string ch = string.Empty;
            string vs = string.Empty;

            string[] tParts = tbTH.Text.Split(" ");
            tbTH.Text = tParts[0];

            BibleTestament testament = Utils.GetTestament(verseReference);

            Match m = Regex.Match(verseReference, @"([1-9a-zA-Z]{1,5})\s([0-9]{1,3})\:([0-9]{1,3})");
            if (m.Success)
            {
                bk = m.Groups[1].Value.Trim();
                ch = m.Groups[2].Value.Trim();
                vs = m.Groups[3].Value.Trim();
            }
            int vsi = int.Parse(vs);

            Verse verseWords = null;
            string actualBookName = string.Empty;


            if (testament == BibleTestament.OT) actualBookName = container.TOTHT[bk];
            else actualBookName = container.TAGNT[bk];

            string current = string.Format("{0} {1}:{2}", actualBookName, ch, vsi);
            string next = string.Format("{0} {1}:{2}", actualBookName, ch, vsi + 1);
            string previous = string.Empty;
            if (vsi > 1) previous = string.Format("{0} {1}:{2}", actualBookName, ch, vsi - 1);

            if (testament == BibleTestament.OT)
            {
                if (container.TOTHT.Bible.ContainsKey(current))
                {
                    verseWords = container.TOTHT.Bible[current];
                }
                if (!container.TOTHT.Bible.ContainsKey(next))
                    next = string.Empty;
                if (!container.TOTHT.Bible.ContainsKey(previous))
                    previous = string.Empty;
            }
            else
            {
                if (container.TAGNT.Bible.ContainsKey(current))
                {
                    verseWords = container.TAGNT.Bible[current];
                }
                if (!container.TAGNT.Bible.ContainsKey(next))
                    next = string.Empty;
                if (!container.TAGNT.Bible.ContainsKey(previous))
                    previous = string.Empty;
            }

            tbTH.Text = tParts[0] + " " + current;
            tbTH_Next.Text = next;
            tbTH_Previous.Text = previous;

            tbTH.ForeColor = Color.White;
            tbTH_Next.ForeColor = Color.Black;
            tbTH_Previous.ForeColor = Color.Black;

            dgvTOTHT.Update(verseWords, testament);
        }

        /// <summary>
        /// Cleean Arabic text
        ///     - replace ??? and 0000 tags with an empty string
        ///     - combine Psalms header words "لإِمَامِ" and "الْمُغَنِّينَ"
        ///     - join the "يَا" with the following word
        ///     
        ///     - special handling for aligner error when the first word is "«" and is assigned a strong's number
        /// </summary>
        /// <param name="verseReference"></param>
        private void DoSepecialHandling(string verseReference)
        {
            if (osis) return;

            bool changed = false;
            Verse v = container.Target.Bible[verseReference];
            //List<int> merges = new List<int>();

            if (v[0].Word == "«" && !(v[0].Strong[0].IsEmpty))
            {
                // shift all strongs to the left one position
                for (int i = v.Count - 1; i > 0; i--)
                {
                    v[i].Strong = v[i - 1].Strong;
                }
                v[0].Strong[0] = new StrongsNumber("");
                container.Target.Bible[verseReference] = v;
            }

            while (true)
            {
                for (int i = 0; i < v.Count; i++)
                {
                    VerseWord vw = v[i];
                    /*                    if (vw.Word == "*" && vw.Strong.Count == 1 && vw.Strong[0].Contains("???"))
                                        {
                                            vw.Strong[0] = "";
                                            changed = true;
                                            break;
                                        }
                                        else if (vw.Strong.Length == 1 && vw.Strong[0].Contains("0000"))
                                        {
                                            vw.Strong[0] = "";
                                            changed = true;
                                            break;
                                        }
                                        else */
                    if (i == 0 && vw.Word == "لإِمَامِ" && v[1].Word == "الْمُغَنِّينَ")
                    {
                        v.Merge(i, 2);
                        changed = true;
                        break;
                    }
                    else if (i < v.Count - 1 && vw.Word == "يَا")
                    {
                        v.Merge(i, 2);
                        changed = true;
                        break;
                    }
                    else if (i < v.Count - 1 && vw.Strong.Count == 1 && (vw.Strong[0].IsEmpty /*|| vw.Strong[0].Contains("???")*/) &&
                        (vw.Word == "مِنَ" || vw.Word == "مِنْ" || vw.Word == "إِلَى" || vw.Word == "أَمَّا"))
                    {
                        v.Merge(i, 2);
                        changed = true;
                        break;
                    }
                    //else if (i < v.Count - 1 && vw.Strong.Length == 1 && v[i+1].Strong.Length == 1 &&
                    //                            vw.Strong[0] == v[i + 1].Strong[0] && 
                    //                            vw.Word.Replace(".","") != v[i + 1].Word.Replace(".", ""))
                    //{
                    //    v.Merge(i, 2);
                    //    changed = true;
                    //    break;
                    //}

                }
                if (changed)
                {
                    container.Target.Bible[verseReference] = v;
                    TargetDirty = true;
                    changed = false;
                }
                else
                    break;
            }
        }

        private void TempTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //            container.FindVerse(cbTagToFind.Text);
        }



        #region Reference verse events
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dgv_CellContentDoubleClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            string tag = string.Empty;
            var cellTag = dgv.Rows[dgv.RowCount - 1].Cells[e.ColumnIndex].Value;

            if (cellTag is string)
                tag = (String)cellTag;
            else
                tag = ((StrongsCluster)cellTag).ToStringS();

            if (!string.IsNullOrEmpty(tag))
            {
                tag = tag.Replace("<", "").Replace(">", "").Replace(",", "").Replace(".", "").Replace(":", "");

                string[] tags = tag.Split(' ');
                if (tags.Length == 1)
                {
                    browser.NavigateToTag(/*(testament == TestamentEnum.OLD ? "H" : "G") + */tags[0]);
                }
                else
                {

                    DataGridViewCell cell = dgv[e.ColumnIndex, e.RowIndex];
                    ContextMenuStrip cms = cell.ContextMenuStrip;
                    if (cms != null)
                    {
                        cms.ItemClicked -= Cms_ItemClicked;
                        cell.ContextMenuStrip.Dispose();
                        cell.ContextMenuStrip = null;

                    }
                    cms = new ContextMenuStrip();
                    cms.ItemClicked += Cms_ItemClicked;
                    cell.ContextMenuStrip = cms;
                    for (int i = 0; i < tags.Length; i++)
                    {
                        cell.ContextMenuStrip.Items.Add(new ToolStripMenuItem(tags[i]));

                        Rectangle r = cell.DataGridView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);
                        Point p = new Point(r.X, r.Y + r.Height);
                        cms.Show(cell.DataGridView, p);
                    }
                }
            }

        }

        private void DgvTOTHTView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            string tag = string.Empty;
            var cellTag = dgv.Rows[dgv.RowCount - 1].Cells[e.ColumnIndex].Value;

            if (cellTag is string)
                tag = (String)cellTag;
            else
                tag = ((StrongsCluster)cellTag).ToStringS();

            if (!string.IsNullOrEmpty(tag))
            {
                tag = tag.Replace("<", "").Replace(">", "").Replace(",", "").Replace(".", "").Replace(":", "");

                string[] tags = tag.Split(' ');
                if (tags.Length == 1)
                {
                    browser.NavigateToTag((testament == TestamentEnum.OLD ? "H" : "G") + tags[0]);
                }
                else
                {

                    DataGridViewCell cell = dgv[e.ColumnIndex, e.RowIndex];
                    ContextMenuStrip cms = cell.ContextMenuStrip;
                    if (cms != null)
                    {
                        cms.ItemClicked -= Cms_ItemClicked;
                        cell.ContextMenuStrip.Dispose();
                        cell.ContextMenuStrip = null;

                    }
                    cms = new ContextMenuStrip();
                    cms.ItemClicked += Cms_ItemClicked;
                    cell.ContextMenuStrip = cms;
                    for (int i = 0; i < tags.Length; i++)
                    {
                        cell.ContextMenuStrip.Items.Add(new ToolStripMenuItem(tags[i]));

                        Rectangle r = cell.DataGridView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);
                        Point p = new Point(r.X, r.Y + r.Height);
                        cms.Show(cell.DataGridView, p);
                    }
                }
            }

        }

        private void Cms_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string tag = e.ClickedItem.Text;
            browser.NavigateToTag((testament == TestamentEnum.OLD ? "H" : "G") + tag);
        }

        #endregion Reference verse events

        #region Higlight same tag
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tag"></param>
        private void DgvTarget_RefernceHighlightRequest(object sender, StrongsCluster tag, bool firstHalf)
        {
            new Thread(() =>
            {
                try
                {
                    SelectReferenceTags(tag, firstHalf);
                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                    container.HandleException(ex);
                }
            }).Start();

            new Thread(() =>
            {
                try
                {
                    SelectTargetTags(tag, firstHalf);
                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                    container.HandleException(ex);
                }
            }).Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        private void SelectReferenceTags(StrongsCluster tag, bool firstHalf)
        {
            if (InvokeRequired)
            {
                Action safeWrite = delegate { SelectReferenceTags(tag, firstHalf); };
                Invoke(safeWrite);
            }
            else
            {
                try
                {
                    dgvTopVersion.ClearSelection();
                    dgvTOTHT.ClearSelection();

                    if (string.IsNullOrEmpty(tag.ToString()))
                    {
                        SetHighlightedCell(dgvTOTHT, null, null, firstHalf);
                        SetHighlightedCell(dgvTopVersion, null, null, firstHalf);
                        return;
                    }

                    //string[] tags = tag.ToString().Split(' ');

                    string[] tags1 = tag.ToString().Split(' ');   // dStrong + position   new string[tags.Length];
                    string[] tags2 = tag.ToStringS().Split(' ');  // sStrong new string[tags.Length];

                    /*                    for (int i = 0; i < tags.Length; i++)
                                        {
                                            string t = tags[i].Replace("<", "").Replace(">", "");
                                            // remove d strong
                                            if (char.IsLetter(t[t.Length-1]))
                                                t = t.Substring(0, t.Length-1);
                                            tags1[i] = t;
                                            tags2[i] = tags[i];
                    //                        while (tags2[i][1] == '0')
                    //                            tags2[i] = tags2[i].Remove(1, 1);
                                        }
                    */
                    SetHighlightedCell(dgvTOTHT, tags1, tags2, firstHalf);
                    SetHighlightedCell(dgvTopVersion, tags1, tags2, firstHalf);

                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                }
            }
        }
        private void SelectTargetTags(StrongsCluster tag, bool firstHalf)
        {
            if (InvokeRequired)
            {
                Action safeWrite = delegate { SelectTargetTags(tag, firstHalf); };
                Invoke(safeWrite);
            }
            else
            {
                try
                {
                    if (dgvTarget.SelectedCells.Count == 1)
                    {
                        if (string.IsNullOrEmpty(tag.ToString()))
                        {
                            SetHighlightedCell(dgvTarget, null, null, firstHalf);
                            return;
                        }

                        //string[] tags = tag.ToString().Split(' ');
                        string[] tags1 = tag.ToString().Split(' ');   // dStrong + position   new string[tags.Length];
                        string[] tags2 = tag.ToStringS().Split(' ');  // sStrong new string[tags.Length];
                        /*                        for (int i = 0; i < tags.Length; i++)
                                                {
                                                    string t = tags[i].Replace("<", "").Replace(">", "");
                                                    // remove d strong
                                                    if (char.IsLetter(t[t.Length - 1]))
                                                        t = t.Substring(0, t.Length - 1);
                                                    tags1[i] = t;
                                                    tags2[i] = tags[i];
                                                    //while (tags2[i][1] == '0')
                                                        //tags2[i] = tags2[i].Remove(1, 1);
                                                }
                        */
                        SetHighlightedCell(dgvTarget, tags1, tags2, firstHalf);
                    }
                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                }
            }
        }

        private void SetHighlightedCell(DataGridView dgv, string[] tags1, string[] tags2, bool firstHalf)
        {
            int count = dgv.ColumnCount;
            int tagsRow = dgv.RowCount - 1;
            //List<int> cells = new List<int>();

            int selectedCellColumn = -1;
            if (dgv == dgvTarget)
                selectedCellColumn = dgvTarget.SelectedCells[0].ColumnIndex;

            List<int> cells = new List<int>();
            // first try with dStrong + position
            if (!checkBsStrongHighlight.Checked)  // checked if we need to use only sStrong
            {
                cells = GetCellsToHighlight(dgv, tags1);
            }
            if (cells.Count == 0)
            {
                // Now try with sStrong
                cells = GetCellsToHighlight(dgv, tags2);
            }

            if (cells.Count == 1)
            {
                if (dgv != dgvTarget)
                    dgv.CurrentCell = dgv.Rows[tagsRow].Cells[cells[0]];
                dgv.Rows[tagsRow].Cells[cells[0]].Style.BackColor = Color.Yellow;
            }
            else if (cells.Count != 0)
            {
                for (int i = 0; i < cells.Count; i++)
                {
                    dgv.Rows[tagsRow].Cells[cells[i]].Style.BackColor = Color.Yellow;
                }
                if (dgv != dgvTarget)
                    if (firstHalf)
                        dgv.CurrentCell = dgv.Rows[tagsRow].Cells[cells[0]];
                    else
                        dgv.CurrentCell = dgv.Rows[tagsRow].Cells[cells[cells.Count - 1]];
            }
            if (dgv != dgvTarget)
                dgv.ClearSelection();
        }

        private List<int> GetCellsToHighlight(DataGridView dgv, string[] tags)
        {
            List<int> cells = new List<int>();
            int count = dgv.ColumnCount;
            int tagsRow = dgv.RowCount - 1;
            int selectedCellColumn = -1;
            if (dgv == dgvTarget)
                selectedCellColumn = dgvTarget.SelectedCells[0].ColumnIndex;

            for (int i = 0; i < count; i++)
            {
                //                if (i == selectedCellColumn)
                //                    continue; // skip target selected cell

                dgv.Rows[tagsRow].Cells[i].Style.BackColor = Color.White;
                if (dgv != dgvTarget)
                    dgv.Rows[tagsRow].Cells[i].Selected = false;

                if (tags == null)
                    continue;
                StrongsCluster refTag = (StrongsCluster)dgv.Rows[tagsRow].Cells[i].Value;

                if (refTag != null)
                {

                    for (int j = 0; j < tags.Length; j++)
                    {
                        if (refTag.ToString().Contains(tags[j]))
                        {
                            cells.Add(i);
                        }
                    }
                }
            }
            return cells;
        }


        #endregion Higlight same tag


        private void DgvTarget_KeyDown(object sender, KeyEventArgs e)
        {
            Keys forward = Keys.Right;
            Keys back = Keys.Left;
            if (tbTarget.Text.ToLower().Contains("arabic"))
            {
                forward = Keys.Left;
                back = Keys.Right;
            }
            if (e.KeyCode == forward)
            {
                if (dgvTarget.IsLastWord)
                {
                    verse.MoveToNext();
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == back)
            {
                if (dgvTarget.IsFirstWord)
                {
                    verse.MoveToPrevious();
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.F3)
            {
                // ignore F3 - we don't want to sort
                e.Handled = true;
            }
        }

        private void DgvTarget_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageUp)
            {
                verse.MoveToPrevious();
            }
            else if (e.KeyCode == Keys.PageDown || e.KeyCode == Keys.Next)
            {
                verse.MoveToNext();
            }
            else if (e.Control)
            {
                if (e.KeyCode == Keys.S)
                {
                    if (osis)
                        container.OsisTarget.Save("");
                    else
                        container.Target.SaveUpdates();
                }
                if (e.KeyCode == Keys.Y) dgvTarget.Redo();
                if (e.KeyCode == Keys.Z) dgvTarget.Undo();
            }
            else if (e.KeyCode == Keys.F3)
            {
                // ignore F3 - we don't want to sort
                e.Handled = true;
            }
        }

        #region Buttons
        private void picPrevVerse_Click(object sender, EventArgs e)
        {
            verse.MoveToPrevious();
        }

        private void picNextVerse_Click(object sender, EventArgs e)
        {
            verse.MoveToNext();
        }

        private void picRedo_Click(object sender, EventArgs e)
        {
            dgvTarget.Redo();
        }

        private void picUndo_Click(object sender, EventArgs e)
        {
            dgvTarget.Undo();
        }

        private void picSave_Click(object sender, EventArgs e)
        {
            if (osis)
                container.OsisTarget.Save("");
            else
                container.Target.SaveUpdates();
        }

        private void picDecreaseFont_Click(object sender, EventArgs e)
        {
            Font font = dgvTopVersion.DefaultCellStyle.Font;
            dgvTopVersion.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);
            font = dgvTopVersion.DefaultCellStyle.Font;
            dgvTarget.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);
            font = dgvTOTHT.DefaultCellStyle.Font;
            dgvTOTHT.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);

        }
        private void picIncreaseFont_Click(object sender, EventArgs e)
        {
            Font font = dgvTopVersion.DefaultCellStyle.Font;
            dgvTopVersion.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
            font = dgvTopVersion.DefaultCellStyle.Font;
            dgvTarget.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
            font = dgvTOTHT.DefaultCellStyle.Font;
            dgvTOTHT.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
        }

        private void picEnableEdit_Click(object sender, EventArgs e)
        {
            dgvTarget.Rows[0].ReadOnly = false;
        }

        private void picFindTagForward_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                container.FindRepetitive();
            }
            else
            {
                dgvTarget.SearchTag = cbTagToFind.Text;
                container.FindVerse(cbTagToFind.Text);
            }
        }


        #endregion Buttons

        private void tbCurrentReference_TextChanged(object sender, EventArgs e)
        {
            dgvTarget.CurrentVerseReferece = tbCurrentReference.Text;
            int b = tbCurrentReference.Text.IndexOf(' ');
            if (b > 0)
            {
                string book = tbCurrentReference.Text.Substring(0, b);
                int bookIndex = Array.IndexOf(Constants.osisNames, book);
                if (bookIndex < 0)
                {
                    bookIndex = Array.IndexOf(Constants.osisAltNames, book);
                    if (bookIndex < 0)
                        bookIndex = Array.IndexOf(Constants.ubsNames, book);
                }
                if (bookIndex >= 0)
                {
                    if (bookIndex > 38)
                        tbTH.Text = container.TAGNT.BibleName;
                    else
                        tbTH.Text = container.TOTHT.BibleName;
                }
            }
        }

        public string TargetBibleName
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => { TargetBibleName = value; }));
                }
                else
                {
                    tbTarget.Text = value;
                    //if(tbTarget.Text.ToLower().Contains("arabic"))
                    //    dgvTarget.RightToLeft = RightToLeft.Yes;
                    //else 
                    //    tbTarget.RightToLeft = RightToLeft.No;
                }
            }
        }


        public string TopReferenceBibleName
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => { TopReferenceBibleName = value; }));
                }
                else
                {
                    tbTopVersion.Text = value;
                }
            }
        }
        private void cbTagToFind_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvTarget.SearchTag = cbTagToFind.Text;
        }

        private void picRefresh_Click(object sender, EventArgs e)
        {
            int savedColumn = dgvTarget.CurrentCell.ColumnIndex;
            int savedRow = dgvTarget.CurrentCell.RowIndex;

            string reference = tbCurrentReference.Text;
            dgvTarget.SaveVerse(reference);
            Verse v = osis ? container.OsisTarget.Bible[reference] : container.Target.Bible[reference];
            dgvTarget.Update(v);

            dgvTarget.CurrentCell = dgvTarget[savedColumn, savedRow];
        }

        internal void EnableSaveButton(bool v)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => { EnableSaveButton(v); }));
            }
            else
            {
                picSave.Visible = v;
            }

        }

        private void TbTH_Next_Click(object sender, EventArgs e)
        {
            UpdateThSpecial(sender);
        }

        private void TbTH_Previous_Click(object sender, EventArgs e)
        {
            UpdateThSpecial(sender);
        }

        private void TbTH_Click(object sender, EventArgs e)
        {
            UpdateThSpecial(sender);
        }

        private void UpdateThSpecial(object sender)
        {
            string reference = ((TextBox)sender).Text;
            if (string.IsNullOrEmpty(reference))
                return;

            tbTH.ForeColor = Color.Black;
            tbTH_Next.ForeColor = Color.Black;
            tbTH_Previous.ForeColor = Color.Black;
            if (sender == tbTH)
            {
                tbTH.ForeColor = Color.White;
                int idx = reference.IndexOf(' ');
                if (idx < 0) reference = string.Empty;
                else reference = reference.Substring(idx + 1).Trim();
            }
            if (sender == tbTH_Next)
            {
                tbTH_Next.ForeColor = Color.White;
            }
            if (sender == tbTH_Previous)
            {
                tbTH_Previous.ForeColor = Color.White;
            }

            BibleTestament testament = Utils.GetTestament(reference);
            if (testament == BibleTestament.OT)
            {
                if (container.TOTHT.Bible.ContainsKey(reference))
                    dgvTOTHT.Update(container.TOTHT.Bible[reference], testament);
                else
                    dgvTOTHT.Update(null, testament);
            }
            else
            {
                if (container.TAGNT.Bible.ContainsKey(reference))
                    dgvTOTHT.Update(container.TAGNT.Bible[reference], testament);
                else
                    dgvTOTHT.Update(null, testament);
            }
        }

        private void checkBsStrongHighlight_CheckedChanged(object sender, EventArgs e)
        {
            if (dgvTarget.SelectedCells.Count == 1)
            {
                try
                {
                    dgvTarget.FireRefernceHighlightRequest((StrongsCluster)dgvTarget.SelectedCells[0].Value);
                }
                catch { }
            }
        }

        private void picBack_Click(object sender, EventArgs e)
        {
            if (navStackBack.Count > 0)
            {
                string currentReference = tbCurrentReference.Text;
                navStackForward.Push(currentReference);

                string prevRef =  navStackBack.Pop();
                verse.GotoVerse(prevRef);
            }

        }

        private void picForward_Click(object sender, EventArgs e)
        {
            if (navStackForward.Count > 0)
            {
                string currentReference = tbCurrentReference.Text;
                navStackBack.Push(currentReference);

                string prevRef = navStackForward.Pop();
                verse.GotoVerse(prevRef);
            }
        }
    }
}
