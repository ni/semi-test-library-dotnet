using System;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.Tests.SemiconductorTestLibrary.InstrumentAbstraction.ScopeTests;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Scope
{
    [Collection("NonParallelizable")]
    public sealed class TimingTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(string pinMapFileName)
        {
            return TestSetup(pinMapFileName, out _tsmContext);
        }

        public void Dispose()
        {
            if (_tsmContext != null)
            {
                TestCleanup(_tsmContext);
            }
        }

        #region ConfigureTiming Tests

        [Fact]
        public void ValidTimingSettings_ConfigureTiming_TimingSettingsCorrectlyConfigured()
        {
            var sessionManager = Initialize("ScopeTests.pinmap");
            var sessionsBundle = sessionManager.Scope("DUTPin1");
            var timingSettings = new TimingSettings
            {
                MinimumSampleRate = 1000000,
                MinimumNumberOfPoints = 1000,
                ReferencePosition = 50,
                NumberOfRecords = 3,
                EnforceRealtime = true
            };

            sessionsBundle.ConfigureTiming(timingSettings);

            sessionsBundle.Do(sessionInfo => AssertTimingSettings(sessionInfo.Session, timingSettings));
        }

        [Fact]
        public void ConfiguredTiming_GetNumberOfRecordsToAcquire_ReturnsConfiguredNumberOfRecords()
        {
            var sessionManager = Initialize("ScopeTests.pinmap");
            var sessionsBundle = sessionManager.Scope("DUTPin1");
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

        [Fact]
        public void MultiplePinsAcrossMultipleSessions_ConfigureTiming_TimingSettingsCorrectlyConfiguredForAllSessions()
        {
            var sessionManager = Initialize("ScopeTests.pinmap");
            var sessionsBundle = sessionManager.Scope(new string[] { "DUTPin1", "DUTPin2", "DUTPin3" });
            var timingSettings = new TimingSettings
            {
                MinimumSampleRate = 2000000,
                MinimumNumberOfPoints = 2000,
                ReferencePosition = 25,
                NumberOfRecords = 2,
                EnforceRealtime = false
            };

            sessionsBundle.ConfigureTiming(timingSettings);

            sessionsBundle.Do(sessionInfo => AssertTimingSettings(sessionInfo.Session, timingSettings));
        }

        #endregion

        #region ConfigureClock Tests

        [Fact]
        public void ValidClockSettings_ConfigureClock_ClockSettingsCorrectlyConfigured()
        {
            var sessionManager = Initialize("ScopeTests.pinmap");
            var sessionsBundle = sessionManager.Scope("DUTPin1");
            var clockSettings = new ClockSettings
            {
                InputClockSource = ScopeInputClockSource.NoSource,
                OutputClockSource = ScopeOutputClockSource.NoSource,
                ClockSynchronizationPulseSource = ScopeClockSynchronizationPulseSource.NoSource,
                MasterEnabled = false
            };

            sessionsBundle.ConfigureClock(clockSettings);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeClockSynchronizationPulseSource.NoSource, sessionInfo.Session.Timing.ClockSynchronizationPulseSource));
        }

        [Fact]
        public void MasterEnabledTrue_ConfigureClock_ClockSettingsCorrectlyConfigured()
        {
            var sessionManager = Initialize("ScopeTests.pinmap");
            var sessionsBundle = sessionManager.Scope("DUTPin1");
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
        public void NewTimingSettings_DefaultConstructor_HasExpectedDefaultValues()
        {
            var timingSettings = new TimingSettings();

            Assert.True(timingSettings.EnforceRealtime);
        }

        [Fact]
        public void NewClockSettings_DefaultConstructor_HasExpectedDefaultValues()
        {
            var clockSettings = new ClockSettings();

            Assert.Equal(ScopeInputClockSource.NoSource, clockSettings.InputClockSource);
            Assert.Equal(ScopeOutputClockSource.NoSource, clockSettings.OutputClockSource);
            Assert.Equal(ScopeClockSynchronizationPulseSource.NoSource, clockSettings.ClockSynchronizationPulseSource);
            Assert.False(clockSettings.MasterEnabled);
        }

        #endregion

        #region Helper Methods

        private void AssertTimingSettings(NationalInstruments.ModularInstruments.NIScope.NIScope session, TimingSettings expectedSettings)
        {
            Assert.True(session.Acquisition.SampleRate >= expectedSettings.MinimumSampleRate);
            Assert.Equal(expectedSettings.ReferencePosition, session.Trigger.ReferenceTrigger.ReferencePosition);
            Assert.Equal(expectedSettings.NumberOfRecords, session.Timing.NumberOfRecordsToAcquire);
            Assert.Equal(expectedSettings.EnforceRealtime, session.Timing.EnforceRealtime);
        }

        #endregion
    }
}
