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

        [Obsolete]
        public void Load()
        {
            try
            {
                string refFile = Properties.ReferenceBibles.Default.TopReference;
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
