using System;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.DataAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Data Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to use the PinStieData objects.
    /// This class, and it's methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// that has already been initiated and configured prior.
    /// Additionally, they are intentionally marked as internal to prevent them from be directly invoked from code outside of this project.
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
            SiteData<double> ratio = measurementB.Divide(measurementA);
            SiteData<double> maximumAToB = measurementA.Maximum(measurementB);
            SiteData<double> minimumA = measurementA.Minimum(2);
            SiteData<double> measurementAMultipledByTwo = measurementA.Multiply(2);
            SiteData<double> measurementAValuesToPowerOfTwo = measurementA.Power(2);
            SiteData<double> difference = measurementB.Subtract(measurementA);

            // Unary Operations: Abs,  Invert, Log10, Negate, SquareRoot, Truncate.
            SiteData<double> absOfDifference = difference.Abs();
            SiteData<double> inverseOfMeasurementA = measurementB.Invert();
            SiteData<double> log10OfMeasurementA = measurementA.Log10();
            SiteData<double> negationOfMeasurementB = measurementB.Negate();
            SiteData<double> sqrtOfMeasurementB = measurementB.SquareRoot();
            SiteData<double> measurementBTruncated = measurementB.Truncate();
        }

        internal static void SiteDataMethods()
        {
            SiteData<double> measurementA = PerSiteMeasure();

            int[] sitesMeasured = measurementA.SiteNumbers;
            double site0Value = measurementA.GetValue(siteNumber: 0);
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
