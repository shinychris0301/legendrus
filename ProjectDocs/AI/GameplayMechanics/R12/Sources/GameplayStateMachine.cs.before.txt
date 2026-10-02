using System;

namespace LegendsOfRus.GameplayCore
{
    public enum GameplayPhase
    {
        Idle = 0,
        Selected,
        BoosterTargeting,
        Swapping,
        InvalidSwapReturning,
        ResolvingMatches,
        CreatingSpecials,
        ActivatingSpecials,
        Falling,
        Refilling,
        Paused,
        Completed,
        Failed
    }

    /// <summary>
    /// Authoritative interaction-state gate for the puzzle board.
    /// It prevents a second tap/swipe/booster request from entering while
    /// the board is swapping, resolving, falling, or completing a level.
    /// </summary>
    public sealed class GameplayStateMachine
    {
        GameplayPhase current = GameplayPhase.Idle;
        GameplayPhase phaseBeforePause = GameplayPhase.Idle;

        public GameplayPhase Current => current;
        public GameplayPhase Previous { get; private set; } = GameplayPhase.Idle;

        public event Action<GameplayPhase, GameplayPhase> Changed;

        public bool CanAcceptPointerInput =>
            current == GameplayPhase.Idle ||
            current == GameplayPhase.Selected ||
            current == GameplayPhase.BoosterTargeting;

        public bool IsTerminal =>
            current == GameplayPhase.Completed ||
            current == GameplayPhase.Failed;

        public bool IsBusy => !CanAcceptPointerInput && current != GameplayPhase.Paused;

        public bool TryTransition(GameplayPhase next)
        {
            if (next == current)
                return true;

            if (!IsAllowed(current, next))
                return false;

            Apply(next);
            return true;
        }

        public void Force(GameplayPhase next)
        {
            if (next == current)
                return;

            Apply(next);
        }

        public bool TryPause()
        {
            if (!CanAcceptPointerInput || IsTerminal)
                return false;

            phaseBeforePause = current;
            Apply(GameplayPhase.Paused);
            return true;
        }

        public bool TryResume()
        {
            if (current != GameplayPhase.Paused)
                return false;

            GameplayPhase resume = phaseBeforePause;
            if (
                resume != GameplayPhase.Idle &&
                resume != GameplayPhase.Selected &&
                resume != GameplayPhase.BoosterTargeting
            )
            {
                resume = GameplayPhase.Idle;
            }

            Apply(resume);
            return true;
        }

        static bool IsAllowed(GameplayPhase from, GameplayPhase to)
        {
            switch (from)
            {
                case GameplayPhase.Idle:
                    return
                        to == GameplayPhase.Selected ||
                        to == GameplayPhase.BoosterTargeting ||
                        to == GameplayPhase.Swapping ||
                        to == GameplayPhase.ActivatingSpecials ||
                        to == GameplayPhase.Paused ||
                        to == GameplayPhase.Completed ||
                        to == GameplayPhase.Failed;

                case GameplayPhase.Selected:
                    return
                        to == GameplayPhase.Idle ||
                        to == GameplayPhase.BoosterTargeting ||
                        to == GameplayPhase.Swapping ||
                        to == GameplayPhase.Paused;

                case GameplayPhase.BoosterTargeting:
                    return
                        to == GameplayPhase.Idle ||
                        to == GameplayPhase.ActivatingSpecials ||
                        to == GameplayPhase.Paused;

                case GameplayPhase.Swapping:
                    return
                        // A planner preflight may reject a blocked command before
                        // any board mutation or swap animation has started.
                        to == GameplayPhase.Idle ||
                        to == GameplayPhase.InvalidSwapReturning ||
                        to == GameplayPhase.ResolvingMatches;

                case GameplayPhase.InvalidSwapReturning:
                    return to == GameplayPhase.Idle;

                case GameplayPhase.ResolvingMatches:
                    return
                        to == GameplayPhase.CreatingSpecials ||
                        to == GameplayPhase.ActivatingSpecials ||
                        to == GameplayPhase.Falling ||
                        to == GameplayPhase.Refilling ||
                        to == GameplayPhase.Idle ||
                        to == GameplayPhase.Completed ||
                        to == GameplayPhase.Failed;

                case GameplayPhase.CreatingSpecials:
                    return
                        to == GameplayPhase.ActivatingSpecials ||
                        to == GameplayPhase.Falling ||
                        to == GameplayPhase.ResolvingMatches;

                case GameplayPhase.ActivatingSpecials:
                    return
                        to == GameplayPhase.ResolvingMatches ||
                        to == GameplayPhase.Falling ||
                        to == GameplayPhase.Refilling ||
                        to == GameplayPhase.Idle ||
                        to == GameplayPhase.Completed ||
                        to == GameplayPhase.Failed;

                case GameplayPhase.Falling:
                    return
                        to == GameplayPhase.Refilling ||
                        to == GameplayPhase.ResolvingMatches ||
                        to == GameplayPhase.Completed ||
                        to == GameplayPhase.Failed;

                case GameplayPhase.Refilling:
                    return
                        to == GameplayPhase.ResolvingMatches ||
                        to == GameplayPhase.Idle ||
                        to == GameplayPhase.Completed ||
                        to == GameplayPhase.Failed;

                case GameplayPhase.Paused:
                    return
                        to == GameplayPhase.Idle ||
                        to == GameplayPhase.Selected ||
                        to == GameplayPhase.BoosterTargeting;

                case GameplayPhase.Completed:
                case GameplayPhase.Failed:
                    return false;

                default:
                    return false;
            }
        }

        void Apply(GameplayPhase next)
        {
            GameplayPhase old = current;
            Previous = old;
            current = next;
            Changed?.Invoke(old, next);
        }
    }
}
