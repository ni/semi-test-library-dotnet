using System;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines helper methods to aid with programming the Digital instrumentation.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Packs digital parallel capture data from each <see langword="uint"/> sample into a single <see langword="long"/> value per pin and site.
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
        /// <param name="pinBitOrder">The bit order for the pins.</param>
        /// <param name="sampleBitOrder">The bit order for the samples.</param>
        /// <returns>
        /// <see cref="PinSiteData{T}"/> object containing a single <see langword="long"/> value representing the capture samples of each pin and site.
        /// </returns>
        public static PinSiteData<long> PackParallelCaptureDataIntoLong(this SiteData<uint[]> siteData, string[] pinNames, BitOrder pinBitOrder = BitOrder.MSBFirst, BitOrder sampleBitOrder = BitOrder.MSBFirst)
        {
            return UnpackParallelCaptureDataByPinCore(siteData, pinNames, PackParallelSamplesInToLongByPin, pinBitOrder, sampleBitOrder);
        }

        /// <summary>
        /// Packs digital parallel capture data from each <see langword="uint"/> sample into multiple <see langword="long"/> value per pin and site.<br/>
        /// Use this method when the number of samples exceeds 64, which is the maximum number of bits that can be represented by a single <see langword="long"/> value.
        /// </summary>
        /// <returns>
        /// <see cref="PinSiteData{T}"/> object containing a multiple <see langword="long"/> values representing the capture samples of each pin and site.
        /// </returns>
        /// <inheritdoc cref="PackParallelCaptureDataIntoLong(SiteData{uint[]}, string[], BitOrder, BitOrder)"/>
        public static PinSiteData<long[]> PackParallelCaptureDataIntoLongArray(this SiteData<uint[]> siteData, string[] pinNames, BitOrder pinBitOrder = BitOrder.MSBFirst, BitOrder sampleBitOrder = BitOrder.MSBFirst)
        {
            return UnpackParallelCaptureDataByPinCore(siteData, pinNames, PackParallelSamplesInToLongArrayByPin, pinBitOrder, sampleBitOrder);
        }

        /// <summary>
        /// Unpacks digital parallel capture data from each <see langword="uint"/> sample to reformat it as per-pin data.
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
        /// <param name="pinBitOrder">The bit order for the pins.</param>
        /// <param name="sampleBitOrder">The bit order for the samples.</param>
        /// <returns>
        /// <see cref="PinSiteData{T}"/> object containing pin and site samples formatted as <see langword="uint"/>[].
        /// </returns>
        public static PinSiteData<uint[]> UnpackParallelCaptureDataByPinAsUintArray(this SiteData<uint[]> siteData, string[] pinNames, BitOrder pinBitOrder = BitOrder.MSBFirst, BitOrder sampleBitOrder = BitOrder.MSBFirst)
        {
            return UnpackParallelCaptureDataByPinCore(siteData, pinNames, UnpackBitsFromEachSampleAsUintArray, pinBitOrder, sampleBitOrder);
        }

        /// <returns>
        /// <see cref="PinSiteData{T}"/> object containing pin and site samples formatted as <see langword="bool"/>[].
        /// </returns>
        /// <inheritdoc cref="UnpackParallelCaptureDataByPinAsUintArray(SiteData{uint[]}, string[], BitOrder, BitOrder)"/>
        public static PinSiteData<bool[]> UnpackParallelCaptureDataByPinAsBoolArray(this SiteData<uint[]> siteData, string[] pinNames, BitOrder pinBitOrder = BitOrder.MSBFirst, BitOrder sampleBitOrder = BitOrder.MSBFirst)
        {
            return UnpackParallelCaptureDataByPinCore(siteData, pinNames, UnpackBitsFromEachSampleAsBoolArray, pinBitOrder, sampleBitOrder);
        }

        private static PinSiteData<TValue> UnpackParallelCaptureDataByPinCore<TValue>(
            SiteData<uint[]> siteData,
            string[] pinNames,
            Func<uint[], int, BitOrder, TValue> sampleBitConverter,
            BitOrder pinBitOrder = BitOrder.MSBFirst,
            BitOrder sampleBitOrder = BitOrder.MSBFirst)
        {
            if (siteData == null)
            {
                throw new ArgumentNullException(nameof(siteData));
            }

            if (pinNames == null)
            {
                throw new ArgumentNullException(nameof(pinNames));
            }

            if (pinNames.Length == 0)
            {
                throw new ArgumentException("At least one pin name is required.", nameof(pinNames));
            }

            int[] siteNumbers = siteData.SiteNumbers;
            uint[][] siteSamples = new uint[siteNumbers.Length][];
            for (int siteIndex = 0; siteIndex < siteNumbers.Length; siteIndex++)
            {
                uint[] samples = siteData.GetValue(siteNumbers[siteIndex]);

                siteSamples[siteIndex] = samples;
            }

            TValue[][] perPinPerSiteData = new TValue[pinNames.Length][];
            for (int pinIndex = 0; pinIndex < pinNames.Length; pinIndex++)
            {
                perPinPerSiteData[pinIndex] = new TValue[siteNumbers.Length];
                int pinShift = GetPinShiftAmount(pinIndex, pinNames.Length, pinBitOrder);

                for (int siteIndex = 0; siteIndex < siteNumbers.Length; siteIndex++)
                {
                    perPinPerSiteData[pinIndex][siteIndex] = sampleBitConverter(siteSamples[siteIndex], pinShift, sampleBitOrder);
                }
            }

            return new PinSiteData<TValue>(pinNames, siteNumbers, perPinPerSiteData);
        }

        private static uint[] UnpackBitsFromEachSampleAsUintArray(uint[] samples, int pinShiftAmount, BitOrder sampleBitOrder)
        {
            if (samples.Length <= 0 || samples.Length > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(samples), $"The number of samples must be between 1 and 32 to be able to unpack as uint array.");
            }

            uint[] result = new uint[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                int sourceIndex = GetOrderedSampleIndex(i, samples.Length, sampleBitOrder);
                result[i] = (samples[sourceIndex] >> pinShiftAmount) & 1u;
            }

            return result;
        }

        private static bool[] UnpackBitsFromEachSampleAsBoolArray(uint[] samples, int pinShiftAmount, BitOrder sampleBitOrder)
        {
            if (samples.Length <= 0 || samples.Length > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(samples), $"The number of samples must be between 1 and 64 to be able to pack into a bool array.");
            }

            bool[] result = new bool[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                int sourceIndex = GetOrderedSampleIndex(i, samples.Length, sampleBitOrder);
                result[i] = ((samples[sourceIndex] >> pinShiftAmount) & 1L) == 1L;
            }

            return result;
        }

        private static long PackParallelSamplesInToLongByPin(uint[] samples, int pinShiftAmount, BitOrder sampleBitOrder)
        {
            if ((long)samples.Length > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(samples), "Packed value exceeds 64 bits. Use a multi-value packing strategy.");
            }

            long value = 0;
            for (int sampleIndex = 0; sampleIndex < samples.Length; sampleIndex++)
            {
                int sourceIndex = GetOrderedSampleIndex(sampleIndex, samples.Length, sampleBitOrder);
                long bit = (samples[sourceIndex] >> pinShiftAmount) & 1u;
                value |= bit << sampleIndex;
            }

            return value;
        }

        private static long[] PackParallelSamplesInToLongArrayByPin(uint[] samples, int pinShiftAmount, BitOrder sampleBitOrder)
        {
            int numLongs = (samples.Length + 63) / 64; // Calculate the number of long values needed to store all samples
            long[] packedValues = new long[numLongs];

            for (int sampleIndex = 0; sampleIndex < samples.Length; sampleIndex++)
            {
                int sourceIndex = GetOrderedSampleIndex(sampleIndex, samples.Length, sampleBitOrder);
                long bit = (samples[sourceIndex] >> pinShiftAmount) & 1u;
                int longIndex = sampleIndex / 64; // Determine which long value to store the bit in
                int bitPosition = sampleIndex % 64; // Determine the position of the bit within the long value
                packedValues[longIndex] |= bit << bitPosition;
            }

            return packedValues;
        }

        private static int GetPinShiftAmount(int pinIndex, int pinCount, BitOrder pinBitOrder)
        {
            switch (pinBitOrder)
            {
                case BitOrder.MSBFirst:
                    return pinCount - 1 - pinIndex;
                case BitOrder.LSBFirst:
                    return pinIndex;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pinBitOrder), pinBitOrder, "Unsupported bit order.");
            }
        }

        private static int GetOrderedSampleIndex(int index, int length, BitOrder sampleBitOrder)
        {
            switch (sampleBitOrder)
            {
                case BitOrder.MSBFirst:
                    return length - 1 - index;
                case BitOrder.LSBFirst:
                    return index;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sampleBitOrder), sampleBitOrder, "Unsupported bit order.");
            }
        }

        /// <summary>
        /// Defines the bit order for interpreting the bits of a digital capture data.
        /// </summary>
        public enum BitOrder
        {
            /// <summary>
            /// Most significant bit (MSB) is the first bit.
            /// </summary>
            MSBFirst,
            /// <summary>
            /// Least significant bit (LSB) is the first bit.
            /// </summary>
            LSBFirst
        }
    }
}
