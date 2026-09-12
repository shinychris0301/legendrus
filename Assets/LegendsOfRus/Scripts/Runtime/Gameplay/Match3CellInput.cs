using LegendsOfRus.GameplayCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LegendsOfRus
{
    /// <summary>
    /// Phone-first gem input.
    /// A single authoritative direction resolver is shared by drag and flick paths.
    /// Pointer coordinates are converted into this cell's local UI space before
    /// normalization, removing Canvas scale/aspect ambiguity.
    /// </summary>
    public sealed class Match3CellInput :
        MonoBehaviour,
        IPointerDownHandler,
        IDragHandler,
        IPointerUpHandler,
        ICancelHandler
    {
        public Match3Board Board;
        public int X;
        public int Y;

        RectTransform rect;
        Vector2 pressLocal;
        bool pointerTracked;
        bool swipeSent;
        int activePointerId = int.MinValue;

        void Awake()
        {
            rect = transform as RectTransform;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (
                eventData == null ||
                (pointerTracked && eventData.pointerId != activePointerId)
            )
            {
                return;
            }

            // Track the gesture even if a previous cascade is finishing. The
            // board remains the authority and will accept it only after Idle;
            // this removes the pointer-down timing hole without allowing two
            // commands to overlap.
            pointerTracked = false;
            if (
                Board != null &&
                Board.TryAcquirePointer(eventData.pointerId) &&
                TryGetLocalPosition(eventData, out pressLocal)
            )
            {
                pointerTracked = true;
            }
            else
            {
                Board?.ReleasePointer(eventData.pointerId);
            }

            swipeSent = false;
            activePointerId = pointerTracked
                ? eventData.pointerId
                : int.MinValue;
            eventData.useDragThreshold = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (
                !OwnsPointer(eventData) ||
                swipeSent ||
                Board == null
            )
                return;

            if (!TryGetLocalPosition(eventData, out Vector2 currentLocal))
                return;

            Vector2 normalized = Normalize(currentLocal - pressLocal);
            if (!TryResolveCommittedDirection(normalized, out BoardDirection direction))
                return;

            // Do not consume the gesture until the board actually owns it.
            // Outward edge jitter or a just-finishing cascade can therefore be
            // corrected by a later drag sample or by pointer-up.
            swipeSent = Board.SwipeCell(
                X,
                Y,
                BoardDirectionResolver.ToBoardDelta(direction)
            );
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (
                eventData == null ||
                !pointerTracked ||
                eventData.pointerId != activePointerId
            )
                return;

            // A recovery, preset reload, or focus change may have released the
            // board-wide owner before this cell receives pointer-up. Clear the
            // local latch as well so the next gesture cannot be stranded.
            if (Board == null || !Board.OwnsPointer(eventData.pointerId))
            {
                ResetPointer();
                return;
            }

            if (swipeSent)
            {
                ResetPointer();
                return;
            }

            if (!TryGetLocalPosition(eventData, out Vector2 releaseLocal))
            {
                ResetPointer();
                return;
            }

            Vector2 normalized = Normalize(releaseLocal - pressLocal);
            bool isTap = Mathf.Max(
                Mathf.Abs(normalized.x),
                Mathf.Abs(normalized.y)
            ) < BoardDirectionResolver.DefaultThresholdInCells;
            bool hasDirection = TryResolveCommittedDirection(
                normalized,
                out BoardDirection direction
            );
            ResetPointer();

            if (isTap)
            {
                Board.TapCell(X, Y);
                return;
            }

            // A release beyond the swipe threshold still needs a stable axis.
            // Ambiguous diagonal motion is cancelled instead of committing an
            // arbitrary vertical move or turning into a tap selection.
            if (!hasDirection)
                return;

            // Fast-flick fallback for devices that produce no OnDrag callback.
            Board.SwipeCell(X, Y, BoardDirectionResolver.ToBoardDelta(direction));
        }

        public void OnCancel(BaseEventData eventData)
        {
            ResetPointer();
        }

        void OnDisable()
        {
            ResetPointer();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                ResetPointer();
        }

        void OnApplicationPause(bool paused)
        {
            if (paused)
                ResetPointer();
        }

        bool OwnsPointer(PointerEventData eventData)
        {
            return
                pointerTracked &&
                eventData != null &&
                eventData.pointerId == activePointerId &&
                Board != null &&
                Board.OwnsPointer(eventData.pointerId);
        }

        void ResetPointer()
        {
            int releasedPointer = activePointerId;
            pointerTracked = false;
            swipeSent = false;
            activePointerId = int.MinValue;
            if (releasedPointer != int.MinValue)
                Board?.ReleasePointer(releasedPointer);
        }

        static bool TryResolveCommittedDirection(
            Vector2 normalized,
            out BoardDirection direction
        )
        {
            direction = BoardDirection.None;
            float absX = Mathf.Abs(normalized.x);
            float absY = Mathf.Abs(normalized.y);
            float strongest = Mathf.Max(absX, absY);
            float weakest = Mathf.Min(absX, absY);

            if (strongest < BoardDirectionResolver.DefaultThresholdInCells)
                return false;

            if (
                weakest > 0f &&
                strongest < weakest * CoreLoopMotionProfile.SwipeAxisDominance
            )
            {
                return false;
            }

            direction = BoardDirectionResolver.ResolveNormalized(normalized);
            return direction != BoardDirection.None;
        }

        bool TryGetLocalPosition(PointerEventData eventData, out Vector2 local)
        {
            local = Vector2.zero;
            if (rect == null)
                return false;

            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect,
                eventData.position,
                eventData.pressEventCamera,
                out local
            );
        }

        Vector2 Normalize(Vector2 localDelta)
        {
            if (rect == null)
                return Vector2.zero;

            float width = Mathf.Max(1f, rect.rect.width);
            float height = Mathf.Max(1f, rect.rect.height);
            return new Vector2(localDelta.x / width, localDelta.y / height);
        }
    }
}
