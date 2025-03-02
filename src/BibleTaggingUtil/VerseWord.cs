using BibleTaggingUtil.Strongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BibleTaggingUtil
{
    [Serializable()]
    public class VerseWord : ICloneable
    {
        public VerseWord(string ancientWord, string english, StrongsCluster strong, string transliteration, string reference, string morphology = "", string rootStrong = "", string wordType = "", string altVerseNumber = "", string wordNumber = "", string meaningVar = "", string dictForm = "", string dictGloss = "")
        {
            this.Reference = reference;
            if (Reference == "Luk 24:11")
            {
                int x = 0;
            }
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
            DictForm = dictForm;
            DictGloss = dictGloss;
            this.VarUsed = false;
            this.VarCorrected = false;

            bool useVar = false;
            if (!string.IsNullOrEmpty(meaningVar))
            {
                int varSourceIndex = meaningVar.IndexOf("in:");
                if (varSourceIndex > 0 && meaningVar.Substring(varSourceIndex+3).Contains("TR"))
                {
                    useVar = true;
                }
            }
            if (Properties.TargetBibles.Default.UseGrkMeaningVar && useVar)
             {
                // e.g. Rev.22.14#03=N(K)O … ποιοῦντες (T=poiountes) doing - G4160=V-PAP-NPM in: TR+Byz 
                //                string pattern = @"^([\u0300-\u03FF\u1F00-\u1FFF\s]+)\s+\([otOT]\=([a-zA-Z\u0100-\u01FF\u1F00-\u1FFF\s']+)\)\s+([a-zA-Z0-9\u2000-\u206F\s<>\[\]\-]+)\s+\-\s+(G[\d]*)\=([-a-zA-Z0-9]+)\s.*";//\s\-\s(G{\d.)\=([-a-zA-Z]+)\s";
                //string pattern = @"^([\u0300-\u03FF\u1F00-\u1FFF\s]+)\s+\([otOT]\=([a-zA-Z\u0100-\u01FF\u1F00-\u1FFF\s']+)\)\s+([a-zA-Z0-9\u2000-\u206F\s<>\[\]\-]+)\s+\-\s+([-a-zA-Z0-9=+\s]+)\s.*";//\s\-\s(G{\d.)\=([-a-zA-Z]+)\s";

                //meaningVar = "δεσμοῖς μου (T=desmois mou) my imprisonments - G1199=N-DPM + G0846|G3165«G3450=P-1GS in: TR+Byz";
                meaningVar = meaningVar.Replace("|", "?").Replace("«", "!");
                string pattern = @"^([\u0300-\u03FF\u1F00-\u1FFF\s]+)\s+\([otOT]\=([a-zA-Z\u0100-\u01FF\u1F00-\u1FFF\s']+)\)\s+([a-zA-Z0-9\u2000-\u206F\s<>\[\]\-]+)\s+\-\s+([-a-zA-Z0-9=+\?\!\s]+)\s.*";//\s\-\s(G{\d.)\=([-a-zA-Z]+)\s";
                Match match = Regex.Match(meaningVar, pattern);
                if(match.Success)
                {
                    this.Greek = match.Groups[1].Value;
                    this.Transliteration = match.Groups[2].Value;
                    this.Word = match.Groups[3].Value;
                    //this.RootStrong = match.Groups[4].Value;
                    //this.Morphology = match.Groups[5].Value;
                    this.RootStrong = string.Empty;
                    this.Morphology = string.Empty;
                    string[] strongMorphs = match.Groups[4].Value.Split('+');
                    foreach (string strongMorph in strongMorphs)
                    {
                        string[] parts = strongMorph.Trim().Split('=');
                        string temp = parts[0];
                        if (temp.Contains("?") || temp.Contains("!"))
                        {
                            int last = temp.LastIndexOf("G");
                            temp = temp.Substring(last);
                        }
                        RootStrong += temp + " ";
                        Morphology += parts[1] + "/";
                    }
                    RootStrong = RootStrong.Trim();
                    Morphology = Morphology.Trim('/');

                    this.DictForm = this.Greek;
                    this.DictGloss = this.Word;
                    this.Strong = new StrongsCluster();
                    this.Strong.Add(RootStrong);

                    this.VarUsed = true;
                }
                else
                {
                    string x = reference;
                }
            }
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
        public string DictForm { get; private set; }
        public string DictGloss { get; private set; }
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
        public bool VarUsed { get; private set; }
        public bool VarCorrected { get; set; }

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
                    int validCount = 0;
                    foreach (StrongsNumber number in Strong.Strongs)
                    {
                        if(!string.IsNullOrEmpty(number.ToStringD()))
                            validCount++;
                    }
                    foreach (StrongsNumber number in Strong.Strongs)
                    {
                        // check if we need to include this strong number
                        string strongNum = number.ToStringD();
                        bool include = !string.IsNullOrEmpty(strongNum);
                        if (include && validCount > 1)
                        {
                            if (Testament == BibleTestament.NT)
                                include = !Properties.OsisFileGeneration.Default.GreekTagsToExclude.Contains(strongNum);
                            else
                                include = !Properties.OsisFileGeneration.Default.HebrewTagsToExclude.Contains(strongNum);
                            if(!include) validCount--;
                        }

                        if (include)
                        {
                            if (Properties.OsisFileGeneration.Default.UseDisambiguatedStrong)
                                strongStr += string.Format(" strong:{0}", number.ToStringD());
                            else
                                strongStr += string.Format(" strong:{0}", number.ToStringS());
                        }
                    }
                    if (!string.IsNullOrEmpty(strongStr))
                    {
                        string w = Word;
                        char[] special = {'.','\'', '"', ',', '?', '’', '‘', '”', '“' }; 
                        string suffix = string.Empty;
                        string prefix = string.Empty;
                        while(true)
                        {
                                if (special.Contains(w[w.Length - 1]))
                                {
                                    suffix = w[w.Length - 1] + suffix;
                                    w = w.Substring(0, w.Length - 1);
                                }
                                else
                                    break;
                        }
                        while (true)
                        {
                            if (special.Contains(w[0]))
                            {
                                prefix += w[0];
                                w = w.Substring(1);
                            }
                            else
                                break;
                        }

                        result = string.Format("{0}<w lemma=\"{1}\">{2}</w>{3}", prefix, strongStr.Trim(), w, suffix);
                    }
                    else
                        result = string.Format("<w>{0}</w>", Word);
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
