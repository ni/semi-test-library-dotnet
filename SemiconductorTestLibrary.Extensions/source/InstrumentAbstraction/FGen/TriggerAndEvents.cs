using System;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    internal class TriggerAndEvents
    {
        public static void ConfigureStartTriggerDigitalEdge(string source, string edgeType)
        { }

        public static void ConfigureStartTriggerSoftwareEdge()
        { }

        public static void DisableStartTrigger()
        { }

        public static void SendSoftwareEdgeTrigger(string triggerID)
        { }

        public static void ExportSignal(string signalIdentifier, string signalType, string outputTerminal)
        { }
    }
}
