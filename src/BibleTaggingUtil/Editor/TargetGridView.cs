using BibleTaggingUtil.BibleVersions;
using BibleTaggingUtil.Strongs;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static WeifenLuo.WinFormsUI.Docking.DockPanel;

namespace BibleTaggingUtil.Editor
{
    public class TargetGridView : DataGridView
    {
        public event VerseViewChangedEventHandler VerseViewChanged;
        public event RefernceHighlightRequestEventHandler RefernceHighlightRequest;
        public event GotoVerseRequestEventHandler GotoVerseRequest;

        private FixedSizeStack<VerseEx> undoStack = new FixedSizeStack<VerseEx>(30);
        private FixedSizeStack<VerseEx> redoStack = new FixedSizeStack<VerseEx>(30);

        public TargetGridView()
        {
            // Enable double buffering
            this.DoubleBuffered = true;

            // Force control to redraw when resized and reduce background flickering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);

            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            this.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
        }

        public string CurrentVerseReferece { get; set; }

        public string SearchTag { get; set; }

        public Verse CurrentVerse { get; set; }

        public TargetVersion Bible { get; set; }
        public TargetOsisVersion OsisBible { get; internal set; }

        public bool IsLastWord
        {
            get
            {
                if (this.SelectedCells.Count == 1 &&
                    this.SelectedCells[0].ColumnIndex == Columns.Count - 1)
                    return true;
                else
                    return false;
            }
        }

        public bool IsFirstWord
        {
            get
            {
                if (this.SelectedCells.Count == 1 &&
                    this.SelectedCells[0].ColumnIndex == 0)
                    return true;
                else
                    return false;
            }
        }

        public bool IsCurrentTextAramaic
        {
            get
            {
                bool result = Utils.IsReferenceAramaic(CurrentVerseReferece);

                //string referencePattern = @"^([0-9A-Za-z]+)\s([0-9]+):([0-9]+)";
                //Match mTx = Regex.Match(CurrentVerseReferece, referencePattern);
                //if (!mTx.Success)
                //{
                //    Tracing.TraceError(MethodBase.GetCurrentMethod().Name, "Incorrect reference format: " + CurrentVerseReferece);
                //    return result;
                //}

                //String book = mTx.Groups[1].Value;
                //string chapter = mTx.Groups[2].Value;
                //string verse = mTx.Groups[3].Value;

                //int ch = 0;
                //int vs = 0;
                //if (!int.TryParse(chapter, out ch))
                //    return result;
                //if (!int.TryParse(verse, out vs))
                //    return result;
                //if ((book == "Gen" && ch == 31 && vs == 47) ||
                //    (book == "Ezr" && ((ch == 4 && vs >= 8) || (ch == 5) || (ch == 6 && vs <= 18))) ||
                //    (book == "Ezr" && (ch >= 7 && vs >= 12 && vs <= 26)) ||
                //    (book == "Pro" && ch == 31 && vs == 2) ||
                //    (book == "Jer" && ch == 10 && vs == 11) ||
                //    (book == "Dan" && ((ch == 2 && vs >= 4) || (ch > 2 && ch < 7) || (ch == 7 && vs <= 28))))
                //    result = true;

                return result;
            }
        }


        #region Context Menue

        private const string MERGE_CONTEXT_MENU = "Merge";
        private const string SWAP_CONTEXT_MENU = "Swap Tags";
        private const string SPLIT_CONTEXT_MENU = "Split";
        private const string DELETE_CONTEXT_MENU = "Delete Tag";
        private const string REVERSE_CONTEXT_MENU = "Reverse Tags";
        private const string DELETE_LEFT_CONTEXT_MENU = "Delete Left Tags";
        private const string DELETE_RIGHT_CONTEXT_MENU = "Delete Right Tags";

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (this.SelectedCells.Count == 0)
                return;

            if (Properties.MainSettings.Default.Osis)
                return;

            if (this.SelectedCells.Count > 1)
            {
                bool sameRow = true;
                foreach (DataGridViewCell cell in this.SelectedCells)
                {
                    // we only merge in the top row
                    if (cell.RowIndex != 0)
                    {
                        sameRow = false;
                        break;
                    }
                }

                bool mergeOk = true; // we only merge adjacent cells
                int colIndex = SelectedCells[0].ColumnIndex;
                for (int i = 1; i < this.SelectedCells.Count; i++)
                {
                    if (Math.Abs(SelectedCells[i].ColumnIndex - colIndex) != 1)
                    {
                        mergeOk = false;
                        break;
                    }
                    colIndex = SelectedCells[i].ColumnIndex;
                }


                if (sameRow)
                {
                    this.ContextMenuStrip.Items.Clear();
                    ToolStripMenuItem mergeMenuItem = new ToolStripMenuItem(MERGE_CONTEXT_MENU);
                    ToolStripMenuItem swapMenuItem = new ToolStripMenuItem(SWAP_CONTEXT_MENU);

                    if (mergeOk)
                        this.ContextMenuStrip.Items.Add(mergeMenuItem);
                    if (this.SelectedCells.Count == 2)
                        this.ContextMenuStrip.Items.Add(swapMenuItem);
                    e.Cancel = false;
                }
            }
            else
            {
                if (this.SelectedCells[0].RowIndex == Rows.Count - 1)
                {
                    StrongsCluster tag = (StrongsCluster)this.SelectedCells[0].Value;

                    if (tag == null)
                        return;
                    //                    if (string.IsNullOrEmpty(text))
                    //                        return;

                    this.ContextMenuStrip.Items.Clear();

                    this.ContextMenuStrip.Items.Clear();

                    string[] strings = tag.ToString().Split(' ');
                    if (strings.Length > 1)
                    {
                        ToolStripMenuItem reverseMenuItem = new ToolStripMenuItem(REVERSE_CONTEXT_MENU);
                        this.ContextMenuStrip.Items.Add(reverseMenuItem);

                        ToolStripMenuItem deleteLeftMenuItem = new ToolStripMenuItem(DELETE_LEFT_CONTEXT_MENU);
                        this.ContextMenuStrip.Items.Add(deleteLeftMenuItem);

                        ToolStripMenuItem deleteRightMenuItem = new ToolStripMenuItem(DELETE_RIGHT_CONTEXT_MENU);
                        this.ContextMenuStrip.Items.Add(deleteRightMenuItem);
                    }

                    ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem(DELETE_CONTEXT_MENU);
                    this.ContextMenuStrip.Items.Add(deleteMenuItem);


                    e.Cancel = false;
                }
                else
                {
                    this.ContextMenuStrip.Items.Clear();
                    ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem(SPLIT_CONTEXT_MENU);

                    this.ContextMenuStrip.Items.Add(deleteMenuItem);
                    e.Cancel = false;
                }
            }
        }

        private void ContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (e.ClickedItem.Text == MERGE_CONTEXT_MENU)
                {
                    int savedColumn = this.CurrentCell.ColumnIndex;
                    int savedRow = this.CurrentCell.RowIndex;
                    if (this.CurrentVerse != null)
                        undoStack.Push(new VerseEx(new Verse(this.CurrentVerse), savedColumn, savedRow));

                    int firstMergeIndex = this.SelectedCells[0].ColumnIndex;
                    for (int i = 1; i < this.SelectedCells.Count; i++)
                    {
                        if (this.SelectedCells[i].ColumnIndex < firstMergeIndex)
                            firstMergeIndex = this.SelectedCells[i].ColumnIndex;
                    }

                    this.CurrentVerse.Merge(firstMergeIndex, this.SelectedCells.Count);

                    if (firstMergeIndex == 0 && SelectedCells.Count == 2 && (string)this[firstMergeIndex, 0].Value == "«")
                    {
                        this.CurrentVerse[0].Word = this.CurrentVerse[0].Word.Replace("« ", "«");
                    }

                    this.Update(CurrentVerse);
                    SaveVerse(CurrentVerseReferece);

                    this[firstMergeIndex, Rows.Count - 1].Selected = true;
                    this.CurrentCell = this[firstMergeIndex, Rows.Count - 1];
                    if (!string.IsNullOrEmpty(((StrongsCluster)this[firstMergeIndex, Rows.Count - 1].Value).ToString()))
                        FireRefernceHighlightRequest((StrongsCluster)this[firstMergeIndex, Rows.Count - 1].Value);
                    FireVerseViewChanged();
                }
                else if (e.ClickedItem.Text == SWAP_CONTEXT_MENU)
                {
                    int savedColumn = this.CurrentCell.ColumnIndex;
                    int savedRow = this.CurrentCell.RowIndex;
                    if (this.CurrentVerse != null)
                        undoStack.Push(new VerseEx(new Verse(this.CurrentVerse), savedColumn, savedRow));

                    int firstSwapIndex = this.SelectedCells[0].ColumnIndex;
                    int secondSwapIndex = this.SelectedCells[1].ColumnIndex;
                    //for (int i = 1; i < this.SelectedCells.Count; i++)
                    //{
                    //    if (this.SelectedCells[i].ColumnIndex < firstSwapIndex)
                    //        firstSwapIndex = this.SelectedCells[i].ColumnIndex;
                    //}
                    this.CurrentVerse.SwapTags(firstSwapIndex, secondSwapIndex);

                    SaveVerse(CurrentVerseReferece);
                    this.Update(CurrentVerse);

                    this[firstSwapIndex, Rows.Count - 1].Selected = true;
                    this.CurrentCell = this[firstSwapIndex, Rows.Count - 1];
                    if (!string.IsNullOrEmpty(((StrongsCluster)this[firstSwapIndex, Rows.Count - 1].Value).ToString()))
                        FireRefernceHighlightRequest((StrongsCluster)this[firstSwapIndex, Rows.Count - 1].Value);
                    FireVerseViewChanged();
                }
                else if (e.ClickedItem.Text == SPLIT_CONTEXT_MENU)
                {
                    int savedColumn = this.CurrentCell.ColumnIndex;
                    int savedRow = this.CurrentCell.RowIndex;
                    if (this.CurrentVerse != null)
                        undoStack.Push(new VerseEx(new Verse(this.CurrentVerse), savedColumn, savedRow));

                    int splitIndex = this.SelectedCells[0].ColumnIndex;
                    for (int i = 1; i < this.SelectedCells.Count; i++)
                    {
                        if (this.SelectedCells[i].ColumnIndex < splitIndex)
                            splitIndex = this.SelectedCells[i].ColumnIndex;
                    }
                    this.CurrentVerse.split(splitIndex);

                    this.Update(CurrentVerse);
                    SaveVerse(CurrentVerseReferece);

                    this[splitIndex, Rows.Count - 1].Selected = true;
                    this.CurrentCell = this[splitIndex, Rows.Count - 1];
                    if (!string.IsNullOrEmpty(((StrongsCluster)this[splitIndex, Rows.Count - 1].Value).ToString()))
                        FireRefernceHighlightRequest((StrongsCluster)this[splitIndex, Rows.Count - 1].Value);
                    FireVerseViewChanged();
                }
                else if (e.ClickedItem.Text == DELETE_CONTEXT_MENU)
                {
                    int savedColumn = this.CurrentCell.ColumnIndex;
                    int savedRow = this.CurrentCell.RowIndex;
                    if (this.CurrentVerse != null)
                        undoStack.Push(new VerseEx(new Verse(this.CurrentVerse), savedColumn, savedRow));

                    int col = this.SelectedCells[0].ColumnIndex;
                    this.CurrentVerse[col].Strong = new StrongsCluster(new string[] { "" });
                    this.Update(CurrentVerse);
                    SaveVerse(CurrentVerseReferece);

                    this[col, Rows.Count - 1].Selected = true;
                    this.CurrentCell = this[col, Rows.Count - 1];
                    FireVerseViewChanged();
                }
                else
                {
                    int col = this.SelectedCells[0].ColumnIndex;
                    StrongsCluster strongsCluster = this.CurrentVerse[col].Strong;
                    if (strongsCluster.Count < 2) return;

                    int savedColumn = this.CurrentCell.ColumnIndex;
                    int savedRow = this.CurrentCell.RowIndex;
                    if (this.CurrentVerse != null)
                        undoStack.Push(new VerseEx(new Verse(this.CurrentVerse), savedColumn, savedRow));

                    if (e.ClickedItem.Text == REVERSE_CONTEXT_MENU)
                    {
                        StrongsNumber temp = strongsCluster[0];
                        strongsCluster[0] = strongsCluster[strongsCluster.Count - 1];
                        strongsCluster[strongsCluster.Count - 1] = temp;
                    }
                    else if (e.ClickedItem.Text == DELETE_LEFT_CONTEXT_MENU)
                    {
                        strongsCluster.DeleteAt(0);
                    }
                    else if (e.ClickedItem.Text == DELETE_RIGHT_CONTEXT_MENU)
                    {
                        strongsCluster.DeleteAt(strongsCluster.Count - 1);
                    }

                    SaveVerse(CurrentVerseReferece);
                    Update(CurrentVerse);

                    this[col, Rows.Count - 1].Selected = true;
                    this.CurrentCell = this[col, Rows.Count - 1];
                    FireRefernceHighlightRequest((StrongsCluster)this[col, Rows.Count - 1].Value);
                    FireVerseViewChanged();
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }
        }

        #endregion Context Menue

        #region Save & Update
        /// <summary>
        /// Updates the target verse display when the verse contains tags already
        /// </summary>
        /// <param name="verse">tagged verse</param>
        public void Update(Verse verse)
        {
            //«
            try
            {
                if (verse == null)
                {
                    this.Rows.Clear();
                    this.ColumnCount = 0;

                    return;
                }

                this.CurrentVerse = verse;
                bool oldTestament = false;

                //string direction = Properties.MainSettings.Default.TargetTextDirection;
                this.Rows.Clear();

                string[] verseWords = new string[verse.Count];
                string[] verseTags = new string[verse.Count];
                string[] verseMorphs = new string[verse.Count];
                StrongsCluster[] strongsClusters = new StrongsCluster[verse.Count];

                GridAncientWord[] ancientWords = null;
                GridAncientMeaning[] ancientMeanings = null;
                if (verse.AncientVerse != null)
                {
                    ancientWords = new GridAncientWord[verse.Count];
                    ancientMeanings = new GridAncientMeaning[verse.Count];
                }

                oldTestament = (verse[0].Testament == BibleTestament.OT);

                // key: strongs number, value: occurance count
                var counts = new Dictionary<string, int>();
                var strongsMultiOccurances = new List<(string Strongs, int Occurrence)>();
                for (int i = 0; i < verse.Count; i++)
                {
                    StrongsCluster sc = verse[i].Strong;
                    foreach (StrongsNumber sn in sc.Strongs)
                    {
                        if (!counts.ContainsKey(sn.ToString()))
                            counts[sn.ToString()] = 0;

                        counts[sn.ToString()]++;
                        strongsMultiOccurances.Add((sn.ToString(), counts[sn.ToString()]));
                    }
                }

                int strongsIndex = 0;
                for (int i = 0; i < verse.Count; i++)
                {
                    verseWords[i] = verse[i].Word;
                    verseTags[i] = verse[i].Strong.ToString();
                    verseMorphs[i] = verse[i].Strong.ToStringMorphs();
                    StrongsCluster strongs = verse[i].Strong;
                    strongsClusters[i] = verse[i].Strong;

                    if (verse.AncientVerse != null)
                    {
                        bool extended = Properties.ReferenceBibles.Default.ExtendedTaggingOT;
                        if (extended)
                        {
                            List<TahotSubWord> aw = new List<TahotSubWord>();
                            foreach (StrongsNumber s in strongs.Strongs)
                            {
                                aw.Add(verse.AncientVerse.GeSubtWordFromStrong(s.ToString(), strongsMultiOccurances[strongsIndex].Occurrence));
                                strongsIndex++;
                            }
                            //VerseWord aw = verse.AncientVerse.GetWordFromStrong(verseTags[i]);
                            ancientWords[i] = new GridAncientWord(aw, oldTestament, verse.AncientVerse);
                            ancientMeanings[i] = new GridAncientMeaning(aw, oldTestament, verse.AncientVerse);
                        }
                        else
                        {
                            List<VerseWord> aw = new List<VerseWord>();
                            foreach (StrongsNumber s in strongs.Strongs)
                            {
                                aw.Add(verse.AncientVerse.GetWordFromStrong(s.ToString(), strongsMultiOccurances[strongsIndex].Occurrence));
                                strongsIndex++;
                            }
                            //VerseWord aw = verse.AncientVerse.GetWordFromStrong(verseTags[i]);
                            ancientWords[i] = new GridAncientWord(aw, oldTestament, verse.AncientVerse);
                            ancientMeanings[i] = new GridAncientMeaning(aw, oldTestament, verse.AncientVerse);
                        }
                    }
                }

                int col = -1;
                for (int i = 0; i < verseTags.Length; i++)
                {
                    // if (verseTags[i].Contains("3068")) //verseTags[i].Contains("???") || verseTags[i].Contains("0000"))
                    //{
                    //    col = i;
                    //    break;
                    // }
                    if (verseTags[i] == "<>")
                        verseTags[i] = string.Empty;
                }

                string[] wordNumber = new string[verseWords.Length];
                Verse[] ancientVerse = new Verse[verseWords.Length];
                for (int i = 0; i < verseWords.Length; i++)
                {
                    wordNumber[i] = (i + 1).ToString();
                    ancientVerse[i] = verse.AncientVerse;
                }

                this.ColumnCount = verseWords.Length;
                this.Rows.Add(verseWords);

                this.Rows.Add(ancientVerse);
                Rows[1].Visible = false;

                if (Properties.TargetBibles.Default.ShowAncientMeaning && ancientMeanings is not null && ancientMeanings.Length != 0)
                    this.Rows.Add(ancientMeanings);

                if (Properties.TargetBibles.Default.ShowAncientWord && ancientWords is not null && ancientWords.Length != 0)
                    this.Rows.Add(ancientWords);


                if (Properties.TargetBibles.Default.ShowAncientMorphology && verseMorphs is not null && verseMorphs.Length != 0)
                    this.Rows.Add(verseMorphs);

                this.Rows.Add(wordNumber);

                this.Rows.Add(strongsClusters);

                //this.Rows.Add(verseTags);

                string tagToHighlight = SearchTag;
                if (string.IsNullOrEmpty(tagToHighlight) || tagToHighlight.ToLower() == "<blank>")
                    tagToHighlight = "<>";

                int tRow = this.Rows.Count - 1;
                for (int i = 0; i < verseWords.Length; i++)
                {
                    string word = (string)this.Rows[0].Cells[i].Value;
                    StrongsCluster tag = (StrongsCluster)this.Rows[tRow].Cells[i].Value;
                    StrongsCluster nextTag = new StrongsCluster();
                    if (i < verseWords.Length - 1)
                    {
                        nextTag = (StrongsCluster)this.Rows[tRow].Cells[i + 1].Value;
                    }
                    if (tag == null)
                        continue;

                    if ((tag.ToString().Contains("3068") || tag.ToString().Contains("3069")) && oldTestament)
                    {
                        this.Rows[tRow].Cells[i].Style.ForeColor = Color.Red;
                    }
                    if (tag.ToString().Contains("0430>") && oldTestament)
                    {
                        //this.Rows[tRow].Cells[i].Style.BackColor = Color.Green;
                        this.Rows[tRow].Cells[i].Style.ForeColor = Color.Green;
                    }

                    if (tag.ToString().Contains("0410>") && oldTestament)
                        this.Rows[tRow].Cells[i].Style.ForeColor = Color.Blue;

                    if (tag.ToString().Contains(tagToHighlight) || tag.ToString().Contains("0000") || (tag.ToString() == string.Empty && tagToHighlight == "<>"))
                    {
                        this.Rows[tRow].Cells[i].Style.ForeColor = Color.Maroon;
                        if (RowCount > 2)
                            this.Rows[tRow - 1].Cells[i].Style.BackColor = Color.Yellow;
                    }
                    else if (!string.IsNullOrEmpty(tag.ToString()) && tag.ToString() == nextTag.ToString())
                    {
                        this.Rows[tRow].Cells[i].Style.BackColor = Color.Yellow;
                        this.Rows[tRow].Cells[i + 1].Style.BackColor = Color.Yellow;
                    }

                    //if (direction.ToLower() == "rtl")
                    if (Properties.TargetBibles.Default.RightToLeft)
                        this.Columns[i].DisplayIndex = verseWords.Length - i - 1;
                    else
                        this.Columns[i].DisplayIndex = i;
                }

                if (Bible.BibleName.ToUpper().Contains("NIV"))
                {
                    //string[] wordsToHiglight = { "were:", "through", "Ard", "the" };
                    //string[] wordsToHiglight = { "Abel", "‘Let’s", "go", "out", "to", "the", "field.’", "While"};
                    //string[] wordsToHiglight = { "hope", "Lord", "," };
                    string reference = Utils.GetUbsReference(verse.VerseWords[0].Reference);

                    if (nivLXX.Keys.Contains(reference))
                    {
                        // wordsToHiglight is a test case to generate code
                        // but the code should be generic enough to handle any wordsToHiglight array
                        // wordsToHiglight may be togther in one cell seperated by a space,
                        // or be ditributed accreoss adjacent cells,
                        // so we need to find the first cell that contains the first wordToHighlight and the cell that contains the last word to highlight
                        // The first and last entry in the wordsToHighlight array act as delimiters.
                        // In the test case, we find the first cell that contains "were:" and is followed by a cell that contains "through"
                        // Then the cell containing "through" will be the first cell to highlight,
                        // we need see if the cell containing through also contains only "through" or also contains "Mard"
                        // and we progress untill we find all adjacent cells that contain the words in the wordsToHighlight array,
                        // Then we highlight all the columns (whole columns) of the adjacent cells excluding the first and the last.
                        List<string> wordsToHiglight = nivLXX[reference];
                        if (wordsToHiglight != null && wordsToHiglight.Count > 0)
                        {
                            int firstIndex = -1;
                            int lastIndex = -1;
                            int n = verseWords.Length;

                            // Find the first cell that contains the first target word.
                            for (int i = 0; i < n; i++)
                            {
                                if(wordsToHiglight[0] == "-1")
                                {
                                    firstIndex = 0;
                                    break;
                                }
                                string cellText = (this.Rows[0].Cells[i].Value as string) ?? string.Empty;
                                if (cellText.IndexOf(wordsToHiglight[0], StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    string nextCellText = (i + 1 < n) ? (this.Rows[0].Cells[i + 1].Value as string) ?? string.Empty : string.Empty;
                                    if (nextCellText.IndexOf(wordsToHiglight[1], StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        firstIndex = i;
                                        break;
                                    }
                                }
                            }


                            // Find the last cell that contains the last target word.
                            for (int i = n - 1; i >= 0; i--)
                            {
                                if(wordsToHiglight[wordsToHiglight.Count - 1] == "-1")
                                {
                                    lastIndex = n - 1;
                                    break;
                                }
                                string cellText = (this.Rows[0].Cells[i].Value as string) ?? string.Empty;
                                if (cellText.IndexOf(wordsToHiglight[wordsToHiglight.Count - 1], StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    string prevCellText = (i - 1 >= 0) ? (this.Rows[0].Cells[i - 1].Value as string) ?? string.Empty : string.Empty;
                                    if (prevCellText.IndexOf(wordsToHiglight[wordsToHiglight.Count - 2], StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        lastIndex = i;
                                        break;
                                    }
                                }
                            }

                            // Fallback: try to locate the whole phrase in a single cell if either end wasn't found independently.
                            if (firstIndex == -1 || lastIndex == -1)
                            {
                                string phrase = string.Join(" ", wordsToHiglight);
                                for (int i = 0; i < n; i++)
                                {
                                    string cellText = (this.Rows[0].Cells[i].Value as string) ?? string.Empty;
                                    if (cellText.IndexOf(phrase, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        firstIndex = lastIndex = i;
                                        break;
                                    }
                                }
                            }

                            // If both indices were found and there is at least one column strictly between them, highlight full columns (exclude boundaries).
                            if (firstIndex != -1 && lastIndex != -1)// && Math.Abs(lastIndex - firstIndex) > 1)
                            {

                                int start = Math.Min(firstIndex, lastIndex) + 1;
                                int end = Math.Max(firstIndex, lastIndex) - 1;
                                if (wordsToHiglight[0] == "-1")
                                    start = 0;
                                if (wordsToHiglight[wordsToHiglight.Count - 1] == "-1")
                                    end = this.Columns.Count - 1;

                                for (int colIndex = start; colIndex <= end; colIndex++)
                                {
                                    // Highlight every row in the column (complete column).
                                    for (int rowIndex = 0; rowIndex < this.Rows.Count; rowIndex++)
                                    {
                                        // Skip null cells defensively.
                                        var cell = this.Rows[rowIndex].Cells[colIndex];
                                        if (cell != null)
                                        {
                                            cell.Style.BackColor = Color.LightGray;
                                        }
                                    }
                                }
                            }
                        }
                        //////////////////////////
                    }
                }

                if (col >= 0)
                    this.CurrentCell = this.Rows[tRow].Cells[col];

                this.ClearSelection();

                if (Properties.TargetBibles.Default.RightToLeft)
                    this.Rows[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                if (Properties.TargetBibles.Default.ShowAncientWord && ancientWords is not null && oldTestament)
                    this.Rows[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                this.Rows[this.Rows.Count - 1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                this.Rows[0].ReadOnly = true;
                this[0, tRow].Selected = true;
                this.CurrentCell = this[0, tRow];

                if (!string.IsNullOrEmpty(((StrongsCluster)this[0, tRow].Value).ToString()))
                    FireRefernceHighlightRequest((StrongsCluster)this[0, tRow].Value);
                //this.Rows[tRow].ReadOnly = true;
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
        }

        Dictionary<string, List<string>> nivLXX = new Dictionary<string, List<string>>()
        {
            { "Gen 4:8", new List<string>(){"Abel", "‘Let’s", "go", "out", "to", "the", "field.’", "While"}},
            {"Num 26:40", new List<string>(){"were:", "through", "Ard", "the" }},
            {"Jdg 16:13", new List<string>(){"loom", "and", "tighten", "...", "-1"}},
            {"1Sa 14:41", new List<string>(){"Israel,",	"‘Why", "have", "...", "fault,",  "respond"}},
            {"2Sa 12:16", new List<string>(){"lying", "in", "sackcloth", "on"}},
            {"2Sa 13:34", new List<string>(){"down",   "the", "side", "of", "the", "hill.", "...", "of", "Horonaim", "on"}},
            {"2Sa 15:8", new List<string>(){"Lord",	"in", "Hebron", "’"}},
            {"2Sa 23:33", new List<string>(){"-1", "son", "of", "Shammah"}},
            {"2Sa 24:17", new List<string>(){"I,", "the", "shepherd", "have"}},
            {"1Ch 4:13", new List<string>(){"Hathath", "and", "Meonothai", "-1"}},
            {"1Ch 6:27", new List<string>(){"Elkanah", "his", "son", "and", "Samuel", "his"}},//6:12
            {"1Ch 6:28", new List<string>(){"Samuel", "Joel", "the"}},//6:13
            {"1Ch 6:59", new List<string>(){"Ashan,",  "Juttah",  "and"}},//6:44
            {"1Ch 6:60", new List<string>(){"given" ,  "Gibeon", "Geba",}},//6:45
            {"1Ch 6:77", new List<string>(){"received", "Jokneam", "Kartah", "Rimmono"}},//6:62
            {"1Ch 7:25", new List<string>(){"Resheph", "his", "son", "Telah"}},
            {"1Ch 8:29", new List<string>(){"-1", "Jeiel", "the"}},
            {"1Ch 8:30", new List<string>(){"Baal", "Ner", "Nadab"}},
            {"1Ch 9:41", new List<string>(){"Tahrea",  "and", "Ahaz", "-1"}},
            {"1Ch 21:17", new List<string>(){"I,", "the", "shepherd", "have"}},
            {"1Ch 25:9", new List<string>(){"Joseph,", "his", "sons", "and", "relatives", "12"}},
            {"2Ch 15:8", new List<string>(){"of", "Azariah", "son", "of", "Oded"}},
            {"Ezr 8:5", new List<string>(){"of",  "Zattu",  "Shekaniah"}},
            {"Ezr 8:10", new List<string>(){"of",  "Bani,", "Shelomith"}},
            {"Est 3:7", new List<string>(){"month",   "And", "the", "lot", "fell",    "on",  "the"}},
            {"Psa 25:21", new List<string>(){"hope", "Lord", ","}},
            {"Psa 56:7", new List<string>(){"do", "not", "let"}},
            {"Psa 145:13", new List<string>(){"generations.",  "The", "Lord", "...", "-1"}},
            {"Ecc 9:2", new List<string>(){"good", "and", "the", "bad,", "the"}},
            {"Isa 53:11", new List<string>(){ "suffered,", "he",  "will", "see", "the", "light", "of", "life", "and" } }
        };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="verse"></param>
    public void SaveVerse(Verse verse)
        {
            if (Properties.MainSettings.Default.Osis)
                OsisBible.Bible[verse[0].Reference] = verse;
            else
                Bible.Bible[verse[0].Reference] = verse;

            if (!Utils.AreReferencesEqual(CurrentVerseReferece, verse[0].Reference))
            {
                FireGotoVerseRequest(verse[0].Reference);
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        public void SaveVerse(string reference)
        {
            var cm = System.Reflection.MethodBase.GetCurrentMethod();
            var name = cm.DeclaringType.FullName + "." + cm.Name;
            Tracing.TraceEntry(name, reference);

            bool osis = Properties.MainSettings.Default.Osis;

            Verse verse = new Verse();

            try
            {
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    StrongsCluster tag = ((StrongsCluster)this[i, Rows.Count - 1].Value);
 
                    verse[i] = new VerseWord((string)this[i, 0].Value, tag, reference);
                    if ((Properties.TargetBibles.Default.ShowAncientWord ||
                        Properties.TargetBibles.Default.ShowAncientMeaning) &&
                        this.Rows.Count > 3)
                     {
                        if (this[i, 1].Value is not null)
                            verse.AncientVerse = (Verse)(this[i, 1].Value);
                    }
                    if (osis)
                    {
                        verse[i].OsisTagIndex = CurrentVerse[i].OsisTagIndex;
                        verse[i].OsisTagLevel = CurrentVerse[i].OsisTagLevel;
                        verse.Dirty = true;
                    }
                }

                reference = osis ? OsisBible.GetCorrectReference(reference) : Bible.GetCorrectReference(reference);

                Dictionary<string, Verse> bible = osis ? OsisBible.Bible : Bible.Bible;
                if (bible.ContainsKey(reference))
                {
                    string oldVerse = bible[reference].ToString();
                    bible[reference] = verse;
                    Tracing.TraceInfo(name, reference, "OLD: " + oldVerse, "NEW: " + verse.ToString());

                }
                else
                {
                    bible.Add(reference, verse);
                }
            }
            catch (Exception ex)
            {
                Tracing.TraceException(name, ex);
                throw;
            }
            Tracing.TraceExit(name);
        }
        #endregion Save & Update


        #region overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            if (row == 0)
            {
                string newWord = (string)this[e.ColumnIndex, e.RowIndex].Value;
                CurrentVerse.UpdateWord(e.ColumnIndex, newWord);
                //                FireVerseViewChanged();
            }
            else if (row == this.RowCount - 1) 
            {
                StrongsCluster oldValue = CurrentVerse[col].Strong;
                StrongsCluster newValue = null;
                // tags Row
                if(this[col, row].Value == null)
                {

                    newValue = new StrongsCluster(new string[] {""});
                }
                else if (this[col, row].Value is string)
                {
                    // if the new value is a string, this means the tag may've been edited been edited
                    string temp = (string)this[col, row].Value;
                    newValue = new StrongsCluster(temp.Split(' '));
                    if (oldValue.ToString() == temp)
                    {
                        this[col, row].Value = newValue;
                    }
                }
                else
                {
                    newValue = (StrongsCluster)this[col, row].Value;
                }
                if (CurrentVerse[col].Strong.ToString() != newValue.ToString())
                {
                    if (this.CurrentVerse != null)
                        undoStack.Push(new VerseEx(new Verse(this.CurrentVerse), col, row));

                    // if (newValue == null) newValue = string.Empty; TODO:this should be an exception
                    CurrentVerse[col].Strong = newValue;

                    this[e.ColumnIndex, e.RowIndex].Value = newValue;

                    FireRefernceHighlightRequest(newValue);

                    //SaveVerse(CurrentVerseReferece);
                    FireVerseViewChanged();
                }
            }

            base.OnCellValueChanged(e);
        }


        bool gotFocus = false;

        protected override void OnLostFocus(EventArgs e)
        {
            gotFocus = false;
            base.OnLostFocus(e);
        }
        protected override void OnGotFocus(EventArgs e)
        {
            gotFocus = true;
            base.OnGotFocus(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellEnter(DataGridViewCellEventArgs e)
        {
            if (this.Rows.Count > 1)
            {
                if (!this.gotFocus)
                {
                    return;
                }
                // during initialsation, we may come here
                // when the grid rows are not fully initialised
                //if (this.SelectedRows.Count > 1)
                //{
                //}
                if (e.RowIndex == Rows.Count - 1)
                {
                    this.ClearSelection();
                    this[e.ColumnIndex, e.RowIndex].Selected = true;
                }
                FireRefernceHighlightRequest((StrongsCluster)this.Rows[Rows.Count - 1].Cells[e.ColumnIndex].Value);

            }
            //base.OnCellEnter(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        //protected override void OnColumnRemoved(DataGridViewColumnEventArgs e)
        //{
        //    FireVerseViewChanged();
        //    base.OnColumnRemoved(e);
        //}

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if(this.SelectedCells.Count == 1)
                {
                    int savedColumn = this.CurrentCell.ColumnIndex;
                    int savedRow = this.CurrentCell.RowIndex;
                    if (this.CurrentVerse != null)
                        undoStack.Push(new VerseEx(new Verse(this.CurrentVerse), savedColumn, savedRow));

                    int col = this.SelectedCells[0].ColumnIndex;
                    this[col, Rows.Count - 1].Value = string.Empty;
                    this.CurrentVerse[col].Strong = new StrongsCluster();
                    SaveVerse(CurrentVerseReferece);
                    this.Update(CurrentVerse);
                    SaveVerse(CurrentVerseReferece);

                    this[col, Rows.Count - 1].Selected = true;
                    this.CurrentCell = this[col, Rows.Count - 1];
                    FireVerseViewChanged();

                }
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            drgevent.Effect = DragDropEffects.Copy;
            base.OnDragEnter(drgevent);
        }
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            var cm = System.Reflection.MethodBase.GetCurrentMethod();
            var name = cm.DeclaringType.FullName + "." + cm.Name;

            DragData data = drgevent.Data.GetData(typeof(DragData)) as DragData;
            StrongsCluster droppedValue = data.Tag;

            Tracing.TraceEntry(name, $"StrongsCluster = {droppedValue.ToString()}");
            
            Point cursorLocation = this.PointToClient(new Point(drgevent.X, drgevent.Y));
            try
            {
                System.Windows.Forms.DataGridView.HitTestInfo hittest = this.HitTest(cursorLocation.X, cursorLocation.Y);
                if (hittest.ColumnIndex != -1
                    && hittest.RowIndex == Rows.Count - 1)
                {  //CHANGE
                    if (data.Source.Equals(this))
                    {
                        if (data.ColumnIndex == hittest.ColumnIndex)
                            return;
                    }
                    Tracing.TraceInfo(name, $"Hit at column {hittest.ColumnIndex}, row {hittest.RowIndex}");
                    int savedColumn = this.CurrentCell.ColumnIndex;
                    int savedRow = this.CurrentCell.RowIndex;
                    if (this.CurrentVerse != null)
                        undoStack.Push(new VerseEx(new Verse(this.CurrentVerse), savedColumn, savedRow));

                    //this[hittest.ColumnIndex, Rows.Count - 1].Value = newValue.Trim();
                    //this.CurrentVerse[hittest.ColumnIndex].StrongString = newValue.Trim();


                    if (Control.ModifierKeys == Keys.Control)
                        this.CurrentVerse[hittest.ColumnIndex].Strong += droppedValue;
                    else
                        this.CurrentVerse[hittest.ColumnIndex].Strong = droppedValue;

                    if (data.Source.Equals(this))
                    {
                        //this[data.ColumnIndex, Rows.Count - 1].Value = string.Empty;
                        if (Control.ModifierKeys != Keys.Control)
                            this.CurrentVerse[data.ColumnIndex].Strong = new StrongsCluster(new string[] {""});
                    }
                    Update(CurrentVerse);
                    SaveVerse(CurrentVerseReferece);
                    
                    FireVerseViewChanged();

                    this.ClearSelection();
                    this[hittest.ColumnIndex, Rows.Count - 1].Selected = true;
                    this.Rows[0].ReadOnly = true;

                    this[hittest.ColumnIndex, Rows.Count - 1].Selected = true;
                    this.CurrentCell = this[hittest.ColumnIndex, Rows.Count - 1];
                    if (!string.IsNullOrEmpty(((StrongsCluster)this[hittest.ColumnIndex, Rows.Count - 1].Value).ToString()))
                        FireRefernceHighlightRequest((StrongsCluster)this[hittest.ColumnIndex, Rows.Count - 1].Value);

                    this.Focus();
                }
            }
            catch(Exception ex)
            {
                Tracing.TraceException(name, ex.Message);
            }

            base.OnDragDrop(drgevent);
        }

        public void SimulateDataDrop(DragData data)
        {
            if(this.SelectedCells.Count == 1)
            {
                int savedColumn = this.CurrentCell.ColumnIndex;
                int savedRow = this.CurrentCell.RowIndex;
                if (this.CurrentVerse != null)
                    undoStack.Push(new VerseEx(new Verse(this.CurrentVerse), savedColumn, savedRow));

                int targetColumn = this.SelectedCells[0].ColumnIndex;
                this.CurrentVerse[targetColumn].Strong = data.Tag;

                Update(CurrentVerse);
                SaveVerse(CurrentVerseReferece);

                FireVerseViewChanged();

                this.ClearSelection();
                this[targetColumn, Rows.Count - 1].Selected = true;
                this.Rows[0].ReadOnly = true;

                this[targetColumn, Rows.Count - 1].Selected = true;
                this.CurrentCell = this[targetColumn, Rows.Count - 1];
                if (!string.IsNullOrEmpty(((StrongsCluster)this[targetColumn, Rows.Count - 1].Value).ToString()))
                    FireRefernceHighlightRequest((StrongsCluster)this[targetColumn, Rows.Count - 1].Value);
                
                this.Focus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                DataGridView.HitTestInfo info = this.HitTest(e.X, e.Y);
                if (info.RowIndex > 0)
                {
                    if (info.RowIndex >= 0 && info.ColumnIndex >= 0)
                    {
                        StrongsCluster tag = (StrongsCluster)this.Rows[Rows.Count - 1].Cells[info.ColumnIndex].Value;
                        DragData data = new DragData(1, info.ColumnIndex, tag, this);
                        if (data != null)
                        {
                            //Need to put braces here  CHANGE
                            this.DoDragDrop(data, DragDropEffects.Copy);
                        }
                    }
                }
            }

            base.OnMouseDown(e);
        }

        #endregion overrides

        #region Undo / Redo
        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                int savedColumn = this.CurrentCell.ColumnIndex;
                int savedRow = this.CurrentCell.RowIndex;
                redoStack.Push(new VerseEx(new Verse(this.CurrentVerse), savedColumn, savedRow));

                VerseEx verseEx = undoStack.Pop();
                Verse verse = verseEx.SavedVerse; 

                if (Utils.AreReferencesEqual(CurrentVerseReferece, verse[0].Reference))
                {
                    Update(verse);
                }
                SaveVerse(verse);
                this.CurrentCell = this[verseEx.Colum, verseEx.Row];
            }
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                VerseEx verseEx = redoStack.Pop();
                Verse verse = verseEx.SavedVerse;
                if (Utils.AreReferencesEqual(CurrentVerseReferece, verse[0].Reference))
                {
                    Update(verse);
                }
                SaveVerse(verse);
                this.CurrentCell = this[verseEx.Colum, verseEx.Row];
            }
        }

        #endregion Undo / Redo

        #region Firing events
        public void FireVerseViewChanged()
        {
            if (this.VerseViewChanged != null)
            {
                new Thread(() =>
                {
                    try
                    {
                        this.VerseViewChanged(this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        var cm = System.Reflection.MethodBase.GetCurrentMethod();
                        var name = cm.DeclaringType.FullName + "." + cm.Name;
                        Tracing.TraceException(name, ex.Message);
                    }
                }).Start();
            }

        }

        public void FireRefernceHighlightRequest(StrongsCluster tag)
        {
            if (this.RefernceHighlightRequest != null)
            {
//                new Thread(() =>
//                {
                    try
                    {
                        bool firstHalf = true;
                        //if (this.SelectedCells.Count == 1)
                        {
                            if (this.SelectedCells[0].ColumnIndex > (this.ColumnCount / 2))
                                firstHalf = false;
                        }
                        this.RefernceHighlightRequest(this, tag, firstHalf);
                    }
                    catch (Exception ex)
                    {
                        var cm = System.Reflection.MethodBase.GetCurrentMethod();
                        var name = cm.DeclaringType.FullName + "." + cm.Name;
                        Tracing.TraceException(name, ex.Message);
                    }
//                }).Start();
            }
        }

        public void FireGotoVerseRequest(string tag)
        {
            if (this.GotoVerseRequest != null)
            {
                new Thread(() =>
                {
                    try
                    {
                        this.GotoVerseRequest(this, tag);
                    }
                    catch (Exception ex)
                    {
                        var cm = System.Reflection.MethodBase.GetCurrentMethod();
                        var name = cm.DeclaringType.FullName + "." + cm.Name;
                        Tracing.TraceException(name, ex.Message);
                    }
                }).Start();
            }

        }
        #endregion Firing events

    }
    public delegate void VerseViewChangedEventHandler(object sender, EventArgs e);
    public delegate void RefernceHighlightRequestEventHandler(object sender, StrongsCluster tag, bool firstHalf);
    public delegate void GotoVerseRequestEventHandler(object sender, string reference);

    internal class GridAncientWord
    {
        public GridAncientWord(List<VerseWord> ancientVerseWord, bool oldTestament, Verse ancientVerse)
        {
            AncientVerseWord = ancientVerseWord;
            AncientVerseSubWord = null;
            OldTestament = oldTestament;
            AncientVerse = ancientVerse;
        }

        public GridAncientWord(List<TahotSubWord> ancientVerseSubWord, bool oldTestament, Verse ancientVerse)
        {
            AncientVerseWord = null;
            AncientVerseSubWord = ancientVerseSubWord;
            OldTestament = oldTestament;
            AncientVerse = ancientVerse;
        }

        public List<VerseWord> AncientVerseWord { get; }
        public List<TahotSubWord> AncientVerseSubWord { get; }

        public bool OldTestament { get; }
        public Verse AncientVerse { get; }
        public override string ToString()
        {
            string result = string.Empty;
            if(AncientVerseWord != null)
            {
                foreach (VerseWord w in AncientVerseWord)
                {
                    if (OldTestament)
                        result += (w is null) ? string.Empty : w.Hebrew + " ";
                    else
                        result += (w is null) ? string.Empty : w.Greek + " ";
                }
            }
            else if (AncientVerseSubWord != null)
            {
                foreach (TahotSubWord w in AncientVerseSubWord)
                {
                    if (OldTestament)
                        result += (w is null) ? string.Empty : w.Hebrew + " ";
                }
            }
            return result.Trim();
        }

    }

    internal class GridAncientMeaning
    {
        public GridAncientMeaning(List<VerseWord> ancientVerseWord, bool oldTestament, Verse ancientVerse)
        {
            AncientVerseWord = ancientVerseWord;
            AncientVerseSubWord = null;
            OldTestament = oldTestament;
            AncientVerse = ancientVerse;
        }

        public GridAncientMeaning(List<TahotSubWord> ancientVerseSubWord, bool oldTestament, Verse ancientVerse)
        {
            AncientVerseWord = null;
            AncientVerseSubWord = ancientVerseSubWord;
            OldTestament = oldTestament;
            AncientVerse = ancientVerse;
        }

        public List<VerseWord> AncientVerseWord { get; }
        public List<TahotSubWord> AncientVerseSubWord { get; }
        public bool OldTestament { get; }
        public Verse AncientVerse { get; }
        public override string ToString()
        {
            string result = string.Empty;
            if (AncientVerseWord != null)
            {
                foreach (VerseWord w in AncientVerseWord)
                {
                    result += (w is null) ? string.Empty : w.Word + " ";
                }
            }
            else if (AncientVerseSubWord != null)
            {
                foreach (TahotSubWord w in AncientVerseSubWord)
                {
                    result += (w is null) ? string.Empty : w.English + " ";
                }

            }
            return result.Trim();
        }

    }

}
