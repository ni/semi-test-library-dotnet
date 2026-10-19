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
        private const string _SCP_5162_Pin = "SCP_5162_Pin";

        private readonly ISemiconductorModuleContext _tsmContext;

        public TriggerTests()
        {
            _tsmContext = CreateTSMContext("ScopeExtensionsTests.pinmap");
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
            var settings = new EdgeTriggerSettings
            {
                TriggerLevel = 0.15,
                TriggerSlope = ScopeTriggerSlope.Negative,
                TriggerCoupling = ScopeTriggerCoupling.DC,
                HoldOff = PrecisionTimeSpan.FromSeconds(1e-6)
            };

            sessionsBundle.ConfigureEdgeTrigger(settings);

            sessionsBundle.Do(sessionInfo =>
            {
                var trigger = sessionInfo.Session.Trigger;
                Assert.Equal(ScopeTriggerType.Edge, trigger.Type);
                Assert.Equal(settings.TriggerSlope, trigger.EdgeTrigger.Slope);
                Assert.Equal(settings.TriggerCoupling, trigger.Coupling);
                Assert.Equal(settings.HoldOff.TotalSeconds, trigger.Holdoff.TotalSeconds, 9);
            });
        }

        [Fact]
        public void SessionsBundle_ConfigureEdgeTriggerWithNullSettings_ThrowsArgumentNullException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            Assert.Throws<ArgumentNullException>(() => sessionsBundle.ConfigureEdgeTrigger(null));
        }

        [Fact]
        public void SessionsBundle_ConfigureEdgeTriggerWithInvalidLevel_ThrowsException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var settings = new EdgeTriggerSettings { TriggerLevel = 1000.0 };

            void ConfigureAndCommit()
            {
                sessionsBundle.ConfigureEdgeTrigger(settings);
                sessionsBundle.Commit();
            }

            Assert.Throws<NISemiconductorTestException>(ConfigureAndCommit);
        }

        [Fact]
        public void SessionsBundle_ConfigureEdgeTriggerWithDefaultSettings_TypeIsEdge()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            sessionsBundle.ConfigureEdgeTrigger(new EdgeTriggerSettings());

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeTriggerType.Edge, sessionInfo.Session.Trigger.Type));
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerHysteresis_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5162_Pin);
            var settings = new HysteresisTriggerSettings
            {
                TriggerLevel = 0.1,
                TriggerHysteresis = 0.05,
                TriggerSlope = ScopeTriggerSlope.Negative,
                TriggerCoupling = ScopeTriggerCoupling.DC,
                HoldOff = PrecisionTimeSpan.FromSeconds(1e-6)
            };

            sessionsBundle.ConfigureTriggerHysteresis(settings);

            sessionsBundle.Do(sessionInfo =>
            {
                var trigger = sessionInfo.Session.Trigger;
                Assert.Equal(ScopeTriggerType.Hysteresis, trigger.Type);
                Assert.InRange(trigger.Level, settings.TriggerLevel - 0.01, settings.TriggerLevel + 0.01);
                Assert.InRange(trigger.ReferenceTrigger.Hysteresis, settings.TriggerHysteresis - 0.01, settings.TriggerHysteresis + 0.01);
                Assert.Equal(settings.TriggerSlope, trigger.EdgeTrigger.Slope);
                Assert.Equal(settings.TriggerCoupling, trigger.Coupling);
                Assert.Equal(settings.HoldOff.TotalSeconds, trigger.Holdoff.TotalSeconds, 9);
            });
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerHysteresisWithNullSettings_ThrowsArgumentNullException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            Assert.Throws<ArgumentNullException>(() => sessionsBundle.ConfigureTriggerHysteresis(null));
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerHysteresisOnUnsupportedDevice_ThrowsException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var settings = new HysteresisTriggerSettings { TriggerLevel = 0.1, TriggerHysteresis = 0.05 };
            void ConfigureTriggerHysteresisOnUnsupportedInsturment()
            {
                sessionsBundle.ConfigureTriggerHysteresis(settings);
                sessionsBundle.Commit();
            }
            var exception = Assert.Throws<NISemiconductorTestException>(ConfigureTriggerHysteresisOnUnsupportedInsturment);
            Assert.Contains("This device does not support hysteresis triggering.", exception.Message);
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerHysteresisWithInvalidLevel_ThrowsException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5162_Pin);
            var settings = new HysteresisTriggerSettings { TriggerLevel = 1000.0, TriggerHysteresis = 0.05 };

            void ConfigureAndCommit()
            {
                sessionsBundle.ConfigureTriggerHysteresis(settings);
                sessionsBundle.Commit();
            }

            Assert.Throws<NISemiconductorTestException>(ConfigureAndCommit);
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerDigitalWithNegativeHoldOff_ThrowsException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var settings = new DigitalTriggerSettings { HoldOff = PrecisionTimeSpan.FromSeconds(-1.0) };

            void ConfigureAndCommit()
            {
                sessionsBundle.ConfigureTriggerDigital(settings);
                sessionsBundle.Commit();
            }

            Assert.Throws<NISemiconductorTestException>(ConfigureAndCommit);
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerDigital_TypeIsDigital()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var settings = new DigitalTriggerSettings { TriggerSlope = ScopeTriggerSlope.Negative };

            sessionsBundle.ConfigureTriggerDigital(settings);

            sessionsBundle.Do(sessionInfo =>
            {
                var trigger = sessionInfo.Session.Trigger;
                Assert.Equal(ScopeTriggerType.DigitalEdge, trigger.Type);
                Assert.Equal(settings.TriggerSlope, trigger.EdgeTrigger.Slope);
            });
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerDigitalWithNullSettings_ThrowsArgumentNullException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            Assert.Throws<ArgumentNullException>(() => sessionsBundle.ConfigureTriggerDigital(null));
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerWindow_TypeIsWindow()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin");
            var settings = new WindowTriggerSettings
            {
                WindowLowLevel = -0.1,
                WindowHighLevel = 0.1,
                TriggerWindowMode = ScopeWindowTriggerMode.Leaving
            };

            sessionsBundle.ConfigureTriggerWindow(settings);

            sessionsBundle.Do(sessionInfo =>
            {
                var trigger = sessionInfo.Session.Trigger;
                Assert.Equal(ScopeTriggerType.Window, trigger.Type);
                Assert.Equal(settings.TriggerWindowMode, trigger.ReferenceTrigger.Window.Mode);
                Assert.Equal(settings.TriggerCoupling, trigger.Coupling);
            });
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerWindowWithNullSettings_ThrowsArgumentNullException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            Assert.Throws<ArgumentNullException>(() => sessionsBundle.ConfigureTriggerWindow(null));
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerWindowWithInvalidLevels_ThrowsException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var settings = new WindowTriggerSettings { WindowLowLevel = -1000.0, WindowHighLevel = 1000.0 };

            void ConfigureAndCommit()
            {
                sessionsBundle.ConfigureTriggerWindow(settings);
                sessionsBundle.Commit();
            }

            Assert.Throws<NISemiconductorTestException>(ConfigureAndCommit);
        }

        [Fact]
        public void SessionsBundle_ConfigureEdgeTriggerAndInitiate_Succeeds()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            sessionsBundle.ConfigureEdgeTrigger(new EdgeTriggerSettings());
            sessionsBundle.Initiate();

            sessionsBundle.Abort();
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerImmediate_TypeIsImmediate()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            sessionsBundle.ConfigureTriggerImmediate();

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeTriggerType.Immediate, sessionInfo.Session.Trigger.Type));
        }

        [Fact]
        public void SessionsBundle_ConfigureTriggerImmediateAndInitiate_Succeeds()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            sessionsBundle.ConfigureTriggerImmediate();
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
