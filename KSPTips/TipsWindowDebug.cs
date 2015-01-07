using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KSPTips
{

#if DEBUG
    [WindowInitials(DragEnabled = true, Visible = true)]
    class TipsWindowDebug : MonoBehaviourWindow
    {
        public Int32 intTest1 = 10;
        public Int32 intTest2 = 3;
        public Int32 intTest3 = 3;
        public Int32 intTest4 = 0;
        public Int32 intTest5 = 0;
        public Int32 intTest6 = 0;
        public Int32 intTest7 = 0;
        public Int32 intTest8 = 0;
        public Int32 intTest9 = 0;
        public Int32 intTest10 = 0;


        internal Boolean DrawTextBox(ref String strVar, params GUILayoutOption[] options)
        {
            return DrawTextBox(ref strVar, SkinsLibrary.CurrentSkin.textField, options);
        }
        internal Boolean DrawTextBox(ref String strVar, GUIStyle style, params GUILayoutOption[] options)
        {
            String strOld = strVar;
            strVar = GUILayout.TextField(strVar, style, options);

            return false;
        }

        internal Boolean DrawTextBox(ref Int32 intVar, params GUILayoutOption[] options)
        {
            return DrawTextBox(ref intVar, SkinsLibrary.CurrentSkin.textField, options);
        }
        internal Boolean DrawTextBox(ref Int32 intVar, GUIStyle style, params GUILayoutOption[] options)
        {
            String strRef = intVar.ToString();
            DrawTextBox(ref strRef, style, options);
            Int32 intOld = intVar;
            intVar = Convert.ToInt32(strRef);
            return false;
        }

        internal Boolean DrawTextBox(ref Double dblVar, params GUILayoutOption[] options)
        {
            return DrawTextBox(ref dblVar, SkinsLibrary.CurrentSkin.textField, options);
        }
        internal Boolean DrawTextBox(ref Double dblVar, GUIStyle style, params GUILayoutOption[] options)
        {
            String strRef = dblVar.ToString();
            DrawTextBox(ref strRef, style, options);
            Double dblOld = dblVar;
            dblVar = Convert.ToDouble(strRef);
            return false;
        }

        internal override void DrawWindow(int id)
        {
            try
            {
                DrawTextBox(ref intTest1);
                DrawTextBox(ref intTest2);
                DrawTextBox(ref intTest3);
                DrawTextBox(ref intTest4);
                DrawTextBox(ref intTest5);
                DrawTextBox(ref intTest6);
                DrawTextBox(ref intTest7);
                DrawTextBox(ref intTest8);
                DrawTextBox(ref intTest9);
                DrawTextBox(ref intTest10);

                GUILayout.Label(HighLogic.CurrentGame.editorFacility.ToString());
                GUILayout.Label(EditorLogic.VesselRotation.ToString());
                GUILayout.Label(Vector3d.up.ToString());
                GUILayout.Label((EditorLogic.VesselRotation * Vector3d.up).ToString());

                GUILayout.Label(((EditorLogic.VesselRotation * Vector3d.up) == Vector3.up).ToString());

                GUILayout.Label(Windows.Guides.isEditorVAB.ToString());
            }
            catch (Exception)
            {

            }

        }


    }
#endif
}
