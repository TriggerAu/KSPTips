///////////////////////////////////////////////////////////////////////////
//Removed Building MouseOver Functionality
///////////////////////////////////////////////////////////////////////////

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using KSP;
//using UnityEngine;
//using KSPPluginFramework;

//namespace KSPTips
//{
//    [WindowInitials(DragEnabled = false)]
//    class KSPTipsWindowBuilding:MonoBehaviourWindow
//    {
//        internal KSPTips mbTip;

//        internal TipBuilding thistip;

//        GUIStyle styleLabel;
//        GUIStyle styleHeading, styleDescription,styleFacilityLevel;
//        GUIStyle styleLevelsHeading, styleLevelNum, styleLevelText, styleLevelNumActive, styleLevelTextActive;

//        internal override void OnGUIOnceOnly()
//        {
//            WindowStyle = new GUIStyle();
//            WindowStyle.normal.background = mbTip.texBox;
//            //Extra border to prevent bleed of color - actual border is only 1 pixel wide
//            WindowStyle.border = new RectOffset(3, 3, 3, 3);
            
//            styleLabel = new GUIStyle(SkinsLibrary.DefUnitySkin.label);
//            styleLabel.fontSize = 11;

//            styleHeading = new GUIStyle(styleLabel);
//            styleHeading.fontStyle = FontStyle.Bold;
//            styleHeading.normal.textColor = new Color32(169, 255, 4, 255);

//            styleDescription = new GUIStyle(styleLabel);
//            styleDescription.wordWrap = true;
            
//            styleFacilityLevel= new GUIStyle(styleLabel);
//            styleFacilityLevel.fontStyle =  FontStyle.Bold;
//            styleFacilityLevel.normal.textColor = new Color32(221, 138, 17, 255);

//            styleLevelsHeading = new GUIStyle(styleLabel);
//            styleLevelsHeading.fontStyle = FontStyle.Bold;

//            styleLevelNum = new GUIStyle(styleLabel);
//            styleLevelNum.normal.textColor = new Color32(192 , 192, 192, 255);
//            styleLevelText = new GUIStyle(styleLabel);
//            styleLevelText.normal.textColor = new Color32(192, 192, 192, 255);

//            styleLevelNumActive = new GUIStyle(styleLevelNum);
//            styleLevelNumActive.normal.textColor = new Color32(169, 255, 4, 255);
//            styleLevelTextActive = new GUIStyle(styleLevelText);
//            styleLevelTextActive.normal.textColor = new Color32(169, 255, 4, 255);

 
//        }

//        internal override void OnGUIEvery()
//        {
//            WindowRect.x = 0;
//            WindowRect.width = 400;
//            WindowRect.y = Screen.height - 280;
//            WindowRect.height = 280-36;
//        }

//        internal override void DrawWindow(int id)
//        {
//            GUILayout.BeginVertical();
//            GUILayout.Label(thistip.Name,styleHeading);
//            GUILayout.Label(thistip.Description, styleDescription);

//            GUILayout.Label(String.Format("Facility Level: {0}", thistip.Facility.FacilityLevel + 1),styleFacilityLevel);

//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Facility Level",styleLevelsHeading,GUILayout.Width(80));
//            GUILayout.Label("Game Mechanic",styleLevelsHeading);
//            GUILayout.EndHorizontal();

//            foreach (TipFacilityLevel ftip in thistip.FacilityLevels)
//            {
//                GUILayout.BeginHorizontal();
//                if (ftip.Level<=(thistip.Facility.FacilityLevel+1))
//                {
//                    GUILayout.Label(String.Format("Level {0}", ftip.Level), styleLevelNumActive, GUILayout.Width(80));
//                    GUILayout.Label(String.Format("{0}", ftip.Text), styleLevelTextActive);
//                }
//                else
//                {
//                    GUILayout.Label(String.Format("Level {0}", ftip.Level), styleLevelNum, GUILayout.Width(80));
//                    GUILayout.Label(String.Format("{0}", ftip.Text), styleLevelText);
//                }
//                GUILayout.EndHorizontal();
//            }
//            GUILayout.EndVertical();
//        }
//    }
//}
