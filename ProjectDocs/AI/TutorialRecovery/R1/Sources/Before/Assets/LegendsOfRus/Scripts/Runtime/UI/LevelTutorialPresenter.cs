using System;
using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Presents authored tutorial steps above the immutable approved Gameplay skin.
    /// The board is paused only while a step is visible and resumes after its queue drains.
    /// </summary>
    public sealed class LevelTutorialPresenter : MonoBehaviour
    {
        const float DesignWidth = 941f;
        const float DesignHeight = 1672f;

        Match3Board board;
        LevelTutorialSequence sequence;
        GameObject overlay;
        bool ownsPause;
        int previousMoves;
        int previousGoalOne;
        int previousGoalTwo;

        public LevelTutorialStepData CurrentStep { get; private set; }
        public bool IsPresenting => overlay != null;
        public int PendingStepCount => sequence?.PendingCount ?? 0;

        public void Initialize(Match3Board match3Board, LevelData level)
        {
            Unsubscribe();
            board = match3Board;
            LevelTutorialStepData[] authored = level?.tutorialSteps ??
                Array.Empty<LevelTutorialStepData>();
            var remaining = new List<LevelTutorialStepData>(authored.Length);
            for (int i = 0; i < authored.Length; i++)
            {
                LevelTutorialStepData step = authored[i];
                if (
                    step != null &&
                    (GameState.Instance == null ||
                        !GameState.Instance.HasCompletedTutorial(step.id))
                )
                {
                    remaining.Add(step);
                }
            }
            sequence = new LevelTutorialSequence(remaining.ToArray());
            previousMoves = board != null ? board.Moves : 0;
            previousGoalOne = board != null ? board.GoalSlot1Remaining : 0;
            previousGoalTwo = board != null ? board.GoalSlot2Remaining + (board.GetGoalSlot(2)?.Remaining ?? 0) : 0;

            if (board == null || remaining.Count == 0) return;

            board.MovesChanged += OnMovesChanged;
            board.GoalSlotsChanged += OnGoalSlotsChanged;
            board.ObstacleInteracted += OnObstacleInteracted;
            board.ObjectiveItemCollected += OnObjectiveItemCollected;
            sequence.Signal(LevelTutorialTriggerTypes.LevelStart);
            TryPresentNext();
        }

        void Update()
        {
            if (
                overlay == null &&
                sequence != null &&
                sequence.HasPending &&
                board != null &&
                board.CanAcceptPointerInput
            )
            {
                TryPresentNext();
            }
        }

        void OnDestroy()
        {
            Unsubscribe();
            if (ownsPause && board != null)
                board.TryResume();
        }

        void Unsubscribe()
        {
            if (board == null) return;
            board.MovesChanged -= OnMovesChanged;
            board.GoalSlotsChanged -= OnGoalSlotsChanged;
            board.ObstacleInteracted -= OnObstacleInteracted;
            board.ObjectiveItemCollected -= OnObjectiveItemCollected;
        }

        void OnMovesChanged(int value)
        {
            if (value < previousMoves)
                sequence?.Signal(LevelTutorialTriggerTypes.FirstMove);
            previousMoves = value;
        }

        void OnGoalSlotsChanged(int first, int second)
        {
            second += board != null ? board.GetGoalSlot(2)?.Remaining ?? 0 : 0;
            if (first < previousGoalOne || second < previousGoalTwo)
                sequence?.Signal(LevelTutorialTriggerTypes.FirstGoalProgress);
            previousGoalOne = first;
            previousGoalTwo = second;
        }

        void OnObstacleInteracted(BoardObstacleInteractionInstruction interaction)
        {
            sequence?.Signal(LevelTutorialTriggerTypes.FirstObstacleHit);
        }

        void OnObjectiveItemCollected(BoardObjectiveItemCollection collection)
        {
            sequence?.Signal(LevelTutorialTriggerTypes.FirstItemCollected);
        }

        void TryPresentNext()
        {
            if (sequence == null || !sequence.HasPending || board == null) return;
            if (!ownsPause)
            {
                if (!board.TryPause()) return;
                ownsPause = true;
            }

            CurrentStep = sequence.DequeueNext();
            if (CurrentStep == null)
            {
                ResumeBoard();
                return;
            }

            BuildOverlay(CurrentStep);
            PrivacyAwareAnalytics.TryTutorialStepView(
                board.CurrentLevel.id,
                CurrentStep.id,
                CurrentStep.trigger
            );
            BalanceTelemetryReviewService.Instance?.TryRecordTutorialShown(
                board.CurrentLevel, CurrentStep.id);
        }

        void BuildOverlay(LevelTutorialStepData step)
        {
            if (overlay != null) Destroy(overlay);

            overlay = new GameObject(
                "TutorialOverlay_" + step.id,
                typeof(RectTransform),
                typeof(Image)
            );
            overlay.transform.SetParent(transform, false);
            Stretch(overlay.GetComponent<RectTransform>());
            var dim = overlay.GetComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.42f);
            dim.raycastTarget = true;

            var panel = new GameObject(
                "TutorialPanel",
                typeof(RectTransform),
                typeof(Image),
                typeof(Outline)
            );
            panel.transform.SetParent(overlay.transform, false);
            SetPixelRect(panel.GetComponent<RectTransform>(), 100f, 520f, 741f, 450f);
            panel.GetComponent<Image>().color = new Color32(6, 22, 40, 252);
            var outline = panel.GetComponent<Outline>();
            outline.effectColor = new Color32(214, 164, 78, 255);
            outline.effectDistance = new Vector2(3f, -3f);

            Text eyebrow = CreateText(
                panel.GetComponent<RectTransform>(),
                "TutorialEyebrow",
                "ОБУЧЕНИЕ • " + MechanicLabel(step.mechanic),
                22,
                TextAnchor.MiddleCenter
            );
            SetLocalRect(eyebrow.rectTransform, 42f, 28f, 657f, 42f);
            eyebrow.color = new Color32(214, 164, 78, 255);

            Text title = CreateText(
                panel.GetComponent<RectTransform>(),
                "TutorialTitle",
                step.title,
                39,
                TextAnchor.MiddleCenter
            );
            SetLocalRect(title.rectTransform, 42f, 78f, 657f, 72f);
            title.color = new Color32(250, 229, 181, 255);
            title.fontStyle = FontStyle.Bold;

            Text body = CreateText(
                panel.GetComponent<RectTransform>(),
                "TutorialBody",
                step.body,
                28,
                TextAnchor.MiddleCenter
            );
            SetLocalRect(body.rectTransform, 58f, 156f, 625f, 154f);
            body.color = new Color32(242, 237, 222, 255);
            body.horizontalOverflow = HorizontalWrapMode.Wrap;
            body.verticalOverflow = VerticalWrapMode.Truncate;
            body.resizeTextForBestFit = true;
            body.resizeTextMinSize = 21;
            body.resizeTextMaxSize = 28;

            var button = new GameObject(
                "TutorialContinue",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(Outline)
            );
            button.transform.SetParent(panel.transform, false);
            SetLocalRect(button.GetComponent<RectTransform>(), 205f, 332f, 331f, 82f);
            button.GetComponent<Image>().color = new Color32(134, 40, 30, 255);
            var buttonOutline = button.GetComponent<Outline>();
            buttonOutline.effectColor = new Color32(232, 190, 104, 255);
            buttonOutline.effectDistance = new Vector2(2f, -2f);
            Button action = button.GetComponent<Button>();
            action.navigation = new Navigation { mode = Navigation.Mode.None };
            action.onClick.AddListener(Advance);

            Text label = CreateText(
                button.GetComponent<RectTransform>(),
                "Label",
                sequence.HasPending ? "ДАЛЕЕ" : "ПОНЯТНО",
                29,
                TextAnchor.MiddleCenter
            );
            Stretch(label.rectTransform);
            label.color = new Color32(255, 235, 189, 255);
            label.fontStyle = FontStyle.Bold;
        }

        public void Advance()
        {
            if (CurrentStep != null && board != null)
            {
                GameState state = GameState.Instance;
                bool completedNow =
                    state != null && state.CompleteTutorial(CurrentStep.id);
                if (completedNow)
                {
                    PrivacyAwareAnalytics.TryTutorialStepComplete(
                        board.CurrentLevel.id,
                        CurrentStep.id
                    );
                    BalanceTelemetryReviewService.Instance?.TryRecordTutorialCompleted(
                        board.CurrentLevel, CurrentStep.id);
                }
            }

            CurrentStep = null;
            if (overlay != null) Destroy(overlay);
            overlay = null;

            if (sequence != null && sequence.HasPending)
                TryPresentNext();
            else
                ResumeBoard();
        }

        void ResumeBoard()
        {
            if (ownsPause && board != null)
                board.TryResume();
            ownsPause = false;
        }

        static string MechanicLabel(string mechanic)
        {
            switch (mechanic)
            {
                case LevelMechanicKeys.Match: return "САМОЦВЕТЫ";
                case LevelMechanicKeys.Score: return "ОЧКИ";
                case LevelMechanicKeys.Ice: return "ЛЁД";
                case LevelMechanicKeys.Chain: return "ЦЕПИ";
                case LevelMechanicKeys.Crate: return "ЯЩИКИ";
                case LevelMechanicKeys.BringDown: return "ПЕРО ЖАР-ПТИЦЫ";
                case LevelMechanicKeys.Survival: return "ИСПЫТАНИЕ";
                default: return "НОВАЯ МЕХАНИКА";
            }
        }

        static Text CreateText(
            RectTransform parent,
            string name,
            string value,
            int size,
            TextAnchor anchor
        )
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null)
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = anchor;
            text.raycastTarget = false;
            return text;
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static void SetPixelRect(
            RectTransform rect,
            float x,
            float yTop,
            float width,
            float height
        )
        {
            rect.anchorMin = new Vector2(x / DesignWidth, 1f - (yTop + height) / DesignHeight);
            rect.anchorMax = new Vector2((x + width) / DesignWidth, 1f - yTop / DesignHeight);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static void SetLocalRect(
            RectTransform rect,
            float x,
            float yTop,
            float width,
            float height
        )
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(x, -yTop);
            rect.sizeDelta = new Vector2(width, height);
        }
    }
}
