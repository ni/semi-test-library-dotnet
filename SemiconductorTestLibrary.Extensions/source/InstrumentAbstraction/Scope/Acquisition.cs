using System;
using System.Globalization;
using System.Linq;
using Ivi.Driver;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for acquiring data.
    /// </summary>
    public static class Acquisition
    {
        /// <summary>
        /// Configures the acquisition type for all sessions in the <see cref="ScopeSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="acquisitionType">The <see cref="ScopeAcquisitionType"/> to configure.</param>
        public static void ConfigureAcquisitionType(this ScopeSessionsBundle sessionsBundle, ScopeAcquisitionType acquisitionType)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Acquisition.Type = acquisitionType;
            });
        }

        /// <summary>
        /// Initiates an acquisition, waits for it to complete, and returns the acquired waveform for each site and pin in the <see cref="ScopeSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="timeout">The time to wait for data to be acquired. Specify 0 to fetch whatever samples are currently available without waiting, or -1 to wait indefinitely for the acquisition to complete.</param>
        /// <param name="numberOfSamples">The maximum number of samples to fetch for each waveform. Specify -1 to retrieve all samples currently available in the record.</param>
        /// <returns>Per-site per-pin analog waveform results.</returns>
        /// <remarks>
        /// If the acquisition does not complete within <paramref name="timeout"/>, the resulting <see cref="NISemiconductorTestException"/>
        /// includes the site, pin, and channel details for every channel on the affected card, since a single site losing its trigger
        /// signal prevents the entire card from acquiring data.
        /// A <paramref name="numberOfSamples"/> value of 0, or a negative value other than -1, is not supported by the
        /// underlying driver, which throws an <see cref="Ivi.Driver.IviCDriverException"/> that is caught and wrapped
        /// in an <see cref="NISemiconductorTestException"/>.
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">Thrown when the acquisition does not complete within <paramref name="timeout"/>, or when an invalid <paramref name="numberOfSamples"/> is specified.</exception>
        public static PinSiteData<AnalogWaveform<double>> ReadWaveform(this ScopeSessionsBundle sessionsBundle, PrecisionTimeSpan timeout, long numberOfSamples)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                return DoWithTimeoutHandling(sessionInfo, () => sessionInfo.Session.Channels[sessionInfo.AllChannelsString].Measurement.Read(timeout, numberOfSamples, null).ToArray());
            });
        }

        /// <summary>
        /// Initiates an acquisition, waits for it to complete, and returns the acquired raw samples for each site and pin in the <see cref="ScopeSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="timeout">The time to wait for data to be acquired. Specify 0 to fetch whatever samples are currently available without waiting, or -1 to wait indefinitely for the acquisition to complete.</param>
        /// <param name="numberOfSamples">The maximum number of samples to acquire for each waveform. Specify -1 to retrieve all samples currently available in the record.</param>
        /// <returns>Per-site per-pin floating-point samples.</returns>
        /// <remarks>
        /// If the acquisition does not complete within <paramref name="timeout"/>, the resulting <see cref="NISemiconductorTestException"/>
        /// includes the site, pin, and channel details for every channel on the affected card, since a single site losing its trigger
        /// signal prevents the entire card from acquiring data.
        /// A <paramref name="numberOfSamples"/> value of 0, or a negative value other than -1, is not supported by the
        /// underlying driver, which throws an <see cref="Ivi.Driver.IviCDriverException"/> that is caught and wrapped
        /// in an <see cref="NISemiconductorTestException"/>.
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">Thrown when the acquisition does not complete within <paramref name="timeout"/>, or when an invalid <paramref name="numberOfSamples"/> is specified.</exception>
        public static PinSiteData<double[]> Read(this ScopeSessionsBundle sessionsBundle, PrecisionTimeSpan timeout, long numberOfSamples)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                return DoWithTimeoutHandling(sessionInfo, () => sessionInfo.Session.Channels[sessionInfo.AllChannelsString].Measurement.Read(timeout, numberOfSamples, null))
                    .Select(waveform => waveform.GetRawData())
                    .ToArray();
            });
        }

        /// <summary>
        /// Returns the waveform from a previously initiated acquisition for each site and pin in the <see cref="ScopeSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="timeout">The time to wait for data to be acquired. Specify 0 to fetch whatever samples are currently available without waiting, or -1 to wait indefinitely for the acquisition to complete.</param>
        /// <param name="numberOfSamples">The maximum number of samples to fetch for each waveform. Specify -1 to retrieve all samples currently available in the record.</param>
        /// <returns>Per-site per-pin analog waveform results.</returns>
        /// <remarks>
        /// If the acquisition does not complete within <paramref name="timeout"/>, the resulting <see cref="NISemiconductorTestException"/>
        /// includes the site, pin, and channel details for every channel on the affected card, since a single site losing its trigger
        /// signal prevents the entire card from acquiring data.
        /// A <paramref name="numberOfSamples"/> value of 0, or a negative value other than -1, is not supported by the
        /// underlying driver, which throws an <see cref="Ivi.Driver.IviCDriverException"/> that is caught and wrapped
        /// in an <see cref="NISemiconductorTestException"/>.
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">Thrown when the acquisition does not complete within <paramref name="timeout"/>, or when an invalid <paramref name="numberOfSamples"/> is specified.</exception>
        public static PinSiteData<AnalogWaveform<double>> FetchWaveform(this ScopeSessionsBundle sessionsBundle, PrecisionTimeSpan timeout, int numberOfSamples)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                return DoWithTimeoutHandling(sessionInfo, () => sessionInfo.Session.Channels[sessionInfo.AllChannelsString].Measurement.FetchDouble(timeout, numberOfSamples, null).ToArray());
            });
        }

        #region Helper Methods

        private static T DoWithTimeoutHandling<T>(ScopeSessionInformation sessionInfo, Func<T> acquisitionFunction)
        {
            try
            {
                return acquisitionFunction();
            }
            catch (MaxTimeExceededException exception)
            {
                throw new NISemiconductorTestException(BuildAcquisitionTimeoutExceptionMessage(sessionInfo), exception);
            }
        }

        private static string BuildAcquisitionTimeoutExceptionMessage(ScopeSessionInformation sessionInfo)
        {
            var channelDetails = string.Join(", ", sessionInfo.GetAssociatedSitePinListForInstrument().Select(sitePinInfo =>
                string.Format(CultureInfo.InvariantCulture, "{0} (channel: {1})", sitePinInfo.SitePinString, sitePinInfo.IndividualChannelString)));
            return string.Format(CultureInfo.InvariantCulture, ResourceStrings.Scope_AcquisitionTimeoutException, channelDetails);
        }

        #endregion
    }
}
