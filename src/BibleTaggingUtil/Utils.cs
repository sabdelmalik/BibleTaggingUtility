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
            int space = reference.IndexOf(' ');
            string book = space > 0 ? reference.Substring(0, space) : "UKN";
            return GetBookIndexFromBook(book);
        }
        public static int GetBookIndexFromBook(string book)
        {

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

            return verse.Trim();
        }

        public static bool AreReferencesEqual(string ref1, string ref2)
        {
            int c1 = 0, v1 = 0, c2 = 0, v2 = 0;

            if (ref1 == ref2) { return true; }

            int index1 = GetBookIndexFromReference(ref1);
            int index2 = GetBookIndexFromReference(ref2);

            string[] ref1Parts = ref1.Split(' ');
            string[] ref2Parts = ref2.Split(' ');

            if (ref1Parts.Length != 2 || ref2Parts.Length != 2)
                return false;

            if (index1 == index2 && ref1Parts[1] == ref2Parts[1])
                return true;

            string[] chapterVerse1 = ref1Parts[1].Split(":");
            string[] chapterVerse2 = ref2Parts[1].Split(":");
            if (int.TryParse(chapterVerse1[0], out c1) &&
                int.TryParse(chapterVerse1[1], out v1) &&
                int.TryParse(chapterVerse2[0], out c2) &&
                int.TryParse(chapterVerse2[1], out v2) &&
                c1 == c2 && v1 == v2 && index1 == index2)
            {
                return true;
            }

            return false;
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
