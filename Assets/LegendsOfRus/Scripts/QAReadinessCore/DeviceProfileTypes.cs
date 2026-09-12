using System;

namespace LegendsOfRus.QA
{
    public enum DeviceCapabilityBand
    {
        Unknown = 0,
        BelowMinimum = 1,
        Low = 2,
        Mid = 3,
        High = 4
    }

    public enum DeviceAspectBucket
    {
        Unknown = 0,
        Standard16By9 = 1,
        Tall18By9 = 2,
        Tall19_5By9 = 3,
        Tall20By9 = 4,
        ExtremeTall = 5
    }

    [Serializable]
    public struct DeviceProfileInput
    {
        public int SystemMemoryMb;
        public int GraphicsMemoryMb;
        public int ProcessorCount;
        public int MaxTextureSize;
        public int ScreenWidth;
        public int ScreenHeight;

        public DeviceProfileInput(
            int systemMemoryMb,
            int graphicsMemoryMb,
            int processorCount,
            int maxTextureSize,
            int screenWidth,
            int screenHeight)
        {
            SystemMemoryMb = systemMemoryMb;
            GraphicsMemoryMb = graphicsMemoryMb;
            ProcessorCount = processorCount;
            MaxTextureSize = maxTextureSize;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }
    }

    [Serializable]
    public struct DeviceProfileSnapshot
    {
        public DeviceCapabilityBand CapabilityBand;
        public DeviceAspectBucket AspectBucket;
        public int SystemMemoryMb;
        public int GraphicsMemoryMb;
        public int ProcessorCount;
        public int MaxTextureSize;
        public int ScreenWidth;
        public int ScreenHeight;
        public int AspectPermille;
        public bool HardwareEvidenceComplete;

        public DeviceProfileSnapshot(
            DeviceCapabilityBand capabilityBand,
            DeviceAspectBucket aspectBucket,
            DeviceProfileInput input,
            int aspectPermille,
            bool hardwareEvidenceComplete)
        {
            CapabilityBand = capabilityBand;
            AspectBucket = aspectBucket;
            SystemMemoryMb = input.SystemMemoryMb;
            GraphicsMemoryMb = input.GraphicsMemoryMb;
            ProcessorCount = input.ProcessorCount;
            MaxTextureSize = input.MaxTextureSize;
            ScreenWidth = input.ScreenWidth;
            ScreenHeight = input.ScreenHeight;
            AspectPermille = aspectPermille;
            HardwareEvidenceComplete = hardwareEvidenceComplete;
        }
    }
}
