using System;
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

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void StandardWaveformConfigured_ConfigureAndDisableStartTriggerDigitalEdgeWithRisingEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var triggerLine = "PXI_Trig0";
            var waveformSettings = new StandardWaveformSettings(StandardWaveform.Sine, 0.5, 0.5, 0.0);
            sessionsBundle.ConfigureStandardWaveform(waveformSettings);

            // Test Digital Edge Trigger - Rising
            sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Rising);

            sessionsBundle.Do(sessionInfo =>
            {
                var digitalEdge = sessionInfo.Session.Trigger.Start.DigitalEdge;
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
            var waveformSettings = new StandardWaveformSettings(StandardWaveform.Sine, 0.5, 0.5, 0.0);
            sessionsBundle.ConfigureStandardWaveform(waveformSettings);
            sessionsBundle.ConfigureTriggerMode(TriggerMode.Continuous);

            void ConfigureStartTriggerDigitalEdgeFalling() => sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Falling);

            var exception = Assert.Throws<NISemiconductorTestException>(ConfigureStartTriggerDigitalEdgeFalling);
            // Assert.IsType<ArgumentException>(exception);
            // Assert.Contains("'Falling' is not supported. Only the Rising edge is supported.", exception.Message);
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
            var waveformSettings = new StandardWaveformSettings(StandardWaveform.Square, 0.5, 0.5, 0.0);
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            sessionsBundle.ConfigureStandardWaveform(waveformSettings);
            sessionsBundle.ConfigureStartTriggerDigitalEdge(triggerLine, DigitalEdge.Rising);
            sessionsBundle.Initiate();

            void DisableStartTrigger() => sessionsBundle.DisableStartTrigger();

            var exception = Assert.Throws<NISemiconductorTestException>(DisableStartTrigger);
            Assert.IsType<NISemiconductorTestException>(exception);
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
            var waveformSettings = new StandardWaveformSettings(StandardWaveform.Square, 0.5, 0.5, 0.0);
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            sessionsBundle.ConfigureStandardWaveform(waveformSettings);
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
            var waveformSettings = new StandardWaveformSettings(StandardWaveform.Sine, 0.5, 0.5, 0.0);
            sessionsBundle.ConfigureStandardWaveform(waveformSettings);

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
            var waveformSettings = new StandardWaveformSettings(StandardWaveform.Square, 0.5, 0.5, 0.0);
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            sessionsBundle.ConfigureStandardWaveform(waveformSettings);
            sessionsBundle.ConfigureStartTriggerSoftwareEdge();
            sessionsBundle.Initiate();

            sessionsBundle.SendSoftwareEdgeTrigger();
            void DisableStartTrigger() => sessionsBundle.DisableStartTrigger();

            var exception = Assert.Throws<NISemiconductorTestException>(DisableStartTrigger);
            Assert.IsType<NISemiconductorTestException>(exception);
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
            var waveformSettings = new StandardWaveformSettings(StandardWaveform.Square, 0.5, 0.5, 0.0);
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            sessionsBundle.ConfigureStandardWaveform(waveformSettings);
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
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundle_ExportStartTriggerSignal_DoesNotThrow(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ExportSignal(SignalSource.DataMarkerEvent, string.Empty, "PXI_Trig0");
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