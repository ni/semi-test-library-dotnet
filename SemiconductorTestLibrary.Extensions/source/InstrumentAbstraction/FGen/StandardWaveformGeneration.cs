using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    /// <summary>
    /// Defines methods for standard waveform configuration
    /// </summary>
    public static class StandardWaveformGeneration
    {
        /// <summary>
        /// Configures the properties of the signal generator that affect standard waveform generation. These settings are the waveform, amplitude, DC offset, frequency, and start phase.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="standardWaveformSettings">Standard Waveform Settings.</param>
        /// <remarks>
        /// You must set Output Mode to Standard Function before you can configure the standard waveform settings.
        /// The 'User' Waveform Function Type is not supported in STL.
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">Thrown when the WaveformFunctionType is set to User.</exception>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, StandardWaveformSettings standardWaveformSettings)
        {
            ValidateWaveformFunctionType(standardWaveformSettings);
            sessionsBundle.Do((sessionInfo, sitePininfo) =>
            {
                sessionInfo.Session.StandardWaveform.Configure(sitePininfo.IndividualChannelString.Split('/').Last(), standardWaveformSettings.WaveformFunctionType, standardWaveformSettings.Amplitude, standardWaveformSettings.DcOffset, standardWaveformSettings.Frequency, standardWaveformSettings.StartPhase);
            });
        }

        /// <inheritdoc cref="ConfigureStandardWaveform(FgenSessionsBundle, StandardWaveformSettings)"/>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, SiteData<StandardWaveformSettings> standardWaveformSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePininfo) =>
            {
                var standardWaveformSettingsPerSite = standardWaveformSettings.GetValue(sitePininfo.SiteNumber);
                ValidateWaveformFunctionType(standardWaveformSettingsPerSite);
                sessionInfo.Session.StandardWaveform.Configure(sitePininfo.IndividualChannelString.Split('/').Last(), standardWaveformSettingsPerSite.WaveformFunctionType, standardWaveformSettingsPerSite.Amplitude, standardWaveformSettingsPerSite.DcOffset, standardWaveformSettingsPerSite.Frequency, standardWaveformSettingsPerSite.StartPhase);
            });
        }

        /// <inheritdoc cref="ConfigureStandardWaveform(FgenSessionsBundle, StandardWaveformSettings)"/>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, PinSiteData<StandardWaveformSettings> standardWaveformSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePininfo) =>
            {
                var standardWaveformSettingsPerPinPerSite = standardWaveformSettings.GetValue(sitePininfo);
                ValidateWaveformFunctionType(standardWaveformSettingsPerPinPerSite);
                sessionInfo.Session.StandardWaveform.Configure(sitePininfo.IndividualChannelString.Split('/').Last(), standardWaveformSettingsPerPinPerSite.WaveformFunctionType, standardWaveformSettingsPerPinPerSite.Amplitude, standardWaveformSettingsPerPinPerSite.DcOffset, standardWaveformSettingsPerPinPerSite.Frequency, standardWaveformSettingsPerPinPerSite.StartPhase);
            });
        }

        #region Private Methods
        private static void ValidateWaveformFunctionType(StandardWaveformSettings standardWaveformSettings)
        {
            // Throw exception if the WaveformFunctionType is User, as it is not supported in STL.
            if (standardWaveformSettings.WaveformFunctionType == ModularInstruments.NIFgen.StandardWaveform.User)
            {
                throw new NISemiconductorTestException(ResourceStrings.FGen_InvalidFunctionType);
            }
        }
        #endregion
    }
}
