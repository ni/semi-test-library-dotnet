using System;
using System.Collections.Generic;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods to deal with digital patterns.
    /// </summary>
    public static class Pattern
    {
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
        /// Gets per-site pass fail comparison results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <returns>The per-site pass/fail results.</returns>
        public static IDictionary<int, bool> GetSitePassFail(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.Do(sessionInfo =>
            {
                return sessionInfo.Session.PatternControl.GetSitePassFail(sessionInfo.SiteListString);
            });
        }

        /// <summary>
        /// Gets fail count.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <returns>The per-site per-pin fail count.</returns>
        public static PinSiteData<long> GetFailCountAndReturnPerSitePerPinResults(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                return sessionInfo.PinSet.GetFailCount();
            });
        }
    }
}
