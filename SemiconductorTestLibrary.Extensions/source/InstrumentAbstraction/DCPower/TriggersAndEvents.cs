using System;
using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
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
            sessionsBundle.Do(sessionInfo =>
            {
                switch (triggerType)
                {
                    case TriggerType.MeasureTrigger:
                        sessionInfo.AllChannelsOutput.Triggers.MeasureTrigger.SendSoftwareEdgeTrigger();
                        break;

                    case TriggerType.PulseTrigger:
                        sessionInfo.AllChannelsOutput.Triggers.PulseTrigger.SendSoftwareEdgeTrigger();
                        break;

                    case TriggerType.SequenceAdvanceTrigger:
                        sessionInfo.AllChannelsOutput.Triggers.SequenceAdvanceTrigger.SendSoftwareEdgeTrigger();
                        break;

                    case TriggerType.SourceTrigger:
                        sessionInfo.AllChannelsOutput.Triggers.SourceTrigger.SendSoftwareEdgeTrigger();
                        break;

                    case TriggerType.StartTrigger:
                        sessionInfo.AllChannelsOutput.Triggers.StartTrigger.SendSoftwareEdgeTrigger();
                        break;

                    default:
                        break;
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
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Control.Abort();
                sessionInfo.ConfigurePulseTriggerDisable(pinSiteInfo.ModelString, pinSiteInfo.IndividualChannelString);
                sessionInfo.ConfigureSequenceAdvanceTriggerDisable(pinSiteInfo.ModelString, pinSiteInfo.IndividualChannelString);
                sessionInfo.ConfigureSourceTriggerDisable(pinSiteInfo.ModelString, pinSiteInfo.IndividualChannelString);
                sessionInfo.ConfigureStartTriggerDisable(pinSiteInfo.ModelString, pinSiteInfo.IndividualChannelString);
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
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Control.Abort();
                switch (triggerType)
                {
                    case TriggerType.MeasureTrigger:
                        sessionInfo.ConfigureMeasureTriggerDigitalEdge(tiggerTerminal, pinSiteInfo.ModelString, triggerEdge, pinSiteInfo.IndividualChannelString);
                        break;
                    case TriggerType.PulseTrigger:
                        sessionInfo.ConfigurePulseTriggerDigitalEdge(tiggerTerminal, pinSiteInfo.ModelString, triggerEdge, pinSiteInfo.IndividualChannelString);
                        break;
                    case TriggerType.SequenceAdvanceTrigger:
                        sessionInfo.ConfigureSequenceAdvanceTriggerDigitalEdge(tiggerTerminal, pinSiteInfo.ModelString, triggerEdge, pinSiteInfo.IndividualChannelString);
                        break;
                    case TriggerType.SourceTrigger:
                        sessionInfo.ConfigureSourceTriggerDigitalEdge(tiggerTerminal, pinSiteInfo.ModelString, triggerEdge, pinSiteInfo.IndividualChannelString);
                        break;
                    case TriggerType.StartTrigger:
                        sessionInfo.ConfigureStartTriggerDigitalEdge(tiggerTerminal, pinSiteInfo.ModelString, triggerEdge, pinSiteInfo.IndividualChannelString);
                        break;
                    default:
                        break;
                }
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
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Control.Abort();
                switch (triggerType)
                {
                    case TriggerType.MeasureTrigger:
                        sessionInfo.ConfigureMeasureTriggerSoftwareEdge(pinSiteInfo.ModelString, pinSiteInfo.IndividualChannelString);
                        break;
                    case TriggerType.PulseTrigger:
                        sessionInfo.ConfigurePulseTriggerSoftwareEdge(pinSiteInfo.ModelString, pinSiteInfo.IndividualChannelString);
                        break;
                    case TriggerType.SequenceAdvanceTrigger:
                        sessionInfo.ConfigureSequenceAdvanceTriggerSoftwareEdge(pinSiteInfo.ModelString, pinSiteInfo.IndividualChannelString);
                        break;
                    case TriggerType.SourceTrigger:
                        sessionInfo.ConfigureSourceTriggerSoftwareEdge(pinSiteInfo.ModelString, pinSiteInfo.IndividualChannelString);
                        break;
                    case TriggerType.StartTrigger:
                        sessionInfo.ConfigureStartTriggerSoftwareEdge(pinSiteInfo.ModelString, pinSiteInfo.IndividualChannelString);
                        break;
                    default:
                        break;
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
                IList<TriggerType> triggerTypesSupported = GetSupportedTriggerTypes(pinSiteInfo.ModelString);
                if (triggerTypesSupported.Count > 0)
                {
                    sessionInfo.AllChannelsOutput.Control.Abort();
                }
                if (triggerTypesSupported.Contains(TriggerType.PulseTrigger))
                {
                    sessionInfo.AllChannelsOutput.Triggers.PulseTrigger.Type = DCPowerPulseTriggerType.None;
                }
                if (triggerTypesSupported.Contains(TriggerType.SequenceAdvanceTrigger))
                {
                    sessionInfo.AllChannelsOutput.Triggers.SequenceAdvanceTrigger.Type = DCPowerSequenceAdvanceTriggerType.None;
                }
                if (triggerTypesSupported.Contains(TriggerType.SourceTrigger))
                {
                    sessionInfo.AllChannelsOutput.Triggers.SourceTrigger.Type = DCPowerSourceTriggerType.None;
                }
                if (triggerTypesSupported.Contains(TriggerType.StartTrigger))
                {
                    sessionInfo.AllChannelsOutput.Triggers.StartTrigger.Type = DCPowerStartTriggerType.None;
                }
            });
        }

        #endregion  methods on DCPowerSessionsBundle

        #region methods on DCPowerSessionInformation

        /// <summary>
        /// Configures a digital edge trigger for the MeasureTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>>
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
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="triggerEdge">The digital edge to look for, either <see cref="DCPowerTriggerEdge.Rising"/> or <see cref="DCPowerTriggerEdge.Falling"/>.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigureMeasureTriggerDigitalEdge(this DCPowerSessionInformation sessionInfo, string tiggerTerminal, string modelString, DCPowerTriggerEdge triggerEdge = DCPowerTriggerEdge.Rising, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.MeasureTrigger,
                output => output.Triggers.MeasureTrigger.DigitalEdge.Configure(tiggerTerminal, triggerEdge));
        }

        /// <summary>
        /// Configures a software edge trigger for the MeasureTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigureMeasureTriggerSoftwareEdge(this DCPowerSessionInformation sessionInfo, string modelString, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.MeasureTrigger,
                output => output.Triggers.MeasureTrigger.ConfigureSoftwareEdgeTrigger());
        }

        /// <summary>
        /// Configures a digital edge trigger for the PulseTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>>
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
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="triggerEdge">The digital edge to look for, either <see cref="DCPowerTriggerEdge.Rising"/> or <see cref="DCPowerTriggerEdge.Falling"/>.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigurePulseTriggerDigitalEdge(this DCPowerSessionInformation sessionInfo, string tiggerTerminal, string modelString, DCPowerTriggerEdge triggerEdge = DCPowerTriggerEdge.Rising, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.PulseTrigger,
                output => output.Triggers.PulseTrigger.DigitalEdge.Configure(tiggerTerminal, triggerEdge));
        }

        /// <summary>
        /// Configures a software edge trigger for the PulseTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigurePulseTriggerSoftwareEdge(this DCPowerSessionInformation sessionInfo, string modelString, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.PulseTrigger,
                output => output.Triggers.PulseTrigger.ConfigureSoftwareEdgeTrigger());
        }

        /// <summary>
        /// Disables the PulseTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigurePulseTriggerDisable(this DCPowerSessionInformation sessionInfo, string modelString, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.PulseTrigger,
                output => output.Triggers.PulseTrigger.Disable());
        }

        /// <summary>
        /// Configures a digital edge trigger for the SequenceAdvanceTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>>
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
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="triggerEdge">The digital edge to look for, either <see cref="DCPowerTriggerEdge.Rising"/> or <see cref="DCPowerTriggerEdge.Falling"/>.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigureSequenceAdvanceTriggerDigitalEdge(this DCPowerSessionInformation sessionInfo, string tiggerTerminal, string modelString, DCPowerTriggerEdge triggerEdge = DCPowerTriggerEdge.Rising, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.SequenceAdvanceTrigger,
                output => output.Triggers.SequenceAdvanceTrigger.DigitalEdge.Configure(tiggerTerminal, triggerEdge));
        }

        /// <summary>
        /// Configures a software edge trigger for the SequenceAdvanceTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigureSequenceAdvanceTriggerSoftwareEdge(this DCPowerSessionInformation sessionInfo, string modelString, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.SequenceAdvanceTrigger,
                output => output.Triggers.SequenceAdvanceTrigger.ConfigureSoftwareEdgeTrigger());
        }

        /// <summary>
        /// Disables the SequenceAdvanceTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigureSequenceAdvanceTriggerDisable(this DCPowerSessionInformation sessionInfo, string modelString, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.SequenceAdvanceTrigger,
                output => output.Triggers.SequenceAdvanceTrigger.Disable());
        }

        /// <summary>
        /// Configures a digital edge trigger for the SourceTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>>
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
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="triggerEdge">The digital edge to look for, either <see cref="DCPowerTriggerEdge.Rising"/> or <see cref="DCPowerTriggerEdge.Falling"/>.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigureSourceTriggerDigitalEdge(this DCPowerSessionInformation sessionInfo, string tiggerTerminal, string modelString, DCPowerTriggerEdge triggerEdge = DCPowerTriggerEdge.Rising, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.SourceTrigger,
                output => output.Triggers.SourceTrigger.DigitalEdge.Configure(tiggerTerminal, triggerEdge));
        }

        /// <summary>
        /// Configures a software edge trigger for the SourceTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigureSourceTriggerSoftwareEdge(this DCPowerSessionInformation sessionInfo, string modelString, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.SourceTrigger,
                output => output.Triggers.SourceTrigger.ConfigureSoftwareEdgeTrigger());
        }

        /// <summary>
        /// Disables the SourceTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigureSourceTriggerDisable(this DCPowerSessionInformation sessionInfo, string modelString, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.SourceTrigger,
                output => output.Triggers.SourceTrigger.Disable());
        }

        /// <summary>
        /// Configures a digital edge trigger for the StartTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
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
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="triggerEdge">The digital edge to look for, either <see cref="DCPowerTriggerEdge.Rising"/> or <see cref="DCPowerTriggerEdge.Falling"/>.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigureStartTriggerDigitalEdge(this DCPowerSessionInformation sessionInfo, string tiggerTerminal, string modelString, DCPowerTriggerEdge triggerEdge = DCPowerTriggerEdge.Rising, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.StartTrigger,
                output => output.Triggers.StartTrigger.DigitalEdge.Configure(tiggerTerminal, triggerEdge));
        }

        /// <summary>
        /// Configures a software edge trigger for the StartTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigureStartTriggerSoftwareEdge(this DCPowerSessionInformation sessionInfo, string modelString, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.StartTrigger,
                output => output.Triggers.StartTrigger.ConfigureSoftwareEdgeTrigger());
        }

        /// <summary>
        /// Disables the StartTrigger.
        /// </summary>
        /// <remarks>This method does not abort the underlying driver session.</remarks>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="modelString">The model string of the associated instrument.</param>
        /// <param name="channelString">
        /// The channel string containing one or more instrument channels.
        /// For Example: "SMU_4147_C2_16/0", or "SMU_4147_C2_16/3, SMU_4137_C2_17/0".
        /// </param>
        public static void ConfigureStartTriggerDisable(this DCPowerSessionInformation sessionInfo, string modelString, string channelString = "")
        {
            sessionInfo.DoForSupportedModels(
                channelString,
                modelString,
                TriggerType.StartTrigger,
                output => output.Triggers.StartTrigger.Disable());
        }

        private static void DoForSupportedModels(this DCPowerSessionInformation sessionInfo, string channelString, string modelString, TriggerType triggerType, Action<DCPowerOutput> action)
        {
            string channelStringToUse = string.IsNullOrEmpty(channelString) ? sessionInfo.AllChannelsString : channelString;
            if (IsTriggerTypeSupported(modelString, triggerType))
            {
                action(sessionInfo.Session.Outputs[channelStringToUse]);
            }
        }

        #endregion methods on DCPowerSessionInformation
    }
}
