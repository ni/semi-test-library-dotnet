using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
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

        [Fact]
        public void InitializeBundleWithSinglePin_InitiateWithoutConfiguration_ThrowsExpectedException()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen("A");

            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.Initiate());

            Assert.Contains("at NationalInstruments.ModularInstruments.NIFgen.NIFgen.InitiateGeneration()", exception.Message); // Ensure that correct driver method call is reported in the exception message.
            Assert.Contains("Error code: -1074118636", exception.Message); // Ensure correct error code is reported in the exception message.
            Assert.Contains("No waveforms have been created", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_InitiateWithoutConfiguration_ThrowsExpectedException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.Initiate());

            Assert.Contains("at NationalInstruments.ModularInstruments.NIFgen.NIFgen.InitiateGeneration()", exception.Message); // Ensure that correct driver method call is reported in the exception message.
            Assert.Contains("Error code: -1074118636", exception.Message); // Ensure correct error code is reported in the exception message.
            Assert.Contains("No waveforms have been created", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        #endregion

        #region Commit Tests
        [Fact]
        public void InitializeBundleWithSinglePin_PerformCommitOperationSucceeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.Commit();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformCommitOperationSucceeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            sessionsBundle.Commit();
        }

        #endregion

        #region Abort Tests

        [Fact]
        public void InitializeBundleWithSinglePin_PerformAbortOperationSucceeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.Abort();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformAbortOperationSucceeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            sessionsBundle.Abort();
        }

        #endregion

        #region IsDone Tests

        [Fact]
        public void InitializeBundleWithSinglePin_PerformIsDoneOperation_SucceedsAndReturnCorrectStatus()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen("A");

            var statusArray = sessionsBundle.IsDone();

            Assert.Single(statusArray);
            Assert.True(statusArray[0]);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformIsDoneOperation_SucceedsAndReturnCorrectStatus(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            var statusArray = sessionsBundle.IsDone();

            Assert.Equal(2, statusArray.Length);
            Assert.All(statusArray, Assert.True);
        }

        #endregion

        #region WaitUntilDone Tests

        [Fact]
        public void InitializeBundleWithSinglePin_PerformWaitUntilDoneOperationWithDefaultTimeoutSucceeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.WaitUntilDone();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformWaitUntilDoneOperationWithDefaultTimeoutSucceeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            sessionsBundle.WaitUntilDone();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformWaitUntilDoneOperationWithNonDefaultTimeoutSucceeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            sessionsBundle.WaitUntilDone(20000);
        }

        #endregion
    }
}
