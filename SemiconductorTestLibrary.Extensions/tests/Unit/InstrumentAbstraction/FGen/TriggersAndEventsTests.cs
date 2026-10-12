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

        public static IEnumerable<object[]> ExportSignalValidTestData()
        {
            var pinMapFileNames = new[]
            {
                "FgenSingleInstrumentPerPin.pinmap",
                "FgenSingleInstrumentPerPinPerSite.pinmap"
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
                "FgenSingleInstrumentPerPinPerSite.pinmap"
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
                "FgenSingleInstrumentPerPinPerSite.pinmap"
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

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentPerPin_SharedPin.pinmap")]
        public void StandardWaveformConfigured_ConfigureAndDisableStartTriggerDigitalEdgeWithRisingEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var triggerLine = "PXI_Trig0";
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            ConfigureStandardWaveform(sessionsBundle, StandardWaveform.Sine);

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
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void StandardWaveformConfigured_ConfigureStartTriggerDigitalEdgeWithFallingEdge_ThrowsException(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen("A");
            var triggerLine = "PXI_Trig1";
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            ConfigureStandardWaveform(sessionsBundle, StandardWaveform.Sine);
            sessionsBundle.ConfigureTriggerMode(TriggerMode.Continuous);

            void ConfigureStartTriggerDigitalEdgeFalling()
            {
                sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Falling);
            }

            var exception = Assert.Throws<ArgumentException>(ConfigureStartTriggerDigitalEdgeFalling);
            Assert.Contains("Falling edge is not supported. Only the Rising edge is supported.", exception.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void DigitalEdgeAndStandardWaveformConfiguredThenInitiated_DisableStartTrigger_ThrowsException(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var triggerLine = "PXI_Trig0";
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            ConfigureStandardWaveform(sessionsBundle, StandardWaveform.Square);
            sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Rising);
            sessionsBundle.Initiate();

            void DisableStartTrigger() => sessionsBundle.DisableStartTrigger();

            var exception = Assert.Throws<NISemiconductorTestException>(DisableStartTrigger);
            Assert.Contains("The operation cannot be completed because the device is not configurable while it is generating a signal.", exception.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void DigitalEdgeAndStandardWaveformConfiguredThenInitiated_AbortAndDisableTrigger_Succeeds(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var triggerLine = "PXI_Trig0";
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            ConfigureStandardWaveform(sessionsBundle, StandardWaveform.Square);
            sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Rising);
            sessionsBundle.Initiate();

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
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void StandardWaveformConfigured_ConfigureAndDisableStartTriggerSoftwareEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            ConfigureStandardWaveform(sessionsBundle, StandardWaveform.Sine);

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
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void SoftwareEdgeAndStandardWaveformConfiguredThenInitiated_SendSoftwareEdgeAndDisableStartTrigger_ThrowsException(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            ConfigureStandardWaveform(sessionsBundle, StandardWaveform.Square);
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
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void SoftwareEdgeAndStandardWaveformConfiguredThenInitiated_SendSoftwareEdgeAndAbortAndDisableTrigger_Succeeds(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            ConfigureStandardWaveform(sessionsBundle, StandardWaveform.Square);
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
        [MemberData(nameof(ExportSignalValidTestData))]
        public void StandardWaveformConfigured_ExportSignalWithValidSignalSource_Succeeds(string pinMapFileName, SignalSource signalSource)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            ConfigureStandardWaveform(sessionsBundle, StandardWaveform.Square);

            sessionsBundle.ExportSignal(signalSource, string.Empty, "PXI_Trig0");
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [MemberData(nameof(ExportSignalInvalidTestData))]
        public void StandardWaveformConfigured_ExportSignalWithInvalidSignalSource_ThrowsException(string pinMapFileName, SignalSource signalSource)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            ConfigureStandardWaveform(sessionsBundle, StandardWaveform.Square);

            void ExportSignal() => sessionsBundle.ExportSignal(signalSource, string.Empty, "PXI_Trig0");

            var exception = Assert.Throws<ArgumentException>(ExportSignal);
            Assert.Contains("The signal source '" + signalSource.ToString() + "' is not supported.", exception.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void StandardWaveformConfigured_ConfigureValidTriggerMode_Succeeds(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            ConfigureStandardWaveform(sessionsBundle, StandardWaveform.Sine);

            sessionsBundle.ConfigureTriggerMode(TriggerMode.Continuous);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [MemberData(nameof(TriggerModeInvalidTestData))]
        public void StandardWaveformConfigured_ConfigureInvalidTriggerModes_ThrowsExceptioin(string pinMapFileName, TriggerMode triggerMode)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            ConfigureStandardWaveform(sessionsBundle, StandardWaveform.Sine);

            sessionsBundle.ConfigureTriggerMode(TriggerMode.Continuous);
            void ConfigureTriggerMode()
            {
                sessionsBundle.ConfigureTriggerMode(triggerMode);
                sessionsBundle.Commit();
            }

            var exception = Assert.Throws<NISemiconductorTestException>(ConfigureTriggerMode);
            Assert.Contains("Only continuous triggering mode is available in standard function output mode.", exception.Message);
        }

        private static void ConfigureStandardWaveform(FgenSessionsBundle sessionsBundle, StandardWaveform waveform)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.StandardWaveform.Configure(sessionInfo.AllChannelsString, waveform, 5, 1, 100, 0);
            });
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
    }
}
