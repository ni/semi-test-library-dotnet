using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.Common
{
    /// <summary>
    /// Provides helper methods.
    /// </summary>
    public static class HelperMethods
    {
        internal static T GetDistinctValue<T>(IEnumerable<T> values, string errorMessage)
        {
            try
            {
                return values.Distinct().Single();
            }
            catch (InvalidOperationException)
            {
                throw new NISemiconductorTestException(errorMessage);
            }
        }

        /// <summary>
        /// Creates a ramp sequence of double values from outputStart to outputStop with the specified number of points.
        /// </summary>
        /// <param name="outputStart">The starting value of the ramp.</param>
        /// <param name="outputStop">The ending value of the ramp.</param>
        /// <param name="numberOfPoints">The number of points in the ramp sequence.</param>
        /// <returns>An array of double values representing the ramp sequence.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when numberOfPoints is less than or equal to zero.</exception>
        /// <exception cref="NISemiconductorTestException">Thrown when outputStart is NaN or Infinity.</exception>
        /// <exception cref="NISemiconductorTestException">Thrown when outputStop is NaN or Infinity.</exception>
        public static double[] CreateRampSequence(double outputStart, double outputStop, int numberOfPoints)
        {
            if (numberOfPoints <= 1)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.CreateRamp_InvalidNumberOfPoints));
            }
            if (double.IsNaN(outputStart) || double.IsInfinity(outputStart))
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.CreateRamp_InvalidOutputStart));
            }
            if (double.IsNaN(outputStop) || double.IsInfinity(outputStop))
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.CreateRamp_InvalidOutputStop));
            }

            double[] rampSequence = new double[numberOfPoints];

            double stepSize = (outputStop - outputStart) / (numberOfPoints - 1);
            for (int i = 0; i < numberOfPoints - 1; i++)
            {
                rampSequence[i] = outputStart + (i * stepSize);
            }
            rampSequence[numberOfPoints - 1] = outputStop;

            return rampSequence;
        }

        /// <summary>
        /// Creates a ramp sequence and wraps it in a SiteData object for the specified site numbers.
        /// </summary>
        /// <param name="siteNumbers">The site numbers to associate with a ramp sequence.</param>
        /// <param name="outputStart">The starting value of the ramp.</param>
        /// <param name="outputStop">The ending value of the ramp.</param>
        /// <param name="numberOfPoints">The number of points in the ramp sequence.</param>
        /// <returns>A SiteData object containing the ramp sequence for the specified sites.</returns>
        public static SiteData<double[]> CreateRampSequence(int[] siteNumbers, double outputStart, double outputStop, int numberOfPoints)
        {
            return new SiteData<double[]>(siteNumbers, CreateRampSequence(outputStart, outputStop, numberOfPoints));
        }

        /// <summary>
        /// Creates a ramp sequence and wraps it in a PinSiteData object for the specified pin names and site numbers.
        /// </summary>
        /// <param name="pinNames">The pin names to associate with a ramp sequence.</param>
        /// <param name="siteNumbers">The site numbers to associate with a ramp sequence.</param>
        /// <param name="outputStart">The starting value of the ramp.</param>
        /// <param name="outputStop">The ending value of the ramp.</param>
        /// <param name="numberOfPoints">The number of points in the ramp sequence.</param>
        /// <returns>A PinSiteData object containing the ramp sequence for the specified pins and sites.</returns>
        public static PinSiteData<double[]> CreateRampSequence(string[] pinNames, int[] siteNumbers, double outputStart, double outputStop, int numberOfPoints)
        {
            return new PinSiteData<double[]>(pinNames, siteNumbers, CreateRampSequence(outputStart, outputStop, numberOfPoints));
        }

        /// <summary>
        /// Creates a unique ramp sequence for each site using per-site input values.
        /// </summary>
        /// <param name="siteNumbers">The site numbers to associate with a ramp sequence.</param>
        /// <param name="outputStart">Array of starting values for each ramp sequence.</param>
        /// <param name="outputStop">Array of ending values for each ramp sequence.</param>
        /// <param name="numberOfPoints">Array specifying the number of points in each ramp sequence.</param>
        /// <returns>A SiteData object containing the ramp sequences for the specified sites.</returns>
        public static SiteData<double[]> CreateRampSequence(int[] siteNumbers, double[] outputStart, double[] outputStop, int[] numberOfPoints)
        {
            var perSiteSequences = new double[siteNumbers.Length][];
            for (int i = 0; i < siteNumbers.Length; i++)
            {
                perSiteSequences[i] = CreateRampSequence(outputStart[i], outputStop[i], numberOfPoints[i]);
            }
            return new SiteData<double[]>(siteNumbers, perSiteSequences);
        }

        /// <summary>
        /// Creates a unique ramp sequence for each pin using per-pin input values.
        /// For a given pin, the generated sequence will be same across all sites.
        /// </summary>
        /// <param name="pinNames">The pin names to associate with a ramp sequence.</param>
        /// <param name="siteNumbers">The site numbers to associate with a ramp sequence.</param>
        /// <param name="outputStart">Array of starting values for each ramp sequence.</param>
        /// <param name="outputStop">Array of ending values for each ramp sequence.</param>
        /// <param name="numberOfPoints">Array specifying the number of points in each ramp sequence.</param>
        /// <returns>A PinSiteData object containing the generated ramp sequences for each pin-site combination.</returns>
        public static PinSiteData<double[]> CreateRampSequence(string[] pinNames, int[] siteNumbers, double[] outputStart, double[] outputStop, int[] numberOfPoints)
        {
            var perPinSiteSequences = new double[pinNames.Length][];
            for (int i = 0; i < pinNames.Length; i++)
            {
                perPinSiteSequences[i] = CreateRampSequence(outputStart[i], outputStop[i], numberOfPoints[i]);
            }
            return new PinSiteData<double[]>(pinNames, siteNumbers, perPinSiteSequences);
        }

        /// <summary>
        /// Creates a unique ramp sequence for each pin and each site using per-pin, per-site input values.
        /// </summary>
        /// <param name="pinNames">The pin names to associate with a ramp sequence.</param>
        /// <param name="siteNumbers">The site numbers to associate with a ramp sequence.</param>
        /// <param name="outputStart">
        /// 2D jagged array of  starting values, one value for each pin and site-unique ramp sequence,
        /// where the first dimension represents pins and the second dimension represents sites.
        /// </param>
        /// <param name="outputStop">
        /// 2D jagged array of ending values, one value for each pin and site-unique ramp sequence,
        /// where the first dimension represents pins and the second dimension represents sites.
        /// </param>
        /// <param name="numberOfPoints">
        /// 2D jagged array specifying the number of points for each pin and site-unique ramp sequence,
        /// where the first dimension represents pins and the second dimension represents sites.
        /// </param>
        /// <returns>A PinSiteData object containing the generated ramp sequences for each pin-site combination.</returns>
        public static PinSiteData<double[]> CreateRampSequence(string[] pinNames, int[] siteNumbers, double[][] outputStart, double[][] outputStop, int[][] numberOfPoints)
        {
            var perPinSiteData = new SiteData<double[]>[pinNames.Length];
            for (int pinIndex = 0; pinIndex < pinNames.Length; pinIndex++)
            {
                perPinSiteData[pinIndex] = CreateRampSequence(siteNumbers, outputStart[pinIndex], outputStop[pinIndex], numberOfPoints[pinIndex]);
            }
            return new PinSiteData<double[]>(pinNames, perPinSiteData);
        }

        internal static string ExcludeSpecificChannel(this string channelString, string channelToExclude)
        {
            return string.Join(",", channelString.Split(',').Where(s => !s.Contains($"/{channelToExclude}")));
        }
    }
}
