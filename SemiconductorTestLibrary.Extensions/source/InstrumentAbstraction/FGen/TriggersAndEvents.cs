using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    /// <summary>
    /// Defines methods for Fgen triggers and events.
    /// </summary>
    public static class TriggersAndEvents
    {
        #region methods on FgenSessionsBundle

        /// <summary>
        /// Configures the Start Trigger to be a digital edge trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="source">The terminal to use as the trigger source, such as PXI_Trig0.</param>
        /// <param name="digitalEdge">The active edge of the trigger signal.</param>
        public static void ConfigureStartTriggerDigitalEdge(this FgenSessionsBundle sessionsBundle, string source, DigitalEdge digitalEdge)
        {
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
        /// <param name="signalSource">The signal source to export, such as StartTrigger, MarkerEvent, or ReadyForStartEvent.</param>
        /// <param name="signalIdentifier">The identifier of the signal to export, such as Marker0. Use an empty string when the signal has no identifier.</param>
        /// <param name="outputTerminal">The terminal to export the signal to, such as PXI_Trig0.</param>
        public static void ExportSignal(this FgenSessionsBundle sessionsBundle, SignalSource signalSource, string signalIdentifier, string outputTerminal)
        {
            // var signal = (FgenHardwareSignal)Enum.Parse(typeof(FgenHardwareSignal), signalType, ignoreCase: true);
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.ExportSignal(signalSource, signalIdentifier ?? string.Empty, outputTerminal);
            });
        }

        /// <summary>
        /// Disables the Start Trigger.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
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
        /// <param name="triggerMode">The trigger mode to set, such as Continuous, Single, Stepped, or Burst.</param>
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