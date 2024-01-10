using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.LinkLabel;


namespace BibleTaggingUtil
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;
            string author = "Sami Abdel Malik";
            string copyright = "Copyright © 2023 by Sami Abdel Malik";
            string title = "Bible Text Tagging with Strong's Numbers";

            textBoxAbout1.Text = "\r\n" +title + "\r\n";
            textBoxAbout1.Text += copyright + "\r\n";
            textBoxAbout1.Text += "Version # " + version.ToString() + "\r\n";

            //textBoxAbout.Text += "\r\nicons from https://icons8.com\r\n";
            //textBoxAbout2.Text += "Hebrew and Greek Strong's numbers\r\n";
            //textBoxAbout2.Text += "are from STEPBible.org under CC BY 4.0";
            //textBoxAbout2.Text += "https://github.com/STEPBible/STEPBible-Data/tree/master/Translators%20Amalgamated%20OT%2BNT";

            textBoxAbout1.Select(0, 0);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            string link = "https://github.com/STEPBible/STEPBible-Data/tree/master/Translators%20Amalgamated%20OT%2BNT";
            OpenBrowser(link);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            string link = "https://wiki.crosswire.org/DevTools:Modules";
            OpenBrowser(link);

        }

        private void OpenBrowser(string link)
        {
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();

            try
            {
                // true is the default, but it is important not to set it to false
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = link;
                myProcess.Start();
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
