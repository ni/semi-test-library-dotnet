using System;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Scope
{
    [Collection("NonParallelizable")]
    public sealed class TimingTests : IDisposable
    {
        private readonly ISemiconductorModuleContext _tsmContext;

        public TimingTests()
        {
            _tsmContext = CreateTSMContext("ScopeTests.pinmap");
            Initialize(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        #region ConfigureTiming Tests

        [Fact]
        public void SessionsBundle_ConfigureTimingWithValidSettings_TimingSettingsCorrectlyConfigured()
        {
            var sessionsBundle = GetSessionsBundle("DUTPin1");
            var timingSettings = new TimingSettings
            {
                MinimumSampleRate = 1000000,
                MinimumNumberOfPoints = 1000,
                ReferencePosition = 50,
                NumberOfRecords = 3,
                EnforceRealtime = true
            };

            sessionsBundle.ConfigureTiming(timingSettings);

            AssertTimingSettings(sessionsBundle, timingSettings);
        }

        [Fact]
        public void SessionsBundle_ConfigureTimingWithMultiplePins_TimingSettingsCorrectlyConfiguredForAllSessions()
        {
            var sessionsBundle = GetSessionsBundle(new[] { "DUTPin1", "DUTPin2", "DUTPin3" });
            var timingSettings = new TimingSettings
            {
                MinimumSampleRate = 2000000,
                MinimumNumberOfPoints = 2000,
                ReferencePosition = 25,
                NumberOfRecords = 2,
                EnforceRealtime = false
            };

            sessionsBundle.ConfigureTiming(timingSettings);

            AssertTimingSettings(sessionsBundle, timingSettings);
        }

        [Fact]
        public void SessionsBundle_ConfigureTimingCalledMultipleTimes_LastSettingsApplied()
        {
            var sessionsBundle = GetSessionsBundle("DUTPin1");
            var finalSettings = new TimingSettings
            {
                MinimumSampleRate = 1000000,
                MinimumNumberOfPoints = 1000,
                ReferencePosition = 50,
                NumberOfRecords = 3,
                EnforceRealtime = true
            };

            sessionsBundle.ConfigureTiming(new TimingSettings
            {
                MinimumSampleRate = 500000,
                MinimumNumberOfPoints = 500,
                ReferencePosition = 10,
                NumberOfRecords = 1,
                EnforceRealtime = false
            });
            sessionsBundle.ConfigureTiming(finalSettings);

            AssertTimingSettings(sessionsBundle, finalSettings);
        }

        [Fact]
        public void SessionsBundle_ConfigureTimingThenGetNumberOfRecordsToAcquire_ReturnsConfiguredNumberOfRecords()
        {
            var sessionsBundle = GetSessionsBundle("DUTPin1");
            var timingSettings = new TimingSettings
            {
                MinimumSampleRate = 1000000,
                MinimumNumberOfPoints = 1000,
                ReferencePosition = 50,
                NumberOfRecords = 3,
                EnforceRealtime = true
            };
            sessionsBundle.ConfigureTiming(timingSettings);

            var numberOfRecordsToAcquire = sessionsBundle.GetNumberOfRecordsToAcquire();

            Assert.All(numberOfRecordsToAcquire, numberOfRecords => Assert.Equal(3, numberOfRecords));
        }

        #endregion

        #region ConfigureClock Tests

        [Fact]
        public void SessionsBundle_ConfigureClockWithDefaultSettings_ClockSettingsCorrectlyConfigured()
        {
            var sessionsBundle = GetSessionsBundle("DUTPin1");

            sessionsBundle.ConfigureClock(new ClockSettings());

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeClockSynchronizationPulseSource.NoSource, sessionInfo.Session.Timing.ClockSynchronizationPulseSource));
        }

        [Fact]
        public void SessionsBundle_ConfigureClockWithMasterEnabled_ClockSettingsCorrectlyConfigured()
        {
            var sessionsBundle = GetSessionsBundle("DUTPin1");
            var clockSettings = new ClockSettings
            {
                InputClockSource = ScopeInputClockSource.NoSource,
                OutputClockSource = ScopeOutputClockSource.NoSource,
                ClockSynchronizationPulseSource = ScopeClockSynchronizationPulseSource.Pfi2,
                MasterEnabled = true
            };

            sessionsBundle.ConfigureClock(clockSettings);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeClockSynchronizationPulseSource.Pfi2, sessionInfo.Session.Timing.ClockSynchronizationPulseSource));
        }

        #endregion

        #region TimingSettings and ClockSettings Default Value Tests

        [Fact]
        public void TimingSettingsConstructedWithDefaults_HasExpectedDefaultValues()
        {
            var timingSettings = new TimingSettings();

            Assert.Equal(0, timingSettings.MinimumSampleRate);
            Assert.Equal(0, timingSettings.MinimumNumberOfPoints);
            Assert.Equal(0, timingSettings.ReferencePosition);
            Assert.Equal(0, timingSettings.NumberOfRecords);
            Assert.True(timingSettings.EnforceRealtime);
        }

        [Fact]
        public void ClockSettingsConstructedWithDefaults_HasExpectedDefaultValues()
        {
            var clockSettings = new ClockSettings();

            Assert.Equal(ScopeInputClockSource.NoSource, clockSettings.InputClockSource);
            Assert.Equal(ScopeOutputClockSource.NoSource, clockSettings.OutputClockSource);
            Assert.Equal(ScopeClockSynchronizationPulseSource.NoSource, clockSettings.ClockSynchronizationPulseSource);
            Assert.False(clockSettings.MasterEnabled);
        }

        #endregion

        #region Helper Methods

        private static void AssertTimingSettings(ScopeSessionsBundle sessionsBundle, TimingSettings expected)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                var session = sessionInfo.Session;
                Assert.True(session.Acquisition.SampleRate >= expected.MinimumSampleRate);
                Assert.Equal(expected.ReferencePosition, session.Trigger.ReferenceTrigger.ReferencePosition);
                Assert.Equal(expected.NumberOfRecords, session.Timing.NumberOfRecordsToAcquire);
                Assert.Equal(expected.EnforceRealtime, session.Timing.EnforceRealtime);
            });
        }

        private ScopeSessionsBundle GetSessionsBundle(string pin)
        {
            return GetSessionsBundle(new[] { pin });
        }

        private ScopeSessionsBundle GetSessionsBundle(string[] pins)
        {
            var sessionManager = new TSMSessionManager(_tsmContext);
            return sessionManager.Scope(pins);
        }

        #endregion
    }
}
