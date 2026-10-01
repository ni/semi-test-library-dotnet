using System;
using System.Linq;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Scope
{
    [Collection("NonParallelizable")]
    public sealed class AcquireTests : IDisposable
    {
        private const string SinglePin = "DUTPin4";

        private readonly ISemiconductorModuleContext _tsmContext;

        public AcquireTests()
        {
            _tsmContext = CreateTSMContext("ScopeTests.pinmap");
            Initialize(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        [Fact]
        public void SessionsBundle_GetAcquisitionStatus_ReturnsOneValuePerSession()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);

            var status = sessionsBundle.GetAcquisitionStatus();

            Assert.Equal(sessionsBundle.InstrumentSessions.Count(), status.Length);
        }

        [Fact]
        public void InitiatedAcquisition_GetAcquisitionStatus_Succeeds()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);
            sessionsBundle.Initiate();

            var status = sessionsBundle.GetAcquisitionStatus();

            sessionsBundle.Abort();
            Assert.Equal(sessionsBundle.InstrumentSessions.Count(), status.Length);
            Assert.All(status, value => Assert.True(Enum.IsDefined(typeof(ScopeAcquisitionStatus), value)));
        }

        [Fact]
        public void SessionsBundle_GetRecordLength_ReturnsDriverValue()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);

            var recordLength = sessionsBundle.GetRecordLength();

            AssertMatchesDriver(sessionsBundle, recordLength, session => session.Acquisition.RecordLength);
            Assert.All(recordLength, value => Assert.True(value > 0));
        }

        [Fact]
        public void SessionsBundle_GetResolution_ReturnsDriverValue()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);

            var resolution = sessionsBundle.GetResolution();

            AssertMatchesDriver(sessionsBundle, resolution, session => session.Acquisition.Resolution);
            Assert.All(resolution, value => Assert.True(value > 0));
        }

        [Fact]
        public void SessionsBundle_GetSampleMode_ReturnsDriverValue()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);

            var sampleMode = sessionsBundle.GetSampleMode();

            AssertMatchesDriver(sessionsBundle, sampleMode, session => session.Acquisition.SampleMode);
        }

        [Fact]
        public void SessionsBundle_GetSampleRate_ReturnsDriverValue()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);

            var sampleRate = sessionsBundle.GetSampleRate();

            AssertMatchesDriver(sessionsBundle, sampleRate, session => session.Acquisition.SampleRate);
            Assert.All(sampleRate, value => Assert.True(value > 0));
        }

        [Fact]
        public void SessionsBundle_GetSettingsCalledMultipleTimes_ReturnsConsistentValues()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);

            Assert.Equal(sessionsBundle.GetRecordLength(), sessionsBundle.GetRecordLength());
            Assert.Equal(sessionsBundle.GetResolution(), sessionsBundle.GetResolution());
            Assert.Equal(sessionsBundle.GetSampleMode(), sessionsBundle.GetSampleMode());
            Assert.Equal(sessionsBundle.GetSampleRate(), sessionsBundle.GetSampleRate());
        }

        [Fact]
        public void SessionsBundleAfterAutoSetup_GetSettings_Succeeds()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);
            sessionsBundle.AutoSetup();

            Assert.Equal(sessionsBundle.InstrumentSessions.Count(), sessionsBundle.GetRecordLength().Length);
            Assert.Equal(sessionsBundle.InstrumentSessions.Count(), sessionsBundle.GetResolution().Length);
            Assert.Equal(sessionsBundle.InstrumentSessions.Count(), sessionsBundle.GetSampleMode().Length);
            Assert.Equal(sessionsBundle.InstrumentSessions.Count(), sessionsBundle.GetSampleRate().Length);
        }

        private static void AssertMatchesDriver<T>(ScopeSessionsBundle sessionsBundle, T[] actual, Func<NIScope, T> getDriverValue)
        {
            var expected = sessionsBundle.InstrumentSessions.Select(sessionInfo => getDriverValue(sessionInfo.Session)).ToArray();
            Assert.Equal(expected, actual);
        }

        private ScopeSessionsBundle GetSessionsBundle(string pin)
        {
            var sessionManager = new TSMSessionManager(_tsmContext);
            return sessionManager.Scope(pin);
        }
    }
}
