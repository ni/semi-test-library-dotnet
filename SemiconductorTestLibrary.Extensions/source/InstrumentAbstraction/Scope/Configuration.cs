using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for configuring oscilloscope instruments.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Configures the vertical verticalSettings of the specified channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="verticalSettings">The channel verticalSettings object.</param>
        public static void ConfigureChannel(this ScopeSessionsBundle sessionsBundle, VerticalSettings verticalSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureChannel(verticalSettings, sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureChannel(ScopeSessionsBundle, VerticalSettings)"/>
        public static void ConfigureChannel(this ScopeSessionsBundle sessionsBundle, SiteData<VerticalSettings> verticalSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureChannel(verticalSettings.GetValue(sitePinInfo.SiteNumber), sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureChannel(ScopeSessionsBundle, VerticalSettings)"/>
        public static void ConfigureChannelSettings(this ScopeSessionsBundle sessionsBundle, PinSiteData<VerticalSettings> verticalSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureChannel(verticalSettings.GetValue(sitePinInfo), sitePinInfo);
            });
        }

        /// <summary>
        /// Configures the electrical characteristics of the specified channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="electricalCharacteristics">The electrical characteristics of the channel.</param>
        public static void ConfigureCharacteristics(this ScopeSessionsBundle sessionsBundle, ElectricalCharacteristics electricalCharacteristics)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureCharacteristics(electricalCharacteristics, sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureCharacteristics(ScopeSessionsBundle, ElectricalCharacteristics)"/>
        public static void ConfigureCharacteristics(this ScopeSessionsBundle sessionsBundle, SiteData<ElectricalCharacteristics> electricalCharacteristics)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureCharacteristics(electricalCharacteristics.GetValue(sitePinInfo.SiteNumber), sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureCharacteristics(ScopeSessionsBundle, ElectricalCharacteristics)"/>
        public static void ConfigureCharacteristics(this ScopeSessionsBundle sessionsBundle, PinSiteData<ElectricalCharacteristics> electricalCharacteristics)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureCharacteristics(electricalCharacteristics.GetValue(sitePinInfo), sitePinInfo);
            });
        }

        internal static void ConfigureChannel(this ScopeSessionInformation sessionInfo, VerticalSettings verticalSettings, SitePinInfo sitePinInfo)
        {
            var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
            channel.Configure(verticalSettings.Range, verticalSettings.Offset, (ScopeVerticalCoupling)verticalSettings.Coupling, verticalSettings.ProbeAttenuation, verticalSettings.Enabled);
            if (verticalSettings.EnableTimeInterleavedSampling.HasValue)
            {
                channel.EnableTimeInterleavedSampling = verticalSettings.EnableTimeInterleavedSampling.Value;
            }
        }

        internal static void ConfigureCharacteristics(this ScopeSessionInformation sessionInfo, ElectricalCharacteristics electricalCharacteristics, SitePinInfo sitePinInfo)
        {
            var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
            channel.ConfigureCharacteristics(electricalCharacteristics.InputImpedance, electricalCharacteristics.InputFrequencyMax);
        }
    }
}