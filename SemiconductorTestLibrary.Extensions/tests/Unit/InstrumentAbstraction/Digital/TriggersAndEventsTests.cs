using System;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    [Collection("NonParallelizable")]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
    public sealed class TriggerAndEventTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager InitializeSessionsAndCreateSessionManager(string pinMapFileName, string digitalPatternProjectFileName)
        {
            _tsmContext = CreateTSMContext(pinMapFileName, digitalPatternProjectFileName);
            Initialize(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", DigitalEdge.Rising)]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", DigitalEdge.Falling)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", DigitalEdge.Rising)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", DigitalEdge.Falling)]
        public void SessionsInitialized_ConfigureConditionalJumpTrigger_ValuesCanBeReadBack(string pinMap, string digitalProject, DigitalEdge digitalEdge)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig0", 0, digitalEdge);
            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig1", 1, digitalEdge);
            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig2", 2, digitalEdge);
            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig3", 3, digitalEdge);

            sessionsBundle.Do(sessionInfo =>
            {
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 0, "PXI_Trig0", digitalEdge);
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 1, "PXI_Trig1", digitalEdge);
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 2, "PXI_Trig2", digitalEdge);
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 3, "PXI_Trig3", digitalEdge);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureConditionalJumpTriggerSoftwareEdge_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            // Test Software Edge
            sessionsBundle.ConfigureConditionalJumpTriggerSoftwareEdge(0);
            sessionsBundle.ConfigureConditionalJumpTriggerSoftwareEdge(1);
            sessionsBundle.ConfigureConditionalJumpTriggerSoftwareEdge(2);
            sessionsBundle.ConfigureConditionalJumpTriggerSoftwareEdge(3);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.Software, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].TriggerType);
                Assert.Equal(TriggerType.Software, sessionInfo.Session.Trigger.ConditionalJumpTriggers[1].TriggerType);
                Assert.Equal(TriggerType.Software, sessionInfo.Session.Trigger.ConditionalJumpTriggers[2].TriggerType);
                Assert.Equal(TriggerType.Software, sessionInfo.Session.Trigger.ConditionalJumpTriggers[3].TriggerType);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_DisableConditionalJumpTriggers_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            // Test None
            sessionsBundle.DisableConditionalJumpTriggers();

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].TriggerType);
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[1].TriggerType);
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[2].TriggerType);
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[3].TriggerType);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", DigitalEdge.Rising)]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", DigitalEdge.Falling)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", DigitalEdge.Rising)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", DigitalEdge.Falling)]
        public void SessionsInitialized_ConfigureStartTriggerDigitalEdge_ValuesCanBeReadBack(string pinMap, string digitalProject, DigitalEdge digitalEdge)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            sessionsBundle.ConfigureStartTriggerDigitalEdge("PXI_Trig2", digitalEdge);

            sessionsBundle.Do(sessionInfo =>
            {
                AssertStartTriggerDigitalEdge(sessionInfo, "PXI_Trig2", digitalEdge);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureStartTriggerSoftwareEdge_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            // Test Software Edge
            sessionsBundle.ConfigureStartTriggerSoftwareEdge();

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.Software, sessionInfo.Session.Trigger.StartTrigger.TriggerType);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_DisableStartTrigger_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            // Test None
            sessionsBundle.DisableStartTrigger();

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.StartTrigger.TriggerType);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ExportPatternOpcodeEvent_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var outputTerminal = "PXI_Trig0";
            sessionsBundle.ExportSignal(SignalType.PatternOpcodeEvent, $"patternOpcodeEvent{1}", outputTerminal);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(outputTerminal, sessionInfo.Session.Event.PatternOpcodeEvents[1].OutputTerminal);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ExportConditionalJumpTrigger_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var outputTerminal = "PXI_Trig1";
            sessionsBundle.ExportSignal(SignalType.ConditionalJumpTrigger, $"conditionalJumpTrigger{2}", outputTerminal);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.ConditionalJumpTriggers[2].OutputTerminal);
                Assert.Equal(string.Empty, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].OutputTerminal);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ExportStartTrigger_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var outputTerminal = "PXI_Trig2";
            outputTerminal = "PXI_Trig2";
            sessionsBundle.ExportSignal(SignalType.StartTrigger, string.Empty, outputTerminal);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.StartTrigger.OutputTerminal);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ExportTwoSignalsToTheSameOutputTerminal_Succeeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var outputTerminal = "PXI_Trig2";
            sessionsBundle.ExportSignal(SignalType.ConditionalJumpTrigger, $"conditionalJumpTrigger{0}", outputTerminal);
            sessionsBundle.ExportSignal(SignalType.StartTrigger, string.Empty, outputTerminal);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].OutputTerminal);
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.StartTrigger.OutputTerminal);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void StartTriggerExported_DisableStartTrigger_OutputTerminalUntouched(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var outputTerminal = "PXI_Trig2";
            sessionsBundle.ExportSignal(SignalType.StartTrigger, string.Empty, outputTerminal);

            // Test Clearing of Start Trigger, this should not change the OutputTerminal property.
            sessionsBundle.DisableStartTrigger();

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.StartTrigger.OutputTerminal);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureAndDisableStartTriggerSoftwareEdge_Succeeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            // Test default property value
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.StartTrigger.TriggerType);
            });

            sessionsBundle.ConfigureStartTriggerSoftwareEdge();
            sessionsBundle.DisableStartTrigger();

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.StartTrigger.TriggerType);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureAndDisableConditionalJumpTriggerSoftwareEdge_Succeeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            // Test default property value
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].TriggerType);
            });

            sessionsBundle.ConfigureConditionalJumpTriggerSoftwareEdge(0);
            sessionsBundle.DisableConditionalJumpTrigger(0);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].TriggerType);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureStartTriggerSoftwareEdgeAndSendSoftwareEdge_Succeeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureStartTriggerSoftwareEdge();
            sessionsBundle.SendSoftwareEdgeStartTrigger();
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureConditionalJumpTriggerSoftwareEdgeAndSendSoftwareEdge_Succeeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureConditionalJumpTriggerSoftwareEdge(2);
            sessionsBundle.SendSoftwareEdgeConditionalJumpTrigger(2);
        }

        #region Private Methods
        private void AssertConditionalJumpTriggerDigitalEdge(DigitalSessionInformation sessionInfo, int index, string expectedSource, DigitalEdge expectedDigitalEdge)
        {
            Assert.Equal(TriggerType.DigitalEdge, sessionInfo.Session.Trigger.ConditionalJumpTriggers[index].TriggerType);
            Assert.Equal(expectedSource, sessionInfo.Session.Trigger.ConditionalJumpTriggers[index].DigitalEdge.Source);
            Assert.Equal(expectedDigitalEdge, sessionInfo.Session.Trigger.ConditionalJumpTriggers[index].DigitalEdge.Edge);
        }

        private void AssertStartTriggerDigitalEdge(DigitalSessionInformation sessionInfo, string expectedSource, DigitalEdge expectedDigitalEdge)
        {
            Assert.Equal(TriggerType.DigitalEdge, sessionInfo.Session.Trigger.StartTrigger.TriggerType);
            Assert.Equal(expectedSource, sessionInfo.Session.Trigger.StartTrigger.DigitalEdge.Source);
            Assert.Equal(expectedDigitalEdge, sessionInfo.Session.Trigger.StartTrigger.DigitalEdge.Edge);
        }

        #endregion Private Methods
    }
}
