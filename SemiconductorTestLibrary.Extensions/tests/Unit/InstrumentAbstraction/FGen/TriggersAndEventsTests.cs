using System;
using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Fgen
{
    [Collection("NonParallelizable")]
    public sealed class TriggersAndEventsTests : IDisposable
    {
        #region Initialization and Cleanup

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

        #endregion

        #region Test Data

        public static IEnumerable<object[]> ExportSignalValidTestData()
        {
            var pinMapFileNames = new[]
            {
                "FgenSingleInstrumentPerPin.pinmap",
                "FgenSingleInstrumentPerSite.pinmap"
            };
            var signalSources = new[]
            {
                SignalSource.StartTrigger,
                SignalSource.ReadyForStartEvent,
                SignalSource.StartedEvent,
                SignalSource.DoneEvent
            };

            foreach (var pinMapFileName in pinMapFileNames)
            {
                foreach (var signalSource in signalSources)
                {
                    yield return new object[] { pinMapFileName, signalSource };
                }
            }
        }

        public static IEnumerable<object[]> ExportSignalInvalidTestData()
        {
            var pinMapFileNames = new[]
            {
                "FgenSingleInstrumentPerPin.pinmap",
                "FgenSingleInstrumentPerSite.pinmap"
            };
            var signalSources = new[]
            {
                SignalSource.SampleClock,
                SignalSource.SampleClockTimebase,
                SignalSource.ReferenceClock,
                SignalSource.DataMarkerEvent,
                SignalSource.MarkerEvent,
                SignalSource.ScriptTrigger,
                SignalSource.OnboardReferenceClock,
                SignalSource.SyncOut,
            };

            foreach (var pinMapFileName in pinMapFileNames)
            {
                foreach (var signalSource in signalSources)
                {
                    yield return new object[] { pinMapFileName, signalSource };
                }
            }
        }

        public static IEnumerable<object[]> TriggerModeInvalidTestData()
        {
            var pinMapFileNames = new[]
            {
                "FgenSingleInstrumentPerPin.pinmap",
                "FgenSingleInstrumentPerSite.pinmap"
            };
            var triggerModes = new[]
            {
                TriggerMode.Single,
                TriggerMode.Burst,
                TriggerMode.Stepped
            };

            foreach (var pinMapFileName in pinMapFileNames)
            {
                foreach (var triggerMode in triggerModes)
                {
                    yield return new object[] { pinMapFileName, triggerMode };
                }
            }
        }

        #endregion

        #region Digital Edge Start Trigger Tests

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void StandardWaveformConfigured_ConfigureAndDisableStartTriggerDigitalEdgeWithRisingEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var triggerLine = "PXI_Trig0";
            ConfigureSettings(sessionsBundle, StandardWaveform.Sine);

            // Test Digital Edge Trigger - Rising
            sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Rising);
            sessionsBundle.Commit();

            sessionsBundle.Do(sessionInfo =>
            {
                AssertStartTriggerSettings(sessionInfo, TriggerType.DigitalEdge, triggerLine, DigitalEdge.Rising);
            });

            // Test Disable Trigger
            sessionsBundle.DisableStartTrigger();

            sessionsBundle.Do(sessionInfo =>
            {
                AssertStartTriggerSettings(sessionInfo, TriggerType.None);
            });
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void StandardWaveformConfigured_ConfigureStartTriggerDigitalEdgeWithFallingEdge_ThrowsException(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var triggerLine = "PXI_Trig1";
            ConfigureSettings(sessionsBundle, StandardWaveform.Sine);

            void ConfigureStartTriggerDigitalEdgeFalling()
            {
                sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Falling);
                sessionsBundle.Commit();
            }

            var exception = Assert.Throws<NISemiconductorTestException>(ConfigureStartTriggerDigitalEdgeFalling);
            Assert.Contains("Invalid Value: Falling Edge", exception.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void StandardWaveformConfigured_ConfigureStartTriggerDigitalEdgeWithNullOrEmptySource_ThrowsException(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            ConfigureSettings(sessionsBundle, StandardWaveform.Sine);

            // Test with null trigger source
            string triggerLine = null;
            void ConfigureStartTriggerDigitalEdge()
            {
                sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Rising);
                sessionsBundle.Commit();
            }

            var exception = Assert.Throws<ArgumentNullException>(ConfigureStartTriggerDigitalEdge);
            Assert.Contains("Trigger source passed as input is either null or an empty string. Please provide valid trigger source.", exception.Message);

            // Test with empty trigger source
            triggerLine = string.Empty;

            var argumentException = Assert.Throws<ArgumentException>(ConfigureStartTriggerDigitalEdge);
            Assert.Contains("Trigger source passed as input is either null or an empty string. Please provide valid trigger source.", argumentException.Message);
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void StandardWaveformConfigured_ConfigureStartTriggerDigitalEdgeWithInvalidSource_ThrowsException(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var triggerLine = "PXITri";
            ConfigureSettings(sessionsBundle, StandardWaveform.Sine);
            sessionsBundle.ConfigureTriggerMode(TriggerMode.Continuous);

            void ConfigureStartTriggerDigitalEdgeFalling()
            {
                sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Rising);
                sessionsBundle.Commit();
            }

            var exception = Assert.Throws<NISemiconductorTestException>(ConfigureStartTriggerDigitalEdgeFalling);
            Assert.Contains("Terminal for the device is invalid", exception.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void DigitalEdgeAndStandardWaveformConfiguredThenInitiated_DisableStartTrigger_ThrowsException(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var triggerLine = "PXI_Trig0";
            ConfigureSettings(sessionsBundle, StandardWaveform.Square);
            sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Rising);
            sessionsBundle.Initiate();

            void DisableStartTrigger() => sessionsBundle.DisableStartTrigger();

            var exception = Assert.Throws<NISemiconductorTestException>(DisableStartTrigger);
            Assert.Contains("The operation cannot be completed because the device is not configurable while it is generating a signal.", exception.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void DigitalEdgeAndStandardWaveformConfiguredThenInitiated_AbortAndDisableTrigger_Succeeds(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var triggerLine = "PXI_Trig0";
            ConfigureSettings(sessionsBundle, StandardWaveform.Square);
            sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Rising);
            sessionsBundle.Initiate();

            sessionsBundle.Abort();
            sessionsBundle.DisableStartTrigger();

            sessionsBundle.Do(sessionInfo =>
            {
                AssertStartTriggerSettings(sessionInfo, TriggerType.None);
            });
        }

        #endregion

        #region Software Edge Start Trigger Tests

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void StandardWaveformConfigured_ConfigureAndDisableStartTriggerSoftwareEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            ConfigureSettings(sessionsBundle, StandardWaveform.Sine);

            // Test Software Edge Trigger
            sessionsBundle.ConfigureStartTriggerSoftwareEdge();

            sessionsBundle.Do(sessionInfo =>
            {
                AssertStartTriggerSettings(sessionInfo, TriggerType.SoftwareEdge);
            });

            // Test Disable Trigger
            sessionsBundle.DisableStartTrigger();

            sessionsBundle.Do(sessionInfo =>
            {
                AssertStartTriggerSettings(sessionInfo, TriggerType.None);
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void SoftwareEdgeAndStandardWaveformConfiguredThenInitiated_SendSoftwareEdgeAndDisableStartTrigger_ThrowsException(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            ConfigureSettings(sessionsBundle, StandardWaveform.Square);
            sessionsBundle.ConfigureStartTriggerSoftwareEdge();
            sessionsBundle.Initiate();

            sessionsBundle.SendSoftwareEdgeTrigger();
            void DisableStartTrigger() => sessionsBundle.DisableStartTrigger();

            var exception = Assert.Throws<NISemiconductorTestException>(DisableStartTrigger);
            Assert.Contains("The operation cannot be completed because the device is not configurable while it is generating a signal.", exception.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void SoftwareEdgeAndStandardWaveformConfiguredThenInitiated_SendSoftwareEdgeAndAbortAndDisableTrigger_Succeeds(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            ConfigureSettings(sessionsBundle, StandardWaveform.Square);
            sessionsBundle.ConfigureStartTriggerSoftwareEdge();
            sessionsBundle.Initiate();

            sessionsBundle.SendSoftwareEdgeTrigger();
            sessionsBundle.Abort();
            sessionsBundle.DisableStartTrigger();

            sessionsBundle.Do(sessionInfo =>
            {
                AssertStartTriggerSettings(sessionInfo, TriggerType.None);
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void SoftwareEdgeAndStandardWaveformConfigured_SendSoftwareEdgeBeforeInitiate_ThrowsException(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            ConfigureSettings(sessionsBundle, StandardWaveform.Square);
            sessionsBundle.ConfigureStartTriggerSoftwareEdge();

            void SendSoftwareEdgeAndInitiate() => sessionsBundle.SendSoftwareEdgeTrigger();

            var exception = Assert.Throws<NISemiconductorTestException>(SendSoftwareEdgeAndInitiate);
            Assert.Contains("Specified operation can be performed only when the task is running.", exception.Message);
        }

        #endregion

        #region Export Signal Tests

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [MemberData(nameof(ExportSignalValidTestData))]
        public void StandardWaveformConfigured_ExportSignalWithValidSignalSource_Succeeds(string pinMapFileName, SignalSource signalSource)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var triggerLine = "PXI_Trig0";
            ConfigureSettings(sessionsBundle, StandardWaveform.Square);

            sessionsBundle.ExportSignal(signalSource, triggerLine);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [MemberData(nameof(ExportSignalInvalidTestData))]
        public void StandardWaveformConfigured_ExportSignalWithInvalidSignalSource_ThrowsException(string pinMapFileName, SignalSource signalSource)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            ConfigureSettings(sessionsBundle, StandardWaveform.Square);

            void ExportSignal() => sessionsBundle.ExportSignal(signalSource, "PXI_Trig0");

            var exception = Assert.Throws<ArgumentException>(ExportSignal);
            Assert.Contains("The signal source '" + signalSource.ToString() + "' is not supported.", exception.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [MemberData(nameof(ExportSignalValidTestData))]
        public void StandardWaveformConfigured_ExportSignalWithValidSignalSourceAndNullOrEmptyOutputTerminal_ThrowsException(string pinMapFileName, SignalSource signalSource)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            ConfigureSettings(sessionsBundle, StandardWaveform.Square);

            // Test with null output terminal
            string outputTerminal = null;
            void ExportSignal() => sessionsBundle.ExportSignal(signalSource, outputTerminal);

            var exception = Assert.Throws<ArgumentNullException>(ExportSignal);
            Assert.Contains("Output terminal passed as input is either null or an empty string. Please provide valid output terminal.", exception.Message);

            // Test with empty output terminal
            outputTerminal = string.Empty;

            var argumentException = Assert.Throws<ArgumentException>(ExportSignal);
            Assert.Contains("Output terminal passed as input is either null or an empty string. Please provide valid output terminal.", argumentException.Message);
        }

        #endregion

        #region Trigger Mode Tests

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void StandardWaveformConfigured_ConfigureValidTriggerMode_Succeeds(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            ConfigureSettings(sessionsBundle, StandardWaveform.Sine);

            sessionsBundle.ConfigureTriggerMode(TriggerMode.Continuous);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [MemberData(nameof(TriggerModeInvalidTestData))]
        public void StandardWaveformConfigured_ConfigureInvalidTriggerModes_ThrowsException(string pinMapFileName, TriggerMode triggerMode)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            ConfigureSettings(sessionsBundle, StandardWaveform.Sine);

            void ConfigureTriggerMode()
            {
                sessionsBundle.ConfigureTriggerMode(triggerMode);
                sessionsBundle.Commit();
            }

            var exception = Assert.Throws<NISemiconductorTestException>(ConfigureTriggerMode);
            Assert.Contains("Only continuous triggering mode is available in standard function output mode.", exception.Message);
        }

        #endregion

        #region Helper Methods

        private static void ConfigureSettings(FgenSessionsBundle sessionsBundle, StandardWaveform waveform)
        {
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var standarWaveformSettings = new StandardWaveformSettings(waveform, 1000, 5, 1, 0);
            sessionsBundle.ConfigureStandardWaveform(standarWaveformSettings);
            sessionsBundle.ConfigureTriggerMode(TriggerMode.Continuous);
        }

        private void AssertStartTriggerSettings(FgenSessionInformation sessionInfo, TriggerType expectedType, string expectedStartTriggerSource = "", DigitalEdge expectedEdge = DigitalEdge.Rising)
        {
            Assert.Equal(expectedType, sessionInfo.Session.Trigger.Start.TriggerType);
            if (expectedType == TriggerType.DigitalEdge)
            {
                Assert.Equal(expectedStartTriggerSource, sessionInfo.Session.Trigger.Start.DigitalEdge.Source);
                Assert.Equal(expectedEdge, sessionInfo.Session.Trigger.Start.DigitalEdge.Edge);
            }
        }

        #endregion
    }
}
