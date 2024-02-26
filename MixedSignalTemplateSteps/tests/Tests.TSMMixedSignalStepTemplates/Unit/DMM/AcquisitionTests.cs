using System;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DMM;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DMM.InitializeAndClose;
using static NationalInstruments.Tests.Utilities.TSMContext;

namespace NationalInstruments.Tests.TSMMixedSignalStepTemplates.Unit.DMM
{
    [Collection("NonParallelizable")]
    public sealed class AcquisitionTests : IDisposable
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        private readonly TSMSessionManager _sessionManager;

        public AcquisitionTests()
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
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle("DUTPin");
            sessionsBundle.Initiate();
            var results = sessionsBundle.Fetch(maximumTimeInMilliseconds: 1000);
            sessionsBundle.Abort();

            Assert.Equal(2, results.Length);
        }

        [Fact]
        public void FetchPerSitePerPinResults_Succeeds()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle("DUTPin");
            sessionsBundle.Initiate();
            var results = sessionsBundle.FetchPerSitePerPinResults(maximumTimeInMilliseconds: 1000);
            sessionsBundle.Abort();

            Assert.Equal(2, results.Values.Count);
            Assert.Single(results.Values[0]);
            Assert.Contains("DUTPin", results.Values[0]);
            Assert.Single(results.Values[1]);
            Assert.Contains("DUTPin", results.Values[1]);
        }

        [Fact]
        public void ReadSinglePoint_Succeeds()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle("DUTPin");
            var results = sessionsBundle.Read(maximumTimeInMilliseconds: 1000);

            Assert.Equal(2, results.Length);
        }

        [Fact]
        public void ReadPerSitePerPinResults_Succeeds()
        {
            var sessionsBundle = _sessionManager.GetDMMSessionsBundle("DUTPin");
            var results = sessionsBundle.ReadPerSitePerPinResults(maximumTimeInMilliseconds: 1000);

            Assert.Equal(2, results.Values.Count);
            Assert.Single(results.Values[0]);
            Assert.Single(results.Values[1]);
        }
    }
}
