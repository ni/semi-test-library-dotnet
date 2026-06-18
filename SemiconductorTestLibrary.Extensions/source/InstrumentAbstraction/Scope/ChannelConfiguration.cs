using System;
using System.Collections.Generic;

using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
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
        /// <param name="channelName">The name of the channel to configure.</param>
        /// <param name="range">The vertical range of the channel.</param>
        /// <param name="offset">The vertical offset of the channel.</param>
        /// <param name="coupling">The coupling for the channel.</param>
        /// <param name="probeAttenuation">The probe attenuation factor.</param>
        /// <param name="enabled">Whether the channel is enabled.</param>
        public static void ConfigureChannel(this ScopeSessionsBundle sessionsBundle, string channelName, double range, double offset, ScopeVerticalCoupling coupling, double probeAttenuation, bool enabled)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Channels[channelName].Configure(range, offset, coupling, probeAttenuation, enabled);
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

        /// <summary>
        /// Configures the settings for the specified channel using a channel settings object.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="channelName">The name of the channel to configure.</param>
        /// <param name="settings">The channel settings object.</param>
        public static void ConfigureChannelSettings(this ScopeSessionsBundle sessionsBundle, string channelName, ScopeChannelSettings settings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Channels[channelName].Configure(settings.Range.Value, settings.Offset.Value, (ScopeVerticalCoupling)settings.Coupling.Value, settings.ProbeAttenuation.Value, settings.Enabled.Value);
            });
        }
    }
}