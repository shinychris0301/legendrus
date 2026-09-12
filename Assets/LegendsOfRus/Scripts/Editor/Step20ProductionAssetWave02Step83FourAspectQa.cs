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
    /// Editor-render containment QA for the Step 8.3 refined candidates. This is
    /// a source/candidate layout check, not the later Play-Mode runtime acceptance.
    /// </summary>
    public static class Step20ProductionAssetWave02Step83FourAspectQa
    {
        const string CandidateRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Wave02Pages";
        const string OutputRoot = "ProjectDocs/V1_20_STEP08_3/UnityFourAspectCaptures";
        const float DesignW = 941f;
        const float DesignH = 1672f;

        static readonly ScreenId[] Pages =
        {
            ScreenId.Characters, ScreenId.DailyReward, ScreenId.Profile, ScreenId.Inventory,
            ScreenId.Shop, ScreenId.SpecialOffer, ScreenId.OutOfEnergy, ScreenId.BoosterSelect,
            ScreenId.RewardClaim
        };

        static readonly Vector2Int[] Sizes =
        {
            new Vector2Int(941, 1672), new Vector2Int(1080, 2340),
            new Vector2Int(1080, 2400), new Vector2Int(1600, 2560)
        };

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 8.3 Wave 2 Four-Aspect Refined Candidate QA")]
        public static void Run()
        {
            if (Step20ProductionAssetWave02PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave02PrefabBuilder.BuildAll(false);

            List<string> failures = Step20ProductionAssetWave02PrefabBuilder.ValidateAll();
            failures.AddRange(Step20ProductionAssetWave02Step82ProviderBoundarySelfTest.Validate(false));
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            Directory.CreateDirectory(OutputRoot);
            int captures = 0;
            foreach (ScreenId id in Pages)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CandidateRoot + "/" + id + ".prefab");
                if (prefab == null) throw new InvalidOperationException("Missing Wave 2 candidate prefab: " + id);
                foreach (Vector2Int size in Sizes)
                {
                    Texture2D capture = Render(prefab, size);
                    File.WriteAllBytes(OutputRoot + "/" + id + "_" + size.x + "x" + size.y + ".png", capture.EncodeToPNG());
                    UnityEngine.Object.DestroyImmediate(capture);
                    captures++;
                }
            }

            File.WriteAllText(OutputRoot + "/UNITY_EXECUTED.lock",
                "v1.20-step08.3-wave02-editor-four-aspect-qa\n" +
                "captures=" + captures + "\n" +
                "candidate-prefabs=9\n" +
                "active-wave02-cutover=false\n" +
                "provider-actions-invoked=false\n");

            Debug.Log("v1.20 Step 8.3 four-aspect refined candidate QA complete: " + captures + "/36.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 8.3 four-aspect source/candidate captures complete: 36/36. This does not authorize runtime cutover.", "OK");
        }

        static Texture2D Render(GameObject prefab, Vector2Int size)
        {
            RenderTexture target = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
            target.Create();
            GameObject cameraGo = new GameObject("Step83Wave02QaCamera", typeof(Camera));
            cameraGo.layer = 5;
            Camera camera = cameraGo.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(4, 13, 24, 255);
            camera.orthographic = true;
            camera.cullingMask = 1 << 5;
            camera.targetTexture = target;

            GameObject canvasGo = new GameObject("Step83Wave02QaCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.layer = 5;
            Canvas canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1f;
            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = Mathf.Min(size.x / DesignW, size.y / DesignH);

            RectTransform viewport = new GameObject("Wave02DesignViewport", typeof(RectTransform)).GetComponent<RectTransform>();
            viewport.gameObject.layer = 5;
            viewport.SetParent(canvasGo.transform, false);
            viewport.anchorMin = viewport.anchorMax = new Vector2(0.5f, 0.5f);
            viewport.pivot = new Vector2(0.5f, 0.5f);
            viewport.sizeDelta = new Vector2(DesignW, DesignH);

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, viewport);
            RectTransform page = instance.transform as RectTransform;
            page.anchorMin = Vector2.zero; page.anchorMax = Vector2.one;
            page.offsetMin = Vector2.zero; page.offsetMax = Vector2.zero;

            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = target;
            Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0, false);
            texture.Apply(false, false);
            RenderTexture.active = previous;

            UnityEngine.Object.DestroyImmediate(canvasGo);
            UnityEngine.Object.DestroyImmediate(cameraGo);
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
            return texture;
        }
    }
}
#endif
