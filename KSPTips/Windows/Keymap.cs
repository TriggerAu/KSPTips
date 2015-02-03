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
    class Keymap:MonoBehaviourWindowPlus
    {
        internal static Boolean isEditorVAB { get { return ((EditorLogic.VesselRotation * Vector3d.up) == Vector3.up); } }

        internal KSPTips mbTip;

        internal static Boolean isFlight = true;


        internal override void Awake()
        {
            
            base.Awake();
        }



        internal override void DrawWindow(int id)
        {
            GUILayout.BeginVertical();



            GUILayout.EndVertical();

        }
    }
}
