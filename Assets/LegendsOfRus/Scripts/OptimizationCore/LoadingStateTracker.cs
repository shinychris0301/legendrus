namespace LegendsOfRus.OptimizationCore
{
    public enum RuntimeLoadingPhase
    {
        NotStarted = 0,
        Bootstrap = 1,
        LocalState = 2,
        UiConstruction = 3,
        Ready = 4,
        Failed = 5
    }

    public sealed class LoadingStateTracker
    {
        public RuntimeLoadingPhase Phase { get; private set; }
        public double Progress01 { get; private set; }

        public LoadingStateTracker()
        {
            Phase = RuntimeLoadingPhase.NotStarted;
            Progress01 = 0.0;
        }

        public bool TryAdvance(RuntimeLoadingPhase phase, double progress01)
        {
            if (phase == RuntimeLoadingPhase.Failed)
            {
                Phase = RuntimeLoadingPhase.Failed;
                return true;
            }

            if (Phase == RuntimeLoadingPhase.Failed ||
                phase < Phase ||
                phase == RuntimeLoadingPhase.NotStarted ||
                progress01 < 0.0 || progress01 > 1.0 ||
                progress01 < Progress01)
                return false;

            if (phase == RuntimeLoadingPhase.Ready && progress01 != 1.0)
                return false;

            Phase = phase;
            Progress01 = progress01;
            return true;
        }
    }
}
