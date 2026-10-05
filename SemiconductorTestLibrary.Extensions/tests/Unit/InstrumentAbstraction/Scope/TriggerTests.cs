using System;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Scope
{
    [Collection("NonParallelizable")]
    public sealed class TriggerTests : IDisposable
    {
        private const string _SCP_5186_Pin = "SCP_5186_Pin";

        private readonly ISemiconductorModuleContext _tsmContext;

        public TriggerTests()
        {
            _tsmContext = CreateTSMContext("ScopeTests.pinmap");
            Initialize(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        [Fact]
        public void SessionsBundle_ConfigureEdgeTrigger_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var settings = new TriggerSettings
            {
                TriggerLevel = 0.15,
                TriggerSlope = ScopeTriggerSlope.Positive,
                TriggerCoupling = ScopeTriggerCoupling.DC
            };

            sessionsBundle.ConfigureEdgeTrigger(settings);

            sessionsBundle.Do(sessionInfo =>
            {
                var trigger = sessionInfo.Session.Trigger;
                Assert.Equal(ScopeTriggerType.Edge, trigger.Type);
                Assert.Equal(settings.TriggerLevel, trigger.Level, 3);
                Assert.Equal(settings.TriggerSlope, trigger.EdgeTrigger.Slope);
                Assert.Equal(settings.TriggerCoupling, trigger.Coupling);
            });
        }

        [Fact]
        public void SessionsBundle_ConfigureEdgeTriggerWithDefaultSettings_TypeIsEdge()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            sessionsBundle.ConfigureEdgeTrigger(new TriggerSettings());

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeTriggerType.Edge, sessionInfo.Session.Trigger.Type));
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerHysteresis_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var settings = new TriggerSettings { TriggerLevel = 0.1, TriggerHysteresis = 0.05 };
            void ConfigureTriggerHysteresisOnUnsupportedInsturment()
            {
                sessionsBundle.ConfigureTriggerHysteresis(settings);
                sessionsBundle.Commit();
            }
            var exception = Assert.Throws<NISemiconductorTestException>(ConfigureTriggerHysteresisOnUnsupportedInsturment);
            Assert.Contains("This device does not support hysteresis triggering.", exception.Message);
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerDigital_TypeIsDigital()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var settings = new TriggerSettings();

            sessionsBundle.ConfigureTriggerDigital(settings);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeTriggerType.DigitalEdge, sessionInfo.Session.Trigger.Type));
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerWindow_TypeIsWindow()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var settings = new TriggerSettings
            {
                WindowLowLevel = -0.1,
                WindowHighLevel = 0.1,
                TriggerWindowMode = ScopeWindowTriggerMode.Leaving
            };

            sessionsBundle.ConfigureTriggerWindow(settings);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeTriggerType.Window, sessionInfo.Session.Trigger.Type));
        }

        [Fact]
        public void SessionsBundle_ConfigureEdgeTriggerAndInitiate_Succeeds()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            sessionsBundle.ConfigureEdgeTrigger(new TriggerSettings());
            sessionsBundle.Initiate();

            sessionsBundle.Abort();
        }

        private ScopeSessionsBundle GetSessionsBundle(string pin)
        {
            var sessionManager = new TSMSessionManager(_tsmContext);
            return sessionManager.Scope(pin);
        }
    }
}
