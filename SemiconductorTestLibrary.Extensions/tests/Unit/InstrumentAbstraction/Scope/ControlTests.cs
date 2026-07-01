using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Scope
{
    [Collection("NonParallelizable")]
    public sealed class ControlTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(bool pinMapWithChannelGroup)
        {
            string pinMapFileName = pinMapWithChannelGroup ? "ScopeWithSingleSession.pinmap" : "ScopeWithPerInstrumentSession.pinmap";
            return Initialize(pinMapFileName);
        }

        public TSMSessionManager Initialize(string pinMapFileName = "ScopeTests.pinmap")
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
        public void Abort_OnScopeBundle_Succeeds()
        {
            var sessionManager = Initialize();
            var sessionsBundle = sessionManager.Scope("DUTPin1");

            sessionsBundle.Abort();
        }

        [Fact]
        public void Commit_OnScopeBundle_Succeeds()
        {
            var sessionManager = Initialize();
            var sessionsBundle = sessionManager.Scope("DUTPin1");

            sessionsBundle.Commit();
        }

        [Fact]
        public void InitiateAndAbort_OnScopeBundle_Succeeds()
        {
            var sessionManager = Initialize();
            var sessionsBundle = sessionManager.Scope("DUTPin1");

            sessionsBundle.Initiate();
            sessionsBundle.Abort();
        }
    }
}