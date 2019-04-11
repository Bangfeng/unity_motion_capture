#if !NET_4_6
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rokoko.MotionLibraryCompatibility
{
    internal class MotionLibraryWindowCompatibility : EditorWindow
    {
        bool refreshing = false;
        internal class Fonts
        {
            public static Font normalText = (Font)Resources.Load("Fonts/ProximaNova/Regular/proximanova-regular-webfont");
            public static Font boldText = (Font)Resources.Load("Fonts/ProximaNova/Bold/proximanova-bold-webfont");
        }

        bool reloadingAssembly = false;
        // Add menu item named "My Window" to the Window menu
        [MenuItem("Window/Motion Library")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(MotionLibraryWindowCompatibility));
        }

        private void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += AssemblyReloadEvents_beforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += AssemblyReloadEvents_afterAssemblyReload;
            minSize = new Vector2(650, 180);
        }

        private void AssemblyReloadEvents_afterAssemblyReload()
        {
            reloadingAssembly = false;
        }

        private void AssemblyReloadEvents_beforeAssemblyReload()
        {
            reloadingAssembly = true;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= AssemblyReloadEvents_beforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= AssemblyReloadEvents_afterAssemblyReload;
        }

        void OnGUI()
        {
            // Bold text style
            GUIStyle boldText = new GUIStyle(GUI.skin.label);
            boldText.alignment = TextAnchor.MiddleCenter;
            boldText.font = Fonts.boldText;
            // Normal text style
            GUIStyle centeredText = new GUIStyle(GUI.skin.label);
            centeredText.font = Fonts.normalText;
            centeredText.wordWrap = true;
            centeredText.alignment = TextAnchor.MiddleCenter;
            // centeredText.normal.textColor = "#EEEEEEFF".ToColor();
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fixedWidth = 260;
            buttonStyle.fixedHeight = 50;

            GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            GUILayout.Label("Motion Library needs different fuel", boldText);
            GUILayout.Space(25);
            
            GUILayout.Label("You are currently using an older version of .NET, however Motion Library requires you to use .NET api version 4.x. You can easily switch by clicking the button below.", centeredText);
            //GUILayout.Label("To fix this, please set:", centeredText);
            bool atLeastOneProblem = false;
            if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest)
            {
                atLeastOneProblem = true;
                GUILayout.Label("", centeredText);
            }
            if (PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup) != ApiCompatibilityLevel.NET_4_6)
            {
                atLeastOneProblem = true;
                GUILayout.Label("", centeredText);
            }
            GUILayout.Space(10);
            GUI.enabled = atLeastOneProblem;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Please, change setting and close Unity", buttonStyle))
            {
                bool needsRestart = false;
                if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest)
                {
                    if (EditorUtility.DisplayDialog("You need to have set the scripting runtime version to .NET 4.x Equivalent", "So like we said The Motion Library plugin can do this for you, but remember you need to manually re-open Unity and the project afterwards. Do you want to proceed?", "Yes, change it and close Unity", "No, not right now"))
                    {
                        Close();
                        needsRestart = true;

                        if (PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup) != ApiCompatibilityLevel.NET_4_6)
                        {
                            PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_4_6);
                        }

                        PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
                        if (needsRestart)
                        {
                            EditorApplication.Exit(0);
                        }
                    }
                } else
                {
                    if (PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup) != ApiCompatibilityLevel.NET_4_6)
                    {
                        PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_4_6);
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = true;
        }
    }

    [InitializeOnLoad]
    internal class MotionLibraryWindowCompatibilityRecompilation
    {
        static MotionLibraryWindowCompatibilityRecompilation()
        {
            bool atLeastOneProblem = false;
            if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest)
            {
                atLeastOneProblem = true;
            }
            if (PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup) != ApiCompatibilityLevel.NET_4_6)
            {
                atLeastOneProblem = true;
            }
            if (!atLeastOneProblem)
            {
                PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_2_0);
                PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_4_6);
            }

        }
    }
}
#endif