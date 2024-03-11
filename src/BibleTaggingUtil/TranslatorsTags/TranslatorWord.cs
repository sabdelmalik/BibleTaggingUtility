using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.TranslationTags
{
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
