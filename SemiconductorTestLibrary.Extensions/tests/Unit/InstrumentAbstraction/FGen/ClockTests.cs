using System;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Fgen
{
    [Collection("NonParallelizable")]
    public sealed class ClockTests : IDisposable
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

        #region ConfigureClockMode Tests

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_ConfigureClockMode_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ClockMode clockMode = ClockMode.HighResolution;

            sessionsBundle.ConfigureClockMode(clockMode);

            AssertClockMode(sessionsBundle, clockMode);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_ConfigureClockModeWhileGenerating_ThrowsException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();

            var exception = Record.Exception(() =>
            {
                sessionsBundle.ConfigureClockMode(ClockMode.HighResolution);
            });

            sessionsBundle.Abort();
            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("Property: Sample Clock Mode", exception.Message);
            Assert.Contains("The operation cannot be completed because the device is not configurable while it is generating a signal", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        #endregion

        #region ConfigureSampleSource Tests

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_ConfigureSampleSource_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            string clockSource = "OnboardClock";

            sessionsBundle.ConfigureSampleSource(clockSource);

            AssertClockSource(sessionsBundle, clockSource);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_ConfigureSampleSourceWhileGenerating_ThrowsException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            string clockSource = "OnboardClock";
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();

            var exception = Record.Exception(() =>
            {
                sessionsBundle.ConfigureSampleSource(clockSource);
            });

            sessionsBundle.Abort();
            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("Attribute Id: Sample Clock Source", exception.Message);
            Assert.Contains("Attribute or property not supported", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        #endregion

        #region ConfigureReferenceClock Tests

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_ConfigureReferenceClock_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            string clockSource = "OnboardRefClk";
            double frequency = 100e6;

            sessionsBundle.ConfigureReferenceClock(clockSource, frequency);

            AssertReferenceClockSettings(sessionsBundle, clockSource, frequency);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_ConfigureReferenceClockWhileGenerating_ThrowsException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            string clockSource = "OnboardRefClk";
            double frequency = 100e6;
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();

            var exception = Record.Exception(() =>
            {
                sessionsBundle.ConfigureReferenceClock(clockSource, frequency);
            });

            sessionsBundle.Abort();
            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("Property: Reference Clock Source", exception.Message);
            Assert.Contains("The operation cannot be completed because the device is not configurable while it is generating a signal", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        #endregion

        #region HelperMethods

        private void AssertClockMode(FgenSessionsBundle sessionsBundle, ClockMode expectedValue)
        {
            sessionsBundle.Do(sessionInformation =>
            {
                var actualValue = sessionInformation.Session.Timing.SampleClock.ClockMode;
                Assert.Equal(expectedValue, actualValue);
            });
        }

        private void AssertClockSource(FgenSessionsBundle sessionsBundle, string expectedValue)
        {
            sessionsBundle.Do(sessionInformation =>
            {
                var actualValue = sessionInformation.Session.Timing.SampleClock.Source;
                Assert.Equal(expectedValue, actualValue);
            });
        }

        private void AssertReferenceClockSettings(FgenSessionsBundle sessionsBundle, string expectedSoureVlue, double expectedFrequencyValue)
        {
            sessionsBundle.Do(sessionInformation =>
            {
                var actualSourceValue = sessionInformation.Session.Timing.ReferenceClock.Source;
                Assert.Equal(expectedSoureVlue, actualSourceValue);

                var actualFrequencyValue = sessionInformation.Session.Timing.ReferenceClock.Frequency;
                Assert.Equal(expectedFrequencyValue, actualFrequencyValue);
            });
        }

        private static void ConfigureStandardWaveformSettings(
            FgenSessionsBundle sessionsBundle,
            StandardWaveform waveformType = StandardWaveform.Sine,
            double frequency = 1000.0,
            double amplitude = 1.0,
            double offset = 0.0,
            double startPhase = 0.0)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Output.OutputMode = OutputMode.Function;
                sessionInfo.Session.StandardWaveform.Configure(sessionInfo.AllChannelsString, waveformType, amplitude, offset, frequency, startPhase);
            });
        }

        #endregion
    }
}