using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.DataAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Data Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to use the PinStieData objects.
    /// This class, and it's methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// that has already been initiated and configured prior.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class SiteDataExamples
    {
        // Generate Random Values
        private static readonly Random RandomNumber = new Random();
        private static readonly int SiteCount = 4;

        internal static void SiteDataMath()
        {
            SiteData<double> measurementA = PerSiteMeasure();
            SiteData<double> measurementB = PerSiteMeasure();

            // Binary Operations: Add, Compare, Divide, Maximum, Minimum, Multiply, Power, Subtract.
            SiteData<double> measurementBAddOffset = measurementB.Add(5);
            SiteData<bool> measurementsInBGreaterThanFive = measurementB.Compare(ComparisonType.GreaterThan, 5);
            SiteData<double> ratioBOverA = measurementB.Divide(measurementA);
            SiteData<double> maximumAToB = measurementA.Maximum(measurementB);
            SiteData<double> minimumA = measurementA.Minimum(2);
            SiteData<double> measurementAMultipledByTwo = measurementA.Multiply(2);
            SiteData<double> measurementAValuesToPowerOfTwo = measurementA.Power(2);
            SiteData<double> difference = measurementB.Subtract(measurementA);

            // Binary Operations Using Supported Operators (added in 24.5.1): Add, Subtract, Multiply, Divide.
            SiteData<double> measurementBAddmeasurementA = measurementB + measurementA;
            SiteData<double> measurementASubtractOffset = measurementB - 5;
            SiteData<double> measurementBMultipledByTwo = measurementB * 2;
            SiteData<double> ratioAOverB = measurementA / measurementB;

            // Unary Operations: Abs,  Invert, Log10, Negate, SquareRoot, Truncate.
            SiteData<double> absOfDifference = difference.Abs();
            SiteData<double> inverseOfMeasurementA = measurementB.Invert();
            SiteData<double> log10OfMeasurementA = measurementA.Log10();
            SiteData<double> negationOfMeasurementB = measurementB.Negate();
            SiteData<double> sqrtOfMeasurementB = measurementB.SquareRoot();
            SiteData<double> measurementBTruncated = measurementB.Truncate();

            // Unary Operations (added in 24.5.1): Max, Min, Mean
            double maxAcrossSites = difference.Max();
            double minAcrossSites = difference.Min();
            double meanAcrossSites = difference.Mean();

            // Unary Operations with greater granularity (added in 24.5.1): Max, Min
            maxAcrossSites = difference.Max(out int[] sitesWhereMaxValueWasFound);
            minAcrossSites = difference.Min(out int[] sitesWhereMinValueWasFound);
        }

        // These operations were added in the 24.5.1 release.
        internal static void SiteDataBitwiseOperations()
        {
            // Setup example dual-site data values
            SiteData<byte> data1 = new SiteData<byte>(new byte[] { 0b_0001, 0b_0010 });
            SiteData<byte> data2 = new SiteData<byte>(new byte[] { 0b_1111, 0b_0110 });

            // Shift Left Operations
            SiteData<byte> data1ShiftedLeft = data1.ShiftLeft(2);           // new per-site values: 0b_0100, 0b_1000
            SiteData<byte> data1ShiftedLeftWithOperator = data1 << 2;       // new per-site values: 0b_0100, 0b_1000
            // Shift Right Operations
            SiteData<byte> data2ShiftedRight = data2.ShiftRight(1);         // new per-site values: 0b_0111, 0b_0011
            SiteData<byte> data2ShiftedRightWithOperator = data2 >> 1;      // new per-site values: 0b_0111, 0b_0011
            // Bitwise And Operations (using method and operator, as both scalar and SiteData values).
            SiteData<byte> data1AndWithData2 = data1.BitwiseAnd(data2);     // new per-site values: 0b_0001, 0b_0010
            SiteData<byte> data1AmdWithOperator = data1 & 0b_0010;          // new per-site values: 0b_0000, 0b_0010
            // Bitwise Or Operations (using method and operator, as both scalar and SiteData values).
            SiteData<byte> data1Or = data1.BitwiseOr(0b_0010);              // new per-site values: 0b_0011, 0b_0010
            SiteData<byte> data1OrWithData2UsingOperator = data1 | data2;   // new per-site values: 0b_1111, 0b_0110
            // Bitwise XOr Operations (using method and operator, as both scalar and SiteData values).
            SiteData<byte> data1XorWithData2 = data1.BitwiseXor(data2);     // new per-site values: 0b_1110, 0b_0100
            SiteData<byte> data1XorWithOperator = data1 ^ 0b_0010;          // new per-site values: 0b_0011, 0b_0000
            // Bitwise Compliment Operations (using method and operator).
            SiteData<byte> data1Complement = data1.BitwiseComplement();     // new per-site values: 0b_1111_1110, 0b_1111_1101
            SiteData<byte> data1ComplementWithOperator = ~data1;            // new per-site values: 0b_1111_1110, 0b_1111_1101
        }

        internal static void SiteDataConvertIntToDouble()
        {
            // Establish new SiteData object with integer values.
            SiteData<int> intData = new SiteData<int>(new int[] { 1, 2 });
            // Use select function to operate on each site value to cast it to a double.
            // Note the select function was introduced in the 24.5.1 release.
            SiteData<double> doubleData = intData.Select(value => (double)value);
        }

        internal static void SiteDataTransformUintArrayToHexString()
        {
            // Establish new SiteData object of signed integer arrays for two sties.
            SiteData<uint[]> intData = new SiteData<uint[]>(new uint[][]
            {
                new uint[] { 192, 3 }, // Site0 data
                new uint[] { 32, 128 } // Site1 data
            });
            // Use select function to operate on each site value to cast it to a double.
            // Note the select function was introduced in the 24.5.1 release.
            // Site0's resulting string value is expected to be: c3.
            // Site1's resulting string value is expected to be: a0.
            SiteData<string> doubleData = intData.Select(SumSamplesAndRepresentAsHexString);

            // Local method to define the data transformation
            string SumSamplesAndRepresentAsHexString(uint[] arrayOfSamples)
            {
                var wholeSample = arrayOfSamples.Sum(x => x);
                return $"{wholeSample:X}";
            }
        }

        internal static void SiteDataMethods()
        {
            SiteData<double> measurementA = PerSiteMeasure();

            int[] sitesMeasured = measurementA.SiteNumbers;
            double site0Value = measurementA.GetValue(siteNumber: 0);
        }

        internal static void ConstructWithArray()
        {
            // Constructs a SiteData object with a per-site data array.
            // The array index will represent site numbers: 0, 1, 2
            // NOTE This should only be used for niche scenarios,
            // as the active site numbers array may not always start with 0 and be sequential.
            // For example the active sites could be: 4, 9, 12, 32.
            var siteData = new SiteData<double>(new double[] { 1, 2, 3 });
        }
        internal static void ConstructWithPerSiteDataDictionary()
        {
            // Constructs a SiteData object with a dictionary of site unique data values.
            var perSiteDataDictionary = new Dictionary<int, double> { [1] = 11, [2] = 22, [3] = 33 };
            var siteData = new SiteData<double>(perSiteDataDictionary);
        }
        internal static void ConstructWithDictionaryWithSystemData()
        {
            // Constructs a SiteData object with a dictionary of site unique data values,
            // inclusive of a site-agnostic data,such as that associated with a system resource or pin.
            // Note there can only be one site-agnostic value represented in a SiteData object.
            var perSiteDataDictionary = new Dictionary<int, double> { [1] = 11, [2] = 22, [-1] = 33 };
            var siteData = new SiteData<double>(perSiteDataDictionary);
        }
        internal static void ConstructWithSingleValue()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 5, 6 };
            var data = 567;
            // Constructs a SiteData object with the same data value across all sites.
            // Each element in the siteNumbers array represents a unique site number/value.
            // The siteNumbers array must contain unique values, or else an exception will be thrown.
            // Non-sequential site order is accepted.
            var siteData = new SiteData<double>(siteNumbers, data);
        }
        internal static void ConstructWithSiteUniqueDataArray()
        {
            // Site numbers to associate with the data.
            var siteNumbers = new int[] { 2, 4, 3 };
            // Array of site unique data, where each element represents the data for the specific site,
            // located at the same index within the siteNumbers array.
            var perSiteData = new double[] { 22, 44, 33 };
            // Constructs a SiteData object with site unique data.
            // Each element in the siteNumbers array represents a unique site number/value.
            // Each element in the perSiteData array represents the data for the specific site
            // located at the same index within the siteNumbers array.
            // The lengths of the array inputs must be equal,
            // and the siteNumbers array must contain unique values,
            // otherwise an exception will be thrown.
            // Non-sequential site order is accepted.
            var siteData = new SiteData<double>(siteNumbers, perSiteData);
        }

        /// <summary>
        /// This method is just for example purposes to simulate a measurement result being collected.
        /// </summary>
        /// <returns>Simulated random measurement result</returns>
        internal static SiteData<double> PerSiteMeasure()
        {
            return new SiteData<double>(
                Enumerable.Range(0, SiteCount).Select(x => x * RandomNumber.NextDouble()).ToArray());
        }
    }
}
