using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class CoreLoopVfxBoosterFeedbackPlayModeTests
    {
        const string PersistedSaveKey = "LegendsOfRus_Save_v1";
        static readonly string[] PersistedSaveKeys =
        {
            PersistedSaveKey,
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        GameObject stateObject;
        GameObject eventSystemObject;
        GameObject canvasObject;
        GameObject boardObject;
        GameState state;
        Match3Board board;
        CoreLoopVfxPresenter presenter;
        bool ownsStateObject;
        bool ownsEventSystemObject;
        bool[] hadPersistedSaves;
        string[] persistedSaveJson;
        string runtimeSaveJson;
        float originalTimeScale;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            originalTimeScale = Time.timeScale;
            Time.timeScale = 1f;

            hadPersistedSaves = new bool[PersistedSaveKeys.Length];
            persistedSaveJson = new string[PersistedSaveKeys.Length];
            for (int i = 0; i < PersistedSaveKeys.Length; i++)
            {
                string key = PersistedSaveKeys[i];
                hadPersistedSaves[i] = PlayerPrefs.HasKey(key);
                persistedSaveJson[i] = hadPersistedSaves[i]
                    ? PlayerPrefs.GetString(key)
                    : string.Empty;
                PlayerPrefs.DeleteKey(key);
            }
            PlayerPrefs.Save();

            state = GameState.Instance;
            if (state == null)
            {
                stateObject = new GameObject("Step14VfxTestGameState");
                state = stateObject.AddComponent<GameState>();
                ownsStateObject = true;
            }

            runtimeSaveJson = JsonUtility.ToJson(state.Save);
            state.Save = new PlayerSaveData
            {
                lastEnergyUtcTicks = DateTime.UtcNow.Ticks
            };

            if (EventSystem.current == null)
            {
                eventSystemObject = new GameObject(
                    "Step14VfxTestEventSystem",
                    typeof(EventSystem)
                );
                ownsEventSystemObject = true;
            }

            canvasObject = new GameObject(
                "Step14VfxTestCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            boardObject = new GameObject(
                "Step14VfxTestBoard",
                typeof(RectTransform)
            );
            boardObject.transform.SetParent(canvasObject.transform, false);
            RectTransform boardRect = boardObject.GetComponent<RectTransform>();
            boardRect.anchorMin = new Vector2(0.5f, 0.5f);
            boardRect.anchorMax = new Vector2(0.5f, 0.5f);
            boardRect.pivot = new Vector2(0.5f, 0.5f);
            boardRect.sizeDelta = new Vector2(786f, 942f);

            board = boardObject.AddComponent<Match3Board>();
            // The QA snapshots deliberately use an 8x9 stress board, independently of
            // the production 8x7 catalog. Bind the fixture to its actual dimensions.
            board.Initialize(new LevelData
            {
                columns = BoardQAPresetFactory.Width,
                rows = BoardQAPresetFactory.Height,
                tileTypes = BoardQAPresetFactory.TileTypes
            }, null, null);
            presenter = board.VfxPresenter;
            Canvas.ForceUpdateCanvases();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Time.timeScale = originalTimeScale;
            if (boardObject != null) UnityEngine.Object.Destroy(boardObject);
            if (canvasObject != null) UnityEngine.Object.Destroy(canvasObject);
            if (ownsEventSystemObject && eventSystemObject != null)
                UnityEngine.Object.Destroy(eventSystemObject);

            RestoreRuntimeSave();
            if (ownsStateObject && stateObject != null)
                UnityEngine.Object.Destroy(stateObject);

            yield return null;
            RestoreRuntimeSave();

            for (int i = 0; i < PersistedSaveKeys.Length; i++)
            {
                string key = PersistedSaveKeys[i];
                PlayerPrefs.DeleteKey(key);
                if (hadPersistedSaves[i])
                    PlayerPrefs.SetString(key, persistedSaveJson[i]);
            }
            PlayerPrefs.Save();
        }

        [UnityTest]
        public IEnumerator NativeFacetsRingsAndTrails_RenderThenReturnToPool()
        {
            var motions = new[] {
                Motion(1, GameplayMotionKind.MatchWaveStarted),
                Motion(2, GameplayMotionKind.SpecialActivationStarted, special: SpecialGemKind.Burst),
                Motion(3, GameplayMotionKind.SpecialActivationStarted, special: SpecialGemKind.Row),
                Motion(4, GameplayMotionKind.SpecialActivationStarted, special: SpecialGemKind.Column),
                Motion(5, GameplayMotionKind.BoosterActivated, booster: GameplayBoosterKind.Firebird)
            };
            string directory = Environment.GetEnvironmentVariable("LOR_VFX_CAPTURE_DIR");
            foreach (var motion in motions)
            {
                presenter.ResetEffects();
                // Controlled cleared-cell view isolates the native effect geometry.
                // Command correctness is covered separately by actual booster executions.
                foreach (var cell in boardObject.GetComponentsInChildren<Match3CellInput>())
                {
                    bool cleared = motion.SpecialKind == SpecialGemKind.Row ? cell.Y == 4 :
                        motion.SpecialKind == SpecialGemKind.Column ? cell.X == 3 :
                        Mathf.Abs(cell.X - 3) <= 1 && Mathf.Abs(cell.Y - 4) <= 1;
                    foreach (var group in cell.GetComponentsInChildren<CanvasGroup>())
                        if (group.name == "GemVisual") group.alpha = cleared ? 0 : 1;
                }
                presenter.Present(motion);
                presenter.TickForTesting(.08f);
                Canvas.ForceUpdateCanvases();
                bool visibleGeometry = false;
                foreach (var effect in presenter.VfxRoot.GetComponentsInChildren<CoreLoopEffectGraphic>())
                {
                    Assert.That(effect.raycastTarget, Is.False);
                    Assert.That(effect.Cue == CoreLoopVfxPresenter.ResolveCue(motion) || effect.Cue == CoreLoopVfxCue.MajorCombinationPulse, Is.True);
                    var mesh = effect.canvasRenderer.GetMesh();
                    visibleGeometry |= mesh != null && mesh.vertexCount > 4;
                }
                Assert.That(visibleGeometry, Is.True, motion.Kind.ToString());
                if (!string.IsNullOrEmpty(directory)) CaptureVfx(directory, motion.Sequence.ToString());
                presenter.TickForTesting(2f);
                Assert.That(presenter.ActiveEffectCount, Is.Zero);
                Assert.That(presenter.AvailableEffectCount, Is.EqualTo(presenter.PoolCapacity));
            }
            yield return null;
        }

        void CaptureVfx(string directory, string name)
        {
            System.IO.Directory.CreateDirectory(directory);
            var canvas = canvasObject.GetComponent<Canvas>();
            var cameraObject = new GameObject("ControlledVfxCapture", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>();
            var target = new RenderTexture(941, 1200, 24);
            var image = new Texture2D(941, 1200, TextureFormat.RGB24, false);
            var previous = RenderTexture.active;
            try
            {
                target.Create(); camera.enabled = false;
                camera.transform.position = new Vector3(0, 0, -100);
                camera.orthographic = true; camera.nearClipPlane = .1f; camera.farClipPlane = 1000;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color32(5, 13, 23, 255);
                camera.targetTexture = target;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera; canvas.planeDistance = 100;
                Canvas.ForceUpdateCanvases(); camera.Render(); RenderTexture.active = target;
                image.ReadPixels(new Rect(0, 0, 941, 1200), 0, 0); image.Apply();
                System.IO.File.WriteAllBytes(System.IO.Path.Combine(directory, "Vfx_" + name + ".png"), image.EncodeToPNG());
                var details = new System.Text.StringBuilder("Controlled native 8x9 QA stress board with hit-area gems hidden for effect visibility; production remains 8x7.\n");
                foreach (var effect in presenter.VfxRoot.GetComponentsInChildren<CoreLoopEffectGraphic>())
                    details.AppendLine(effect.Cue + " color=" + effect.color + " progress=" + effect.Progress + " size=" + effect.rectTransform.rect.size);
                System.IO.File.WriteAllText(System.IO.Path.Combine(directory, "Vfx_" + name + ".txt"), details.ToString());
            }
            finally
            {
                RenderTexture.active = previous; canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.worldCamera = null;
                UnityEngine.Object.Destroy(image); UnityEngine.Object.Destroy(cameraObject);
                target.Release(); UnityEngine.Object.Destroy(target);
            }
        }

        [Test]
        public void VfxProfile_HasBoundedPoolBurstAndStyleContracts()
        {
            Assert.That(CoreLoopVfxProfile.DefaultPoolCapacity, Is.GreaterThan(0));
            Assert.That(
                CoreLoopVfxProfile.DefaultPoolCapacity,
                Is.LessThanOrEqualTo(CoreLoopVfxProfile.MaximumPoolCapacity)
            );
            Assert.That(
                CoreLoopVfxProfile.MaximumBurstEffectsPerCue,
                Is.InRange(1, CoreLoopVfxProfile.DefaultPoolCapacity)
            );
            Assert.That(CoreLoopVfxProfile.MaximumCascadePulses, Is.InRange(1, 4));

            foreach (CoreLoopVfxCue cue in Enum.GetValues(typeof(CoreLoopVfxCue)))
            {
                if (cue == CoreLoopVfxCue.None) continue;
                CoreLoopVfxStyle style = CoreLoopVfxProfile.Resolve(cue);
                Assert.That(style.DurationSeconds, Is.InRange(0.01f, 0.75f), cue.ToString());
                Assert.That(style.StartScale, Is.InRange(0f, 2f), cue.ToString());
                Assert.That(style.EndScale, Is.InRange(0f, 2f), cue.ToString());
                Assert.That(style.StartColor.a, Is.InRange(0f, 1f), cue.ToString());
                Assert.That(style.EndColor.a, Is.InRange(0f, 1f), cue.ToString());
            }
        }

        [Test]
        public void CueResolver_MapsCoreTimelineStages()
        {
            AssertCue(GameplayMotionKind.SwapStarted, CoreLoopVfxCue.SwapTrail);
            AssertCue(GameplayMotionKind.SwapCommitted, CoreLoopVfxCue.SwapCommitGlint);
            AssertCue(GameplayMotionKind.SwapRejected, CoreLoopVfxCue.SwapRejectPulse);
            AssertCue(GameplayMotionKind.MatchWaveStarted, CoreLoopVfxCue.MatchBurst);
            AssertCue(GameplayMotionKind.MatchWaveCompleted, CoreLoopVfxCue.MatchResolveGlint);
            AssertCue(GameplayMotionKind.CascadeStarted, CoreLoopVfxCue.CascadePulse);
            AssertCue(GameplayMotionKind.SpecialCreationStarted, CoreLoopVfxCue.SpecialCreationHalo);
            AssertCue(GameplayMotionKind.FallStarted, CoreLoopVfxCue.FallTrail);
            AssertCue(GameplayMotionKind.FallCompleted, CoreLoopVfxCue.FallSettleSpark);
            AssertCue(GameplayMotionKind.ReshuffleStarted, CoreLoopVfxCue.ReshuffleWash);
            AssertCue(GameplayMotionKind.ReshuffleCompleted, CoreLoopVfxCue.ReshuffleSettle);
            AssertCue(GameplayMotionKind.CommandRecovered, CoreLoopVfxCue.RecoveryWash);
        }

        [Test]
        public void CueResolver_MapsAllSpecialKindsDistinctly()
        {
            Assert.That(
                CoreLoopVfxPresenter.ResolveCue(
                    Motion(1, GameplayMotionKind.SpecialActivationStarted, special: SpecialGemKind.Row)
                ),
                Is.EqualTo(CoreLoopVfxCue.SpecialRowSweep)
            );
            Assert.That(
                CoreLoopVfxPresenter.ResolveCue(
                    Motion(2, GameplayMotionKind.SpecialActivationStarted, special: SpecialGemKind.Column)
                ),
                Is.EqualTo(CoreLoopVfxCue.SpecialColumnSweep)
            );
            Assert.That(
                CoreLoopVfxPresenter.ResolveCue(
                    Motion(3, GameplayMotionKind.SpecialActivationStarted, special: SpecialGemKind.Burst)
                ),
                Is.EqualTo(CoreLoopVfxCue.SpecialBurstPulse)
            );
            Assert.That(
                CoreLoopVfxPresenter.ResolveCue(
                    Motion(4, GameplayMotionKind.SpecialActivationStarted, special: SpecialGemKind.Color)
                ),
                Is.EqualTo(CoreLoopVfxCue.SpecialColorWash)
            );
        }

        [Test]
        public void CueResolver_MapsAllFiveBoosterVariantsAndCompletions()
        {
            GameplayBoosterKind[] kinds =
            {
                GameplayBoosterKind.BogatyrMaceRow,
                GameplayBoosterKind.BogatyrMaceColumn,
                GameplayBoosterKind.Firebird,
                GameplayBoosterKind.LivingLight,
                GameplayBoosterKind.SeaPearl
            };
            CoreLoopVfxCue[] activations =
            {
                CoreLoopVfxCue.BoosterMaceRow,
                CoreLoopVfxCue.BoosterMaceColumn,
                CoreLoopVfxCue.BoosterFirebird,
                CoreLoopVfxCue.BoosterLivingLight,
                CoreLoopVfxCue.BoosterSeaPearl
            };
            CoreLoopVfxCue[] completions =
            {
                CoreLoopVfxCue.BoosterMaceCompleted,
                CoreLoopVfxCue.BoosterMaceCompleted,
                CoreLoopVfxCue.BoosterFirebirdCompleted,
                CoreLoopVfxCue.BoosterLivingLightCompleted,
                CoreLoopVfxCue.BoosterSeaPearlCompleted
            };

            for (int i = 0; i < kinds.Length; i++)
            {
                Assert.That(
                    CoreLoopVfxPresenter.ResolveCue(
                        Motion(i + 1, GameplayMotionKind.BoosterActivated, booster: kinds[i])
                    ),
                    Is.EqualTo(activations[i]),
                    kinds[i].ToString()
                );
                Assert.That(
                    CoreLoopVfxPresenter.ResolveCue(
                        Motion(i + 11, GameplayMotionKind.BoosterCompleted, booster: kinds[i])
                    ),
                    Is.EqualTo(completions[i]),
                    kinds[i].ToString()
                );
            }
        }

        [Test]
        public void PresentationTrace_IsDeterministicForIdenticalMotionSequence()
        {
            GameplayMotionEvent[] input =
            {
                Motion(1, GameplayMotionKind.SwapStarted, affected: 2),
                Motion(2, GameplayMotionKind.MatchWaveStarted, combo: 1, affected: 7),
                Motion(3, GameplayMotionKind.CascadeStarted, combo: 3, affected: 8),
                Motion(
                    4,
                    GameplayMotionKind.SpecialActivationStarted,
                    combo: 3,
                    affected: 2,
                    special: SpecialGemKind.Burst,
                    combination: SpecialCombinationKind.BurstWithBurst
                ),
                Motion(
                    5,
                    GameplayMotionKind.BoosterCompleted,
                    affected: 9,
                    booster: GameplayBoosterKind.Firebird
                )
            };

            string first = CaptureTrace(input);
            string second = CaptureTrace(input);

            Assert.That(second, Is.EqualTo(first));
        }

        [Test]
        public void FixedPool_IsPrewarmedAndBoundedUnderBurstLoad()
        {
            presenter.ResetEffects();
            presenter.ResetDiagnostics();

            Assert.That(presenter.PoolCapacity, Is.EqualTo(CoreLoopVfxProfile.DefaultPoolCapacity));
            Assert.That(presenter.CreatedEffectCount, Is.EqualTo(presenter.PoolCapacity));
            Assert.That(presenter.AvailableEffectCount, Is.EqualTo(presenter.PoolCapacity));
            Assert.That(presenter.VfxRoot.childCount, Is.EqualTo(presenter.PoolCapacity));

            for (int i = 1; i <= 11; i++)
                presenter.Present(
                    Motion(i, GameplayMotionKind.MatchWaveStarted, combo: i, affected: 72)
                );

            Assert.That(presenter.SpawnRequestCount, Is.EqualTo(33));
            Assert.That(presenter.EmittedEffectCount, Is.EqualTo(presenter.PoolCapacity));
            Assert.That(presenter.ActiveEffectCount, Is.EqualTo(presenter.PoolCapacity));
            Assert.That(presenter.AvailableEffectCount, Is.Zero);
            Assert.That(presenter.PeakActiveEffectCount, Is.EqualTo(presenter.PoolCapacity));
            Assert.That(presenter.DroppedEffectCount, Is.EqualTo(1));
            Assert.That(presenter.CreatedEffectCount, Is.EqualTo(presenter.PoolCapacity));

            CoreLoopVfxTrace trace = presenter.LastEffectTrace;
            Assert.That(trace.RequestedEffects, Is.EqualTo(3));
            Assert.That(trace.EmittedEffects, Is.EqualTo(2));
            Assert.That(trace.DroppedEffects, Is.EqualTo(1));
            Assert.That(trace.ActiveEffectsAfter, Is.EqualTo(presenter.PoolCapacity));
        }

        [Test]
        public void PoolReuse_DoesNotCreateObjectsAfterRelease()
        {
            presenter.ResetEffects();
            presenter.ResetDiagnostics();
            int created = presenter.CreatedEffectCount;
            int childCount = presenter.VfxRoot.childCount;

            presenter.Present(
                Motion(1, GameplayMotionKind.MatchWaveStarted, affected: 10)
            );
            Assert.That(presenter.ActiveEffectCount, Is.GreaterThan(0));

            presenter.TickForTesting(10f);
            Assert.That(presenter.ActiveEffectCount, Is.Zero);
            Assert.That(presenter.AvailableEffectCount, Is.EqualTo(presenter.PoolCapacity));

            presenter.Present(
                Motion(2, GameplayMotionKind.BoosterActivated, affected: 9, booster: GameplayBoosterKind.Firebird)
            );
            Assert.That(presenter.ActiveEffectCount, Is.GreaterThan(0));
            Assert.That(presenter.CreatedEffectCount, Is.EqualTo(created));
            Assert.That(presenter.VfxRoot.childCount, Is.EqualTo(childCount));
        }

        [Test]
        public void PooledGraphics_AreNonRaycastClippedAndBoardBounded()
        {
            presenter.ResetEffects();
            presenter.Present(
                Motion(
                    1,
                    GameplayMotionKind.BoosterActivated,
                    affected: 72,
                    booster: GameplayBoosterKind.LivingLight
                )
            );

            Assert.That(presenter.IsRaycastSafe, Is.True);
            Assert.That(presenter.VfxRoot.GetComponent<RectMask2D>(), Is.Not.Null);
            CanvasGroup group = presenter.VfxRoot.GetComponent<CanvasGroup>();
            Assert.That(group, Is.Not.Null);
            Assert.That(group.blocksRaycasts, Is.False);
            Assert.That(group.interactable, Is.False);
            Assert.That(presenter.VfxRoot.parent, Is.EqualTo(board.transform));
            Assert.That(
                presenter.VfxRoot.GetSiblingIndex(),
                Is.EqualTo(board.transform.Find("SharedGrid").GetSiblingIndex() - 1)
            );

            Image[] images = presenter.VfxRoot.GetComponentsInChildren<Image>(true);
            Assert.That(images.Length, Is.EqualTo(presenter.PoolCapacity));
            for (int i = 0; i < images.Length; i++)
            {
                Assert.That(images[i].raycastTarget, Is.False, images[i].name);
                RectTransform rect = images[i].rectTransform;
                Assert.That(rect.anchorMin.x, Is.InRange(0f, 1f), images[i].name);
                Assert.That(rect.anchorMin.y, Is.InRange(0f, 1f), images[i].name);
                Assert.That(rect.anchorMax.x, Is.InRange(0f, 1f), images[i].name);
                Assert.That(rect.anchorMax.y, Is.InRange(0f, 1f), images[i].name);
                Assert.That(rect.anchorMax.x, Is.GreaterThanOrEqualTo(rect.anchorMin.x));
                Assert.That(rect.anchorMax.y, Is.GreaterThanOrEqualTo(rect.anchorMin.y));
            }
        }

        [UnityTest]
        public IEnumerator ActiveVfx_DoesNotInterceptBoardCellRaycasts()
        {
            presenter.Present(
                Motion(
                    1,
                    GameplayMotionKind.BoosterActivated,
                    affected: 72,
                    booster: GameplayBoosterKind.LivingLight
                )
            );
            Canvas.ForceUpdateCanvases();
            yield return null;

            RectTransform cell = board.GetCellRectForTesting(3, 4);
            Vector2 point = RectTransformUtility.WorldToScreenPoint(
                null,
                cell.TransformPoint(cell.rect.center)
            );
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = point
            };
            var results = new List<RaycastResult>();
            canvasObject.GetComponent<GraphicRaycaster>().Raycast(pointer, results);

            bool hitCell = false;
            for (int i = 0; i < results.Count; i++)
            {
                Transform hit = results[i].gameObject.transform;
                Assert.That(
                    hit.IsChildOf(presenter.VfxRoot),
                    Is.False,
                    "A pooled VFX object intercepted the board raycast."
                );
                if (results[i].gameObject.GetComponent<Match3CellInput>() != null)
                    hitCell = true;
            }
            Assert.That(hitCell, Is.True);
        }

        [UnityTest]
        public IEnumerator ResetPresetAndDisable_CleanEffectsAndRestoreSubscription()
        {
            presenter.Present(
                Motion(1, GameplayMotionKind.SpecialCreationStarted, affected: 1)
            );
            Assert.That(presenter.ActiveEffectCount, Is.GreaterThan(0));

            presenter.ResetEffects();
            presenter.ResetEffects();
            Assert.That(presenter.ActiveEffectCount, Is.Zero);
            Assert.That(presenter.AvailableEffectCount, Is.EqualTo(presenter.PoolCapacity));

            presenter.Present(
                Motion(2, GameplayMotionKind.BoosterActivated, affected: 9, booster: GameplayBoosterKind.Firebird)
            );
            Assert.That(presenter.ActiveEffectCount, Is.GreaterThan(0));
            Assert.That(board.LoadQAPreset("booster.firebird"), Is.True);
            Assert.That(presenter.ActiveEffectCount, Is.Zero);

            presenter.Present(
                Motion(3, GameplayMotionKind.ReshuffleStarted, affected: 1)
            );
            presenter.enabled = false;
            Assert.That(presenter.ActiveEffectCount, Is.Zero);
            presenter.enabled = true;

            Assert.That(board.LoadQAPreset("booster.invalid-empty"), Is.True);
            presenter.ResetDiagnostics();
            Assert.That(board.SwipeCell(0, 0, Vector2Int.right), Is.True);
            yield return WaitUntilInteractive();

            Assert.That(presenter.CueCount, Is.EqualTo(1));
            Assert.That(presenter.LastCue, Is.EqualTo(CoreLoopVfxCue.SwapRejectPulse));
        }

        [UnityTest]
        public IEnumerator AllFiveBoosterVariants_ActivateCompleteAndSpendExactlyOnce()
        {
            string[] presets =
            {
                "booster.mace-row",
                "booster.mace-column",
                "booster.firebird",
                "booster.living-light",
                "booster.firebird"
            };
            BoosterType[] inventoryKinds =
            {
                BoosterType.BogatyrMace,
                BoosterType.BogatyrMace,
                BoosterType.Firebird,
                BoosterType.LivingLight,
                BoosterType.SeaPearl
            };
            GameplayBoosterKind[] presentationKinds =
            {
                GameplayBoosterKind.BogatyrMaceRow,
                GameplayBoosterKind.BogatyrMaceColumn,
                GameplayBoosterKind.Firebird,
                GameplayBoosterKind.LivingLight,
                GameplayBoosterKind.SeaPearl
            };

            for (int i = 0; i < presentationKinds.Length; i++)
            {
                Assert.That(board.LoadQAPreset(presets[i]), Is.True, presets[i]);
                BoardQAPresetFactory.TryGet(presets[i], out BoardQAPreset preset);
                int beforeInventory = state.BoosterCount(inventoryKinds[i]);
                var motions = new List<GameplayMotionEvent>();
                Action<GameplayMotionEvent> handler = motions.Add;
                board.MotionEventPublished += handler;
                presenter.ResetDiagnostics();

                board.UseBooster(inventoryKinds[i]);
                if (presentationKinds[i] == GameplayBoosterKind.SeaPearl)
                {
                    Assert.That(board.HasActiveCommand, Is.True);
                }
                else if (presentationKinds[i] == GameplayBoosterKind.BogatyrMaceColumn)
                {
                    Assert.That(
                        board.SwipeCell(preset.Primary.X, preset.Primary.Y, Vector2Int.up),
                        Is.True
                    );
                }
                else
                {
                    Assert.That(board.TapCell(preset.Primary.X, preset.Primary.Y), Is.True);
                }

                yield return WaitUntilInteractive();
                board.MotionEventPublished -= handler;

                int activated = IndexOf(motions, GameplayMotionKind.BoosterActivated);
                int completed = IndexOf(motions, GameplayMotionKind.BoosterCompleted);
                int rejected = IndexOf(motions, GameplayMotionKind.BoosterRejected);
                Assert.That(activated, Is.GreaterThanOrEqualTo(0), presentationKinds[i].ToString());
                Assert.That(completed, Is.GreaterThan(activated), presentationKinds[i].ToString());
                Assert.That(rejected, Is.EqualTo(-1), presentationKinds[i].ToString());
                Assert.That(motions[activated].BoosterKind, Is.EqualTo(presentationKinds[i]));
                Assert.That(motions[completed].BoosterKind, Is.EqualTo(presentationKinds[i]));
                Assert.That(motions[activated].AffectedCount, Is.GreaterThan(0));
                Assert.That(
                    state.BoosterCount(inventoryKinds[i]),
                    Is.EqualTo(beforeInventory - 1),
                    presentationKinds[i].ToString()
                );
                Assert.That(presenter.LastBoosterKind, Is.EqualTo(presentationKinds[i]));
                Assert.That(
                    presenter.LastCue,
                    Is.EqualTo(CoreLoopVfxPresenter.ResolveCue(motions[completed]))
                );

                if (presentationKinds[i] == GameplayBoosterKind.SeaPearl)
                {
                    Assert.That(motions[activated].Primary, Is.EqualTo(new BoardPosition(-1, -1)));
                    Assert.That(motions[activated].PlanHash, Is.Empty);
                }
                else
                {
                    Assert.That(motions[activated].Primary, Is.EqualTo(preset.Primary));
                    Assert.That(motions[activated].PlanHash, Is.Not.Empty);
                    Assert.That(board.LastWaveVerificationPassed, Is.True);
                }
            }
        }

        [UnityTest]
        public IEnumerator InvalidBoosterTarget_RejectsWithoutSpendOrActivation()
        {
            Assert.That(board.LoadQAPreset("booster.invalid-empty"), Is.True);
            int beforeInventory = state.BoosterCount(BoosterType.Firebird);
            string beforeSave = JsonUtility.ToJson(state.Save);
            var motions = new List<GameplayMotionEvent>();
            board.MotionEventPublished += motions.Add;

            board.UseBooster(BoosterType.Firebird);
            Assert.That(board.TapCell(0, 0), Is.True);
            yield return null;

            Assert.That(IndexOf(motions, GameplayMotionKind.BoosterRejected), Is.GreaterThanOrEqualTo(0));
            Assert.That(IndexOf(motions, GameplayMotionKind.BoosterActivated), Is.EqualTo(-1));
            Assert.That(IndexOf(motions, GameplayMotionKind.BoosterCompleted), Is.EqualTo(-1));
            Assert.That(state.BoosterCount(BoosterType.Firebird), Is.EqualTo(beforeInventory));
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(beforeSave));
            Assert.That(board.Phase, Is.EqualTo(GameplayPhase.BoosterTargeting));
            Assert.That(board.ArmedBooster, Is.EqualTo(BoosterType.Firebird));
            Assert.That(board.LastRejectedCommandReason, Is.Not.Empty);
            Assert.That(presenter.LastCue, Is.EqualTo(CoreLoopVfxCue.BoosterRejected));
            Assert.That(presenter.LastBoosterKind, Is.EqualTo(GameplayBoosterKind.Firebird));

            board.CancelArmedBooster(false);
            Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Idle));
        }

        [UnityTest]
        public IEnumerator SeaPearl_GrantsFiveMovesWithoutBoardOrUnrelatedSaveMutation()
        {
            Assert.That(board.LoadQAPreset("booster.firebird"), Is.True);
            string beforeHash = board.BoardStateHash;
            int beforeMoves = board.Moves;
            int beforeInventory = state.BoosterCount(BoosterType.SeaPearl);
            string beforeSave = JsonUtility.ToJson(state.Save);
            PlayerSaveData expectedSave = JsonUtility.FromJson<PlayerSaveData>(beforeSave);
            expectedSave.boosterSeaPearl--;
            var motions = new List<GameplayMotionEvent>();
            board.MotionEventPublished += motions.Add;

            board.UseBooster(BoosterType.SeaPearl);
            yield return WaitUntilInteractive();

            Assert.That(board.BoardStateHash, Is.EqualTo(beforeHash));
            Assert.That(board.Moves, Is.EqualTo(beforeMoves + 5));
            Assert.That(state.BoosterCount(BoosterType.SeaPearl), Is.EqualTo(beforeInventory - 1));
            Assert.That(
                JsonUtility.ToJson(state.Save),
                Is.EqualTo(JsonUtility.ToJson(expectedSave))
            );
            int activated = IndexOf(motions, GameplayMotionKind.BoosterActivated);
            int completed = IndexOf(motions, GameplayMotionKind.BoosterCompleted);
            Assert.That(activated, Is.GreaterThanOrEqualTo(0));
            Assert.That(completed, Is.GreaterThan(activated));
            Assert.That(motions[activated].BoosterKind, Is.EqualTo(GameplayBoosterKind.SeaPearl));
            Assert.That(motions[activated].AffectedCount, Is.EqualTo(5));
            Assert.That(motions[activated].PlanHash, Is.Empty);
        }

        [UnityTest]
        public IEnumerator Recovery_ClearsBoosterVfxSuppressesCompletionAndRefundsOnce()
        {
            const string presetId = "booster.firebird";
            Assert.That(board.LoadQAPreset(presetId), Is.True);
            BoardQAPresetFactory.TryGet(presetId, out BoardQAPreset preset);
            string beforeHash = board.BoardStateHash;
            string beforeSave = JsonUtility.ToJson(state.Save);
            int beforeInventory = state.BoosterCount(BoosterType.Firebird);
            var motions = new List<GameplayMotionEvent>();
            board.MotionEventPublished += motions.Add;

            board.UseBooster(BoosterType.Firebird);
            Assert.That(board.TapCell(preset.Primary.X, preset.Primary.Y), Is.True);
            Assert.That(board.HasActiveCommand, Is.True);
            Assert.That(state.BoosterCount(BoosterType.Firebird), Is.EqualTo(beforeInventory - 1));
            Assert.That(
                presenter.HasActiveCue(CoreLoopVfxCue.BoosterFirebird),
                Is.True
            );
            Match3CellInput heldInput = board.GetCellInputForTesting(2, 2);
            heldInput.OnPointerDown(CreatePointer(91, 2, 2));
            Assert.That(board.OwnsPointer(91), Is.True);

            LogAssert.Expect(
                LogType.Error,
                new Regex("Gameplay command recovered: booster:FirebirdArea")
            );
            board.ForceCommandTimeoutForTesting();
            yield return null;
            yield return null;

            Assert.That(board.HasActiveCommand, Is.False);
            Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Idle));
            Assert.That(board.BoardStateHash, Is.EqualTo(beforeHash));
            Assert.That(state.BoosterCount(BoosterType.Firebird), Is.EqualTo(beforeInventory));
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(beforeSave));
            Assert.That(IndexOf(motions, GameplayMotionKind.BoosterActivated), Is.GreaterThanOrEqualTo(0));
            Assert.That(IndexOf(motions, GameplayMotionKind.BoosterCompleted), Is.EqualTo(-1));
            Assert.That(IndexOf(motions, GameplayMotionKind.CommandRecovered), Is.GreaterThanOrEqualTo(0));
            Assert.That(
                presenter.HasActiveCue(CoreLoopVfxCue.BoosterFirebird),
                Is.False
            );
            Assert.That(presenter.LastCue, Is.EqualTo(CoreLoopVfxCue.RecoveryWash));

            PointerEventData replacement = CreatePointer(92, 2, 2);
            heldInput.OnPointerDown(replacement);
            Assert.That(board.OwnsPointer(92), Is.True);
            heldInput.OnCancel(new BaseEventData(EventSystem.current));
            Assert.That(board.OwnsPointer(92), Is.False);

            presenter.TickForTesting(10f);
            Assert.That(presenter.ActiveEffectCount, Is.Zero);
            Assert.That(presenter.AvailableEffectCount, Is.EqualTo(presenter.PoolCapacity));
        }

        [Test]
        public void PresentationOnlyVfx_DoesNotMutateSaveOrFireStateChanged()
        {
            string before = JsonUtility.ToJson(state.Save);
            int changed = 0;
            Action handler = () => changed++;
            state.Changed += handler;
            try
            {
                presenter.Present(
                    Motion(1, GameplayMotionKind.MatchWaveStarted, affected: 10)
                );
                presenter.Present(
                    Motion(
                        2,
                        GameplayMotionKind.BoosterActivated,
                        affected: 9,
                        booster: GameplayBoosterKind.Firebird
                    )
                );
                presenter.TickForTesting(10f);
            }
            finally
            {
                state.Changed -= handler;
            }

            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before));
            Assert.That(changed, Is.Zero);
        }

        string CaptureTrace(IReadOnlyList<GameplayMotionEvent> motions)
        {
            presenter.ResetEffects();
            presenter.ResetDiagnostics();
            var trace = new List<string>();
            Action<CoreLoopVfxTrace> handler = item => trace.Add(TraceKey(item));
            presenter.EffectTracePublished += handler;
            try
            {
                for (int i = 0; i < motions.Count; i++)
                    presenter.Present(motions[i]);
            }
            finally
            {
                presenter.EffectTracePublished -= handler;
            }
            return string.Join("|", trace);
        }

        static string TraceKey(CoreLoopVfxTrace trace)
        {
            return
                $"{trace.MotionSequence}:{trace.Cue}:{trace.BoosterKind}:" +
                $"{trace.RequestedEffects}:{trace.EmittedEffects}:" +
                $"{trace.DroppedEffects}:{trace.ActiveEffectsAfter}";
        }

        void RestoreRuntimeSave()
        {
            if (
                state == null ||
                GameState.Instance != state ||
                string.IsNullOrEmpty(runtimeSaveJson)
            )
            {
                return;
            }

            PlayerSaveData restored = JsonUtility.FromJson<PlayerSaveData>(runtimeSaveJson);
            if (restored != null)
                state.Save = restored;
        }

        IEnumerator WaitUntilInteractive(float timeout = 10f)
        {
            float end = Time.realtimeSinceStartup + timeout;
            while (
                Time.realtimeSinceStartup < end &&
                (board.HasActiveCommand || !board.CanAcceptPointerInput)
            )
            {
                yield return null;
            }

            Assert.That(board.HasActiveCommand, Is.False, board.ActiveCommandLabel);
            Assert.That(board.CanAcceptPointerInput, Is.True, board.Phase.ToString());
        }

        PointerEventData CreatePointer(int pointerId, int x, int y)
        {
            RectTransform rect = board.GetCellRectForTesting(x, y);
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(
                null,
                rect.TransformPoint(rect.rect.center)
            );
            return new PointerEventData(EventSystem.current)
            {
                pointerId = pointerId,
                position = screen,
                pressPosition = screen,
                button = PointerEventData.InputButton.Left
            };
        }

        static void AssertCue(GameplayMotionKind kind, CoreLoopVfxCue expected)
        {
            GameplayMotionEvent motion = Motion(1, kind);
            Assert.That(
                CoreLoopVfxPresenter.ResolveCue(motion),
                Is.EqualTo(expected),
                kind.ToString()
            );
            Assert.That(
                CoreLoopVfxMapper.TryCreateRequest(
                    motion,
                    out CoreLoopVfxRequest request
                ),
                Is.True,
                kind.ToString()
            );
            Assert.That(request.Cue, Is.EqualTo(expected), kind.ToString());
            Assert.That(request.Primary, Is.EqualTo(motion.Primary));
            Assert.That(request.Secondary, Is.EqualTo(motion.Secondary));
            Assert.That(request.PlanHash, Is.EqualTo(motion.PlanHash));
        }

        static GameplayMotionEvent Motion(
            int sequence,
            GameplayMotionKind kind,
            int combo = 1,
            int affected = 3,
            SpecialGemKind special = SpecialGemKind.None,
            SpecialCombinationKind combination = SpecialCombinationKind.None,
            GameplayBoosterKind booster = GameplayBoosterKind.None
        )
        {
            return new GameplayMotionEvent(
                sequence,
                kind,
                new BoardPosition(3, 4),
                new BoardPosition(4, 4),
                combo,
                affected,
                special,
                "step14-vfx-plan",
                combination,
                booster,
                kind == GameplayMotionKind.MatchWaveStarted ? new[] {
                    new BoardClearedCellInstruction(new BoardPosition(2,4),default),
                    new BoardClearedCellInstruction(new BoardPosition(3,4),default),
                    new BoardClearedCellInstruction(new BoardPosition(4,4),default)
                } : null
            );
        }

        static int IndexOf(
            IReadOnlyList<GameplayMotionEvent> motions,
            GameplayMotionKind kind
        )
        {
            for (int i = 0; i < motions.Count; i++)
                if (motions[i].Kind == kind) return i;
            return -1;
        }
    }
}
