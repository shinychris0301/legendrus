using LegendsOfRus.QA;
using UnityEngine;

namespace LegendsOfRus
{
    public sealed class ClosedTestQaService : MonoBehaviour
    {
        public static ClosedTestQaService Instance { get; private set; }

        ClosedTestQaActivationSnapshot activation;
        ClosedTestObservationBuffer observations;

        public ClosedTestQaActivationSnapshot Activation => activation;
        public int ObservationCount => observations != null ? observations.Count : 0;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            activation = ClosedTestQaActivation.Load();
            observations = activation.LocalObservationBufferApproved
                ? new ClosedTestObservationBuffer()
                : null;
            TryRecord(ClosedTestObservationCode.AppBoot);
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public bool TryCaptureCurrentDevice(out DeviceProfileSnapshot snapshot)
        {
            snapshot = default;
            if (!activation.LocalDeviceProfileCaptureApproved)
                return false;

            var input = new DeviceProfileInput(
                SystemInfo.systemMemorySize,
                SystemInfo.graphicsMemorySize,
                SystemInfo.processorCount,
                SystemInfo.maxTextureSize,
                Screen.width,
                Screen.height);
            snapshot = DeviceProfileClassifier.Classify(input);
            return true;
        }

        public bool TryRecord(
            ClosedTestObservationCode code,
            int levelNumber = 0,
            int primaryValue = 0,
            int secondaryValue = 0)
        {
            return observations != null && observations.TryRecord(
                code, levelNumber, primaryValue, secondaryValue);
        }

        public ClosedTestObservation[] CopyObservations()
        {
            return observations != null
                ? observations.Snapshot()
                : new ClosedTestObservation[0];
        }
    }
}
