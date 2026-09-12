#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    public static class SplashTitleFidelityFix02Validation
    {
        [MenuItem("Legends of Rus/Production UI/Splash R3/Validate FidelityFix02")]
        public static void Run()
        {
            var issues = Validate();
            if (issues.Count > 0)
            {
                Debug.LogError("Splash FidelityFix02 validation failed:\n" + string.Join("\n", issues));
                return;
            }
            Debug.Log("Splash FidelityFix02 serialized layout/import checks and synthetic glyph-alignment check passed. " +
                "This is not a real-device performance test or a reference-font / visual-fidelity approval.");
        }

        public static List<string> Validate()
        {
            var issues = SplashTitleR3Validation.ValidateWiring();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Step20SplashTitleR3PrefabRebuilder.PrefabPath);
            if (prefab == null) return issues;
            if (!File.Exists(Step20SplashTitleR3PrefabRebuilder.LayoutPath))
            { issues.Add("Layout JSON is missing."); return issues; }
            var layout = JsonUtility.FromJson<Step20SplashTitleR3PrefabRebuilder.LayoutDoc>(
                File.ReadAllText(Step20SplashTitleR3PrefabRebuilder.LayoutPath));
            if (layout == null || layout.schemaVersion != 4)
            { issues.Add("Expected layout schema 4 (FidelityFix02)."); return issues; }

            Text[] texts = prefab.GetComponentsInChildren<Text>(true);
            foreach (var spec in layout.texts)
            {
                Text text = Array.Find(texts, t => t.name == spec.name);
                if (text == null) { issues.Add("Missing label " + spec.name); continue; }
                int d = Mathf.Clamp(spec.rasterScale, 1, 3);
                if (text.fontSize != spec.size * d ||
                    Mathf.Abs(text.rectTransform.localScale.x - 1f / d) > .0001f ||
                    Mathf.Abs(text.rectTransform.localScale.y - 1f / d) > .0001f)
                    issues.Add("Incorrect raster density / compensated scale: " + spec.name);
                if (Mathf.Abs(text.rectTransform.sizeDelta.x / d - spec.rect[2]) > .001f ||
                    Mathf.Abs(text.rectTransform.sizeDelta.y / d - spec.rect[3]) > .001f)
                    issues.Add("Changed effective label rectangle: " + spec.name);
                if (text.fontStyle != (FontStyle)spec.fontStyle || text.resizeTextForBestFit || text.raycastTarget)
                    issues.Add("Incorrect font style / best-fit / input configuration: " + spec.name);
                if (text.GetComponent<SplashTitleTextGeometry>() == null)
                    issues.Add("Missing optical alignment: " + spec.name);
                Shadow shadow = text.GetComponent<Shadow>();
                if (spec.shadow != (shadow != null)) issues.Add("Unexpected shadow policy: " + spec.name);
            }

            foreach (var button in prefab.GetComponentsInChildren<Button>(true))
            {
                Color selected = button.colors.selectedColor;
                Color hover = button.colors.highlightedColor;
                if (selected.r > 1 || selected.g > 1 || selected.b > 1 ||
                    hover.r > 1 || hover.g > 1 || hover.b > 1)
                    issues.Add("Overbright tint state remains: " + button.name);
            }
            foreach (var image in prefab.GetComponentsInChildren<Image>(true))
            {
                if (image.sprite == null) continue;
                string path = AssetDatabase.GetAssetPath(image.sprite);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null || !importer.sRGBTexture || importer.isReadable ||
                    importer.mipmapEnabled || importer.textureCompression != TextureImporterCompression.Uncompressed)
                    issues.Add("Unexpected review texture import settings: " + path);
                if (image.color != Color.white)
                    issues.Add("Unexpected double colour grade / non-white sprite tint: " + image.name);
            }

            TestGlyphAlignment(issues);
            return issues;
        }

        static void TestGlyphAlignment(List<string> issues)
        {
            var go = new GameObject("FidelityFix02_GlyphProbe", typeof(RectTransform),
                typeof(CanvasRenderer), typeof(Text), typeof(SplashTitleTextGeometry));
            try
            {
                var rt = go.GetComponent<RectTransform>();
                rt.pivot = new Vector2(0, 1); rt.sizeDelta = new Vector2(200, 80);
                go.GetComponent<Text>().text = "A";
                var effect = go.GetComponent<SplashTitleTextGeometry>();
                effect.SnapToScreenPixels = false;
                using (var vh = new VertexHelper())
                {
                    vh.AddVert(new Vector3(10, -70), Color.white, Vector2.zero);
                    vh.AddVert(new Vector3(10, -30), Color.white, Vector2.zero);
                    vh.AddVert(new Vector3(50, -30), Color.white, Vector2.zero);
                    vh.AddVert(new Vector3(50, -70), Color.white, Vector2.zero);
                    vh.AddTriangle(0, 1, 2); vh.AddTriangle(0, 2, 3);
                    effect.ModifyMesh(vh);
                    UIVertex a = default(UIVertex), b = default(UIVertex);
                    vh.PopulateUIVertex(ref a, 0); vh.PopulateUIVertex(ref b, 2);
                    Vector3 center = (a.position + b.position) * .5f;
                    if (Mathf.Abs(center.x - 100) > .001f || Mathf.Abs(center.y + 40) > .001f)
                        issues.Add("Synthetic glyph optical-center calculation failed.");
                    if (a.color.a != 255 || b.color.a != 255 || vh.currentVertCount != 4)
                        issues.Add("Glyph alignment altered alpha or topology.");
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(go); }
        }
    }
}
#endif
