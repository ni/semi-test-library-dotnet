using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines helper methods to aid with programming the Digital instrumentation.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Unpacks digital parallel capture data from each <see langword="uint"/> sample to reformat it as per-pin data.<br/>
        /// </summary>
        /// <remarks>
        /// Each pin's value is represented by an individual bit of each <see langword="uint"/> sample,
        /// where a bit value indicates the state of a particular pin (1 for high, 0 for low).<br/>
        /// This method assumes that the order of the pin names in the <paramref name="pinNames"/> array,
        /// corresponds to the bit positions in the <see langword="uint"/> samples,
        /// which also matches the order of the pins in the pattern.
        /// The first pin in the <paramref name="pinNames"/> array corresponds to the most significant bit (MSB) of the <see langword="uint"/> sample,
        /// and the last pin corresponds to the least significant bit (LSB).
        /// </remarks>
        /// <param name="siteData">The site data containing the parallel capture samples for each site.</param>
        /// <param name="pinNames">The names of the pins corresponding to the pins in the pattern.</param>
        /// <returns>
        /// <see cref="PinSiteData{T}"/> object containing pin and site samples formatted as <see langword="uint"/>[].
        /// </returns>
        public static PinSiteData<uint[]> UnpackParallelCaptureDataByPin(this SiteData<uint[]> siteData, string[] pinNames)
        {
            return UnpackParallelCaptureDataByPinCore(siteData, pinNames, UnpackBitFromUintAsUint);
        }

        /// <returns>
        /// <see cref="PinSiteData{T}"/> object containing pin and site samples formatted as <see langword="bool"/>[].
        /// </returns>
        /// <inheritdoc cref="UnpackParallelCaptureDataByPin(SiteData{uint[]}, string[])"/>
        public static PinSiteData<bool[]> UnpackParallelCaptureDataByPinAsBoolArray(this SiteData<uint[]> siteData, string[] pinNames)
        {
            return UnpackParallelCaptureDataByPinCore(siteData, pinNames, UnpackBitFromUintAsBool);
        }

        private static PinSiteData<T[]> UnpackParallelCaptureDataByPinCore<T>(SiteData<uint[]> siteData, string[] pinNames, System.Func<uint, int, T> sampleBitConverter)
        {
            int[] siteNumbers = siteData.SiteNumbers;
            T[][][] perPinPerSiteData = new T[pinNames.Length][][];
            uint[][] siteSamples = new uint[siteNumbers.Length][];

            // Call GetValue upfront so that it does not have to be repeated for each pin in the loop below.
            for (int siteIndex = 0; siteIndex < siteNumbers.Length; siteIndex++)
            {
                siteSamples[siteIndex] = siteData.GetValue(siteNumbers[siteIndex]);
            }

            for (int pinIndex = 0; pinIndex < pinNames.Length; pinIndex++)
            {
                perPinPerSiteData[pinIndex] = new T[siteNumbers.Length][];
                int shift = pinNames.Length - 1 - pinIndex; // first pin maps to MSB
                for (int siteIndex = 0; siteIndex < siteNumbers.Length; siteIndex++)
                {
                    uint[] samples = siteSamples[siteIndex];
                    T[] pinSampleValues = new T[samples.Length];
                    for (int sampleIndex = 0; sampleIndex < samples.Length; sampleIndex++)
                    {
                        pinSampleValues[sampleIndex] = sampleBitConverter(samples[sampleIndex], shift);
                    }

                    perPinPerSiteData[pinIndex][siteIndex] = pinSampleValues;
                }
            }

            return new PinSiteData<T[]>(pinNames, siteNumbers, perPinPerSiteData);
        }

        private static uint UnpackBitFromUintAsUint(uint sample, int bitShift)
        {
            return (sample >> bitShift) & 1u;
        }

        private static bool UnpackBitFromUintAsBool(uint sample, int bitShift)
        {
            return UnpackBitFromUintAsUint(sample, bitShift) == 1u;
        }
    }
}
