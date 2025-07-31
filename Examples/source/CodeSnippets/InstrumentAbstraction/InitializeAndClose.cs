using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using DAQmxTask = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose;
using DCPower = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower.InitializeAndClose;
using Digital = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using DMM = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM.InitializeAndClose;
using Fgen = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen.InitializeAndClose;
using Relay = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Relay.InitializeAndClose;
using Scope = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose;
using Sync = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Sync.InitializeAndClose;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to invoke the methods included in the InitializeAndClose class for each supported instrument type.
    /// These methods will only execute if based on the instruments defined in the pin map.
    /// For example, if there are no NI Scope instruments defined in the target pin map,
    /// then invoking the InstrumentAbstraction.Scope.InitializeAndClose.Initialize() method will simply return without an exception.
    /// This class and its methods are intended for example purposes only,
    /// and are therefore intentionally marked as internal to prevent them from being directly invoked from code outside of this project.from code outside of this project.
    /// </summary>
    internal static class InitializeAndClose
    {
        internal static void InitializingInstrumentSessions(ISemiconductorModuleContext tsmContext)
        {
            DCPower.Initialize(tsmContext, resetDevice: true);
            Digital.Initialize(tsmContext, resetDevice: true);
            DMM.Initialize(tsmContext, resetDevice: true);
            Fgen.Initialize(tsmContext, resetDevice: true);
            Relay.Initialize(tsmContext, resetDevice: true);
            Scope.Initialize(tsmContext, resetDevice: true);
            Sync.Initialize(tsmContext, resetDevice: true);
        }

        internal static void CreatingDAQmxTasks(ISemiconductorModuleContext tsmContext)
        {
            DAQmxTask.CreateDAQmxAIVoltageTasks(tsmContext);
            DAQmxTask.CreateDAQmxAOFunctionGenerationTasks(tsmContext);
            DAQmxTask.CreateDAQmxAOVoltageTasks(tsmContext);
            DAQmxTask.CreateDAQmxDITasks(tsmContext);
            DAQmxTask.CreateDAQmxDOTasks(tsmContext);
        }

        internal static void ResettingInstrumentSessions(ISemiconductorModuleContext tsmContext)
        {
            DCPower.Reset(tsmContext, resetDevice: true);
            Digital.Reset(tsmContext, resetDevice: true);
            DMM.Reset(tsmContext);
            Fgen.Reset(tsmContext, resetDevice: true);
            Relay.Reset(tsmContext);
            Scope.Reset(tsmContext, resetDevice: true);
            Sync.Reset(tsmContext);
        }

        internal static void ClosingInstrumentsSessions(ISemiconductorModuleContext tsmContext)
        {
            DCPower.Close(tsmContext, reset: true);
            Digital.Close(tsmContext, reset: true);
            DMM.Close(tsmContext, resetDevice: true);
            Fgen.Close(tsmContext, reset: true);
            Relay.Close(tsmContext, resetDevice: true);
            Scope.Close(tsmContext, reset: true);
            Sync.Close(tsmContext, resetDevice: true);
        }
        internal static void ClearingDAQmxTasks(ISemiconductorModuleContext tsmContext)
        {
            DAQmxTask.ClearDAQmxAIVoltageTasks(tsmContext);
            DAQmxTask.ClearDAQmxAOFunctionGenerationTasks(tsmContext);
            DAQmxTask.ClearDAQmxAOVoltageTasks(tsmContext);
            DAQmxTask.ClearDAQmxDITasks(tsmContext);
            DAQmxTask.ClearDAQmxDOTasks(tsmContext);
        }
    }
}