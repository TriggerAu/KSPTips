using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Linq;
using System.Text;

using KSPPluginFramework;
using UnityEngine;
using KSP;


namespace KSPTips.Windows
{
    [WindowInitials(Caption="",DragEnabled=true,TooltipsEnabled=true,Visible=false)]
    public class Guides: MonoBehaviourWindowPlus
    {
        internal enum WindowDisplayEnum
        {
            [Description("Guide Pages")]
            GuidePages,
            [Description("Keyboard Maps")]
            KeyboardMap
        }
        
        internal static Boolean isEditorVAB { get { return ((EditorLogic.VesselRotation * Vector3d.up) == Vector3.up); } }

        internal KSPTips mbTip;

        internal WindowDisplayEnum WindowToDisplay = WindowDisplayEnum.GuidePages;
        internal DropDownList ddlWindowDisplay;

        internal Int32 CurrentPage = 0;
        internal Texture2D texPage = new Texture2D(1500,1000,TextureFormat.ARGB32,false);


        GUIStyle styleButton, stylePage, styleTitle, stylePageNums, styleToggle, styleKeyMapTex;

        internal static GUIStyle styleDropDownGlyph;
        internal static GUIStyle styleSeparatorV;


        internal static GUIStyle styleDropDownButton,styleDropDownListBox,styleDropDownListItem;

        List<GuidePage> lstPages { 
            get {
                if (listCanBeFiltered && listIsFiltered)
                    return lstPagesFiltered;
                else
                    return KSPTips.lstGuidePages;
            }
        }

        List<GuidePage> lstPagesFiltered;
        
        Boolean listCanBeFiltered = false;
        Boolean listIsFiltered = false;

        DropDownList ddlGuide;

        internal override void Awake()
        {
            ddlGuide = new DropDownList(KSPTips.lstGuides.Select(g=>g.Title).ToList(),this);
            ddlManager.Add(ddlGuide);

            ddlWindowDisplay = new DropDownList(EnumExtensions.ToEnumDescriptions<WindowDisplayEnum>() ,this);
            ddlManager.Add(ddlWindowDisplay);

            ddlGuide.OnSelectionChanged += ddlGuide_OnSelectionChanged;

            base.Awake();
        }

        internal override void Start()
        {
            listIsFiltered = true;
            if (HighLogic.LoadedScene == GameScenes.EDITOR)
            {
                listCanBeFiltered = true;
                if (isEditorVAB) {
                    lstPagesFiltered = KSPTips.lstGuidePages.Where(p => p.guide.TargetScene == "VAB").ToList();
                } else {
                    lstPagesFiltered = KSPTips.lstGuidePages.Where(p => p.guide.TargetScene == "SPH").ToList();
                }
            }
            else if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                listCanBeFiltered = true;
                lstPagesFiltered = KSPTips.lstGuidePages.Where(p => p.guide.TargetScene == "Flight").ToList();
            }
            else
            {
                listCanBeFiltered = false;
                listIsFiltered = false;
            }
            CurrentPage = 0;
            UpdateGuidePage();
            ResetddlGuideList();

            RemoveInputLock();

            base.Start();
        }

        void ResetddlGuideList()
        {
            if (listIsFiltered)
            {
                if (HighLogic.LoadedScene == GameScenes.EDITOR)
                {
                    if (isEditorVAB)
                        ddlGuide.Items = KSPTips.lstGuides.Where(p => p.TargetScene == "VAB").Select(g => g.Title).ToList();
                    else
                        ddlGuide.Items = KSPTips.lstGuides.Where(p => p.TargetScene == "SPH").Select(g => g.Title).ToList();
                }
                else if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                {
                    ddlGuide.Items = KSPTips.lstGuides.Where(p => p.TargetScene == "Flight").Select(g => g.Title).ToList();
                }
                else
                {
                    ddlGuide.Items = KSPTips.lstGuides.Select(g => g.Title).ToList();
                }
                ddlGuide.SelectedIndex = 0;
            }
            else
            {
                ddlGuide.Items = KSPTips.lstGuides.Select(g => g.Title).ToList();
            }

        }

        internal override void OnDestroy()
        {
            ddlGuide.OnSelectionChanged -= ddlGuide_OnSelectionChanged;
            base.OnDestroy();
        }

        internal override void Update()
        {
            if (Visible && Input.GetKey(KeyCode.Escape))
                mbTip.AppButton.btnAppLauncher.SetFalse(true);

            base.Update();
        }

        void ddlGuide_OnSelectionChanged(MonoBehaviourWindowPlus.DropDownList sender, int OldIndex, int NewIndex)
        {
            GuidePage tmp = KSPTips.lstGuidePages.FirstOrDefault(p => p.guide.Title == ddlGuide.Items[NewIndex]);
            CurrentPage = lstPages.IndexOf(tmp);
            UpdateGuidePage();
        }

        internal override void DrawWindow(int id)
        {
            GUILayout.BeginVertical();
            
            DrawWindow_Header();
            
            switch (WindowToDisplay) {
                case WindowDisplayEnum.KeyboardMap:
                    DrawWindow_KeyboardMap();
                    break;
                case WindowDisplayEnum.GuidePages:
                default:
                    DrawWindow_Guide();
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawWindow_Header()
        {
            if (WindowToDisplay != WindowDisplayEnum.GuidePages || !listCanBeFiltered)
                GUILayout.Space(4);

            GUILayout.BeginHorizontal(GUILayout.Height(WindowToDisplay == WindowDisplayEnum.GuidePages && listCanBeFiltered ? 32 : 24));

            GUILayout.Space(4);
            ///Buttone for toolbar type here
            ///

            GUILayout.Label("Info: ", styleTitle);
            ddlWindowDisplay.DrawButton();
            WindowToDisplay = (WindowDisplayEnum)ddlWindowDisplay.SelectedIndex;

            GUILayout.Space(mbTip.debugwin.intTest1);

            switch (WindowToDisplay) {
                case WindowDisplayEnum.KeyboardMap:
                    DrawWindow_Header_KeyboardMap();
                    break;
                case WindowDisplayEnum.GuidePages:
                default:
                    DrawWindow_Header_GuidePages();
            break;
            }
            

            if (GUILayout.Button("X", styleButton)) {
                mbTip.AppButton.btnAppLauncher.SetFalse(true);
            }
            GUILayout.Space(2);

            GUILayout.EndHorizontal();

        }

        Int32 BuildOrFlight = 1;
        Int32 LightOrDark = 0;
        Boolean blnZoom = false;
        String KeyboardMapTexture = "";
        internal Texture2D texKeyboardMap = new Texture2D(2560, 1810, TextureFormat.ARGB32, false);
        private void DrawWindow_Header_KeyboardMap()
        {
            //Flight or Build
            GUILayout.Label("Layout: ", styleTitle);
            BuildOrFlight = GUILayout.Toolbar(BuildOrFlight, new String[] { "Build Map", "Flight Map" }, styleButton);

            //LightOrDark
            GUILayout.Space(mbTip.debugwin.intTest2);
            GUILayout.Label("Background: ", styleTitle);
            LightOrDark = GUILayout.Toolbar(LightOrDark, new String[] { "Light", "Dark" }, styleButton);

            //Zoom or not
            GUILayout.Space(mbTip.debugwin.intTest3);
            GUILayout.Label("Zoom: ", styleTitle);
            blnZoom = GUILayout.Toggle(blnZoom, "Zoom In", styleButton);

            String NewKeyboardMapTexture = "KeyboardLayout" +
                (BuildOrFlight==0 ? "-Build" : "-Flight") + 
                (LightOrDark==0 ? "" : "-Dark") +
                "_Hires.png";

            if (NewKeyboardMapTexture != KeyboardMapTexture)
            {
                KeyboardMapTexture = NewKeyboardMapTexture;

                if (!KSPTips.LoadImageFromFile(ref texKeyboardMap, "KeyboardMaps/" + KeyboardMapTexture, KSPTips.PathPluginGuideImages))
                {
                    LogFormatted("Unable to load Keyboard Map: {0}", KeyboardMapTexture);
                }

            }

            GUILayout.FlexibleSpace();
        }

        private void DrawWindow_Header_GuidePages()
        {
            // GUILayout.Label(String.Format("{0} - {1}", lstPages[CurrentPage].guide.Title, lstPages[CurrentPage].Title), styleTitle);
            //ddlGuide.styleButton.fixedWidth = mbTip.debugwin.intTest10;
            GUILayout.Label("Section: ", styleTitle);
            ddlGuide.DrawButton();

            if (listCanBeFiltered) {
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                GUILayout.Space(-2);
                Boolean listwasFiltered = listIsFiltered;
                listIsFiltered = GUILayout.Toggle(listIsFiltered, "Scene Specific Guides Only", styleToggle);
                if (listwasFiltered != listIsFiltered) {
                    ResetddlGuideList();
                    if (listIsFiltered) {
                        CurrentPage = 0;
                        UpdateGuidePage();
                    }
                    else {
                        CurrentPage = lstPages.IndexOf(lstPagesFiltered[CurrentPage]);
                    }
                }
                GUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();

            GUILayout.Label(String.Format("Page {0}/{1}", CurrentPage + 1, lstPages.Count), stylePageNums);
            if (GUILayout.Button("<< Prev", styleButton)) {
                CurrentPage--;
                if (CurrentPage < 0)
                    CurrentPage = lstPages.Count - 1;
                UpdateGuidePage();
            }
            if (GUILayout.Button("Next >>", styleButton)) {
                CurrentPage++;
                if (CurrentPage >= lstPages.Count)
                    CurrentPage = 0;
                UpdateGuidePage();
            }
        }

        Vector2 KeyMapScrollPos = new Vector2();
        private void DrawWindow_KeyboardMap()
        {
            GUILayout.Space(-7);

            //draw the texture and drag with the mouse, or maybe scrollbar
            if(!blnZoom){
                GUILayout.Box(texKeyboardMap, stylePage);
            }
            else
            {
                KeyMapScrollPos = GUILayout.BeginScrollView(KeyMapScrollPos);

                GUILayout.Box(texKeyboardMap, styleKeyMapTex);

                GUILayout.EndScrollView();
            }
        }

        private void DrawWindow_Guide()
        {
            GUILayout.Space(-7);

            GUILayout.Box(texPage, stylePage);

        }

        internal override void OnGUIEvery()
        {
            SetPageSize();
            InputLockMonitor();

            base.OnGUIEvery();
        }

        void SetPageSize()
        {
            Int32 MaxHeight = Screen.height - 168;
            Int32 MaxWidth = Screen.width - 124;

            Single CalculatedWidth = (Single)Math.Truncate(MaxHeight * 3.0f / 2.0f);

            //LogFormatted("{0}-{1}-{2}", MaxHeight, MaxWidth, CalculatedWidth);

            if (CalculatedWidth > MaxWidth)
                stylePage.fixedWidth = MaxWidth;
            else
                stylePage.fixedWidth = CalculatedWidth;

            if (stylePage.fixedWidth > 1500)
                stylePage.fixedWidth = 1500;
            else if (stylePage.fixedWidth < 900)
                stylePage.fixedWidth = 900;

            stylePage.fixedHeight = (stylePage.fixedWidth * 2.0f / 3.0f) - 1.0f;
            WindowRect.width = stylePage.fixedWidth + 1;
            WindowRect.height = stylePage.fixedHeight + 25;
        }

        internal override void OnGUIOnceOnly()
        {
            styleButton = new GUIStyle(HighLogic.Skin.button);
            styleButton.fontSize = 14;
            styleButton.padding.top = 2;
            styleButton.padding.bottom = -2;
            styleButton.normal.textColor = Color.white;

            WindowStyle = new GUIStyle();
            WindowStyle.normal.background = mbTip.texBox;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            WindowStyle.border = new RectOffset(3, 3, 3, 3);

            styleTitle = new GUIStyle(HighLogic.Skin.label);
            styleTitle.fontStyle = FontStyle.Bold;
            styleTitle.normal.textColor = Color.white;
            styleTitle.fontSize = 14;
            styleTitle.padding.top = 3;
            styleTitle.padding.bottom = 4;

            stylePageNums = new GUIStyle(HighLogic.Skin.label);
            stylePageNums.normal.textColor = Color.white;
            stylePageNums.fontSize = 12;
            stylePageNums.padding.top = 4;
            stylePageNums.padding.bottom = 0;

            styleToggle = new GUIStyle(HighLogic.Skin.toggle);
            styleToggle.normal.textColor = Color.white;
            styleToggle.fontSize = 12;
            styleToggle.padding.top = 4;
            styleToggle.padding.bottom = 4;


            stylePage = new GUIStyle();
            //stylePage.fixedWidth = mbTip.debugwin.intTest1;
            //stylePage.fixedHeight = mbTip.debugwin.intTest2;// stylePage.fixedWidth * 2 / 3 - 1;
            stylePage.padding.left = 1;
            stylePage.padding.right = -1;
            stylePage.padding.top = -1;
            stylePage.padding.bottom = 1;

            styleKeyMapTex = new GUIStyle(stylePage);
            styleKeyMapTex.fixedWidth = 2560;
            styleKeyMapTex.fixedHeight = 1810;

            styleDropDownGlyph = new GUIStyle();
            styleDropDownGlyph.alignment = TextAnchor.MiddleCenter;

            styleSeparatorV = new GUIStyle();
            Texture2D texDrop = new Texture2D(10,10,TextureFormat.ARGB32,false),texSep = new Texture2D(2,16,TextureFormat.ARGB32,false) ;
            KSPTips.ExtractToTexture(ref texDrop,"img_DropDown");
            KSPTips.ExtractToTexture(ref texSep, "img_SeparatorVertical");
            styleSeparatorV.normal.background =  texSep;
            styleSeparatorV.border = new RectOffset(0, 0, 6, 6);
            styleSeparatorV.fixedWidth = 2;


            ddlManager.DropDownGlyphs = new GUIContentWithStyle(texDrop, styleDropDownGlyph);
            ddlManager.DropDownSeparators = new GUIContentWithStyle("", styleSeparatorV);

            styleDropDownButton = new GUIStyle(styleButton);
            styleDropDownButton.alignment = TextAnchor.MiddleLeft;
            styleDropDownButton.padding.right = 26;
            styleDropDownButton.padding.left = 10;
            styleDropDownButton.fixedWidth = 245;

            ddlGuide.styleButton = styleDropDownButton;
            ddlWindowDisplay.styleButton = styleDropDownButton;
            ddlWindowDisplay.styleButton.fixedWidth = 125;

            styleDropDownListBox = new GUIStyle();
            styleDropDownListBox.normal.background = mbTip.texBox;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            styleDropDownListBox.border = new RectOffset(3, 3, 3, 3);
            ddlGuide.styleListBox = styleDropDownListBox;
            ddlWindowDisplay.styleListBox = styleDropDownListBox;

            styleDropDownListItem = new GUIStyle();
            styleDropDownListItem.normal.textColor = new Color(207, 207, 207);
            Texture2D texBack = CreateColorPixel(new Color(207, 207, 207));
            styleDropDownListItem.hover.background = texBack;
            styleDropDownListItem.onHover.background = texBack;
            styleDropDownListItem.hover.textColor = Color.black;
            styleDropDownListItem.onHover.textColor = Color.black;
            styleDropDownListItem.padding = new RectOffset(4, 4, 3, 4);
            ddlGuide.styleListItem = styleDropDownListItem;
            ddlWindowDisplay.styleListItem = styleDropDownListItem;

            SkinsLibrary.AddStyle("Default", "DropDownButton", styleDropDownButton);
            SkinsLibrary.AddStyle("Default", "DropDownListBox", styleDropDownListBox);
            SkinsLibrary.AddStyle("Default", "DropDownListItem", styleDropDownListItem);


            SetPageSize();
            WindowRect.x = (Screen.width - WindowRect.width) / 2;
            WindowRect.y = (Screen.height - WindowRect.height) / 2;
        }

        internal Boolean isPageImageLoaded = false;
        internal void UpdateGuidePage(){
             
            isPageImageLoaded = KSPTips.LoadImageFromFile(ref texPage, lstPages[CurrentPage].guide.Folder + "/" + lstPages[CurrentPage].Image, KSPTips.PathPluginGuideImages);

            if (ddlGuide.SelectedValue != lstPages[CurrentPage].guide.Title)
            {
                Int32 ddlIndex = ddlGuide.Items.IndexOf(lstPages[CurrentPage].guide.Title);
                ddlGuide.SelectedIndex = ddlIndex;
            }
        }




        internal Boolean MouseOverWindow = false;
        internal Boolean InputLockExists = false;
        void InputLockMonitor()
        {
            MouseOverWindow = Visible && WindowRect.Contains(Event.current.mousePosition);

            //If the setting is on and the mouse is over any window then lock it
            if (MouseOverWindow && !InputLockExists)
            {
                Boolean AddLock = false;
                switch (HighLogic.LoadedScene)
                {
                    case GameScenes.SPACECENTER: AddLock = !(  InputLockManager.GetControlLock("KSPTipsControlLock") != ControlTypes.None); break;
                    case GameScenes.EDITOR: AddLock = !(InputLockManager.GetControlLock("KSPTipsControlLock") != ControlTypes.None); break;
                    case GameScenes.FLIGHT: AddLock = !(InputLockManager.GetControlLock("KSPTipsControlLock") != ControlTypes.None); break;
                    case GameScenes.TRACKSTATION:
                        break;
                    default:
                        break;
                }
                if (AddLock)
                {
                    LogFormatted_DebugOnly("AddingLock-{0}", "KSPTipsControlLock");

                    switch (HighLogic.LoadedScene)
                    {
                        case GameScenes.SPACECENTER: InputLockManager.SetControlLock(ControlTypes.KSC_FACILITIES, "KSPTipsControlLock"); break;
                        case GameScenes.EDITOR: InputLockManager.SetControlLock(ControlTypes.EDITOR_LOCK, "KSPTipsControlLock"); break;
                        case GameScenes.FLIGHT: InputLockManager.SetControlLock(ControlTypes.ALL_SHIP_CONTROLS, "KSPTipsControlLock"); break;
                        case GameScenes.TRACKSTATION:
                            break;
                        default:
                            break;
                    }
                    InputLockExists = true;
                }
            }
            //Otherwise make sure the lock is removed
            else if (!MouseOverWindow && InputLockExists)
            {
                RemoveInputLock();
            }
        }

        internal void RemoveInputLock()
        {
            if (InputLockManager.GetControlLock("KSPTipsControlLock") != ControlTypes.None)
            {
                LogFormatted_DebugOnly("Removing-{0}", "KSPTipsControlLock");
                InputLockManager.RemoveControlLock("KSPTipsControlLock");
            }
            InputLockExists = false;
        }

        
    }
}
