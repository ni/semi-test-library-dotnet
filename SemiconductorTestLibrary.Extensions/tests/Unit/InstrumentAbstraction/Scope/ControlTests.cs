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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Abort_OnScopeBundle_Succeeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.Scope("Vosc1");

            sessionsBundle.Abort();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Commit_OnScopeBundle_Succeeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.Scope("Vosc1");

            sessionsBundle.Commit();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InitiateAndAbort_OnScopeBundle_Succeeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.Scope("Vosc1");

            sessionsBundle.Initiate();
            sessionsBundle.Abort();
        }
    }
}