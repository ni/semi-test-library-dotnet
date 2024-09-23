using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM
{
    /// <summary>
    /// Defines methods for data acquisition.
    /// </summary>
    public static class Measurement
    {
        /// <summary>
        /// Initiates an acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        public static void Initiate(this DMMSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Measurement.Initiate();
            });
        }

        /// <summary>
        /// Aborts an acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        public static void Abort(this DMMSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Measurement.Abort();
            });
        }

        /// <summary>
        /// Fetches the values from previously initiated measurements and publishes measurement results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="maximumTimeInMilliseconds">The maximum time for the fetch to complete in milliseconds.</param>
        /// <param name="publishedDataId">The unique data id to be used when publishing.</param>
        /// <returns>The measurement results in per-instrument format.</returns>
        public static double[] FetchAndPublish(this DMMSessionsBundle sessionsBundle, double maximumTimeInMilliseconds, string publishedDataId = "")
        {
            return sessionsBundle.DoAndPublishResults(
                sessionInfo =>
                {
                    return sessionInfo.Session.Measurement.Fetch(PrecisionTimeSpan.FromMilliseconds(maximumTimeInMilliseconds));
                },
                publishedDataId);
        }

        /// <summary>
        /// Fetches the values from previously initiated measurements.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="maximumTimeInMilliseconds">The maximum time for the fetch to complete in milliseconds.</param>
        /// <returns>The measurement results in per-site per-pin format.</returns>
        public static PinSiteData<double> Fetch(this DMMSessionsBundle sessionsBundle, double maximumTimeInMilliseconds)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                return new double[] { sessionInfo.Session.Measurement.Fetch(PrecisionTimeSpan.FromMilliseconds(maximumTimeInMilliseconds)) };
            });
        }

        /// <summary>
        /// Fetches multiple points from previously initiated multipoint acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="numberOfPoints">The number of points to fetch.</param>
        /// <param name="maximumTimeInMilliseconds">The maximum time for the fetch to complete in milliseconds.</param>
        /// <returns>The measurement results in per-site per-pin format.</returns>
        public static PinSiteData<double[]> FetchMultiPoint(this DMMSessionsBundle sessionsBundle, int numberOfPoints, double maximumTimeInMilliseconds)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                return new double[][] { sessionInfo.Session.Measurement.FetchMultiPoint(PrecisionTimeSpan.FromMilliseconds(maximumTimeInMilliseconds), numberOfPoints) };
            });
        }

        /// <summary>
        /// Reads measurement results and publishes measurement results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="maximumTimeInMilliseconds">The maximum time for the fetch to complete in milliseconds.</param>
        /// <param name="publishedDataId">The unique data id to use when publishing.</param>
        /// <returns>The measurement results in per-instrument format.</returns>
        public static double[] ReadAndPublish(this DMMSessionsBundle sessionsBundle, double maximumTimeInMilliseconds, string publishedDataId = "")
        {
            return sessionsBundle.DoAndPublishResults(
                sessionInfo =>
                {
                    return sessionInfo.Session.Measurement.Read(PrecisionTimeSpan.FromMilliseconds(maximumTimeInMilliseconds));
                },
                publishedDataId);
        }

        /// <summary>
        /// Reads measurement results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="maximumTimeInMilliseconds">The maximum time for the fetch to complete in milliseconds.</param>
        /// <returns>The measurement results in per-site per-pin format.</returns>
        public static PinSiteData<double> Read(this DMMSessionsBundle sessionsBundle, double maximumTimeInMilliseconds)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                return new double[] { sessionInfo.Session.Measurement.Read(PrecisionTimeSpan.FromMilliseconds(maximumTimeInMilliseconds)) };
            });
        }

        /// <summary>
        /// Reads specified number of measurement results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="numberOfPoints">The number of points to read.</param>
        /// <param name="maximumTimeInMilliseconds">The maximum time for the fetch to complete in milliseconds.</param>
        /// <returns>The measurement results in per-site per-pin format.</returns>
        public static PinSiteData<double[]> ReadMultiPoint(this DMMSessionsBundle sessionsBundle, int numberOfPoints, double maximumTimeInMilliseconds)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                return new double[][] { sessionInfo.Session.Measurement.ReadMultiPoint(PrecisionTimeSpan.FromMilliseconds(maximumTimeInMilliseconds), numberOfPoints) };
            });
        }
    }
}
