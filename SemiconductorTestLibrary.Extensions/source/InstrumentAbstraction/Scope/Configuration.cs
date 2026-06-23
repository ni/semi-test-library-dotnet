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
        /// Configures the vertical settings of the specified channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="settings">The channel settings object.</param>
        public static void ConfigureChannel(this ScopeSessionsBundle sessionsBundle, ChannelSettings settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureChannel(settings, sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureChannel(ScopeSessionsBundle, ChannelSettings)"/>
        public static void ConfigureChannel(this ScopeSessionsBundle sessionsBundle, SiteData<ChannelSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureChannel(settings.GetValue(sitePinInfo.SiteNumber), sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureChannel(ScopeSessionsBundle, ChannelSettings)"/>
        public static void ConfigureChannelSettings(this ScopeSessionsBundle sessionsBundle, PinSiteData<ChannelSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureChannel(settings.GetValue(sitePinInfo), sitePinInfo);
            });
        }

        /// <summary>
        /// Configures the electrical characteristics of the specified channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="channelCharacteristics">The electrical characteristics of the channel.</param>
        public static void ConfigureCharacteristics(this ScopeSessionsBundle sessionsBundle, ChannelCharacteristics channelCharacteristics)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureCharacteristics(channelCharacteristics, sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureCharacteristics(ScopeSessionsBundle, ChannelCharacteristics)"/>
        public static void ConfigureCharacteristics(this ScopeSessionsBundle sessionsBundle, SiteData<ChannelCharacteristics> channelCharacteristics)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureCharacteristics(channelCharacteristics.GetValue(sitePinInfo.SiteNumber), sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureCharacteristics(ScopeSessionsBundle, ChannelCharacteristics)"/>
        public static void ConfigureCharacteristics(this ScopeSessionsBundle sessionsBundle, PinSiteData<ChannelCharacteristics> channelCharacteristics)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureCharacteristics(channelCharacteristics.GetValue(sitePinInfo), sitePinInfo);
            });
        }

        internal static void ConfigureChannel(this ScopeSessionInformation sessionInfo, ChannelSettings settings, SitePinInfo sitePinInfo)
        {
            var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
            channel.Configure(settings.Range, settings.Offset, (ScopeVerticalCoupling)settings.Coupling, settings.ProbeAttenuation, settings.Enabled);
            if (settings.EnableTimeInterleavedSampling.HasValue)
            {
                channel.EnableTimeInterleavedSampling = settings.EnableTimeInterleavedSampling.Value;
            }
        }

        internal static void ConfigureCharacteristics(this ScopeSessionInformation sessionInfo, ChannelCharacteristics channelCharacteristics, SitePinInfo sitePinInfo)
        {
            var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
            channel.ConfigureCharacteristics(channelCharacteristics.InputImpedance, channelCharacteristics.InputFrequencyMax);
        }
    }
}