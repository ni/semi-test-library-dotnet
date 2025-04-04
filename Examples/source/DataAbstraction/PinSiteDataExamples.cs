using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.DataAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Data Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to use the PinStieData objects.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// that has already been initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class PinSiteDataExamples
    {
        // Generate Random Values
        private static readonly Random RandomNumber = new Random();
        private static readonly int SiteCount = 4;
        private static readonly string[] PinNames = new string[] { "PinA", "PinB", "PinC" };

        internal static void PinSiteDataMath()
        {
            PinSiteData<double> measurementA = Measure();
            PinSiteData<double> measurementB = Measure();

            // Binary Operations: Add, Compare, Divide, Maximum, Minimum, Multiply, Power, Subtract.
            PinSiteData<double> measurementBAddOffset = measurementB.Add(5);
            PinSiteData<bool> measurementsInBGreaterThanFive = measurementB.Compare(ComparisonType.GreaterThan, 5);
            PinSiteData<double> ratio = measurementB.Divide(measurementA);
            PinSiteData<double> maximumAToB = measurementA.Maximum(measurementB);
            PinSiteData<double> minimumA = measurementA.Minimum(2);
            PinSiteData<double> measurementAMultipledByTwo = measurementA.Multiply(2);
            PinSiteData<double> measurementAValuesToPowerOfTwo = measurementA.Power(2);
            PinSiteData<double> difference = measurementB.Subtract(measurementA);

            // Binary Operations Using Supported Operators (added in 24.5.1): Add, Subtract, Multiply, Divide.
            PinSiteData<double> measurementBAddmeasurementA = measurementB + measurementA;
            PinSiteData<double> measurementASubtractOffset = measurementB - 5;
            PinSiteData<double> measurementBMultipledByTwo = measurementB * 2;
            PinSiteData<double> ratioAOverB = measurementA / measurementB;

            // Unary Operations: Abs,  Invert, Log10, Negate, SquareRoot, Truncate.
            PinSiteData<double> absOfDifference = difference.Abs();
            PinSiteData<double> inverseOfMeasurementA = measurementB.Invert();
            PinSiteData<double> log10OfMeasurementA = measurementA.Log10();
            PinSiteData<double> negationOfMeasurementB = measurementB.Negate();
            PinSiteData<double> sqrtOfMeasurementB = measurementB.SquareRoot();
            PinSiteData<double> measurementBTruncated = measurementB.Truncate();

            // Unary Operations (added in 24.5.1): Max, Min, Mean
            SiteData<double> maxAcrossPinsForEachSite = difference.Max();
            SiteData<double> minAcrossPinsForEachSite = difference.Min();
            SiteData<double> meanAcrossPinsForEachSite = difference.Mean();

            // Unary Operations with greater granularity (added in 24.5.1): MaxByPin, MaxBySite, MinByPin, MinBySite, MeanBySite
            Dictionary<int, (double, string[])> maxAcrossPinsWithWhichPinsForEachSite = difference.MaxByPin();
            Dictionary<string, (double, int[])> maxAcrossSitesWithWhichSitesForEachPin = difference.MaxBySite();
            Dictionary<int, (double, string[])> minAcrossPinsWithWhichPinsForEachSite = difference.MinByPin();
            Dictionary<string, (double, int[])> minAcrossSitesWithWhichSitesForEachPin = difference.MinBySite();
            Dictionary<string, double> meanAcrossSitesForEachPin = difference.MeanBySite();
        }

        // These operations were added in the 24.5.1 release.
        internal static void PinSiteDataBitwiseOperations()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0, 1 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // Setup example dual-site data values
            PinSiteData<byte> data1 = new PinSiteData<byte>(pinNames, siteNumbers, 0b_0001);
            PinSiteData<byte> data2 = new PinSiteData<byte>(pinNames, siteNumbers, 0b_0010);

            // Shift Left Operations
            PinSiteData<byte> data1ShiftedLeft = data1.ShiftLeft(2);           // new value for all pins and sites: 0b_0100
            PinSiteData<byte> data2ShiftedLeftWithOperator = data2 << 2;       // new value for all pins and sites: 0b_1000
            // Shift Right Operations
            PinSiteData<byte> data1ShiftedRight = data1.ShiftRight(2);         // new value for all pins and sites: 0b_0000
            PinSiteData<byte> data2ShiftedRightWithOperator = data2 >> 1;      // new value for all pins and sites: 0b_0001
            // Bitwise And Operations (using method and operator, as both scalar and PinSiteData values).
            PinSiteData<byte> data1AndWithData2 = data1.BitwiseAnd(data2);     // new value for all pins and sites: 0b_0000
            PinSiteData<byte> data2AmdWithOperator = data2 & 0b_0010;          // new value for all pins and sites: 0b_0010
            // Bitwise Or Operations (using method and operator, as both scalar and PinSiteData values).
            PinSiteData<byte> data1Or = data1.BitwiseOr(0b_0011);              // new value for all pins and sites: 0b_0011
            PinSiteData<byte> data1OrWithData2UsingOperator = data1 | data2;   // new value for all pins and sites: 0b_0011
            // Bitwise XOr Operations (using method and operator, as both scalar and PinSiteData values).
            PinSiteData<byte> data1XorWithData2 = data1.BitwiseXor(data2);     // new value for all pins and sites: 0b_0011
            PinSiteData<byte> data1XorWithOperator = data1 ^ 0b_0011;          // new value for all pins and sites: 0b_0010
            // Bitwise Compliment Operations (using method and operator).
            PinSiteData<byte> data1Complement = data1.BitwiseComplement();     // new value for all pins and sites: 0b_1111_1110
            PinSiteData<byte> data1ComplementWithOperator = ~data2;            // new value for all pins and sites: 0b_1111_1101
        }

        internal static void PinSiteDataConvertIntToDouble()
        {
            // Establish new PinSiteData object with integer values.
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0, 1 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1", "VCC2" };
            // Constructs a PinSiteData object with the same data value across all pins and sites.
            var integerPinSiteData = new PinSiteData<int>(pinNames, siteNumbers, 256);

            // Use select function to operate on each site value to cast it to a double.
            // Note the select function was introduced in the 24.5.1 release.
            PinSiteData<double> doublePinSiteData = integerPinSiteData.Select(value => (double)value);
        }

        internal static void PinSiteDataTransformUintArrayToHexString()
        {
            // Establish new PinSiteData object of signed integer arrays for two sties.
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0, 1, };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1", "VCC2" };
            // Constructs a PinSiteData object with the same data value across all pins and sites.
            var pinSiteData = new PinSiteData<uint[]>(pinNames, siteNumbers, new uint[] { 128, 32, 64 });

            // Use select function to operate on each site value to cast it to a double.
            // Note the select function was introduced in the 24.5.1 release.
            // The expected resulting string value would be: e0
            PinSiteData<string> doubleData = pinSiteData.Select(SumSamplesAndRepresentAsHexString);

            // Local method to define the data transformation
            string SumSamplesAndRepresentAsHexString(uint[] arrayOfSamples)
            {
                var wholeSample = arrayOfSamples.Sum(x => x);
                return $"{wholeSample:X}";
            }
        }

        internal static void PinSiteDataMethods()
        {
            PinSiteData<double> measurementsForPinsABandC = Measure();

            int[] sitesMeasured = measurementsForPinsABandC.SiteNumbers;
            string[] pinsMeasured = measurementsForPinsABandC.PinNames;
            double site0ValueForPinA = measurementsForPinsABandC.GetValue(siteNumber: 0, pinName: "PinA");
            IDictionary<string, double> site0ValuesForAllPins = measurementsForPinsABandC.ExtractSite(siteNumber: 0);
            SiteData<double> justPinAMeasurments = measurementsForPinsABandC.ExtractPin(pinName: "PinA");
            string[] pinNamesToExtract = new string[] { "PinA", "PinB" };
            PinSiteData<double> subsetOfPinAandPinBMeasurments = measurementsForPinsABandC.ExtractPins(pinNamesToExtract);
        }

        internal static void ConstructWithArrays()
        {
            // Pin names to associate with the data.
            var pinNames = new string[] { "VCC1", "VCC2" };
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0 };
            // Per-pin SiteData objects.
            var perPinSiteData = new[]
            {
                new SiteData<double>(siteNumbers, 1.5),
                new SiteData<double>(siteNumbers, 2.5)
            };
            // Constructs a PinSiteData object with pin names and associated SiteData object array.
            var pinSiteData = new PinSiteData<double>(pinNames, perPinSiteData);
        }

        internal static void ConstructWithArraysWithSystemPin()
        {
            // Pin names to associate with the data.
            var pinNames = new string[] { "VCC1", "VCC2", "SystemSupply" };
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 0, 1 };
            // Per-pin SiteData objects.
            // Note that data associated with system pins is considered site-agnostic,
            // and site-agnostic data can represented with -1 as the site value.
            var perPinSiteData = new[]
            {
                new SiteData<double>(siteNumbers, 1.5),
                new SiteData<double>(siteNumbers, 2.5),
                new SiteData<double>(new[] { -1 }, -22.5)
            };
            // Constructs a PinSiteData object with pin names and associated SiteData object array,
            // inclusive of system pin data.
            var pinSiteData = new PinSiteData<double>(pinNames, perPinSiteData);
        }

        internal static void ConstructWithDictionaryWithSystemPin()
        {
            // Dictionary containing pin- and site-unique data, including system pin data.
            // Note that data associated with system pins is considered site-agnostic,
            // and site-agnostic data can represented with -1 as the site value.
            var pinAndSiteUnqiueDataDictionary = new Dictionary<string, IDictionary<int, double>>
            {
                ["VCC1"] = new Dictionary<int, double> { [0] = 1.5, [1] = 11.5 },
                ["SystemSupply"] = new Dictionary<int, double> { [-1] = -22.5 }
            };
            // Constructs a PinSiteData object with pin and site unique data dictionary,
            // inclusive of system pin data.
            var pinSiteData = new PinSiteData<double>(pinAndSiteUnqiueDataDictionary);
        }

        internal static void ConstructWithPinDataDictionaryAndSiteNumbersArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3, 1 };
            // Dictionary containing pin-unique data.
            var perPinData = new Dictionary<string, double> { ["VDET"] = 22, ["VCC1"] = 44, ["VCC2"] = 33 };
            // Constructs a PinSiteData object with a pin specific data dictionary and siteNumbers array.
            var pinSiteData = new PinSiteData<double>(siteNumbers, perPinData);
        }

        internal static void ConstructWithPinDataDictionaryAndSiteNumbersArrayWithSystemPin()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3, 1 };
            // Dictionaries containing pin-unique data.
            // Two separate dictionaries are used to delineate between DUT pins from System pin data.
            var perDutPinData = new Dictionary<string, double> { ["VDET"] = 22, ["VCC1"] = 44, ["VCC2"] = 33 };
            var perSystemPinSiteData = new Dictionary<string, double> { ["SystemSupply"] = -15 };
            // First, construct a PinSiteData object with the DUT pin specific data dictionary and the siteNumbers array.
            // Then, combine it with a new PinSiteData object constructed for the system pin specific data dictionary,
            // where the siteNumbers input is an array containing a single element value of -1.
            // Note that data associated with system pins is considered site-agnostic,
            // and site-agnostic data can represented with -1 as the site value.
            var pinSiteData = new PinSiteData<double>(siteNumbers, perDutPinData)
                .Combine(new PinSiteData<double>(new[] { -1 }, perSystemPinSiteData));
        }

        internal static void ConstructWithArraysForCommonDataValue()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1", "VCC2" };
            // Constructs a PinSiteData object with the same data value across all pins and sites.
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers, 55);
        }

        internal static void ConstructWithPinUniqueDataArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // Per-pin data values.
            var perPinData = new double[] { 42, 105 };
            // Constructs a PinSiteData object with pin unique data.
            // Where the specified pin unique data will be repeated across all sites.
            // The lengths of the pinNames and perPinData inputs must be equal,
            // and both the pinNames and siteNumbers arrays must each contain unique values,
            // otherwise an exception will be thrown.
            // Non-sequential site order is accepted.
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers, perPinData);
        }

        internal static void ConstructWithSiteUniqueDataArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // Per-site data values.
            var perSiteData = new double[] { 42, 105, 55 };
            // Constructs a PinSiteData object with site unique data.
            // Where the specified site unique data will be repeated for all pins.
            // The lengths of the siteNumbers and perSiteData inputs must be equal,
            // and both the pinNames and siteNumbers arrays must each contain unique values,
            // otherwise an exception will be thrown.
            // Non-sequential site order is accepted.
            var pinSiteData = new PinSiteData<double>(siteNumbers, pinNames, perSiteData);
        }

        internal static void ConstructWithPinAndSiteUniqueDataArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Pin names to associate with the data.
            var pinNames = new string[] { "VDET", "VCC1" };
            // 2D jagged array of pin and site unique data,
            // where the first dimension represents pins (2) and the second dimension represents sites (3).
            var perPinPerSiteData = new double[][]
            {
                new double[] { 42, 105, 206 },
                new double[] { 55, 2048, 0.5 }
            };
            // Constructs a PinSiteData object with pin and site unique data.
            // Where the specified data value is unique for each pin and each site.
            // The length of pinNames must be equal to the length of the first dimension of perPinPerSiteData.
            // Similarly, the length of siteNumbers must be equal to the length of the second dimension of perPinPerSiteData.
            // Additionally, both the pinNames and siteNumbers arrays must each contain unique values.
            // If any of the above conditions are not met, an exception will be thrown.
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers, perPinPerSiteData);
        }

        internal static void ConstructWithSiteAndPinUniqueDataArray()
        {
            var siteNumbers = new int[] { 2, 4, 3 };
            var pinNames = new string[] { "VDET", "VCC1" };
            // 2D jagged array of pin and site unique data,
            // where the first dimension represents sites (3) and the second dimension represents pins (2).
            var perSitePerPinData = new double[][]
            {
                new double[] { 42,  55 },
                new double[] { 105, 2048 },
                new double[] { 206, 0.5 },
            };
            // Constructs a PinSiteData object with site and pin unique data.
            // Where the specified data value is unique for each site and each pin.
            // The length of siteNumbers must be equal to the length of the first dimension of perSitePerPinData.
            // Similarly, the length of pinNames must be equal to the length of the second dimension of perSitePerPinData.
            // Additionally, both the pinNames and siteNumbers arrays must each contain unique values.
            // If any of the above conditions are not met, an exception will be thrown.
            // Non-sequential site order is accepted
            var pinSiteData = new PinSiteData<double>(siteNumbers, pinNames, perSitePerPinData);
        }

        /// <summary>
        /// This method is just for example purposes to simulate a measurement result being collected.
        /// </summary>
        /// <returns>Simulated random measurement result.</returns>
        internal static PinSiteData<double> Measure()
        {
            var siteDataArray = new SiteData<double>[PinNames.Length];
            for (int i = 0; i < PinNames.Length; i++)
            {
                siteDataArray[i] = new SiteData<double>(GenerateRandomPerSiteData());
            }

            return new PinSiteData<double>(PinNames, siteDataArray);
        }
        private static double[] GenerateRandomPerSiteData()
        {
            return Enumerable.Range(0, SiteCount).Select(x => x * RandomNumber.NextDouble()).ToArray();
        }
    }
}
