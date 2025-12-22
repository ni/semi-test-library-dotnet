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
        /// <exception cref="ArgumentException">Thrown when numberOfPoints is less than or equal to zero.</exception>
        /// <exception cref="ArgumentException">Thrown when outputStart is NaN or Infinity.</exception>
        /// <exception cref="ArgumentException">Thrown when outputStop is NaN or Infinity.</exception>
        public static double[] CreateRampSequence(double outputStart, double outputStop, int numberOfPoints)
        {
            if (numberOfPoints <= 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.InvalidNumberOfPoints));
            }
            if (double.IsNaN(outputStart) || double.IsInfinity(outputStart))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.InvalidOutPutStart));
            }
            if (double.IsNaN(outputStop) || double.IsInfinity(outputStop))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.InvalidOutputStop));
            }

            double[] rampSequence = new double[numberOfPoints];

            if (numberOfPoints == 1)
            {
                rampSequence[0] = outputStart;
                return rampSequence;
            }

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
        /// <param name="siteNumbers">The site numbers to associate with the ramp sequence.</param>
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
        /// <param name="pinNames">The pin names to associate with the ramp sequence.</param>
        /// <param name="siteNumbers">The site numbers to associate with the ramp sequence.</param>
        /// <param name="outputStart">The starting value of the ramp.</param>
        /// <param name="outputStop">The ending value of the ramp.</param>
        /// <param name="numberOfPoints">The number of points in the ramp sequence.</param>
        /// <returns>A PinSiteData object containing the ramp sequence for the specified pins and sites.</returns>
        public static PinSiteData<double[]> CreateRampSequence(string[] pinNames, int[] siteNumbers, double outputStart, double outputStop, int numberOfPoints)
        {
            return new PinSiteData<double[]>(pinNames, siteNumbers, CreateRampSequence(outputStart, outputStop, numberOfPoints));
        }
    }
}
