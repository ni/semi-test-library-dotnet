using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;

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
        public static void ExportSignal(this DCPowerSessionsBundle sessionsBundle, DCPowerSignalSource signalSource, string outputTerminal)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.ExportSignal(signalSource, outputTerminal);
            });
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
                if (!triggerTypesUnsupported.Contains(triggerType))
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
        /// Disables all triggers by configuring them to None: PulseTrigger, SequenceAdvanceTrigger, SourceTrigger, and StartTrigger.
        /// <para> Note that MeasureTrigger is not supported. It does not need to be disabled.</para>
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        public static void DisableTriggers(this DCPowerSessionsBundle sessionsBundle)
        {
            // Need to loop over each channel because not all channels in the sessionInfo.ChannelString are guaranteed to be
            // mapped to the same model, and therefore not all channels in the sessionInfo.ChannelString may support this operation.
            // Hence, need the ability to check the operation against each channel when configuring triggers.
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                var triggerTypesUnsupported = GetUnsupportedTriggerTypes(pinSiteInfo.ModelString);
                var triggerTypesToDisable = new List<TriggerType>() { TriggerType.PulseTrigger, TriggerType.SequenceAdvanceTrigger, TriggerType.SourceTrigger, TriggerType.StartTrigger };
                var supportedTriggerTypesToDisable = triggerTypesToDisable.Except(triggerTypesUnsupported);
                if (supportedTriggerTypesToDisable.Any())
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
        /// Configures a digital edge trigger for the selected TriggerType: MeasureTrigger, PulseTrigger, SequenceAdvanceTrigger, SourceTrigger, and StartTrigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="triggerType">Type of trigger, either MeasureTrigger, PulseTrigger, SequenceAdvanceTrigger, SourceTrigger, StartTrigger.</param>>
        /// <param name="tiggerTerminal">The input terminal to configure the trigger to look for a Digital Edge.
        /// <para>
        /// This is the fully qualified terminal string, which should be in the form of <code>"/Dev1/PXI_Trig0"</code>,
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
                var output = sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString];
                output.ConfigureTriggerDigitalEdge(triggerType, tiggerTerminal, triggerEdge, pinSiteInfo.ModelString);
            });
        }

        /// <summary>
        /// Configures a software edge trigger for the selected TriggerType: MeasureTrigger, PulseTrigger, SequenceAdvanceTrigger, SourceTrigger, and StartTrigger.
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
                if (!triggerTypesUnsupported.Contains(triggerType))
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
                if (supportedTriggerTypesToClear.Any())
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

        internal static void ConfigureSourceTriggerForCascading(this DCPowerOutput dcPowerOutput, SitePinInfo sitePin)
        {
            var gangingInfo = sitePin?.CascadingInfo as GangingInfo;
            if (IsFollowerOfGangedChannels(gangingInfo))
            {
                dcPowerOutput.ConfigureTriggerDigitalEdge(TriggerType.SourceTrigger, gangingInfo.SourceTriggerName, DCPowerTriggerEdge.Rising);
            }
        }

        internal static void ConfigureMeasureTriggerForCascading(this DCPowerSessionInformation sessionInfo, string channelString, string modelString, SitePinInfo sitePinInfo)
        {
            var output = sessionInfo.Session.Outputs[channelString];
            if (sessionInfo.HasGangedChannels)
            {
                var sitePinInfoList = sitePinInfo != null ? new List<SitePinInfo>() { sitePinInfo } : sessionInfo.AssociatedSitePinList.Where(sitePin => channelString.Contains(sitePin.IndividualChannelString));
                Parallel.ForEach(sitePinInfoList, sitePin =>
                {
                    output.ConfigureMeasureTriggerForCascading(sitePin);
                });
            }
        }

        private static void ConfigureMeasureTriggerForCascading(this DCPowerOutput dcPowerOutput, SitePinInfo sitePin)
        {
            var gangingInfo = sitePin?.CascadingInfo as GangingInfo;
            if (IsFollowerOfGangedChannels(gangingInfo))
            {
                dcPowerOutput.ConfigureTriggerDigitalEdge(TriggerType.MeasureTrigger, gangingInfo.MeasureTriggerName, DCPowerTriggerEdge.Rising);
            }
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
