using System;
using UnityEngine;

namespace LegendsOfRus.CommercialRuntime
{
    /// <summary>
    /// Independent pre-activation bootstrap. It does not create a GameObject,
    /// touch GameState, register an SDK, or participate in scene routing.
    /// Existing offline boot remains independent if this boundary rejects.
    /// </summary>
    public static class CommercialRuntimeBootstrap
    {
        static CommercialRuntimeComposition current =
            CommercialRuntimeComposition.SafeDisabled();

        public static CommercialRuntimeComposition Current
        {
            get { return current; }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetForDomainReloadDisabled()
        {
            current = CommercialRuntimeComposition.SafeDisabled();
            global::LegendsOfRus.AnalyticsService.ResetTrusted();
            global::LegendsOfRus.AnalyticsService.TryInstallTrusted(
                global::LegendsOfRus.RuntimeAnalyticsBridge.Disabled
            );
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Boot()
        {
            // Publish the conservative state before performing any load. An
            // exception at any later point therefore leaves a usable offline game.
            current = CommercialRuntimeComposition.SafeDisabled();
            global::LegendsOfRus.AnalyticsService.TryInstallTrusted(
                global::LegendsOfRus.RuntimeAnalyticsBridge.Disabled
            );
            try
            {
                CommercialRuntimeComposition candidate =
                    CommercialRuntimeComposition.BuildTrustedDisabled();
                if (candidate != null)
                    current = candidate;
            }
            catch (Exception)
            {
                current = CommercialRuntimeComposition.SafeDisabled();
            }

            Debug.Log(
                "[COMMERCIAL_RUNTIME] phase=" + current.Activation.Phase +
                " reason=" + current.Activation.ReasonCode +
                " analytics=" + current.Capabilities.AnalyticsCaptureEnabled +
                " iap=" + current.Capabilities.IapEnabled +
                " rewarded_ads=" + current.Capabilities.RewardedAdsEnabled +
                " durable_fulfillment=" +
                current.Capabilities.DurableFulfillmentEnabled
            );
        }
    }
}
