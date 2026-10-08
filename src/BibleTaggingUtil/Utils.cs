using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    public enum BibleTestament
    {
        OT,
        NT,
    }

    public class Utils
    {
        public static BibleTestament GetTestament(string reference)
        {
            return (GetBookIndexFromReference(reference) < 39)? BibleTestament.OT : BibleTestament.NT;   
        }

        public static int GetBookIndexFromReference(string reference)
        {
            // reference can be in the form of "Gen 1:1" or "Gen.1.1"
            if(reference.Contains("."))
            {
                string[] parts = reference.Split('.');
                if (parts.Length > 0)
                {
                    return GetBookIndexFromBook(parts[0]);
                }
                else
                {
                    throw new Exception(string.Format("Invalid reference format: {0}", reference));
                }
            }
            int space = reference.IndexOf(' ');
            string book = space > 0 ? reference.Substring(0, space) : "UKN";
            return GetBookIndexFromBook(book);
        }
        public static int GetBookIndexFromBook(string book)
        {
            // if book is all caps, change it to title case
            if (book == book.ToUpper())
            {
                // if book starts with a number, keep the number as is and capitalize the first letter of the rest of the book name
                if (char.IsDigit(book[0]))
                {
                    book = book.Substring(0,2) + book.Substring(2).ToLower();
                }
                else
                {
                    book = book.Substring(0, 1) + book.Substring(1).ToLower();
                } //return GetBookIndexFromBook(book);    
            }

            if (Constants.osisNames.Contains(book, StringComparer.OrdinalIgnoreCase))
                return Array.IndexOf(Constants.osisNames, book);

            if (Constants.ubsNames.Keys.Contains(book, StringComparer.OrdinalIgnoreCase))
                return Array.IndexOf(Constants.ubsNames.Keys.ToArray(), book);

            if (Constants.osisAltNames.Contains(book, StringComparer.OrdinalIgnoreCase))
                return Array.IndexOf(Constants.osisAltNames, book);

            if (Constants.osisAltNames2.Contains(book, StringComparer.OrdinalIgnoreCase))
                return Array.IndexOf(Constants.osisAltNames2, book);

            throw new Exception(string.Format("Failed to find '{0}' in any book list!", book));

        }
        public static string GetUbsReference(string reference)
        {
            string result = string.Empty;
            // the reference can be in the form of "Gen 1:1" or "Gen.1.1"
            // extract the book, chapter and verse from the reference base on its format
            // convert the book name to its UBS equivalent if it is not already in that format
            // reconstruct the reference and return it
            bool dotFormat = reference.Contains(".");
            if (dotFormat)
            {
                string[] parts = reference.Split('.');
                if (parts.Length == 3)
                {
                    string book = parts[0];
                    string chapter = parts[1];
                    string verse = parts[2];
                    int bkIndex = GetBookIndexFromBook(book);
                    book = Constants.ubsNames.Keys.ToArray()[bkIndex];
                    result = $"{book}.{chapter}.{verse}";
                }
            }
            else
            {
                string[] parts = reference.Split(' ');
                if (parts.Length == 2)
                {
                    string book = parts[0];
                    string[] chapterVerse = parts[1].Split(':');
                    if (chapterVerse.Length == 2)
                    {
                        string chapter = chapterVerse[0];
                        string verse = chapterVerse[1];
                        int bkIndex = GetBookIndexFromBook(book);
                        book = Constants.ubsNames.Keys.ToArray()[bkIndex];
                        result = $"{book} {chapter}:{verse}";
                    }
                }
            }



            return result;
        }



        public static string GetVerseText(Verse words, bool includeTags)
        {
            string verse = string.Empty;

            for (int i = 0; i < words.Count; i++)
            {
                bool extended = Properties.ReferenceBibles.Default.ExtendedTaggingOT;
                if (extended && words[i].TahotSubWords.Count > 0)
                {
                    for (int j = 0; j < words[i].TahotSubWords.Count; j++)
                    {
                        verse += " " + words[i].TahotSubWords[j].English;
                        if (includeTags)
                        {
                            verse += " " + words[i].TahotSubWords[j].Tag.ToStringBracketed();
                        }
                    }
                }
                else
                {
                    verse += " " + words[i].Word;
                    if (includeTags)
                    {
                        verse += " " + words[i].Strong.ToStringBracketed();
                        /*                 for (int j = 0; j < words[i].Strong.Count; j++)
                                           {
                                               verse += (" <" + words[i].Strong[j].ToString()) + ">";
                                           }
                        */
                    }
                }
            }

            return verse.Trim();
        }

        /// <summary>
        /// Determines whether two Bible references are equal.
        /// </summary>
        /// <param name="ref1"></param>
        /// <param name="ref2"></param>
        /// <returns></returns>
        public static bool AreReferencesEqual(string ref1, string ref2)
        {
            // references can be in the form of "Gen 1:1" or "Gen.1.1"
            if (ref1 == ref2)
                return true;

            try
            {
                int index1 = GetBookIndexFromReference(ref1);
                int index2 = GetBookIndexFromReference(ref2);

                if (index1 != index2)
                    return false;

                // here we know that the book is the same, so we need to check the chapter and verse
                if (!TryParseChapterVerse(ref1, out int ch1, out int v1))
                    return false;

                if (!TryParseChapterVerse(ref2, out int ch2, out int v2))
                    return false;

                return ch1 == ch2 && v1 == v2;
            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Parses the chapter and verse numbers out of a reference string.
        /// Supports both "Gen 1:1" and "Gen.1.1" formats.
        /// </summary>
        private static bool TryParseChapterVerse(string reference, out int chapter, out int verse)
        {
            chapter = 0;
            verse = 0;

            ReadOnlySpan<char> span = reference.AsSpan();

            if (reference.Contains('.'))
            {
                int firstDot = span.IndexOf('.');
                if (firstDot < 0)
                    return false;

                int secondDot = span.Slice(firstDot + 1).IndexOf('.');
                if (secondDot < 0)
                    return false;
                secondDot += firstDot + 1;

                ReadOnlySpan<char> chapterSpan = span.Slice(firstDot + 1, secondDot - firstDot - 1);
                ReadOnlySpan<char> verseSpan = span.Slice(secondDot + 1);

                return int.TryParse(chapterSpan, out chapter) && int.TryParse(verseSpan, out verse);
            }
            else
            {
                int space = span.IndexOf(' ');
                if (space < 0)
                    return false;

                ReadOnlySpan<char> rest = span.Slice(space + 1);
                int colon = rest.IndexOf(':');
                if (colon < 0)
                    return false;

                ReadOnlySpan<char> chapterSpan = rest.Slice(0, colon);
                ReadOnlySpan<char> verseSpan = rest.Slice(colon + 1);

                return int.TryParse(chapterSpan, out chapter) && int.TryParse(verseSpan, out verse);
            }
        }
        public static bool AreReferencesEqual1(string ref1, string ref2)
        {
            // references can be in the form of "Gen 1:1" or "Gen.1.1"
            bool result = false;
            bool result1 = false;
            bool result2 = false;
            try
            {
                if (ref1 == ref2)
                {
                    result = true;
                }
                else
                {
                    int ch1 = 0, v1 = 0, ch2 = 0, v2 = 0;

                    // get the book index for each reference
                    int index1 = GetBookIndexFromReference(ref1);
                    int index2 = GetBookIndexFromReference(ref2);

                    if (ref1.Contains("."))
                    {
                        string[] parts1 = ref1.Split('.');
                        if (parts1.Length == 3)
                        {
                            if (int.TryParse(parts1[0], out ch1))
                            {
                                result1 = int.TryParse(parts1[1], out v1);
                            }
                        }
                    }
                    else
                    {
                        string[] parts1 = ref1.Split(' ');
                        if (parts1.Length == 2)
                        {
                            string[] chapterVerse1 = parts1[1].Split(":");
                            if (chapterVerse1.Length == 2)
                            {
                                if (int.TryParse(chapterVerse1[0], out ch1))
                                {
                                    result1 = int.TryParse(chapterVerse1[1], out v1);
                                }
                            }
                        }
                    }
                    if (ref2.Contains("."))
                    {
                        string[] parts2 = ref2.Split('.');
                        if (parts2.Length == 3)
                        {
                            if (int.TryParse(parts2[0], out ch2))
                            {
                                result2 = int.TryParse(parts2[1], out v2);
                            }
                        }
                    }
                    else
                    {
                        string[] parts2 = ref2.Split(' ');
                        if (parts2.Length == 2)
                        {
                            string[] chapterVerse2 = parts2[1].Split(":");
                            if (chapterVerse2.Length == 2)
                            {
                                if (int.TryParse(chapterVerse2[0], out ch2))
                                    result2 = int.TryParse(chapterVerse2[1], out v2);
                            }
                        }
                    }

                    result = result1 && result2 && index1 == index2 && ch1 == ch2 && v1 == v2;
                }
            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
                result = false;
            }

            return result;
        }

        public static string RemoveDiacritics(string lineToClean)
        {
            // Remove diacretics
            string result = lineToClean.
                                Replace("\u064B", "").  // ARABIC FATHATAN
                                Replace("\u064C", "").  // ARABIC DAMMATAN
                                Replace("\u064D", "").  // ARABIC KASRATAN
                                Replace("\u064E", "").  // ARABIC FATHA
                                Replace("\u064F", "").  // ARABIC DAMMA
                                Replace("\u0650", "").  // ARABIC KASRA
                                Replace("\u0651", "").  // ARABIC SHADDA
                                Replace("\u0652", "").  // ARABIC SUKUN
                                Replace("\u0653", "").  // Madda
                                Replace("\u0654", "").  // Hamza above
                                Replace("\u0655", "").  // Hamza below
                                Replace("\u0656", "").  // 
                                Replace("\u0657", "").
                                Replace("\u0658", "").
                                Replace("\u0659", "").
                                Replace("\u065A", "").
                                Replace("\u065B", "").
                                Replace("\u065C", "").
                                Replace("\u065D", "").
                                Replace("\u065E", "").
                                Replace("\u065F", "");

            return result;
        }
        public static string RemovePuctuations(string lineToClean)
        {
            string result = lineToClean.
                Replace("«", "").
                Replace("»", "").
                Replace(": ", " ").
                Replace("؟", ".").
                Replace("!", ".");

            return result;
        }

        public static bool IsReferenceAramaic(string refernce)
        {
            string referencePattern = @"^([0-9A-Za-z]+)\s([0-9]+):([0-9]+)";
            Match mTx = Regex.Match(refernce, referencePattern);
            if (!mTx.Success)
            {
                Tracing.TraceError(MethodBase.GetCurrentMethod().Name, "Incorrect reference format: " + refernce);
                return false;
            }

            String book = mTx.Groups[1].Value;
            string chapter = mTx.Groups[2].Value;
            string verse = mTx.Groups[3].Value;

            return IsAramaic(book, chapter, verse);
        }

        public static bool IsAramaic(string book, string chapter, string verse)
        {
            bool result = false;
            int ch = 0;
            int vs = 0;
            if (!int.TryParse(chapter, out ch))
                return result;
            if (!int.TryParse(verse, out vs))
                return result;
            if ((book == "Gen" && ch == 31 && vs == 47) ||
                (book == "Ezr" && ((ch == 4 && vs >= 8) || (ch == 5) || (ch == 6 && vs <= 18))) ||
                (book == "Ezr" && (ch >= 7 && vs >= 12 && vs <= 26)) ||
                (book == "Pro" && ch == 31 && vs == 2) ||
                (book == "Jer" && ch == 10 && vs == 11) ||
                (book == "Dan" && ((ch == 2 && vs >= 4) || (ch > 2 && ch < 7) || (ch == 7 && vs <= 28))))
                result = true;

            return result;
        }
    }
}
