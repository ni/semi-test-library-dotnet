using System;
using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    [Collection("NonParallelizable")]
    [Trait("GP3", "Digital")]
    public sealed class SourceAndCaptureTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager InitializeSessionsAndCreateSessionManager(string pinMapFileName, string digitalPatternProjectFileName)
        {
            _tsmContext = CreateTSMContext(pinMapFileName, digitalPatternProjectFileName);
            Initialize(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateSerialSourceWaveformWithoutPinSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.CreateSerialSourceWaveform("SourceWaveform", SourceDataMapping.Broadcast, sampleWidth: 8, BitOrder.MostSignificantBitFirst);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateSerialSourceWaveformWithSinglePinSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.CreateSerialSourceWaveform("C0", "SourceWaveform", SourceDataMapping.Broadcast, sampleWidth: 8, BitOrder.MostSignificantBitFirst);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateSerialCaptureWaveformWithoutPinSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.CreateSerialCaptureWaveform("CaptureWaveform", sampleWidth: 8, BitOrder.MostSignificantBitFirst);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateSerialCaptureWaveformWithSinglePinSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.CreateSerialCaptureWaveform("C0", "CaptureWaveform", sampleWidth: 8, BitOrder.MostSignificantBitFirst);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateParallelCaptureWaveformWithoutPinSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.CreateParallelCaptureWaveform("CaptureWaveform");
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateParallelCaptureWaveformWithSinglePinSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.CreateParallelCaptureWaveform("C0", "CaptureWaveform");
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateParallelCaptureWaveformWithMultiplePinsSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.CreateParallelCaptureWaveform(new string[] { "C0", "C1" }, "CaptureWaveform");
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", true)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", true)]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", false)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", false)]
        public void SessionsInitialized_WriteSourceWaveformBroadcastSucceeds(string pinMap, string digitalProject, bool expandToMinimumSize)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.CreateSerialSourceWaveform("SourceWaveform", SourceDataMapping.Broadcast, sampleWidth: 8, BitOrder.MostSignificantBitFirst);
            sessionsBundle.WriteSourceWaveformBroadcast("SourceWaveform", new uint[] { 0, 1, 2, 3, 4 }, expandToMinimumSize);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WriteSourceWaveformBroadcastWithExpandingToMinimumSizeLessThanRealSizeSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.CreateSerialSourceWaveform("SourceWaveform", SourceDataMapping.Broadcast, sampleWidth: 8, BitOrder.MostSignificantBitFirst);
            sessionsBundle.WriteSourceWaveformBroadcast("SourceWaveform", new uint[] { 0, 1, 2, 3, 4 }, expandToMinimumSize: true, minimumSize: 4);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", true)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", true)]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", false)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", false)]
        public void SessionsInitialized_WriteSourceWaveformSiteUniqueWithPerSiteWaveformDataSucceeds(string pinMap, string digitalProject, bool expandToMinimumSize)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.CreateSerialSourceWaveform("SourceWaveform", SourceDataMapping.SiteUnique, sampleWidth: 8, BitOrder.MostSignificantBitFirst);
            var waveformData = new SiteData<uint[]>(new Dictionary<int, uint[]>()
            {
                [0] = new uint[] { 0, 1, 2, 3, 4 },
                [1] = new uint[] { 5, 6, 7, 8, 9 }
            });
            sessionsBundle.WriteSourceWaveformSiteUnique("SourceWaveform", waveformData, expandToMinimumSize);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateParallelSourceWaveformSiteUniqueSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.CreateParallelSourceWaveform(new string[] { "C0", "C1" }, "ParallelSourceWaveform", SourceDataMapping.SiteUnique);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateParallelSourceWaveformBroadcastSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.CreateParallelSourceWaveform(new string[] { "C0", "C1" }, "ParallelSourceWaveform", SourceDataMapping.Broadcast);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_FetchCaptureWaveform_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.BurstPattern("CaptureWaveform");
            var results = sessionsBundle.FetchCaptureWaveform("New_Waveform", samplesToRead: 8);

            Assert.Equal(2, results.SiteNumbers.Length);
        }

        [Fact(Skip = "Not sure why fetch waveform doesn't return result for site 1.")]
        public void OneDeviceWorksForOnePinOnTwoSites_FetchCaptureWaveform_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.BurstPattern("CaptureWaveform");
            var results = sessionsBundle.FetchCaptureWaveform("New_Waveform", samplesToRead: 8);

            Assert.Single(results.SiteNumbers);
        }
    }
}
