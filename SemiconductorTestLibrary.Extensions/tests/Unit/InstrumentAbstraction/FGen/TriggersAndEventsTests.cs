using System;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Fgen
{
    [Collection("NonParallelizable")]
    public sealed class TriggersAndEventsTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(string pinMapFileName)
        {
            _tsmContext = CreateTSMContext(pinMapFileName);
            InitializeAndClose.Initialize(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            InitializeAndClose.Close(_tsmContext);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundle_ConfigureStartTriggerDigitalEdge_TriggerConfigured(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen("A");
            var triggerLine = "PXI_Trig0";

            sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Rising);

            sessionsBundle.Do(sessionInfo =>
            {
                var digitalEdge = sessionInfo.Session.Trigger.Start.DigitalEdge;
                Assert.Contains(triggerLine, digitalEdge.Source);
                Assert.Equal(DigitalEdge.Rising, digitalEdge.Edge);
            });
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundle_ConfigureStartTriggerDigitalEdgeFalling_EdgeIsFalling(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ConfigureStartTriggerDigitalEdge("PXI_Trig1", DigitalEdge.Falling);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(DigitalEdge.Falling, sessionInfo.Session.Trigger.Start.DigitalEdge.Edge);
            });
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentSharedAcrossPinsAndSites.pinmap")]
        public void InitializeBundleWithMultiplePins_ConfigureStartTriggerDigitalEdge_TriggerConfiguredOnAllSessions(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B", "SystemPin" });
            var triggerLine = "PXI_Trig0";

            sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Rising);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Contains(triggerLine, sessionInfo.Session.Trigger.Start.DigitalEdge.Source);
            });
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundle_ConfigureStartTriggerSoftwareEdge_DoesNotThrow(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ConfigureStartTriggerSoftwareEdge();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundle_SendSoftwareEdgeTriggerAfterConfigure_DoesNotThrow(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen("A");
            sessionsBundle.ConfigureStartTriggerSoftwareEdge();
            sessionsBundle.Do(sessionInfo => sessionInfo.Session.InitiateGeneration());

            sessionsBundle.SendSoftwareEdgeTrigger();

            sessionsBundle.Do(sessionInfo => sessionInfo.Session.AbortGeneration());
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void StartTriggerConfigured_DisableStartTrigger_DoesNotThrow(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen("A");
            sessionsBundle.ConfigureStartTriggerDigitalEdge("PXI_Trig0", DigitalEdge.Rising);

            sessionsBundle.DisableStartTrigger();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundle_ExportStartTriggerSignal_DoesNotThrow(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ExportSignal(SignalSource.StartTrigger, string.Empty, "PXI_Trig0");
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundle_ExportSignalWithNullIdentifier_DoesNotThrow(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ExportSignal(SignalSource.StartTrigger, null, "PXI_Trig1");
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", TriggerMode.Single)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", TriggerMode.Continuous)]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap", TriggerMode.Single)]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap", TriggerMode.Continuous)]
        public void InitializeBundle_ConfigureTriggerMode_TriggerModeSet(string pinMapFileName, TriggerMode triggerMode)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ConfigureTriggerMode(triggerMode);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(triggerMode, sessionInfo.Session.Trigger.GetTriggerMode(sessionInfo.AllChannelsString));
            });
        }
    }
}