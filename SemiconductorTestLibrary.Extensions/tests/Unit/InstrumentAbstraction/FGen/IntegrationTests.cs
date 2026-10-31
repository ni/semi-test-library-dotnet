using System;
using System.Threading;
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
    public sealed class IntegrationTests : IDisposable
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

        [Fact]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void InitializeFgenBundle_GenerateSineWaveformForFiveSeconds_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var pinNames = new string[] { "A", "B" };
            var sessionsBundle = sessionManager.Fgen(pinNames);

            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var standardWaveformSettings = new StandardWaveformSettings(
                waveformFunctionType: StandardWaveform.Sine,
                amplitude: 5.0,
                frequency: 100000.0,
                dcOffset: 0,
                startPhase: 0);
            sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings);
            var generationStatusBeforeInitiate = sessionsBundle.IsDone(); // Idle before Initiate, so done.
            sessionsBundle.Initiate();
            var generationStatusAfterInitiate = sessionsBundle.IsDone(); // Generating after Initiate, so not done.
            // A continuous sine waveform never completes on its own, so WaitUntilDone is expected to time out.
            Exception exception = null;
            try
            {
                sessionsBundle.WaitUntilDone(5000); // Wait for 5 seconds, then time out.
            }
            catch (Exception ex)
            {
                exception = ex;
                sessionsBundle.Abort(); // Stop generation only when the wait times out.
            }
            var generationStatusAfterAbort = sessionsBundle.IsDone(); // Idle again after Abort, so done.

            Assert.All(generationStatusBeforeInitiate, isDone => Assert.True(isDone));
            Assert.All(generationStatusAfterInitiate, isDone => Assert.False(isDone));
            Assert.NotNull(exception);
            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.All(generationStatusAfterAbort, isDone => Assert.True(isDone));
            sessionsBundle.Abort(); // Ensure generation is stopped in case of test failure.
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void InitializeFgenBundle_GenerateWaveformsFrequencySweepSucceeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var pinNames = new string[] { "A", "B" };
            var sessionsBundle = sessionManager.Fgen(pinNames);

            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var frequencySweepData = new double[] { 100000, 200000, 300000, 400000, 50000 }; // Frequency sweep data in Hz.
            var standardWaveformSettings = new StandardWaveformSettings(
                waveformFunctionType: StandardWaveform.Sine,
                amplitude: 5.0,
                frequency: 0,
                dcOffset: 0,
                startPhase: 0);

            for (int i = 0; i < frequencySweepData.Length; i++)
            {
                standardWaveformSettings.Frequency = frequencySweepData[i];
                sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings);
                sessionsBundle.Initiate();
                Thread.Sleep(1000); // Wait for 1 second.
                sessionsBundle.Abort();
            }
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void InitializeFgenBundle_GenerateSineWaveformWithReferenceClock_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var pinNames = new string[] { "A", "B" };
            var sessionsBundle = sessionManager.Fgen(pinNames);

            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var standardWaveformSettings = new StandardWaveformSettings(
                waveformFunctionType: StandardWaveform.Sine,
                amplitude: 5.0,
                frequency: 100000.0,
                dcOffset: 0,
                startPhase: 0);
            sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings);
            sessionsBundle.ConfigureReferenceClock("OnboardRefClk", 100000000); // Configure reference clock to 100 MHz.
            sessionsBundle.Initiate();
            var generationStatus = sessionsBundle.IsDone(); // Generating after Initiate, so not done.
            Thread.Sleep(5000); // Wait for 5 seconds to allow generation to occur.
            sessionsBundle.Abort(); // Stop generation after 5 seconds.

            Assert.All(generationStatus, isDone => Assert.False(isDone));
        }

        [Fact]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void InitializeFgenBundle_GenerateSineWaveformWithTriggers_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var pinNames = new string[] { "A", "B" };
            var sessionsBundle = sessionManager.Fgen(pinNames);

            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var standardWaveformSettings = new StandardWaveformSettings(
                waveformFunctionType: StandardWaveform.Sine,
                amplitude: 5.0,
                frequency: 100000.0,
                dcOffset: 0,
                startPhase: 0);
            sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings);
            sessionsBundle.ConfigureStartTriggerSoftwareEdge(); // Configure Software Edge Start Trigger.
            sessionsBundle.Initiate();
            var hwStatusBeforeTrigger = GetHWState(sessionsBundle); // Waiting for trigger after Initiate, Still not generating.
            Thread.Sleep(5000); // Wait for 5 seconds to allow generation to occur.
            sessionsBundle.SendSoftwareEdgeTrigger(); // Send Software Edge Trigger to start generation.
            var hwStatusAfterTrigger = GetHWState(sessionsBundle); // Trigger received, should be generating now.
            sessionsBundle.Abort(); // Stop generation.
            var hwStatusAfterAbort = GetHWState(sessionsBundle); // Should be in idle state.

            Assert.All(hwStatusBeforeTrigger, state => Assert.Equal(HardwareState.WaitingForStartTrigger, state));
            Assert.All(hwStatusAfterTrigger, state => Assert.Equal(HardwareState.Running, state));
            Assert.All(hwStatusAfterAbort, state => Assert.Equal(HardwareState.Idle, state));
        }

        #region helper methods

        private static HardwareState[] GetHWState(FgenSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Utility.HardwareState;
            });
        }
        #endregion
    }
}
