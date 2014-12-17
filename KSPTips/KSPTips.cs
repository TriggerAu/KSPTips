using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPTips
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre,false)]
    public class KSPTips:MonoBehaviourExtended
    {
        internal static String PathPlugin = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        
        internal KSPTipsWindow tipwindow;
        
        ///////////////////////////////////////////////////////////////////////////
        //Removed Building MouseOver Functionality
        ///////////////////////////////////////////////////////////////////////////
        //internal KSPTipsWindowBuilding tipwindowBuilding;
#if DEBUG
        internal TransferWindowPlanner.TipsWindowDebug debugwin;
#endif

        internal static Settings settings;

        internal Texture2D texBox,texCross,texPlay,texNext,texPrev,texPause;

        public List<Tip> lstTips;

        internal override void Awake()
        {
            LogFormatted("KSPTips Awake");

            LogFormatted("Loading Settings");
            settings = new Settings("settings.cfg");
            if (!settings.Load())
                LogFormatted("Settings Load Failed");

            loadTips();

            LogFormatted("KSPTips loaded: {0}",lstTips.Count);
            string tipslist = "";
            foreach (Tip item in lstTips)
            {
                tipslist += "\r\n" + String.Format("{0}-{1}-{2}-{3}-{4}", item.Question, item.Answer, item.Image,item.GameMode,item.ModAssembly);
            }
            LogFormatted_DebugOnly(tipslist);

            tipwindow = gameObject.AddComponent<KSPTipsWindow>();
            tipwindow.mbTip = this;
            tipwindow.Visible = !settings.Hidden;

            ///////////////////////////////////////////////////////////////////////////
            //Removed Building MouseOver Functionality
            ///////////////////////////////////////////////////////////////////////////
            //tipwindowBuilding = gameObject.AddComponent<KSPTipsWindowBuilding>();
            //tipwindowBuilding.mbTip = this;


            if (!settings.Hidden)
            {
                SetRepeatRate(15);
                StartRepeatingWorker();
                ChangeTip();
            }

            //Now do a background download of the tips as well


#if DEBUG
            debugwin = gameObject.AddComponent<TransferWindowPlanner.TipsWindowDebug>();
            debugwin.WindowRect = new Rect(0, 0, 100, 500);
#endif
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

            texBox = new Texture2D(9, 9,TextureFormat.ARGB32,false);
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
        }

        internal override void OnGUIEvery()
        {
            ///////////////////////////////////////////////////////////////////////////
            //Removed Building MouseOver Functionality
            ///////////////////////////////////////////////////////////////////////////
            //if (settings.Hidden && tipwindowBuilding.Visible==false)

            if (settings.Hidden)
                {
                if (GUI.Button(new Rect(0, Screen.height - 56, 20, 20), "?",HighLogic.Skin.button))
                {
                    settings.Hidden = false;
                    settings.Save();
                    tipwindow.Visible = true;
                    SetRepeatRate(15);
                    StartRepeatingWorker();
                    ChangeTip();

                }
            }
        }

        internal override void RepeatingWorker()
        {
            ChangeTip();
        }

        internal override void OnDestroy()
        {
            LogFormatted("KSPTips Destroyed");


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

            tipwindow.thistip = lstTips[DisplayedTip];

            LogFormatted("NewTip:{0}-{1}-{2}-{3}", DisplayedTip, tipwindow.thistip.Question, tipwindow.thistip.Answer, tipwindow.thistip.Image);
            tipwindow.texImage = new Texture2D(64,64,TextureFormat.ARGB32,false);
            if (tipwindow.thistip.Image != "") {
                tipwindow.thistip.ImageLoaded = ExtractToTexture(ref tipwindow.texImage, tipwindow.thistip.Image);
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

        public void loadTips()
        {
            //Extract the Tips file resource if theres no file
            if (!System.IO.File.Exists(PathPlugin + "/Tips.cfg"))
                System.IO.File.WriteAllBytes(PathPlugin + "/Tips.cfg", Properties.Resources.Tips);
            
            lstTips = new List<Tip>();

            LogFormatted(PathPlugin + "/Tips.cfg");
            ConfigNode cnToLoad = ConfigNode.Load(PathPlugin + "/Tips.cfg");

            LogFormatted("{0}",cnToLoad.nodes.Count);
            foreach (ConfigNode item in cnToLoad.nodes)
            {
                Tip tmp = new Tip();
                tmp.Question = item.GetValue("Question");
                tmp.Answer = item.GetValue("Answer");
                tmp.Image = item.GetValue("Image");
                tmp.GameMode = item.GetValue("GameMode");
                tmp.ModAssembly = item.GetValue("ModAssembly");

                lstTips.Add(tmp);
            }

            if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
            {
                LogFormatted_DebugOnly("{0}-{1}", lstTips.Count, lstTips.Where(t => t.GameMode == null || t.GameMode.ToLower() != "career").Count());
                lstTips = lstTips.Where(t => t.GameMode == null || t.GameMode.ToLower() != "career").ToList();
            }

            //lstTips = lstTips.Where(tip => tip.ModAssembly == "" ||
            //        (AssemblyLoader.loadedAssemblies
            //            .Select(a => a.assembly.GetExportedTypes())
            //            .SelectMany(t => t)
            //            .Any(t => t.FullName.ToLower().EndsWith("." + tip.ModAssembly.ToLower()))
            //        )
            //    ).ToList();

            System.Random rand = new System.Random();
            lstTips = lstTips.OrderBy(t => rand.Next()).ToList();
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