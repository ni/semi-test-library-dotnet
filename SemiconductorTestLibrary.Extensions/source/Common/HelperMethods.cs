using System;
using System.Collections.Generic;
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
        public static double[] CreateRampSequence(double outputStart, double outputStop, double numberOfPoints)
        {
            double stepSize = 0.0;

            if (numberOfPoints > 1)
            {
                stepSize = (outputStop - outputStart) / (numberOfPoints - 1);
            }
            double[] rampSequence = new double[(int)numberOfPoints];
            for (int i = 0; i < numberOfPoints; i++)
            {
                rampSequence[i] = outputStart + (i * (double)stepSize);
            }
            return rampSequence;
        }

        /// <summary>
        /// Creates a ramp sequence and wraps it in a SiteData object for the specified site numbers.
        /// </summary>
        /// <param name="outputStart">The starting value of the ramp.</param>
        /// <param name="outputStop">The ending value of the ramp.</param>
        /// <param name="numberOfPoints">The number of points in the ramp sequence.</param>
        /// <param name="siteNumbers">The site numbers to associate with the ramp sequence.</param>
        /// <returns>A SiteData object containing the ramp sequence for the specified sites.</returns>
        public static SiteData<double[]> CreateRampSequence(double outputStart, double outputStop, int numberOfPoints, int[] siteNumbers)
        {
            return new SiteData<double[]>(siteNumbers, CreateRampSequence(outputStart, outputStop, numberOfPoints));
        }

        /// <summary>
        /// Creates a ramp sequence and wraps it in a PinSiteData object for the specified pin names and site numbers.
        /// </summary>
        /// <param name="outputStart">The starting value of the ramp.</param>
        /// <param name="outputStop">The ending value of the ramp.</param>
        /// <param name="numberOfPoints">The number of points in the ramp sequence.</param>
        /// <param name="pinNames">The pin names to associate with the ramp sequence.</param>
        /// <param name="siteNumbers">The site numbers to associate with the ramp sequence.</param>
        /// <returns>A PinSiteData object containing the ramp sequence for the specified pins and sites.</returns>
        public static PinSiteData<double[]> CreateRampSequence(double outputStart, double outputStop, int numberOfPoints, string[] pinNames, int[] siteNumbers)
        {
            return new PinSiteData<double[]>(pinNames, siteNumbers, CreateRampSequence(outputStart, outputStop, numberOfPoints));
        }
    }
}
