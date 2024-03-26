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
      //private void ProcessVerseNT(string verseRef, string originalRef, Verse verse, int startIndex, int end, Verse greekVerse)
        private void ProcessVerseNT(string verseRef, string originalRef, Verse verse, int startIndex, int count, Verse greekVerse)
        {
            Dictionary<string, int> usedStrongs = new Dictionary<string, int>();

            List<TranslatorWord> newVerse = new List<TranslatorWord>();

            if (originalRef == "1Sa 19:2")
            {
                int x = 0;
            }

            #region set repeated word indices and counts
            Dictionary<string, List<VerseWord>> temp = new Dictionary<string, List<VerseWord>>();
            for (int i = 0; i < greekVerse.Count; i++)
            {
                VerseWord vw1 = greekVerse[i];
                if (vw1.Strong.Count != 1) continue; // for now, handle only single strong's.

                string strongString = vw1.StrongStringS;
                if (temp.ContainsKey(strongString)) continue; // we already processed this number

                for (int j = i + 1; j < greekVerse.Count; j++)
                {
                    VerseWord vw2 = greekVerse[j];
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

                // key: target word index
                // val: hebrew word index
                Dictionary<int, List<int>> target2HebrewMap = new Dictionary<int, List<int>>();

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

                            //VerseWord hw = greekVerse.GetWordFromStrong(strong.ToStringS(), lastHebIndex, comb); // offset);
                            List<VerseWord> hwList = greekVerse.GetWordListFromStrong(strong);//, lastHebIndex, comb); // offset);
                            VerseWord hw = null;
                            if (hwList.Count == 0)
                            {
                                ntErrors.Add(string.Format("Empty hwList:  {0}\t{1}-{2}\t{3}",
                                   verseRef,
                                   verse[i].WordIndex,
                                   verse[i].Word,
                                   strong));
                            }
                            else if (hwList.Count > 1)
                            {

                                if (target2HebrewMap.Count == 0)
                                {
                                    hw = hwList[0];
                                }
                                else
                                {
                                    int lastAraWordIdx = target2HebrewMap.Keys.ToArray()[target2HebrewMap.Keys.Count - 1];
                                    lastHebIndex = 0;
                                    foreach (int hi in target2HebrewMap[lastAraWordIdx])
                                    {
                                        if (hi > lastHebIndex) lastHebIndex = hi;
                                    }


                                    List<VerseWord> unusedHebWds = new List<VerseWord>();
                                    bool found = false;
                                    for (int j = 0; j < hwList.Count; j++)
                                    {
                                        hw = hwList[j];
                                        foreach (List<int> ints in target2HebrewMap.Values)
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
                                ntErrors.Add(string.Format("hw is null: {0}\t{1}-{2}\t{3}",
                                   verseRef,
                                   verse[i].WordIndex + 1,
                                   verse[i].Word,
                                   strong));

                            }
                            else
                            {
                                if (comb)
                                    lastHebIndex = Math.Min(lastHebIndex, hw.WordIndex);
                                else
                                    lastHebIndex = hw.WordIndex;

                                comb = true; // in case next strong is still within the same target word
                                usedStrongs[strong.ToStringS()] = hw.WordIndex + 1;
                                if (target2HebrewMap.ContainsKey(i))
                                {
                                    target2HebrewMap[i].Add(hw.WordIndex);
                                }
                                else
                                {
                                    target2HebrewMap[i] = new List<int> { hw.WordIndex };
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

                // Now map contains all Target word indices and corresponding Hebrew word indices
                // if a Hebrew index is missing
                // we find the target word preceding 

                SortedDictionary<int, int> missedWords = new SortedDictionary<int, int>();
                int lastHebrewIndex = -1;
                int indexOffset = 1;
                bool doContinue = false;

                Dictionary<int, int> reverseMap = new Dictionary<int, int>();
                Dictionary<int, List<int>> correctedMap = new Dictionary<int, List<int>>();

                // fill in the Target Words missing from map
                // Target words with no hebrew words, are assigned hebrew word numbers beyond the greekVerse.Count
                int missingTargetWords = 0;
                if (count > 0)
                {
                    //int targetWordCount = count;
                    missingTargetWords = count - target2HebrewMap.Count;
                    if (missingTargetWords > 0)
                    {
                        int lastHebrewWord = greekVerse.Count - 1;
                        for (int i = startIndex; i < startIndex + count; i++)
                        {
                            if (target2HebrewMap.ContainsKey(i))
                            {
                                correctedMap[i] = target2HebrewMap[i];
                            }
                            else
                            {
                                correctedMap[i] = new List<int> { ++lastHebrewWord };
                            }
                        }
                    }
                    else
                    {
                        correctedMap = target2HebrewMap;
                    }
                }

                // initialise reverse map
                // making room at the end for the unmapped Target words
                for (int i = 0; i < (greekVerse.Count + missingTargetWords); i++)
                {
                    reverseMap[i] = -1;
                }

                int restoredAllowance = 100;  // alowance for restored Hebrew words (RHW) e.g. if 3 RHW follow word index 5, they will be numbered 501, 502, 503
                // assign target indices to hebrew indeces
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
                            // this code is to deal with an Target word with multiple Hebrew words with gaps in btween
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
                        if (hIndex < greekVerse.Count)
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
                        VerseWord hw = greekVerse[hIndex[0]];
                        List<OriginalWordDetails> ow = new List<OriginalWordDetails>();
                        ow.Add(new OriginalWordDetails(hw.RootStrong, hw.Morphology, hw.Hebrew, hw.WordIndex, hw.Transliteration, hw.Word, hw.WordNumber, hw.WordType, hw.AltVerseNumber, hw.Reference));
                        updatedVerse.Add(new TranslatorWord(string.Empty, ow));

                    }
                }

                if (ntWords.ContainsKey(originalRef))
                {
                    int offset = 0;
                    // find last Target word index
                    for (int i = ntWords[originalRef].Count - 1; i >= 0; i--)
                    {
                        TranslatorWord word = ntWords[originalRef][i];
                        if (string.IsNullOrEmpty(word.Word)) continue;
                        offset = word.TargetWordIndex + 1;
                        break;
                    }
                    //append this verse to previous verse
                    foreach (TranslatorWord w in updatedVerse)
                    {
                        w.TargetWordIndex += offset;
                        ntWords[originalRef].Add(w);
                    }
                }
                else
                {
                    ntWords[originalRef] = updatedVerse;
                }


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
                        sw.WriteLine("Ara W#\tTarget\tStrongs\tGrammar\tHeb w#\tHebrew\tTransliteration\tWord-by-word");
                        //sw.WriteLine("ESV W#\tTarget\tStrongs\tGrammar\tHeb w#\tHebrew\tTransliteration\tWord-by-word");
                        int targetIndexAdjust = 1;
                        foreach (TranslatorWord word in verseWords)
                        {
                            string tgtWordNo = string.Empty;
                            if (!string.IsNullOrEmpty(word.Word))
                            {
                                string[] parts = word.Word.Split(new char[] { ' ' });
                                if (parts.Length == 1)
                                {
                                    tgtWordNo = string.Format("#{0:d2}", word.TargetWordIndex + targetIndexAdjust);
                                }
                                else
                                {
                                    tgtWordNo = string.Format("#{0:d2} - #{1:d2}", word.TargetWordIndex + targetIndexAdjust,
                                                                       word.TargetWordIndex + targetIndexAdjust + parts.Length - 1);
                                    targetIndexAdjust += (parts.Length - 1);
                                }
                            }

                            string line = string.Empty;
                            if (word.OtiginalWords == null)
                            {
                                line = string.Format("{0}\t{1}\t\t\t\t\t\t",
                                        tgtWordNo,
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
                                    tgtWordNo,
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
