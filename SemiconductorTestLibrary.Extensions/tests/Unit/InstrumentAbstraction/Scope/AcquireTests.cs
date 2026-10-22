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
        private const string SinglePin = "SCP_5105_Pin";
        private static readonly string[] MultiPins = { "SCP_5105_Pin", "SCP_5162_Pin", "SCP_5622_Pin" };

        private readonly ISemiconductorModuleContext _tsmContext;

        public AcquireTests()
        {
            _tsmContext = CreateTSMContext("ScopeExtensionsTests.pinmap");
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

            var recordLength = sessionsBundle.GetRecordLength();
            var resolution = sessionsBundle.GetResolution();
            var sampleMode = sessionsBundle.GetSampleMode();
            var sampleRate = sessionsBundle.GetSampleRate();

            Assert.Equal(recordLength, sessionsBundle.GetRecordLength());
            Assert.Equal(resolution, sessionsBundle.GetResolution());
            Assert.Equal(sampleMode, sessionsBundle.GetSampleMode());
            Assert.Equal(sampleRate, sessionsBundle.GetSampleRate());
        }

        [Fact]
        public void SessionsBundleAfterAutoSetup_GetSettings_Succeeds()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);
            var sessionsCount = sessionsBundle.InstrumentSessions.Count();
            sessionsBundle.AutoSetup();

            var recordLength = sessionsBundle.GetRecordLength();
            var resolution = sessionsBundle.GetResolution();
            var sampleMode = sessionsBundle.GetSampleMode();
            var sampleRate = sessionsBundle.GetSampleRate();

            Assert.Equal(sessionsCount, recordLength.Length);
            Assert.Equal(sessionsCount, resolution.Length);
            Assert.Equal(sessionsCount, sampleMode.Length);
            Assert.Equal(sessionsCount, sampleRate.Length);
        }

        [Theory]
        [InlineData("SCP_5105_Pin", 12L)]
        [InlineData("SCP_5162_Pin", 10L)]
        [InlineData("SCP_5622_Pin", 16L)]
        [InlineData("SCP_5186_Pin", 8L)]
        [InlineData("SCP_5172_Pin_1", 14L)]
        public void SessionsBundle_GetResolution_ReturnsExpectedValueForModel(string pin, long expectedResolution)
        {
            var sessionsBundle = GetSessionsBundle(pin);
            var sessionsCount = sessionsBundle.InstrumentSessions.Count();

            var resolution = sessionsBundle.GetResolution();

            Assert.Equal(sessionsCount, resolution.Length);
            Assert.All(resolution, value => Assert.Equal(expectedResolution, value));
        }

        [Fact]
        public void MultiPinMultiSite_GetSettings_ReturnsOneValuePerSession()
        {
            var sessionsBundle = GetSessionsBundle(MultiPins);
            var sessionsCount = sessionsBundle.InstrumentSessions.Count();

            var status = sessionsBundle.GetAcquisitionStatus();
            var recordLength = sessionsBundle.GetRecordLength();
            var resolution = sessionsBundle.GetResolution();
            var sampleMode = sessionsBundle.GetSampleMode();
            var sampleRate = sessionsBundle.GetSampleRate();

            Assert.True(sessionsCount > 1);
            Assert.Equal(sessionsCount, status.Length);
            AssertMatchesDriver(sessionsBundle, recordLength, session => session.Acquisition.RecordLength);
            AssertMatchesDriver(sessionsBundle, resolution, session => session.Acquisition.Resolution);
            AssertMatchesDriver(sessionsBundle, sampleMode, session => session.Acquisition.SampleMode);
            AssertMatchesDriver(sessionsBundle, sampleRate, session => session.Acquisition.SampleRate);
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

        private ScopeSessionsBundle GetSessionsBundle(string[] pins)
        {
            var sessionManager = new TSMSessionManager(_tsmContext);
            return sessionManager.Scope(pins);
        }
    }
}
