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
    public sealed class ControlTests : IDisposable
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

        #region Initiate Tests

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveform_Initiate_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);

            var exception = Record.Exception(() => sessionsBundle.Initiate());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveformAndInitiate_Initiate_ThrowsExpectedException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();

            var exception = Record.Exception(() => sessionsBundle.Initiate());

            Assert.Contains("at NationalInstruments.ModularInstruments.NIFgen.NIFgen.InitiateGeneration()", exception.Message); // Ensure that correct driver method call is reported in the exception message.
            Assert.Contains("Error code: -1074126847", exception.Message); // Ensure correct error code is reported in the exception message.
            Assert.Contains("The operation cannot be completed because the device is not configurable while it is generating a signal", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithoutConfiguration_Initiate_ThrowsExpectedException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            var exception = Record.Exception(() => sessionsBundle.Initiate());

            Assert.Contains("at NationalInstruments.ModularInstruments.NIFgen.NIFgen.InitiateGeneration()", exception.Message); // Ensure that correct driver method call is reported in the exception message.
            Assert.Contains("Error code: -1074118636", exception.Message); // Ensure correct error code is reported in the exception message.
            Assert.Contains("No waveforms have been created", exception.Message); // Ensure correct error message is reported in the exception message.
        }
        #endregion

        #region Commit Tests

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithoutConfiguration_Commit_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            var exception = Record.Exception(() => sessionsBundle.Commit());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveform_Commit_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);

            var exception = Record.Exception(() => sessionsBundle.Commit());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveformAndInitiate_Commit_ThrowsExpectedException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();

            var exception = Record.Exception(() => sessionsBundle.Commit());

            Assert.Contains("at NationalInstruments.ModularInstruments.NIFgen.NIFgen.Commit()", exception.Message); // Ensure that correct driver method call is reported in the exception message.
            Assert.Contains("Error code: -1074126847", exception.Message); // Ensure correct error code is reported in the exception message.
            Assert.Contains("The operation cannot be completed because the device is not configurable while it is generating a signal", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        #endregion

        #region Abort Tests

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithoutConfiguration_Abort_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            var exception = Record.Exception(() => sessionsBundle.Abort());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveform_Abort_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);

            var exception = Record.Exception(() => sessionsBundle.Abort());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveformAndInitiate_Abort_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();

            var exception = Record.Exception(() => sessionsBundle.Abort());

            Assert.Null(exception);
        }

        #endregion

        #region IsDone Tests

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithoutConfiguration_IsDone_SucceedsAndReturnCorrectStatus(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            var statusArray = sessionsBundle.IsDone();

            Assert.Equal(2, statusArray.Length);
            Assert.All(statusArray, Assert.True);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveformAndInitiate_IsDone_SucceedsAndReturnCorrectStatus(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();

            var statusArray = sessionsBundle.IsDone();

            Assert.Equal(2, statusArray.Length);
            Assert.All(statusArray, Assert.False);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveformInitiateAndAbort_IsDone_SucceedsAndReturnCorrectStatus(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();
            Thread.Sleep(10); // Wait for 10 ms second to ensure the sessions are running before aborting
            sessionsBundle.Abort();

            var statusArray = sessionsBundle.IsDone();

            Assert.Equal(2, statusArray.Length);
            Assert.All(statusArray, Assert.True);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        public void InitializeBundleConfigureStandardWaveformInitiateAndPartialAbort_IsDone_SucceedsAndReturnCorrectStatus(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();
            Thread.Sleep(10); // Wait for 10 ms second to ensure the sessions are running before aborting
            sessionsBundle.FilterByPin("A").Abort(); // Abort only the session associated with pin "A".

            var statusArray = sessionsBundle.IsDone();

            Assert.Equal(2, statusArray.Length);
            Assert.Equal(new[] { true, false }, statusArray); // Pin "A" is aborted; pin "B" is still generating.
        }

        #endregion

        #region WaitUntilDone Tests

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithoutConfiguration_WaitUntilDone_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            var exception = Record.Exception(() => sessionsBundle.WaitUntilDone());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveformInitiateAndAbort_WaitUntilDone_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();
            sessionsBundle.Abort();

            var exception = Record.Exception(() => sessionsBundle.WaitUntilDone());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveformInitiateAndAbort_WaitUntilDoneWithNonDefaultTimeout_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();
            sessionsBundle.Abort();

            var exception = Record.Exception(() => sessionsBundle.WaitUntilDone(100));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveformInitiateAndAbort_WaitUntilDoneWithInvalidTimeout_ThrowsExpectedException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();
            sessionsBundle.Abort();

            var exception = Record.Exception(() => sessionsBundle.WaitUntilDone(-10));

            Assert.Contains("at NationalInstruments.ModularInstruments.NIFgen.Internal.FgenImpl.WaitUntilDone", exception.Message); // Ensure that correct driver method call is reported in the exception message.
            Assert.Contains("Error code: -1074135025", exception.Message); // Ensure correct error code is reported in the exception message.
            Assert.Contains("Invalid parameter", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleConfigureStandardWaveformInitiate_WaitUntilDone_ThrowsExpectedException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });
            ConfigureStandardWaveformSettings(sessionsBundle);
            sessionsBundle.Initiate();

            var exception = Record.Exception(() => sessionsBundle.WaitUntilDone(100));

            Assert.Contains("at NationalInstruments.ModularInstruments.NIFgen.Internal.FgenImpl.WaitUntilDone", exception.Message); // Ensure that correct driver method call is reported in the exception message.
            Assert.Contains("Error code: -1074135025", exception.Message); // Ensure correct error code is reported in the exception message.
            Assert.Contains("Invalid parameter", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        #endregion

        #region Helper Methods
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