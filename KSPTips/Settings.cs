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

        [Persistent]
        public Vector2 TipsTopLeftPos = new Vector2();
        [Persistent]
        public Boolean TipsTopLeftSet = false;
    }


}
