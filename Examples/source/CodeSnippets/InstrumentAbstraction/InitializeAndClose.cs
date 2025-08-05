using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using DAQmxInitializeAndClose = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose;
using DCPowerInitializeAndClose = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower.InitializeAndClose;
using DigitalInitializeAndClose = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using DMMInitializeAndClose = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM.InitializeAndClose;
using FgenInitializeAndClose = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen.InitializeAndClose;
using RelayInitializeAndClose = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Relay.InitializeAndClose;
using ScopeInitializeAndClose = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose;
using SyncInitializeAndClose = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Sync.InitializeAndClose;

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
            DCPowerInitializeAndClose.Initialize(tsmContext, resetDevice: true);
            DigitalInitializeAndClose.Initialize(tsmContext, resetDevice: true);
            DMMInitializeAndClose.Initialize(tsmContext, resetDevice: true);
            FgenInitializeAndClose.Initialize(tsmContext, resetDevice: true);
            RelayInitializeAndClose.Initialize(tsmContext, resetDevice: true);
            ScopeInitializeAndClose.Initialize(tsmContext, resetDevice: true);
            SyncInitializeAndClose.Initialize(tsmContext, resetDevice: true);
        }

        internal static void CreatingDAQmxTasks(ISemiconductorModuleContext tsmContext)
        {
            DAQmxInitializeAndClose.CreateDAQmxAIVoltageTasks(tsmContext);
            DAQmxInitializeAndClose.CreateDAQmxAOFunctionGenerationTasks(tsmContext);
            DAQmxInitializeAndClose.CreateDAQmxAOVoltageTasks(tsmContext);
            DAQmxInitializeAndClose.CreateDAQmxDITasks(tsmContext);
            DAQmxInitializeAndClose.CreateDAQmxDOTasks(tsmContext);
        }

        internal static void ResettingInstrumentSessions(ISemiconductorModuleContext tsmContext)
        {
            DCPowerInitializeAndClose.Reset(tsmContext, resetDevice: true);
            DigitalInitializeAndClose.Reset(tsmContext, resetDevice: true);
            DMMInitializeAndClose.Reset(tsmContext);
            FgenInitializeAndClose.Reset(tsmContext, resetDevice: true);
            RelayInitializeAndClose.Reset(tsmContext);
            ScopeInitializeAndClose.Reset(tsmContext, resetDevice: true);
            SyncInitializeAndClose.Reset(tsmContext);
        }

        internal static void ClosingInstrumentsSessions(ISemiconductorModuleContext tsmContext)
        {
            DCPowerInitializeAndClose.Close(tsmContext, reset: true);
            DigitalInitializeAndClose.Close(tsmContext, reset: true);
            DMMInitializeAndClose.Close(tsmContext, resetDevice: true);
            FgenInitializeAndClose.Close(tsmContext, reset: true);
            RelayInitializeAndClose.Close(tsmContext, resetDevice: true);
            ScopeInitializeAndClose.Close(tsmContext, reset: true);
            SyncInitializeAndClose.Close(tsmContext, resetDevice: true);
        }
        internal static void ClearingDAQmxTasks(ISemiconductorModuleContext tsmContext)
        {
            DAQmxInitializeAndClose.ClearDAQmxAIVoltageTasks(tsmContext);
            DAQmxInitializeAndClose.ClearDAQmxAOFunctionGenerationTasks(tsmContext);
            DAQmxInitializeAndClose.ClearDAQmxAOVoltageTasks(tsmContext);
            DAQmxInitializeAndClose.ClearDAQmxDITasks(tsmContext);
            DAQmxInitializeAndClose.ClearDAQmxDOTasks(tsmContext);
        }
    }
}