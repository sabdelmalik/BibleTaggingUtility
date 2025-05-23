using BibleTaggingUtil.TranslationTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.TranslatorsTags
{
    internal class VerseMappings
    {
        public static Dictionary<string, List<int[]>> OldTestament
        {
            get
            {
                Dictionary<string, List<int[]>> map = new Dictionary<string, List<int[]>>();

                //==============================================================
                string book = "Jos";
                int ch = 16; int v = 8;
                string vRef = string.Format("{0} {1}:{2}", book, ch, v);
                map.Add(vRef, new List<int[]>());
                //                              ch  v  s  l  v    s   l
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 9 });

                v++;
                vRef = string.Format("{0} {1}:{2}", book, ch, v);
                map.Add(vRef, new List<int[]>());
                //                              ch  v  s  l  v    s   l
                map[vRef].Add(new int[] { ch, v, 0, 6, v - 1, 9, -1 });
                map[vRef].Add(new int[] { ch, v, 6, -1, v, 0, -1 });
                //==============================================================
                book = "Jdg";
                ch = 15; v = 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v);
                map.Add(vRef, new List<int[]>());
                //                              ch  v  s  l  v    s   l
                map[vRef].Add(new int[] { ch, v, 0, 10, v, 0, 11 });

                v++;
                vRef = string.Format("{0} {1}:{2}", book, ch, v);
                map.Add(vRef, new List<int[]>());
                //                              ch  v  s  l  v    s   l
                map[vRef].Add(new int[] { ch, v, 0, 8, v - 1, 11, -1 });
                map[vRef].Add(new int[] { ch, v, 8, -1, v, 0, -1 });
                //==============================================================
                book = "1Sa";
                ch = 10; v = 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 13, v, 0, 14 });
                map[vRef].Add(new int[] { ch, v, 13, 0, v, 14, 19 });
                map[vRef].Add(new int[] { ch, v, 13, -1, v, 33, -1 });
                //==============================================================
                book = "1Sa";
                ch = 13; v = 15;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 5, v, 0, 5 });
                map[vRef].Add(new int[] { ch, v, 5, 0, v, 5, 13 });
                map[vRef].Add(new int[] { ch, v, 5, -1, v, 18, -1 });
                //==============================================================
                book = "1Sa";
                ch = 14; v = 41;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 5, v, 0, 6 });
                map[vRef].Add(new int[] { ch, v, 5, 0, v, 6, 25 });
                map[vRef].Add(new int[] { ch, v, 5, -1, v, 31, -1 });
                //==============================================================
                book = "1Sa";
                ch = 19; v = 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 11 });

                v++;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 6, v - 1, 11, -1 });
                map[vRef].Add(new int[] { ch, v, 6, -1, v, 0, -1 });
                //==============================================================
                book = "2Sa";
                ch = 13; v = 34;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 12, v, 0, 14 });
                map[vRef].Add(new int[] { ch, v, 12, 0, v, 14, 11 });
                map[vRef].Add(new int[] { ch, v, 12, -1, v, 25, -1 });
                //==============================================================
                book = "1Ki";
                ch = 22; v = 21;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 8, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 8, -1, v + 1, 0, 4 });

                v++;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 4, -1 });
                //==============================================================
                book = "2Ki";
                ch = 4; v = 43;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 18, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 18, -1, v + 1, 0, -1 });
                //==============================================================
                book = "Psa";
                ch = 72; v = 19;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 10, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 10, -1, v + 1, 0, -1 });
                //==============================================================
                book = "Psa";
                ch = 18; v = 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 19, v-1, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 19, -1, v, 0, -1 });
                //==============================================================
                book = "Neh";
                ch = 6; v = 5;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 10, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 10, -1, v + 1, 0, 2 });

                v++;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 2, -1 });


                return map;
            }
        }
        
        
        public static Dictionary<string, List<int[]>> NewTestament
        {
            get
            {
                Dictionary<string, List<int[]>> map = new Dictionary<string, List<int[]>>();
                string book, vRef;
                int ch, v;

                //==============================================================
                book = "Mat"; // Others
                ch = 15; v = 5;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 12, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 12, -1, v + 1, 0, 11 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 11, -1 });
                //==============================================================
                book = "Mat"; // KJV
                ch = 20; v = 4;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 10, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 10, -1, v + 1, 0, 3 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 3, -1 });
                //==============================================================
                book = "Mar";
                ch = 3; v = 19; // KJV
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 4, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 4, -1, v + 1, 0, 4 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 4, -1 });
                //==============================================================
                book = "Mar"; // ??
                ch = 6; v = 27;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 11 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 4, v - 1, 11, -1 });
                map[vRef].Add(new int[] { ch, v, 4, -1, v, 0, -1 });
                //==============================================================
                book = "Mar"; // ??
                ch = 7; v = 21;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 8, v, 0, 12 });
                map[vRef].Add(new int[] { ch, v, 8, 1, v + 1, 0, 1 });
                map[vRef].Add(new int[] { ch, v, 9, 1, v, 12, 1 });
                map[vRef].Add(new int[] { ch, v, 10, 1, v, 14, 1 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 1, v - 1, 13, 1 });
                map[vRef].Add(new int[] { ch, v, 1, -1, v, 1, -1 });
                //==============================================================
                book = "Mar"; // NA
                ch = 12; v = 14;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 28, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 28, -1, v + 1, 0, 4 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 4, -1 });
                //==============================================================
                book = "Luk"; // KJV
                ch = 1; v = 73;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v , 0, 8 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 1, v-1, 8, -1 });
                map[vRef].Add(new int[] { ch, v, 1, -1, v, 0, -1 });
                //==============================================================
                book = "Luk"; // KJV
                ch = 6; v = 17;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 20, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 20, -1, v + 1, 0, 10 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 10, -1 });
                //==============================================================
                book = "Luk"; // KJV
                ch = 7; v = 18;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 9});

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 4, v - 1, 9, -1 });
                map[vRef].Add(new int[] { ch, v, 4, -1, v, 0, -1 });
                //==============================================================
                book = "Act"; // KJV
                ch = 2; v = 10;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 10, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 10, -1, v + 1, 0, 4 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 4, -1 });
                //==============================================================
                book = "Act";
                ch = 3; v = 19; // KJV
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 4, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 4, -1, v + 1, 0, 9 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 9, -1 });
                //==============================================================
                book = "Act"; // KJV
                ch = 5; v = 39;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 13 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 2, v - 1, 13, -1 });
                map[vRef].Add(new int[] { ch, v, 2, -1, v, 0, -1 });
                //==============================================================
                book = "Act";  // ???
                ch = 11; v = 25;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 7, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 7, -1, v + 1, 0, 7 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 7, -1 });
                //==============================================================
                //book = "Act";  // ???
                //ch = 14; v = 14;
                //vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                //map.Add(vRef, new List<int[]>());
                //map[vRef].Add(new int[] { ch, v, 0, 11, v, 0, -1 });
                //map[vRef].Add(new int[] { ch, v, 11, -1, v + 1, 0, 2 });

                //v += 1;
                //vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                //map.Add(vRef, new List<int[]>());
                //map[vRef].Add(new int[] { ch, v, 0, -1, v, 2, -1 });
                //==============================================================
                book = "Act"; // ???
                ch = 24; v = 2;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 8 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 10, v - 1, 8, -1 });
                map[vRef].Add(new int[] { ch, v, 10, -1, v, 0, -1 });
                //==============================================================
                book = "Act";  // KJV
                ch = 24; v = 18;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 11, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 11, -1, v + 1, 0, 6 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 6, -1 });
                //==============================================================
                //book = "Rom";  // KJV
                //ch = 3; v = 25;
                //vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                //map.Add(vRef, new List<int[]>());
                //map[vRef].Add(new int[] { ch, v, 0, 13, v, 0, -1 });
                //map[vRef].Add(new int[] { ch, v, 13, -1, v + 1, 0, 5 });

                //v += 1;
                //vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                //map.Add(vRef, new List<int[]>());
                //map[vRef].Add(new int[] { ch, v, 0, -1, v, 5, -1 });
                ////==============================================================
                book = "Rom";  // KJV
                ch = 7; v = 9;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 12, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 12, -1, v + 1, 0, 3 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 3, -1 });
                //==============================================================
                book = "Rom";  // KJV
                ch = 9; v = 11;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 16, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 16, -1, v + 1, 0, 7 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 7, -1 });
                //==============================================================
                book = "2Co";  // KJV
                ch = 8; v = 13;
                vRef = string.Format("{0} {1}:{2}", book, ch, v);
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 8 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 3, v - 1, 8, -1 });
                map[vRef].Add(new int[] { ch, v, 3, -1, v, 0, -1 });
                //==============================================================
                book = "2Co";  // KJV
                ch = 10; v = 4;
                vRef = string.Format("{0} {1}:{2}", book, ch, v);
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 15 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 2, v - 1, 15, -1 });
                map[vRef].Add(new int[] { ch, v, 2, -1, v, 0, -1 });
                //==============================================================
                book = "2Co";  // ???
                ch = 11; v = 8;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 7, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 7, -1, v + 1, 0, 9 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 9, -1 });
                //==============================================================
                book = "2Co";  // KJV
                ch = 13; v = 12;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 5 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v - 1, 5, -1 });
                //==============================================================
                book = "2Co";  // KJV
                ch = 13; v = 14;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v - 1, 0, 21 });
                //==============================================================
                book = "Gal";  // KJV
                ch = 2; v = 19;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 9 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 2, v - 1, 9, -1 });
                map[vRef].Add(new int[] { ch, v, 2, -1, v, 0, -1 });
                //==============================================================
                book = "Eph";  // KJV ???
                ch = 2; v = 14;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 17 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 1, v - 1, 17, 2 });
                map[vRef].Add(new int[] { ch, v, 1, 1, v, 6, 1 });
                map[vRef].Add(new int[] { ch, v, 2, 1, v - 1, 19, -1 });
                map[vRef].Add(new int[] { ch, v, 3, 4, v, 0, 6 });
                map[vRef].Add(new int[] { ch, v, 7, -1, v, 7, -1 });
                //==============================================================
                book = "Eph";  // ???
                ch = 3; v = 17;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 10 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 4, v - 1, 10, -1 });
                map[vRef].Add(new int[] { ch, v, 4, -1, v, 0, -1 });
                //==============================================================
                book = "Eph";  // KJV
                ch = 5; v = 13;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 5, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 5, -1, v + 1, 0, 6 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 6, -1 });
                //==============================================================
                book = "Phi";  // KJV
                ch = 1; v = 16;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v + 1, 0, -1 });
                //==============================================================
                book = "Phi";  // KJV
                ch = 1; v = 17;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v - 1, 0, -1 });
                //==============================================================
                book = "Phi";  // KJV
                ch = 2; v = 8;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 4, v - 1, 10, -1 });
                map[vRef].Add(new int[] { ch, v, 4, -1, v, 0, -1 });
                //==============================================================
                book = "Col";  // KJV
                ch = 1; v = 21;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 9, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 9, -1, v + 1, 0, 3 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 3, -1 });
                //==============================================================
                book = "1Ti";  // KJV
                ch = 6; v = 21;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 7 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v-1, 7, 12 });
                //==============================================================
                book = "1Th";  // KJV
                ch = 1; v = 2;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 14 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 1, v, 0, 1 });
                map[vRef].Add(new int[] { ch, v, 1, 1, v - 1, 14, 1 });
                map[vRef].Add(new int[] { ch, v, 2, -1, v, 1, -1 });
                //==============================================================
                book = "1Th";  // KJV
                ch = 2; v = 6;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 9, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 9, -1, v + 1, 0, 7 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 7, -1 });
                //==============================================================
                book = "1Th";  // KJV ???
                ch = 2; v = 11;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 3, v, 0, 3 });
                map[vRef].Add(new int[] { ch, v, 3, 1, v + 1, 0, 2 });
                map[vRef].Add(new int[] { ch, v, 4, 5, v, 3, -1 });
                map[vRef].Add(new int[] { ch, v, 9, -1, v + 1, 2, 4 });
                //==============================================================
                book = "Heb";  // KJV ???
                ch = 3; v = 9;
                vRef = string.Format("{0} {1}:{2}", book, ch, v);
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 6, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 6, -1, v + 1, 0, 2 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 2, -1 });
                //==============================================================
                book = "Heb";  // KJV ???
                ch = 6; v = 6;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 1, v, 0, 2 });
                map[vRef].Add(new int[] { ch, v, 1, 1, v-2, 0, 1 });
                map[vRef].Add(new int[] { ch, v, 2, -1, v, 2, -1 });
                //==============================================================
                book = "Heb";  // KJV ???
                ch = 7; v = 20;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 6 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 7, v - 1, 6, -1 });
                map[vRef].Add(new int[] { ch, v, 7, -1, v, 0, -1 });
                //==============================================================
                book = "1Pe";  // KJV ???
                ch = 3; v = 15;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 16, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 16, -1, v + 1, 0, 5 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 5, -1 });
                //==============================================================
                book = "1Jo";  // KJV
                ch = 2; v = 13;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 14, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 14, -1, v + 1, 0, 7 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 7, -1 });
                //==============================================================
                book = "Rev"; // KJV
                ch = 2; v = 27;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 7, v, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 7, -1, v + 1, 0, 7 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 7, -1 });
                //==============================================================
                book = "Rev"; // KJV
                ch = 13; v = 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 5, v - 1, 0, -1 });
                map[vRef].Add(new int[] { ch, v, 5, -1, v, 0, -1 });
                //==============================================================
                book = "Rev"; // KJV
                ch = 17; v = 9;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, -1, v, 0, 18 });

                v += 1;
                vRef = string.Format("{0} {1}:{2}", book, ch, v); ;
                map.Add(vRef, new List<int[]>());
                map[vRef].Add(new int[] { ch, v, 0, 2, v - 1, 18, -1 });
                map[vRef].Add(new int[] { ch, v, 2, -1, v, 0, -1 });

                return map;
            }
        }
    }


}
