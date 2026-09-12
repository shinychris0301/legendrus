#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Editor Play-Mode-only v1.20 Step 9.4 capture harness. It renders the eight
    /// refined Wave 3 candidate prefabs at four target resolutions and invokes no
    /// UI, LiveOps, leaderboard, social, mail, reward, provider, or economy action.
    /// </summary>
    public sealed class Step94Wave03RuntimeCaptureHarness : MonoBehaviour
    {
        public const string Version = "v1.20-step09.4-wave03-refined-unity-runtime-capture";
        const float DesignWidth = 941f;
        const float DesignHeight = 1672f;

        static readonly ScreenId[] Pages =
        {
            ScreenId.Events, ScreenId.EventDetail, ScreenId.IndividualRanking, ScreenId.TeamHub,
            ScreenId.TeamRanking, ScreenId.TeamChat, ScreenId.FriendsList, ScreenId.Mail
        };

        static readonly Vector2Int[] Sizes =
        {
            new Vector2Int(941, 1672), new Vector2Int(1080, 2340),
            new Vector2Int(1080, 2400), new Vector2Int(1600, 2560)
        };

        string outputDirectory;
        bool started;

        public void Begin(string absoluteOutputDirectory)
        {
            if (started) return;
            started = true;
            outputDirectory = absoluteOutputDirectory;
            StartCoroutine(CaptureAll());
        }

        IEnumerator CaptureAll()
        {
            Directory.CreateDirectory(outputDirectory);
            string errorPath = Path.Combine(outputDirectory, "UNITY_RUNTIME_CAPTURE_FAILED.txt");
            string manifestPath = Path.Combine(outputDirectory, "UNITY_RUNTIME_CAPTURE_EXECUTED.json");
            if (File.Exists(errorPath)) File.Delete(errorPath);
            if (File.Exists(manifestPath)) File.Delete(manifestPath);

            int captures = 0;
            var failures = new List<string>();
            yield return null;

            foreach (ScreenId id in Pages)
            {
                GameObject prefab = Resources.Load<GameObject>("ProductionUI/Wave03Pages/" + id);
                if (prefab == null)
                {
                    failures.Add("Missing runtime refined Wave 3 candidate prefab: " + id);
                    continue;
                }
                foreach (Vector2Int size in Sizes)
                {
                    Texture2D capture = null;
                    try
                    {
                        capture = RenderCandidate(prefab, size);
                        File.WriteAllBytes(Path.Combine(outputDirectory, id + "_" + size.x + "x" + size.y + ".png"), capture.EncodeToPNG());
                        captures++;
                    }
                    catch (Exception ex)
                    {
                        failures.Add(id + " " + size.x + "x" + size.y + ": " + ex.Message);
                    }
                    finally
                    {
                        if (capture != null) Destroy(capture);
                    }
                    yield return null;
                }
            }

            if (failures.Count == 0 && captures == 32)
            {
                string json = "{\n" +
                    "  \"schemaVersion\": 1,\n" +
                    "  \"milestone\": \"" + Version + "\",\n" +
                    "  \"unityVersion\": \"" + Escape(Application.unityVersion) + "\",\n" +
                    "  \"runtimePlatform\": \"" + Escape(Application.platform.ToString()) + "\",\n" +
                    "  \"isEditorPlayMode\": " + (Application.isEditor ? "true" : "false") + ",\n" +
                    "  \"capturedAtUtc\": \"" + DateTime.UtcNow.ToString("o") + "\",\n" +
                    "  \"candidatePrefabs\": 8,\n" +
                    "  \"aspectTargets\": 4,\n" +
                    "  \"captures\": " + captures + ",\n" +
                    "  \"refinedWave03Candidates\": true,\n" +
                    "  \"uiActionsInvoked\": false,\n" +
                    "  \"liveOpsActionsInvoked\": false,\n" +
                    "  \"leaderboardActionsInvoked\": false,\n" +
                    "  \"socialActionsInvoked\": false,\n" +
                    "  \"mailActionsInvoked\": false,\n" +
                    "  \"economyMutationsInvoked\": false,\n" +
                    "  \"screenCaptureApiUsed\": false,\n" +
                    "  \"activeRuntimeCutoverModified\": false\n" +
                    "}\n";
                File.WriteAllText(manifestPath, json);
                Debug.Log("v1.20 Step 9.4 runtime capture PASS: 32/32 refined Wave 3 candidate captures.");
            }
            else
            {
                File.WriteAllText(errorPath, string.Join(Environment.NewLine, failures.ToArray()));
                Debug.LogError("v1.20 Step 9.4 runtime capture failed: " + captures + "/32. See " + errorPath);
            }
        }

        static Texture2D RenderCandidate(GameObject prefab, Vector2Int size)
        {
            RenderTexture target = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
            target.name = "Step94Wave03_" + size.x + "x" + size.y;
            target.Create();

            GameObject cameraGo = new GameObject("Step94Wave03CaptureCamera", typeof(Camera));
            cameraGo.layer = 5;
            Camera camera = cameraGo.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(4, 13, 24, 255);
            camera.orthographic = true;
            camera.cullingMask = 1 << 5;
            camera.targetTexture = target;

            GameObject canvasGo = new GameObject("Step94Wave03CaptureCanvas", typeof(RectTransform), typeof(Canvas));
            canvasGo.layer = 5;
            Canvas canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1f;

            RectTransform canvasRect = canvasGo.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(size.x, size.y);

            GameObject viewportGo = new GameObject("Wave03DesignViewport", typeof(RectTransform));
            viewportGo.layer = 5;
            viewportGo.transform.SetParent(canvasGo.transform, false);
            RectTransform viewport = viewportGo.GetComponent<RectTransform>();
            viewport.anchorMin = viewport.anchorMax = new Vector2(0.5f, 0.5f);
            viewport.pivot = new Vector2(0.5f, 0.5f);
            viewport.sizeDelta = new Vector2(DesignWidth, DesignHeight);
            float scale = Mathf.Min(size.x / DesignWidth, size.y / DesignHeight);
            viewport.localScale = new Vector3(scale, scale, 1f);

            GameObject instance = Instantiate(prefab, viewport, false);
            instance.name = "RuntimeCandidate_" + prefab.name;
            RectTransform page = instance.transform as RectTransform;
            if (page != null)
            {
                page.anchorMin = Vector2.zero; page.anchorMax = Vector2.one;
                page.offsetMin = Vector2.zero; page.offsetMax = Vector2.zero;
            }

            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = target;
            Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0, false);
            texture.Apply(false, false);
            RenderTexture.active = previous;

            DestroyImmediate(instance); DestroyImmediate(canvasGo); DestroyImmediate(cameraGo);
            target.Release(); DestroyImmediate(target);
            return texture;
        }

        static string Escape(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
#endif
