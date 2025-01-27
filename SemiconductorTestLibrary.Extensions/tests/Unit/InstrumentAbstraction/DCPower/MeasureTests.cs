using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
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

        public TSMSessionManager Initialize(string pinMapFileName)
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
        public void DifferentSMUDevices_SetMeasureWhen_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.OnMeasureTrigger);

            Assert.Equal(DCPowerMeasurementWhen.OnDemand, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Measurement.MeasureWhen);
            Assert.Equal(DCPowerMeasurementWhen.OnDemand, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Measurement.MeasureWhen);
            Assert.Equal(DCPowerMeasurementWhen.OnMeasureTrigger, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Measurement.MeasureWhen);
            Assert.Equal(DCPowerMeasurementWhen.OnMeasureTrigger, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Measurement.MeasureWhen);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetPowerLineFrequency_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");
            var defaultValues = sessionsBundle.GetPowerLineFrequency();
            Assert.Equal(0, defaultValues.ExtractSite(0)["VCC"]);
            Assert.Equal(0, defaultValues.ExtractSite(1)["VCC"]);
            Assert.Equal(0, defaultValues.ExtractSite(2)["VCC"]);
            Assert.Equal(60, defaultValues.ExtractSite(3)["VCC"]);

            sessionsBundle.ConfigurePowerLineFrequency(50);

            var setValues = sessionsBundle.GetPowerLineFrequency();
            Assert.Equal(50, setValues.ExtractSite(0)["VCC"]);
            Assert.Equal(50, setValues.ExtractSite(1)["VCC"]);
            Assert.Equal(50, setValues.ExtractSite(2)["VCC"]);
            Assert.Equal(50, setValues.ExtractSite(3)["VCC"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetMeasurementSenseRemote_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            sessionsBundle.ConfigureMeasurementSense(DCPowerMeasurementSense.Remote);

            if (pinMapWithChannelGroup)
            {
                // 4110
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"].Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"].Measurement.Sense);
                // 4130
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/0"].Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/1"].Measurement.Sense);
                // 4154
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/0"].Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/1"].Measurement.Sense);
                // 4112
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/0"].Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/1"].Measurement.Sense);
            }
            else
            {
                // 4110
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Measurement.Sense);
                // 4130
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Measurement.Sense);
                // 4154
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(4).AllChannelsOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsOutput.Measurement.Sense);
                // 4112
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(7).AllChannelsOutput.Measurement.Sense);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetMeasurementSenseLocal_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            sessionsBundle.ConfigureMeasurementSense(DCPowerMeasurementSense.Local);

            if (pinMapWithChannelGroup)
            {
                // 4110
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"].Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"].Measurement.Sense);
                // 4130
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/0"].Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/1"].Measurement.Sense);
                // 4154
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/0"].Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/1"].Measurement.Sense);
                // 4112
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/0"].Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/1"].Measurement.Sense);
            }
            else
            {
                // 4110
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Measurement.Sense);
                // 4130
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Measurement.Sense);
                // 4154
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(4).AllChannelsOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsOutput.Measurement.Sense);
                // 4112
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(7).AllChannelsOutput.Measurement.Sense);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_GetApertureTimes_CorrectValuesAreGet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            var apertureTimes = sessionsBundle.GetApertureTimeInSeconds(out var maximumApertureTime);

            Assert.Equal(0.0033, apertureTimes.ExtractSite(0)["VCC"], 4);
            Assert.Equal(0.0033, apertureTimes.ExtractSite(1)["VCC"], 4);
            Assert.Equal(0.0017, apertureTimes.ExtractSite(2)["VCC"], 4);
            Assert.Equal(0.017, apertureTimes.ExtractSite(3)["VCC"], 3);
            Assert.Equal(0.017, maximumApertureTime, 3);
        }

        /*
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentVoltageOnDifferentSMUDevices_MeasurePerInstrumentResultsOnDemand_ReturnCorrectValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VCC", "VDET" });
            sessionsBundle.ForceVoltage(voltageLevels: new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 2, ["VDET"] = 3 }, currentLimit: 0.1);

            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults();

            Assert.Equal(DCPowerMeasurementWhen.OnDemand, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Measurement.MeasureWhen);
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
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.OnMeasureTrigger);
            sessionsBundle.ForceVoltage(voltageLevels: new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 2, ["VDET"] = 3 }, currentLimit: 0.1);

            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults();

            Assert.Equal(DCPowerMeasurementWhen.OnMeasureTrigger, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Measurement.MeasureWhen);
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
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            sessionsBundle.ForceVoltage(voltageLevels: new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 2, ["VDET"] = 3 }, currentLimit: 0.1);

            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults();

            Assert.Equal(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Measurement.MeasureWhen);
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
        */

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentCurrentOnDifferentSMUDevices_MeasurePerInstrumentResults_ReturnCorrectValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD" });
            var currentLevels = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC"] = new DCPowerSourceSettings() { Level = 0.1, Limit = 5 },
                ["VDD"] = new DCPowerSourceSettings() { Level = 0.2, Limit = 5 }
            };
            sessionsBundle.ForceCurrent(currentLevels);

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
            var currentLevels = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC"] = new DCPowerSourceSettings() { Level = 0.1, Limit = 5 },
                ["VDD"] = new DCPowerSourceSettings() { Level = 0.2, Limit = 5 }
            };
            sessionsBundle.ForceCurrent(currentLevels);

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
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
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
                Assert.Equal(expectedOriginalApertureTime, sessionInfo.AllChannelsOutput.Measurement.ApertureTime, 3);
                Assert.Equal(expectedOriginalMeasureWhen, sessionInfo.AllChannelsOutput.Measurement.MeasureWhen);
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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureMeasureSettings_CorrectValuesAreSetWithSameValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            var settings = new DCPowerMeasureSettings()
            {
                ApertureTime = 0.001,
                ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.Seconds,
                MeasureWhen = DCPowerMeasurementWhen.OnDemand,
                Sense = DCPowerMeasurementSense.Remote,
            };
            sessionsBundle.ConfigureMeasureSettings(settings);

            Assert.Equal(settings.ApertureTime.Value, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Measurement.ApertureTime);
            Assert.Equal(settings.ApertureTimeUnits.Value, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Measurement.ApertureTimeUnits);
            Assert.Equal(settings.MeasureWhen.Value, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Measurement.MeasureWhen);
            Assert.Equal(settings.Sense.Value, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Measurement.Sense);
            Assert.Equal(1, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Measurement.RecordLength);
        }

        [Theory]
        [InlineData("DifferentSMUDevicesForEachSiteSharedChannelGroup.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerInstr.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerCh.pinmap")]
        public void DifferentSMUDevices_ConfigureMeasureSettings_CorrectValuesAreSetWithPerPinValues(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var pinNames = new string[] { "VDD", "VEE" };
            var sessionsBundle = sessionManager.DCPower(pinNames);
            var siteCount = GetActiveSites(sessionsBundle);

            var settingsForVDD = new DCPowerMeasureSettings()
            {
                ApertureTime = 0.010,
                ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.Seconds,
                Sense = DCPowerMeasurementSense.Remote,
            };
            var settingsForVEE = new DCPowerMeasureSettings()
            {
                ApertureTime = 0.001,
                ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.Seconds,
                MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger,
            };

            var perPinSettings = new PinSiteData<DCPowerMeasureSettings>(
                pinNames,
                new SiteData<DCPowerMeasureSettings>[]
                {
                    new SiteData<DCPowerMeasureSettings>(siteCount, settingsForVDD),
                    new SiteData<DCPowerMeasureSettings>(siteCount, settingsForVEE)
                });

            sessionsBundle.ConfigureMeasureSettings(perPinSettings);

            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                if (pinSiteInfo.PinName == "VDD")
                {
                    var output = sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString];
                    Assert.Equal(settingsForVDD.ApertureTime.Value, output.Measurement.ApertureTime, 3);
                    Assert.Equal(settingsForVDD.ApertureTimeUnits.Value, output.Measurement.ApertureTimeUnits);
                    Assert.Equal(DCPowerMeasurementWhen.OnDemand, output.Measurement.MeasureWhen);
                    Assert.Equal(settingsForVDD.Sense, output.Measurement.Sense);
                }
                if (pinSiteInfo.PinName == "VEE")
                {
                    var output = sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString];
                    Assert.Equal(settingsForVEE.ApertureTime.Value, Math.Round(output.Measurement.ApertureTime, 3));
                    Assert.Equal(settingsForVEE.ApertureTimeUnits.Value, output.Measurement.ApertureTimeUnits);
                    Assert.Equal(settingsForVEE.MeasureWhen, output.Measurement.MeasureWhen);
                    Assert.Equal(DCPowerMeasurementSense.Local, output.Measurement.Sense);
                }
            });
        }

        [Theory]
        [InlineData("DifferentSMUDevicesForEachSiteSharedChannelGroup.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerInstr.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerCh.pinmap")]
        public void DifferentSMUDevices_GetPowerLineFrequency_GetPerSiteValues(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var defaultPowerLineFrequency = 60;
            var testValue = 50;

            // Test Default Value
            var values = sessionsBundle.GetPowerLineFrequency();
            Assert.Equal(defaultPowerLineFrequency, values.ExtractSite(0)["VDD"]);
            Assert.Equal(defaultPowerLineFrequency, values.ExtractSite(1)["VDD"]);
            Assert.Equal(defaultPowerLineFrequency, values.ExtractSite(2)["VDD"]);
            Assert.Equal(defaultPowerLineFrequency, values.ExtractSite(3)["VDD"]);

            // Test Updated Value
            sessionsBundle.Do(sessionInfo => sessionInfo.AllChannelsOutput.Measurement.PowerLineFrequency = testValue);
            values = sessionsBundle.GetPowerLineFrequency();
            Assert.Equal(testValue, values.ExtractSite(0)["VDD"]);
            Assert.Equal(testValue, values.ExtractSite(1)["VDD"]);
            Assert.Equal(testValue, values.ExtractSite(2)["VDD"]);
            Assert.Equal(testValue, values.ExtractSite(3)["VDD"]);
        }

        [Theory]
        [InlineData("DifferentSMUDevicesForEachSiteSharedChannelGroup.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerInstr.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerCh.pinmap")]
        public void DifferentSMUDevice_FetchResults_ReturnsValues(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });
            sessionsBundle.ForceVoltage(voltageLevel: 1, currentLimit: 0.1);

            var results = sessionsBundle.FetchMeasurement();

            foreach (var site in GetActiveSites(sessionsBundle))
            {
                Assert.NotNull(results.ExtractSite(site)["VDD"]);
            }
        }

        [Theory]
        [InlineData("DifferentSMUDevicesForEachSiteSharedChannelGroup.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerInstr.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerCh.pinmap")]
        public void DifferentSMUDevice_FetchResults_ThrowsExceptionForUnsupportedPins(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VCC");

            sessionsBundle.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });
            sessionsBundle.ForceVoltage(voltageLevel: 1, currentLimit: 0.1);

            void Operation() => sessionsBundle.FetchMeasurement();

            AggregateException aggregateException = Assert.Throws<AggregateException>(Operation);
            foreach (Exception innerExeption in aggregateException.InnerExceptions)
            {
                Assert.Contains("Function or method not supported.", innerExeption.InnerException.Message);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureSameMeasureSettings_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            var settings = new DCPowerMeasureSettings()
            {
                ApertureTime = 5,
                ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.PowerLineCycles,
            };
            sessionsBundle.ConfigurePowerLineFrequency(50);
            sessionsBundle.ConfigureMeasureSettings(settings);

            var apertureTimes = sessionsBundle.GetApertureTimeInSeconds(out _);
            Assert.Equal(0.1, apertureTimes.ExtractSite(0)["VCC"]);
            Assert.Equal(0.1, apertureTimes.ExtractSite(1)["VCC"]);
            Assert.Equal(0.1, apertureTimes.ExtractSite(2)["VCC"]);
            Assert.Equal(0.1, apertureTimes.ExtractSite(3)["VCC"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureDifferentMeasureSettings_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });
            sessionsBundle.Do(sessionInfo => sessionInfo.PowerLineFrequency = 1000);

            var settingsForVCC = new DCPowerMeasureSettings()
            {
                ApertureTime = 5,
                ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.PowerLineCycles,
            };
            var settingsForVDD = new DCPowerMeasureSettings()
            {
                MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete,
                RecordLength = 2
            };
            var settingsForVDET = new DCPowerMeasureSettings()
            {
                Sense = DCPowerMeasurementSense.Remote
            };
            var settings = new Dictionary<string, DCPowerMeasureSettings>()
            {
                ["VCC"] = settingsForVCC,
                ["VDD"] = settingsForVDD,
                ["VDET"] = settingsForVDET
            };
            sessionsBundle.ConfigureMeasureSettings(settings);

            if (pinMapWithChannelGroup)
            {
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"].Measurement.Sense);
                Assert.Equal(settingsForVCC.ApertureTime, sessionsBundle.InstrumentSessions.ElementAt(6).Session.Outputs["SMU_4112_C4_S04/0"].Measurement.ApertureTime);
                Assert.Equal(settingsForVCC.ApertureTimeUnits, sessionsBundle.InstrumentSessions.ElementAt(6).Session.Outputs["SMU_4112_C4_S04/0"].Measurement.ApertureTimeUnits);
                Assert.Equal(DCPowerMeasureApertureTimeUnits.Seconds, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Measurement.ApertureTimeUnits);
                Assert.Equal(settingsForVDD.MeasureWhen, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Measurement.MeasureWhen);
                Assert.Equal(settingsForVDD.RecordLength, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Measurement.RecordLength);
                Assert.Equal(DCPowerSourceTransientResponse.Normal, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.TransientResponse);
                Assert.Equal(DCPowerMeasurementWhen.OnDemand, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"].Measurement.MeasureWhen);
                Assert.Equal(1, sessionsBundle.InstrumentSessions.ElementAt(4).Session.Outputs["SMU_4154_C3_S04/1"].Measurement.RecordLength);
                Assert.Equal(settingsForVDET.Sense, sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4130_C2_S04/1"].Measurement.Sense);
            }
            else
            {
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Measurement.Sense);
                Assert.Equal(settingsForVCC.ApertureTime, sessionsBundle.InstrumentSessions.ElementAt(9).AllChannelsOutput.Measurement.ApertureTime);
                Assert.Equal(settingsForVCC.ApertureTimeUnits, sessionsBundle.InstrumentSessions.ElementAt(9).AllChannelsOutput.Measurement.ApertureTimeUnits);
                Assert.Equal(DCPowerMeasureApertureTimeUnits.Seconds, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Measurement.ApertureTimeUnits);
                Assert.Equal(settingsForVDD.MeasureWhen, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Measurement.MeasureWhen);
                Assert.Equal(settingsForVDD.RecordLength, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Measurement.RecordLength);
                Assert.Equal(DCPowerSourceTransientResponse.Normal, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.TransientResponse);
                Assert.Equal(DCPowerMeasurementWhen.OnDemand, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Measurement.MeasureWhen);
                Assert.Equal(1, sessionsBundle.InstrumentSessions.ElementAt(8).AllChannelsOutput.Measurement.RecordLength);
                Assert.Equal(settingsForVDET.Sense, sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsOutput.Measurement.Sense);
            }
        }

        [Fact]
        public void DifferentSMUDevices_ConfigureJustApertureTime_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("DifferentSMUDevices.pinmap");
            var sessionsBundle = sessionManager.DCPower("VCC");
            // 4112
            Assert.Equal(0.0167, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Measurement.ApertureTime, 4);

            var settings = new DCPowerMeasureSettings()
            {
                ApertureTime = 0.1,
            };
            sessionsBundle.ConfigureMeasureSettings(settings);

            // 4110
            Assert.Equal(300, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Measurement.SamplesToAverage);
            // 4130
            Assert.Equal(300, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Measurement.SamplesToAverage);
            // 4154
            Assert.Equal(30000, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Measurement.SamplesToAverage);
            // 4112
            Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Measurement.ApertureTime);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        public void DifferentSMUDevices_ConfigureJustApertureTimeUnits_Succeeds()
        {
            var sessionManager = Initialize("DifferentSMUDevices.pinmap");
            var sessionsBundle = sessionManager.DCPower("VCC");
            // 4112
            Assert.Equal(DCPowerMeasureApertureTimeUnits.Seconds, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Measurement.ApertureTimeUnits);

            var settings = new DCPowerMeasureSettings()
            {
                ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.PowerLineCycles,
            };
            sessionsBundle.ConfigureMeasureSettings(settings);

            // 4112
            Assert.Equal(DCPowerMeasureApertureTimeUnits.PowerLineCycles, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Measurement.ApertureTimeUnits);
        }

        [Fact]
        public void DifferentSMUDevices_ConfigureApertureTimeAndApertureTimeUnits_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("DifferentSMUDevices.pinmap");
            var sessionsBundle = sessionManager.DCPower("VCC");
            sessionsBundle.Do(sessionInfo => sessionInfo.PowerLineFrequency = 1000);

            var settings = new DCPowerMeasureSettings()
            {
                ApertureTime = 5,
                ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.PowerLineCycles,
            };
            sessionsBundle.ConfigureMeasureSettings(settings);

            // 4110
            Assert.Equal(15, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Measurement.SamplesToAverage);
            // 4130
            Assert.Equal(15, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Measurement.SamplesToAverage);
            // 4154
            Assert.Equal(1500, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Measurement.SamplesToAverage);
            // 4112
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Measurement.ApertureTime);
            Assert.Equal(DCPowerMeasureApertureTimeUnits.PowerLineCycles, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Measurement.ApertureTimeUnits);
        }

        [Fact]
        public void ApertureTimeUnitsSetToPowerLineCycles_ConfigureJustApertureTime_UnitDoesNotPersistForUnsupportedDevices()
        {
            var sessionManager = Initialize("DifferentSMUDevices.pinmap");
            var sessionsBundle = sessionManager.DCPower("VCC");
            var settings1 = new DCPowerMeasureSettings()
            {
                ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.PowerLineCycles,
            };
            sessionsBundle.ConfigureMeasureSettings(settings1);

            var settings2 = new DCPowerMeasureSettings()
            {
                ApertureTime = 0.1,
            };
            sessionsBundle.ConfigureMeasureSettings(settings2);

            // 4110
            Assert.Equal(300, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Measurement.SamplesToAverage);
        }

        [Theory]
        [InlineData("DifferentSMUDevicesForEachSiteSharedChannelGroup.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerInstr.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerCh.pinmap")]
        public void AllChannelsMeasureOnDemand_ForceCurrentMeasureVoltage_AllChannelsMeasured(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VDD");
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.OnDemand);

            sessionsBundle.ForceCurrent(currentLevel: 0.1, waitForSourceCompletion: true);
            var results = sessionsBundle.MeasureVoltage();

            AssertAllChannelsHaveResult(results);
        }

        [Theory]
        [InlineData("DifferentSMUDevicesForEachSiteSharedChannelGroup.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerInstr.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerCh.pinmap")]
        public void OneChannelMeasureOnTriggerOthersMeasureOnDemand_ForceCurrentMeasureVoltage_AllChannelsMeasured(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VDD");
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.OnDemand);
            string channel0String = sessionsBundle.InstrumentSessions.ElementAt(0).AssociatedSitePinList[0].IndividualChannelString;
            sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs[channel0String].Measurement.MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger;

            sessionsBundle.ForceCurrent(currentLevel: 0.1, waitForSourceCompletion: true);
            var results = sessionsBundle.MeasureVoltage();

            AssertAllChannelsHaveResult(results);
        }

        [Theory]
        [InlineData("DifferentSMUDevicesForEachSiteSharedChannelGroup.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerInstr.pinmap")]
        [InlineData("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerCh.pinmap")]
        public void AllChannelsMeasureAfterSourceComplete_ForceVoltageMeasureCurrent_AllChannelsMeasured(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VDD");
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            sessionsBundle.ForceVoltage(voltageLevel: 3.6, waitForSourceCompletion: true);
            var results = sessionsBundle.MeasureCurrent();

            AssertAllChannelsHaveResult(results);
        }

        private int[] GetActiveSites(DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.AggregateSitePinList
                .Where(sitePinInfo => sitePinInfo.SiteNumber != -1)
                .Select(sitePinInfo => sitePinInfo.SiteNumber)
                .Distinct()
                .ToArray();
        }

        private void AssertAllChannelsHaveResult(PinSiteData<double> results)
        {
            foreach (var siteNumber in results.SiteNumbers)
            {
                foreach (var pin in results.PinNames)
                {
                    Assert.NotEqual(0, results.GetValue(siteNumber, pin));
                }
            }
        }
    }
}
