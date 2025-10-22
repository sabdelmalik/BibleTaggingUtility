using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace TraceParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string line = "BibleTaggingUtil.Editor.TargetGridView.SaveVerse	Information	1	[Isa 30:16], [OLD: وَقُلْتُمْ: <H0559> «لاَ <H3808> بَلْ <H3588> عَلَى <H5921A_A> خَيْل <H5483> نَهْرُبُ». <H5127> لِذلِكَ <H5921A_B> <H3651C_A> تَهْرُبُونَ. <H5127> «وَعَلَى <H5921A_C> خَيْل سَرِيعَةٍ <> <H7031> نَرْكَبُ». <H7392> لِذلِكَ <H5921A_D> <H3651C_B> يُسْرُعُ <H7043> طَارِدُوكُمْ. <H7291>], [NEW: وَقُلْتُمْ: <H0559> «لاَ <H3808> بَلْ <H3588> عَلَى <H5921A_A> خَيْل <H5483> نَهْرُبُ». <H5127> لِذلِكَ <H5921A_B> <H3651C_A> تَهْرُبُونَ. <H5127> «وَعَلَى <H5921A_C> خَيْل سَرِيعَةٍ <> <H7031> نَرْكَبُ». <H7392> لِذلِكَ <H5921A_D> <H3651C_B> يُسْرُعُ <H7043> طَارِدُوكُمْ. <H7291>]	2025-10-16 02:28:47Z";
            SaveVerseInfo verse = new SaveVerseInfo(line);
        } 

        private void openTraceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select Trace File";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ProcessTraceFile(openFileDialog1.FileName);
            }

            if(verses.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var verse in verses)
                {
                    if(verse.Reference != null)
                        sb.AppendLine($"{verse.Timestamp:yyyy-MM-dd HH:mm:ss}\t{verse.Reference}");
                    else
                        sb.AppendLine($"{verse.Timestamp:yyyy-MM-dd HH:mm:ss}\tRuth.1.");
                    sb.AppendLine($"\tOld:\t{verse.Old}");
                    sb.AppendLine($"\tNew:\t{verse.New}");
                }

                File.WriteAllText(@"c:\tmp\TraceParce.txt", sb.ToString());
            }
        }

        List<SaveVerseInfo> verses = new List<SaveVerseInfo>();
        private void ProcessTraceFile(string filePath)
        {
            verses.Clear();
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("BibleTaggingUtil.Editor.TargetGridView.SaveVerse"))
                        {
                            var verse = new SaveVerseInfo(line);
                            verses.Add(verse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

    internal class SaveVerseInfo
    {
        public SaveVerseInfo(string line)
        {
            string n = "[NEW: ";
            string o = "[OLD: ";
            char opnBracket = '[';
            char clsBracket = ']';

            // =====================
            // NEW
            int startIndex = line.IndexOf(n);
            if (startIndex == -1)
            { 
                return;
            }
            string nString = line.Substring(startIndex + n.Length);
            int endIndex = nString.IndexOf(clsBracket);
            if (endIndex == -1)
            {
                return;
            }
            string dt = nString.Substring(endIndex + 1).Trim();
            New = nString.Substring(0, endIndex).Trim();
            if (DateTime.TryParse(dt, out var timestamp))
            {
                Timestamp = timestamp;
            }
            line = line.Substring(0, startIndex).Trim();
            // =====================
            // OLD
            startIndex = line.IndexOf(o);
            if (startIndex == -1)
            {
                return;
            }
            string oString = line.Substring(startIndex + o.Length);
            endIndex = oString.IndexOf(clsBracket);
            if (endIndex == -1)
            {
                return;
            }
            Old = oString.Substring(0, endIndex).Trim();
            line = line.Substring(0, startIndex).Trim();
            // Reference may be missing
            startIndex = line.IndexOf(opnBracket);
            if (startIndex == -1)
            {
                Reference = "Ruth 1:";
                return;
            }
            endIndex = line.IndexOf(clsBracket);
            Reference = line.Substring(startIndex+1, endIndex - startIndex-1).Trim();
        }
        public DateTime Timestamp { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Old { get; set; } = string.Empty;
        public string New { get; set; } = string.Empty;
    }
}
