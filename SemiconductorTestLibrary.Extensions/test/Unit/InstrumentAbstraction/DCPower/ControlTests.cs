using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DCPower
{
    [Collection("NonParallelizable")]
    public sealed class ControlTests : IDisposable
    {
        /* Connections defined in pinmap:
         * VCC - Chassis:1 - PXI4110 - Channel:0
         * VCC - Chassis:2 - PXI4130 - Channel:0
         * VCC - Chassis:3 - PXIe4154 - Channel:0
         * VCC - Chassis:4 - PXIe4112 - Channel:0
         * VDD - Chassis:1 - PXIe4147 - Channel:0
         * VDD - Chassis:2 - PXIe4147 - Channel:0
         * VDD - Chassis:3 - PXIe4147 - Channel:0
         * VDD - Chassis:4 - PXIe4147 - Channel:0
         * VDET - Chassis:1 - PXI4110 - Channel:1
         * VDET - Chassis:2 - PXI4130 - Channel:1
         * VDET - Chassis:3 - PXIe4154 - Channel:1
         * VDET - Chassis:4 - PXIe4112 - Channel:1
         */

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
        public void DifferentSMUDevices_SetMeasureWhen_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            sessionsBundle.SetMeasureWhen(DCPowerMeasurementWhen.OnMeasureTrigger);

            Assert.Equal(DCPowerMeasurementWhen.OnDemand, sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput.Measurement.MeasureWhen);
            Assert.Equal(DCPowerMeasurementWhen.OnDemand, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Measurement.MeasureWhen);
            Assert.Equal(DCPowerMeasurementWhen.OnMeasureTrigger, sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput.Measurement.MeasureWhen);
            Assert.Equal(DCPowerMeasurementWhen.OnMeasureTrigger, sessionsBundle.InstrumentSessions.ElementAt(3).ChannelOutput.Measurement.MeasureWhen);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetPowerLineFrequency_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");
            var defaultValues = sessionsBundle.GetPowerLineFrequencies();
            Assert.Equal(0, defaultValues.ExtractSite(0)["VCC"]);
            Assert.Equal(0, defaultValues.ExtractSite(1)["VCC"]);
            Assert.Equal(0, defaultValues.ExtractSite(2)["VCC"]);
            Assert.Equal(60, defaultValues.ExtractSite(3)["VCC"]);

            sessionsBundle.SetPowerLineFrequency(50);

            var setValues = sessionsBundle.GetPowerLineFrequencies();
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

            sessionsBundle.SetMeasurementSense(DCPowerMeasurementSense.Remote);

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
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Measurement.Sense);
                // 4130
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(3).ChannelOutput.Measurement.Sense);
                // 4154
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(4).ChannelOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(5).ChannelOutput.Measurement.Sense);
                // 4112
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(6).ChannelOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(7).ChannelOutput.Measurement.Sense);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetMeasurementSenseLocal_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            sessionsBundle.SetMeasurementSense(DCPowerMeasurementSense.Local);

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
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Measurement.Sense);
                // 4130
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(3).ChannelOutput.Measurement.Sense);
                // 4154
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(4).ChannelOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(5).ChannelOutput.Measurement.Sense);
                // 4112
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(6).ChannelOutput.Measurement.Sense);
                Assert.Equal(DCPowerMeasurementSense.Remote, sessionsBundle.InstrumentSessions.ElementAt(7).ChannelOutput.Measurement.Sense);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetSameCurrentLimitWithoutCurrentLimitRange_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            sessionsBundle.SetCurrentLimit(0.1);

            Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput.Source.Voltage.CurrentLimit);
            Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Source.Voltage.CurrentLimit);
            Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput.Source.Voltage.CurrentLimit);
            Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(3).ChannelOutput.Source.Voltage.CurrentLimit);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetPerPinCurrentLimitWithoutCurrentLimitRange_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });

            sessionsBundle.SetCurrentLimits(new Dictionary<string, double>()
            {
                ["VDET"] = 0.01,
                ["VDD"] = 0.1,
                ["VCC"] = 1
            });

            if (pinMapWithChannelGroup)
            {
                Assert.Equal(8, sessionsBundle.InstrumentSessions.Count());
                Assert.Equal(1, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"].Source.Voltage.CurrentLimit);
                Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.Voltage.CurrentLimit);
                Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"].Source.Voltage.CurrentLimit);
            }
            else
            {
                // 4 sites * 3 single-channel sessions = 12 sessions totally
                Assert.Equal(12, sessionsBundle.InstrumentSessions.Count());
                Assert.Equal(1, sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput.Source.Voltage.CurrentLimit);
                Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Source.Voltage.CurrentLimit);
                Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput.Source.Voltage.CurrentLimit);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesSetSameCurrentLimit_GetCurrentLimits_GetTheSameValue(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");
            sessionsBundle.SetCurrentLimit(0.1);

            var values = sessionsBundle.GetCurrentLimits();

            Assert.Equal(0.1, values.ExtractSite(0)["VCC"]);
            Assert.Equal(0.1, values.ExtractSite(1)["VCC"]);
            Assert.Equal(0.1, values.ExtractSite(2)["VCC"]);
            Assert.Equal(0.1, values.ExtractSite(3)["VCC"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesSetPerPinCurrentLimit_GetCurrentLimits_GetPerDeviceValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });
            sessionsBundle.SetCurrentLimits(new Dictionary<string, double>()
            {
                ["VDET"] = 0.01,
                ["VDD"] = 0.1,
                ["VCC"] = 1
            });

            var values = sessionsBundle.GetCurrentLimits();

            Assert.Equal(1, values.ExtractSite(0)["VCC"]);
            Assert.Equal(0.1, values.ExtractSite(1)["VDD"]);
            Assert.Equal(0.01, values.ExtractSite(2)["VDET"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetSequence_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.SetMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequence = new double[] { 1, 2, 3, 4, 5 };
            // Cannot test sequence loop count because it's not supported in offline mode.
            sessionsBundle.SetSequence(expectedSequence, sequenceLoopCount: 1);
            var results = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                sessionInfo.ChannelOutput.Control.Initiate();
                // Cannot test fetch backlog because it's always 1 in offline mode.
                return sessionInfo.Session.Measurement.Fetch(sessionInfo.ChannelString, PrecisionTimeSpan.FromSeconds(1), 5);
            });

            Assert.Equal(expectedSequence, results[0].VoltageMeasurements);
            Assert.Equal(expectedSequence, results[1].VoltageMeasurements);
            Assert.Equal(expectedSequence, results[2].VoltageMeasurements);
            Assert.Equal(expectedSequence, results[3].VoltageMeasurements);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetSequenceWithStepDeltaTime_SequenceStepDeltaTimeEnabled(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.False(sessionInfo.ChannelOutput.Source.SequenceStepDeltaTimeEnabled);
            });
            sessionsBundle.SetSequence(sequence: new double[] { 1, 2, 3 }, sequenceLoopCount: 1, sequenceStepDeltaTimeInSeconds: 0.05);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.True(sessionInfo.ChannelOutput.Source.SequenceStepDeltaTimeEnabled);
                Assert.Equal(0.05, sessionInfo.ChannelOutput.Source.SequenceStepDeltaTime.TotalSeconds);
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_GetApertureTimes_CorrectValuesAreGet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            var apertureTimes = sessionsBundle.GetApertureTimesInSeconds(out var maximumApertureTime);

            Assert.Equal(0.0033, apertureTimes.ExtractSite(0)["VCC"], 4);
            Assert.Equal(0.0033, apertureTimes.ExtractSite(1)["VCC"], 4);
            Assert.Equal(0.0017, apertureTimes.ExtractSite(2)["VCC"], 4);
            Assert.Equal(0.017, apertureTimes.ExtractSite(3)["VCC"], 3);
            Assert.Equal(0.017, maximumApertureTime, 3);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndAllInVoltageMode_CheckVoltageModeAndLevels_ReturnsTrue(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            var result = sessionsBundle.CheckVoltageModeAndLevels(out _);

            Assert.True(result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndTwoInCurrentMode_CheckVoltageModeAndLevels_ReturnsFalse(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");
            sessionsBundle.Do((sessionInfo, sessionIndex) =>
            {
                if (sessionIndex < 2)
                {
                    sessionInfo.ChannelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                }
            });

            var result = sessionsBundle.CheckVoltageModeAndLevels(out var failedChannels);

            Assert.False(result);
            Assert.Equal(2, failedChannels.Count());
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(0).ChannelString, failedChannels);
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(1).ChannelString, failedChannels);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesForcedDifferentVoltageLevels_CheckVoltageModeAndLevelsWithMatchingValues_ReturnsTrue(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD" });
            var voltageLevels = new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 2 };
            sessionsBundle.ForceVoltageSymmetricLimit(voltageLevels, currentLimit: 0.1);

            var result = sessionsBundle.CheckVoltageModeAndLevels(out _, expectedVoltages: voltageLevels);

            Assert.True(result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesForcedDifferentVoltageLevels_CheckVoltageModeAndLevelsWithMismatchingValues_ReturnsFalse(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD" });
            var voltageLevels = new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 2 };
            sessionsBundle.ForceVoltageSymmetricLimit(voltageLevels, currentLimit: 0.1);

            var result = sessionsBundle.CheckVoltageModeAndLevels(out var failedChannels, expectedVoltages: new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 3 });

            Assert.False(result);
            Assert.Equal(4, failedChannels.Count());
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(1).ChannelString, failedChannels);
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(3).ChannelString, failedChannels);
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(5).ChannelString, failedChannels);
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(7).ChannelString, failedChannels);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureSameSourceSettings_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            var settings = new DCPowerSettings()
            {
                SourceSettings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    Level = 0.2,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Asymmetric,
                    LimitHigh = 3,
                    LimitLow = -1,
                    SourceDelayInSeconds = 0.02
                }
            };
            sessionsBundle.ConfigureDCPowerSettings(settings);

            Assert.Equal(settings.SourceSettings.OutputFunction, sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput.Source.Output.Function);
            Assert.Equal(settings.SourceSettings.Level, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Source.Current.CurrentLevel);
            Assert.Equal(settings.SourceSettings.LimitSymmetry, sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput.Source.ComplianceLimitSymmetry);
            Assert.Equal(settings.SourceSettings.LimitHigh, sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput.Source.Current.VoltageLimitHigh);
            Assert.Equal(settings.SourceSettings.LimitLow, sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput.Source.Current.VoltageLimitLow);
            Assert.Equal(settings.SourceSettings.SourceDelayInSeconds.Value, sessionsBundle.InstrumentSessions.ElementAt(3).ChannelOutput.Source.SourceDelay.TotalSeconds);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureDifferentSourceSettings_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });

            var settingsForVCC = new DCPowerSettings()
            {
                SourceSettings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 0.1,
                    Limit = 3
                }
            };
            var settingsForVDD = new DCPowerSettings()
            {
                SourceSettings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 3.6,
                    Limit = 0.1,
                    TransientResponse = DCPowerSourceTransientResponse.Fast
                }
            };
            var settingsForVDET = new DCPowerSettings()
            {
                SourceSettings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 5,
                    Limit = 0.2,
                    SourceDelayInSeconds = 0.02
                }
            };
            var settings = new Dictionary<string, DCPowerSettings>()
            {
                ["VCC"] = settingsForVCC,
                ["VDD"] = settingsForVDD,
                ["VDET"] = settingsForVDET
            };
            sessionsBundle.ConfigureDCPowerSettings(settings);

            if (pinMapWithChannelGroup)
            {
                Assert.Equal(settingsForVCC.SourceSettings.Level, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"].Source.Current.CurrentLevel);
                Assert.Equal(DCPowerSourceTransientResponse.Normal, sessionsBundle.InstrumentSessions.ElementAt(4).Session.Outputs["SMU_4154_C3_S04/0"].Source.TransientResponse);
                Assert.Equal(settingsForVDD.SourceSettings.Limit, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.Voltage.CurrentLimit);
                Assert.Equal(settingsForVDD.SourceSettings.TransientResponse, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.TransientResponse);
                Assert.Equal(0.017, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.SourceDelay.TotalSeconds, 3);
                Assert.Equal(settingsForVDET.SourceSettings.SourceDelayInSeconds, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"].Source.SourceDelay.TotalSeconds);
            }
            else
            {
                Assert.Equal(settingsForVCC.SourceSettings.Level, sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput.Source.Current.CurrentLevel);
                Assert.Equal(DCPowerSourceTransientResponse.Normal, sessionsBundle.InstrumentSessions.ElementAt(6).ChannelOutput.Source.TransientResponse);
                Assert.Equal(settingsForVDD.SourceSettings.Limit, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Source.Voltage.CurrentLimit);
                Assert.Equal(settingsForVDD.SourceSettings.TransientResponse, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Source.TransientResponse);
                Assert.Equal(0.017, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Source.SourceDelay.TotalSeconds, 3);
                Assert.Equal(settingsForVDET.SourceSettings.SourceDelayInSeconds, sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput.Source.SourceDelay.TotalSeconds);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureSameMeasureSettings_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            var settings = new DCPowerSettings()
            {
                MeasureSettings = new DCPowerMeasureSettings()
                {
                    ApertureTime = 5,
                    ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.PowerLineCycles,
                }
            };
            sessionsBundle.SetPowerLineFrequency(50);
            sessionsBundle.ConfigureDCPowerSettings(settings);

            var apertureTimes = sessionsBundle.GetApertureTimesInSeconds(out _);
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

            var settingsForVCC = new DCPowerSettings()
            {
                MeasureSettings = new DCPowerMeasureSettings()
                {
                    ApertureTime = 5,
                    ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.PowerLineCycles,
                }
            };
            var settingsForVDD = new DCPowerSettings()
            {
                MeasureSettings = new DCPowerMeasureSettings()
                {
                    MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete,
                    RecordLength = 2
                }
            };
            var settingsForVDET = new DCPowerSettings()
            {
                MeasureSettings = new DCPowerMeasureSettings()
                {
                    Sense = DCPowerMeasurementSense.Remote
                }
            };
            var settings = new Dictionary<string, DCPowerSettings>()
            {
                ["VCC"] = settingsForVCC,
                ["VDD"] = settingsForVDD,
                ["VDET"] = settingsForVDET
            };
            sessionsBundle.ConfigureDCPowerSettings(settings);

            if (pinMapWithChannelGroup)
            {
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"].Measurement.Sense);
                Assert.Equal(settingsForVCC.MeasureSettings.ApertureTime, sessionsBundle.InstrumentSessions.ElementAt(6).Session.Outputs["SMU_4112_C4_S04/0"].Measurement.ApertureTime);
                Assert.Equal(settingsForVCC.MeasureSettings.ApertureTimeUnits, sessionsBundle.InstrumentSessions.ElementAt(6).Session.Outputs["SMU_4112_C4_S04/0"].Measurement.ApertureTimeUnits);
                Assert.Equal(DCPowerMeasureApertureTimeUnits.Seconds, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Measurement.ApertureTimeUnits);
                Assert.Equal(settingsForVDD.MeasureSettings.MeasureWhen, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Measurement.MeasureWhen);
                Assert.Equal(settingsForVDD.MeasureSettings.RecordLength, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Measurement.RecordLength);
                Assert.Equal(DCPowerSourceTransientResponse.Normal, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.TransientResponse);
                Assert.Equal(DCPowerMeasurementWhen.OnDemand, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"].Measurement.MeasureWhen);
                Assert.Equal(1, sessionsBundle.InstrumentSessions.ElementAt(4).Session.Outputs["SMU_4154_C3_S04/1"].Measurement.RecordLength);
                Assert.Equal(settingsForVDET.MeasureSettings.Sense, sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4130_C2_S04/1"].Measurement.Sense);
            }
            else
            {
                Assert.Equal(DCPowerMeasurementSense.Local, sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput.Measurement.Sense);
                Assert.Equal(settingsForVCC.MeasureSettings.ApertureTime, sessionsBundle.InstrumentSessions.ElementAt(9).ChannelOutput.Measurement.ApertureTime);
                Assert.Equal(settingsForVCC.MeasureSettings.ApertureTimeUnits, sessionsBundle.InstrumentSessions.ElementAt(9).ChannelOutput.Measurement.ApertureTimeUnits);
                Assert.Equal(DCPowerMeasureApertureTimeUnits.Seconds, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Measurement.ApertureTimeUnits);
                Assert.Equal(settingsForVDD.MeasureSettings.MeasureWhen, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Measurement.MeasureWhen);
                Assert.Equal(settingsForVDD.MeasureSettings.RecordLength, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Measurement.RecordLength);
                Assert.Equal(DCPowerSourceTransientResponse.Normal, sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput.Source.TransientResponse);
                Assert.Equal(DCPowerMeasurementWhen.OnDemand, sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput.Measurement.MeasureWhen);
                Assert.Equal(1, sessionsBundle.InstrumentSessions.ElementAt(8).ChannelOutput.Measurement.RecordLength);
                Assert.Equal(settingsForVDET.MeasureSettings.Sense, sessionsBundle.InstrumentSessions.ElementAt(5).ChannelOutput.Measurement.Sense);
            }
        }
    }
}
