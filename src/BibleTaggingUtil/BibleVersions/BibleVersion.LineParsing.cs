using BibleTaggingUtil.Strongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BibleTaggingUtil.BibleVersions
{
    public abstract partial class BibleVersion
    {
        /// <summary>
        /// Parses a line of text representing a verse and extracts its components.
        /// </summary>
        /// <param name="line">The line of text to parse.</param>
        /// <exception cref="Exception">Thrown when the line is not in a valid verse format.</exception>
        protected virtual void ParseLine(string line)
        {
            Match mTx = Regex.Match(line, @"^([0-9A-Za-z]+)\s([0-9]+):([0-9]+)\s*(.*)");

            string book = string.Empty;
            string chapter = string.Empty;
            string verseNo = string.Empty;
            string verse = string.Empty;

            if (mTx.Success)
            {
                book = mTx.Groups[1].Value;
                chapter = mTx.Groups[2].Value;
                verseNo = mTx.Groups[3].Value;
                verse = mTx.Groups[4].Value;
            }
            else
            {
                throw new Exception(string.Format("Ill formed verse line!"));
            }

            string reference = string.Format("{0} {1}:{2}", book, chapter, verseNo);

            bool hasPsalmTitle = false;
            if (book.StartsWith("Ps") && verseNo.Trim() == "1")
                hasPsalmTitle = line.Contains('*');

            BibleTestament testament = Utils.GetTestament(reference);

            Verse verseWords = new Verse();
            try
            {
                // better implemetation is to use regex to split verse into words and tags, but this should work for most cases. We can always improve it later if we find some edge cases that are not handled by this implementation.
                string pattern = @"([^<>]+)[ \t]((<[A-Za-z_0-9=\-#]*>[ \t]*)+)";
                MatchCollection matches = Regex.Matches(verse, pattern);
                int index = 0;
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        string word = match.Groups[1].Value;
                        string strongsTag = match.Groups[2].Value;
                        List<string> strongs = new List<string>();
                        List<string> morph = new List<string>();
                        // do we have morphology? i.e. do we have = in the strongs?
                        if (strongsTag.Contains('='))
                        {
                            // The target Bible already has morphology in the strongs tags.
                            // We need to preseve the morphology bysetting SaveMorphology to true
                            Properties.TargetBibles.Default.SaveMorphology = true;

                            var ms = Regex.Matches(strongsTag, @"<([GHa-zA-Z0-9_#]+)=([A-Za-z0-9\-]+)>");
                            foreach (Match m in ms)
                            {
                                strongs.Add(m.Groups[1].Value.Trim());
                                if (m.Groups.Count > 2)
                                    morph.Add(m.Groups[2].Value.Trim());
                            }
                        }
                        else
                        {
                            if (bibleName == "KJV")
                            {
                                strongsTag = strongsTag.Replace("<", "").Replace(">", "").Trim();
                                if (strongsTag.Length > 4)
                                    strongsTag = strongsTag.Substring(strongsTag.Length - 4);
                                strongsTag = testament == BibleTestament.OT ? $"<H{strongsTag}>" : $"<G{strongsTag}>";
                            }
                            var ms = Regex.Matches(strongsTag, @"<([GHa-zA-Z0-9_]+)>");
                            foreach (Match m in ms)
                            {
                                strongs.Add(m.Groups[1].Value.Trim());
                            }
                        }

                        VerseWord verseWord = new VerseWord(word, new StrongsCluster(strongs, morph), reference);
                        verseWords[index++] = verseWord;
                    }
                }
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }


            string[] verseParts = verse.Split(' ');
            List<string> words = new List<string>();
            List<string> tags = new List<string>();
            string tempWord = string.Empty;
            string tmpTag = string.Empty;
            for (int i = 0; i < verseParts.Length; i++)
            {
                string versePart = verseParts[i].Trim();
                if (string.IsNullOrEmpty(versePart))
                    continue; // some extra space

                if (i == 0 || versePart[0] != '<') // add i == 0 test because a verse can not start with a tag.
                {
                    // this is a word

                    // Save constructed tag
                    if (!string.IsNullOrEmpty(tmpTag))
                        tags.Add(tmpTag);
                    tmpTag = string.Empty;

                    tempWord += (string.IsNullOrEmpty(tempWord)) ? verseParts[i] : (" " + verseParts[i]);
                    if (i == verseParts.Length - 1)
                    {
                        // last word
                        words.Add(tempWord);
                    }
                }
                else
                {
                    // This is a tag

                    // save constructed phrase
                    if (!string.IsNullOrEmpty(tempWord))
                        words.Add(tempWord);
                    tempWord = string.Empty;

                    if (verseParts[i] == "<>")
                    {
                        // if tmpTag is not empty, ignore this empty tag
                        if (string.IsNullOrEmpty(tmpTag))
                            tmpTag = "<>";
                    }
                    else
                    {
                        tmpTag += (string.IsNullOrEmpty(tmpTag)) ? verseParts[i] : (" " + verseParts[i]);
                        tmpTag = tmpTag.Replace(".", "").Replace("?", "").Trim(); // erroneous KJV characters
                        if (!string.IsNullOrEmpty(tmpTag))
                        {
                            string[] pts = tmpTag.Split(' ');
                            tmpTag = string.Empty;
                            foreach (string t in pts)
                            {
                                string prefix = Utils.GetTestament(reference) == BibleTestament.NT ? "G" : "H";
                                if (t[0] == '<' && char.IsDigit(t[1]))
                                    tmpTag = t.Replace("<", "<" + prefix) + " ";
                                else
                                    tmpTag += t/*.Replace("<", "<" + prefix)*/ + " ";
                            }
                        }
                        tmpTag = tmpTag.Trim();
                        if (i == verseParts.Length - 1)
                        {
                            // last word
                            if (tmpTag.EndsWith('.'))
                                tmpTag.Remove(tmpTag.Length - 1, 1);
                            tags.Add(tmpTag);
                        }
                    }
                }
            }

            if (words.Count == (tags.Count + 1)) // last word was not tagged
                tags.Add(string.Empty);
            string[] vWords = words.ToArray();
            string[] vTags = tags.ToArray();
            if (vWords.Length != vTags.Length)
            {
                throw new Exception(string.Format("Word Count = {0}, Tags Count {1}", vWords, vTags.Length));
            }

            // remove <> from tags
            for (int i = 0; i < vTags.Length; i++)
                vTags[i] = vTags[i].Replace("<", "").Replace(">", "");


            if (book.StartsWith("Ps") && verseNo.Trim() == "1")
                verseWords.HasPsalmTitle = hasPsalmTitle;

            bible.Add(reference, verseWords);
            currentVerseCount++;
            container.UpdateProgress("Loading " + bibleName, (100 * currentVerseCount) / totalVerses);
        }
    }
}
