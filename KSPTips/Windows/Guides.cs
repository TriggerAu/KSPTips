using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSPPluginFramework;
using UnityEngine;
using KSP;

namespace KSPTips.Windows
{
    [WindowInitials(Caption="",DragEnabled=true,TooltipsEnabled=true,Visible=false)]
    class Guides: MonoBehaviourWindow
    {
        internal KSPTips mbTip;

        internal Int32 CurrentPage = 0;
        internal Texture2D texPage = new Texture2D(100,100,TextureFormat.ARGB32,false);

        GUIStyle styleButton;

        internal override void Awake()
        {
            LoadGuideImage(0);

            base.Awake();
        }

        internal override void DrawWindow(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Page goes here");

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<< Previous Page"))
            {
                CurrentPage--;
                if (CurrentPage < 0)
                    CurrentPage = KSPTips.lstGuides.Count - 1;
                LoadGuideImage(CurrentPage);
            }
            if (GUILayout.Button("Next Page >>"))
            {
                CurrentPage++;
                if (CurrentPage >= KSPTips.lstGuides.Count )
                    CurrentPage = 0;
                LoadGuideImage(CurrentPage);
            }
            GUILayout.EndHorizontal();

            GUILayout.Box(texPage, GUILayout.MaxWidth(580), GUILayout.MaxHeight(480));

            GUILayout.EndVertical();
        }

        internal override void OnGUIEvery()
        {
            WindowRect.height = mbTip.debugwin.intTest2;
            WindowRect.width = mbTip.debugwin.intTest1;

            base.OnGUIEvery();
        }

        internal override void OnGUIOnceOnly()
        {
            styleButton = new GUIStyle(HighLogic.Skin.button);

            WindowStyle = new GUIStyle();
            WindowStyle.normal.background = mbTip.texBox;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            WindowStyle.border = new RectOffset(3, 3, 3, 3);


        }

        internal void LoadGuideImage(Int32 ImageIndex){

            KSPTips.LoadImageFromFile(ref texPage, KSPTips.lstGuides[ImageIndex].Image, KSPTips.PathPluginGuideImages);

        }
    }
}
