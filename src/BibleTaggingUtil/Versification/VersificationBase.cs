using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.Versification
{
    internal class VersificationBase
    {
        public Sbook[] otbooks;
        public Sbook[] ntbooks;
        public int[][] LAST_VERSE;

        public VersificationBase()
        {
            otbooks = new Sbook[]
            {
                new Sbook("Genesis", "Gen", "Gen", 50),
                new Sbook("Exodus", "Exod", "Exod", 40),
                new Sbook("Leviticus", "Lev", "Lev", 27),
                new Sbook("Numbers", "Num", "Num", 36),
                new Sbook("Deuteronomy", "Deut", "Deut", 34),
                new Sbook("Joshua", "Josh", "Josh", 24),
                new Sbook("Judges", "Judg", "Judg", 21),
                new Sbook("Ruth", "Ruth", "Ruth", 4),
                new Sbook("I Samuel", "1Sam", "1Sam", 31),
                new Sbook("II Samuel", "2Sam", "2Sam", 24),
                new Sbook("I Kings", "1Kgs", "1Kgs", 22),
                new Sbook("II Kings", "2Kgs", "2Kgs", 25),
                new Sbook("I Chronicles", "1Chr", "1Chr", 29),
                new Sbook("II Chronicles", "2Chr", "2Chr", 36),
                new Sbook("Ezra", "Ezra", "Ezra", 10),
                new Sbook("Nehemiah", "Neh", "Neh", 13),
                new Sbook("Esther", "Esth", "Esth", 10),
                new Sbook("Job", "Job", "Job", 42),
                new Sbook("Psalms", "Ps", "Ps", 150),
                new Sbook("Proverbs", "Prov", "Prov", 31),
                new Sbook("Ecclesiastes", "Eccl", "Eccl", 12),
                new Sbook("Song of Solomon", "Song", "Song", 8),
                new Sbook("Isaiah", "Isa", "Isa", 66),
                new Sbook("Jeremiah", "Jer", "Jer", 52),
                new Sbook("Lamentations", "Lam", "Lam", 5),
                new Sbook("Ezekiel", "Ezek", "Ezek", 48),
                new Sbook("Daniel", "Dan", "Dan", 12),
                new Sbook("Hosea", "Hos", "Hos", 14),
                new Sbook("Joel", "Joel", "Joel", 3),
                new Sbook("Amos", "Amos", "Amos", 9),
                new Sbook("Obadiah", "Obad", "Obad", 1),
                new Sbook("Jonah", "Jonah", "Jonah", 4),
                new Sbook("Micah", "Mic", "Mic", 7),
                new Sbook("Nahum", "Nah", "Nah", 3),
                new Sbook("Habakkuk", "Hab", "Hab", 3),
                new Sbook("Zephaniah", "Zeph", "Zeph", 3),
                new Sbook("Haggai", "Hag", "Hag", 2),
                new Sbook("Zechariah", "Zech", "Zech", 14),
                new Sbook("Malachi", "Mal", "Mal", 4),
            };

            ntbooks = new Sbook[]{
                  new Sbook("Matthew", "Matt", "Matt", 28),
                  new Sbook("Mark", "Mark", "Mark", 16),
                  new Sbook("Luke", "Luke", "Luke", 24),
                  new Sbook("John", "John", "John", 21),
                  new Sbook("Acts", "Acts", "Acts", 28),
                  new Sbook("Romans", "Rom", "Rom", 16),
                  new Sbook("I Corinthians", "1Cor", "1Cor", 16),
                  new Sbook("II Corinthians", "2Cor", "2Cor", 13),
                  new Sbook("Galatians", "Gal", "Gal", 6),
                  new Sbook("Ephesians", "Eph", "Eph", 6),
                  new Sbook("Philippians", "Phil", "Phil", 4),
                  new Sbook("Colossians", "Col", "Col", 4),
                  new Sbook("I Thessalonians", "1Thess", "1Thess", 5),
                  new Sbook("II Thessalonians", "2Thess", "2Thess", 3),
                  new Sbook("I Timothy", "1Tim", "1Tim", 6),
                  new Sbook("II Timothy", "2Tim", "2Tim", 4),
                  new Sbook("Titus", "Titus", "Titus", 3),
                  new Sbook("Philemon", "Phlm", "Phlm", 1),
                  new Sbook("Hebrews", "Heb", "Heb", 13),
                  new Sbook("James", "Jas", "Jas", 5),
                  new Sbook("I Peter", "1Pet", "1Pet", 5),
                  new Sbook("II Peter", "2Pet", "2Pet", 3),
                  new Sbook("I John", "1John", "1John", 5),
                  new Sbook("II John", "2John", "2John", 1),
                  new Sbook("III John", "3John", "3John", 1),
                  new Sbook("Jude", "Jude", "Jude", 1),
                  new Sbook("Revelation of John", "Rev", "Rev", 22),
            };

        }
    
        protected void PopulateLastVerse(Sbook[] otBooks, Sbook[] ntBooks, int[] lastVerse)
        {
            LAST_VERSE = new int[otBooks.Length + ntBooks.Length][];
            int bookOffset = 0;

            for (int i = 0; i < otBooks.Length; i++)
            {
                int chapters = otBooks[i].Chapmax;
                LAST_VERSE[i] = new int[chapters];

                Array.Copy(lastVerse, bookOffset, LAST_VERSE[i], 0, chapters);
                bookOffset += chapters;
            }

            for (int i = 0; i < ntBooks.Length; i++)
            {
                int idx = i + otBooks.Length;
                int chapters = ntBooks[i].Chapmax;
                LAST_VERSE[idx] = new int[chapters];

                Array.Copy(lastVerse, bookOffset, LAST_VERSE[idx], 0, chapters);
                bookOffset += chapters;
            }

        }
    }
}
