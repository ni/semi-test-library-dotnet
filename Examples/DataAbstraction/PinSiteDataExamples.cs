using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.DataAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Data Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to use the PinStieData objects.
    /// This class, and it's methods are intended for example purposes only,
    /// and are therefore intentionally marked as internal to prevent them from be directly invoked from code outside of this project.
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

            // Unary Operations: Abs,  Invert, Log10, Negate, SquareRoot, Truncate.
            PinSiteData<double> absOfDifference = difference.Abs();
            PinSiteData<double> inverseOfMeasurementA = measurementB.Invert();
            PinSiteData<double> log10OfMeasurementA = measurementA.Log10();
            PinSiteData<double> negationOfMeasurementB = measurementB.Negate();
            PinSiteData<double> sqrtOfMeasurementB = measurementB.SquareRoot();
            PinSiteData<double> measurementBTruncated = measurementB.Truncate();
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

        private static double[] GenerateRandomPerSiteData()
        {
            return Enumerable.Range(0, SiteCount).Select(x => x * RandomNumber.NextDouble()).ToArray();
        }

        private static PinSiteData<double> Measure()
        {
            var siteDataArray = new SiteData<double>[PinNames.Length];
            for (int i = 0; i < PinNames.Length; i++)
            {
                siteDataArray[i] = new SiteData<double>(GenerateRandomPerSiteData());
            }

            return new PinSiteData<double>(PinNames, siteDataArray);
        }
    }
}
