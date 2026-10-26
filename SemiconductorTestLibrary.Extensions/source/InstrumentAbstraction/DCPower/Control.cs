using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for controlling the NI-DCPower session.
    /// </summary>
    public static class Control
    {
        #region methods on DCPowerSessionsBundle

        /// <summary>
        /// Transitions the NI-DCPower session from the Running state to the Uncommitted state.
        /// If a sequence is running, the NI-DCPower session is stopped.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <remarks>
        /// Note: This is a lower level function for controlling over the NI-DCPower session.
        /// Any low level driver property updated after this method will not be applied until the next sourcing operation,
        /// or when the Commit method is explicitly called.
        /// If power output is enabled when you call the Abort method, the channels remain in their current state and continue providing power.
        /// Refer to the Programming States topic in the NI-DCPower User Manual and the document of your SMU model for information about the specific NI-DCPower software states.
        /// Use the OutputEnable method to disable power output per channel. Use the Reset method to disable output on all channels.
        /// </remarks>
        public static void Abort(this DCPowerSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Abort();
            });
        }

        /// <summary>
        /// Applies previously configured settings to the underlying device channel(s).
        /// Calling this methods moves the underlying channel(s) from the Uncommitted state into the Committed state.
        /// After calling this method, modifying any property reverts the underlying device channel(s) to the Uncommitted state.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <remarks>
        /// Note: This is a lower level function for controlling over the NI-DCPower session.
        /// Refer to the Programming States topic in the NI-DCPower User Manual and the document of your SMU model for information about the specific NI-DCPower software states.
        /// </remarks>
        public static void Commit(this DCPowerSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Commit();
            });
        }

        /// <summary>
        /// Starts generation or acquisition and moves the underlying devices channel(s) from the Uncommitted state or Committed state to the Running state.
        /// To return to the Uncommitted state, call the Abort method.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <remarks>
        /// Note: This is a lower level function for controlling over the NI-DCPower session.
        /// Refer to the Programming States topic in the NI-DCPower User Manual and the document of your SMU model for information about the specific NI-DCPower software states.
        /// </remarks>
        public static void Initiate(this DCPowerSessionsBundle sessionsBundle)
        {
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    if (IsFollowerOfGangedChannels(sitePinInfo.CascadingInfo))
                    {
                        sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Initiate();
                    }
                });
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    if (!IsFollowerOfGangedChannels(sitePinInfo.CascadingInfo))
                    {
                        sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Initiate();
                    }
                });
            }
            else
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.Control.Initiate();
                });
            }
        }

        /// <summary>
        /// Initiates the specified advanced sequence on all sessions in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequenceName">The name of the advanced sequence to initiate.</param>
        /// <param name="waitForSequenceCompletion"><see langword="true"/> to wait for the sequence to complete before returning; <see langword="false"/> to
        /// return immediately after initiating the sequence.</param>
        /// <param name="sequenceTimeoutInSeconds">The maximum time, in seconds, to wait for the sequence to complete. Used only if <paramref name="waitForSequenceCompletion"/> is <see langword="true"/>. Must be greater then zero.</param>
        public static void InitiateAdvancedSequence(this DCPowerSessionsBundle sessionsBundle, string sequenceName, bool waitForSequenceCompletion = false, double sequenceTimeoutInSeconds = 5.0)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                var allChannelOutput = sessionInfo.AllChannelsOutput;

                allChannelOutput.Control.Abort();

                allChannelOutput.Source.Mode = DCPowerSourceMode.Sequence;

                allChannelOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = sequenceName;
            });

            sessionsBundle.Initiate();

            if (waitForSequenceCompletion)
            {
                sessionsBundle.WaitForEvent(EventType.SequenceEngineDoneEvent, sequenceTimeoutInSeconds);
            }
        }

        /// <summary>
        /// Resets the specified channels to their default state.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="channelString">The channels to reset, as a comma-separated list of instrument-channel pairs, e.g. "SMU_4147_C1_S11/0, SMU_4147_C1_S11/1".
        /// Every channel must be part of the bundle.</param>
        /// <remarks>
        /// <para>
        /// This is a soft reset that applies only to the specified channels. For each session in the bundle, the specified channels that belong to
        /// that session are reset. Other channels in the same session or on the same instrument keep their configuration, and every session stays
        /// valid. Resetting a channel disables its output, aborts it, and puts its properties back to their default values.
        /// </para>
        /// <para>
        /// For merged pin groups, specify the primary channel. Resetting it also clears the driver-level channel merging, so merge the pin group
        /// again after the reset. For ganged pin groups, specify every channel in the group, so the group stays consistent.
        /// </para>
        /// <para>
        /// Use <see cref="ResetDevice(DCPowerSessionsBundle, bool)"/> to perform a hard reset of the whole instrument.
        /// </para>
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">Thrown when <paramref name="channelString"/> is null or empty, or contains a channel that is not part of the bundle
        /// or that is a secondary channel of a merged pin group.</exception>
        public static void Reset(this DCPowerSessionsBundle sessionsBundle, string channelString)
        {
            var channelsToReset = GetChannelsToReset(sessionsBundle, channelString);
            sessionsBundle.Do(sessionInfo =>
            {
                var sessionChannels = GetChannels(sessionInfo.AllChannelsString);
                var sessionChannelsToReset = sessionChannels.Where(channelsToReset.Contains).ToList();
                if (sessionChannelsToReset.Count > 0)
                {
                    sessionInfo.Session.Utility.Reset(string.Join(",", sessionChannelsToReset));
                }
            });
        }

        /// <summary>
        /// Performs a hard reset of the instrument(s) that the channels in the bundle belong to, keeps those channels in their default state,
        /// and recovers every other channel on the same instrument(s).
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="restoreConfiguration">Whether to restore the configuration of the channels that are not in the bundle after the reset.
        /// The channels in the bundle are always left in their default state.</param>
        /// <remarks>
        /// <para>
        /// A hard reset applies to the whole instrument, not only to the channels in the bundle. When an instrument is split across several channel
        /// groups in the pin map, each channel group is a separate driver session, and the reset invalidates every session on the instrument except
        /// the one that issued it. This method resets each instrument once, then reinitializes every invalidated session, registers it with TSM again,
        /// and updates every <see cref="DCPowerSessionsBundle"/> that holds it, including bundles that are filtered by site or pin. All bundles can
        /// keep being used without getting them again from the <see cref="TSMSessionManager"/>.
        /// </para>
        /// <para>
        /// When <paramref name="restoreConfiguration"/> is <see langword="true"/>, the configuration of every channel that is not in the bundle is
        /// restored after the reset, including channels that share a channel group with the channels in the bundle. Because the driver restores
        /// configuration per session, the channels in the bundle are soft reset first in a shared channel group, so they stay in their default state.
        /// When <paramref name="restoreConfiguration"/> is <see langword="false"/>, every channel on the instrument(s) is left in its default state.
        /// </para>
        /// <para>
        /// The channel groups of sites that are disabled later in the lot are recovered too, using the topology captured when the sessions were
        /// initialized. Their configuration is only restored if it can be read from the active sites; otherwise they are left in their default state.
        /// </para>
        /// <para>
        /// Calls are serialized across threads. When several threads reset the same instrument, for example a step that runs one thread per site,
        /// the instrument is hard reset once and the other calls only reset the channels in their own bundle.
        /// </para>
        /// <para>
        /// The reset clears driver-level channel merging. Merge the merged pin groups in the bundle again after the reset.
        /// A hard reset typically takes several seconds per instrument, plus the time to register each reinitialized session with TSM,
        /// so avoid calling it for every DUT. Use <see cref="Reset(DCPowerSessionsBundle, string)"/> to reset only specific channels.
        /// </para>
        /// </remarks>
        public static void ResetDevice(this DCPowerSessionsBundle sessionsBundle, bool restoreConfiguration = true)
        {
            sessionsBundle.ResetDeviceAndRecoverSessions(restoreConfiguration);
        }
        #endregion methods on DCPowerSessionsBundle

        private static HashSet<string> GetChannelsToReset(DCPowerSessionsBundle sessionsBundle, string channelString)
        {
            if (string.IsNullOrWhiteSpace(channelString))
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_ChannelStringNullOrEmpty, nameof(channelString)));
            }

            var requestedChannels = GetChannels(channelString);
            if (requestedChannels.Count == 0)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_ChannelStringNullOrEmpty, nameof(channelString)));
            }
            var resettableChannels = new HashSet<string>(sessionsBundle.InstrumentSessions.SelectMany(sessionInfo => GetChannels(sessionInfo.AllChannelsString)), StringComparer.OrdinalIgnoreCase);
            var bundleChannels = new HashSet<string>(sessionsBundle.AggregateSitePinList.SelectMany(sitePinInfo => GetChannels(sitePinInfo.IndividualChannelString)), StringComparer.OrdinalIgnoreCase);

            var mergedSecondaryChannels = requestedChannels.Where(channel => bundleChannels.Contains(channel) && !resettableChannels.Contains(channel)).ToList();
            if (mergedSecondaryChannels.Count > 0)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_ResetSecondaryChannelsOfMergedPinGroup, string.Join(", ", mergedSecondaryChannels)));
            }

            var channelsNotInBundle = requestedChannels.Where(channel => !bundleChannels.Contains(channel)).ToList();
            if (channelsNotInBundle.Count > 0)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_ChannelsNotInSessionsBundle, string.Join(", ", channelsNotInBundle)));
            }

            return new HashSet<string>(requestedChannels, StringComparer.OrdinalIgnoreCase);
        }

        private static List<string> GetChannels(string channelString)
        {
            var channels = channelString.Split(',').Select(channel => channel.Trim());
            return channels.Where(channel => !string.IsNullOrEmpty(channel)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        internal static void ApplyUpdateMode(
            this DCPowerSessionsBundle sessionsBundle, UpdateMode updateMode)
        {
            switch (updateMode)
            {
                case UpdateMode.Commit:
                    sessionsBundle.Commit();
                    break;
                case UpdateMode.Immediate:
                    sessionsBundle.Initiate();
                    break;
                case UpdateMode.Deferred:
                default:
                    break;
            }
        }

        internal static void ApplyUpdateMode(
            this DCPowerSessionInformation sessionInfo, DCPowerOutput channelOutput, UpdateMode updateMode)
        {
            switch (updateMode)
            {
                case UpdateMode.Commit:
                    channelOutput.Control.Commit();
                    break;
                case UpdateMode.Immediate:
                    if (sessionInfo.HasGangedChannels)
                    {
                        throw new NISemiconductorTestException(ResourceStrings.DCPower_ImmediateUpdateModeNotSupportedForGangedChannels);
                    }
                    else
                    {
                        channelOutput.Control.Initiate();
                    }
                    break;
                case UpdateMode.Deferred:
                default:
                    break;
            }
        }
    }
}
