using System;
using System.Linq;
using UnityEngine;

namespace LegendsOfRus
{
    public sealed class CampaignBoardResult
    {
        public bool Won;
        public int Score;
        public int[] Goals;
    }
    public interface ICampaignBoardHost
    {
        bool Start(LevelData level, int committedAttempt, Action<CampaignBoardResult> completed);
        // Disable input and callbacks immediately; destroy only objects owned by this host.
        void Stop();
    }
    public interface ICampaignGameplayRoutes
    {
        bool ShowMap();
        bool ShowResult(CampaignRunReceipt receipt);
        bool ShowInterruptedRound(CampaignRunReceipt receipt);
    }
    public interface ICampaignStoryRoutes { bool ShowStory(CampaignStoryScene story); }
    public enum CampaignGameplayState { Unopened, Ready, Playing, Result, Recovery, NavigationFailed, StorageBlocked, Disposed, Story }

    // Explicit development composition: one coordinator and one writer per save session.
    // No bootstrap, global singleton, scene creation or automatic save migration.
    public sealed class CampaignGameplayCoordinator : IDisposable
    {
        readonly CampaignSaveSession session;
        readonly ICampaignBoardHost board;
        readonly ICampaignGameplayRoutes routes;
        bool busy, constructing;
        long generation, activeSequence;
        CampaignBoardResult deferred;
        public CampaignGameplayState State { get; private set; } = CampaignGameplayState.Unopened;
        public string Diagnostic { get; private set; }
        public CampaignGameplayCoordinator(CampaignSaveSession session, ICampaignBoardHost board, ICampaignGameplayRoutes routes)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.board = board ?? throw new ArgumentNullException(nameof(board));
            this.routes = routes ?? throw new ArgumentNullException(nameof(routes));
        }
        bool Reject(string code) { Diagnostic = code; return false; }
        bool Load(out CampaignPlayerSave save)
        {
            if (session.TryLoad(out save, out var reason)) return true;
            State = CampaignGameplayState.StorageBlocked; Diagnostic = reason; return false;
        }
        bool StopBoard()
        {
            generation++; activeSequence = 0;
            try { board.Stop(); return true; }
            catch { State = CampaignGameplayState.NavigationFailed; return Reject("campaign_board_stop_failed"); }
        }
        bool Route(Func<bool> action, CampaignGameplayState success)
        {
            try
            {
                if (action()) { State = success; Diagnostic = null; return true; }
            }
            catch { /* Durable state remains authoritative; caller can retry navigation. */ }
            State = CampaignGameplayState.NavigationFailed; return Reject("campaign_navigation_failed");
        }
        bool Present(CampaignPlayerSave save)
        {
            if (save.run.state == CampaignRunState.Running)
                return Route(() => routes.ShowInterruptedRound(Clone(save.run)), CampaignGameplayState.Recovery);
            if (save.run.state != CampaignRunState.None && !save.run.acknowledged)
                return Route(() => routes.ShowResult(Clone(save.run)), CampaignGameplayState.Result);
            if (routes is ICampaignStoryRoutes stories)
            {
                if (!session.TryGetPendingStory(out var story, out var reason))
                { State = CampaignGameplayState.StorageBlocked; return Reject(reason); }
                if (story != null) return Route(() => stories.ShowStory(story), CampaignGameplayState.Story);
            }
            return Route(routes.ShowMap, CampaignGameplayState.Ready);
        }
        static CampaignRunReceipt Clone(CampaignRunReceipt receipt) => JsonUtility.FromJson<CampaignRunReceipt>(JsonUtility.ToJson(receipt));

        // Startup/retry navigation reads receipts; it never recreates an unfinished board for free.
        public bool Recover()
        {
            if (busy || State == CampaignGameplayState.Disposed || State == CampaignGameplayState.Playing) return Reject("campaign_coordinator_busy");
            busy = true;
            try {
                if (!StopBoard()) return false;
                var recovered = session.RecoverReservedBooster();
                if (!recovered.Committed) { State = CampaignGameplayState.StorageBlocked; return Reject(recovered.Diagnostic); }
                return Load(out var save) && Present(save);
            }
            finally { busy = false; }
        }
        public bool Start(long expectedSequence, long utcTicks, bool retry = false)
        {
            if (busy || State == CampaignGameplayState.Unopened || State == CampaignGameplayState.Disposed || State == CampaignGameplayState.StorageBlocked || State == CampaignGameplayState.Story)
                return Reject("campaign_coordinator_not_ready");
            busy = true;
            try
            {
                if (!retry && State != CampaignGameplayState.Playing && routes is ICampaignStoryRoutes)
                {
                    if (!Load(out var ready)) return false;
                    if (!session.TryGetPendingStory(out var pending, out var reason))
                    { State = CampaignGameplayState.StorageBlocked; return Reject(reason); }
                    if (pending != null) return Present(ready);
                }
                var operation = session.StartRound(expectedSequence, utcTicks, retry);
                if (!operation.Committed)
                {
                    Diagnostic = operation.Diagnostic;
                    if (!session.TryLoad(out _, out _)) { StopBoard(); State = CampaignGameplayState.StorageBlocked; }
                    return false;
                }
                // A repeated delivery must not reset a board, including a run already completed.
                if (operation.NoOp) { Diagnostic = null; return true; }
                if (!StopBoard() || !Load(out var save)) return false;
                var receipt = save.run;
                activeSequence = receipt.sequence; long token = generation;
                var level = JsonUtility.FromJson<LevelData>(receipt.levelJson);
                int attempt = save.player.levelProgress.First(p => p.chapterId == receipt.chapter && p.levelId == receipt.round).attempts;
                bool created;
                constructing = true; deferred = null; State = CampaignGameplayState.Playing;
                try { created = board.Start(level, attempt, result => Completed(token, receipt.sequence, result)); }
                catch { created = false; }
                finally { constructing = false; }
                if (!created)
                {
                    deferred = null;
                    if (StopBoard()) Present(save);
                    return Reject("campaign_board_start_failed");
                }
                return true;
            }
            finally
            {
                busy = false;
                var queued = deferred; deferred = null;
                if (queued != null && State == CampaignGameplayState.Playing) Completed(generation, activeSequence, queued);
            }
        }
        void Completed(long token, long sequence, CampaignBoardResult result)
        {
            if (State != CampaignGameplayState.Playing || token != generation || sequence != activeSequence || result == null) return;
            var snapshot = new CampaignBoardResult { Won = result.Won, Score = result.Score, Goals = result.Goals == null ? null : (int[])result.Goals.Clone() };
            if (constructing) { if (deferred == null) deferred = snapshot; return; }
            if (busy) return;
            busy = true;
            try
            {
                var operation = session.FinishRound(sequence, snapshot.Won, snapshot.Score, snapshot.Goals);
                // Freeze this finished board regardless of storage outcome. It cannot continue spending moves.
                if (!StopBoard()) return;
                if (!operation.Committed)
                {
                    if (Load(out var unchanged)) Present(unchanged);
                    Diagnostic = operation.Diagnostic; return;
                }
                if (Load(out var save)) Present(save);
            }
            finally { busy = false; }
        }
        public bool AcknowledgeAndReturn(long sequence)
        {
            if (busy || (State != CampaignGameplayState.Result && State != CampaignGameplayState.NavigationFailed)) return Reject("campaign_result_not_ready");
            busy = true;
            try
            {
                var result = session.AcknowledgeResult(sequence, routes is ICampaignStoryRoutes);
                if (!result.Committed) { Diagnostic = result.Diagnostic; if (!session.TryLoad(out _, out _)) State = CampaignGameplayState.StorageBlocked; return false; }
                return StopBoard() && Load(out var save) && Present(save);
            }
            finally { busy = false; }
        }
        public bool CompleteStory(string key)
        {
            if (busy || State != CampaignGameplayState.Story) return Reject("campaign_story_not_ready");
            busy = true;
            try
            {
                var operation = session.AcknowledgeStory(key);
                if (!operation.Committed)
                {
                    Diagnostic = operation.Diagnostic;
                    if (!session.TryLoad(out _, out _)) State = CampaignGameplayState.StorageBlocked;
                    return false;
                }
                return Load(out var save) && Present(save);
            }
            finally { busy = false; }
        }
        // Explicit recovery choice after a lost board: keep spent energy and record one failure.
        // Partial score/cell state is unavailable and is not invented.
        public bool AbandonInterruptedRound(long sequence)
        {
            if (busy || (State != CampaignGameplayState.Recovery && State != CampaignGameplayState.NavigationFailed)) return Reject("campaign_recovery_not_ready");
            busy = true;
            try
            {
                if (!Load(out var save) || save.run.sequence != sequence || save.run.state != CampaignRunState.Running) return Reject("campaign_stale_run");
                var level = JsonUtility.FromJson<LevelData>(save.run.levelJson);
                var result = session.FinishRound(sequence, false, 0, new int[level.goals.Length]);
                if (!result.Committed) { Diagnostic = result.Diagnostic; if (!session.TryLoad(out _, out _)) State = CampaignGameplayState.StorageBlocked; return false; }
                return Load(out save) && Present(save);
            }
            finally { busy = false; }
        }
        public bool Interrupt()
        {
            if (busy || State == CampaignGameplayState.Disposed) return false;
            if (!StopBoard()) return false;
            State = CampaignGameplayState.Recovery;
            return Recover();
        }
        public void Dispose()
        {
            if (State == CampaignGameplayState.Disposed) return;
            StopBoard(); State = CampaignGameplayState.Disposed; deferred = null;
        }
    }
}
