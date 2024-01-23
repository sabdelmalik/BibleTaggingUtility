using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BibleTaggingUtil.OsisXml
{
    internal class OsisUtils
    {
        private static OsisUtils instance = null;
        private static readonly object lockObj = new object();

        public static OsisUtils Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new OsisUtils();
                        }
                    }
                }
                return instance;
            }
        }

        private OsisUtils()
        {
        }

        public List<OsisTag> GetOsisTags(XmlNode? root, string verseRef, int level=0)
        {
            List<OsisTag> osisTags = new List<OsisTag>();
            try
            {
                foreach (XmlNode node in root.ChildNodes)
                {
                    osisTags.Add(GetVerseTag(level, osisTags.Count, node, verseRef));
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);

            }

            return osisTags;
        }

        public List<string> GetStrongsFromLemma(string value)
        {
            List<string> strings = new List<string>();
            MatchCollection matches = Regex.Matches(value, @"strong:([GH]\d\d\d\d)");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    strings.Add(match.Groups[1].Value);
                }
            }

            return strings;
        }

        public OsisVerse? GetVersesTags(string book, string osisSegment, Match VerseMatch)
        {
            OsisVerse result = null;

            int startIndex = VerseMatch.Index + VerseMatch.Groups[0].Length;
            string sID = VerseMatch.Groups[1].Value;
            string eID = string.Empty;
            string verseRef = string.Format("{0}.{1}.{2}", book, VerseMatch.Groups[2].Value, VerseMatch.Groups[3].Value);
            string verseXML = string.Empty;
            int endIndex = 0;

            if (verseRef == "Gen.2.7")
            {
                int x = 0;
            }

            try
            {
                Regex regex = new Regex(string.Format(@"<verse\s*eID\=""{0}""\s*/>", sID));
                //                VerseMatch = regex.Match(osisDoc, startIndex);
                VerseMatch = regex.Match(osisSegment, startIndex);
                if (VerseMatch.Success)
                {
                    endIndex = VerseMatch.Index;
                    eID = VerseMatch.Groups[1].Value;
                }

                string verseXml = string.Empty;
                if (endIndex > startIndex)
                    verseXml = osisSegment.Substring(startIndex, endIndex - startIndex);
                //                    verseXml = osisDoc.Substring(startIndex, endIndex - startIndex);

                result = new OsisVerse(verseRef, startIndex, sID, verseXml, eID);
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }

            return result;
        }

        public string ChangeReferenceToLocalFormat(string VerseRef)
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
        private OsisTag GetVerseTag(int level, int index, XmlNode node, string verseRef)
        {
            VerseTagType tagType = VerseTagType.UNKNOWN;
            switch (node.Name)
            {
                case "#text":
                    tagType = VerseTagType.text;
                    break;
                case "w":
                    tagType = VerseTagType.w;
                    break;
                case "hi":
                    tagType = VerseTagType.hi;
                    break;
                case "q":
                    tagType = VerseTagType.q;
                    break;
                case "l":
                    tagType = VerseTagType.l;
                    break;
                case "divineName":
                    tagType = VerseTagType.divineName;
                    break;
                case "note":
                    tagType = VerseTagType.note;
                    break;
                case "milestone":
                    tagType = VerseTagType.milestone;
                    break;
                case "b":
                    tagType = VerseTagType.b;
                    break;
            }

            return new OsisTag(level, index, tagType, node, verseRef);

        }


    }
}
