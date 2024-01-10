using BibleTaggingUtil;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BibleTaggingUtil.OsisXml
{
    internal class OsisVerse
    {
        private const string zeroWidthSpace = "\u200B";
        List<OsisTag> osisTags = null;
        public OsisVerse(string verseRef, int startIndex, string sid, string verseXml, string  eid)
        {
            if(verseRef == "Ps.42.5")
            {
                int x = 0;
            }
            VerseRef = verseRef;
            StartIndex = startIndex;
            Length = verseXml.Length;
            sID = sid;
            VerseXml = verseXml;
            eID = eid;

            Dirty = false;

            BuildOsisTags();
        }

        private void BuildOsisTags()
        {
            string header = @"<?xml version=""1.0"" encoding=""UTF-8""?><osis>";
            string trailer = @"</osis>";

            // Gen.3.1
            VerseXml = TreatOddTags(VerseXml, "p");
            // Gen.2.23
            VerseXml = TreatOddTags(VerseXml, "lg");
            // 1Sam.25.1
            VerseXml = TreatOddTags(VerseXml, "div");
            // Ps.42.5 , 6
            VerseXml = TreatOddTags(VerseXml, "l");

            XmlDocument document = new XmlDocument();
            VerseXml = VerseXml.Replace("> <", ">" + zeroWidthSpace + "<");
            document.PreserveWhitespace = true;
            try
            {
                document.LoadXml(header + VerseXml + trailer);
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }


            osisTags = OsisUtils.Instance.GetOsisTags(document.DocumentElement, VerseRef);

        }

        public string VerseCompleteXml
        {
            get
            {
                string verseRebuild = string.Empty;
                foreach (OsisTag osisTag in osisTags)
                {
                    verseRebuild += osisTag.ToString();
                }
                verseRebuild = verseRebuild.Replace(">" + zeroWidthSpace + "<", "> <");

                return verseRebuild;
            }
        }

        private string TreatOddTags(string VerseXml, string tag)
        {
            string s = string.Format("<{0}>", tag);
            string e = string.Format("</{0}>", tag);
            string startAdded = string.Format("{0}$start${1}", s, e);
            string endAdded = string.Empty;

            int startIndex = 0;
            try
            {
                while (true)
                {
                    Regex regex = new Regex(@"(<" + tag + @"[^>]*>)");
                    if (tag == "div" || tag == "l")
                        regex = new Regex(string.Format(@"(<{0}\s[^>]+>)", tag));

                    int startP = -1;
                    Match match = regex.Match(VerseXml, startIndex);
                    if (match.Success)
                    {
                        startP = match.Index;
                        s = match.Groups[1].Value;
                    }
                    else
                        startP = VerseXml.IndexOf(s, startIndex);

                    endAdded = string.Format("{0}$end${1}", s, e);

                    int endP = VerseXml.IndexOf(e, startIndex);
                    if (startP < 0 && endP < 0) break;
                    if (startP < 0 && endP >= 0)
                    {
                        VerseXml = VerseXml.Remove(endP, e.Length).Insert(endP, startAdded); //Replace(e, startAdded);
                        startIndex = endP + startAdded.Length;
                        continue;
                    }
                    else if (startP >= 0 && endP < 0)
                    {
                        VerseXml = VerseXml.Remove(startP, s.Length).Insert(startP, endAdded); //Replace(s, endAdded);
                        startIndex = startP + endAdded.Length;
                        continue;
                    }
                    else if (startP < endP)
                    {
                        startIndex = endP + e.Length;
                        continue;
                    }
                    else if (startP > endP)
                    {
                        VerseXml = VerseXml.Remove(endP, e.Length).Insert(endP, startAdded); //Replace(e, startAdded);
                        startIndex = endP + startAdded.Length;
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
            return VerseXml;
        }

        internal Verse GetVerseWords()
        {
            Verse verse = new Verse();

            int idx = 0;
            foreach(OsisTag tag in osisTags)
            {
                List<VerseWord> words = tag.GetVerseWords();
                foreach(VerseWord word in words)
                {
                    verse[idx++] = word;
                }
            }

            return verse;
        }

        public string VerseRefX
        {
            get
            {
                string verseRefX = string.Empty;

                Match m = Regex.Match(VerseRef, @"([1-9A-Za-z]*)\.(\d{1,3})\.(\d{1,3})");
                if (m != null)
                {
                    string book = m.Groups[1].Value;
                    string chapter = m.Groups[2].Value;
                    string verse = m.Groups[3].Value;

                    verseRefX = string.Format("{0} {1}:{2}", book, chapter, verse);
                }
                return verseRefX;
            }
        }

        public void UpdateVerse(Verse verseWords)
        {
            for(int i = 0; i < verseWords.Count; i++)
            {
                osisTags[verseWords[i].OsisTagIndex].Update(verseWords[i]);
            }

        }
        public string VerseRef { get; }
        public int StartIndex { get; }
        public int Length { get; }
        public string sID { get; }
        public string VerseXml { get; private set; }
        public string eID { get; }
        public bool Dirty { get; set; }
    }
}
