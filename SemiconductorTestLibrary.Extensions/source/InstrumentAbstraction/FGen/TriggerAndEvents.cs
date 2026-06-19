using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Trigger and Events container class.
    /// </summary>
    public static class TriggerAndEvents
    {
        /// <summary>
        /// Configure start trigger Digital Edge.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="source">source</param>
        /// <param name="edgeType">edgeType</param>
        public static void ConfigureStartTriggerDigitalEdge(this FgenSessionsBundle sessionsBundle, string source, string edgeType)
        { }

        /// <summary>
        /// Configure Start Trigger Software Edge.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void ConfigureStartTriggerSoftwareEdge(this FgenSessionsBundle sessionsBundle)
        { }

        /// <summary>
        /// Disable Start trigger.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void DisableStartTrigger(this FgenSessionsBundle sessionsBundle)
        { }

        /// <summary>
        /// Send Software Edge trigger.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="triggerID">triggerID</param>
        public static void SendSoftwareEdgeTrigger(this FgenSessionsBundle sessionsBundle, string triggerID)
        { }

        /// <summary>
        /// Export signal.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="signalIdentifier">signalIdentifier</param>
        /// <param name="signalType">signalType</param>
        /// <param name="outputTerminal">outputTerminal</param>
        public static void ExportSignal(this FgenSessionsBundle sessionsBundle, string signalIdentifier, string signalType, string outputTerminal)
        { }
    }
}
