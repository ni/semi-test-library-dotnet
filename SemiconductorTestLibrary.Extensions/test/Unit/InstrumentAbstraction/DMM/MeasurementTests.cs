using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DMM
{
    [Collection("NonParallelizable")]
    public sealed class MeasurementTests : IDisposable
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        private readonly TSMSessionManager _sessionManager;

        public MeasurementTests()
        {
            _tsmContext = CreateTSMContext("DMMTests.pinmap");
            _sessionManager = new TSMSessionManager(_tsmContext);
            Initialize(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        [Fact]
        public void FetchSinglePoint_Succeeds()
        {
            var sessionsBundle = _sessionManager.DMM("DUTPin");
            sessionsBundle.Initiate();
            var results = sessionsBundle.FetchAndPublish(maximumTimeInMilliseconds: 1000);
            sessionsBundle.Abort();

            Assert.Equal(2, results.Length);
        }

        [Fact]
        public void FetchPerSitePerPinResults_Succeeds()
        {
            var sessionsBundle = _sessionManager.DMM("DUTPin");
            sessionsBundle.Initiate();
            var results = sessionsBundle.Fetch(maximumTimeInMilliseconds: 1000);
            sessionsBundle.Abort();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Single(results.ExtractSite(0));
            Assert.Contains("DUTPin", results.ExtractSite(0));
            Assert.Single(results.ExtractSite(1));
            Assert.Contains("DUTPin", results.ExtractSite(1));
        }

        [Fact]
        public void ReadSinglePoint_Succeeds()
        {
            var sessionsBundle = _sessionManager.DMM("DUTPin");
            var results = sessionsBundle.ReadAndPublish(maximumTimeInMilliseconds: 1000);

            Assert.Equal(2, results.Length);
        }

        [Fact]
        public void ReadPerSitePerPinResults_Succeeds()
        {
            var sessionsBundle = _sessionManager.DMM("DUTPin");
            var results = sessionsBundle.Read(maximumTimeInMilliseconds: 1000);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Single(results.ExtractSite(0));
            Assert.Single(results.ExtractSite(1));
        }
    }
}
