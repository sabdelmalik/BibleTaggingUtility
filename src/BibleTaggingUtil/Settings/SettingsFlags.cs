using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.Settings
{
    internal class SettingsFlags
    {
        public bool TopRefChanged {  get; set; }
        public bool MainOtChanged { get; set;}
        public bool MainNtChanged { get; set; }
        public bool TargetBibleChanged { get; set; }
        public bool VersificaltionChanged { get; set; }
    }
}
