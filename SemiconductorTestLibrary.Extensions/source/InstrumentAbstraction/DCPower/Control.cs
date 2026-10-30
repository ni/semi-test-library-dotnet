using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
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
        /// Resets the channels associated with the specified pins to their default state.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a soft reset that applies only to the channels of the specified pins, across all sites in the bundle. To reset the pins of
        /// specific sites only, filter the bundle by site first. Other channels in the same session or on the same instrument keep their
        /// configuration, and every session stays valid. Resetting a channel disables its output, aborts it, and puts its properties back to
        /// their default values.
        /// </para>
        /// <para>
        /// Pin group names are expanded to the pins they contain. For merged pin groups, specify the merged pin group or its primary pin.
        /// Resetting the primary pin also clears the driver-level channel merging, so merge the pin group again after the reset.
        /// For ganged pin groups, specify the ganged pin group or every pin in it, so the group stays consistent.
        /// </para>
        /// <para>
        /// All pin and pin group names are validated before any channel is reset, so no channel is reset when any of them is not valid.
        /// Throws an <see cref="NISemiconductorTestException"/> when <paramref name="pinNames"/> is null or empty or contains a null or empty name,
        /// when it contains a pin or pin group that is not part of the bundle, or when it contains a secondary pin of a merged pin group without
        /// its primary pin.
        /// </para>
        /// <para>
        /// Use <see cref="ResetDevice(DCPowerSessionsBundle, bool)"/> to perform a hard reset of the whole instrument.
        /// </para>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="pinNames">
        /// The pins or pin groups to reset. Every pin, including every pin of a pin group, must be part of the bundle.
        /// </param>
        /// <exception cref="NISemiconductorTestException">Thrown when <paramref name="pinNames"/> is null or empty, or contains a null or empty name.</exception>
        /// <exception cref="NISemiconductorTestException">Thrown when <paramref name="pinNames"/> contains a pin or pin group that is not part of the bundle.</exception>
        /// <exception cref="NISemiconductorTestException">Thrown when <paramref name="pinNames"/> contains a secondary pin of a merged pin group but not its primary pin.</exception>
        public static void Reset(this DCPowerSessionsBundle sessionsBundle, params string[] pinNames)
        {
            var channelsToReset = GetValidatedChannelsToReset(sessionsBundle, pinNames);
            sessionsBundle.Do(sessionInfo =>
            {
                var sessionChannelStringToReset = string.Join(",", GetChannels(sessionInfo.AllChannelsString).Where(channelsToReset.Contains));
                if (sessionChannelStringToReset.Length > 0)
                {
                    sessionInfo.Session.Utility.Reset(sessionChannelStringToReset);
                }
            });
        }

        /// <summary>
        /// Performs a hard reset of the instrument(s) that the channels in the bundle belong to, and recovers every other channel on the same instrument(s).
        /// </summary>
        /// <remarks>
        /// <para>
        /// The device reset operation is applied to the instrument as a whole, across all channels, not only to the channels in the bundle.
        /// </para>
        /// <para>
        /// When an instrument is split across several channel groups in the pin map, each channel group is a separate driver session, and the reset
        /// invalidates every session on the instrument except the one that issued it. This method resets each instrument once, then reinitializes
        /// every invalidated session and re-registers them with the TSM session manager.
        /// Session references in existing <see cref="DCPowerSessionsBundle"/> objects are updated, including bundles that are filtered by site or pin.
        /// This allows all bundle objects to continue being used in subsequent operations.
        /// </para>
        /// <para>
        /// When <paramref name="restoreConfiguration"/> is <see langword="true"/> (default), the configuration of every channel that is not present in
        /// the bundle is preserved and restored after the reset, including channels that share a channel group with the channels in the bundle.
        /// Since the driver configuration can only be applied to the session as a whole, and not to individual channels, the target channels in the
        /// bundle are first soft reset to ensure they are in their default state when the driver configuration is applied after the reset device
        /// operation is performed. When <paramref name="restoreConfiguration"/> is <see langword="false"/>, every channel on the instrument(s) is left
        /// in its default state.
        /// </para>
        /// <para>
        /// Additionally, when an instrument is split across several channel groups and sites are disabled, the channels that map to disabled sites
        /// are also recovered, using the channel group information captured when the sessions were first initialized. Their configuration is only
        /// restored if it can be read from the active sites; otherwise they are left in their default state./// </para>
        /// <para>
        /// Calls are serialized across threads. When several threads reset the same instrument,
        /// the instrument is hard reset once and the other calls only reset the channels in their own bundle.
        /// For example, when calling code is parallelized or invoked from a TestStand step configured to execute in one thread per site./// </para>
        /// <para>
        /// The reset clears driver-level channel merging. Merge the merged pin groups in the bundle again after the reset. The secondary channel of
        /// a merge cannot be reset on its own, so a secondary channel in the bundle whose primary channel is in another bundle is not reset and
        /// keeps following its primary channel. The merged channels of other bundles on the instrument(s) are restored with their configuration
        /// when <paramref name="restoreConfiguration"/> is <see langword="true"/>; otherwise unmerge and merge those pin groups again.
        /// </para>
        /// <para>
        /// Avoid calling this method often, as it is expected to take several seconds to execute per session.
        /// Use the <see cref="Reset(DCPowerSessionsBundle, string[])"/> method instead to reset specific channels.
        /// </para>
        /// <para>
        /// Throws an <see cref="NISemiconductorTestException"/> when the sessions were not initialized with
        /// <see cref="InitializeAndClose.Initialize"/> or mapped with
        /// <see cref="Initialization.MapInitializedInstrumentSessions"/>, or were already closed;
        /// when a session in the bundle is not registered with TSM; or when a channel group fails to be prepared before the reset. In these cases,
        /// no instrument is reset. Also throws an <see cref="NISemiconductorTestException"/> when one or more channel groups fail to be recovered
        /// after the reset, for example when a reinitialized session cannot be registered with TSM. The other channel groups are still recovered,
        /// and the error of each channel group that failed is reported.
        /// </para>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="restoreConfiguration">
        /// Whether to preserve and restore the configuration of any channels that share the same device as the channels being reset but are not present in the bundle.
        /// Where, the channels that are present in the bundle will be reset to their default state.
        /// Set to <see langword="true"/> by default.
        /// </param>
        /// <exception cref="NISemiconductorTestException">Thrown when the sessions were not initialized with
        /// <see cref="InitializeAndClose.Initialize"/> or mapped with
        /// <see cref="Initialization.MapInitializedInstrumentSessions"/>, or were already closed.
        /// No instrument is reset.</exception>
        /// <exception cref="NISemiconductorTestException">Thrown when a session in the bundle is not registered with TSM, so its full channel group
        /// cannot be determined. No instrument is reset.</exception>
        /// <exception cref="NISemiconductorTestException">Thrown when a channel group fails to be prepared before the reset, for example when its
        /// configuration cannot be exported. No instrument is reset.</exception>
        /// <exception cref="NISemiconductorTestException">Thrown when a channel group fails to be recovered after the reset. When several channel groups
        /// fail, the exception lists all of them, and its inner <see cref="AggregateException"/> contains the error of each one.</exception>
        /// <exception cref="NISemiconductorTestException">Thrown when a reinitialized session cannot be registered with TSM because the context used for
        /// recovery does not contain the sites of its channel group.</exception>
        public static void ResetDevice(this DCPowerSessionsBundle sessionsBundle, bool restoreConfiguration = true)
        {
            sessionsBundle.ResetDeviceAndRecoverSessions(restoreConfiguration);
        }
        #endregion methods on DCPowerSessionsBundle

        private static HashSet<string> GetValidatedChannelsToReset(DCPowerSessionsBundle sessionsBundle, string[] pinNames)
        {
            if (pinNames == null || pinNames.Length == 0 || pinNames.Any(string.IsNullOrWhiteSpace))
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_PinNamesNullOrEmpty, nameof(pinNames)));
            }

            var pinsToReset = GetPinsInBundle(sessionsBundle, pinNames.Select(pinName => pinName.Trim()).Distinct());
            var channelsOfPinsToReset = new HashSet<string>(
                sessionsBundle.AggregateSitePinList
                    .Where(sitePinInfo => pinsToReset.Contains(sitePinInfo.PinName))
                    .SelectMany(sitePinInfo => GetChannels(sitePinInfo.IndividualChannelString)),
                StringComparer.OrdinalIgnoreCase);

            // The secondary channel of a merge is not in AllChannelsString and cannot be reset on its own. It is reset with its primary channel.
            var resettableChannels = new HashSet<string>(sessionsBundle.InstrumentSessions.SelectMany(sessionInfo => GetChannels(sessionInfo.AllChannelsString)), StringComparer.OrdinalIgnoreCase);
            var channelsToReset = new HashSet<string>(channelsOfPinsToReset.Where(resettableChannels.Contains), StringComparer.OrdinalIgnoreCase);
            var secondaryChannels = channelsOfPinsToReset.Where(channel => !resettableChannels.Contains(channel)).ToList();
            if (secondaryChannels.Count > 0)
            {
                var channelsMergedIntoChannelsToReset = GetChannelsMergedInto(sessionsBundle, channelsToReset, secondaryChannels);
                var secondarySitePinsWithoutPrimary = sessionsBundle.AggregateSitePinList
                    .Where(sitePinInfo => pinsToReset.Contains(sitePinInfo.PinName)
                        && GetChannels(sitePinInfo.IndividualChannelString).Any(channel => secondaryChannels.Contains(channel) && !channelsMergedIntoChannelsToReset.Contains(channel)))
                    .Select(sitePinInfo => sitePinInfo.SitePinString)
                    .Distinct()
                    .ToList();
                if (secondarySitePinsWithoutPrimary.Count > 0)
                {
                    throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_ResetSecondaryPinsOfMergedPinGroup, FormatNames(secondarySitePinsWithoutPrimary)));
                }
            }
            return channelsToReset;
        }

        // Pins of the bundle are used as they are, and any other name is expanded as a pin group. A name that is neither, and a pin group
        // with pins that are not in the bundle, are reported together.
        private static HashSet<string> GetPinsInBundle(DCPowerSessionsBundle sessionsBundle, IEnumerable<string> pinOrPinGroupNames)
        {
            var pinsOfBundle = new HashSet<string>(sessionsBundle.AggregateSitePinList.Select(sitePinInfo => sitePinInfo.PinName));
            var pins = new HashSet<string>();
            var namesNotInBundle = new List<string>();
            foreach (var pinOrPinGroupName in pinOrPinGroupNames)
            {
                if (pinsOfBundle.Contains(pinOrPinGroupName))
                {
                    pins.Add(pinOrPinGroupName);
                    continue;
                }
                var pinsInPinGroup = GetPinsInPinGroupOrEmpty(sessionsBundle.TSMContext, pinOrPinGroupName);
                if (pinsInPinGroup.Length == 0)
                {
                    namesNotInBundle.Add(pinOrPinGroupName);
                    continue;
                }
                namesNotInBundle.AddRange(pinsInPinGroup.Where(pin => !pinsOfBundle.Contains(pin)));
                pins.UnionWith(pinsInPinGroup.Where(pinsOfBundle.Contains));
            }

            if (namesNotInBundle.Count > 0)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_PinsNotInSessionsBundle, FormatNames(namesNotInBundle.Distinct())));
            }
            return pins;
        }

        // TSM throws for a name that is not a pin group of the pin map, such as a pin or an unknown name.
        private static string[] GetPinsInPinGroupOrEmpty(ISemiconductorModuleContext tsmContext, string pinGroupName)
        {
            try
            {
                return tsmContext.GetPinsInPinGroup(pinGroupName) ?? Array.Empty<string>();
            }
            catch (Exception exception) when (!(exception is OutOfMemoryException))
            {
                return Array.Empty<string>();
            }
        }

        // Returns the channels that the primary channels being reset merge, as reported by the driver. Merging is only possible within one instrument,
        // so only the primary channels on the instruments of the secondary channels are read.
        private static HashSet<string> GetChannelsMergedInto(DCPowerSessionsBundle sessionsBundle, ISet<string> channelsToReset, IEnumerable<string> secondaryChannels)
        {
            var instrumentsOfSecondaryChannels = new HashSet<string>(secondaryChannels.Select(GetInstrumentName), StringComparer.OrdinalIgnoreCase);
            var mergedChannels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var sessionInfo in sessionsBundle.InstrumentSessions)
            {
                foreach (var primaryChannel in GetChannels(sessionInfo.AllChannelsString).Where(channel => channelsToReset.Contains(channel) && instrumentsOfSecondaryChannels.Contains(GetInstrumentName(channel))))
                {
                    var instrumentName = GetInstrumentName(primaryChannel);
                    var mergedChannelString = sessionInfo.Session.Outputs[primaryChannel].Source.MergedChannels;
                    mergedChannels.UnionWith(GetChannels(mergedChannelString ?? string.Empty).Select(channel => channel.Contains("/") ? channel : $"{instrumentName}/{channel}"));
                }
            }
            return mergedChannels;
        }

        private static string GetInstrumentName(string channel)
        {
            return channel.Split('/')[0].Trim();
        }

        private static string FormatNames(IEnumerable<string> names)
        {
            return string.Join(", ", names.Select(name => $"\"{name}\""));
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
