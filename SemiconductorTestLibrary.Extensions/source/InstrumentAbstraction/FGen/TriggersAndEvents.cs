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
        /// Configures the Start Trigger for digital edge triggering.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="source">
        /// The terminal to use as the trigger source, such as PXI_Trig0.
        /// </param>
        /// <param name="digitalEdge">The active edge of the trigger signal.</param>
        /// <remarks>
        /// Throws <see cref="ArgumentNullException"/> if <paramref name="source"/> is null.<br/>
        /// Throws <see cref="ArgumentException"/> if <paramref name="source"/> is an empty string.<br/>
        /// Throws <see cref="NISemiconductorTestException"/> if <paramref name="source"/> is not a valid trigger source terminal.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="source"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="source"/> is an empty string.
        /// </exception>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when <paramref name="source"/> is not a valid trigger source terminal.
        /// </exception>
        public static void ConfigureStartTriggerDigitalEdge(this FgenSessionsBundle sessionsBundle, string source, DigitalEdge digitalEdge)
        {
            ValidateParameter(source, nameof(source), ResourceStrings.FGen_TriggerSourceNullOrEmpty);
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Trigger.Start.DigitalEdge.Configure(source, digitalEdge);
            });
        }

        /// <summary>
        /// Configures the Start Trigger to use software triggering.
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
        /// <remarks>
        /// Throws <see cref="NISemiconductorTestException"/> when this method is called before <c>Initiate</c>.
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when this method is called before <c>Initiate</c>.
        /// </exception>
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
        /// <see cref="SignalSource.StartedEvent"/>, and <see cref="SignalSource.DoneEvent"/>.<br/>
        /// Throws <see cref="ArgumentException"/> for all other values.<br/>
        /// Throws <see cref="ArgumentNullException"/> if <paramref name="outputTerminal"/> is null.<br/>
        /// Throws <see cref="ArgumentException"/> if <paramref name="outputTerminal"/> is an empty string.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputTerminal"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="signalSource"/> is not one of StartTrigger, ReadyForStartEvent, StartedEvent, or DoneEvent,<br/>
        /// or when <paramref name="outputTerminal"/> is an empty string.
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

            ValidateParameter(outputTerminal, nameof(outputTerminal), ResourceStrings.FGen_OutputTerminalNullOrEmpty);

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

        #region Helper Methods

        private static void ValidateParameter(string parameter, string parameterName, string errorMessage)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(parameterName, errorMessage);
            }
            if (parameter.Length == 0)
            {
                throw new ArgumentException(errorMessage, parameterName);
            }
        }

        #endregion
    }
}
