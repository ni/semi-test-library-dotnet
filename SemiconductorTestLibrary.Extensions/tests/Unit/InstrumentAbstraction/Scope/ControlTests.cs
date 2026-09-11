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

        private ScopeSessionsBundle GetSessionsBundle(string pin)
        {
            var sessionManager = new TSMSessionManager(_tsmContext);
            return sessionManager.Scope(pin);
        }

        [Theory]
        [InlineData(SinglePin)]
        [InlineData(PinGroup)]
        public void InitiatedAcquisition_Abort_Succeeds(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);
            sessionsBundle.Initiate();

            sessionsBundle.Abort();
        }

        [Theory]
        [InlineData(SinglePin)]
        [InlineData(PinGroup)]
        public void SessionsBundle_AutoSetup_Succeeds(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);

            sessionsBundle.AutoSetup();
        }

        [Theory]
        [InlineData(SinglePin)]
        [InlineData(PinGroup)]
        public void SessionsBundle_Commit_Succeeds(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);

            sessionsBundle.Commit();
        }

        [Theory]
        [InlineData(SinglePin)]
        [InlineData(PinGroup)]
        public void SessionsBundle_Initiate_Succeeds(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);

            sessionsBundle.Initiate();

            sessionsBundle.Abort();
        }

        [Fact]
        public void SessionsBundle_AutoSetupCommitInitiateThenAbort_Succeeds()
        {
            var sessionsBundle = GetSessionsBundle(PinGroup);

            sessionsBundle.AutoSetup();
            sessionsBundle.Commit();
            sessionsBundle.Initiate();
            sessionsBundle.Abort();
        }
    }
}
