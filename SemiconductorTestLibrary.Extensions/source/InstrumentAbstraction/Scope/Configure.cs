using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for configuring oscilloscope instruments.
    /// </summary>
    public static class Configure
    {
        /// <summary>
        /// Configures the vertical verticalSettings of the specified channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="verticalSettings">The channel verticalSettings object.</param>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, VerticalSettings verticalSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureVertical(verticalSettings, sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureVertical(ScopeSessionsBundle, VerticalSettings)"/>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, SiteData<VerticalSettings> verticalSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureVertical(verticalSettings.GetValue(sitePinInfo.SiteNumber), sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureVertical(ScopeSessionsBundle, VerticalSettings)"/>
        public static void ConfigureVertical(this ScopeSessionsBundle sessionsBundle, PinSiteData<VerticalSettings> verticalSettings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureVertical(verticalSettings.GetValue(sitePinInfo), sitePinInfo);
            });
        }

        /// <summary>
        /// Configures the electrical characteristics of the specified channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="electricalCharacteristics">The electrical characteristics of the channel.</param>
        public static void ConfigureElectricalCharacteristics(this ScopeSessionsBundle sessionsBundle, ElectricalCharacteristics electricalCharacteristics)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureElectricalCharacteristics(electricalCharacteristics, sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureElectricalCharacteristics(ScopeSessionsBundle, ElectricalCharacteristics)"/>
        public static void ConfigureElectricalCharacteristics(this ScopeSessionsBundle sessionsBundle, SiteData<ElectricalCharacteristics> electricalCharacteristics)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureElectricalCharacteristics(electricalCharacteristics.GetValue(sitePinInfo.SiteNumber), sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureElectricalCharacteristics(ScopeSessionsBundle, ElectricalCharacteristics)"/>
        public static void ConfigureElectricalCharacteristics(this ScopeSessionsBundle sessionsBundle, PinSiteData<ElectricalCharacteristics> electricalCharacteristics)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureElectricalCharacteristics(electricalCharacteristics.GetValue(sitePinInfo), sitePinInfo);
            });
        }

        internal static void ConfigureVertical(this ScopeSessionInformation sessionInfo, VerticalSettings verticalSettings, SitePinInfo sitePinInfo)
        {
            var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
            channel.Configure(verticalSettings.Range, verticalSettings.Offset, (ScopeVerticalCoupling)verticalSettings.Coupling, verticalSettings.ProbeAttenuation, verticalSettings.Enabled);
            if (verticalSettings.EnableTimeInterleavedSampling.HasValue)
            {
                channel.EnableTimeInterleavedSampling = verticalSettings.EnableTimeInterleavedSampling.Value;
            }
        }

        internal static void ConfigureElectricalCharacteristics(this ScopeSessionInformation sessionInfo, ElectricalCharacteristics electricalCharacteristics, SitePinInfo sitePinInfo)
        {
            var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
            channel.ConfigureCharacteristics(electricalCharacteristics.InputImpedance, electricalCharacteristics.InputFrequencyMax);
        }
    }
}