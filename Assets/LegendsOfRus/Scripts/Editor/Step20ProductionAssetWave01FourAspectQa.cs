#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// v1.20 Step 7.2 executable visual QA for the 12 INACTIVE Wave 1 prefabs.
    /// It renders candidate prefabs at four target aspect ratios without changing
    /// ProductionUI/Pages or the runtime registry. Reference metrics are diagnostic;
    /// final signoff remains a human + Android-device decision.
    /// </summary>
    public static class Step20ProductionAssetWave01FourAspectQa
    {
        const string CandidateRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Wave01Pages";
        const string OutputRoot = "ProjectDocs/V1_20_STEP07_2/UnityFourAspectCaptures";
        const float DesignW = 941f;
        const float DesignH = 1672f;
        static readonly ScreenId[] Pages = { ScreenId.SplashTitle, ScreenId.Loading, ScreenId.Home, ScreenId.WorldMap, ScreenId.ChapterMap, ScreenId.PreLevel, ScreenId.Gameplay, ScreenId.Pause, ScreenId.Victory, ScreenId.Fail, ScreenId.ChapterComplete, ScreenId.Dialogue };
        static readonly Vector2Int[] Sizes = { new Vector2Int(941,1672), new Vector2Int(1080,2340), new Vector2Int(1080,2400), new Vector2Int(1600,2560) };

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 7.2 Four-Aspect Candidate Visual QA")]
        public static void Run()
        {
            List<string> failures = Step20ProductionAssetWave01PrefabBuilder.ValidateAll();
            if (failures.Count > 0) throw new InvalidOperationException(string.Join("\n", failures.ToArray()));
            Directory.CreateDirectory(OutputRoot);
            int captures = 0;
            foreach (ScreenId id in Pages)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CandidateRoot + "/" + id + ".prefab");
                foreach (Vector2Int size in Sizes)
                {
                    Texture2D capture = Render(prefab, size);
                    string path = OutputRoot + "/" + id + "_" + size.x + "x" + size.y + ".png";
                    File.WriteAllBytes(path, capture.EncodeToPNG());
                    UnityEngine.Object.DestroyImmediate(capture);
                    captures++;
                }
            }
            File.WriteAllText(OutputRoot + "/UNITY_EXECUTED.lock",
                "v1.20-step07.2-unity-four-aspect-qa\n" +
                "captures=" + captures + "\n" +
                "candidate-prefabs=12\n" +
                "active-runtime-cutover=false\n");
            Debug.Log("v1.20 Step 7.2 four-aspect candidate capture complete: " + captures + "/48. Active cutover remains disabled; review captures and cutover blockers before activation.");
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus", "Four-aspect Wave 1 captures complete: 48/48. Cutover remains disabled.", "OK");
        }

        static Texture2D Render(GameObject prefab, Vector2Int size)
        {
            var rt = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
            var camGo = new GameObject("Step72QaCamera", typeof(Camera));
            Camera cam = camGo.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color32(4,13,24,255); cam.orthographic = true; cam.cullingMask = 1 << 5; cam.targetTexture = rt;
            var canvasGo = new GameObject("Step72QaCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGo.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = cam; canvas.planeDistance = 1f;
            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize; scaler.scaleFactor = Mathf.Min(size.x / DesignW, size.y / DesignH);
            RectTransform viewport = new GameObject("ApprovedDesignViewport", typeof(RectTransform)).GetComponent<RectTransform>(); viewport.SetParent(canvasGo.transform, false); viewport.anchorMin = viewport.anchorMax = new Vector2(.5f,.5f); viewport.pivot = new Vector2(.5f,.5f); viewport.sizeDelta = new Vector2(DesignW,DesignH);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, viewport); RectTransform page = instance.transform as RectTransform; page.anchorMin=Vector2.zero; page.anchorMax=Vector2.one; page.offsetMin=Vector2.zero; page.offsetMax=Vector2.zero;
            Canvas.ForceUpdateCanvases(); cam.Render(); RenderTexture previous = RenderTexture.active; RenderTexture.active = rt;
            var tex = new Texture2D(size.x,size.y,TextureFormat.RGBA32,false); tex.ReadPixels(new Rect(0,0,size.x,size.y),0,0,false); tex.Apply(false,false); RenderTexture.active = previous;
            UnityEngine.Object.DestroyImmediate(canvasGo); UnityEngine.Object.DestroyImmediate(camGo); rt.Release(); UnityEngine.Object.DestroyImmediate(rt); return tex;
        }
    }
}
#endif
