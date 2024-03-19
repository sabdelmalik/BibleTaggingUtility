using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.Versification
{
    internal class Sbook
    {
        public Sbook(string name, string osis, string prefAbbrev, short chapmax, short[] versemax = null)
        {
            Name = name;
            OSIS = osis;
            PrefAbbrev = prefAbbrev;
            Chapmax = chapmax;
            Versemax = versemax;
        }
        /// <summary>
        /// Name of book
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// OSIS name
        /// </summary>
        public string OSIS { get; }

        /// <summary>
        /// Preferred Abbreviation
        /// </summary>
        public string PrefAbbrev { get; }

        /// <summary>
        /// Maximum chapters in book
        /// </summary>
        public short Chapmax { get; }

        /// <summary>
        /// Array[chapmax] of maximum verses in chapters
        /// </summary>
        public short[] Versemax { get; }

    }
}
