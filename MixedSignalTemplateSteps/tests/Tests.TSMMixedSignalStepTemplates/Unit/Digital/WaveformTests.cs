using System.Collections.Generic;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.ModularInstruments.NIDigital;
using Xunit;
using static NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.Utilities.TSMContext;

namespace NationalInstruments.Tests.TSMMixedSignalStepTemplates.Unit.Digital
{
    [Collection("NonParallelizable")]
    public class WaveformTests
    {
        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateSerialSourceWaveformWithoutPin_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle("C0");
            sessionsBundle.CreateSerialSourceWaveform("SourceWaveform", SourceDataMapping.Broadcast, sampleWidth: 8, BitOrder.MostSignificantBitFirst);

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateSerialSourceWaveformWithSinglePin_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.CreateSerialSourceWaveform("C0", "SourceWaveform", SourceDataMapping.Broadcast, sampleWidth: 8, BitOrder.MostSignificantBitFirst);

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateSerialCaptureWaveformWithoutPin_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle("C0");
            sessionsBundle.CreateSerialCaptureWaveform("CaptureWaveform", sampleWidth: 8, BitOrder.MostSignificantBitFirst);

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateSerialCaptureWaveformWithSinglePin_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.CreateSerialCaptureWaveform("C0", "CaptureWaveform", sampleWidth: 8, BitOrder.MostSignificantBitFirst);

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateParallelCaptureWaveformWithoutPin_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle("C0");
            sessionsBundle.CreateParallelCaptureWaveform("CaptureWaveform");

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateParallelCaptureWaveformWithSinglePin_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.CreateParallelCaptureWaveform("C0", "CaptureWaveform");

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_CreateParallelCaptureWaveformWithMultiplePins_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.CreateParallelCaptureWaveform(new string[] { "C0", "C1" }, "CaptureWaveform");

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", true)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", true)]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", false)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", false)]
        public void SessionsInitialized_WriteSourceWaveformBroadcast_Succeeds(string pinMap, string digitalProject, bool expandToMinimumSize)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle("C0");
            sessionsBundle.CreateSerialSourceWaveform("SourceWaveform", SourceDataMapping.Broadcast, sampleWidth: 8, BitOrder.MostSignificantBitFirst);
            sessionsBundle.WriteSourceWaveformBroadcast("SourceWaveform", new uint[] { 0, 1, 2, 3, 4 }, expandToMinimumSize);

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WriteSourceWaveformBroadcastWithExpandingToMinimumSizeLessThanRealSize_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle("C0");
            sessionsBundle.CreateSerialSourceWaveform("SourceWaveform", SourceDataMapping.Broadcast, sampleWidth: 8, BitOrder.MostSignificantBitFirst);
            sessionsBundle.WriteSourceWaveformBroadcast("SourceWaveform", new uint[] { 0, 1, 2, 3, 4 }, expandToMinimumSize: true, minimumSize: 4);

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", true)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", true)]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", false)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", false)]
        public void SessionsInitialized_WriteSourceWaveformSiteUniqueWithPerSiteWaveformData_Succeeds(string pinMap, string digitalProject, bool expandToMinimumSize)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle("C0");
            sessionsBundle.CreateSerialSourceWaveform("SourceWaveform", SourceDataMapping.SiteUnique, sampleWidth: 8, BitOrder.MostSignificantBitFirst);
            var waveformData = new Dictionary<int, uint[]>()
            {
                [0] = new uint[] { 0, 1, 2, 3, 4 },
                [1] = new uint[] { 5, 6, 7, 8, 9 }
            };
            sessionsBundle.WriteSourceWaveformSiteUnique("SourceWaveform", waveformData, expandToMinimumSize);

            Close(tsmContext);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_FetchCaptureWaveform_Succeeds()
        {
            var tsmContext = CreateTSMContext("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle("C0");
            sessionsBundle.BurstPattern("CaptureWaveform");
            var results = sessionsBundle.FetchCaptureWaveform("New_Waveform", samplesToRead: 8);

            Assert.Equal(2, results.Length);
            Close(tsmContext);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_FetchCaptureWaveform_Succeeds()
        {
            var tsmContext = CreateTSMContext("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle("C0");
            sessionsBundle.BurstPattern("CaptureWaveform");
            var results = sessionsBundle.FetchCaptureWaveform("New_Waveform", samplesToRead: 8);

            Assert.Single(results);
            Close(tsmContext);
        }
    }
}
