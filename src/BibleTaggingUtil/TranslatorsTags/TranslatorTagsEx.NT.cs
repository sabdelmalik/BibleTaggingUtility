using BibleTaggingUtil.BibleVersions;
using BibleTaggingUtil.Strongs;
using BibleTaggingUtil.TranslatorsTags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil.TranslationTags
{
    public partial class TranslationTagsEx
    {
        private Dictionary<string, List<TranslatorWord>> ntWords = new Dictionary<string, List<TranslatorWord>>();
        private List<string> ntErrors = new List<string>();

        /// <summary>
        /// key: verse Ref
        /// Value: int array :  startWord, length, hebVerse#, hebStartWord, hebLength
        /// </summary>
        private Dictionary<string, List<int[]>> ntVerseMap;
        private void ProcessGreekVerse(Verse verse, int bookIndex, string book, string verseRef, ReferenceVersionTAGNT tagnt)
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
        private void ProcessVerseNT(string verseRef, string originalRef, Verse verse, int startIndex, int end, Verse hebrewVerse)
        {
            Dictionary<string, int> usedStrongs = new Dictionary<string, int>();

            List<TranslatorWord> newVerse = new List<TranslatorWord>();

            try
            {
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

                    if (verse[i].Strong == null || verse[i].Strong.Count == 0 ||
                        (verse[i].Strong.Count == 1 && verse[i].Strong[0].IsEmpty))
                    {
                        ow = null;
                    }
                    else
                    {
                        foreach (StrongsNumber strong in verse[i].Strong.Strongs)
                        {
                            int offset = 0;
                            if (usedStrongs.Keys.Contains(strong.ToString()))
                                offset = usedStrongs[strong.ToStringS()];

                            VerseWord hw = hebrewVerse.GetWordFromStrong(strong.ToStringS(), offset, false);
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
                                usedStrongs[strong.ToStringS()] = hw.WordIndex + 1;
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

                if (ntWords.ContainsKey(originalRef))
                {
                    int offset = 0;
                    // find last Arabic word index
                    for (int i = ntWords[originalRef].Count - 1; i >= 0; i--)
                    {
                        TranslatorWord word = ntWords[originalRef][i];
                        if (string.IsNullOrEmpty(word.Word)) continue;
                        offset = word.ArabicWordIndex + 1;
                        break;
                    }
                    //append this verse to previous verse
                    foreach (TranslatorWord w in updatedVerse)
                    {
                        w.ArabicWordIndex += offset;
                        ntWords[originalRef].Add(w);
                    }
                }
                else
                    ntWords[originalRef] = updatedVerse;
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
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



 }
