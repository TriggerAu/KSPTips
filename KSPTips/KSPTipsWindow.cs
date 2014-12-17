﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPTips
{
    [WindowInitials(DragEnabled=false,Visible=true,TooltipsEnabled=true)]
    class KSPTipsWindow:MonoBehaviourWindow
    {
        internal KSPTips mbTip;

        internal Tip thistip;
        internal Texture2D texImage;

        GUIStyle tipQlabel,tipAlabel;

        GUIStyle styleButton;


        internal override void OnGUIOnceOnly()
        {

            styleButton = new GUIStyle(HighLogic.Skin.button);

            WindowStyle = new GUIStyle();
            WindowStyle.normal.background = mbTip.texBox;
            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
            WindowStyle.border = new RectOffset(3, 3, 3, 3);

            //SkinsLibrary.InitSkinList();

            //WindowStyle.stretchWidth = true;
            //WindowStyle.margin = new RectOffset(0, 0, 0, 0);
            //WindowStyle.padding = new RectOffset(0, 0, 0, 0);

            tipQlabel = new GUIStyle(SkinsLibrary.DefUnitySkin.label);
            tipQlabel.padding = new RectOffset(62, 40, 0, 0);
            tipQlabel.fontSize = 13;
            tipQlabel.fontStyle = FontStyle.Bold;
            tipQlabel.wordWrap = false;
            tipQlabel.clipping = TextClipping.Clip;
            tipQlabel.stretchWidth = false;
            tipQlabel.padding.top = tipQlabel.padding.bottom = -1;
                        
            tipAlabel = new GUIStyle(tipQlabel);
            tipAlabel.fontSize = 12;
            tipAlabel.fontStyle = FontStyle.Normal;
            tipAlabel.wordWrap = true;
        }


        internal override void OnGUIEvery()
        {
            WindowRect.height = 56;
            WindowRect.width = 400;
            WindowRect.x = 0;
            WindowRect.y = Screen.height - (WindowRect.height + 36);

            tipQlabel.fontSize = mbTip.debugwin.intTest1;

            if (Visible && drawingStarted)
            {
                if (thistip.Image != "" && thistip.ImageLoaded)
                {
                    GUI.Box(new Rect(-2, Screen.height - (64 + 36 + 1 ) , 64,64), texImage, new GUIStyle());
                }

                Texture2D texPlayPause = mbTip.texPlay;
                if (mbTip.RepeatingWorkerRunning)
                    texPlayPause = mbTip.texPause;

                if (GUI.Button(new Rect(WindowRect.x + WindowRect.width - 44 , WindowRect.y - 8, 20, 20), new GUIContent(texPlayPause, "Play / Pause"), styleButton))
                {
                    if (mbTip.RepeatingWorkerRunning)
                        mbTip.StopRepeatingWorker();
                    else
                        mbTip.StartRepeatingWorker();
                }

                if (GUI.Button(new Rect(WindowRect.x + WindowRect.width - 22 , WindowRect.y - 8, 20, 20), new GUIContent(mbTip.texCross, "Next Tip"), styleButton))
                {
                    KSPTips.settings.Hidden = true;
                    KSPTips.settings.Save();
                    Visible = false;
                }


                if (GUI.Button(new Rect(WindowRect.x + WindowRect.width - 44, WindowRect.y + 22 - 8, 20, 20), new GUIContent(mbTip.texPrev, "Prev Tip"), styleButton))
                {
                    if (mbTip.RepeatingWorkerRunning)
                    {
                        mbTip.ChangeTip(-2);
                        mbTip.StopRepeatingWorker();
                        mbTip.StartRepeatingWorker();
                    }
                    else { mbTip.ChangeTip(-1); }

                }

                if (GUI.Button(new Rect(WindowRect.x + WindowRect.width - 22 , WindowRect.y + 22 - 8, 20, 20), new GUIContent(mbTip.texNext, "Next Tip"), styleButton))
                {
                    if (mbTip.RepeatingWorkerRunning)
                    {
                        mbTip.StopRepeatingWorker();
                        mbTip.StartRepeatingWorker();
                    }
                    else { mbTip.ChangeTip(+1); }
                }

                GUI.depth = 0;
            }
        }

        private Boolean drawingStarted = false;
        internal override void DrawWindow(int id)
        {
            if (!drawingStarted) drawingStarted = true;
            GUILayout.BeginVertical();
            GUILayout.Space(6);
            GUILayout.Label(thistip.Question, tipQlabel,GUILayout.Width(390));
            GUILayout.Label(thistip.Answer,tipAlabel);
            GUILayout.EndHorizontal();

            //GUI.Box(new Rect(debugwin.intTest5, debugwin.intTest6, 50, 50), texImage, new GUIStyle());

        }


    }
}
