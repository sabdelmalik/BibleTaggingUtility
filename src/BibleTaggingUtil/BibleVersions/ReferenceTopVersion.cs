using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.BibleVersions
{
    public class ReferenceTopVersion : BibleVersion
    {
        public ReferenceTopVersion(BibleTaggingForm container) : base(container, 23145 + 7957) { }

        public void Load()
        {
            try
            {
                string referenceBibleFileFolder = Properties.Settings.Default.referenceBibleFileFolder;
                if (string.IsNullOrEmpty(referenceBibleFileFolder))
                {
                    referenceBibleFileFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "bibles");
                }

                string refFile = container.GetBibleFilePath(referenceBibleFileFolder, "Select Reference File");
                string referenceBibleFileName = Path.GetFileName(refFile);
                Properties.Settings.Default.ReferenceBibleFileName = referenceBibleFileName;
                referenceBibleFileFolder = Path.GetDirectoryName(refFile);
                Properties.Settings.Default.referenceBibleFileFolder = referenceBibleFileFolder;

                Properties.Settings.Default.Save();

                LoadBibleFile(refFile, true, false);
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
