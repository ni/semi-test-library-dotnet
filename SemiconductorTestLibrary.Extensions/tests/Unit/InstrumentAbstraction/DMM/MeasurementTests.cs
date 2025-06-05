using System;
using NationalInstruments.ModularInstruments.NIDmm;
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
            _tsmContext = CreateTSMContext("SharedPinTests.pinmap");
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
            var sessionsBundle = _sessionManager.DMM("DMM_PIN1");
            sessionsBundle.Initiate();
            var results = sessionsBundle.FetchAndPublish(maximumTimeInMilliseconds: 1000);
            sessionsBundle.Abort();

            Assert.Single(results);
        }

        [Fact]
        public void FetchPerSitePerPinResults_Succeeds()
        {
            var sessionsBundle = _sessionManager.DMM("DMM_PIN1");
            sessionsBundle.Initiate();
            var results = sessionsBundle.Fetch(maximumTimeInMilliseconds: 1000);
            sessionsBundle.Abort();

            Assert.Equal(3, results.SiteNumbers.Length);
            Assert.Single(results.ExtractSite(0));
            Assert.Contains("DMM_PIN1", results.ExtractSite(0));
            Assert.Single(results.ExtractSite(1));
            Assert.Single(results.ExtractSite(2));
            Assert.Contains("DMM_PIN1", results.ExtractSite(1));
            Assert.Contains("DMM_PIN1", results.ExtractSite(2));
        }

        [Fact]
        public void ReadSinglePoint_Succeeds()
        {
            var sessionsBundle = _sessionManager.DMM("DMM_PIN1");
            var results = sessionsBundle.ReadAndPublish(maximumTimeInMilliseconds: 1000);

            Assert.Single(results);
        }

        [Fact]
        public void ReadPerSitePerPinResults_Succeeds()
        {
            var sessionsBundle = _sessionManager.DMM("DMM_PIN1");
            var results = sessionsBundle.Read(maximumTimeInMilliseconds: 1000);

            Assert.Equal(3, results.SiteNumbers.Length);
            Assert.Single(results.ExtractSite(0));
            Assert.Single(results.ExtractSite(1));
            Assert.Single(results.ExtractSite(2));
        }

        [Fact]
        public void ConfigureAndFetchMuliPoint_Succeeds()
        {
            var sessionsBundle = _sessionManager.DMM("DMM_PIN1");
            var numberOfPoints = 4;
            sessionsBundle.ConfigureMultiPoint(1, numberOfPoints, DmmTriggerSource.Immediate.ToString(), 1);
            sessionsBundle.Initiate();
            sessionsBundle.SendSoftwareTrigger();
            var results = sessionsBundle.FetchMultiPoint(numberOfPoints, maximumTimeInMilliseconds: 1000);
            sessionsBundle.Abort();

            Assert.Equal(3, results.SiteNumbers.Length);
            Assert.Single(results.ExtractSite(0));
            Assert.Contains("DMM_PIN1", results.ExtractSite(0));
            Assert.Equal(numberOfPoints, results.GetValue(0, "DMM_PIN1").Length);
            Assert.Single(results.ExtractSite(2));
            Assert.Contains("DMM_PIN1", results.ExtractSite(2));
            Assert.Equal(numberOfPoints, results.GetValue(2, "DMM_PIN1").Length);
            Assert.Single(results.ExtractSite(1));
            Assert.Contains("DMM_PIN1", results.ExtractSite(1));
            Assert.Equal(numberOfPoints, results.GetValue(1, "DMM_PIN1").Length);
        }

        [Fact]
        public void ConfigureAndReadMuliPoint_Succeeds()
        {
            var sessionsBundle = _sessionManager.DMM("DMM_PIN2");
            var numberOfPoints = 4;
            sessionsBundle.ConfigureMultiPoint(1, numberOfPoints, DmmTriggerSource.SoftwareTrigger.ToString(), 1);
            var results = sessionsBundle.ReadMultiPoint(numberOfPoints, maximumTimeInMilliseconds: 1000);
            sessionsBundle.Abort();

            Assert.Equal(3, results.SiteNumbers.Length);
            Assert.Single(results.ExtractSite(0));
            Assert.Contains("DMM_PIN2", results.ExtractSite(0));
            Assert.Equal(numberOfPoints, results.GetValue(0, "DMM_PIN2").Length);
            Assert.Single(results.ExtractSite(1));
            Assert.Contains("DMM_PIN2", results.ExtractSite(1));
            Assert.Equal(numberOfPoints, results.GetValue(1, "DMM_PIN2").Length);
            Assert.Single(results.ExtractSite(2));
            Assert.Contains("DMM_PIN2", results.ExtractSite(2));
            Assert.Equal(numberOfPoints, results.GetValue(2, "DMM_PIN2").Length);
        }
    }
}
