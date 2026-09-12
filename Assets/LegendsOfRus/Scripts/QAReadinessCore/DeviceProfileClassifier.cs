using System;

namespace LegendsOfRus.QA
{
    public static class DeviceProfileClassifier
    {
        // These are QA prioritization bands, not store/device support promises.
        public const int MinimumSystemMemoryMb = 2048;
        public const int MinimumMaxTextureSize = 2048;
        public const int MinimumProcessorCount = 2;

        public static DeviceProfileSnapshot Classify(DeviceProfileInput input)
        {
            bool complete = input.SystemMemoryMb > 0 &&
                input.ProcessorCount > 0 &&
                input.MaxTextureSize > 0 &&
                input.ScreenWidth > 0 &&
                input.ScreenHeight > 0;

            int aspectPermille = AspectPermille(input.ScreenWidth, input.ScreenHeight);
            DeviceAspectBucket aspect = ClassifyAspect(aspectPermille);
            DeviceCapabilityBand capability = complete
                ? ClassifyCapability(input)
                : DeviceCapabilityBand.Unknown;

            return new DeviceProfileSnapshot(
                capability,
                aspect,
                input,
                aspectPermille,
                complete);
        }

        public static DeviceCapabilityBand ClassifyCapability(DeviceProfileInput input)
        {
            if (input.SystemMemoryMb <= 0 ||
                input.ProcessorCount <= 0 ||
                input.MaxTextureSize <= 0)
                return DeviceCapabilityBand.Unknown;

            if (input.SystemMemoryMb < MinimumSystemMemoryMb ||
                input.ProcessorCount < MinimumProcessorCount ||
                input.MaxTextureSize < MinimumMaxTextureSize)
                return DeviceCapabilityBand.BelowMinimum;

            if (input.SystemMemoryMb < 4096 ||
                input.ProcessorCount < 4 ||
                (input.GraphicsMemoryMb > 0 && input.GraphicsMemoryMb < 512))
                return DeviceCapabilityBand.Low;

            if (input.SystemMemoryMb < 6144 ||
                input.ProcessorCount < 6 ||
                (input.GraphicsMemoryMb > 0 && input.GraphicsMemoryMb < 1024))
                return DeviceCapabilityBand.Mid;

            return DeviceCapabilityBand.High;
        }

        public static DeviceAspectBucket ClassifyAspect(int aspectPermille)
        {
            if (aspectPermille <= 0)
                return DeviceAspectBucket.Unknown;
            if (aspectPermille <= 1890)
                return DeviceAspectBucket.Standard16By9;
            if (aspectPermille <= 2080)
                return DeviceAspectBucket.Tall18By9;
            if (aspectPermille <= 2195)
                return DeviceAspectBucket.Tall19_5By9;
            if (aspectPermille <= 2280)
                return DeviceAspectBucket.Tall20By9;
            return DeviceAspectBucket.ExtremeTall;
        }

        public static int AspectPermille(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return 0;
            int longSide = Math.Max(width, height);
            int shortSide = Math.Min(width, height);
            return (int)Math.Round((longSide * 1000.0) / shortSide);
        }
    }
}
