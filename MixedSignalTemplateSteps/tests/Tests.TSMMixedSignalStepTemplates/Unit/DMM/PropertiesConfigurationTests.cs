using System;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DMM;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DMM.InitializeAndClose;
using static NationalInstruments.Tests.Utilities.TSMContext;

namespace NationalInstruments.Tests.TSMMixedSignalStepTemplates.Unit.DMM
{
    [Collection("NonParallelizable")]
    public sealed class PropertiesConfigurationTests : IDisposable
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        private readonly TSMSessionManager _sessionManager;

        public PropertiesConfigurationTests()
        {
            _tsmContext = CreateTSMContext("DMMTests.pinmap");
            _sessionManager = new TSMSessionManager(_tsmContext);
            Initialize(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        [Fact]
        public void ConfigureACBandwidth_ValuesConfigured()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle(new string[] { "DUTPin", "SystemPin" });
            sessionsBundle.ConfigureACBandwidth(minimumFrequency: 100, maximumFrequency: 10000);

            foreach (var sessionInfo in sessionsBundle.InstrumentSessions)
            {
                Assert.Equal(100, sessionInfo.Session.AC.FrequencyMin);
                // It seems the maximum frequency is not configurable, it's fixed at 100kHz for 4065 and 300kHz for 4070.
                // Assert.Equal(1000, sessionInfo.Session.AC.FrequencyMax);
            }
        }

        [Fact]
        public void ConfigureApertureTimeInSeconds_ValuesConfigured()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle(new string[] { "DUTPin", "SystemPin" });
            sessionsBundle.ConfigureApertureTime(DmmApertureTimeUnits.Seconds, 0.1);

            foreach (var sessionInfo in sessionsBundle.InstrumentSessions)
            {
                Assert.Equal(DmmApertureTimeUnits.Seconds, sessionInfo.Session.Advanced.ApertureTimeUnits);
                Assert.Equal(0.1, sessionInfo.Session.Advanced.ApertureTime);
            }
        }

        [Fact]
        public void ConfigureApertureTimeInPowerLineCycles_ValuesConfigured()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle(new string[] { "DUTPin", "SystemPin" });
            sessionsBundle.ConfigureApertureTime(DmmApertureTimeUnits.PowerLineCycles, 2);

            foreach (var sessionInfo in sessionsBundle.InstrumentSessions)
            {
                Assert.Equal(DmmApertureTimeUnits.PowerLineCycles, sessionInfo.Session.Advanced.ApertureTimeUnits);
                Assert.Equal(2, sessionInfo.Session.Advanced.ApertureTime);
            }
        }

        [Fact]
        public void ConfigureMeasurementAbsolute_ValuesConfigured()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle(new string[] { "DUTPin", "SystemPin" });
            sessionsBundle.ConfigureMeasurementAbsolute(DmmMeasurementFunction.DCCurrent, 0.2, 0.001);

            foreach (var sessionInfo in sessionsBundle.InstrumentSessions)
            {
                Assert.Equal(DmmMeasurementFunction.DCCurrent, sessionInfo.Session.MeasurementFunction);
                // It seems these properties are not configurable for certain devices.
                // Assert.Equal(0.2, sessionInfo.Session.Range);
                // Assert.Equal(0.01, sessionInfo.Session.Resolution);
            }
        }

        [Fact]
        public void ConfigureMeasurementDigits_ValuesConfigured()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle(new string[] { "DUTPin", "SystemPin" });
            sessionsBundle.ConfigureMeasurementDigits(DmmMeasurementFunction.ACVolts, 0.2, 3.5);

            foreach (var sessionInfo in sessionsBundle.InstrumentSessions)
            {
                Assert.Equal(DmmMeasurementFunction.ACVolts, sessionInfo.Session.MeasurementFunction);
                // It seems these properties are not configurable for certain devices.
                // Assert.Equal(0.2, sessionInfo.Session.Range);
                Assert.Equal(3.5, sessionInfo.Session.DigitsResolution);
            }
        }

        [Fact]
        public void ConfigureMultiPoint_ValuesConfigured()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle(new string[] { "DUTPin", "SystemPin" });
            sessionsBundle.ConfigureMultiPoint(triggerCount: 2, sampleCount: 3, sampleTrigger: "Immediate", sampleIntervalInSeconds: 0.1);

            foreach (var sessionInfo in sessionsBundle.InstrumentSessions)
            {
                Assert.Equal(2, sessionInfo.Session.Trigger.MultiPoint.TriggerCount);
                Assert.Equal(3, sessionInfo.Session.Trigger.MultiPoint.SampleCount);
                Assert.Equal("Immediate", sessionInfo.Session.Trigger.MultiPoint.SampleTrigger);
                // It seems sample internal is not configurable.
                // Assert.Equal(0.1, sessionInfo.Session.Trigger.MultiPoint.SampleInterval.TotalSeconds);
            }
        }

        [Fact]
        public void ConfigurePowerlineFrequency_ValuesConfigured()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle(new string[] { "DUTPin", "SystemPin" });
            sessionsBundle.ConfigurePowerlineFrequency(50);

            foreach (var sessionInfo in sessionsBundle.InstrumentSessions)
            {
                Assert.Equal(50, sessionInfo.Session.Advanced.PowerlineFrequency);
            }
        }

        [Fact]
        public void ConfigureTrigger_ValuesConfigured()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle(new string[] { "DUTPin", "SystemPin" });
            sessionsBundle.ConfigureTrigger("External", triggerDelayInSeconds: 0.01);

            foreach (var sessionInfo in sessionsBundle.InstrumentSessions)
            {
                Assert.Equal("External", sessionInfo.Session.Trigger.Source);
                Assert.Equal(0.01, sessionInfo.Session.Trigger.Delay.TotalSeconds);
            }
        }

        [Fact]
        public void SendSoftwareTriggerSucceeds()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle(new string[] { "DUTPin", "SystemPin" });
            sessionsBundle.SendSoftwareTrigger();
        }
    }
}
