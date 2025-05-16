using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples
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
            SemiconductorTestLibrary.InstrumentAbstraction.DCPower.InitializeAndClose.Initialize(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose.Initialize(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.DMM.InitializeAndClose.Initialize(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Fgen.InitializeAndClose.Initialize(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Relay.InitializeAndClose.Initialize(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose.Initialize(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Sync.InitializeAndClose.Initialize(tsmContext, resetDevice: true);
        }

        internal static void CreatingDAQmxTasks(ISemiconductorModuleContext tsmContext)
        {
            SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose.CreateDAQmxAIVoltageTasks(tsmContext);
            SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose.CreateDAQmxAOFunctionGenerationTasks(tsmContext);
            SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose.CreateDAQmxAOVoltageTasks(tsmContext);
            SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose.CreateDAQmxDITasks(tsmContext);
            SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose.CreateDAQmxDOTasks(tsmContext);
        }

        internal static void ResettingInstrumentSessions(ISemiconductorModuleContext tsmContext)
        {
            SemiconductorTestLibrary.InstrumentAbstraction.DCPower.InitializeAndClose.Reset(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose.Reset(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.DMM.InitializeAndClose.Reset(tsmContext);
            SemiconductorTestLibrary.InstrumentAbstraction.Fgen.InitializeAndClose.Reset(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Relay.InitializeAndClose.Reset(tsmContext);
            SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose.Reset(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Sync.InitializeAndClose.Reset(tsmContext);
        }

        internal static void ClosingInstrumentsSessions(ISemiconductorModuleContext tsmContext)
        {
            SemiconductorTestLibrary.InstrumentAbstraction.DCPower.InitializeAndClose.Close(tsmContext, reset: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose.Close(tsmContext, reset: true);
            SemiconductorTestLibrary.InstrumentAbstraction.DMM.InitializeAndClose.Close(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Fgen.InitializeAndClose.Close(tsmContext, reset: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Relay.InitializeAndClose.Close(tsmContext, resetDevice: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose.Close(tsmContext, reset: true);
            SemiconductorTestLibrary.InstrumentAbstraction.Sync.InitializeAndClose.Close(tsmContext, resetDevice: true);
        }
        internal static void ClearingDAQmxTasks(ISemiconductorModuleContext tsmContext)
        {
            SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose.ClearDAQmxAIVoltageTasks(tsmContext);
            SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose.ClearDAQmxAOFunctionGenerationTasks(tsmContext);
            SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose.ClearDAQmxAOVoltageTasks(tsmContext);
            SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose.ClearDAQmxDITasks(tsmContext);
            SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose.ClearDAQmxDOTasks(tsmContext);
        }
    }
}