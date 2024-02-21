using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil.BibleVersions
{
    internal class TranslatorTags
    {
        Dictionary<string, List<TranslatorWord>> otWords = new Dictionary<string, List<TranslatorWord>>();
        Dictionary<string, List<TranslatorWord>> ntWords = new Dictionary<string, List<TranslatorWord>>();


        List<string> otErrors = new List<string>();
        List<string> ntErrors = new List<string>();

        /// <summary>
        /// key: verse Ref
        /// Value: int array :  startWord, length, hebVerse#, hebStartWord, hebLength
        /// </summary>
        Dictionary<string, List<int[]>> otVerseMap = new Dictionary<string, List<int[]>>();
        Dictionary<string, List<int[]>> ntVerseMap = new Dictionary<string, List<int[]>>();

        public TranslatorTags()
        {
            #region OT verse Map
            string book = "Jdg";
            int ch = 15; int v = 2;
            string vRef = string.Format("{0} {1}:{2}", book, ch, v);
            otVerseMap.Add(vRef, new List<int[]>());
            //                              ch  v  s  l  v    s   l
            otVerseMap[vRef].Add(new int[] { ch, v, 0, 8, v - 1, 11, -1 });
            otVerseMap[vRef].Add(new int[] { ch, v, 8, -1, v, 0, -1 });

            //==============================================================
            book = "1Sa";
            ch = 19; v = 2;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, 6, v - 1, 11, -1 });
            otVerseMap[vRef].Add(new int[] { ch, v, 6, -1, v, 0, -1 });
            //==============================================================
            book = "1Ki";
            ch = 22; v = 21;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, 8, v, 0, -1 });
            otVerseMap[vRef].Add(new int[] { ch, v, 8, -1, v + 1, 0, 4 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 4, -1 });
            #endregion OT verse Map

            #region NT verse Map
            book = "1Jn";  // KJV
            ch = 2; v = 13;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 14, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 14, -1, v + 1, 0, 7 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 7, -1 });
            //==============================================================
            book = "Rev"; // KJV
            ch = 2; v = 27;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 7, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 7, -1, v + 1, 0, 7 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 7, -1 });
            //==============================================================
            book = "Mat"; // Others
            ch = 15; v = 5;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 12, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 12, -1, v + 1, 0, 11 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 11, -1 });
            //==============================================================
            book = "Mat"; // KJV
            ch = 20; v = 4;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 11, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 11, -1, v + 1, 0, 3 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 3, -1 });
            //==============================================================
            book = "Mar";
            ch = 3; v = 19; // KJV
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 4, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 4, -1, v + 1, 0, 4});

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 4, -1 });
            //==============================================================
            book = "Mar"; // ??
            ch = 6; v = 28;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, 4, v-1, 12, -1 });
            otVerseMap[vRef].Add(new int[] { ch, v, 4, -1, v, 0, -1});
            //==============================================================
            book = "Mar"; // NA
            ch = 12; v = 14;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 28, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 28, -1, v + 1, 0, 4 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 4, -1 });
            //==============================================================
            book = "Luk"; // KJV
            ch = 6; v = 17;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 20, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 20, -1, v + 1, 0, 10 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 10, -1 });
            //==============================================================
            book = "Luk"; // KJV
            ch = 7; v = 19;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 4, v-1, 9, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 4, -1, v, 0, -1 });
            //==============================================================
            book = "Act"; // KJV
            ch = 2; v = 10;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 10, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 10, -1, v + 1, 0, 4});

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 4, -1 });
            //==============================================================
            book = "Act";
            ch = 3; v = 19; // KJV
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 4, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 4, -1, v + 1, 0, 9});

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 9, -1 });
            //==============================================================
            book = "Act"; // KJV
            ch = 5; v = 40;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 2, v - 1, 13, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 2, -1, v, 0, -1 });
            //==============================================================
            book = "Act";  // ???
            ch = 11; v = 25;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 7, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 7, -1, v + 1, 0, 7 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 7, -1 });
            //==============================================================
            book = "Act"; // ???
            ch = 24; v = 3;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 10, v - 1, 8, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 10, -1, v, 0, -1 });
            //==============================================================
            book = "Act";  // KJV
            ch = 24; v = 18;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 12, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 12, -1, v + 1, 0, 6});

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 6, -1 });
            //==============================================================
            book = "Rom";  // KJV
            ch = 7; v = 9;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 12, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 12, -1, v + 1, 0, 3 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 3, -1 });
            //==============================================================
            book = "Rom";  // KJV
            ch = 9; v = 11;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 16, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 16, -1, v + 1, 0, 7 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 7, -1 });
            //==============================================================
            book = "2Co";  // KJV
            ch = 8; v = 14;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 3, v - 1, 8, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 3, -1, v, 0, -1 });
            //==============================================================
            book = "2Co";  // KJV
            ch = 10; v = 5;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 2, v - 1, 15, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 2, -1, v, 0, -1 });
            //==============================================================
            book = "2Co";  // ???
            ch = 11; v = 8;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 7, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 7, -1, v + 1, 0, 9 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 9, -1 });
            //==============================================================
            book = "2Co";  // KJV
            ch = 13; v = 13;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v - 1, 5, -1 });
            //==============================================================
            book = "2Co";  // KJV
            ch = 13; v = 14;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v - 1, 0, -1 });
            //==============================================================
            book = "Gal";  // KJV
            ch = 2; v = 20;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 2, v - 1, 9, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 2, -1, v, 0, -1 });
            //==============================================================
            book = "Eph";  // KJV ???
            ch = 2; v = 15;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 2, v - 1, 17, 2 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 2, 1, v, 6, 1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 3, 1, v-1, 19, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 4, 4, v, 0, 6 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 8, -1, v,7, -1 });
            //==============================================================
            book = "Eph";  // ???
            ch = 3; v = 18;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 4, v - 1, 10, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 4, -1, v, 0, -1 });
            //==============================================================
            book = "Eph";  // KJV
            ch = 5; v = 13;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 5, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 5, -1, v + 1, 0, 6 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 6, -1 });
            //==============================================================
            book = "Phi";  // KJV
            ch = 1; v = 16;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v + 1, 0, -1 });
            //==============================================================
            book = "Phi";  // KJV
            ch = 1; v = 17;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v - 1, 0, -1 });
            //==============================================================
            book = "Phi";  // KJV
            ch = 2; v = 8;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 3, v - 1, 10, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 3, -1, v, 0, -1 });
            //==============================================================
            book = "Col";  // KJV
            ch = 5; v = 13;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            ntVerseMap.Add(vRef, new List<int[]>());
            ntVerseMap[vRef].Add(new int[] { ch, v, 0, 9, v, 0, -1 });
            ntVerseMap[vRef].Add(new int[] { ch, v, 9, -1, v + 1, 0, 3 });

            v += 1;
            vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
            otVerseMap.Add(vRef, new List<int[]>());
            otVerseMap[vRef].Add(new int[] { ch, v, 0, -1, v, 3, -1 });

            #endregion NT verse Map

        }

        public void Export(BibleVersion target, ReferenceVersionTOTHT totht, ReferenceVersionTAGNT tagnt, bool publicDomain = false)
        {
            Prepare(target, totht, tagnt, publicDomain);
        }

        private void Prepare(BibleVersion target, ReferenceVersionTOTHT totht, ReferenceVersionTAGNT tagnt, bool publicDomain)
        {

            foreach ((string verseRef, Verse verse) in target.Bible)
            {
                string book = string.Empty;

                int idx = verseRef.IndexOf(' ');
                if (idx != -1)
                {
                    book = verseRef.Substring(0, idx);
                }

                int bookIndex = target.GetBookIndex(book);

                if (bookIndex < 39)
                {
                    // OT
                    ProcessHebrewVerse(verse, bookIndex, book, verseRef, totht, publicDomain);
                }
                else
                {
                    // NT
                    ProcessGreekVerse(verse, bookIndex, book, verseRef, tagnt, publicDomain);
                }
                string x = verseRef;
                Verse y = verse;

            }

            if (otErrors.Count > 0)
            {
                OutputErrors(@"C:\temp\SVD_OT_Errors.txt", otErrors);
                MessageBox.Show("OT Errors Found");
            }
            else
            {
                OutputTranslatorTagsOT(@"C:\temp\TTAraSVD - Translators Tags etc. for Arabic SVD OT - STEPBible.org CC BY.txt", otWords, publicDomain);
                //OutputTranslatorTags(@"C:\temp\TTESV - Translators Tags for ESV.txt", otWords);
                MessageBox.Show("OT Success");
            }

            if (ntErrors.Count > 0)
            {
                OutputErrors(@"C:\temp\SVD_NT_Errors.txt", ntErrors);
                MessageBox.Show("NT Errors Found");
            }
            else
            {
                OutputTranslatorTagsNT(@"C:\temp\TTAraSVD - Translators Tags etc. for Arabic SVD NT - STEPBible.org CC BY.txt", ntWords);
                //OutputTranslatorTags(@"C:\temp\TTESV - Translators Tags for ESV.txt", otWords);
                MessageBox.Show("NT Success");
            }


        }

        private void ProcessHebrewVerse(Verse verse, int bookIndex, string book, string verseRef, ReferenceVersionTOTHT totht, bool publicDomain)
        {
            string bookName = totht.GetBookNameFromIndex(bookIndex);
            string originalRef = verseRef.Replace(book, bookName);
            int colon = originalRef.IndexOf(':');
            string verseNum = originalRef.Substring(colon);
            #region Psalms Title
            if (verseRef == "Psa 18:1")
            {
                int a = 0;
            }
            if (verse.HasPsalmTitle && verseNum == ":1")
            {
                string originalPrevRef = originalRef.Replace(verseNum, ":0");
                Verse title = new Verse();
                int indx = 0;
                while (true)
                {
                    if (verse[indx].Word.Trim() == "*")
                        break;
                    title[indx] = verse[indx];
                    indx++;
                }
                if (totht.Bible.ContainsKey(originalPrevRef))
                {
                    // get the Hebrew verse
                    Verse hebrewVerse = totht.Bible[originalPrevRef];
                    ProcessVerseOT(verseRef, originalRef, title, 0, title.Count, hebrewVerse); ;
                }
                if (totht.Bible.ContainsKey(originalRef))
                {
                    // get the Hebrew verse
                    Verse hebrewVerse = totht.Bible[originalRef];
                    ProcessVerseOT(verseRef, originalRef, verse, indx + 1, verse.Count, hebrewVerse); ;
                }
            }
            #endregion Psalms Title

            else if (otVerseMap.ContainsKey(verseRef))
            {
                int len = 0;
                try
                {
                    foreach (int[] map in otVerseMap[verseRef])
                    {
                        //  ch  v  s  l  v  s    
                        int chapterNum = map[0];
                        int verseNumbr = map[1];
                        int start = map[2];
                        if (map[3] == -1)
                            len = verse.Count;
                        else
                            len += map[3];
                        int mapVerse = map[4];
                        int mapStart = map[5];
                        int mapLen = map[6];

                        int sp = verseRef.IndexOf(' ');
                        string bk = verseRef.Substring(0, sp);
                        string hebRef = string.Format("{0} {1}:{2}", bk, chapterNum, mapVerse);

                        // get the Hebrew verse
                        Verse hebrewVerse = totht.Bible[hebRef];
                        Verse hebrewVerseSec = hebrewVerse.SubVerse(mapStart, mapLen);
                        ProcessVerseOT(verseRef, originalRef, verse, start, len, hebrewVerseSec);
                    }

                }
                catch (Exception ex)
                {
                    var cm = System.Reflection.MethodBase.GetCurrentMethod();
                    var name = cm.DeclaringType.FullName + "." + cm.Name;
                    Tracing.TraceException(name, ex.Message);
                }
            }
            else if (totht.Bible.ContainsKey(originalRef))
            {
                // get the Hebrew verse
                Verse hebrewVerse = totht.Bible[originalRef];
                ProcessVerseOT(verseRef, originalRef, verse, 0, verse.Count, hebrewVerse); ;
            }
        }


        private void ProcessGreekVerse(Verse verse, int bookIndex, string book, string verseRef, ReferenceVersionTAGNT tagnt, bool publicDomain)
        {
            string bookName = tagnt.GetBookNameFromIndex(bookIndex);
            string originalRef = verseRef.Replace(book, bookName);
            int colon = originalRef.IndexOf(':');
            string verseNum = originalRef.Substring(colon);
            
            if (tagnt.Bible.ContainsKey(originalRef))
            {
                // get the Hebrew verse
                Verse greekVerse = tagnt.Bible[originalRef];
                ProcessVerseNT(verseRef, originalRef, verse, 0, verse.Count, greekVerse); ;
            }
        }

        private void ProcessVerseOT(string verseRef, string originalRef, Verse verse, int startIndex, int end, Verse hebrewVerse)
        {
            Dictionary<string, int> usedStrongs = new Dictionary<string, int>();

            List<TranslatorWord> newVerse = new List<TranslatorWord>();

            if (originalRef == "Gen 2:5")
            {
                //break;
            }

            // key: arabic word index
            // val: hebrew word index
            Dictionary<int, int> map = new Dictionary<int, int>();

            if (verseRef == "Gen 7:4")
            {
                int d = 0;
            }
            for (int i = startIndex; i < end; i++)
            {
                List<OriginalWordDetails> ow = new List<OriginalWordDetails>();

                if (verse[i].Strong == null || verse[i].Strong.Length == 0 ||
                    (verse[i].Strong.Length == 1 && string.IsNullOrEmpty(verse[i].Strong[0])))
                {
                    ow = null;
                }
                else
                {
                    foreach (string strong in verse[i].Strong)
                    {
                        int offset = 0;
                        if (usedStrongs.Keys.Contains(strong))
                            offset = usedStrongs[strong];

                        VerseWord hw = hebrewVerse.GetWordFromStrong(strong, offset);
                        if (hw == null)
                        {
                            otErrors.Add(string.Format("{0}\t{1}-{2}\t{3}",
                               verseRef,
                               verse[i].WordIndex,
                               verse[i].Word,
                               strong));
                        }
                        else
                        {
                            usedStrongs[strong] = hw.WordIndex + 1;
                            map[i] = hw.WordIndex;
                            OriginalWordDetails wd = new OriginalWordDetails(hw.RootStrong, hw.Morphology, hw.Hebrew, hw.WordIndex, hw.Transliteration, hw.Word, hw.WordNumber, hw.AltVerseNumber, hw.Reference);
                            ow.Add(wd);
                        }
                    }
                }
                TranslatorWord aw = new TranslatorWord(verse[i].Word, ow);
                aw.ArabicWordIndex = newVerse.Count;
                newVerse.Add(aw);
            }
            SortedDictionary<int, int> missedWords = new SortedDictionary<int, int>();
            for (int i = 0; i < hebrewVerse.Count; i++)
            {
                if (map.Values.Contains(i)) continue;
                int indexA = map.FirstOrDefault(x => x.Value == i + 1).Key;
                missedWords[indexA] = i;
            }

            List<TranslatorWord> updatedVerse = new List<TranslatorWord>();
            int s = 0;
            foreach (int p in missedWords.Keys)
            {
                int i = 0;
                try
                {
                    for (i = s; i <= p; i++)
                    {
                        updatedVerse.Add(newVerse[i]);
                    }
                }
                catch (Exception ex)
                {
                    int t = 0;
                }

                s = i;
                VerseWord hw = hebrewVerse[missedWords[p]];
                List<OriginalWordDetails> ow = new List<OriginalWordDetails>();
                ow.Add(new OriginalWordDetails(hw.RootStrong, hw.Morphology, hw.Hebrew, hw.WordIndex, hw.Transliteration, hw.Word, hw.WordNumber, hw.AltVerseNumber, hw.Reference));
                updatedVerse.Add(new TranslatorWord(string.Empty, ow));
            }
            for (int i = s; i < newVerse.Count; i++)
                updatedVerse.Add(newVerse[i]);

            if(otWords.ContainsKey(originalRef))
            {
                int offset = 0;
                // find last Arabic word index
                for(int i = otWords[originalRef].Count -1; i >= 0; i-- )
                {
                    TranslatorWord word = otWords[originalRef][i];
                    if (string.IsNullOrEmpty(word.Word)) continue;
                    offset = word.ArabicWordIndex + 1;
                    break;
                }
                //append this verse to previous verse
                foreach(TranslatorWord w in updatedVerse)
                {
                    w.ArabicWordIndex += offset;
                    otWords[originalRef].Add(w);
                }
            }
            else
                otWords[originalRef] = updatedVerse;
        }
        private void ProcessVerseNT(string verseRef, string originalRef, Verse verse, int startIndex, int end, Verse hebrewVerse)
        {
            Dictionary<string, int> usedStrongs = new Dictionary<string, int>();

            List<TranslatorWord> newVerse = new List<TranslatorWord>();

            if (originalRef == "Gen 2:5")
            {
                //break;
            }

            // key: arabic word index
            // val: hebrew word index
            Dictionary<int, int> map = new Dictionary<int, int>();

            if (verseRef == "Gen 7:4")
            {
                int d = 0;
            }
            for (int i = startIndex; i < end; i++)
            {
                List<OriginalWordDetails> ow = new List<OriginalWordDetails>();

                if (verse[i].Strong == null || verse[i].Strong.Length == 0 ||
                    (verse[i].Strong.Length == 1 && string.IsNullOrEmpty(verse[i].Strong[0])))
                {
                    ow = null;
                }
                else
                {
                    foreach (string strong in verse[i].Strong)
                    {
                        int offset = 0;
                        if (usedStrongs.Keys.Contains(strong))
                            offset = usedStrongs[strong];

                        VerseWord hw = hebrewVerse.GetWordFromStrong(strong, offset);
                        if (hw == null)
                        {
                            ntErrors.Add(string.Format("{0}\t{1}-{2}\t{3}",
                               verseRef,
                               verse[i].WordIndex + 1,
                               verse[i].Word,
                               strong));
                        }
                        else
                        {
                            usedStrongs[strong] = hw.WordIndex + 1;
                            map[i] = hw.WordIndex;
                            OriginalWordDetails wd = new OriginalWordDetails(hw.RootStrong, hw.Morphology, hw.Hebrew, hw.WordIndex, hw.Transliteration, hw.Word, hw.WordNumber, hw.AltVerseNumber, hw.Reference);
                            ow.Add(wd);
                        }
                    }
                }
                TranslatorWord aw = new TranslatorWord(verse[i].Word, ow);
                aw.ArabicWordIndex = newVerse.Count;
                newVerse.Add(aw);
            }
            SortedDictionary<int, int> missedWords = new SortedDictionary<int, int>();
            for (int i = 0; i < hebrewVerse.Count; i++)
            {
                if (map.Values.Contains(i)) continue;
                int indexA = map.FirstOrDefault(x => x.Value == i + 1).Key;
                missedWords[indexA] = i;
            }

            List<TranslatorWord> updatedVerse = new List<TranslatorWord>();
            int s = 0;
            foreach (int p in missedWords.Keys)
            {
                int i = 0;
                try
                {
                    for (i = s; i <= p; i++)
                    {
                        updatedVerse.Add(newVerse[i]);
                    }
                }
                catch (Exception ex)
                {
                    int t = 0;
                }

                s = i;
                VerseWord hw = hebrewVerse[missedWords[p]];
                List<OriginalWordDetails> ow = new List<OriginalWordDetails>();
                ow.Add(new OriginalWordDetails(hw.RootStrong, hw.Morphology, hw.Hebrew, hw.WordIndex, hw.Transliteration, hw.Word, hw.WordNumber, hw.AltVerseNumber, hw.Reference));
                updatedVerse.Add(new TranslatorWord(string.Empty, ow));
            }
            for (int i = s; i < newVerse.Count; i++)
                updatedVerse.Add(newVerse[i]);

            if(ntWords.ContainsKey(originalRef))
            {
                int offset = 0;
                // find last Arabic word index
                for(int i = ntWords[originalRef].Count -1; i >= 0; i-- )
                {
                    TranslatorWord word = ntWords[originalRef][i];
                    if (string.IsNullOrEmpty(word.Word)) continue;
                    offset = word.ArabicWordIndex + 1;
                    break;
                }
                //append this verse to previous verse
                foreach(TranslatorWord w in updatedVerse)
                {
                    w.ArabicWordIndex += offset;
                    ntWords[originalRef].Add(w);
                }
            }
            else
                ntWords[originalRef] = updatedVerse;
        }
        private void OutputErrors(string errorFile, List<string> errors)
        {
            using (StreamWriter sw = new StreamWriter(errorFile, false))
            {
                int i = 0;
                while (true)
                {
                    if (i >= errors.Count) break;

                    sw.Write(errors[i]);
                    sw.Write("\t");
                    if (i + 50 < errors.Count) sw.Write(errors[i + 50]);
                    sw.Write("\t");
                    if (i + 100 < errors.Count) sw.Write(errors[i + 100]);
                    sw.WriteLine();
                    i++;
                    if (i % 50 == 0)
                    {
                        i += 100;
                        sw.WriteLine();
                    }
                }
            }
        }
        private void OutputTranslatorTagsOT(string destination, Dictionary<string, List<TranslatorWord>> words, bool publicDomain)
        {
            using (StreamWriter sw = new StreamWriter(destination, false))
            {
                if(publicDomain)
                    WritePreamble(sw);
                foreach ((string verseRef, List<TranslatorWord> verseWords) in words)
                {
                    if (verseRef == "1Ki 22:21")
                    {
                        int z = 0;
                    }
                    try
                    {
                        sw.WriteLine(verseRef.Replace(" ", ".").Replace(":", "."));
                        sw.WriteLine("========");
                        sw.WriteLine("Ara W#\tArabic\tStrongs\tGrammar\tHeb w#\tHebrew\tTransliteration\tWord-by-word");
                        //sw.WriteLine("ESV W#\tArabic\tStrongs\tGrammar\tHeb w#\tHebrew\tTransliteration\tWord-by-word");
                        int arabicIndexAdjust = 1;
                        foreach (TranslatorWord word in verseWords)
                        {
                            string araWordNo = string.Empty;
                            if (!string.IsNullOrEmpty(word.Word))
                            {
                                string[] parts = word.Word.Split(new char[] { ' ' });
                                if (parts.Length == 1)
                                {
                                    araWordNo = string.Format("#{0:d2}", word.ArabicWordIndex + arabicIndexAdjust);
                                }
                                else
                                {
                                    araWordNo = string.Format("#{0:d2} - #{1:d2}", word.ArabicWordIndex + arabicIndexAdjust,
                                                                       word.ArabicWordIndex + arabicIndexAdjust + parts.Length - 1);
                                    arabicIndexAdjust += (parts.Length - 1);
                                }
                            }

                            string line = string.Empty;
                            if (word.OtiginalWords == null)
                            {
                                line = string.Format("{0}\t{1}\t\t\t\t\t\t",
                                        araWordNo,
                                        word.Word);

                            }
                            else
                            {
                                // concatenate all Hebrew words
                                string Strongs = string.Empty;
                                string Morphology = string.Empty;
                                string AncientWordVerse = string.Empty;
                                string AncientWordNumber = string.Empty;
                                string AncientWord = string.Empty;
                                string Transliteration = string.Empty;
                                string WordByWord = string.Empty;

                                foreach (OriginalWordDetails owd in word.OtiginalWords)
                                {
                                    string ancientVerseNumber = owd.AncientWordVerse; 
                                    // if empty, and verse number is different from current verse number
                                    // construct verse number
                                    string chV = verseRef.Substring(verseRef.IndexOf(' ') + 1).Replace(":", ".");
                                    if (string.IsNullOrEmpty(ancientVerseNumber))
                                    {
                                        string hebChV = owd.AncientWordReference;
                                        hebChV = hebChV.Substring(verseRef.IndexOf(' ') + 1).Replace(":", ".");
                                        if (hebChV != chV)
                                            ancientVerseNumber = string.Format("({0})", hebChV);
                                    }
                                    else
                                    {
                                        if(ancientVerseNumber.Trim(new char[] {'(',')'}) == chV)
                                                ancientVerseNumber = string.Empty;
                                    }

                                    Strongs += owd.Strongs + "; ";
                                    Morphology += owd.Morphology + "; ";
                                    AncientWordVerse += ancientVerseNumber + "; ";
                                    AncientWordNumber += owd.AncientWordNumber + "; ";
                                    AncientWord += owd.AncientWord + "; ";
                                    Transliteration += owd.Transliteration + "; ";
                                    WordByWord += owd.WordByWord + "; ";
                                }

                                line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}#{5}\t{6}\t{7}\t{8}",
                                    araWordNo,
                                    word.Word,
                                    Strongs.TrimEnd(new char[] {';', ' ' }),
                                    Morphology.TrimEnd(new char[] { ';', ' ' }),
                                    AncientWordVerse.TrimEnd(new char[] { ';', ' ' }),
                                    AncientWordNumber.TrimEnd(new char[] { ';', ' ' }),
                                    AncientWord.TrimEnd(new char[] { ';', ' ' }),
                                    Transliteration.TrimEnd(new char[] { ';', ' ' }),
                                    WordByWord.TrimEnd(new char[] { ';', ' ' }));
                            }
                            if(publicDomain)
                                sw.WriteLine(Utils.RemoveDiacritics(line));
                            else
                                sw.WriteLine(line);
                        }
                        sw.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        var cm = System.Reflection.MethodBase.GetCurrentMethod();
                        var name = cm.DeclaringType.FullName + "." + cm.Name;
                        Tracing.TraceException(name, ex.Message);
                    }
                }
            }
        }

        private void WritePreamble(StreamWriter sw)
        {
            string preamble = Properties.Resources.TTArabicSVDPreamble;
            string created = string.Format("Created {0} {1}.", 
                DateTime.Now.ToString("yyyy-MM-dd"), 
                ConfigurationHolder.Instance.OSIS[OsisConstants.creator_name]); 
            preamble = preamble.Replace("Created", created);
            sw.WriteLine(preamble);
        }

        private void OutputTranslatorTagsNT(string destination, Dictionary<string, List<TranslatorWord>> words)
        {
            using (StreamWriter sw = new StreamWriter(destination, false))
            {
                foreach ((string verseRef, List<TranslatorWord> verseWords) in words)
                {
                    if (verseRef == "Psa 3:2")
                    {
                        int z = 0;
                    }
                    try
                    {
                        sw.WriteLine(verseRef.Replace(" ", ".").Replace(":", "."));
                        sw.WriteLine("========");
                        sw.WriteLine("Ara W#\tArabic\tStrongs\tGrammar\tHeb w#\tHebrew\tTransliteration\tWord-by-word");
                        //sw.WriteLine("ESV W#\tArabic\tStrongs\tGrammar\tHeb w#\tHebrew\tTransliteration\tWord-by-word");
                        int arabicIndexAdjust = 1;
                        foreach (TranslatorWord word in verseWords)
                        {
                            string araWordNo = string.Empty;
                            if (!string.IsNullOrEmpty(word.Word))
                            {
                                string[] parts = word.Word.Split(new char[] { ' ' });
                                if (parts.Length == 1)
                                {
                                    araWordNo = string.Format("#{0:d2}", word.ArabicWordIndex + arabicIndexAdjust);
                                }
                                else
                                {
                                    araWordNo = string.Format("#{0:d2} - #{1:d2}", word.ArabicWordIndex + arabicIndexAdjust,
                                                                       word.ArabicWordIndex + arabicIndexAdjust + parts.Length - 1);
                                    arabicIndexAdjust += (parts.Length - 1);
                                }
                            }

                            string line = string.Empty;
                            if (word.OtiginalWords == null)
                            {
                                line = string.Format("{0}\t{1}\t\t\t\t\t\t",
                                        araWordNo,
                                        word.Word);

                            }
                            else
                            {
                                // concatenate all Hebrew words
                                string Strongs = string.Empty;
                                string Morphology = string.Empty;
                                string AncientWordVerse = string.Empty;
                                string AncientWordNumber = string.Empty;
                                string AncientWord = string.Empty;
                                string Transliteration = string.Empty;
                                string WordByWord = string.Empty;

                                foreach (OriginalWordDetails owd in word.OtiginalWords)
                                {
                                    string ancientVerseNumber = owd.AncientWordVerse;
                                    // if empty, and verse number is different from current verse number
                                    // construct verse number
                                    string chV = verseRef.Substring(verseRef.IndexOf(' ') + 1).Replace(":", ".");
                                    if (string.IsNullOrEmpty(ancientVerseNumber))
                                    {
                                        string hebChV = owd.AncientWordReference;
                                        hebChV = hebChV.Substring(verseRef.IndexOf(' ') + 1).Replace(":", ".");
                                        if (hebChV != chV)
                                            ancientVerseNumber = string.Format("({0})", hebChV);
                                    }
                                    else
                                    {
                                        if (ancientVerseNumber.Trim(new char[] { '(', ')' }) == chV)
                                            ancientVerseNumber = string.Empty;
                                    }

                                    Strongs += owd.Strongs + "; ";
                                    Morphology += owd.Morphology + "; ";
                                    AncientWordVerse += ancientVerseNumber + "; ";
                                    AncientWordNumber += owd.AncientWordNumber + "; ";
                                    AncientWord += owd.AncientWord + "; ";
                                    Transliteration += owd.Transliteration + "; ";
                                    WordByWord += owd.WordByWord + "; ";
                                }

                                line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}#{5}\t{6}\t{7}\t{8}",
                                    araWordNo,
                                    word.Word,
                                    Strongs.TrimEnd(new char[] { ';', ' ' }),
                                    Morphology.TrimEnd(new char[] { ';', ' ' }),
                                    AncientWordVerse.TrimEnd(new char[] { ';', ' ' }),
                                    AncientWordNumber.TrimEnd(new char[] { ';', ' ' }),
                                    AncientWord.TrimEnd(new char[] { ';', ' ' }),
                                    Transliteration.TrimEnd(new char[] { ';', ' ' }),
                                    WordByWord);
                            }
                            sw.WriteLine(line);
                        }
                        sw.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        var cm = System.Reflection.MethodBase.GetCurrentMethod();
                        var name = cm.DeclaringType.FullName + "." + cm.Name;
                        Tracing.TraceException(name, ex.Message);
                    }
                }
            }
        }
    }


    internal class OriginalWordDetails
    {
        public OriginalWordDetails(string strongs, string morphology, string ancientWord, int ancientWordIndex, string transliteration, string wordbyWord, string ancientWordNumber, string ancientWordVerse, string ancientWordReference)
        {
            AncientWordIndex = ancientWordIndex;
            Strongs = strongs;
            Morphology = morphology;
            AncientWord = ancientWord;
            Transliteration = transliteration;
            WordByWord = wordbyWord;
            AncientWordNumber = ancientWordNumber;
            AncientWordVerse = ancientWordVerse;
            AncientWordReference = ancientWordReference;
        }

        public int AncientWordIndex { get; }
        public string AncientWordReference { get; }
        public string AncientWordVerse{ get; }
        public string AncientWordNumber{ get; }
        public string Strongs { get; }
        public string Morphology { get; }
        public string AncientWord { get; }
        public string Transliteration { get; }
        public string WordByWord { get; }
    }

    internal class TranslatorWord
    {
        public TranslatorWord(string word, List<OriginalWordDetails> otiginalWords)
        {
            Word = word;
            OtiginalWords = otiginalWords;
        }

        public string Word { get; }
        public List<OriginalWordDetails> OtiginalWords { get; }
        public int ArabicWordIndex { get; set; }

    }
}
