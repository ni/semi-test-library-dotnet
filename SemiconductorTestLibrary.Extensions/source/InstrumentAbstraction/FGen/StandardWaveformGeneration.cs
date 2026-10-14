using System.Globalization;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    /// <summary>
    /// Defines methods for standard waveform configuration.
    /// </summary>
    public static class StandardWaveformGeneration
    {
        /// <summary>
        /// Configures the properties of the signal generator that affect standard waveform generation. These settings are the waveform, amplitude, DC offset, frequency, and start phase.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="standardWaveformSettings">The <see cref="StandardWaveformSettings"/> object.</param>
        /// <remarks>
        /// You must set the output mode to <see cref="OutputMode.Function"/> by calling <see cref="Output.ConfigureOutputMode"/> before you configure the standard waveform settings.<br/>
        /// The <see cref="StandardWaveform.User"/> waveform function type is not supported.<br/>
        /// Throws <see cref="NISemiconductorTestException"/> if the waveform function type is <see cref="StandardWaveform.User"/>,
        /// if the output mode is not set to <see cref="OutputMode.Function"/>, or if any of the specified settings is invalid for the instrument.
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when <see cref="StandardWaveformSettings.WaveformFunctionType"/> is set to <see cref="StandardWaveform.User"/>,
        /// when the output mode is not set to <see cref="OutputMode.Function"/>,
        /// or when any of the specified <see cref="StandardWaveformSettings"/> values is invalid for the underlying devices.
        /// </exception>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, StandardWaveformSettings standardWaveformSettings)
        {
            ValidateWaveformFunctionType(standardWaveformSettings);
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.StandardWaveform.Configure(
                    sessionInfo.AllChannelsString,
                    standardWaveformSettings.WaveformFunctionType,
                    standardWaveformSettings.Amplitude,
                    standardWaveformSettings.DcOffset,
                    standardWaveformSettings.Frequency,
                    standardWaveformSettings.StartPhase);
            });
        }

        /// <inheritdoc cref="ConfigureStandardWaveform(FgenSessionsBundle, StandardWaveformSettings)"/>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, SiteData<StandardWaveformSettings> standardWaveformSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePininfo) =>
            {
                var standardWaveformSettingsPerSite = standardWaveformSettings.GetValue(sitePininfo.SiteNumber);
                ValidateWaveformFunctionType(standardWaveformSettingsPerSite);
                var channelName = CommonUtils.GetChannelName(sitePininfo.IndividualChannelString);
                sessionInfo.Session.StandardWaveform.Configure(
                    channelName,
                    standardWaveformSettingsPerSite.WaveformFunctionType,
                    standardWaveformSettingsPerSite.Amplitude,
                    standardWaveformSettingsPerSite.DcOffset,
                    standardWaveformSettingsPerSite.Frequency,
                    standardWaveformSettingsPerSite.StartPhase);
            });
        }

        /// <inheritdoc cref="ConfigureStandardWaveform(FgenSessionsBundle, StandardWaveformSettings)"/>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, PinSiteData<StandardWaveformSettings> standardWaveformSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePininfo) =>
            {
                var standardWaveformSettingsPerPinPerSite = standardWaveformSettings.GetValue(sitePininfo);
                ValidateWaveformFunctionType(standardWaveformSettingsPerPinPerSite);
                var channelName = CommonUtils.GetChannelName(sitePininfo.IndividualChannelString);
                sessionInfo.Session.StandardWaveform.Configure(
                    channelName,
                    standardWaveformSettingsPerPinPerSite.WaveformFunctionType,
                    standardWaveformSettingsPerPinPerSite.Amplitude,
                    standardWaveformSettingsPerPinPerSite.DcOffset,
                    standardWaveformSettingsPerPinPerSite.Frequency,
                    standardWaveformSettingsPerPinPerSite.StartPhase);
            });
        }

        #region Private Methods
        private static void ValidateWaveformFunctionType(StandardWaveformSettings standardWaveformSettings)
        {
            // Throw exception if the WaveformFunctionType is User, as it is not supported in STL.
            if (standardWaveformSettings.WaveformFunctionType == StandardWaveform.User)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.FGen_UnsupportedWaveFunctionType));
            }
        }
        #endregion
    }
}
