using System.Linq;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    /// <summary>
    /// Defines methods for standard waveform configuration
    /// </summary>
    public static class StadardWaveform
    {
        /// <summary>
        /// Configures the properties of the signal generator that affect standard waveform generation. These settings are the waveform, amplitude, DC offset, frequency, and start phase.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="standardWaveformSettings">Standard Waveform Settings.</param>
        /// <remarks>
        /// You must set Output Mode to Standard Function before you can configure the standard waveform settings.
        /// The 'User' Waveform Function Type is not supported in STL.
        /// </remarks>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, StandardWaveformSettings standardWaveformSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePininfo) =>
            {
                ValidateWaveformFunctionType(standardWaveformSettings);
                sessionInfo.Session.StandardWaveform.Configure(sitePininfo.IndividualChannelString.Split('/').Last(), standardWaveformSettings.WaveformFunctionType, standardWaveformSettings.Amplitude, standardWaveformSettings.DCOffset, standardWaveformSettings.Frequency, standardWaveformSettings.StartPhase);
            });
        }

        /// <inheritdoc cref="ConfigureStandardWaveform(FgenSessionsBundle, StandardWaveformSettings)"/>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, SiteData<StandardWaveformSettings> standardWaveformSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePininfo) =>
            {
                var standardWaveformSettingsPerSite = standardWaveformSettings.GetValue(sitePininfo.SiteNumber);
                ValidateWaveformFunctionType(standardWaveformSettingsPerSite);
                sessionInfo.Session.StandardWaveform.Configure(sitePininfo.IndividualChannelString.Split('/').Last(), standardWaveformSettingsPerSite.WaveformFunctionType, standardWaveformSettingsPerSite.Amplitude, standardWaveformSettingsPerSite.DCOffset, standardWaveformSettingsPerSite.Frequency, standardWaveformSettingsPerSite.StartPhase);
            });
        }

        /// <inheritdoc cref="ConfigureStandardWaveform(FgenSessionsBundle, StandardWaveformSettings)"/>
        public static void ConfigureStandardWaveform(this FgenSessionsBundle sessionsBundle, PinSiteData<StandardWaveformSettings> standardWaveformSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePininfo) =>
            {
                var standardWaveformSettingsPerPinPerSite = standardWaveformSettings.GetValue(sitePininfo);
                ValidateWaveformFunctionType(standardWaveformSettingsPerPinPerSite);
                sessionInfo.Session.StandardWaveform.Configure(sitePininfo.IndividualChannelString.Split('/').Last(), standardWaveformSettingsPerPinPerSite.WaveformFunctionType, standardWaveformSettingsPerPinPerSite.Amplitude, standardWaveformSettingsPerPinPerSite.DCOffset, standardWaveformSettingsPerPinPerSite.Frequency, standardWaveformSettingsPerPinPerSite.StartPhase);
            });
        }

        #region Private Methods
        private static void ValidateWaveformFunctionType(StandardWaveformSettings standardWaveformSettings)
        {
            // Throw exception if the WaveformFunctionType is User, as it is not supported in STL.
            if (standardWaveformSettings.WaveformFunctionType == StandardWaveform.User)
            {
                throw new NISemiconductorTestException(ResourceStrings.FGen_InvalidFunctionType);
            }
        }
        #endregion
    }
}
