using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPTips
{
    class Settings:ConfigNodeStorage
    {
        internal Settings(String FilePath)
            : base(FilePath) { }
        
        [Persistent]
        public Boolean Hidden = false;

        [Persistent]
        public String DateTipsDownloaded = "";
    }
}
