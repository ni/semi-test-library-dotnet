using System;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    [Collection("NonParallelizable")]
    public class TriggerAndEventTests
    {
        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureConditionalJumpTrigger_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            // Test Rising Edge
            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig0", 0);
            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig1", 1);
            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig2", 2);
            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig3", 3);
            sessionsBundle.Do(sessionInfo =>
            {
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 0, "PXI_Trig0", DigitalEdge.Rising);
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 1, "PXI_Trig1", DigitalEdge.Rising);
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 2, "PXI_Trig2", DigitalEdge.Rising);
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 3, "PXI_Trig3", DigitalEdge.Rising);
            });

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

            // Test None
            sessionsBundle.DisableConditionalJumpTriggers();
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].TriggerType);
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[1].TriggerType);
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[2].TriggerType);
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[3].TriggerType);
            });

            // Test Falling Edge
            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig0", 0, DigitalEdge.Falling);
            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig1", 1, DigitalEdge.Falling);
            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig2", 2, DigitalEdge.Falling);
            sessionsBundle.ConfigureConditionalJumpTriggerDigitalEdge("PXI_Trig3", 3, DigitalEdge.Falling);
            sessionsBundle.Do(sessionInfo =>
            {
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 0, "PXI_Trig0", DigitalEdge.Falling);
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 1, "PXI_Trig1", DigitalEdge.Falling);
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 2, "PXI_Trig2", DigitalEdge.Falling);
                AssertConditionalJumpTriggerDigitalEdge(sessionInfo, 3, "PXI_Trig3", DigitalEdge.Falling);
            });

            // Test None Again
            sessionsBundle.DisableConditionalJumpTrigger(0);
            sessionsBundle.DisableConditionalJumpTrigger(1);
            sessionsBundle.DisableConditionalJumpTrigger(2);
            sessionsBundle.DisableConditionalJumpTrigger(3);
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].TriggerType);
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[1].TriggerType);
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[2].TriggerType);
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[3].TriggerType);
            });

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureStartTrigger_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            // Test Rising Edge
            sessionsBundle.ConfigureStartTriggerDigitalEdge("PXI_Trig2");
            sessionsBundle.Do(sessionInfo =>
            {
                AssertStartTriggerDigitalEdge(sessionInfo, "PXI_Trig2", DigitalEdge.Rising);
            });

            // Test Software Edge
            sessionsBundle.ConfigureStartTriggerSoftwareEdge();
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.Software, sessionInfo.Session.Trigger.StartTrigger.TriggerType);
            });

            // Test None
            sessionsBundle.DisableStartTrigger();
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.StartTrigger.TriggerType);
            });

            // Test Falling Edge
            sessionsBundle.ConfigureStartTriggerDigitalEdge("PXI_Trig3", DigitalEdge.Falling);
            sessionsBundle.Do(sessionInfo =>
            {
                AssertStartTriggerDigitalEdge(sessionInfo, "PXI_Trig3", DigitalEdge.Falling);
            });

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ExportPatternOpcodeEvent_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var outputTerminal = "PXI_Trig0";

            // Test Exporting Pattern Opcode Event
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
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var outputTerminal = "PXI_Trig1";

            // Test Exporting Conditional Jump Trigger
            sessionsBundle.ExportSignal(SignalType.ConditionalJumpTrigger, $"conditionalJumpTrigger{2}", outputTerminal);
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.ConditionalJumpTriggers[2].OutputTerminal);
                Assert.Equal(string.Empty, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].OutputTerminal);
            });
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ExportStartTrigger_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var outputTerminal = "PXI_Trig2";

            // Test Start Trigger
            outputTerminal = "PXI_Trig2";
            sessionsBundle.ExportSignal(SignalType.StartTrigger, string.Empty, outputTerminal);
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.StartTrigger.OutputTerminal);
            });

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ExportSignal_TwoSignalsCanBeRoutedToSameOutputTerminal(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var outputTerminal = "PXI_Trig2";

            // Export Conditional Jump Trigger
            sessionsBundle.ExportSignal(SignalType.ConditionalJumpTrigger, $"conditionalJumpTrigger{0}", outputTerminal);
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].OutputTerminal);
            });

            // Export Start Trigger but use an already exported terminal.
            sessionsBundle.ExportSignal(SignalType.StartTrigger, string.Empty, outputTerminal);
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].OutputTerminal);
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.StartTrigger.OutputTerminal);
            });

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_DisableStartTrigger_OutputTerminalUntouched(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var outputTerminal = "PXI_Trig2";

            // Export Start Trigger
            sessionsBundle.ExportSignal(SignalType.StartTrigger, string.Empty, outputTerminal);
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.StartTrigger.OutputTerminal);
            });

            // Test Clearing of Start Trigger, this should not change the OutputTerminal property.
            sessionsBundle.DisableStartTrigger();
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(outputTerminal, sessionInfo.Session.Trigger.StartTrigger.OutputTerminal);
            });

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureAndDisableStartTriggerSoftwareEdge_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            // Test default property value
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.StartTrigger.TriggerType);
            });

            // Export Start Trigger & confirm property changes.
            sessionsBundle.ConfigureStartTriggerSoftwareEdge();
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.Software, sessionInfo.Session.Trigger.StartTrigger.TriggerType);
            });

            // Test Clearing of Start Trigger, this should not change the OutputTerminal property.
            sessionsBundle.DisableStartTrigger();
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.StartTrigger.TriggerType);
            });

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureAndDisableConditionalJumpTriggerSoftwareEdge_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            // Test default property value
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.StartTrigger.TriggerType);
            });

            // Export Start Trigger & confirm property changes.
            sessionsBundle.ConfigureConditionalJumpTriggerSoftwareEdge(0);
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.Software, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].TriggerType);
            });

            // Test Clearing of Start Trigger, this should not change the OutputTerminal property.
            sessionsBundle.DisableConditionalJumpTrigger(0);
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TriggerType.None, sessionInfo.Session.Trigger.ConditionalJumpTriggers[0].TriggerType);
            });

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureStartTriggerSoftwareEdgeAndSendSoftwareEdg_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            sessionsBundle.ConfigureStartTriggerSoftwareEdge();
            sessionsBundle.SendSoftwareEdgeStartTrigger();

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureConditionalJumpTriggerSoftwareEdgeAndSendSoftwareEdge_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });

            sessionsBundle.ConfigureConditionalJumpTriggerSoftwareEdge(2);
            sessionsBundle.SendSoftwareEdgeConditionalJumpTrigger(2);

            Close(tsmContext);
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
