using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSPPluginFramework;
using UnityEngine;

namespace KSPPluginFramework
{
    internal class AppLauncherButtonWrapper
    {
        internal AppLauncherButtonWrapper(Texture2D ButtonTexture)
        {
            texButton = ButtonTexture;
        }

        internal ApplicationLauncherButton btnAppLauncher = null;
        internal Texture2D texButton = new Texture2D(38,38,TextureFormat.ARGB32,false);

        internal event EventHandler onHover;
        internal event EventHandler onHoverOff;
        internal event EventHandler onTrue;
        internal event EventHandler onFalse;

        internal ApplicationLauncherButton InitAppLauncherButton()
        {
            btnAppLauncher = null;
            try
            {
                btnAppLauncher = ApplicationLauncher.Instance.AddModApplication(
                    onAppLaunchToggleOn, onAppLaunchToggleOff,
                    onAppLaunchHoverOn, onAppLaunchHoverOff,
                    null, null,
                    ApplicationLauncher.AppScenes.ALWAYS,
                    texButton);

                //AppLauncherButtonMutuallyExclusive(settings.AppLauncherMutuallyExclusive);

                //btnAppLauncher = ApplicationLauncher.Instance.AddApplication(
                //    onAppLaunchToggleOn, onAppLaunchToggleOff,
                //    onAppLaunchHoverOn, onAppLaunchHoverOff,
                //    null, null,
                //    (Texture)Resources.texAppLaunchIcon);
                //btnAppLauncher.VisibleInScenes = ApplicationLauncher.AppScenes.FLIGHT;

            }
            catch (Exception ex)
            {
                MonoBehaviourExtended.LogFormatted("AppLauncher: Failed to set up App Launcher Button\r\n{0}", ex.Message);
                btnAppLauncher = null;
            }

            return btnAppLauncher;
        }

        internal void DestroyAppLauncherButton()
        {
            MonoBehaviourExtended.LogFormatted("AppLauncher: Destroying Button-BEFORE NULL CHECK");
            if (btnAppLauncher != null)
            {
                MonoBehaviourExtended.LogFormatted("AppLauncher: Found button to destroy");
                ApplicationLauncher.Instance.RemoveModApplication(btnAppLauncher);
                btnAppLauncher = null;
            }
        }

        internal void OnGUIAppLauncherReady()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("AppLauncherReady");
            if (ApplicationLauncher.Ready)
            {
                InitAppLauncherButton();
            }
            else
            {
                MonoBehaviourExtended.LogFormatted("App Launcher-Not Actually Ready");
            }
        }

        internal void OnGUIAppLauncherUnreadify(GameScenes SceneToLoad)
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("GameSceneLoadRequest");
            DestroyAppLauncherButton();
        }


        void onAppLaunchToggleOn()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("TOn");
            if (onTrue!=null)
                onTrue(this,new EventArgs());

        }
        void onAppLaunchToggleOff()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("TOff");
            if (onFalse != null)
                onFalse(this, new EventArgs());
        }
        void onAppLaunchHoverOn()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("HovOn");
            if (onHover != null)
                onHover(this, new EventArgs());
        }
        void onAppLaunchHoverOff()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("HovOff");
            if (onHoverOff != null)
                onHoverOff(this, new EventArgs());
        }
    }
}
