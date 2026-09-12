#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave03Step93CutoverDecision
    {
        [MenuItem("Legends of Rus/Production UI/Evaluate v1.20 Step 9.3 Wave 3 Cutover Decision")]
        public static void Evaluate()
        {
            const string decision = "NO_GO_FULL_WAVE03_CUTOVER";
            if (!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP09_3_CUTOVER_DECISION.json"))
                throw new FileNotFoundException("Missing Step 9.3 cutover decision document.");
            Debug.Log("v1.20 Step 9.3 cutover decision: " + decision + ". Direct approved-reference coverage is 0/8; refined candidates remain inactive.");
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus", decision + "\n\nStep 9.3 improves candidate fidelity but cannot authorize Wave 3 production cutover without direct approved references.", "OK");
        }
    }
}
#endif
