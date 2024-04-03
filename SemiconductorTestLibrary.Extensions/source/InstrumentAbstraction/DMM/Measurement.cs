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
        /// <returns>The measurement results in per-instrument format.</returns>
        public static double[] FetchAndPublish(this DMMSessionsBundle sessionsBundle, double maximumTimeInMilliseconds)
        {
            return sessionsBundle.DoAndPublishResults(sessionInfo =>
            {
                return sessionInfo.Session.Measurement.Fetch(PrecisionTimeSpan.FromMilliseconds(maximumTimeInMilliseconds));
            });
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
        /// Fetches the specified number of values from previously initiated measurements.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="numberToRead">The number of values to fetch.</param>
        /// <param name="maximumTimeInMilliseconds">The maximum time for the fetch to complete in milliseconds.</param>
        /// <returns>The measurement results in per-instrument per-point format.</returns>
        public static double[][] FetchMultiPoint(this DMMSessionsBundle sessionsBundle, int numberToRead, double maximumTimeInMilliseconds)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Measurement.FetchMultiPoint(PrecisionTimeSpan.FromMilliseconds(maximumTimeInMilliseconds), numberToRead);
            });
        }

        /// <summary>
        /// Reads measurement results and publishes measurement results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DMMSessionsBundle"/> object.</param>
        /// <param name="maximumTimeInMilliseconds">The maximum time for the fetch to complete in milliseconds.</param>
        /// <returns>The measurement results in per-instrument format.</returns>
        public static double[] ReadAndPublish(this DMMSessionsBundle sessionsBundle, double maximumTimeInMilliseconds)
        {
            return sessionsBundle.DoAndPublishResults(sessionInfo =>
            {
                return sessionInfo.Session.Measurement.Read(PrecisionTimeSpan.FromMilliseconds(maximumTimeInMilliseconds));
            });
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
        /// <param name="numberToRead">The number of values to fetch.</param>
        /// <param name="maximumTimeInMilliseconds">The maximum time for the fetch to complete in milliseconds.</param>
        /// <returns>The measurement results in per-instrument per-value format.</returns>
        public static double[][] ReadMultiPoint(this DMMSessionsBundle sessionsBundle, int numberToRead, double maximumTimeInMilliseconds)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Measurement.ReadMultiPoint(PrecisionTimeSpan.FromMilliseconds(maximumTimeInMilliseconds), numberToRead);
            });
        }
    }
}
