using BibleTaggingUtil.Strongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    public class TahotSubWord
    {
        public TahotSubWord(string english, string hebrew, string altWordNumber, string wordNumber, string wordType, string lexicalForm, string gloss, string morphology, string meaningVariant, string transliteration, string altStrongs, string rootStrongs, StrongsCluster tag, bool isRoot)
        {
            English = english;
            Hebrew = hebrew;
            AltWordNumber = altWordNumber;
            WordNumber = wordNumber;
            WordType = wordType;
            LexicalForm = lexicalForm;
            Gloss = gloss;
            Morphology = morphology;
            MeaningVariant = meaningVariant;
            Transliteration = transliteration;
            AltStrongs = altStrongs;
            RootStrongs = rootStrongs;
            Tag = tag;
            IsRoot = isRoot;
        }

        public string English { get; set; }             // English translation in/ beginning
        public string Hebrew { get; set; }              // Hebrew בְּ/רֵאשִׁ֖ית
        public string AltWordNumber { get; set; }       // Alternate word number in TAHOT
        public string WordNumber { get; set; }          // Word number in TAHOT
        public string WordType { get; set; }            // Type L=Leningrad text; Q=Qere scribal corrections; K=original text; R=Restored text
        public string LexicalForm { get; set; }         // Lexical form in Heberew (from Expanded Strong tags: H9003=ב=in/{H7225G=רֵאשִׁית=: beginning»first:1_beginning})
        public string Gloss { get; set; }               // English gloss (from Expanded Strong tags: H9003=ב=in/{H7225G=רֵאשִׁית=: beginning»first:1_beginning})
        public string Morphology { get; set; }          // Grammar HR/Ncfsa
        public string MeaningVariant { get; set; }      // Meaning Variants
        public string Transliteration { get; set; }     // Transliteration be./re.Shit
        public string AltStrongs { get; set; }          // Alternative Strong
        public string RootStrongs { get; set; }         // dStrong H9003/{H7225G}
        public StrongsCluster Tag { get; set; }         // Tags (from Expanded Strong tags: H9003=ב=in/{H7225G=רֵאשִׁית=: beginning»first:1_beginning})
        public bool IsRoot { get; set; } = false;       // Strong was in in {curly brackets}
    }
}
