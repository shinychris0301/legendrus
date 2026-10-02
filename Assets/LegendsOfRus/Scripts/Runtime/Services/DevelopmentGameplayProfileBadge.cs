#if UNITY_EDITOR || (UNITY_ANDROID && DEVELOPMENT_BUILD)
using UnityEngine;

namespace LegendsOfRus
{
    // Debug-only identifier on the existing services root; no extra Canvas or input target.
    internal sealed class DevelopmentGameplayProfileBadge : MonoBehaviour
    {
        void OnGUI()
        {
            if (!DevelopmentGameplayProfile.Active) return;
            var safe = Screen.safeArea;
            float scale = Mathf.Max(1f, Screen.width / 540f);
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(12 * scale),
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = Color.yellow;
            GUI.Label(new Rect(safe.x, Screen.height - safe.yMax, safe.width, 22 * scale),
                "QA · DISPOSABLE SAVE", style);
        }
    }
}
#endif
