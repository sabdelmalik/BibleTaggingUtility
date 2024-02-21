using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    /// <summary>
    /// Represents a verse as a dictionary of verse words
    /// </summary>
    [Serializable()]
    public class Verse
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

        public VerseWord GetWordFromStrong(string strong, int startIndex)
        {
            VerseWord result = null;
            try
            {
                for (int i = startIndex; i < verse.Count; i++)
                {
                    if (verse[i].StrongString.Contains(strong))
                    {
                        result = verse[i];
                        break;
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

        public Verse SubVerse(int start, int length)
        {
            int l = length;
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
            string[] temp = verse[index1].Strong;
            verse[index1].Strong = verse[index2].Strong;
            verse[index2].Strong = temp;
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

            string[] tagsToSplit = verse[index].Strong;
            List<string>[] strongs = new List<string>[splitWords.Length];
            strongs[1] = new List<string>();
            int k;
            for (k = 0; k < strongs.Length; k++)
            {
                strongs[k] = new List<string>();
                if (k < tagsToSplit.Length)
                    strongs[k].Add(tagsToSplit[k]);
                else
                    strongs[k].Add("");
            }
            for (int j = k; j < tagsToSplit.Length; j++)
            {
                strongs[k - 1].Add(tagsToSplit[j]);
            }



            int columnIndex = 0;

            // copy from to start to the index
            for (int i = 0; i < index; i++)
            {
                temp[i] = verse[i];
                columnIndex++;
            }
            // create the new split words 
            for (int i = index; i < (index + splitWords.Length); i++)
            {
                temp[i] = new VerseWord(splitWords[i - index], strongs[i - index].ToArray(), verse[0].Reference);
            }
            // copy the remaining words
            for (int i = index + splitWords.Length; i < newCount; i++)
            {
                temp[i] = verse[++columnIndex];
            }
            verse = temp;
        }

        public bool Dirty { get; set; }


    }

    /// <summary>
    /// Represents a verse as a dictionary of verse words
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
