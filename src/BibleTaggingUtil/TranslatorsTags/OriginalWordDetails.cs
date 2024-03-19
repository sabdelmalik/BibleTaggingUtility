using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.TranslationTags
{
    internal class OriginalWordDetails
    {
        public OriginalWordDetails(string strongs, string morphology, string ancientWord, int ancientWordIndex, string transliteration, string wordbyWord, string ancientWordNumber, string wordType, string ancientWordVerse, string ancientWordReference)
        {
            AncientWordIndex = ancientWordIndex;
            Strongs = strongs;
            Morphology = morphology;
            AncientWord = ancientWord;
            Transliteration = transliteration;
            WordByWord = wordbyWord;
            AncientWordNumber = ancientWordNumber;
            if (!string.IsNullOrEmpty(wordType) && !wordType.Contains("L"))
                AncientWordNumber += wordType;
            AncientWordVerse = ancientWordVerse;
            AncientWordReference = ancientWordReference;
        }

        public int AncientWordIndex { get; }
        public string AncientWordReference { get; }
        public string AncientWordVerse { get; }
        public string AncientWordNumber { get; }
        public string Strongs { get; }
        public string Morphology { get; }
        public string AncientWord { get; }
        public string Transliteration { get; }
        public string WordByWord { get; }
    }
}
