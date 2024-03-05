using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
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
            return (GetBookIndex(reference) < 39)? BibleTestament.OT : BibleTestament.NT;   
        }
        public static int GetBookIndex(string reference)
        {
            int space = reference.IndexOf(' ');
            string book = space > 0 ? reference.Substring(0, space) : "UKN";

            if (Constants.osisNames.Contains(book, StringComparer.OrdinalIgnoreCase))
                return Array.IndexOf(Constants.osisNames, book);

            if (Constants.ubsNames.Contains(book, StringComparer.OrdinalIgnoreCase))
                return Array.IndexOf(Constants.ubsNames, book);

            if (Constants.osisAltNames.Contains(book, StringComparer.OrdinalIgnoreCase))
                return Array.IndexOf(Constants.osisAltNames, book);

            if (Constants.osisAltNames2.Contains(book, StringComparer.OrdinalIgnoreCase))
                return Array.IndexOf(Constants.osisAltNames2, book);

            throw new Exception(string.Format("Failed to find '{0}' in any book list!", book));

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
 /*                   for (int j = 0; j < words[i].Strong.Count; j++)
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

            int index1 = GetBookIndex(ref1);
            int index2 = GetBookIndex(ref2);

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

        public static string RemoveDiacritics(string lineToCkean)
        {
            // Remove diacretics
            string result = lineToCkean.
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
                Replace("\u065F", "").
                Replace("«", "").
                Replace("»", "").
                Replace(": ", " ").
                Replace("؟", ".").
                Replace("!", ".");

            return result;
        }
    }
}
