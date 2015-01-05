using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPTips
{

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class KSPTips_Editor : KSPTips
    {
        public KSPTips_Editor()
        {
            isShowingGuides = true;
        }
    }
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KSPTips_Flight : KSPTips
    {
        public KSPTips_Flight()
        {
            isShowingGuides = true;
        }
    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre,false)]
    public class KSPTips_SC : KSPTips
    {
        public KSPTips_SC()
        {
            isShowingTips = true;
            isShowingGuides = true;
        }
    }

    //[KSPAddon(KSPAddon.Startup.Instantly, false)]
    //public class KSPTips_Instant : KSPTips
    //{
    //    public KSPTips_Instant()
    //    {
    //        isShowingTips = true;
    //        isShowingGuides = false;
    //    }
    //}
    
    public class KSPTips:MonoBehaviourExtended
    {
        internal static Boolean isShowingTips = false;
        internal static Boolean isShowingGuides = true;

        internal static String PathPlugin = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        internal static String PathPluginGuideImages = string.Format("{0}/PluginData",PathPlugin);

        internal Windows.Tips windowTips;
        internal Windows.Guides windowGuides;
        
        ///////////////////////////////////////////////////////////////////////////
        //Removed Building MouseOver Functionality
        ///////////////////////////////////////////////////////////////////////////
        //internal KSPTipsWindowBuilding tipwindowBuilding;
#if DEBUG
        internal TipsWindowDebug debugwin;
#endif

        internal static Settings settings;
        internal Texture2D texBox,texBoxWithHeader,texCross,texPlay,texNext,texPrev,texPause;

        internal AppLauncherButtonWrapper AppButton;

        public static List<Tip> lstTips;
        public static List<Guide> lstGuides;
        public static List<GuidePage> lstGuidePages;

        internal override void Awake()
        {
            LogFormatted("KSPTips Awake");

            LogFormatted("Loading Settings");
            settings = new Settings("settings.cfg");
            if (!settings.Load())
                LogFormatted("Settings Load Failed");

            if (isShowingGuides) { 
            loadGuides();
            LogFormatted("GuidePages loaded: {0}",lstGuides.Count);
            string guideslist = "";
            foreach (GuidePage item in lstGuidePages)
            {
                guideslist += "\r\n" + String.Format("{0}-{1}  {2}-{3}  {4}", item.guide.TargetScene, item.guide.Folder, item.guide.Title, item.Title, item.Image);
            }
            LogFormatted_DebugOnly(guideslist);

            windowGuides = gameObject.AddComponent<Windows.Guides>();
            windowGuides.mbTip = this;
            windowGuides.WindowRect = new Rect(100, 100, 600, 400);

            Texture2D texMainButton = new Texture2D(38, 38, TextureFormat.ARGB32, false);
            KSPTips.ExtractToTexture(ref texMainButton, "img_Book");
            AppButton = new AppLauncherButtonWrapper(texMainButton);
            GameEvents.onGUIApplicationLauncherReady.Add(AppButton.OnGUIAppLauncherReady);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(AppButton.OnGUIAppLauncherUnreadify);

            AppButton.onTrue += AppButton_onTrue;
            AppButton.onFalse += AppButton_onFalse;
            }

            if (isShowingTips)
            {
                loadTips();

                LogFormatted("KSPTips loaded: {0}", lstTips.Count);
                string tipslist = "";
                foreach (Tip item in lstTips)
                {
                    tipslist += "\r\n" + String.Format("{0}-{1}-{2}-{3}-{4}", item.Question, item.Answer, item.Image, item.GameMode, item.ModAssembly);
                }
                LogFormatted_DebugOnly(tipslist);

                windowTips = gameObject.AddComponent<Windows.Tips>();
                windowTips.mbTip = this;
                windowTips.Visible = !settings.Hidden;

                if (!settings.Hidden)
                {
                    SetRepeatRate(15);
                    StartRepeatingWorker();
                    ChangeTip();
                }

            }

            ///////////////////////////////////////////////////////////////////////////
            //Removed Building MouseOver Functionality
            ///////////////////////////////////////////////////////////////////////////
            //tipwindowBuilding = gameObject.AddComponent<KSPTipsWindowBuilding>();
            //tipwindowBuilding.mbTip = this;


            //Now do a background download of the tips as well
            KSPTipsDownloader.BeginCheck();

#if DEBUG
            debugwin = gameObject.AddComponent<TipsWindowDebug>();
            debugwin.WindowRect = new Rect(0, 0, 100, 500);
#endif
        }

        void AppButton_onFalse(object sender, EventArgs e)
        {
            windowGuides.Visible = false;
        }

        void AppButton_onTrue(object sender, EventArgs e)
        {
            windowGuides.Visible = true;
        }
        internal override void Start()
        {
            ///////////////////////////////////////////////////////////////////////////
            //Removed Building MouseOver Functionality
            ///////////////////////////////////////////////////////////////////////////
            //ScreenSafeUI.fetch.GetComponentInChildren<ScreenSafeGUIText>().gameObject.AddComponent<KSCMouseOver>();
            //KSCMouseOver.onMouseEnter += KSCMouseOver_onMouseEnter;
            //KSCMouseOver.onMouseExit += KSCMouseOver_onMouseExit;

        }

        internal override void OnGUIOnceOnly()
        {
            //SkinsLibrary.SetCurrent(SkinsLibrary.DefSkinType.Unity);

            texBox = new Texture2D(9, 9, TextureFormat.ARGB32, false);
            texBoxWithHeader = new Texture2D(9, 20, TextureFormat.ARGB32, false);
            texNext = new Texture2D(10, 10, TextureFormat.ARGB32, false);
            texPrev = new Texture2D(10, 10, TextureFormat.ARGB32, false);
            texPlay = new Texture2D(16, 16, TextureFormat.ARGB32, false);
            texPause = new Texture2D(16, 16, TextureFormat.ARGB32, false);
            texCross = new Texture2D(16, 16, TextureFormat.ARGB32, false);

            KSPTips.ExtractToTexture(ref texCross, "img_Cross");
            KSPTips.ExtractToTexture(ref texPlay, "img_Play");
            KSPTips.ExtractToTexture(ref texPause, "img_Pause");
            KSPTips.ExtractToTexture(ref texNext, "img_Next");
            KSPTips.ExtractToTexture(ref texPrev, "img_Prev");
            KSPTips.ExtractToTexture(ref texBox, "tex_Box");
            KSPTips.ExtractToTexture(ref texBoxWithHeader, "tex_BoxWithHeader");
        }

        internal override void OnGUIEvery()
        {
            ///////////////////////////////////////////////////////////////////////////
            //Removed Building MouseOver Functionality
            ///////////////////////////////////////////////////////////////////////////
            //if (settings.Hidden && tipwindowBuilding.Visible==false)
            if (isShowingTips)
            {
                if (settings.Hidden)
                {
                    if (GUI.Button(new Rect(0, Screen.height - 56, 20, 20), "?", HighLogic.Skin.button))
                    {
                        settings.Hidden = false;
                        settings.Save();
                        windowTips.Visible = true;
                        windowTips.WindowRect.x = 0;
                        windowTips.WindowRect.y = Screen.height - (67 + 36);

                        SetRepeatRate(15);
                        StartRepeatingWorker();
                        ChangeTip();

                    }
                }
            }
        }

        internal override void RepeatingWorker()
        {
            if (isShowingTips)
            {
                ChangeTip();
            }
        }

        internal override void OnDestroy()
        {
            LogFormatted("KSPTips Destroyed");


            GameEvents.onGUIApplicationLauncherReady.Remove(AppButton.OnGUIAppLauncherReady);
            GameEvents.onGUIApplicationLauncherUnreadifying.Remove(AppButton.OnGUIAppLauncherUnreadify);

            ///////////////////////////////////////////////////////////////////////////
            //Removed Building MouseOver Functionality
            ///////////////////////////////////////////////////////////////////////////
            //KSCMouseOver.onMouseEnter -= KSCMouseOver_onMouseEnter;
            //KSCMouseOver.onMouseExit -= KSCMouseOver_onMouseExit;
        }

        Int32 DisplayedTip = 0;
        public void ChangeTip(Int32 IncrementBy=1)
        {
            DisplayedTip = DisplayedTip + IncrementBy;

            if (DisplayedTip >= lstTips.Count)
                DisplayedTip -= lstTips.Count;
            else if (DisplayedTip < 0)
                DisplayedTip += lstTips.Count;

            windowTips.thistip = lstTips[DisplayedTip];

            LogFormatted_DebugOnly("NewTip:{0}-{1}-{2}-{3}", DisplayedTip, windowTips.thistip.Question, windowTips.thistip.Answer, windowTips.thistip.Image);
            windowTips.texImage = new Texture2D(64,64,TextureFormat.ARGB32,false);
            if (windowTips.thistip.Image != "") {
                windowTips.thistip.ImageLoaded = ExtractToTexture(ref windowTips.texImage, windowTips.thistip.Image);
            }

        }


        ///////////////////////////////////////////////////////////////////////////
        //Removed Building MouseOver Functionality
        ///////////////////////////////////////////////////////////////////////////

        //void KSCMouseOver_onMouseExit(TipBuilding building)
        //{
        //    tipwindowBuilding.Visible = false;
        //    tipwindow.Visible = !settings.Hidden;

        //    print("GOODBYE:" + building.Name);

        //}

        //void KSCMouseOver_onMouseEnter(TipBuilding building)
        //{
        //    tipwindowBuilding.thistip = building;
        //    tipwindowBuilding.Visible = true;
        //    tipwindow.Visible = false;

        //    print("HELLO:" + building.Name);
        //    if (building.FacilityExists)
        //    {
        //        print (String.Format("{0},{1},{2}-{3}",building.Facility.CurrentLevel,building.Facility.MaxLevel,building.Facility.FacilityLevel
        //            ,building.Facility.GetLevelText()));

        //        foreach (TipFacilityLevel item in building.FacilityLevels)
        //        {
        //            print(string.Format("Level {0}({2}):{1}", item.Level, item.Text, item.Level <= building.Facility.FacilityLevel ? "Active" : "Locked"));

        //        }
        //    }
        //}

        internal static Boolean ExtractToTexture(ref Texture2D texImage, String resourceName)
        {
            Boolean result = false;
            try
            {
                Byte[] image = (Byte[])Properties.Resources.ResourceManager.GetObject(resourceName);
                texImage.LoadImage(image);
                result = true;
            }
            catch (Exception ex)
            {
                LogFormatted("Cant load resource texture {0}\r\n{1}", resourceName, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Loads a texture from the file system directly
        /// </summary>
        /// <param name="tex">Unity Texture to Load</param>
        /// <param name="FileName">Image file name</param>
        /// <param name="FolderPath">Optional folder path of image</param>
        /// <returns></returns>
        public static Boolean LoadImageFromFile(ref Texture2D tex, String FileName, String FolderPath = "")
        {
            //DebugLogFormatted("{0},{1}",FileName, FolderPath);
            Boolean blnReturn = false;
            try
            {
                if (FolderPath == "") FolderPath = PathPlugin;

                //File Exists check
                if (System.IO.File.Exists(String.Format("{0}/{1}", FolderPath, FileName)))
                {
                    try
                    {
                        MonoBehaviourExtended.LogFormatted_DebugOnly("Loading: {0}", String.Format("{0}/{1}", FolderPath, FileName));
                        tex.LoadImage(System.IO.File.ReadAllBytes(String.Format("{0}/{1}", FolderPath, FileName)));
                        blnReturn = true;
                    }
                    catch (Exception ex)
                    {
                        MonoBehaviourExtended.LogFormatted("Failed to load the texture:{0} ({1})", String.Format("{0}/{1}", FolderPath, FileName), ex.Message);
                    }
                }
                else
                {
                    MonoBehaviourExtended.LogFormatted("Cannot find texture to load:{0}", String.Format("{0}/{1}", FolderPath, FileName));
                }


            }
            catch (Exception ex)
            {
                MonoBehaviourExtended.LogFormatted("Failed to load (are you missing a file):{0} ({1})", String.Format("{0}/{1}", FolderPath, FileName), ex.Message);
            }
            return blnReturn;
        }

        public static void loadTips()
        {
            //Extract the Tips file resource if theres no file
            if (!System.IO.File.Exists(PathPlugin + "/Tips.cfg"))
                System.IO.File.WriteAllBytes(PathPlugin + "/Tips.cfg", Properties.Resources.Tips);
            
            lstTips = new List<Tip>();

            LogFormatted(PathPlugin + "/Tips.cfg");
            ConfigNode cnToLoad = ConfigNode.Load(PathPlugin + "/Tips.cfg");

            LogFormatted_DebugOnly("TipsInFile={0}",cnToLoad.GetNodes("TIP").Length);
            foreach (ConfigNode item in cnToLoad.GetNodes("TIP"))
            {
                Tip tmp = new Tip();
                tmp.Question = item.GetValue("Question");
                tmp.Answer = item.GetValue("Answer");
                tmp.Image = item.GetValue("Image");
                tmp.GameMode = item.GetValue("GameMode");
                tmp.ModAssembly = item.GetValue("ModAssembly");

                lstTips.Add(tmp);
            }

            if (HighLogic.CurrentGame!= null && HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
            {
                LogFormatted_DebugOnly("{0}-{1}", lstTips.Count, lstTips.Where(t => t.GameMode == null || t.GameMode.ToLower() != "career").Count());
                lstTips = lstTips.Where(t => t.GameMode == null || t.GameMode.ToLower() != "career").ToList();
            }

            lstTips = lstTips.Where(tip => tip.ModAssembly == "" ||
                    (AssemblyLoader.loadedAssemblies
                        .Select(a => a.assembly.GetExportedTypes())
                        .SelectMany(t => t)
                        .Any(t => t.FullName.ToLower().EndsWith("." + tip.ModAssembly.ToLower()))
                    )
                ).ToList();
            
            System.Random rand = new System.Random();
            lstTips = lstTips.OrderBy(t => rand.Next()).ToList();
        }

        public static void loadGuides()
        {
            //Extract the Guides file resource if theres no file
            //if (!System.IO.File.Exists(PathPlugin + "/Guides.cfg"))
                System.IO.File.WriteAllBytes(PathPlugin + "/Guides.cfg", Properties.Resources.Guides);

            lstGuides = new List<Guide>();
            lstGuidePages = new List<GuidePage>();

            LogFormatted(PathPlugin + "/Guides.cfg");
            ConfigNode cnToLoad = ConfigNode.Load(PathPlugin + "/Guides.cfg");

            LogFormatted_DebugOnly("Guides in file={0}", cnToLoad.GetNodes("GUIDE").Length);
            foreach (ConfigNode itemG in cnToLoad.GetNodes("GUIDE"))
            {
                Guide newGuide = new Guide();
                newGuide.Title = itemG.GetValue("Title");
                newGuide.Folder = itemG.GetValue("Folder");
                newGuide.TargetScene = itemG.GetValue("TargetScene");
                lstGuides.Add(newGuide);

                LogFormatted_DebugOnly("Pages in Guide={0}", itemG.GetNodes("PAGE").Length);

                foreach (ConfigNode itemP in itemG.GetNodes("PAGE"))
                {
                    GuidePage tmp = new GuidePage();
                    tmp.guide = newGuide;
                    tmp.Title= itemP.GetValue("Title");
                    tmp.Image = itemP.GetValue("Image");

                    lstGuidePages.Add(tmp);
                }
            }
        }

    }


    public class Tip
    {
        public String Question { get; set; }
        public String Answer { get; set; }
        public String Image { get; set; }
        public String GameMode { get; set; }
        public String ModAssembly { get; set; }
        public Boolean ImageLoaded { get; set; }
    }

    public class Guide{
        public String Title { get; set; }
        public String Folder {get;set;}
        public String TargetScene { get; set; }
    }

    public class GuidePage
    {
        public Guide guide { get; set; }
        public String Title { get; set; }
        public String Image { get; set; }
    }

#if DEBUG
    //This will kick us into the save called default and set the first vessel active
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class Debug_AutoLoadPersistentSaveOnStartup : MonoBehaviour
    {
        //use this variable for first run to avoid the issue with when this is true and multiple addons use it
        public static bool first = true;
        public void Start()
        {
            //only do it on the first entry to the menu
            if (first)
            {
                first = false;
                HighLogic.SaveFolder = "career";
                Game game = GamePersistence.LoadGame("persistent", HighLogic.SaveFolder, true, false);

                if (game != null && game.flightState != null && game.compatible)
                {
                    HighLogic.CurrentGame = game;
                    HighLogic.LoadScene(GameScenes.SPACECENTER);

                    //Int32 FirstVessel;
                    //Boolean blnFoundVessel = false;
                    //for (FirstVessel = 0; FirstVessel < game.flightState.protoVessels.Count; FirstVessel++)
                    //{
                    //    if (game.flightState.protoVessels[FirstVessel].vesselType != VesselType.SpaceObject &&
                    //        game.flightState.protoVessels[FirstVessel].vesselType != VesselType.Unknown)
                    //    {
                    //        blnFoundVessel = true;
                    //        break;
                    //    }
                    //}
                    //if (!blnFoundVessel)
                    //    FirstVessel = 0;
                    //FlightDriver.StartAndFocusVessel(game, FirstVessel);
                }

                //CheatOptions.InfiniteFuel = true;
            }
        }
    }
#endif
}


//http://drive.google.com/uc?id=0BzVqb8YpCXy6cldlSllmRzNMRXM&export=download