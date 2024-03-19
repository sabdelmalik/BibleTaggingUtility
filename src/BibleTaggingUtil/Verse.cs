using BibleTaggingUtil.Strongs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    /// <summary>
    /// Represents a verse as a dictionary of verse words
    /// </summary>
    [Serializable()]
    public class Verse : IEnumerable
    {
        private Dictionary<int, VerseWord> verse = new Dictionary<int, VerseWord>();

        private bool hasPsalmTitle = false;

        public Dictionary<int, VerseWord> VerseWords
        {
            get
            {
                return verse;
            }
        }
        public Verse() { Dirty = false; }

        public bool Dirty { get; set; }

        public bool HasPsalmTitle 
        {
            get 
            {
                bool result = false;
                if (verse[0].Reference.StartsWith("Ps") && verse[0].Reference.EndsWith(":1"))
                {
                    foreach(VerseWord vw in verse.Values)
                    {
                        if(vw.Word.Trim() == "*")
                        {
                            result = true;
                            break;
                        }
                    }
                }
                //return hasPsalmTitle; 
                return result;
            }
            set
            {
                if (verse.Count > 0 && verse[0].Reference == "Psa 18:1")
                {
                    int a = 0;
                }

                hasPsalmTitle = value; 
            }
        }

        public List<VerseWord> GetWordListFromStrong(StrongsNumber strongNumber)
        {

            List<VerseWord> result = new List<VerseWord>();
            try
            {
                // first, let us see if we can find an exact Match
                for (int i = 0; i < verse.Count; i++)
                {
                    if (verse[i].StrongStringEx.Contains(strongNumber.ToString()))
                    {
                        result.Add(verse[i]);
                    }
                }
                if (result.Count == 0)
                {

                    for (int i = 0; i < verse.Count; i++)
                    {
                        if (verse[i].StrongString.Contains(strongNumber.ToStringS()))
                        {
                            result.Add(verse[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
            return result;
        }
        public VerseWord GetWordFromStrong(string strong, int startIndex, bool comb)
        {
            VerseWord result = null;
            try
            {
                VerseWord fResult = null;
                VerseWord bResult = null;
                
                // search forward
                for (int i = startIndex + 1; i < verse.Count; i++)
                {
                    if (verse[i].StrongString.Contains(strong))
                    {
                        fResult = verse[i];
                        break;
                    }
                }

                if(fResult == null && startIndex > 0)
                {
                    // search backward
                    for (int i = startIndex - 1; i >= 0; i--)
                    {
                        if (verse[i].StrongString.Contains(strong))
                        {
                            bResult = verse[i];
                            break;
                        }
                    }
                }

                if(comb && fResult != null && bResult != null)
                {
                    // we are within the same Aravbic word
                    int fDiff = Math.Abs(startIndex - fResult.WordIndex);
                    int bDiff = Math.Abs(startIndex - bResult.WordIndex);
                    if (fDiff < bDiff) result = fResult;
                    else result = bResult;
                }
                else
                {
                    // prefer forward!
                    if(fResult != null) result = fResult;
                    else result = bResult;
                }

            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Creates a deep copy of itself
        /// </summary>
        /// <param name="verseToClone"></param>
        public Verse(Verse verseToClone)
        {
            this.HasPsalmTitle = verseToClone.HasPsalmTitle;
            this.Dirty = verseToClone.Dirty;
            
            for (int i = 0; i < verseToClone.Count; i++)
            {
                this.verse[i] = (VerseWord)verseToClone[i].Clone();
            }
        }

        public Verse SubVerse(int start, int last)
        {
            int l = last;
            Verse verse = new Verse();
            if (l == -1) l = this.Count;

            int idx = 0;
            for (int i = start; i < l; i++)
            {
                verse[idx++] = this[i];
            }

            return verse;
        }
        public void UpdateWord(int index, string word)
        {
            verse[index].Word = word;
        }
        /// <summary>
        /// returns the number of words in the verse
        /// </summary>
        public int Count
        {
            get
            {
                return verse.Count;
            }
        }

        /// <summary>
        /// puts or removes the word at a specific index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public VerseWord this[int index]
        {
            get
            {
                return verse[index];
            }
            set
            {
                verse[index] = value;
                verse[index].WordIndex = index;
            }
        }

        public void SwapTags(int index1, int index2)
        {
            StrongsCluster temp = verse[index1].Strong;
            verse[index1].Strong = verse[index2].Strong;
            verse[index2].Strong = temp;
        }

        public string OsisPsalmTitleData
        {
            get
            {
                string result = string.Empty;
                if (hasPsalmTitle)
                {
                    foreach (VerseWord verseWord in verse.Values)
                    {
                        if (verseWord.Word == "*")
                            break;
                        result += verseWord.OsisWord + " ";
                    }
                }

                return result.Trim();
            }
        }

        public string OsisVerseData
        {
            get
            {
                string result = string.Empty;
                bool psaTitleflag = hasPsalmTitle;
                foreach (VerseWord verseWord in verse.Values)
                {
                    if(psaTitleflag)
                    {
                        // skip up to th "*"
                        if (verseWord.Word != "*") continue;
                        psaTitleflag = false;
                        continue;
                    }
                    result += verseWord.OsisWord + " ";
                }

                return result.Trim();
            }
        }

        /// <summary>
        /// Merges two verse words together
        /// </summary>
        /// <param name="start">index of the starting word</param>
        /// <param name="count">number of words to merge</param>
        public void Merge(int start, int count)
        {
            Dictionary<int, VerseWord> temp = new Dictionary<int, VerseWord>();
            int newCount = verse.Count - count + 1;

            int columnIndex = 0;
            for (int i = 0; i < newCount; i++)
            {
                VerseWord newWord = verse[columnIndex];
                if (columnIndex == start)
                {
                    for (int j = 1; j < count; j++)
                    {
                        newWord += verse[++columnIndex];
                    }
                }

                temp[i] = newWord;
                columnIndex++;
            }

            verse = temp;
        }

        public override string ToString()
        {
            return Utils.GetVerseText(this, true);
        }

        /// <summary>
        /// Splits the word at index into two
        /// </summary>
        /// <param name="index"></param>
        public void split(int index)
        {
            string[] splitWords = verse[index].Word.Split(' ');
            if (splitWords.Length == 1)
            {
                // nothing to do
                return;
            }

            Dictionary<int, VerseWord> temp = new Dictionary<int, VerseWord>();
            int newCount = verse.Count + splitWords.Length - 1;

            StrongsCluster tagsToSplit = verse[index].Strong;
            List<StrongsNumber>[] strongs = new List<StrongsNumber>[splitWords.Length];
            //strongs[1] = new List<StrongsNumber>();
            int k;
            for (k = 0; k < strongs.Length; k++)
            {
                strongs[k] = new List<StrongsNumber>();
                if (k < tagsToSplit.Count)
                    strongs[k].Add(tagsToSplit[k]);
                else
                    strongs[k].Add(new StrongsNumber(""));
            }
            for (int j = k; j < tagsToSplit.Count; j++)
            {
                strongs[k - 1].Add(tagsToSplit[j]);
            }

            int columnIndex = 0;

            // copy from start to the index
            for (int i = 0; i < index; i++)
            {
                temp[i] = verse[i];
                columnIndex++;
            }
            // create the new split words 
            for (int i = index; i < (index + splitWords.Length); i++)
            {
                temp[i] = new VerseWord(splitWords[i - index], new StrongsCluster(strongs[i - index]), verse[0].Reference);
            }
            // copy the remaining words
            for (int i = index + splitWords.Length; i < newCount; i++)
            {
                temp[i] = verse[++columnIndex];
            }
            verse = temp;
            Dirty = true;
        }

        #region IEnumerable

        private class VerseEnumerator : IEnumerator
        {
            private Dictionary<int, VerseWord> verse;
            int position = -1;

            //constructor
            public VerseEnumerator(Dictionary<int, VerseWord> verse)
            {
                this.verse = verse;
            }
            private IEnumerator getEnumerator()
            {
                return (IEnumerator)this;
            }
            //IEnumerator
            public bool MoveNext()
            {
                position++;
                return (position < verse.Count);
            }
            //IEnumerator
            public void Reset()
            {
                position = -1;
            }
            //IEnumerator
            public object Current
            {
                get
                {
                    try
                    {
                        return verse[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }  //end nested class

        public IEnumerator GetEnumerator()
        {
            return new VerseEnumerator(verse);
        }

        #endregion IEnumerator and IEnumerable

    }

    /// <summary>
    /// Represents a verse as a dictionary of verse words
    /// used for Do/Undo
    /// </summary>
    public class VerseEx
    {
        public VerseEx(Verse verse, int col, int row)
        {
            SavedVerse = verse;
            Colum = col;
            Row = row;
            Dirty = false;
        }

        public Verse SavedVerse { get; private set; }
        public int Colum { get; private set; }
        public int Row { get; private set; }

        public bool Dirty { get; set; }

    }
}
