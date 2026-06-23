using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for configuring oscilloscope instruments.
    /// </summary>
    public static class ChannelConfiguration
    {
        /// <summary>
        /// Configures the vertical settings of the specified channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="settings">The channel settings object.</param>
        public static void ConfigureChannel(this ScopeSessionsBundle sessionsBundle, ScopeChannelSettings settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureChannel(settings, sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureChannel(ScopeSessionsBundle, ScopeChannelSettings)"/>
        public static void ConfigureChannelSettings(this ScopeSessionsBundle sessionsBundle, SiteData<ScopeChannelSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureChannel(settings.GetValue(sitePinInfo.SiteNumber), sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureChannel(ScopeSessionsBundle, ScopeChannelSettings)"/>
        public static void ConfigureChannelSettings(this ScopeSessionsBundle sessionsBundle, PinSiteData<ScopeChannelSettings> settings)
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
        /// <param name="channelName">The name of the channel to configure.</param>
        /// <param name="inputImpedance">The input impedance of the channel.</param>
        /// <param name="inputFrequencyMax">The maximum input frequency for the channel.</param>
        public static void ConfigureChannelCharacteristics(this ScopeSessionsBundle sessionsBundle, string channelName, double inputImpedance, double inputFrequencyMax)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Channels[channelName].ConfigureCharacteristics(inputImpedance, inputFrequencyMax);
            });
        }

        /// <summary>
        /// Enables or disables time-interleaved sampling for the specified channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="channelName">The name of the channel to configure.</param>
        /// <param name="enableTimeInterleavedSampling">Whether to enable time-interleaved sampling.</param>
        public static void ConfigureTimeInterleavedSampling(this ScopeSessionsBundle sessionsBundle, string channelName, bool enableTimeInterleavedSampling)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Channels[channelName].EnableTimeInterleavedSampling = enableTimeInterleavedSampling;
            });
        }

        internal static void ConfigureChannel(this ScopeSessionInformation sessionInfo, ScopeChannelSettings settings, SitePinInfo sitePinInfo)
        {
            sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString].Configure(settings.Range, settings.Offset, (ScopeVerticalCoupling)settings.Coupling, settings.ProbeAttenuation, settings.Enabled);
        }
    }
}