using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
