using BibleTaggingUtil.Strongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BibleTaggingUtil
{
    [Serializable()]
    public class VerseWord : ICloneable
    {
        public VerseWord(string ancientWord, string english, StrongsCluster strong, string transliteration, string reference, string morphology="", string rootStrong = "", string wordType = "", string altVerseNumber = "", string wordNumber = "", string meaningVar = "")
        {
            this.Reference = reference;
            Testament = Utils.GetTestament(reference);

            if (this.Testament == BibleTestament.OT)
                this.Hebrew = ancientWord;
            else if (this.Testament == BibleTestament.NT)
                this.Greek = ancientWord;

            this.Word = english;
            this.Strong = strong;
            this.Transliteration = transliteration;
            Morphology = morphology;
            RootStrong = rootStrong;
            WordType = wordType;
            AltVerseNumber = altVerseNumber;
            WordNumber = wordNumber;
            MeaningVar = meaningVar;
        }

        public VerseWord(string word, StrongsCluster strong, string reference)
        {
            this.Reference = reference;
            Testament = Utils.GetTestament(reference);

            this.Word = word;
            this.Strong = strong;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="strong">strongs in the form &lt;G1234A&gt;  &lt;G5678A&gt; - may not have the enclosing angle brackets
        /// </param>
        /// <param name="reference"></param>
        public VerseWord(string word, string strong, string reference)
        {
            this.Reference = reference;
            Testament = Utils.GetTestament(reference);
            this.Word = word;
            this.Strong = new StrongsCluster();
            this.Strong.Add(strong);
        }


        public BibleTestament Testament { get; private set; }
        public string MeaningVar { get; private set; }
        public string AltVerseNumber { get; private set; }
        public string WordNumber { get; private set; }
        public string Hebrew { get; private set; }
        public string Greek { get; private set; }
        public string Word { get; set; }
        public StrongsCluster Strong { get; set; }

        public String StrongStringEx
        {
            get
            {
                return this.Strong.ToString();
            }
        }

        public String StrongString
        {
            get
            {
                return this.Strong.ToString();
            }
            set
            {
                this.Strong.Set(value);
            }
        }

        public String StrongStringS
        {
            get
            {
                return this.Strong.ToStringS();
            }
        }


        public int WordIndex { get; set; }
        public string Transliteration { get; private set; }
        public string Reference { get; private set; }
        public string Morphology { get; private set; }
        public string RootStrong { get; private set; }
        public string WordType { get; private set; }

        public override string ToString()
        {
            return Reference + ": " + Word + this.StrongString;
        }

        /// <summary>
        /// Constructs an OSIS w tag. 
        /// Strongs are either sStrong or dStrong depending on settings.
        /// </summary>
        public string OsisWord
        {
            get 
            { 
                string result = string.Empty;

                if (Strong.Count == 0 || (Strong.Count == 1 && Strong[0].IsEmpty))
                {
                    result = string.Format("<w>{0}</w>", Word);
                }
                else
                {
                    string strongStr = string.Empty;
                    foreach (StrongsNumber number in Strong.Strongs)
                    {
                        if(Properties.OsisFileGeneration.Default.UseDisambiguatedStrong)
                            strongStr += string.Format(" strong:{0}", number.ToStringD());
                        else
                            strongStr += string.Format(" strong:{0}", number.ToStringS());
                    }
                    result = string.Format("<w lemma=\"{0}\">{1}</w>", strongStr.Trim(), Word);
                }
                return result; 

            }
        }
        public static VerseWord operator +(VerseWord a, VerseWord b)
        {
            return new VerseWord(
                (a.Word + " " + b.Word).Trim(),
                (a.Strong + b.Strong),
                a.Reference
                );
        }

        /// <summary>
        /// Implements "deep" copy for StrongsCluster
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            VerseWord newWord = (VerseWord) MemberwiseClone();
            newWord.Strong = new StrongsCluster(Strong.Strongs);

            return newWord;
        }


        #region OSIS XML Support
        public int OsisTagIndex { get; internal set; }
        public int OsisTagLevel { get; internal set; }

        #endregion OSIS XML Support

    }
}
