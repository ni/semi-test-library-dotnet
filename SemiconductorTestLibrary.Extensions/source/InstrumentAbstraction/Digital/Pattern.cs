using System;
using System.Linq;
using Ivi.Driver;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods to deal with digital patterns.
    /// </summary>
    public static class Pattern
    {
        /// <summary>
        /// Stops bursting the pattern.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        public static void AbortPattern(this DigitalSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.PatternControl.Abort();
            });
        }

        /// <summary>
        /// Stops the keep alive pattern if it is currently running.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        public static void AbortKeepAlivePattern(this DigitalSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.PatternControl.AbortKeepAlive();
            });
        }

        /// <summary>
        /// Bursts a digital pattern.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="startLabel">The pattern name or exported pattern label from which to start bursting the pattern.</param>
        /// <param name="selectDigitalFunction">Whether to set selected function to digital.</param>
        /// <param name="waitUntilDone">Whether to wait for pattern burst to complete.</param>
        /// <param name="timeoutInSeconds">The maximum time interval allowed for the pattern burst to complete.</param>
        public static void BurstPattern(this DigitalSessionsBundle sessionsBundle, string startLabel, bool selectDigitalFunction = true, bool waitUntilDone = true, double timeoutInSeconds = 5.0)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.PatternControl.BurstPattern(sessionInfo.SiteListString, startLabel, selectDigitalFunction, waitUntilDone, TimeSpan.FromSeconds(timeoutInSeconds));
            });
        }

        /// <summary>
        /// Bursts a digital pattern and publishes pass/fail comparison results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="startLabel">The pattern name or exported pattern label from which to start bursting the pattern.</param>
        /// <param name="selectDigitalFunction">Whether to set selected function to digital.</param>
        /// <param name="timeoutInSeconds">The maximum time interval allowed for the pattern burst to complete.</param>
        /// <param name="publishedDataId">The unique data id to be used when publishing.</param>
        /// <returns>The pass/fail comparison results.</returns>
        public static bool[][] BurstPatternAndPublishResults(this DigitalSessionsBundle sessionsBundle, string startLabel, bool selectDigitalFunction = true, double timeoutInSeconds = 5.0, string publishedDataId = "")
        {
            var results = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.PatternControl.BurstPattern(sessionInfo.SiteListString, startLabel, selectDigitalFunction, TimeSpan.FromSeconds(timeoutInSeconds));
            });
            (sessionsBundle.PinQueryContext as NIDigitalPatternPinQueryContext).PublishPatternResults(results, publishedDataId);
            return results;
        }

        /// <summary>
        /// Starts a pattern burst on digital pattern instruments that you have previously synchronized using NI-TClk.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="startLabel">The pattern name or exported pattern label from which to start bursting the pattern.</param>
        /// <param name="selectDigitalFunction">Whether to set selected function to digital.</param>
        /// <param name="waitUntilDone">Whether to wait for pattern burst to complete.</param>
        /// <param name="timeoutInSeconds">The maximum time interval allowed for the pattern burst to complete.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "Not Necessary")]
        public static void BurstPatternSynchronized(this DigitalSessionsBundle sessionsBundle, string startLabel, bool selectDigitalFunction = true, bool waitUntilDone = true, double timeoutInSeconds = 5.0)
        {
            string siteListString = string.Join(",", sessionsBundle.AggregateSitePinList.Select(sitePinList => sitePinList.SiteNumber.ToString()).ToArray());
            NIDigital[] niDigitalSessions = sessionsBundle.InstrumentSessions.Select(x => x.Session).ToArray();
            DigitalPatternControl.BurstPatternSynchronized(niDigitalSessions, siteListString, startLabel, selectDigitalFunction, waitUntilDone, TimeSpan.FromSeconds(timeoutInSeconds));
        }

        /// <summary>
        /// Gets the per-site pass fail comparison results of last burst pattern (long) as a SiteData object of type Bool.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <returns>The per-site pass/fail results.</returns>
        public static SiteData<bool> GetSitePassFail(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSiteResults(sessionInfo =>
            {
                return sessionInfo.Session.PatternControl.GetSitePassFail(sessionInfo.SiteListString);
            });
        }

        /// <summary>
        /// Gets fail count on a per-pin per-site basis of last burst pattern (long).
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <returns>The per-site per-pin fail count.</returns>
        public static PinSiteData<long> GetFailCount(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                return sessionInfo.PinSet.GetFailCount();
            });
        }

        /// <summary>
        /// Waits until the pattern burst is done. This method is a blocking call, but will timeout after the specified time.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds for which to abort this wait operation.</param>
        /// <exception cref="ArgumentException">The value for maxTime is invalid.</exception>
        /// <exception cref="MaxTimeExceededException"> The pattern burst took longer than the specified maxTime.</exception>
        public static void WaitUntilDone(this DigitalSessionsBundle sessionsBundle, double timeoutInSeconds = 10.0)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.PatternControl.WaitUntilDone(TimeSpan.FromSeconds(timeoutInSeconds));
            });
        }
    }
}
