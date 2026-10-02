from pathlib import Path
root=Path(__file__).resolve().parents[5]
p=root/'Assets/LegendsOfRus/Tests/CampaignPlayMode/ActiveCampaignIntegrationTests.cs'
s=p.read_text()
assert 'static void CaptureFinale' not in s
s=s.replace('using UnityEngine.TestTools;', 'using UnityEngine.TestTools;\nusing UnityEngine.UI;\nusing UnityEngine.EventSystems;')
start=s.index('                        string capture=Environment.GetEnvironmentVariable("LOR_CHAPTER_COMPLETE_CAPTURE_DIR");')
end=s.index('                        complete.Actions[0].onClick.Invoke();', start)
s=s[:start]+'''                        string previousLanguage=DemoFeatureState.LanguageCode;
                        foreach(string language in new[]{"ru","en"}) {
                            DemoFeatureState.SetLanguage(language);complete.RefreshContent();yield return Settle(router);
                            CaptureFinale(router,complete,language,new Vector2Int(576,1280),new Rect(0,48,576,1172));
                            CaptureFinale(router,complete,language,new Vector2Int(360,640),new Rect(0,20,360,600));
                        }
                        DemoFeatureState.SetLanguage(previousLanguage);complete.RefreshContent();
''' +s[end:]
at=s.index('        static IEnumerator Settle(')
method='''        // Render the actual native page with full-chapter totals and the production safe-area scaler.
        static void CaptureFinale(ScreenRouter router,ProductionChapterCompleteView view,string language,Vector2Int size,Rect area)
        {
            string directory=Environment.GetEnvironmentVariable("LOR_CHAPTER_COMPLETE_CAPTURE_DIR");
            Assert.That(directory,Is.Not.Null.And.Not.Empty);Directory.CreateDirectory(directory);
            var canvas=router.GameCanvas;var scaler=canvas.GetComponent<CanvasScaler>();var adaptive=canvas.GetComponent<AdaptiveCanvasScaler>();var safe=router.SafeArea.GetComponent<SafeAreaFitter>();
            var mode=canvas.renderMode;var oldCamera=canvas.worldCamera;float distance=canvas.planeDistance,scale=canvas.scaleFactor;
            bool se=scaler.enabled,ae=adaptive.enabled,fe=safe.enabled;var active=RenderTexture.active;
            var target=new RenderTexture(size.x,size.y,24,RenderTextureFormat.ARGB32);var cameraObject=new GameObject("FullChapterCapture",typeof(Camera));var texture=new Texture2D(size.x,size.y,TextureFormat.RGBA32,false);
            try {
                target.Create();scaler.enabled=false;adaptive.enabled=false;safe.enabled=false;
                var camera=cameraObject.GetComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.transform.position=new Vector3(0,0,-100);camera.nearClipPlane=.1f;camera.farClipPlane=1000;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;camera.targetTexture=target;
                canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;
                adaptive.Apply(area,size);canvas.scaleFactor=AdaptiveCanvasScaler.ResolveScale(new Vector2Int(Mathf.RoundToInt(area.width),Mathf.RoundToInt(area.height)),941,1672);safe.Apply(area,size);
                foreach(var text in view.GetComponentsInChildren<Text>(true))text.SetVerticesDirty();Canvas.ForceUpdateCanvases();
                foreach(var text in view.GetComponentsInChildren<Text>(true))text.SetVerticesDirty();Canvas.ForceUpdateCanvases();
                Assert.That(view.Summary.text,Does.Contain("500/500"));
                Assert.That(view.RewardCounts[0].text,Is.EqualTo("91 970"));
                foreach(var text in view.GetComponentsInChildren<Text>())if(text.enabled&&!string.IsNullOrEmpty(text.text)) {
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.name+" width");
                    Assert.That(text.preferredHeight,Is.LessThanOrEqualTo(text.rectTransform.rect.height+2),text.name+" height");
                }
                foreach(var button in view.Actions) {
                    var corners=new Vector3[4];((RectTransform)button.transform).GetWorldCorners(corners);
                    Vector2 min=RectTransformUtility.WorldToScreenPoint(camera,corners[0]),max=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
                    Assert.That(area.Contains(min)&&area.Contains(max),Is.True,button.name+" safe area");
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=(min+max)*.5f},hits);
                    Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));
                }
                camera.Render();RenderTexture.active=target;texture.ReadPixels(new Rect(0,0,size.x,size.y),0,0);texture.Apply();
                File.WriteAllBytes(Path.Combine(directory,$"Chapter1_500_{language}_{size.x}.png"),texture.EncodeToPNG());
            } finally {
                RenderTexture.active=active;canvas.renderMode=mode;canvas.worldCamera=oldCamera;canvas.planeDistance=distance;canvas.scaleFactor=scale;
                scaler.enabled=se;adaptive.enabled=ae;safe.enabled=fe;adaptive.ApplyNow();safe.ApplyNow();
                UnityEngine.Object.Destroy(texture);UnityEngine.Object.Destroy(cameraObject);target.Release();UnityEngine.Object.Destroy(target);
            }
        }
'''
s=s[:at]+method+s[at:]
p.write_text(s)
