using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for DCPower triggers and events.
    /// </summary>
    public static class TriggersAndEvents
    {
        #region methods on DCPowerSessionsBundle

        /// <summary>
        /// Exports the selected DCPowerSignalSource to the target output terminal.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="signalSource">The signal source to export.</param>
        /// <param name="outputTerminal">The output terminal the signal routes to.</param>
        [Obsolete("An output terminal is reserved by the first channel that exports a signal to it, which makes exporting the same terminal from every channel in the bundle fail. Use ExportSignal(DCPowerSessionsBundle, DCPowerSignalSource, PinSiteData<string>) to specify a unique output terminal for each site-pin pair instead.")]
        public static void ExportSignal(this DCPowerSessionsBundle sessionsBundle, DCPowerSignalSource signalSource, string outputTerminal)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.ExportSignal(signalSource, outputTerminal);
            });
        }

        /// <summary>
        /// Exports the selected DCPowerSignalSource to the output terminal specified for each site-pin pair.
        /// </summary>
        /// <remarks>
        /// An output terminal is reserved by the channel that exports a signal to it, therefore each site-pin pair requires its own unique output terminal.
        /// Since a PXIe chassis provides only eight common backplane trigger lines, keep the number of site-pin pairs in <paramref name="outputTerminals"/> within that limit.<br/>
        /// The signal is exported only for the site-pin pairs specified in <paramref name="outputTerminals"/>.
        /// Site-pin pairs in the <paramref name="sessionsBundle"/> that are not present in <paramref name="outputTerminals"/> are not exported.<br/>
        /// Each output terminal can be specified as a trigger line name, for example "PXI_Trig0", or in the fully qualified format, for example "/Dev1/PXI_Trig0".<br/>
        /// The following exceptions can be thrown:
        /// <list type="bullet">
        /// <item><description><see cref="ArgumentNullException"/> if <paramref name="outputTerminals"/> is null or contains a null output terminal.</description></item>
        /// <item><description><see cref="ArgumentException"/> if <paramref name="outputTerminals"/> does not contain any pins or sites or contains empty string values.</description></item>
        /// <item><description><see cref="NISemiconductorTestException"/> if the same output terminal is assigned to more than one site-pin pair.</description></item>
        /// <item><description><see cref="NISemiconductorTestException"/> wrapping an <see cref="Ivi.Driver.IviCDriverException"/> if the destination terminal to be routed cannot be found on the device.</description></item>
        /// <item><description><see cref="NISemiconductorTestException"/> wrapping an <see cref="Ivi.Driver.IviCDriverException"/> if the specified route requires resources that are currently in use by another route.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="signalSource">The signal source to export.</param>
        /// <param name="outputTerminals">The per-site per-pin output terminals the signal routes to, for example "PXI_Trig0".</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputTerminals"/> is null or contains a null output terminal.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="outputTerminals"/> does not contain any pins or sites or contains empty string values.
        /// </exception>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the same output terminal is assigned to more than one site-pin pair,
        /// or when the driver throws an <see cref="Ivi.Driver.IviCDriverException"/> because the destination terminal to be routed cannot be found on the device.
        /// </exception>
        public static void ExportSignal(this DCPowerSessionsBundle sessionsBundle, DCPowerSignalSource signalSource, PinSiteData<string> outputTerminals)
        {
            ValidateOutputTerminals(outputTerminals);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (outputTerminals.TryGetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName, out var outputTerminal))
                {
                    sessionInfo.Session.ExportSignal(sitePinInfo.IndividualChannelString, signalSource, outputTerminal);
                }
            });
        }

        /// <summary>
        /// Exports the selected DCPowerSignalSource from the channel mapped to the specified pin and site to the target output terminal.
        /// </summary>
        /// <remarks>
        /// Use this overload when only one signal needs to be exported from the bundle. Otherwise, use the <see cref="ExportSignal(DCPowerSessionsBundle, DCPowerSignalSource, PinSiteData{string})"/> overload instead.<br/>
        /// The output terminal can be specified as a trigger line name, for example "PXI_Trig0", or in the fully qualified format, for example "/Dev1/PXI_Trig0".<br/>
        /// No signal is exported if the <paramref name="sessionsBundle"/> does not contain the specified pin and site.<br/>
        /// The following exceptions can be thrown:
        /// <list type="bullet">
        /// <item><description><see cref="ArgumentNullException"/> if <paramref name="outputTerminal"/> is null.</description></item>
        /// <item><description><see cref="ArgumentException"/> if <paramref name="outputTerminal"/> is an empty string.</description></item>
        /// <item><description><see cref="NISemiconductorTestException"/> wrapping an <see cref="Ivi.Driver.IviCDriverException"/> if the destination terminal to be routed cannot be found on the device.</description></item>
        /// <item><description><see cref="NISemiconductorTestException"/> wrapping an <see cref="Ivi.Driver.IviCDriverException"/> if the specified route requires resources that are currently in use by another route.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="signalSource">The signal source to export.</param>
        /// <param name="pinName">The name of the pin to export the signal from.</param>
        /// <param name="siteNumber">The site number to export the signal from.</param>
        /// <param name="outputTerminal">The output terminal the signal routes to, for example "PXI_Trig0".</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputTerminal"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="outputTerminal"/> is an empty string.
        /// </exception>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the driver throws an <see cref="Ivi.Driver.IviCDriverException"/> because the destination terminal to be routed cannot be found on the device.
        /// </exception>
        public static void ExportSignal(this DCPowerSessionsBundle sessionsBundle, DCPowerSignalSource signalSource, string pinName, int siteNumber, string outputTerminal)
        {
            ValidateOutputTerminal(outputTerminal, string.Format(CultureInfo.InvariantCulture, "Site{0}/{1}", siteNumber, pinName), nameof(outputTerminal));
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (sitePinInfo.PinName == pinName && sitePinInfo.SiteNumber == siteNumber)
                {
                    sessionInfo.Session.ExportSignal(sitePinInfo.IndividualChannelString, signalSource, outputTerminal);
                }
            });
        }

        /// <summary>
        /// Validates that every site-pin pair is assigned a unique and non-empty output terminal.
        /// </summary>
        /// <param name="outputTerminals">The per-site per-pin output terminals to validate.</param>
        private static void ValidateOutputTerminals(PinSiteData<string> outputTerminals)
        {
            if (outputTerminals is null)
            {
                throw new ArgumentNullException(nameof(outputTerminals), ResourceStrings.DCPower_ExportSignalOutputTerminalsEmpty);
            }
            if (!outputTerminals.PinNames.Any() || outputTerminals.PinNames.Any(pinName => !outputTerminals.SiteNumbersByPin[pinName].Any()))
            {
                throw new ArgumentException(ResourceStrings.DCPower_ExportSignalOutputTerminalsEmpty, nameof(outputTerminals));
            }

            var assignments = outputTerminals.PinNames
                .SelectMany(pinName => outputTerminals.SiteNumbersByPin[pinName].Select(siteNumber => new
                {
                    SitePinString = string.Format(CultureInfo.InvariantCulture, "Site{0}/{1}", siteNumber, pinName),
                    OutputTerminal = outputTerminals.GetValue(siteNumber, pinName)
                }))
                .ToList();
            foreach (var assignment in assignments)
            {
                ValidateOutputTerminal(assignment.OutputTerminal, assignment.SitePinString, nameof(outputTerminals));
            }

            var duplicateAssignments = assignments
                .GroupBy(assignment => assignment.OutputTerminal, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(group => group.Count() > 1);
            if (duplicateAssignments != null)
            {
                throw new NISemiconductorTestException(string.Format(
                    CultureInfo.InvariantCulture,
                    ResourceStrings.DCPower_ExportSignalOutputTerminalNotUnique,
                    duplicateAssignments.Key,
                    string.Join(", ", duplicateAssignments.Select(assignment => assignment.SitePinString))));
            }
        }

        /// <summary>
        /// Validates that an output terminal is not null or empty.
        /// </summary>
        /// <param name="outputTerminal">The output terminal to validate.</param>
        /// <param name="sitePinString">The site-pin pair the output terminal is assigned to.</param>
        /// <param name="parameterName">The name of the parameter that provided the output terminal.</param>
        private static void ValidateOutputTerminal(string outputTerminal, string sitePinString, string parameterName)
        {
            var message = string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_ExportSignalOutputTerminalEmpty, sitePinString);
            if (outputTerminal is null)
            {
                throw new ArgumentNullException(parameterName, message);
            }
            if (string.IsNullOrEmpty(outputTerminal))
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Sends a software trigger as the selected TriggerType.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="triggerType">The type of the trigger.</param>
        public static void SendSoftwareEdgeTrigger(this DCPowerSessionsBundle sessionsBundle, TriggerType triggerType)
        {
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                var triggerTypesUnsupported = GetUnsupportedTriggerTypes(pinSiteInfo.ModelString);
                if (!triggerTypesUnsupported.Contains(triggerType) && !IsFollowerOfGangedChannels(pinSiteInfo.CascadingInfo))
                {
                    var output = sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString];
                    switch (triggerType)
                    {
                        case TriggerType.MeasureTrigger:
                            output.Triggers.MeasureTrigger.SendSoftwareEdgeTrigger();
                            break;

                        case TriggerType.PulseTrigger:
                            output.Triggers.PulseTrigger.SendSoftwareEdgeTrigger();
                            break;

                        case TriggerType.SequenceAdvanceTrigger:
                            output.Triggers.SequenceAdvanceTrigger.SendSoftwareEdgeTrigger();
                            break;

                        case TriggerType.SourceTrigger:
                            output.Triggers.SourceTrigger.SendSoftwareEdgeTrigger();
                            break;

                        case TriggerType.StartTrigger:
                            output.Triggers.StartTrigger.SendSoftwareEdgeTrigger();
                            break;

                        default:
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// Waits for the selected EventType to occur.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="eventType">The type of event to wait for.</param>
        /// <param name="timeout">The maximum time to wait. Default is 5 seconds.</param>
        public static void WaitForEvent(this DCPowerSessionsBundle sessionsBundle, EventType eventType, double timeout = 5.0)
        {
            var timeoutAsPrecisionTimeSpan = PrecisionTimeSpan.FromSeconds(timeout);
            sessionsBundle.Do(sessionInfo =>
            {
                switch (eventType)
                {
                    case EventType.MeasureCompleteEvent:
                        sessionInfo.AllChannelsOutput.Events.MeasureCompleteEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;

                    case EventType.PulseCompleteEvent:
                        sessionInfo.AllChannelsOutput.Events.PulseCompleteEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;

                    case EventType.ReadyForPulseTriggerEvent:
                        sessionInfo.AllChannelsOutput.Events.ReadyForPulseTriggerEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;

                    case EventType.SequenceEngineDoneEvent:
                        sessionInfo.AllChannelsOutput.Events.SequenceEngineDoneEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;

                    case EventType.SequenceIterationCompleteEvent:
                        sessionInfo.AllChannelsOutput.Events.SequenceIterationCompleteEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;

                    case EventType.SourceCompleteEvent:
                        sessionInfo.AllChannelsOutput.Events.SourceCompleteEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;
                }
            });
        }

        /// <summary>
        /// Disables the following triggers by configuring them to None: PulseTrigger, SequenceAdvanceTrigger, SourceTrigger, and StartTrigger.
        /// <para> MeasureTrigger is not supported. It does not need to be disabled.</para>
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="triggerTypes">Optional list of trigger types to disable. If null or empty, all supported triggers are disabled.</param>
        public static void DisableTriggers(this DCPowerSessionsBundle sessionsBundle, IEnumerable<TriggerType> triggerTypes = null)
        {
            // Need to loop over each channel because not all channels in the sessionInfo.ChannelString are guaranteed to be
            // mapped to the same model, and therefore not all channels in the sessionInfo.ChannelString may support this operation.
            // Hence, need the ability to check the operation against each channel when configuring triggers.
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                var triggerTypesUnsupported = GetUnsupportedTriggerTypes(pinSiteInfo.ModelString);
                var triggerTypesToDisable = (triggerTypes == null || !triggerTypes.Any()) ? new List<TriggerType>() { TriggerType.PulseTrigger, TriggerType.SequenceAdvanceTrigger, TriggerType.SourceTrigger, TriggerType.StartTrigger } : triggerTypes;
                var supportedTriggerTypesToDisable = triggerTypesToDisable.Except(triggerTypesUnsupported);
                if (supportedTriggerTypesToDisable.Any() && !IsFollowerOfGangedChannels(pinSiteInfo.CascadingInfo))
                {
                    var output = sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString];
                    output.Control.Abort();
                    if (supportedTriggerTypesToDisable.Contains(TriggerType.PulseTrigger))
                    {
                        output.Triggers.PulseTrigger.Disable();
                    }
                    if (supportedTriggerTypesToDisable.Contains(TriggerType.SequenceAdvanceTrigger))
                    {
                        output.Triggers.SequenceAdvanceTrigger.Disable();
                    }
                    if (supportedTriggerTypesToDisable.Contains(TriggerType.SourceTrigger))
                    {
                        output.Triggers.SourceTrigger.Disable();
                    }
                    if (supportedTriggerTypesToDisable.Contains(TriggerType.StartTrigger))
                    {
                        output.Triggers.StartTrigger.Disable();
                    }
                }
            });
        }

        /// <summary>
        /// Configures a digital edge trigger for one of the following selected TriggerTypes: MeasureTrigger, PulseTrigger, SequenceAdvanceTrigger, SourceTrigger, and StartTrigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="triggerType">One of the following trigger types: MeasureTrigger, PulseTrigger, SequenceAdvanceTrigger, SourceTrigger, StartTrigger.</param>>
        /// <param name="tiggerTerminal">The input terminal to configure the trigger to look for a Digital Edge.
        /// <para>
        /// This is the fully qualified terminal string, which must be in the form of <code>"/Dev1/PXI_Trig0"</code>,
        /// where Dev1 is the instrument generating the trigger and PXI_Trig0 is the trigger line the trigger is being sent on.
        /// </para>
        /// <para>Note that the input terminal can also be a terminal from another instrument or channel.</para>
        /// <para>
        /// For example, you can set the input terminal on Dev1 to be /Dev2/Engine0/SourceCompleteEvent, where Engine0 is channel 0.
        /// </para>
        /// </param>
        /// <param name="triggerEdge">The digital edge to look for, either <see cref="DCPowerTriggerEdge.Rising"/> or <see cref="DCPowerTriggerEdge.Falling"/>.</param>
        public static void ConfigureTriggerDigitalEdge(this DCPowerSessionsBundle sessionsBundle, TriggerType triggerType, string tiggerTerminal, DCPowerTriggerEdge triggerEdge = DCPowerTriggerEdge.Rising)
        {
            // Need to loop over each channel because not all channels in the sessionInfo.ChannelString are guaranteed to be
            // mapped to the same model, and therefore not all channels in the sessionInfo.ChannelString may support this operation.
            // Hence, need the ability to check the operation against each channel when configuring triggers.
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                if (!IsFollowerOfGangedChannels(pinSiteInfo.CascadingInfo))
                {
                    ConfigureTriggerDigitalEdge(sessionInfo, pinSiteInfo, triggerType, tiggerTerminal, triggerEdge);
                }
            });
        }

        /// <summary>
        /// Configures a software edge trigger for one of the following selected TriggerTypes: MeasureTrigger, PulseTrigger, SequenceAdvanceTrigger, SourceTrigger, and StartTrigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="triggerType">Type of trigger, either MeasureTrigger, PulseTrigger, SequenceAdvanceTrigger, SourceTrigger, or StartTrigger.</param>>
        public static void ConfigureTriggerSoftwareEdge(this DCPowerSessionsBundle sessionsBundle, TriggerType triggerType)
        {
            // Need to loop over each channel because not all channels in the sessionInfo.ChannelString are guaranteed to be
            // mapped to the same model, and therefore not all channels in the sessionInfo.ChannelString may support this operation.
            // Hence, need the ability to check the operation against each channel when configuring triggers.
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                var triggerTypesUnsupported = GetUnsupportedTriggerTypes(pinSiteInfo.ModelString);
                if (!triggerTypesUnsupported.Contains(triggerType) && !IsFollowerOfGangedChannels(pinSiteInfo.CascadingInfo))
                {
                    var output = sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString];
                    output.Control.Abort();
                    switch (triggerType)
                    {
                        case TriggerType.MeasureTrigger:
                            output.Triggers.MeasureTrigger.ConfigureSoftwareEdgeTrigger();
                            break;
                        case TriggerType.PulseTrigger:
                            output.Triggers.PulseTrigger.ConfigureSoftwareEdgeTrigger();
                            break;
                        case TriggerType.SequenceAdvanceTrigger:
                            output.Triggers.SequenceAdvanceTrigger.ConfigureSoftwareEdgeTrigger();
                            break;
                        case TriggerType.SourceTrigger:
                            output.Triggers.SourceTrigger.ConfigureSoftwareEdgeTrigger();
                            break;
                        case TriggerType.StartTrigger:
                            output.Triggers.StartTrigger.ConfigureSoftwareEdgeTrigger();
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// Clears all trigger types.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        public static void ClearTriggers(this DCPowerSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                var triggerTypesUnsupported = GetUnsupportedTriggerTypes(pinSiteInfo.ModelString);
                var triggerTypesToClear = new List<TriggerType>() { TriggerType.PulseTrigger, TriggerType.SequenceAdvanceTrigger, TriggerType.SourceTrigger, TriggerType.StartTrigger };
                var supportedTriggerTypesToClear = triggerTypesToClear.Except(triggerTypesUnsupported);
                if (supportedTriggerTypesToClear.Any() && !IsFollowerOfGangedChannels(pinSiteInfo.CascadingInfo))
                {
                    var output = sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString];
                    output.Control.Abort();
                    if (supportedTriggerTypesToClear.Contains(TriggerType.PulseTrigger))
                    {
                        output.Triggers.PulseTrigger.Type = DCPowerPulseTriggerType.None;
                    }
                    if (supportedTriggerTypesToClear.Contains(TriggerType.SequenceAdvanceTrigger))
                    {
                        output.Triggers.SequenceAdvanceTrigger.Type = DCPowerSequenceAdvanceTriggerType.None;
                    }
                    if (supportedTriggerTypesToClear.Contains(TriggerType.SourceTrigger))
                    {
                        output.Triggers.SourceTrigger.Type = DCPowerSourceTriggerType.None;
                    }
                    if (supportedTriggerTypesToClear.Contains(TriggerType.StartTrigger))
                    {
                        output.Triggers.StartTrigger.Type = DCPowerStartTriggerType.None;
                    }
                }
            });
        }

        #endregion  methods on DCPowerSessionsBundle

        #region methods on DCPowerOutput

        internal static void ConfigureSourceTriggerForCascading(this DCPowerOutput dcPowerOutput, SitePinInfo sitePinInfo)
        {
            var gangingInfo = sitePinInfo?.CascadingInfo as GangingInfo;
            if (IsFollowerOfGangedChannels(gangingInfo))
            {
                dcPowerOutput.ConfigureTriggerDigitalEdge(TriggerType.SourceTrigger, gangingInfo.SourceTriggerName, DCPowerTriggerEdge.Rising);
            }
        }

        internal static void ConfigureStartTriggerForCascadedSequencing(this DCPowerOutput dcPowerOutput, SitePinInfo sitePinInfo)
        {
            var gangingInfo = sitePinInfo?.CascadingInfo as GangingInfo;
            if (IsFollowerOfGangedChannels(gangingInfo))
            {
                dcPowerOutput.ConfigureTriggerDigitalEdge(TriggerType.StartTrigger, gangingInfo.StartTriggerName, DCPowerTriggerEdge.Rising);
            }
        }

        internal static void ConfigureSequenceAdvanceTriggerForCascadedSequencing(this DCPowerOutput dcPowerOutput, SitePinInfo sitePinInfo)
        {
            var gangingInfo = sitePinInfo?.CascadingInfo as GangingInfo;
            if (IsFollowerOfGangedChannels(gangingInfo))
            {
                dcPowerOutput.ConfigureTriggerDigitalEdge(TriggerType.SequenceAdvanceTrigger, gangingInfo.SequenceAdvanceTriggerName, DCPowerTriggerEdge.Rising);
            }
        }

        internal static void ConfigureMeasureTriggerForCascading(this DCPowerSessionInformation sessionInfo, string channelString)
        {
            if (sessionInfo.HasGangedChannels)
            {
                var sitePinInfoList = sessionInfo.AssociatedSitePinList.Where(sitePinInfo => channelString.Contains(sitePinInfo.IndividualChannelString));
                Parallel.ForEach(sitePinInfoList, sitePinInfo =>
                {
                    sessionInfo.ConfigureMeasureTriggerForCascading(sitePinInfo);
                });
            }
        }

        internal static void ConfigureMeasureTriggerForCascading(this DCPowerSessionInformation sessionInfo, SitePinInfo sitePinInfo)
        {
            var gangingInfo = sitePinInfo?.CascadingInfo as GangingInfo;
            if (IsFollowerOfGangedChannels(gangingInfo))
            {
                sessionInfo.ConfigureTriggerDigitalEdge(sitePinInfo, TriggerType.MeasureTrigger, gangingInfo.MeasureTriggerName, DCPowerTriggerEdge.Rising);
            }
        }

        private static void ConfigureTriggerDigitalEdge(this DCPowerSessionInformation sessionInfo, SitePinInfo sitePinInfo, TriggerType triggerType, string tiggerTerminal, DCPowerTriggerEdge triggerEdge = DCPowerTriggerEdge.Rising)
        {
            var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
            output.ConfigureTriggerDigitalEdge(triggerType, tiggerTerminal, triggerEdge, sitePinInfo.ModelString);
        }

        private static void ConfigureTriggerDigitalEdge(this DCPowerOutput dcPowerOutput, TriggerType triggerType, string tiggerTerminal, DCPowerTriggerEdge triggerEdge = DCPowerTriggerEdge.Rising, string instrumentModel = "")
        {
            var triggerTypesUnsupported = GetUnsupportedTriggerTypes(instrumentModel);
            if (!triggerTypesUnsupported.Contains(triggerType))
            {
                dcPowerOutput.Control.Abort();
                switch (triggerType)
                {
                    case TriggerType.MeasureTrigger:
                        dcPowerOutput.Triggers.MeasureTrigger.DigitalEdge.Configure(tiggerTerminal, triggerEdge);
                        break;
                    case TriggerType.PulseTrigger:
                        dcPowerOutput.Triggers.PulseTrigger.DigitalEdge.Configure(tiggerTerminal, triggerEdge);
                        break;
                    case TriggerType.SequenceAdvanceTrigger:
                        dcPowerOutput.Triggers.SequenceAdvanceTrigger.DigitalEdge.Configure(tiggerTerminal, triggerEdge);
                        break;
                    case TriggerType.SourceTrigger:
                        dcPowerOutput.Triggers.SourceTrigger.DigitalEdge.Configure(tiggerTerminal, triggerEdge);
                        break;
                    case TriggerType.StartTrigger:
                        dcPowerOutput.Triggers.StartTrigger.DigitalEdge.Configure(tiggerTerminal, triggerEdge);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion methods on DCPowerOutput
    }
}
