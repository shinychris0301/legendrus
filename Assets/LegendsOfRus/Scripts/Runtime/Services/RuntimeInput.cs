using UnityEngine.InputSystem;

namespace LegendsOfRus
{
    /// <summary>
    /// Unity 6 input facade. The project intentionally uses only the new Input System.
    /// This avoids UnityEngine.Input / legacy Input Manager dependencies.
    /// </summary>
    public static class RuntimeInput
    {
        public static bool EscapePressedThisFrame()
        {
            var keyboard = Keyboard.current;
            return keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
        }

        public static bool QaCapturePressedThisFrame()
        {
            var keyboard = Keyboard.current;
            return keyboard != null && keyboard.f12Key.wasPressedThisFrame;
        }
    }
}
