// Old Testament
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
        private Dictionary<string, List<TranslatorWord>> otWords = new Dictionary<string, List<TranslatorWord>>();
        private List<string> otErrors = new List<string>();
        private Dictionary<string, List<int>> otMissedWords = new Dictionary<string, List<int>>();

        /// <summary>
        /// key: verse Ref
        /// Value: int array :  startWord, length, hebVerse#, hebStartWord, hebLength
        /// </summary>
        private Dictionary<string, List<int[]>> otVerseMap;

        private void ProcessHebrewVerse(Verse verse, int bookIndex, string book, string verseRef, ReferenceVersionTAHOT tahot)
        {
            string bookName = tahot.GetBookNameFromIndex(bookIndex);
            string originalRef = verseRef.Replace(book, bookName);
            int colon = originalRef.IndexOf(':');
            string verseNum = originalRef.Substring(colon);
            if (verseRef == "1Sa 10:1")
            {
                int a = 0;
            }
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
                if (tahot.Bible.ContainsKey(originalPrevRef))
                {
                    // get the Hebrew verse
                    Verse hebrewVerse = tahot.Bible[originalPrevRef];
                    ProcessVerseOT(verseRef, originalRef, title, 0, title.Count, hebrewVerse); ;
                }
                if (tahot.Bible.ContainsKey(originalRef))
                {
                    // get the Hebrew verse
                    Verse hebrewVerse = tahot.Bible[originalRef];
                    ProcessVerseOT(verseRef, originalRef, verse, indx + 1, verse.Count - indx - 1, hebrewVerse); ;
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
                            len = verse.Count - start;
                        else
                            len = map[3];
                            //len += map[3];
                        int mapVerse = map[4];
                        int mapStart = map[5];
                        int mapLen = map[6];

                        int sp = verseRef.IndexOf(' ');
                        string bk = verseRef.Substring(0, sp);
                        string hebRef = string.Format("{0} {1}:{2}", bk, chapterNum, mapVerse);

                        // get the Hebrew verse
                        Verse hebrewVerse = tahot.Bible[hebRef];
                        Verse hebrewVerseSec = hebrewVerse.SubVerse(mapStart, mapLen);
                        if (map[3] == 0) // unmapped section of reconstructed words
                            ProcessVerseOT(verseRef, originalRef, verse, start, 0, hebrewVerseSec);
                        else
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
            else if (tahot.Bible.ContainsKey(originalRef))
            {
                // get the Hebrew verse
                Verse hebrewVerse = tahot.Bible[originalRef];
                ProcessVerseOT(verseRef, originalRef, verse, 0, verse.Count, hebrewVerse); ;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verseRef">target book ref: book ch:v</param>
        /// <param name="originalRef">original language reference: book ch:v</param>
        /// <param name="verse">complete target verse</param>
        /// <param name="startIndex">index in verse of starting word to map to hebrew </param>
        /// <param name="end">index in verse of last word + 1 to map to hebrew</param>
        /// <param name="hebrewVerse">segment of the Hebrew verse to map to</param>
        private void ProcessVerseOT(string verseRef, string originalRef, Verse verse, int startIndex, int count, Verse hebrewVerse)
        {
            Dictionary<string, int> usedStrongs = new Dictionary<string, int>();

            List<TranslatorWord> newVerse = new List<TranslatorWord>();

            if (originalRef == "1Sa 19:2")
            {
                int x = 0;
            }

            #region set repeated word indices and counts
            Dictionary<string, List<VerseWord>> temp = new Dictionary<string, List<VerseWord>>();
            for (int i = 0; i < hebrewVerse.Count; i++)
            {
                VerseWord vw1 = hebrewVerse[i];
                if (vw1.Strong.Count != 1) continue; // for now, handle only single strong's.

                string strongString = vw1.StrongStringS;
                if (temp.ContainsKey(strongString)) continue; // we already processed this number

                for (int j = i + 1; j < hebrewVerse.Count; j++)
                {
                    VerseWord vw2 = hebrewVerse[j];
                    if (vw2.Strong.Count != 1) continue; // for now, handle only single strong's.

                    if (strongString == vw2.StrongStringS)
                    {
                        if (temp.ContainsKey(strongString))
                        {
                            if (vw2.Strong[0].Occurance == 0)
                            {
                                vw2.Strong[0].Occurance = temp[strongString].Count + 1;
                            }
                            else if (vw2.Strong[0].Occurance != temp[strongString].Count + 1)
                            {
                                int w = 0;
                                vw2.Strong[0].Occurance = temp[strongString].Count + 1;
                            }

                            temp[strongString].Add(vw2);
                        }
                        else
                        {
                            // first match
                            temp[strongString] = new List<VerseWord>();
                            if (vw1.Strong[0].Occurance == 0)
                            {
                                vw1.Strong[0].Occurance = 1;
                            }
                            else if (vw1.Strong[0].Occurance != 1)
                            {
                                int w = 0;
                                vw1.Strong[0].Occurance = 1;
                            }
                            if (vw2.Strong[0].Occurance == 0)
                            {
                                vw2.Strong[0].Occurance = 2;
                            }
                            else if (vw2.Strong[0].Occurance != 2)
                            {
                                int w = 0;
                                vw2.Strong[0].Occurance = 2;
                            }
                            temp[strongString].Add(vw1);
                            temp[strongString].Add(vw2);
                        }
                    }
                }
            }
            foreach (List<VerseWord> list in temp.Values)
            {
                foreach (VerseWord w in list)
                {
                    w.Strong[0].CountInVerse = list.Count;
                }
            }
            #endregion set repeated word indices and counts

            try
            {

                // key: arabic word index
                // val: hebrew word index
                Dictionary<int, List<int>> arabic2HebrewMap = new Dictionary<int, List<int>>();

                int lastHebIndex = -1;
                for (int i = startIndex; i < startIndex + count; i++)
                {
                    List<OriginalWordDetails> ow = new List<OriginalWordDetails>();

                    if (verse[i].Strong == null || verse[i].Strong.Count == 0 ||
                        (verse[i].Strong.Count == 1 && verse[i].Strong[0].IsEmpty))
                    {
                        ow = null;
                    }
                    else
                    {
                        bool comb = false;
                        foreach (StrongsNumber strong in verse[i].Strong.Strongs)
                        {
                            // int offset = 0;
                            //                            if (usedStrongs.Keys.Contains(strong.ToStringS()))
                            //                                offset = usedStrongs[strong.ToStringS()];

                            //VerseWord hw = hebrewVerse.GetWordFromStrong(strong.ToStringS(), lastHebIndex, comb); // offset);
                            List<VerseWord> hwList = hebrewVerse.GetWordListFromStrong(strong);//, lastHebIndex, comb); // offset);
                            VerseWord hw = null;
                            if (hwList.Count == 0)
                            {
                                otErrors.Add(string.Format("Empty hwList:  {0}\t{1}-{2}\t{3}",
                                   verseRef,
                                   verse[i].WordIndex,
                                   verse[i].Word,
                                   strong));
                            }
                            if (hwList.Count > 1)
                            {

                                if (arabic2HebrewMap.Count == 0)
                                {
                                    hw = hwList[0];
                                }
                                else
                                {
                                    int lastAraWordIdx = arabic2HebrewMap.Keys.ToArray()[arabic2HebrewMap.Keys.Count - 1];
                                    lastHebIndex = 0;
                                    foreach (int hi in arabic2HebrewMap[lastAraWordIdx])
                                    {
                                        if (hi > lastHebIndex) lastHebIndex = hi;
                                    }


                                    List<VerseWord> unusedHebWds = new List<VerseWord>();
                                    bool found = false;
                                    for (int j = 0; j < hwList.Count; j++)
                                    {
                                        hw = hwList[j];
                                        foreach (List<int> ints in arabic2HebrewMap.Values)
                                        {
                                            if (ints.Contains(hw.WordIndex))
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                        if (!found)
                                        {
                                            unusedHebWds.Add(hw);
                                        }
                                        found = false;
                                    }

                                    // decide which Hebrew word to use.
                                    if (unusedHebWds.Count > 0)
                                    {
                                        // Start with the first occurrence
                                        hw = unusedHebWds[0];

                                        if (unusedHebWds.Count > 1)
                                        {
                                            // if אֹת֖ H0853 or H0176 א֚וֹ or H0413 אֶל choose closest to last
                                            if (hw.Strong.Strongs.Count == 1 &&
                                                (hw.Strong.Strongs[0].ToString().Contains("H0853")) ||
                                                (hw.Strong.Strongs[0].ToString().Contains("H0176")) ||
                                                 (hw.Strong.Strongs[0].ToString().Contains("H0413")))
                                            {
                                                int span = Math.Abs(lastHebIndex - hw.WordIndex);
                                                // get the first in the list
                                                // choose closest to last
                                                for (int c = 1; c < unusedHebWds.Count; c++)
                                                {
                                                    int sp = Math.Abs(lastHebIndex - unusedHebWds[c].WordIndex);
                                                    if (sp < span)
                                                    {
                                                        span = sp;
                                                        hw = unusedHebWds[c];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                /*
                                                                bool found = false;
                                                                for (int j = 0; j < hwList.Count; j++)
                                                                {
                                                                    hw = hwList[j];
                                                                    foreach (List<int> ints in map.Values)
                                                                    {
                                                                        if (ints.Contains(hw.WordIndex))
                                                                        {
                                                                            found = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (!found) break;
                                                                }
                                */
                            }
                            else
                            {
                                hw = hwList[0];
                            }
                            if (hw == null)
                            {
                                otErrors.Add(string.Format("hw is null: {0}\t{1}-{2}\t{3}",
                                   verseRef,
                                   verse[i].WordIndex,
                                   verse[i].Word,
                                   strong));

                            }
                            else
                            {
                                if (comb)
                                    lastHebIndex = Math.Min(lastHebIndex, hw.WordIndex);
                                else
                                    lastHebIndex = hw.WordIndex;

                                comb = true; // in case next strong is still within the same arabic word
                                usedStrongs[strong.ToStringS()] = hw.WordIndex + 1;
                                if (arabic2HebrewMap.ContainsKey(i))
                                {
                                    arabic2HebrewMap[i].Add(hw.WordIndex);
                                }
                                else
                                {
                                    arabic2HebrewMap[i] = new List<int> { hw.WordIndex };
                                }
                                OriginalWordDetails wd = new OriginalWordDetails(hw.RootStrong, hw.Morphology, hw.Hebrew, hw.WordIndex, hw.Transliteration, hw.Word, hw.WordNumber, hw.WordType, hw.AltVerseNumber, hw.Reference);
                                ow.Add(wd);
                            }
                        }
                    }
                    TranslatorWord aw = new TranslatorWord(verse[i].Word, ow);
                    aw.TargetWordIndex = newVerse.Count;
                    newVerse.Add(aw);
                }

                // Now map contains all Arabic word indices and corresponding Hebrew word indices
                // if a Hebrew index is missing
                // we find the arabic word preceding 

                SortedDictionary<int, int> missedWords = new SortedDictionary<int, int>();
                int lastHebrewIndex = -1;
                int indexOffset = 1;
                bool doContinue = false;

                Dictionary<int, int> reverseMap = new Dictionary<int, int>();
                Dictionary<int, List<int>> correctedMap = new Dictionary<int, List<int>>();

                // fill in the Arabic Words missing from map
                // Arabic words with no hebrew words, are assigned hebrew word numbers beyond the hebrewVerse.Count
                int missingArabicWords = 0;
                if (count > 0)
                {
                    //int arabicWordCount = count;
                    missingArabicWords = count - arabic2HebrewMap.Count;
                    if (missingArabicWords > 0)
                    {
                        int lastHebrewWord = hebrewVerse.Count - 1;
                        for (int i = startIndex; i < startIndex + count; i++)
                        {
                            if (arabic2HebrewMap.ContainsKey(i))
                            {
                                correctedMap[i] = arabic2HebrewMap[i];
                            }
                            else
                            {
                                correctedMap[i] = new List<int> { ++lastHebrewWord };
                            }
                        }
                    }
                    else
                    {
                        correctedMap = arabic2HebrewMap;
                    }
                }

                // initialise reverse map
                // making room at the end for the unmapped Arabic words
                for (int i = 0; i < (hebrewVerse.Count + missingArabicWords); i++)
                {
                    reverseMap[i] = -1;
                }

                int restoredAllowance = 100;  // alowance for restored Hebrew words (RHW) e.g. if 3 RHW follow word index 5, they will be numbered 501, 502, 503
                // assign arabic indices to hebrew indeces
                foreach ((int mapKey, List<int> mapValues) in correctedMap)
                {
                    foreach (int idx in mapValues)
                    {
                        reverseMap[idx] = (mapKey + 1) * restoredAllowance; // allow for in-between Hebrew and avoid 0 index
                    }
                }

                // fill in the gaps
                int lastAraIndex = 0;
                List<int> seenAraIndeces = new List<int>();
                foreach (int hIndex in reverseMap.Keys)
                {
                    if (reverseMap[hIndex] == -1)
                    {
                        reverseMap[hIndex] = ++lastAraIndex;
                    }
                    else
                    {
                        if (!seenAraIndeces.Contains(reverseMap[hIndex]))
                        {
                            // this code is to deal with an Arabic word with multiple Hebrew words with gaps in btween
                            lastAraIndex = reverseMap[hIndex];
                            seenAraIndeces.Add(lastAraIndex);
                        }
                    }
                }

                SortedDictionary<int, List<int>> newMap = new SortedDictionary<int, List<int>>();
                foreach ((int hIndex, int aIndex) in reverseMap)
                {
                    if (newMap.ContainsKey(aIndex))
                    {
                        newMap[aIndex].Add(hIndex);
                    }
                    else
                    {
                        if (hIndex < hebrewVerse.Count)
                            newMap[aIndex] = new List<int> { hIndex };
                        else
                            newMap[aIndex] = new List<int> { };
                    }
                }

                List<TranslatorWord> updatedVerse = new List<TranslatorWord>();
                foreach ((int aIndex, List<int> hIndex) in newMap)
                {
                    if ((aIndex % restoredAllowance) == 0)
                    {
                        int index = (aIndex / restoredAllowance) - 1;
                        int adjIndex = index - startIndex;
                        if (hIndex.Count > 0)
                            updatedVerse.Add(newVerse[adjIndex]);
                        else
                        {
                            TranslatorWord unmapped = new TranslatorWord(verse[index].Word, null);
                            unmapped.TargetWordIndex = adjIndex;
                            updatedVerse.Add(unmapped);
                        }
                    }
                    else
                    {
                        if (hIndex.Count != 1)
                        {
                            int n = 0;
                        }
                        VerseWord hw = hebrewVerse[hIndex[0]];
                        List<OriginalWordDetails> ow = new List<OriginalWordDetails>();
                        ow.Add(new OriginalWordDetails(hw.RootStrong, hw.Morphology, hw.Hebrew, hw.WordIndex, hw.Transliteration, hw.Word, hw.WordNumber, hw.WordType, hw.AltVerseNumber, hw.Reference));
                        updatedVerse.Add(new TranslatorWord(string.Empty, ow));

                    }
                }

                if (otWords.ContainsKey(originalRef))
                {
                    int offset = 0;
                    // find last Arabic word index
                    for (int i = otWords[originalRef].Count - 1; i >= 0; i--)
                    {
                        TranslatorWord word = otWords[originalRef][i];
                        if (string.IsNullOrEmpty(word.Word)) continue;
                        offset = word.TargetWordIndex + 1;
                        break;
                    }
                    //append this verse to previous verse
                    foreach (TranslatorWord w in updatedVerse)
                    {
                        w.TargetWordIndex += offset;
                        otWords[originalRef].Add(w);
                    }
                }
                else
                {
                    otWords[originalRef] = updatedVerse;
                }


            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
        }
        private void OutputTranslatorTagsOT(Dictionary<string, List<TranslatorWord>> words, bool publicDomain)
        {
            string version = Properties.TranslationTags.Default.Version.Replace(".", "_");

            StreamWriter swR = null;
            StreamWriter sw = null;

            bool perBook = Properties.TranslationTags.Default.FilesPerBook;
            bool preambleWritten = false;
            string lastWrittenBook = string.Empty;
            int bookNum = 0;

            foreach ((string verseRef, List<TranslatorWord> verseWords) in words)
            {
                container.UpdateProgress("Processing xlation Tags", (100 * verseCounter++) / totalVerseCount);

                if (perBook)
                {
                    int space = verseRef.IndexOf(' ');
                    string book = verseRef.Substring(0, space);
                    if (string.IsNullOrEmpty(lastWrittenBook))
                    {
                        // this is the first time through
                        bookNum = 1;
                        lastWrittenBook = book;
                    }
                    else if (book != lastWrittenBook)
                    {
                        // we are starting a new book
                        // flush and close for last book
                        if (sw != null) { sw.Flush(); sw.Close(); sw = null; }
                        if (swR != null) { swR.Flush(); swR.Close(); swR = null; }
                        preambleWritten = false;
                        lastWrittenBook = book;
                        bookNum++;
                    }
                    if (sw == null)
                    {
                        string output = Path.Combine(booksFolderPath,
                                     string.Format("{0}_{1}_{2}_OT_{3}.txt", bookNum, book, Properties.TranslationTags.Default.OutputFileName, version));
                        sw = new StreamWriter(output, false);
                    }

                    if (swR == null)
                    {
                        if (!string.IsNullOrEmpty(Properties.TranslationTags.Default.ForReviewFileName))
                        {
                            string forReview = Path.Combine(booksFolderPath,
                                string.Format("{0}_{1}_{2}.txt", bookNum, book, Properties.TranslationTags.Default.ForReviewFileName));
                            swR = new StreamWriter(forReview, false);
                        }
                    }
                }
                else
                {
                    if (sw == null)
                    {
                        string output = Path.Combine(TranslationTagsFolderPath,
                                     string.Format("{0}_OT_{1}.txt", Properties.TranslationTags.Default.OutputFileName, version));
                        sw = new StreamWriter(output, false);
                    }
                    if (swR == null)
                    {
                        if (!string.IsNullOrEmpty(Properties.TranslationTags.Default.ForReviewFileName))
                        {
                            string forReview = Path.Combine(TranslationTagsFolderPath,
                            string.Format("{0}.txt", Properties.TranslationTags.Default.ForReviewFileName));
                            swR = new StreamWriter(forReview, false);
                        }
                    }
                }

                if (publicDomain && !preambleWritten)
                {
                    WritePreamble(sw);
                    preambleWritten = true;
                }

                if (verseRef == "Neh 7:67")
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
                                araWordNo = string.Format("#{0:d2}", word.TargetWordIndex + arabicIndexAdjust);
                            }
                            else
                            {
                                araWordNo = string.Format("#{0:d2} - #{1:d2}", word.TargetWordIndex + arabicIndexAdjust,
                                                                   word.TargetWordIndex + arabicIndexAdjust + parts.Length - 1);
                                arabicIndexAdjust += (parts.Length - 1);
                            }
                        }

                        string line = string.Empty;
                        if (word.OtiginalWords == null)
                        {
                            line = string.Format("{0}\t{1}\t\t\t\t\t\t",
                                    araWordNo,
                                    word.Word);
                            string reviewLine = string.Format("{0}\t{1}", verseRef, word.Word);
                            swR.WriteLine(reviewLine);
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
                                WordByWord.TrimEnd(new char[] { ';', ' ' }));
                            if (string.IsNullOrEmpty(araWordNo))
                            {
                                if (!(Strongs.Contains("H0853") && AncientWord.TrimEnd(new char[] { ';', ' ' }) == "אֶת") && !Strongs.Contains("H0834") && !Strongs.Contains("H4994")) // אֲשֶׁר֙ אֶת 
                                {
                                    string reviewLine = string.Format("{0}\t#{1}:{2}",
                                        verseRef,
                                        AncientWordNumber.TrimEnd(new char[] { ';', ' ' }),
                                        AncientWord.TrimEnd(new char[] { ';', ' ' }));
                                    swR.WriteLine(reviewLine);
                                }
                            }
                        }
                        if (publicDomain)
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
            if (sw != null)
            {
                sw.Flush();
                sw.Close();
                sw = null;
            }
            if (swR != null)
            {
                swR.Flush();
                swR.Close();
                swR = null;
            }

        }

    }

}
