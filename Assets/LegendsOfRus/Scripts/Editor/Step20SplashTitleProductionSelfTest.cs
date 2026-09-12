#if UNITY_EDITOR
using UnityEditor;

namespace LegendsOfRus.Editor
{
    public static class Step20SplashTitleProductionSelfTest
    {
        [MenuItem("Legends of Rus/Production UI/v1.20 Visual Page 01/Validate Splash-Title Production Page")]
        public static void Run() { SplashTitleR3Validation.ValidateMenu(); }
        [MenuItem("Legends of Rus/Production UI/v1.20 Visual Page 01/Validate Splash-Title Source Integration Only")]
        public static void RunSourceChecks() { SplashTitleR3Validation.ValidateMenu(); }
    }
}
#endif
