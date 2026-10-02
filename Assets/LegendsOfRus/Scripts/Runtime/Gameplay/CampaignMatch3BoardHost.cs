using System;
using UnityEngine;

namespace LegendsOfRus
{
    // Uses a supplied existing UI container. No Canvas/EventSystem/bootstrap is created.
    public sealed class CampaignMatch3BoardHost : ICampaignBoardHost
    {
        readonly RectTransform parent;
        public Func<ICampaignBoardInventory> InventoryFactory;
        Match3Board current;
        int generation;
        public Match3Board Current => current;
        public CampaignMatch3BoardHost(RectTransform parent) { this.parent = parent ? parent : throw new ArgumentNullException(nameof(parent)); }
        public bool Start(LevelData level, int committedAttempt, Action<CampaignBoardResult> completed)
        {
            if (!parent || current || level == null || committedAttempt < 1 || completed == null) return false;
            var go = new GameObject("DevelopmentCampaignBoard", typeof(RectTransform));
            var rect = (RectTransform)go.transform; rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
            var board = current = go.AddComponent<Match3Board>(); int token = ++generation;
            try
            {
                board.InitializeCampaign(level, committedAttempt, score => Complete(board, token, true, score, completed),
                    score => Complete(board, token, false, score, completed), InventoryFactory?.Invoke());
                return true;
            }
            catch { Stop(); throw; }
        }
        void Complete(Match3Board board, int token, bool won, int score, Action<CampaignBoardResult> completed)
        {
            if (token != generation || !board || board != current) return;
            int[] goals = new int[board.GoalCount];
            for (int i = 0; i < goals.Length; i++) goals[i] = board.GetGoalSlot(i).Current;
            completed(new CampaignBoardResult { Won = won, Score = score, Goals = goals });
        }
        public void Stop()
        {
            generation++;
            if (!current) return;
            var go = current.gameObject; current = null;
            go.SetActive(false);
            if (Application.isPlaying) UnityEngine.Object.Destroy(go);
            else UnityEngine.Object.DestroyImmediate(go);
        }
    }
}
