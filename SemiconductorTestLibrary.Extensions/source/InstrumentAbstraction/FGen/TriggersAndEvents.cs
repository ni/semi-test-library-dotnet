using System;
using System.Globalization;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    /// <summary>
    /// Defines methods for Fgen triggers and events.
    /// </summary>
    public static class TriggersAndEvents
    {
        #region Methods on FgenSessionsBundle

        /// <summary>
        /// Configures the Start Trigger to be a digital edge trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="source">The terminal to use as the trigger source, such as PXI_Trig0.</param>
        /// <param name="digitalEdge">The active edge of the trigger signal. Only Rising edge is supported.</param>
        /// <remarks>
        /// Only <see cref="DigitalEdge.Rising"/> is supported for <paramref name="digitalEdge"/>.
        /// <see cref="DigitalEdge.Falling"/> throws an <see cref="ArgumentException"/>.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="digitalEdge"/> is not <see cref="DigitalEdge.Rising"/>.
        /// </exception>
        public static void ConfigureStartTriggerDigitalEdge(this FgenSessionsBundle sessionsBundle, string source, DigitalEdge digitalEdge)
        {
            if (digitalEdge != DigitalEdge.Rising)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.FGen_UnsupportedDigitalEdge));
            }
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.Start.DigitalEdge.Configure(source, digitalEdge);
            });
        }

        /// <summary>
        /// Configures the Start Trigger to be a software edge trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        public static void ConfigureStartTriggerSoftwareEdge(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.Start.SoftwareEdge.Configure();
            });
        }

        /// <summary>
        /// Sends a software edge trigger to the instrument.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        public static void SendSoftwareEdgeTrigger(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.Start.SoftwareEdge.Send();
            });
        }

        /// <summary>
        /// Exports the specified signal to the specified output terminal.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="signalSource"> The signal source to export.</param>
        /// <param name="signalIdentifier">The identifier of the signal to export. Use an empty string when the signal has no identifier.</param>
        /// <param name="outputTerminal">The terminal to export the signal to, such as PXI_Trig0.</param>
        /// <remarks>
        /// Allowed values for <paramref name="signalSource"/> are <see cref="SignalSource.StartTrigger"/>, <see cref="SignalSource.ReadyForStartEvent"/>,
        /// <see cref="SignalSource.StartedEvent"/>, and <see cref="SignalSource.DoneEvent"/>.
        /// All other values throw a <see cref="ArgumentException"/>.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="signalSource"/> is not one of StartTrigger, ReadyForStartEvent, StartedEvent, or DoneEvent.
        /// </exception>
        public static void ExportSignal(this FgenSessionsBundle sessionsBundle, SignalSource signalSource, string signalIdentifier, string outputTerminal)
        {
            switch (signalSource)
            {
                case SignalSource.StartTrigger:
                case SignalSource.ReadyForStartEvent:
                case SignalSource.StartedEvent:
                case SignalSource.DoneEvent:
                    break;
                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.FGen_UnsupportedSignalSource, signalSource));
            }

            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.ExportSignal(signalSource, signalIdentifier ?? string.Empty, outputTerminal);
            });
        }

        /// <summary>
        /// Disables the Start Trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <remarks>
        /// Throws <see cref="NISemiconductorTestException"/> if the Start Trigger is disabled while the FGen is generating a waveform.
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the Start Trigger is disabled while the FGen is generating a waveform.
        /// </exception>
        public static void DisableStartTrigger(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.Start.Disable();
            });
        }

        /// <summary>
        /// Configures the trigger mode of the instrument.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="triggerMode">The trigger mode to set. Only Continuous trigger mode is supported for standard waveforms.</param>
        /// <remarks>
        /// Throws <see cref="NISemiconductorTestException"/> for all values except <see cref="TriggerMode.Continuous"/>.
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when <paramref name="triggerMode"/> is not <see cref="TriggerMode.Continuous"/>.
        /// </exception>
        public static void ConfigureTriggerMode(this FgenSessionsBundle sessionsBundle, TriggerMode triggerMode)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.SetTriggerMode(sessionInfo.AllChannelsString, triggerMode);
            });
        }

        #endregion
    }
}