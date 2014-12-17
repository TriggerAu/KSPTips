using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using UnityEngine;
using KSPPluginFramework;

namespace KSPTips
{
    internal static class KSPTipsDownloader
    {
        const String DOWNLOADURL = "https://github.com/TriggerAu/KSPTips/raw/master/KSPTips/Resources/Tips.cfg";


        static internal BackgroundWorker bw;

        static KSPTipsDownloader()
        {
            bw = new BackgroundWorker();
        }



        static internal void BeginCheck()
        {
            MonoBehaviourExtended.LogFormatted("Checking if we need to download a new tips file (Last={0})...", KSPTips.settings.DateTipsDownloaded);
            if (KSPTips.settings.DateTipsDownloaded != string.Format("{0:yyyy-MM-dd}", DateTime.Now))
            {
                bw.DoWork += DownloadTipsFile;
                bw.RunWorkerCompleted += DownloadJobDone;
                bw.RunWorkerAsync();
            }
        }

        static WWW wwwTips;
        static void DownloadTipsFile(object sender, DoWorkEventArgs e)
        {
            MonoBehaviourExtended.LogFormatted("Downloading tips from: {0}", DOWNLOADURL);
            wwwTips = new WWW(DOWNLOADURL);
            while (!wwwTips.isDone) { }
        }

        static void DownloadJobDone(object sender, RunWorkerCompletedEventArgs e)
        {
            String strTipsFile = wwwTips.text;
            MonoBehaviourExtended.LogFormatted("Download Complete: {0} Bytes", strTipsFile.Length);
            if (strTipsFile.Length > 0)
            {
                if (System.IO.File.Exists(KSPTips.PathPlugin + "/TipsDownloaded.cfg"))
                    System.IO.File.Delete(KSPTips.PathPlugin + "/TipsDownloaded.cfg");

                System.IO.File.WriteAllText(KSPTips.PathPlugin + "/TipsDownloaded.cfg", strTipsFile);

                ConfigNode cnToLoad = ConfigNode.Load(KSPTips.PathPlugin + "/TipsDownloaded.cfg");
                if (cnToLoad.GetNodes("TIP").Length > 0)
                {
                    MonoBehaviourExtended.LogFormatted("Contains Nodes, updating file, and reloading");
                    if (System.IO.File.Exists(KSPTips.PathPlugin + "/Tips.cfg"))
                        System.IO.File.Delete(KSPTips.PathPlugin + "/Tips.cfg");
                    System.IO.File.Copy(KSPTips.PathPlugin + "/TipsDownloaded.cfg", KSPTips.PathPlugin + "/Tips.cfg");

                    KSPTips.loadTips();
                }

                if (System.IO.File.Exists(KSPTips.PathPlugin + "/TipsDownloaded.cfg"))
                    System.IO.File.Delete(KSPTips.PathPlugin + "/TipsDownloaded.cfg");

                KSPTips.settings.DateTipsDownloaded = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                KSPTips.settings.Save();
            }
        }
    }
}
