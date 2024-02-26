using System;
using System.Linq;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.Utilities.TSMContext;

namespace NationalInstruments.Tests.TSMMixedSignalStepTemplates.Unit.DCPower
{
    [Collection("NonParallelizable")]
    public sealed class WaveformAcquisitionTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(bool pinMapWithChannelGroup)
        {
            string pinMapFileName = pinMapWithChannelGroup ? "DifferentSMUDevicesWithChannelGroup.pinmap" : "DifferentSMUDevices.pinmap";
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
        public void SessionsInitialized_ConfigureAndStartWaveformAcquisition_OriginalSettingsAreCorrectlyReturned(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.GetDCPowerSessionsBundle("VDD");
            var expectedOriginalApertureTime = 0.017;
            var expectedOriginalMeasureWhen = DCPowerMeasurementWhen.OnDemand;
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(expectedOriginalApertureTime, sessionInfo.ChannelOutput.Measurement.ApertureTime, 3);
                Assert.Equal(expectedOriginalMeasureWhen, sessionInfo.ChannelOutput.Measurement.MeasureWhen);
            });

            var originalSettings = sessionsBundle.ConfigureAndStartWaveformAcquisition(sampleRate: 50, bufferLength: 10);

            foreach (var settings in originalSettings.Values.Values.SelectMany(v => v.Values))
            {
                Assert.Equal(expectedOriginalApertureTime, settings.ApertureTime, 3);
                Assert.Equal(expectedOriginalMeasureWhen, settings.MeasureWhen);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void WaveformAcquisitionStarted_FinishWaveformAcquisition_ResultsAreCorrectlyReturned(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.GetDCPowerSessionsBundle("VDD");
            var originalSttings = sessionsBundle.ConfigureAndStartWaveformAcquisition(sampleRate: 50, bufferLength: 10);

            var results = sessionsBundle.FinishWaveformAcquisition(fetchWaveformLength: 2, originalSttings);

            foreach (var result in results.Values)
            {
                Assert.Equal(0.02, result.Value["VDD"].DeltaTime);
                // pointsToFetch = fetchWaveformLength / deltaTime = 2 / 0.02 = 100
                Assert.Equal(100, result.Value["VDD"].Result.VoltageMeasurements.Length);
            }
        }
    }
}
