#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System.Reflection;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Unity 6 Player-settings helper.
    /// Active Input Handling is set programmatically.
    /// Dynamic Batching is intentionally changed manually because the public
    /// PlayerSettings batching API is deprecated as of Unity 6000.5.
    /// </summary>
    public static class Unity6ProjectSettingsMigration
    {
        [MenuItem("Legends of Rus/Unity 6/Apply Input System (New)")]
        public static void ApplyInputSystem()
        {
            bool changed = SetNewInputSystemOnly();
            AssetDatabase.SaveAssets();

            string message =
                "Active Input Handling is set to Input System Package (New).\n\n" +
                "Unity 6000.5 also deprecates Dynamic Batching. Open Player settings and " +
                "turn Dynamic Batching OFF under Other Settings > Rendering.";

            if (changed)
                message += "\n\nRestart Unity once so the new input backend is fully activated.";

            EditorUtility.DisplayDialog("Legends of Rus — Unity 6.5", message, "OK");
            SettingsService.OpenProjectSettings("Project/Player");

            Debug.Log(
                "Legends of Rus: Input System Package (New) selected. " +
                "Disable Dynamic Batching manually in Player > Other Settings > Rendering."
            );
        }

        [MenuItem("Legends of Rus/Unity 6/Open Player Settings")]
        public static void OpenPlayerSettings()
        {
            SettingsService.OpenProjectSettings("Project/Player");
        }

        [MenuItem("Legends of Rus/Unity 6/Validate Input Setting")]
        public static void Validate()
        {
            var playerSettings = GetCurrentPlayerSettings();
            int inputHandler = -1;

            if (playerSettings != null)
            {
                var so = new SerializedObject(playerSettings);
                var prop = so.FindProperty("activeInputHandler");
                if (prop != null)
                    inputHandler = prop.intValue;
            }

            Debug.Log(
                $"Legends of Rus Unity 6 validation: activeInputHandler={inputHandler} " +
                "(expected 1 = Input System Package (New))."
            );
        }

        static bool SetNewInputSystemOnly()
        {
            var playerSettings = GetCurrentPlayerSettings();
            if (playerSettings == null)
            {
                Debug.LogError("Could not resolve PlayerSettings.");
                return false;
            }

            var serialized = new SerializedObject(playerSettings);
            var prop = serialized.FindProperty("activeInputHandler");
            if (prop == null)
            {
                Debug.LogError("Could not find PlayerSettings.activeInputHandler.");
                return false;
            }

            if (prop.intValue == 1)
                return false;

            // 0 = Input Manager (Old), 1 = Input System Package (New), 2 = Both.
            prop.intValue = 1;
            serialized.ApplyModifiedProperties();
            return true;
        }

        static PlayerSettings GetCurrentPlayerSettings()
        {
            var buildProfileType = typeof(BuildProfile);
            var globalField = buildProfileType.GetField(
                "s_GlobalPlayerSettings",
                BindingFlags.Static | BindingFlags.NonPublic
            );

            if (globalField == null)
                return null;

            var playerSettings = globalField.GetValue(null) as PlayerSettings;
            var activeProfile = BuildProfile.GetActiveBuildProfile();

            if (activeProfile != null)
            {
                var overrideField = buildProfileType.GetField(
                    "m_PlayerSettings",
                    BindingFlags.Instance | BindingFlags.NonPublic
                );

                if (overrideField != null)
                {
                    var profileSettings = overrideField.GetValue(activeProfile) as PlayerSettings;
                    if (profileSettings != null)
                        playerSettings = profileSettings;
                }
            }

            return playerSettings;
        }
    }
}
#endif
