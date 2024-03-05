using BibleTaggingUtil;
using BibleTaggingUtil.Strongs;
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
    internal class OsisTag
    {
        private int level = 0;
        private int index = 0;
        private VerseTagType tagType;
        private XmlNode tagXml;
        private VerseWord? verseWord = null;
        private List<OsisTag> osisTags = new List<OsisTag>();

        public OsisTag(int level, int index, VerseTagType tagType, XmlNode tagXML, string verseRef)
        {
            this.tagType = tagType;
            tagXml = tagXML;
            this.level = level;
            this.index = index;
            string verseRefX = GetVerseRefX(verseRef);
            switch (tagType)
            {
                case VerseTagType.text:
                    CreateUntaggedWord(index, tagXML, verseRefX);
                    break;
                case VerseTagType.w:
                    CreateTaggedWord(index, tagXML, verseRefX);
                    break;
                case VerseTagType.note:
                case VerseTagType.milestone:
                case VerseTagType.b:
                case VerseTagType.UNKNOWN:
                    break;

                default:
                    {
                        osisTags = OsisUtils.Instance.GetOsisTags(tagXML, verseRef, ++level);
                        //foreach (OsisTag tag in osisTags)
                        //{
                        //    new OsisTag(++level, index, tag.tagType, tag.tagXml, verseRef); ;
                        //}
                    }
                    break;
            }
        }

        private string GetVerseRefX(string VerseRef)
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

        private void CreateUntaggedWord(int index, XmlNode tagXML, string verseRef)
        {
            verseWord = new VerseWord(tagXML.InnerText, new StrongsCluster(), verseRef);
            verseWord.OsisTagIndex = index;
            verseWord.OsisTagLevel = level;
        }
        private void CreateTaggedWord(int index, XmlNode tagXML, string verseRef)
        {
            string word = tagXML.InnerText;

            StrongsCluster strongs = new StrongsCluster();

            XmlAttributeCollection? attrib = tagXML.Attributes;
            if (attrib != null)
            {
                foreach (XmlAttribute attr in attrib)
                {
                    if (attr.Name == "lemma")
                    {
                        strongs = GetStrongs(attr.Value);
                    }
                }
            }

            verseWord = new VerseWord(tagXML.InnerText, strongs, verseRef);
            verseWord.OsisTagIndex = index;
            verseWord.OsisTagLevel = level;
        }

        private StrongsCluster GetStrongs(string value)
        {
            StrongsCluster strongs = new StrongsCluster();
            MatchCollection matches = Regex.Matches(value, @"strong:[GH](\d\d\d\d)");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    strongs.Add(match.Groups[1].Value);
                }
            }

            return strongs;
        }

        public List<VerseWord> GetVerseWords()
        {
            List<VerseWord> verseWords = new List<VerseWord>();
            try
            {
                switch (tagType)
                {
                    case VerseTagType.text:
                    case VerseTagType.w:
                        verseWords.Add(verseWord);
                        break;
                    default:
                        {
                            if (osisTags.Count > 0)
                            {
                                foreach (OsisTag tag in osisTags)
                                {
                                    List<VerseWord> verseWords1 = tag.GetVerseWords();
                                    if (verseWords1.Count > 0)
                                        verseWords.AddRange(verseWords1);
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
                throw;
            }

            return verseWords;
        }
        public override string ToString()
        {
            string result = string.Empty;
            try
            {
                switch (tagType)
                {
                    case VerseTagType.text:

                        if (verseWord != null)
                        {
                            if (verseWord.Strong.Count > 1 ||
                                (verseWord.Strong.Count == 1 && !(verseWord.Strong[0].IsEmpty)))
                            {
                                string lemmaValue = string.Empty;
                                foreach (StrongsNumber s in verseWord.Strong.Strongs)
                                {
                                    lemmaValue += string.Format("strong:{0} ", s.ToString());
                                }
                                lemmaValue = lemmaValue.Trim();
                                result = string.Format("<w lemma=\"{0}\">{1}</w>", lemmaValue, verseWord.Word);
                            }
                            else
                                result = verseWord.Word;
                        }

                        break;
                    case VerseTagType.w:
                        if (verseWord.Strong.Count == 0 ||
                            (verseWord.Strong.Count == 1 && verseWord.Strong[0].IsEmpty))
                        {
                            result = verseWord.Word;
                        }
                        else
                        {
                            string lemmaValue = string.Empty;
                            if (verseWord != null)
                            {
                                foreach (StrongsNumber s in verseWord.Strong.Strongs)
                                {
                                    lemmaValue += string.Format("strong:{0} ", s.ToString());
                                }
                                lemmaValue = lemmaValue.Trim();
                                XmlNode tagXml1 = tagXml.Clone();
                                if (tagXml1 != null)
                                {
                                    ((XmlElement)tagXml1).SetAttribute("lemma", lemmaValue); // Set to new value.
                                    result = tagXml1.OuterXml;
                                }
                            }
                        }
                        break;
                    case VerseTagType.note:
                    case VerseTagType.milestone:
                    case VerseTagType.b:
                        result = tagXml.OuterXml;
                        break;

                    case VerseTagType.UNKNOWN:
                        string temp = tagXml.OuterXml;
                        string marker = tagXml.InnerXml;
                        int idx = temp.IndexOf(marker);
                        switch (marker)
                        {
                            case "$start$":
                                result = temp.Substring(idx + marker.Length);
                                break;
                            case "$end$":
                                result = temp.Substring(0, idx);
                                break;
                            default:
                                result = tagXml.OuterXml;
                                break;
                        }
                        break;

                    default:
                        {
                            string result1 = string.Empty;
                            if (osisTags.Count > 0)
                            {
                                foreach (OsisTag tag in osisTags)
                                {
                                    result1 += tag.ToString();
                                }
                                XmlNode tagXmlTemp = tagXml.Clone();
                                tagXmlTemp.InnerXml = result1;
                                result = tagXmlTemp.OuterXml;
                                if (tagType == VerseTagType.l)
                                {
                                    int i = result.IndexOf("$end$");
                                    if (i > 0)
                                        result = result.Remove(i, "$end$</l>".Length);
                                    i = result.IndexOf("$start$");
                                    if (i > 0)
                                    {
                                        int x = result.LastIndexOf("<l", i);
                                        result = result.Remove(x, i - x + "$start$".Length);
                                    }
                                }

                            }
                            else
                                result = tagXml.OuterXml;
                        }
                        break;
                }
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

        internal void Update(VerseWord verseWord)
        {
            // Currently we only update Strong's numbers
            try
            {
                if (level == verseWord.OsisTagLevel)
                {
                    VerseWord w;
                    if (verseWord != null) w = verseWord;
                    else
                    {
                        w = osisTags[verseWord.OsisTagIndex].verseWord;
                    }
                    w.Strong = verseWord.Strong;
                    if (tagType == VerseTagType.w)
                    {
                        if (w.Strong.Count == 0 ||
                            (w.Strong.Count == 1 && w.Strong[0].IsEmpty))
                            tagType = VerseTagType.text;
                    }
                    else if (tagType == VerseTagType.text)
                    {
                        if (w.Strong.Count > 1 ||
                            (w.Strong.Count == 1 && !(w.Strong[0].IsEmpty)))
                            tagType = VerseTagType.w;
                    }
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

        public XmlNode TagNode
        {
            get
            {
                return tagXml;
            }
        }


    }
}
