using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Windows.Forms.Design.AxImporter;

namespace BibleTaggingUtil.BibleVersions
{
    internal class JsonBibleConverter
    {
         public string Write(Dictionary<string, Verse> bible)
        {
            string result = string.Empty;
            try
            {
                var options = new JsonWriterOptions
                {
                    Indented = true
                };

                using var stream = new MemoryStream();
                using var writer = new Utf8JsonWriter(stream, options);
                writer.WriteStartObject();
                {
                    writer.WritePropertyName("Bible");
                    writer.WriteStartObject();
                    {
                        foreach (var (reference, verse) in bible)
                        {
                            writer.WritePropertyName(reference);
                            writer.WriteStartObject();
                            {
                                WriteVerse(writer, verse);
                            }
                            writer.WriteEndObject();
                        }
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
                writer.Flush();

                result = Encoding.UTF8.GetString(stream.ToArray());
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
            return result;
        }

        public void Read(string jsonPath, Dictionary<string, Verse> bible, Dictionary<string, string> bookNames, List<string> bookNamesList)
        {
            string jsonString = string.Empty;
            using (StreamReader sr = new StreamReader(jsonPath))
            {
                jsonString = sr.ReadToEnd();
            }
            JsonNode root = JsonNode.Parse(jsonString);
            JsonNode bibleNode = root["Bible"];
            bible.Clear();
            int wordIndex = 0;
            var verses = bibleNode.AsObject();
            foreach (var verse in verses)
            {
                string currentReference = verse.Key;
                Verse theVerse = new Verse();
                wordIndex = 0;
                var value = verse.Value["Verse"];
                var dirty = value["Dirty"];
                var verseWords = value["VerseWords"].AsArray();  
                foreach (JsonObject verseWord in verseWords)
                {
                    string? reference = (string)verseWord["Reference"];
                    string? morf = (string)verseWord["Morphology"];
                    string? word = (string)verseWord["Word"];
                    #region Strong's Numbers                 
                    string? s = (string)verseWord["StrongString"];
                    string[] strongs= new string[0];
                    bool isHebrew = Utils.GetTestament(reference)== BibleTestament.OT;
                    if(s != null)
                    {
                        strongs = s.Split(new char[] { ' ' });  
                        for (int i = 0; i<strongs.Length; i++)
                        {
                            strongs[i] = strongs[i].Trim().Replace(">","").Replace("<","");
                            if(strongs[i].Length > 0) 
                                strongs[i] = strongs[i].Substring(1);
                        }
                    }
                    #endregion Strong's Numbers
                    string? hebrew = (string)verseWord["Hebrew"];
                    string? greek = (string)verseWord["Greek"];
                    string? xlit = (string)verseWord["Transliteration"];
                    VerseWord theWord = new VerseWord(isHebrew ? hebrew : greek, word, strongs, xlit, reference, morf);
                    theVerse[wordIndex++] = theWord;
                }
                bible[currentReference] = theVerse;
            }
        }
        private void WriteVerse(Utf8JsonWriter writer, Verse verse)
        {
            try
            {
                writer.WritePropertyName("Verse");

                writer.WriteStartObject();

                writer.WritePropertyName(nameof(verse.Count));
                writer.WriteStringValue(verse.Count.ToString());

                writer.WritePropertyName(nameof(verse.Dirty));
                writer.WriteStringValue(verse.Dirty ? "true" : "false");

                writer.WritePropertyName("VerseWords");
                writer.WriteStartArray();
                for (int i = 0; i < verse.Count; i++)
                {
                    WriteVerseWord(writer, verse[i]);
                }
                writer.WriteEndArray();

                writer.WriteEndObject();
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }

        }

        private void WriteVerseWord(Utf8JsonWriter writer, VerseWord verseWord)
        {
            try
            { 
                if (verseWord.Reference == "Gen 20:3")
                {
                    int s = 0;
                }
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(verseWord.Reference));
            writer.WriteStringValue(verseWord.Reference);

            writer.WritePropertyName(nameof(verseWord.Morphology));
            writer.WriteStringValue(verseWord.Morphology);

            writer.WritePropertyName(nameof(verseWord.Word));
            writer.WriteStringValue(verseWord.Word);

            writer.WritePropertyName(nameof(verseWord.StrongString));
            writer.WriteStringValue(verseWord.StrongString);

            writer.WritePropertyName(nameof(verseWord.Hebrew));
            writer.WriteStringValue(verseWord.Hebrew);

            writer.WritePropertyName(nameof(verseWord.Greek));
            writer.WriteStringValue(verseWord.Greek);

            writer.WritePropertyName(nameof(verseWord.Transliteration));
            writer.WriteStringValue(verseWord.Transliteration);

            writer.WriteEndObject();
            }
            catch (Exception ex)
            {
                var cm = System.Reflection.MethodBase.GetCurrentMethod();
                var name = cm.DeclaringType.FullName + "." + cm.Name;
                Tracing.TraceException(name, ex.Message);
            }
        }
    }
}
