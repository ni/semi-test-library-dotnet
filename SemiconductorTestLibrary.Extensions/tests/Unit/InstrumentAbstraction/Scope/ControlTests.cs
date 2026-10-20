using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Scope
{
    [Collection("NonParallelizable")]
    public sealed class ControlTests : IDisposable
    {
        private const string SinglePin = "DUTPin1";
        private const string PinGroup = "PinGroup";

        private readonly ISemiconductorModuleContext _tsmContext;

        public ControlTests()
        {
            _tsmContext = CreateTSMContext("ScopeTests.pinmap");
            Initialize(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        [Theory]
        [InlineData(SinglePin)]
        [InlineData(PinGroup)]
        public void InitiatedAcquisition_Abort_DoesNotThrowException(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);
            sessionsBundle.Initiate();

            var exception = Record.Exception(() => sessionsBundle.Abort());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData(SinglePin)]
        [InlineData(PinGroup)]
        public void ScopeSessionsBundle_Initiate_DoesNotThrowException(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);

            var exception = Record.Exception(() => sessionsBundle.Initiate());

            Assert.Null(exception);
            sessionsBundle.Abort();
        }

        [Theory]
        [InlineData(SinglePin)]
        [InlineData(PinGroup)]
        public void ScopeSessionsBundle_AutoSetup_DoesNotThrow(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);

            var exception = Record.Exception(() => sessionsBundle.AutoSetup());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData(SinglePin)]
        [InlineData(PinGroup)]
        public void ScopeSessionsBundle_Commit_DoesNotThrowException(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);

            var exception = Record.Exception(() => sessionsBundle.Commit());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData(SinglePin)]
        [InlineData(PinGroup)]
        public void ScopeSessionsBundle_AutoSetupCommitInitiateThenAbort_DoesNotThrowException(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);

            sessionsBundle.AutoSetup();
            sessionsBundle.Commit();
            sessionsBundle.Initiate();
            var exception = Record.Exception(() => sessionsBundle.Abort());

            Assert.Null(exception);
        }

        private ScopeSessionsBundle GetSessionsBundle(string pin)
        {
            var sessionManager = new TSMSessionManager(_tsmContext);
            return sessionManager.Scope(pin);
        }
    }
}
