using System.Collections;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class Match3RuntimePlayModeTests
    {
        static readonly string[] PersistedSaveKeys =
        {
            "LegendsOfRus_Save_v1",
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        GameObject stateObject;
        GameObject eventSystemObject;
        GameObject canvasObject;
        GameObject boardObject;
        Match3Board board;
        bool ownsStateObject;
        bool ownsEventSystemObject;
        bool[] hadPersistedSaves;
        string[] persistedSaveValues;
        GameState previousState;
        string previousRuntimeSaveJson;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            SnapshotAndClearSaveJournal();
            // RuntimeBootstrap normally creates the persistent GameState,
            // ScreenRouter and EventSystem before PlayMode test setup begins.
            // Keep that root alive so runtime toast/navigation calls remain valid.
            yield return null;

            GameState state = GameState.Instance;
            if (state == null)
            {
                stateObject = new GameObject("TestGameState");
                state = stateObject.AddComponent<GameState>();
                ownsStateObject = true;
            }
            else
            {
                previousState = state;
                previousRuntimeSaveJson = JsonUtility.ToJson(state.Save);
            }

            state.ResetProgress();
            state.Save.boosterMace = 5;
            state.Save.boosterFirebird = 5;
            state.Save.boosterLivingLight = 5;
            state.Save.boosterSeaPearl = 5;
            state.Persist();

            if (EventSystem.current == null)
            {
                eventSystemObject = new GameObject(
                    "TestEventSystem",
                    typeof(EventSystem)
                );
                ownsEventSystemObject = true;
            }

            canvasObject = new GameObject(
                "TestCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            boardObject = new GameObject("TestBoard", typeof(RectTransform));
            boardObject.transform.SetParent(canvasObject.transform, false);
            RectTransform boardRect = boardObject.GetComponent<RectTransform>();
            boardRect.anchorMin = new Vector2(0.5f, 0.5f);
            boardRect.anchorMax = new Vector2(0.5f, 0.5f);
            boardRect.pivot = new Vector2(0.5f, 0.5f);
            boardRect.sizeDelta = new Vector2(786f, 942f);

            board = boardObject.AddComponent<Match3Board>();
            board.Initialize(null, null);
            Canvas.ForceUpdateCanvases();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (boardObject != null) Object.Destroy(boardObject);
            if (canvasObject != null) Object.Destroy(canvasObject);
            if (ownsEventSystemObject && eventSystemObject != null)
                Object.Destroy(eventSystemObject);
            if (ownsStateObject && stateObject != null)
                Object.Destroy(stateObject);
            yield return null;
            RestoreSaveJournal();
            RestoreRuntimeState();
        }

        [UnityTest]
        public IEnumerator ActualCellGesture_MapsUpCorrectly()
        {
            yield return AssertGesture(
                new BoardPosition(3, 4),
                Vector2.up,
                new BoardPosition(3, 3)
            );
        }

        [UnityTest]
        public IEnumerator ActualCellGesture_MapsDownCorrectly()
        {
            yield return AssertGesture(
                new BoardPosition(3, 4),
                Vector2.down,
                new BoardPosition(3, 5)
            );
        }

        [UnityTest]
        public IEnumerator ActualCellGesture_MapsLeftCorrectly()
        {
            yield return AssertGesture(
                new BoardPosition(3, 4),
                Vector2.left,
                new BoardPosition(2, 4)
            );
        }

        [UnityTest]
        public IEnumerator ActualCellGesture_MapsRightCorrectly()
        {
            yield return AssertGesture(
                new BoardPosition(3, 4),
                Vector2.right,
                new BoardPosition(4, 4)
            );
        }

        [UnityTest]
        public IEnumerator TapThenAdjacentTap_RequestsSameSwap()
        {
            BoardPosition first = new BoardPosition(3, 4);
            BoardPosition second = new BoardPosition(4, 4);

            Tap(first);
            yield return null;
            Tap(second);
            yield return null;

            Assert.That(board.LastRequestedPrimary, Is.EqualTo(first));
            Assert.That(board.LastRequestedSecondary, Is.EqualTo(second));
            yield return WaitUntilInteractive();
        }

        [UnityTest]
        public IEnumerator PauseResume_PreservesStableBoardHash()
        {
            string before = board.BoardStateHash;

            Assert.That(board.TryPause(), Is.True);
            Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Paused));
            Assert.That(board.TryResume(), Is.True);
            Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Idle));
            Assert.That(board.BoardStateHash, Is.EqualTo(before));
            yield return null;
        }

        [UnityTest]
        public IEnumerator BoosterCancel_DoesNotConsumeInventory()
        {
            int before = GameState.Instance.BoosterCount(BoosterType.Firebird);

            board.UseBooster(BoosterType.Firebird);
            Assert.That(board.IsBoosterArmed, Is.True);
            board.UseBooster(BoosterType.Firebird);

            Assert.That(board.IsBoosterArmed, Is.False);
            Assert.That(
                GameState.Instance.BoosterCount(BoosterType.Firebird),
                Is.EqualTo(before)
            );
            yield return null;
        }

        [UnityTest]
        public IEnumerator InvalidBoosterTarget_DoesNotConsumeInventory()
        {
            Assert.That(board.LoadQAPreset("booster.invalid-empty"), Is.True);
            int before = GameState.Instance.BoosterCount(BoosterType.Firebird);

            board.UseBooster(BoosterType.Firebird);
            board.TapCell(0, 0);
            yield return null;

            Assert.That(board.IsBoosterArmed, Is.True);
            Assert.That(
                GameState.Instance.BoosterCount(BoosterType.Firebird),
                Is.EqualTo(before)
            );
        }

        [UnityTest]
        public IEnumerator BlockedSwap_ReleasesInputWithoutMutationOrMoveSpend()
        {
            Assert.That(board.LoadQAPreset("booster.invalid-empty"), Is.True);
            string beforeHash = board.BoardStateHash;
            int beforeMoves = board.Moves;
            string rejectedReason = null;
            board.CommandRejected += reason => rejectedReason = reason;

            board.SwipeCell(0, 0, Vector2Int.right);
            yield return WaitUntilInteractive();

            Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Idle));
            Assert.That(board.CanAcceptPointerInput, Is.True);
            Assert.That(board.HasActiveCommand, Is.False);
            Assert.That(board.Moves, Is.EqualTo(beforeMoves));
            Assert.That(board.BoardStateHash, Is.EqualTo(beforeHash));
            Assert.That(rejectedReason, Does.Contain("blocks this swap"));
            Assert.That(board.LastRejectedCommandReason, Is.EqualTo(rejectedReason));
        }

        [UnityTest]
        public IEnumerator ValidBoosterTarget_ConsumesExactlyOne()
        {
            Assert.That(board.LoadQAPreset("booster.firebird"), Is.True);
            int before = GameState.Instance.BoosterCount(BoosterType.Firebird);

            board.UseBooster(BoosterType.Firebird);
            BoardQAPresetFactory.TryGet("booster.firebird", out BoardQAPreset preset);
            board.TapCell(preset.Primary.X, preset.Primary.Y);
            yield return WaitUntilInteractive();

            Assert.That(
                GameState.Instance.BoosterCount(BoosterType.Firebird),
                Is.EqualTo(before - 1)
            );
        }

        [UnityTest]
        public IEnumerator TimeoutRecovery_RestoresBoardAndRefundsBooster()
        {
            string beforeHash = board.BoardStateHash;
            int beforeCount = GameState.Instance.BoosterCount(BoosterType.Firebird);

            board.SimulateStuckCommandForTesting(BoosterType.Firebird);
            Assert.That(board.HasActiveCommand, Is.True);
            LogAssert.Expect(LogType.Error,
                "Gameplay command recovered: qa:stuck-command. Reason=Command timeout: qa:stuck-command; board="+beforeHash);
            board.ForceCommandTimeoutForTesting();

            yield return null;
            yield return null;

            Assert.That(board.HasActiveCommand, Is.False);
            Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Idle));
            Assert.That(board.BoardStateHash, Is.EqualTo(beforeHash));
            Assert.That(
                GameState.Instance.BoosterCount(BoosterType.Firebird),
                Is.EqualTo(beforeCount)
            );
        }

        [UnityTest]
        public IEnumerator SpecialCombination_VerifiesEveryRuntimeWaveHash()
        {
            const string id = "special.row-column";
            Assert.That(board.LoadQAPreset(id), Is.True);
            BoardQAPresetFactory.TryGet(id, out BoardQAPreset preset);

            yield return GestureBetween(preset.Primary, preset.Secondary);
            yield return WaitUntilInteractive();

            Assert.That(board.LastWaveVerificationPassed, Is.True);
            Assert.That(board.LastVerifiedWaveCount, Is.GreaterThanOrEqualTo(1));
            Assert.That(board.BoardStateHash, Is.EqualTo(board.LastPlanFinalStateHash));
        }

        IEnumerator AssertGesture(
            BoardPosition source,
            Vector2 direction,
            BoardPosition expectedTarget
        )
        {
            Assert.That(board.LoadQAPreset("special.color-normal"), Is.True);
            yield return Gesture(source, direction);

            Assert.That(board.LastRequestedPrimary, Is.EqualTo(source));
            Assert.That(board.LastRequestedSecondary, Is.EqualTo(expectedTarget));
            yield return WaitUntilInteractive();
        }

        IEnumerator GestureBetween(BoardPosition first, BoardPosition second)
        {
            Vector2 direction = new Vector2(
                second.X - first.X,
                -(second.Y - first.Y)
            );
            yield return Gesture(first, direction);
        }

        IEnumerator Gesture(BoardPosition source, Vector2 direction)
        {
            Match3CellInput input = board.GetCellInputForTesting(source.X, source.Y);
            RectTransform rect = board.GetCellRectForTesting(source.X, source.Y);
            Assert.That(input, Is.Not.Null);
            Assert.That(rect, Is.Not.Null);

            Vector3 centerWorld = rect.TransformPoint(rect.rect.center);
            Vector2 start = RectTransformUtility.WorldToScreenPoint(null, centerWorld);
            Vector2 delta = new Vector2(
                direction.x * rect.rect.width * 0.45f,
                direction.y * rect.rect.height * 0.45f
            );

            var data = new PointerEventData(EventSystem.current)
            {
                position = start,
                pressPosition = start,
                button = PointerEventData.InputButton.Left
            };

            input.OnPointerDown(data);
            data.position = start + delta;
            input.OnDrag(data);
            input.OnPointerUp(data);
            yield return null;
        }

        void Tap(BoardPosition position)
        {
            Match3CellInput input = board.GetCellInputForTesting(position.X, position.Y);
            RectTransform rect = board.GetCellRectForTesting(position.X, position.Y);
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(
                null,
                rect.TransformPoint(rect.rect.center)
            );

            var data = new PointerEventData(EventSystem.current)
            {
                position = screen,
                pressPosition = screen,
                button = PointerEventData.InputButton.Left
            };

            input.OnPointerDown(data);
            input.OnPointerUp(data);
        }

        IEnumerator WaitUntilInteractive(float timeout = 8f)
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

        void SnapshotAndClearSaveJournal()
        {
            hadPersistedSaves = new bool[PersistedSaveKeys.Length];
            persistedSaveValues = new string[PersistedSaveKeys.Length];
            for (int i = 0; i < PersistedSaveKeys.Length; i++)
            {
                string key = PersistedSaveKeys[i];
                hadPersistedSaves[i] = PlayerPrefs.HasKey(key);
                persistedSaveValues[i] = hadPersistedSaves[i]
                    ? PlayerPrefs.GetString(key)
                    : string.Empty;
                PlayerPrefs.DeleteKey(key);
            }
            PlayerPrefs.Save();
        }

        void RestoreRuntimeState()
        {
            if (
                previousState == null ||
                GameState.Instance != previousState ||
                string.IsNullOrEmpty(previousRuntimeSaveJson)
            )
            {
                return;
            }

            PlayerSaveData restored = JsonUtility.FromJson<PlayerSaveData>(
                previousRuntimeSaveJson
            );
            if (restored != null)
                previousState.Save = restored;
        }

        void RestoreSaveJournal()
        {
            for (int i = 0; i < PersistedSaveKeys.Length; i++)
            {
                string key = PersistedSaveKeys[i];
                PlayerPrefs.DeleteKey(key);
                if (hadPersistedSaves[i])
                    PlayerPrefs.SetString(key, persistedSaveValues[i]);
            }
            PlayerPrefs.Save();
        }
    }
}
