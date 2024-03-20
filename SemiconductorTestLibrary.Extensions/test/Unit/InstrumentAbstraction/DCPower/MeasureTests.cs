using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DCPower
{
    [Collection("NonParallelizable")]
    public sealed class MeasureTests : IDisposable
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
        public void DifferentVoltageOnDifferentSMUDevices_MeasurePerInstrumentResultsOnDemand_ReturnCorrectValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VCC", "VDET" });
            sessionsBundle.ForceVoltageSymmetricLimit(voltageLevels: new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 2, ["VDET"] = 3 }, currentLimit: 0.1);

            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults();

            Assert.Equal(DCPowerMeasurementWhen.OnDemand, sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput.Measurement.MeasureWhen);
            if (pinMapWithChannelGroup)
            {
                Assert.Equal(2, results.Item1[0][0]);
                Assert.Equal(1, results.Item1[1][0]);
                Assert.Equal(3, results.Item1[1][1]);
            }
            else
            {
                Assert.Equal(2, results.Item1[0][0]);
                Assert.Equal(1, results.Item1[1][0]);
                Assert.Equal(3, results.Item1[2][0]);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentVoltageOnDifferentSMUDevices_MeasurePerInstrumentResultsOnMeasureTrigger_ReturnCorrectValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VCC", "VDET" });
            sessionsBundle.SetMeasureWhen(DCPowerMeasurementWhen.OnMeasureTrigger);
            sessionsBundle.ForceVoltageSymmetricLimit(voltageLevels: new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 2, ["VDET"] = 3 }, currentLimit: 0.1);

            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults();

            Assert.Equal(DCPowerMeasurementWhen.OnMeasureTrigger, sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput.Measurement.MeasureWhen);
            if (pinMapWithChannelGroup)
            {
                Assert.Equal(2, results.Item1[0][0]);
                Assert.Equal(1, results.Item1[1][0]);
                Assert.Equal(3, results.Item1[1][1]);
            }
            else
            {
                Assert.Equal(2, results.Item1[0][0]);
                Assert.Equal(1, results.Item1[1][0]);
                Assert.Equal(3, results.Item1[2][0]);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentVoltageOnDifferentSMUDevices_MeasurePerInstrumentResultsAutomaticallyAfterSourceComplete_ReturnCorrectValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });
            sessionsBundle.SetMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            sessionsBundle.ForceVoltageSymmetricLimit(voltageLevels: new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 2, ["VDET"] = 3 }, currentLimit: 0.1);

            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults();

            Assert.Equal(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Measurement.MeasureWhen);
            if (pinMapWithChannelGroup)
            {
                Assert.Equal(1, results.Item1[0][0]);
                Assert.Equal(3, results.Item1[0][1]);
                Assert.Equal(2, results.Item1[1][0]);
            }
            else
            {
                Assert.Equal(1, results.Item1[0][0]);
                Assert.Equal(2, results.Item1[1][0]);
                Assert.Equal(3, results.Item1[2][0]);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentCurrentOnDifferentSMUDevices_MeasurePerInstrumentResults_ReturnCorrectValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD" });
            var currentLevels = new Dictionary<string, DCPowerSettings>()
            {
                ["VCC"] = new DCPowerSettings() { SourceSettings = new DCPowerSourceSettings() { Level = 0.1, Limit = 5 } },
                ["VDD"] = new DCPowerSettings() { SourceSettings = new DCPowerSourceSettings() { Level = 0.2, Limit = 5 } }
            };
            sessionsBundle.ForceCurrentSymmetricLimit(currentLevels);

            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults();

            Assert.Equal(0.1, results.Item2[0][0]);
            Assert.Equal(0.2, results.Item2[1][0]);
            Assert.Equal(0.1, results.Item2[2][0]);
            Assert.Equal(0.2, results.Item2[3][0]);
            Assert.Equal(0.1, results.Item2[4][0]);
            Assert.Equal(0.2, results.Item2[5][0]);
            Assert.Equal(0.1, results.Item2[6][0]);
            Assert.Equal(0.2, results.Item2[7][0]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentCurrentOnDifferentSMUDevices_MeasurePerSiteResults_ReturnCorrectValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD" });
            var currentLevels = new Dictionary<string, DCPowerSettings>()
            {
                ["VCC"] = new DCPowerSettings() { SourceSettings = new DCPowerSourceSettings() { Level = 0.1, Limit = 5 } },
                ["VDD"] = new DCPowerSettings() { SourceSettings = new DCPowerSourceSettings() { Level = 0.2, Limit = 5 } }
            };
            sessionsBundle.ForceCurrentSymmetricLimit(currentLevels);

            var results = sessionsBundle.MeasureAndReturnPerSitePerPinResults();

            Assert.Equal(0.1, results.Item2.ExtractSite(0)["VCC"]);
            Assert.Equal(0.2, results.Item2.ExtractSite(0)["VDD"]);
            Assert.Equal(0.1, results.Item2.ExtractSite(1)["VCC"]);
            Assert.Equal(0.2, results.Item2.ExtractSite(1)["VDD"]);
            Assert.Equal(0.1, results.Item2.ExtractSite(2)["VCC"]);
            Assert.Equal(0.2, results.Item2.ExtractSite(2)["VDD"]);
            Assert.Equal(0.1, results.Item2.ExtractSite(3)["VCC"]);
            Assert.Equal(0.2, results.Item2.ExtractSite(3)["VDD"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SessionsInitialized_ConfigureAndStartWaveformAcquisition_OriginalSettingsAreCorrectlyReturned(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var expectedOriginalApertureTime = 0.017;
            var expectedOriginalMeasureWhen = DCPowerMeasurementWhen.OnDemand;
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(expectedOriginalApertureTime, sessionInfo.ChannelOutput.Measurement.ApertureTime, 3);
                Assert.Equal(expectedOriginalMeasureWhen, sessionInfo.ChannelOutput.Measurement.MeasureWhen);
            });

            var originalSettings = sessionsBundle.ConfigureAndStartWaveformAcquisition(sampleRate: 50, bufferLength: 10);

            foreach (var settings in originalSettings.SiteNumbers.SelectMany(siteNumber => originalSettings.ExtractSite(siteNumber).Values))
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
            var sessionsBundle = sessionManager.DCPower("VDD");
            var originalSttings = sessionsBundle.ConfigureAndStartWaveformAcquisition(sampleRate: 50, bufferLength: 10);

            var results = sessionsBundle.FinishWaveformAcquisition(fetchWaveformLength: 2, originalSttings);

            foreach (var siteNumber in results.SiteNumbers)
            {
                var result = results.ExtractSite(siteNumber);
                Assert.Equal(0.02, result["VDD"].DeltaTime);
                // pointsToFetch = fetchWaveformLength / deltaTime = 2 / 0.02 = 100
                Assert.Equal(100, result["VDD"].Result.VoltageMeasurements.Length);
            }
        }
    }
}
