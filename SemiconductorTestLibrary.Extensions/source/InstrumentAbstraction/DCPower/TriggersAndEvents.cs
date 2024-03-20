using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for DCPower triggers and events.
    /// </summary>
    public static class TriggersAndEvents
    {
        /// <summary>
        /// Exports DCPower signal.
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
        /// Sends software edge trigger.
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
                        sessionInfo.ChannelOutput.Triggers.MeasureTrigger.SendSoftwareEdgeTrigger();
                        break;

                    case TriggerType.PulseTrigger:
                        sessionInfo.ChannelOutput.Triggers.PulseTrigger.SendSoftwareEdgeTrigger();
                        break;

                    case TriggerType.SequenceAdvanceTrigger:
                        sessionInfo.ChannelOutput.Triggers.SequenceAdvanceTrigger.SendSoftwareEdgeTrigger();
                        break;

                    case TriggerType.SourceTrigger:
                        sessionInfo.ChannelOutput.Triggers.SourceTrigger.SendSoftwareEdgeTrigger();
                        break;

                    case TriggerType.StartTrigger:
                        sessionInfo.ChannelOutput.Triggers.StartTrigger.SendSoftwareEdgeTrigger();
                        break;

                    default:
                        break;
                }
            });
        }

        /// <summary>
        /// Waits for an event.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="eventType">The type of event to wait for.</param>
        /// <param name="timeout">The maximum time to wait.</param>
        public static void WaitForEvent(this DCPowerSessionsBundle sessionsBundle, EventType eventType, double timeout = 5.0)
        {
            var timeoutAsPrecisionTimeSpan = PrecisionTimeSpan.FromSeconds(timeout);
            sessionsBundle.Do(sessionInfo =>
            {
                switch (eventType)
                {
                    case EventType.MeasureCompleteEvent:
                        sessionInfo.ChannelOutput.Events.MeasureCompleteEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;

                    case EventType.PulseCompleteEvent:
                        sessionInfo.ChannelOutput.Events.PulseCompleteEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;

                    case EventType.ReadyForPulseTriggerEvent:
                        sessionInfo.ChannelOutput.Events.ReadyForPulseTriggerEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;

                    case EventType.SequenceEngineDoneEvent:
                        sessionInfo.ChannelOutput.Events.SequenceEngineDoneEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;

                    case EventType.SequenceIterationCompleteEvent:
                        sessionInfo.ChannelOutput.Events.SequenceIterationCompleteEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;

                    case EventType.SourceCompleteEvent:
                        sessionInfo.ChannelOutput.Events.SourceCompleteEvent.WaitForEvent(timeoutAsPrecisionTimeSpan);
                        break;
                }
            });
        }

        /// <summary>
        /// Clears all triggers.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        public static void ClearTriggers(this DCPowerSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                if (sessionInfo.ModelString != DCPowerModelStrings.PXI_4110)
                {
                    sessionInfo.ChannelOutput.Control.Abort();
                    sessionInfo.ChannelOutput.Triggers.PulseTrigger.Type = DCPowerPulseTriggerType.None;
                    sessionInfo.ChannelOutput.Triggers.SequenceAdvanceTrigger.Type = DCPowerSequenceAdvanceTriggerType.None;
                    sessionInfo.ChannelOutput.Triggers.SourceTrigger.Type = DCPowerSourceTriggerType.None;
                    sessionInfo.ChannelOutput.Triggers.StartTrigger.Type = DCPowerStartTriggerType.None;
                }
            });
        }
    }

    /// <summary>
    /// Defines DCPower trigger type.
    /// </summary>
    public enum TriggerType
    {
        /// <summary>
        /// The measure trigger.
        /// </summary>
        MeasureTrigger,

        /// <summary>
        /// The pulse trigger.
        /// </summary>
        PulseTrigger,

        /// <summary>
        /// The sequence advance trigger.
        /// </summary>
        SequenceAdvanceTrigger,

        /// <summary>
        /// The source trigger.
        /// </summary>
        SourceTrigger,

        /// <summary>
        /// The start trigger.
        /// </summary>
        StartTrigger
    }

    /// <summary>
    /// Defines DCPower event type.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// The measure complete event.
        /// </summary>
        MeasureCompleteEvent,

        /// <summary>
        /// The pulse complete event.
        /// </summary>
        PulseCompleteEvent,

        /// <summary>
        /// The ready for pulse trigger event.
        /// </summary>
        ReadyForPulseTriggerEvent,

        /// <summary>
        /// The sequence engine done event.
        /// </summary>
        SequenceEngineDoneEvent,

        /// <summary>
        /// The sequence iteration complete event.
        /// </summary>
        SequenceIterationCompleteEvent,

        /// <summary>
        /// The source complete event.
        /// </summary>
        SourceCompleteEvent
    }
}
