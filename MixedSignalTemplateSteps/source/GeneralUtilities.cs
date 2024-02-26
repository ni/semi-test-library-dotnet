using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.MixedSignalLibrary
{
    /// <summary>
    /// Defines generic purpose utility methods.
    /// </summary>
    public static class GeneralUtilities
    {
        /// <summary>
        /// Returns the element at a specified index in a sequence or the first element if the index is out of range.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source array.</typeparam>
        /// <param name="values">The array to return an element from.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>The first element if the index is outside the bounds of the source sequence. Otherwise, the element at the specified position in the source sequence.</returns>
        public static T ElementAtOrFirst<T>(this T[] values, int index)
        {
            return values.Length > index ? values[index] : values.First();
        }

        internal static PinInfo FilterPinsAndPinGroups(ISemiconductorModuleContext tsmContext, string[] pins)
        {
            if (pins.Length == 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Parameter_PinNotSpecified));
            }

            var dcPins = new List<string[]>();
            var digitalPins = new List<string[]>();
            var dcPinsFlat = new List<string>();
            var dcPinIndices = new List<int>();
            var digitalPinIndices = new List<int>();
            var digitalPinsFlat = new List<string>();
            for (int i = 0; i < pins.Length; i++)
            {
                var pinArray = new string[] { pins[i] };

                var associatedDCPins = tsmContext.FilterPinsByInstrumentType(pinArray, InstrumentTypeIdConstants.NIDCPower);
                if (associatedDCPins.Length != 0)
                {
                    dcPins.Add(associatedDCPins);
                    dcPinIndices.Add(i);
                    dcPinsFlat.AddRange(associatedDCPins);
                }

                var associatedDigitalPins = tsmContext.FilterPinsByInstrumentType(pinArray, InstrumentTypeIdConstants.NIDigitalPattern);
                if (associatedDigitalPins.Length != 0)
                {
                    digitalPins.Add(associatedDigitalPins);
                    digitalPinIndices.Add(i);
                    digitalPinsFlat.AddRange(associatedDigitalPins);
                }
            }

            return new PinInfo(dcPins, dcPinIndices, dcPinsFlat.ToArray(), digitalPins, digitalPinIndices, digitalPinsFlat.ToArray());
        }

        internal static void VerifySizeOfArrayParameters(
            int expectedSize,
            double[] supplies,
            double[] limits,
            double[] apertureTimes,
            double[] settlingTimes,
            DCPowerMeasurementSense[] senses,
            DCPowerSourceTransientResponse[] transientResponses)
        {
            VerifySizeOfArrayParameter(expectedSize, supplies.Length, nameof(supplies));
            VerifySizeOfArrayParameter(expectedSize, limits.Length, nameof(limits));
            VerifySizeOfArrayParameter(expectedSize, apertureTimes.Length, nameof(apertureTimes));
            VerifySizeOfArrayParameter(expectedSize, settlingTimes.Length, nameof(settlingTimes));
            VerifySizeOfArrayParameter(expectedSize, senses.Length, nameof(senses));
            VerifySizeOfArrayParameter(expectedSize, transientResponses.Length, nameof(transientResponses));
        }

        private static void VerifySizeOfArrayParameter(int expectedSize, int actualSize, string parameterName)
        {
            if (actualSize == 0 || actualSize == 1 || actualSize == expectedSize)
            {
                return;
            }

            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Parameter_ArraySizeInvalid, parameterName, actualSize));
        }
    }

    internal readonly struct PinInfo
    {
        public readonly IList<string[]> DCPins;
        public readonly IList<int> DCPinIndexes;
        public readonly string[] DCPinsFlat;
        public readonly IList<string[]> DigitalPins;
        public readonly IList<int> DigitalPinIndexes;
        public readonly string[] DigitalPinsFlat;

        public PinInfo(IList<string[]> dcPins, IList<int> dcPinIndexes, string[] dcPinsFlat, IList<string[]> digitalPins, IList<int> digitalPinIndexes, string[] digitalPinsFlat)
        {
            DCPins = dcPins;
            DCPinIndexes = dcPinIndexes;
            DCPinsFlat = dcPinsFlat;
            DigitalPins = digitalPins;
            DigitalPinIndexes = digitalPinIndexes;
            DigitalPinsFlat = digitalPinsFlat;
        }
    }
}
