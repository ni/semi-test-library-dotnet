using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower.Utilities;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DCPower
{
    [Collection("NonParallelizable")]
    public sealed class SourceTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        private const string TwoPinsGangedGroup = "TwoPinsGangedGroup";
        private const string ThreePinsGangedGroup = "ThreePinsGangedGroup";
        private const string FourPinsGangedGroup = "FourPinsGangedGroup";
        private const string AllPinsGangedGroup = "AllPinsGangedGroup";

        public TSMSessionManager Initialize(bool pinMapWithChannelGroup)
        {
            return Initialize(pinMapWithChannelGroup ? "DifferentSMUDevicesWithChannelGroup.pinmap" : "DifferentSMUDevices.pinmap");
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
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceSameVoltageWithSymmetricLimit_SameVoltageForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceVoltage(voltageLevel: 3.6, currentLimit: 0.1);

            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1));
            sessionsBundle.Do(sessionInfo => Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, sessionInfo.AllChannelsOutput.Source.ComplianceLimitSymmetry));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForcePerPinVoltagesWithSymmetricLimit_CorrectVoltagesForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });

            sessionsBundle.ForceVoltage(voltageLevels: new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 2, ["VDET"] = 3 }, currentLimit: 0.1);

            if (pinMapWithChannelGroup)
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"], expectedVoltageLevel: 1, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"], expectedVoltageLevel: 2, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"], expectedVoltageLevel: 3, expectedCurrentLimit: 0.1);
            }
            else
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedVoltageLevel: 1, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedVoltageLevel: 2, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedVoltageLevel: 3, expectedCurrentLimit: 0.1);
            }
        }

        [Theory]
        [InlineData("DifferentSMUDevices.pinmap", false)]
        [InlineData("DifferentSMUDevicesOfSameModelSharedChannelGroup.pinmap", true)]
        public void DifferentSMUDevices_ForcePerSiteVoltagesWithSymmetricLimit_CorrectVoltageMeasurementsFetched(string pinMapFileName, bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VDD");

            var voltageLevels = new SiteData<double>(new[] { 3.5, 3.6, 3.7, 3.8 });
            sessionsBundle.ForceVoltage(voltageLevels, currentLimit: 0.1);

            if (pinMapWithChannelGroup)
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C1_S11/0"], expectedVoltageLevel: 3.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C2_S10/0"], expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C3_S12/0"], expectedVoltageLevel: 3.7, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C4_S18/0"], expectedVoltageLevel: 3.8, expectedCurrentLimit: 0.1);
            }
            else
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedVoltageLevel: 3.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedVoltageLevel: 3.7, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput, expectedVoltageLevel: 3.8, expectedCurrentLimit: 0.1);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForcePerPinPerSiteVoltagesWithSymmetricLimit_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            var voltageLevels = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VCC"] = new Dictionary<int, double>() { [0] = 1, [1] = 2, [2] = 3, [3] = 4 },
                ["VDET"] = new Dictionary<int, double>() { [0] = 1.5, [1] = 2.5, [2] = 3.5, [3] = 4.5 }
            });
            sessionsBundle.ForceVoltage(voltageLevels, currentLimit: 0.1);

            if (pinMapWithChannelGroup)
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"], expectedVoltageLevel: 1, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"], expectedVoltageLevel: 1.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/0"], expectedVoltageLevel: 2, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/1"], expectedVoltageLevel: 2.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/0"], expectedVoltageLevel: 3, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/1"], expectedVoltageLevel: 3.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/0"], expectedVoltageLevel: 4, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/1"], expectedVoltageLevel: 4.5, expectedCurrentLimit: 0.1);
            }
            else
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedVoltageLevel: 1, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedVoltageLevel: 1.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedVoltageLevel: 2, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput, expectedVoltageLevel: 2.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(4).AllChannelsOutput, expectedVoltageLevel: 3, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsOutput, expectedVoltageLevel: 3.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput, expectedVoltageLevel: 4, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(7).AllChannelsOutput, expectedVoltageLevel: 4.5, expectedCurrentLimit: 0.1);
            }
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequenceSynchronized_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { 0.000, 0.005, 0.010 };
            sessionsBundle.ForceVoltageSequenceSynchronized(voltageSequence: sequence, currentLimit: 0.5, voltageLevelRange: 1.0, currentLimitRange: 0.5);

            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: 0.5));
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequenceSynchronizedWithPerPinPerSiteValues_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            var sequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.001, 0.002, 0.003 },
                    [1] = new[] { 0.004, 0.005, 0.006 },
                    [2] = new[] { 0.007, 0.008, 0.009 },
                    [3] = new[] { 0.010, 0.011, 0.012 }
                }
            });
            var currentLimits = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 1.0, [1] = 1.1, [2] = 1.2, [3] = 1.3 }
            });
            var voltageLevelRanges = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 1.0, [1] = 1.0, [2] = 1.0, [3] = 1.0 }
            });
            var currentLimitRanges = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 1.5, [1] = 1.5, [2] = 1.5, [3] = 1.5 }
            });
            sessionsBundle.ForceVoltageSequenceSynchronized(
                voltageSequence: sequences,
                currentLimit: currentLimits,
                voltageLevelRange: voltageLevelRanges,
                currentLimitRange: currentLimitRanges);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: currentLimits.GetValue(sitePinInfo.SiteNumber, "VDD"));
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequenceSynchronizedWithPerSiteValues_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            // Create per-site sequences
            var sequences = new SiteData<double[]>(new[]
            {
                new[] { 0.001, 0.002, 0.003 },
                new[] { 0.004, 0.005, 0.006 },
                new[] { 0.007, 0.008, 0.009 },
                new[] { 0.010, 0.011, 0.012 }
            });
            var currentLimits = new SiteData<double>(new double[] { 1.0, 1.1, 1.2, 1.3 });
            var voltageLevelRanges = new SiteData<double>(new double[] { 1.0, 1.0, 1.0, 1.0 });
            var currentLimitRanges = new SiteData<double>(new double[] { 1.5, 1.5, 1.5, 1.5 });
            sessionsBundle.ForceVoltageSequenceSynchronized(
                voltageSequence: sequences,
                currentLimit: currentLimits,
                voltageLevelRange: voltageLevelRanges,
                currentLimitRange: currentLimitRanges);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: currentLimits.GetValue(sitePinInfo.SiteNumber));
            });
        }

        [Theory]
        [InlineData("DifferentSMUDevices.pinmap")]
        [InlineData("DifferentSMUDevicesOfSameModelSharedChannelGroup.pinmap")]
        public void DifferentSMUDevices_ForceVoltageWithSingleSettingsObject_CorrectVoltageForced(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceVoltage(new DCPowerSourceSettings() { Level = 3.6, Limit = 0.1 });

            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageWithPerPinSettingsObject_CorrectVoltagesForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            var settings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC"] = new DCPowerSourceSettings() { Level = 3.6, Limit = 0.1 },
                ["VDET"] = new DCPowerSourceSettings() { Level = 5.0, Limit = 0.2 }
            };
            sessionsBundle.ForceVoltage(settings);

            if (pinMapWithChannelGroup)
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"], expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"], expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/0"], expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/1"], expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/0"], expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/1"], expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/0"], expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/1"], expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
            }
            else
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput, expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(4).AllChannelsOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsOutput, expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(7).AllChannelsOutput, expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
            }
        }

        [Theory]
        [InlineData("DifferentSMUDevices.pinmap", false)]
        [InlineData("DifferentSMUDevicesOfSameModelSharedChannelGroup.pinmap", true)]
        public void DifferentSMUDevices_ForceVoltageWithPerSiteSettingsObject_CorrectVoltagesForced(string pinMapFileName, bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VDD");

            var settings = new SiteData<DCPowerSourceSettings>(new[]
            {
                new DCPowerSourceSettings() { Level = 3.5, Limit = 0.1 },
                new DCPowerSourceSettings() { Level = 3.6, Limit = 0.2 },
                new DCPowerSourceSettings() { Level = 3.7, Limit = 0.1 },
                new DCPowerSourceSettings() { Level = 3.8, Limit = 0.2 }
            });
            sessionsBundle.ForceVoltage(settings);

            if (pinMapWithChannelGroup)
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C1_S11/0"], expectedVoltageLevel: 3.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C2_S10/0"], expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C3_S12/0"], expectedVoltageLevel: 3.7, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C4_S18/0"], expectedVoltageLevel: 3.8, expectedCurrentLimit: 0.2);
            }
            else
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedVoltageLevel: 3.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedVoltageLevel: 3.7, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput, expectedVoltageLevel: 3.8, expectedCurrentLimit: 0.2);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageWithPerPinPerSiteSettingsObject_CorrectVoltagesForced(bool pinMapWithChannelGroup)
        {
            var pinNames = new string[] { "VCC", "VDET" };
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(pinNames);

            var settings = new PinSiteData<DCPowerSourceSettings>(pinNames, new int[] { 0, 1, 2, 3 }, new DCPowerSourceSettings[][]
            {
                new DCPowerSourceSettings[]
                {
                    new DCPowerSourceSettings() { Level = 1, Limit = 0.1 },
                    new DCPowerSourceSettings() { Level = 1.5, Limit = 0.2 },
                    new DCPowerSourceSettings() { Level = 2, Limit = 0.1 },
                    new DCPowerSourceSettings() { Level = 2.5, Limit = 0.2 }
                },
                new DCPowerSourceSettings[]
                {
                    new DCPowerSourceSettings() { Level = 3, Limit = 0.1 },
                    new DCPowerSourceSettings() { Level = 3.5, Limit = 0.2 },
                    new DCPowerSourceSettings() { Level = 4, Limit = 0.1 },
                    new DCPowerSourceSettings() { Level = 4.5, Limit = 0.2 }
                }
            });
            sessionsBundle.ForceVoltage(settings);

            if (pinMapWithChannelGroup)
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"], expectedVoltageLevel: 1, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"], expectedVoltageLevel: 3, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/0"], expectedVoltageLevel: 1.5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/1"], expectedVoltageLevel: 3.5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/0"], expectedVoltageLevel: 2, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/1"], expectedVoltageLevel: 4, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/0"], expectedVoltageLevel: 2.5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/1"], expectedVoltageLevel: 4.5, expectedCurrentLimit: 0.2);
            }
            else
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedVoltageLevel: 1, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedVoltageLevel: 3, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedVoltageLevel: 1.5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput, expectedVoltageLevel: 3.5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(4).AllChannelsOutput, expectedVoltageLevel: 2, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsOutput, expectedVoltageLevel: 4, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput, expectedVoltageLevel: 2.5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(7).AllChannelsOutput, expectedVoltageLevel: 4.5, expectedCurrentLimit: 0.2);
            }
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceDifferentVoltageWithPerPinPerSiteSettingsOnGangedPins_ThrowsException()
        {
            var pinNames = new string[] { "VCC1", "VCC2", "VCC3" };
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var settings = new PinSiteData<DCPowerSourceSettings>(pinNames, new int[] { 0, 1 }, new DCPowerSourceSettings[][]
            {
                new DCPowerSourceSettings[]
                {
                    new DCPowerSourceSettings() { Level = 1, Limit = 0.1 },
                    new DCPowerSourceSettings() { Level = 1.5, Limit = 0.2 },
                },
                new DCPowerSourceSettings[]
                {
                    new DCPowerSourceSettings() { Level = 3, Limit = 0.1 },
                    new DCPowerSourceSettings() { Level = 3.5, Limit = 0.2 },
                },
                new DCPowerSourceSettings[]
                {
                    new DCPowerSourceSettings() { Level = 3, Limit = 0.1 },
                    new DCPowerSourceSettings() { Level = 3.5, Limit = 0.2 },
                }
            });
            void ForceVoltageTest() => sessionsBundle.ForceVoltage(settings);

            var exception = Assert.Throws<AggregateException>(ForceVoltageTest);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different values for Cascaded pins", exception.InnerException.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedWithPerSiteValuesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new SiteData<DCPowerSourceSettings[]>(new[]
            {
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 2.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 2.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 3.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 3.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 4.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 4.5,
                        Limit = 0.1
                    }
                }
            });
            sessionsBundle.ForceAdvancedSequenceSynchronized(sequence, sequenceLoopCount: 1, waitForSequenceCompletion: true, sequenceTimeoutInSeconds: 5.0);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedWithPerPinPerSiteValuesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var vddSequence = new DCPowerSourceSettings[]
            {
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 1.0,
                    Limit = 0.1
                },
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 2.0,
                    Limit = 0.1
                }
            };
            var sequence = new PinSiteData<DCPowerSourceSettings[]>(
                new[] { "VDD" },
                new[]
                {
                    new SiteData<DCPowerSourceSettings[]>(sites, vddSequence)
                });
            sessionsBundle.ForceAdvancedSequenceSynchronized(sequence, sequenceLoopCount: 1, waitForSequenceCompletion: true, sequenceTimeoutInSeconds: 10.0);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithInconsistenceProperties_ThrowsException(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new PinSiteData<DCPowerSourceSettings[]>(
                new[] { "VDD" },
                new[]
                {
                    new SiteData<DCPowerSourceSettings[]>(
                        sites,
                        new[]
                        {
                            new DCPowerSourceSettings
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                                Level = 1.0,
                                Limit = 0.1,
                                TransientResponse = DCPowerSourceTransientResponse.Fast
                            },
                            new DCPowerSourceSettings
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                                Level = 1.5
                            }
                        })
                });
            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0));

            Assert.Contains("Inconsistent advanced sequence properties. The following properties must be either specified or omitted for all steps in the sequence", exception.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetch_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new[]
            {
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 1.0,
                    Limit = 0.1
                },
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 1.5,
                    Limit = 0.1
                },
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 2.0,
                    Limit = 0.1
                }
            };
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 3,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithPerSiteSequence_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequencePerSite = new SiteData<DCPowerSourceSettings[]>(new[]
            {
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.5,
                        Limit = 0.1
                    }
                }
            });
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequencePerSite,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithPerPinPerSiteSequence_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new PinSiteData<DCPowerSourceSettings[]>(
                new[] { "VDD" },
                new[]
                {
                    new SiteData<DCPowerSourceSettings[]>(
                        sites,
                        new[]
                        {
                            new DCPowerSourceSettings
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                                Level = 1.0,
                                Limit = 0.1
                            },
                            new DCPowerSourceSettings
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                                Level = 1.5,
                                Limit = 0.1
                            }
                        })
                });
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithInconsistencesPerSiteDCPowerAdvancedSequenceStepProperties_ThrowsExceptions(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequencePerSite = new SiteData<DCPowerAdvancedSequenceStepProperties[]>(
                sites,
                sites.Select(_ => new[]
                {
                    new DCPowerAdvancedSequenceStepProperties
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        VoltageLevel = 1.0,
                        CurrentLimit = 0.1,
                        CurrentLevel = 0.05,
                        TransientResponse = DCPowerSourceTransientResponse.Slow,
                        CurrentLevelRange = 0.2,
                        VoltageLevelRange = 5.0
                    },
                    new DCPowerAdvancedSequenceStepProperties
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        VoltageLevel = 1.5,
                        CurrentLimit = 0.1
                    }
                }).ToArray());
            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequencePerSite,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0));

            Assert.Contains("Inconsistent advanced sequence properties. The following properties must be either specified or omitted for all steps in the sequence", exception.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithPerSiteDCPowerAdvancedSequenceStepProperties_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequencePerSite = new SiteData<DCPowerAdvancedSequenceStepProperties[]>(
                sites,
                sites.Select(_ => new[]
                {
                    new DCPowerAdvancedSequenceStepProperties
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        VoltageLevel = 1.0,
                        CurrentLimit = 0.1
                    },
                    new DCPowerAdvancedSequenceStepProperties
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        VoltageLevel = 1.5,
                        CurrentLimit = 0.1
                    }
                }).ToArray());
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequencePerSite,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithDCPowerAdvancedSequenceStepProperties_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new[]
            {
                new DCPowerAdvancedSequenceStepProperties
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    VoltageLevel = 1.0,
                    CurrentLimit = 0.1
                },
                new DCPowerAdvancedSequenceStepProperties
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    VoltageLevel = 1.5,
                    CurrentLimit = 0.1
                },
                new DCPowerAdvancedSequenceStepProperties
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    VoltageLevel = 2.0,
                    CurrentLimit = 0.1
                }
            };
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 3,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithPerPinPerSiteDCPowerAdvancedSequenceStepProperties_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new PinSiteData<DCPowerAdvancedSequenceStepProperties[]>(
                new[] { "VDD" },
                new[]
                {
                    new SiteData<DCPowerAdvancedSequenceStepProperties[]>(
                        sites,
                        new[]
                        {
                            new DCPowerAdvancedSequenceStepProperties
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                VoltageLevel = 1.0,
                                CurrentLimit = 0.1
                            },
                            new DCPowerAdvancedSequenceStepProperties
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                VoltageLevel = 1.5,
                                CurrentLimit = 0.1
                            }
                        })
                });
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureAdvancedSequence_CorrectValueAreSet(bool setAsActiveSequence)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var steps = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, ApertureTime = 0.016, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, SourceDelay = 0.5, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            const string sequenceName = "ScalarAdvancedSequence";
            sessionsBundle.ConfigureAdvancedSequence(
                sequenceName,
                steps,
                setAsActiveSequence: setAsActiveSequence,
                commitFirstElementAsInitialState: false);

            if (setAsActiveSequence && !_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => new double[] { 1.0, 2.0, 3.0 }, precision: 3, itemsToFetch: 3, checkForCurrentMeasurement: false);
            }
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                var adv = output.Source.AdvancedSequencing;
                if (setAsActiveSequence)
                {
                    Assert.Equal(sequenceName, adv.ActiveAdvancedSequence);
                }
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureAdvancedSequenceWithPerSiteDataPerStep_CorrectValueAreSet(bool setAsActiveSequence)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var perSiteSteps = new SiteData<IList<DCPowerAdvancedSequenceStepProperties>>(new[]
            {
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { CurrentLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCCurrent },
                    new DCPowerAdvancedSequenceStepProperties { CurrentLevel = 1.2, OutputFunction = DCPowerSourceOutputFunction.DCCurrent }
                },
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { CurrentLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCCurrent },
                    new DCPowerAdvancedSequenceStepProperties { CurrentLevel = 2.2, OutputFunction = DCPowerSourceOutputFunction.DCCurrent }
                },
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                },
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 4.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 4.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                }
            });
            const string sequenceName = "PerSiteAdvancedSequence";
            sessionsBundle.ConfigureAdvancedSequence(
                sequenceName,
                perSiteSteps,
                setAsActiveSequence: setAsActiveSequence,
                commitFirstElementAsInitialState: false);

            if (setAsActiveSequence && !_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                var siteData = new SiteData<double[]>(new[]
                {
                    new double[] { 1.0, 1.2 },
                    new double[] { 2.0, 2.2 }
                });
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => siteData.GetValue(siteNumber), precision: 3, itemsToFetch: 3, checkForCurrentMeasurement: true);
            }
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                var adv = output.Source.AdvancedSequencing;
                if (setAsActiveSequence)
                {
                    Assert.Equal(sequenceName, adv.ActiveAdvancedSequence);
                }
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureAdvancedSequenceWithPinSiteDataPerStep_CorrectValueAreSet(bool setAsActiveSequence)
        {
            var pinNames = new[] { "VDD" };
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower(pinNames);

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var perSiteSteps = new Dictionary<string, IDictionary<int, IList<DCPowerAdvancedSequenceStepProperties>>>
            {
                ["VDD"] = new Dictionary<int, IList<DCPowerAdvancedSequenceStepProperties>>
                {
                    [0] = new List<DCPowerAdvancedSequenceStepProperties>
                    {
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                    },
                    [1] = new List<DCPowerAdvancedSequenceStepProperties>
                    {
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                    },
                    [2] = new List<DCPowerAdvancedSequenceStepProperties>
                    {
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                    },
                    [3] = new List<DCPowerAdvancedSequenceStepProperties>
                    {
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 4.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 4.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                    }
                }
            };
            var pinSiteData = new PinSiteData<IList<DCPowerAdvancedSequenceStepProperties>>(perSiteSteps);
            const string sequenceName = "PerPinPerSiteAdvancedSequence";
            sessionsBundle.ConfigureAdvancedSequence(
                sequenceName,
                pinSiteData,
                setAsActiveSequence: setAsActiveSequence,
                commitFirstElementAsInitialState: true);

            if (setAsActiveSequence && !_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                var data = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
                {
                    ["VDD"] = new Dictionary<int, double[]>()
                    {
                        [0] = new double[] { 1.0, 1.2 },
                        [1] = new double[] { 2.0, 2.2 }
                    }
                });
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => data.GetValue(siteNumber, pinName), precision: 3, itemsToFetch: 3, checkForCurrentMeasurement: false);
            }
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                var adv = output.Source.AdvancedSequencing;
                if (setAsActiveSequence)
                {
                    Assert.Equal(sequenceName, adv.ActiveAdvancedSequence);
                }
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureAdvancedSequenceWithNotSupportedProperties_ThrowsException(bool setAsActiveSequence)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var steps = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, LcrVoltageRange = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            const string sequenceName = "AdvancedSequenceException";
            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureAdvancedSequence(
                sequenceName,
                steps,
                setAsActiveSequence: setAsActiveSequence,
                commitFirstElementAsInitialState: false));

            Assert.Contains("The specified instrument (NI PXIe-4147) does not support one or more of the requested advanced sequence properties.", exception.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceSynchronized_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { 0.000, 0.005, 0.010 };
            sessionsBundle.ForceCurrentSequenceSynchronized(currentSequence: sequence, voltageLimit: 0.5, currentLevelRange: 0.1, voltageLimitRange: 0.5);

            sessionsBundle.Abort();
            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                Assert.Equal(0.1, sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Source.Current.CurrentLevelRange);
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceSynchronizedWithPerPinPerSiteValues_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            var currentSequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.001, 0.002, 0.003 },
                    [1] = new[] { 0.004, 0.005, 0.006 },
                    [2] = new[] { 0.007, 0.008, 0.009 },
                    [3] = new[] { 0.010, 0.011, 0.012 }
                }
            });
            var voltageLimits = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 1.0, [1] = 1.1, [2] = 1.2, [3] = 1.3 }
            });
            var currentLevelRanges = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 0.1, [1] = 0.1, [2] = 0.1, [3] = 0.1 }
            });
            var voltageLimitRanges = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 1.5, [1] = 1.5, [2] = 1.5, [3] = 1.5 }
            });
            sessionsBundle.ForceCurrentSequenceSynchronized(
                currentSequence: currentSequence,
                voltageLimit: voltageLimits,
                currentLevelRange: currentLevelRanges,
                voltageLimitRange: voltageLimitRanges);

            sessionsBundle.Abort();
            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                Assert.Equal(currentLevelRanges.GetValue(sitePinInfo.SiteNumber, "VDD"), sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Source.Current.CurrentLevelRange);
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceSynchronizedWithPerSiteValues_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            // Create per-site sequences
            var currentSequences = new SiteData<double[]>(new[]
            {
                new[] { 0.001, 0.002, 0.003 },
                new[] { 0.004, 0.005, 0.006 },
                new[] { 0.007, 0.008, 0.009 },
                new[] { 0.010, 0.011, 0.012 }
            });
            var voltageLimits = new SiteData<double>(new double[] { 1.0, 1.1, 1.2, 1.3 });
            var currentLevelRanges = new SiteData<double>(new double[] { 0.1, 0.1, 0.1, 0.1 });
            var voltageLimitRanges = new SiteData<double>(new double[] { 1.5, 1.5, 1.5, 1.5 });
            sessionsBundle.ForceCurrentSequenceSynchronized(
                currentSequence: currentSequences,
                voltageLimit: voltageLimits,
                currentLevelRange: currentLevelRanges,
                voltageLimitRange: voltageLimitRanges);

            sessionsBundle.Abort();
            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                Assert.Equal(currentLevelRanges.GetValue(sitePinInfo.SiteNumber), sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Source.Current.CurrentLevelRange, 2);
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageWithSymmetricLimitAndRangesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceVoltage(voltageLevel: 3.6, currentLimit: 0.1, voltageLevelRange: 5, currentLimitRange: 0.5);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceSameVoltageWithAsymmetricLimit_SameVoltageForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceVoltageAsymmetricLimit(voltageLevel: 3.6, currentLimitHigh: 0.2, currentLimitLow: -0.1);

            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedVoltageLevel: 3.6, expectedCurrentLimitHigh: 0.2, expectedCurrentLimitLow: -0.1));
            sessionsBundle.Do(sessionInfo => Assert.Equal(DCPowerComplianceLimitSymmetry.Asymmetric, sessionInfo.AllChannelsOutput.Source.ComplianceLimitSymmetry));
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageWithAsymmetricLimitAndRangesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceVoltageAsymmetricLimit(voltageLevel: 3.6, currentLimitHigh: 0.2, currentLimitLow: -0.1, voltageLevelRange: 5, currentLimitRange: 0.5);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequence_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { 1.0, 1.5, 2.0, 2.5, 3.0 };
            var currentLimit = 0.2;
            var currentLimitRange = 3.0;
            sessionsBundle.ForceVoltageSequence(
                voltageSequence: sequence,
                currentLimit: currentLimit,
                voltageLevelRange: 5.0,
                currentLimitRange: currentLimitRange,
                sequenceLoopCount: 1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: currentLimit, expectedCurrentLimitRange: currentLimitRange));
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequenceSiteData_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new[]
            {
                new[] { 1.0, 1.1, 1.2 },
                new[] { 2.0, 2.1, 2.2 },
                new[] { 3.0, 3.1, 3.2 },
                new[] { 4.0, 4.1, 4.2 }
            });
            var currentLimit = 0.3;
            var currentLimitRange = 3;
            sessionsBundle.ForceVoltageSequence(
                voltageSequence: sequence,
                currentLimit: currentLimit,
                voltageLevelRange: 6.0,
                currentLimitRange: currentLimitRange,
                sequenceLoopCount: 1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: currentLimit, expectedCurrentLimitRange: (double?)currentLimitRange));
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequencePinSiteData_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>
                {
                    [0] = new[] { 1.0, 1.1, 1.2 },
                    [1] = new[] { 1.3, 1.4, 1.5 },
                    [2] = new[] { 1.6, 1.7, 1.8 },
                    [3] = new[] { 1.9, 2.0, 2.1 }
                }
            });
            var currentLimit = 0.25;
            var currentLimitRange = 3;
            sessionsBundle.ForceVoltageSequence(
                voltageSequence: sequence,
                currentLimit: currentLimit,
                voltageLevelRange: 7.0,
                currentLimitRange: currentLimitRange,
                sequenceLoopCount: 1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: currentLimit, expectedCurrentLimitRange: (double?)currentLimitRange));
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequenceWithDefaultParametersSequenceAppliedSuccessfully(bool pinMapWithChannelGroup)
        {
            var pinNames = new string[] { "VDD" };
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(pinNames);

            var sequence = new[] { 0.5, 1.0, 1.5 };
            sessionsBundle.ForceVoltageSequence(voltageSequence: sequence);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageSequence_CorrectValueAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { 0.5, 1.0, 1.5, 2.0 };
            sessionsBundle.ForceVoltageSequence(
                voltageSequence: sequence,
                currentLimit: 1.0,
                voltageLevelRange: 5.0,
                currentLimitRange: 3,
                sequenceLoopCount: 1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: 0.2));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageSequenceWithPerSiteSequence_CorrectValueAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new double[][]
            {
                new[] { 0.4, 1.0, 1.6 },
                new[] { 0.6, 1.1, 1.6 }
            });
            sessionsBundle.ForceVoltageSequence(voltageSequence: sequence, currentLimit: 1.5, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                AssertVoltageSettings(output, expectedCurrentLimit: sitePinInfo.CascadingInfo is GangingInfo ? 0.5 : 1.5);
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageSequenceWithSamePerPinPerSiteSequence_CorrectValueAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            sessionsBundle.ForceVoltageSequence(voltageSequence: sequence, currentLimit: 1.8, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: 0.6));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageSequenceWithDifferentPerPinPerSiteSequence_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 1.0, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            void ForceVoltageTest()
            {
                sessionsBundle.ForceVoltageSequence(voltageSequence: sequence, currentLimit: 1.8, sequenceLoopCount: 2);
            }

            sessionsBundle.Abort();
            var exception = Assert.Throws<AggregateException>(ForceVoltageTest);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different sequences for Cascaded pins", exception.InnerException.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceSameCurrentWithSymmetricLimit_SameCurrentForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceCurrent(currentLevel: 0.1, voltageLimit: 5);

            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 5));
            sessionsBundle.Do(sessionInfo => Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, sessionInfo.AllChannelsOutput.Source.ComplianceLimitSymmetry));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForcePerPinCurrentsWithSymmetricLimit_CorrectCurrentsForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });

            sessionsBundle.ForceCurrent(currentLevels: new Dictionary<string, double>() { ["VCC"] = 0.1, ["VDD"] = 0.2, ["VDET"] = 0.3 }, voltageLimit: 5);

            if (pinMapWithChannelGroup)
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"], expectedCurrentLevel: 0.1, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"], expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"], expectedCurrentLevel: 0.3, expectedVoltageLimit: 5);
            }
            else
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedCurrentLevel: 0.3, expectedVoltageLimit: 5);
            }
        }

        [Theory]
        [InlineData("DifferentSMUDevices.pinmap", false)]
        [InlineData("DifferentSMUDevicesOfSameModelSharedChannelGroup.pinmap", true)]
        public void DifferentSMUDevices_ForcePerSiteCurrentsWithSymmetricLimit_CorrectCurrentsForced(string pinMapFileName, bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VDD");

            var currentLevels = new SiteData<double>(new[] { 0.1, 0.2, 0.3, 0.4 });
            sessionsBundle.ForceCurrent(currentLevels, voltageLimit: 5);

            if (pinMapWithChannelGroup)
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C1_S11/0"], expectedCurrentLevel: 0.1, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C2_S10/0"], expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C3_S12/0"], expectedCurrentLevel: 0.3, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C4_S18/0"], expectedCurrentLevel: 0.4, expectedVoltageLimit: 5);
            }
            else
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedCurrentLevel: 0.3, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput, expectedCurrentLevel: 0.4, expectedVoltageLimit: 5);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForcePerPinPerSiteCurrentsWithSymmetricLimit_CorrectCurrentsForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            var currentLevels = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VCC"] = new Dictionary<int, double>() { [0] = 0.1, [1] = 0.2, [2] = 0.3, [3] = 0.4 },
                ["VDET"] = new Dictionary<int, double>() { [0] = 0.15, [1] = 0.25, [2] = 0.35, [3] = 0.45 }
            });
            sessionsBundle.ForceCurrent(currentLevels, voltageLimit: 5);

            if (pinMapWithChannelGroup)
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"], expectedCurrentLevel: 0.1, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"], expectedCurrentLevel: 0.15, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/0"], expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/1"], expectedCurrentLevel: 0.25, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/0"], expectedCurrentLevel: 0.3, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/1"], expectedCurrentLevel: 0.35, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/0"], expectedCurrentLevel: 0.4, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/1"], expectedCurrentLevel: 0.45, expectedVoltageLimit: 5);
            }
            else
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedCurrentLevel: 0.15, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput, expectedCurrentLevel: 0.25, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(4).AllChannelsOutput, expectedCurrentLevel: 0.3, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsOutput, expectedCurrentLevel: 0.35, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput, expectedCurrentLevel: 0.4, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(7).AllChannelsOutput, expectedCurrentLevel: 0.45, expectedVoltageLimit: 5);
            }
        }

        [Theory]
        [InlineData("DifferentSMUDevices.pinmap")]
        [InlineData("DifferentSMUDevicesOfSameModelSharedChannelGroup.pinmap")]
        public void DifferentSMUDevices_ForceCurrentWithSingleSettingsObject_CorrectCurrentForced(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceCurrent(new DCPowerSourceSettings() { Level = 0.1, Limit = 3.6 });

            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 3.6));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentWithPerPinSettingsObject_CorrectCurrentsForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            var settings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC"] = new DCPowerSourceSettings() { Level = 0.1, Limit = 3 },
                ["VDET"] = new DCPowerSourceSettings() { Level = 0.2, Limit = 5 }
            };
            sessionsBundle.ForceCurrent(settings);

            if (pinMapWithChannelGroup)
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"], expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"], expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/0"], expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/1"], expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/0"], expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/1"], expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/0"], expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/1"], expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
            }
            else
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(4).AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(7).AllChannelsOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
            }
        }

        [Theory]
        [InlineData("DifferentSMUDevices.pinmap", false)]
        [InlineData("DifferentSMUDevicesOfSameModelSharedChannelGroup.pinmap", true)]
        public void DifferentSMUDevices_ForceCurrentWithPerSiteSettingsObject_CorrectCurrentsForced(string pinMapFileName, bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower("VDD");

            var settings = new SiteData<DCPowerSourceSettings>(new[]
            {
                new DCPowerSourceSettings() { Level = 0.1, Limit = 3.6 },
                new DCPowerSourceSettings() { Level = 0.2, Limit = 5 },
                new DCPowerSourceSettings() { Level = 0.3, Limit = 3.6 },
                new DCPowerSourceSettings() { Level = 0.4, Limit = 5 }
            });
            sessionsBundle.ForceCurrent(settings);

            if (pinMapWithChannelGroup)
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C1_S11/0"], expectedCurrentLevel: 0.1, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C2_S10/0"], expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C3_S12/0"], expectedCurrentLevel: 0.3, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.Single().Session.Outputs["SMU_4147_C4_S18/0"], expectedCurrentLevel: 0.4, expectedVoltageLimit: 5);
            }
            else
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedCurrentLevel: 0.3, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput, expectedCurrentLevel: 0.4, expectedVoltageLimit: 5);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentWithPerPinPerSiteSettingsObject_CorrectCurrentsForced(bool pinMapWithChannelGroup)
        {
            var pinNames = new string[] { "VCC", "VDET" };
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(pinNames);

            var settings = new PinSiteData<DCPowerSourceSettings>(pinNames, new int[] { 0, 1, 2, 3 }, new DCPowerSourceSettings[][]
            {
                new DCPowerSourceSettings[]
                {
                    new DCPowerSourceSettings() { Level = 0.1, Limit = 3.6 },
                    new DCPowerSourceSettings() { Level = 0.15, Limit = 5 },
                    new DCPowerSourceSettings() { Level = 0.2, Limit = 3.6 },
                    new DCPowerSourceSettings() { Level = 0.25, Limit = 5 }
                },
                new DCPowerSourceSettings[]
                {
                    new DCPowerSourceSettings() { Level = 0.3, Limit = 3.6 },
                    new DCPowerSourceSettings() { Level = 0.35, Limit = 5 },
                    new DCPowerSourceSettings() { Level = 0.4, Limit = 3.6 },
                    new DCPowerSourceSettings() { Level = 0.45, Limit = 5 }
                }
            });
            sessionsBundle.ForceCurrent(settings);

            if (pinMapWithChannelGroup)
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"], expectedCurrentLevel: 0.1, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"], expectedCurrentLevel: 0.3, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/0"], expectedCurrentLevel: 0.15, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/1"], expectedCurrentLevel: 0.35, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/0"], expectedCurrentLevel: 0.2, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/1"], expectedCurrentLevel: 0.4, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/0"], expectedCurrentLevel: 0.25, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/1"], expectedCurrentLevel: 0.45, expectedVoltageLimit: 5);
            }
            else
            {
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput, expectedCurrentLevel: 0.3, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput, expectedCurrentLevel: 0.15, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput, expectedCurrentLevel: 0.35, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(4).AllChannelsOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsOutput, expectedCurrentLevel: 0.4, expectedVoltageLimit: 3.6);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput, expectedCurrentLevel: 0.25, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(7).AllChannelsOutput, expectedCurrentLevel: 0.45, expectedVoltageLimit: 5);
            }
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentWithSymmetricLimitAndRangesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceCurrent(currentLevel: 0.1, voltageLimit: 5, currentLevelRange: 0.5, voltageLimitRange: 5);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceSameCurrentWithAsymmetricLimit_SameCurrentForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceCurrentAsymmetricLimit(currentLevel: 0.1, voltageLimitHigh: 3, voltageLimitLow: -1);

            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedCurrentLevel: 0.1, expectedVoltageLimitHigh: 3, expectedVoltageLimitLow: -1));

            sessionsBundle.Do(sessionInfo => Assert.Equal(DCPowerComplianceLimitSymmetry.Asymmetric, sessionInfo.AllChannelsOutput.Source.ComplianceLimitSymmetry));
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentWithAsymmetricLimitAndRangesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceCurrentAsymmetricLimit(currentLevel: 0.1, voltageLimitHigh: 3, voltageLimitLow: -1, currentLevelRange: 0.5, voltageLimitRange: 5);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentWithSymmetricLimit_DividedCurrentForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ForceCurrent(currentLevel: 1.5, voltageLimit: 2);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertCurrentSettings(sitePinInfo, channelOutput, 0.5, 1.5, 2);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForcePerPinCurrentsWithSymmetricLimit_CorrectCurrentsForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ForceCurrent(currentLevels: new Dictionary<string, double>() { ["VCC1"] = 1, ["VCC2"] = 1, ["VCC3"] = 1, ["VCC4"] = 1, ["VCC5"] = 1 }, voltageLimit: 5);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertCurrentSettings(channelOutput, expectedCurrentLevel: 1, expectedVoltageLimit: 5);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForcePerPinCurrentsWithSymmetricLimitOnPinGroupName_CorrectCurrentsForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ForceCurrent(currentLevels: new Dictionary<string, double>() { [ThreePinsGangedGroup] = 3, ["VCC4"] = 1, ["VCC5"] = 1 }, voltageLimit: 5);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertCurrentSettings(channelOutput, expectedCurrentLevel: 1, expectedVoltageLimit: 5);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForcePerSiteCurrentsWithSymmetricLimit_CorrectCurrentsForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var currentLevels = new SiteData<double>(new double[] { 1, 3 });
            sessionsBundle.ForceCurrent(currentLevels, voltageLimit: 3);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(channelOutput, expectedCurrentLevel: sitePinInfo.SiteNumber == 0 ? 0.2 : 0.6, expectedVoltageLimit: 3);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForcePerPinPerSiteCurrentsWithSymmetricLimit_CorrectCurrentsForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var currentLevels = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VCC1"] = new Dictionary<int, double>() { [0] = 0.5, [1] = 0.6 },
                ["VCC2"] = new Dictionary<int, double>() { [0] = 0.5, [1] = 0.6 },
                ["VCC3"] = new Dictionary<int, double>() { [0] = 0.5, [1] = 0.6 },
                ["VCC4"] = new Dictionary<int, double>() { [0] = 0.5, [1] = 0.6 },
                ["VCC5"] = new Dictionary<int, double>() { [0] = 0.5, [1] = 0.6 }
            });
            sessionsBundle.ForceCurrent(currentLevels, voltageLimit: 4.5);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(channelOutput, expectedCurrentLevel: sitePinInfo.SiteNumber == 0 ? 0.5 : 0.6, expectedVoltageLimit: 4.5);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForcePerPinPerSiteCurrentsWithSymmetricLimitOnPinGroupName_CorrectCurrentsForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var currentLevels = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["AllPinsGangedGroup"] = new Dictionary<int, double>() { [0] = 2.5, [1] = 3.0 }
            });
            sessionsBundle.ForceCurrent(currentLevels, voltageLimit: 4.5);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(channelOutput, expectedCurrentLevel: sitePinInfo.SiteNumber == 0 ? 0.5 : 0.6, expectedVoltageLimit: 4.5);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGangedinTwoPinGroups_ForceCurrentWithSingleSettingsObject_CorrectCurrentForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(TwoPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ForceCurrent(new DCPowerSourceSettings() { Level = 3, Limit = 3.6 });
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                if (sitePinInfo.PinName == "VCC4" || sitePinInfo.PinName == "VCC5")
                {
                    AssertCurrentSettings(channelOutput, 1.5, 3.6);
                    AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4147_C1_S11/2" : "SMU_4147_C2_S10/2");
                }
                else
                {
                    AssertCurrentSettings(channelOutput, 1, 3.6);
                    AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
                }
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageWithPerSiteSettingsObject_SameVoltageForcedAndCurrentLimitDividedEqually()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var settings = new SiteData<DCPowerSourceSettings>(new[]
            {
                new DCPowerSourceSettings() { Level = 3.6, Limit = 5 },
                new DCPowerSourceSettings() { Level = 5, Limit = 2 },
            });
            sessionsBundle.ForceVoltage(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                if (sitePinInfo.SiteNumber == 0)
                {
                    AssertVoltageSettings(channelOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 1);
                }
                else
                {
                    AssertVoltageSettings(channelOutput, expectedVoltageLevel: 5, expectedCurrentLimit: 0.4);
                }
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageWithPerPinSettingsObject_SameVoltageForcedAndCurrentLimitsAppliedCorrectly()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var settings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC1"] = new DCPowerSourceSettings() { Level = 4, Limit = 1 },
                ["VCC2"] = new DCPowerSourceSettings() { Level = 4, Limit = 1 },
                ["VCC3"] = new DCPowerSourceSettings() { Level = 4, Limit = 1 },
                ["VCC4"] = new DCPowerSourceSettings() { Level = 4, Limit = 1 },
                ["VCC5"] = new DCPowerSourceSettings() { Level = 4, Limit = 1 }
            };
            sessionsBundle.ForceVoltage(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertVoltageSettings(channelOutput, expectedVoltageLevel: 4, expectedCurrentLimit: 1);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageWithPerPinSettingsOnPinGroupName_SameVoltageForcedAndCurrentLimitDividedEqually()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var settings = new Dictionary<string, DCPowerSourceSettings>()
            {
                [ThreePinsGangedGroup] = new DCPowerSourceSettings() { Level = 4, Limit = 3 },
                ["VCC4"] = new DCPowerSourceSettings() { Level = 4, Limit = 1 },
                ["VCC5"] = new DCPowerSourceSettings() { Level = 4, Limit = 1 }
            };
            sessionsBundle.ForceVoltage(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertVoltageSettings(channelOutput, expectedVoltageLevel: 4, expectedCurrentLimit: 1);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceDifferentVoltageWithPerPinSettingsObjectOnGangedPins_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var settings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC1"] = new DCPowerSourceSettings() { Level = 2, Limit = 3 },
                ["VCC2"] = new DCPowerSourceSettings() { Level = 3, Limit = 3 },
                ["VCC3"] = new DCPowerSourceSettings() { Level = 4, Limit = 3 },
                ["VCC4"] = new DCPowerSourceSettings() { Level = 4, Limit = 2 },
                ["VCC5"] = new DCPowerSourceSettings() { Level = 4, Limit = 2 }
            };
            void ForceVoltageTest()
            {
                sessionsBundle.ForceVoltage(settings);
            }

            var exception = Assert.Throws<AggregateException>(ForceVoltageTest);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different values for Cascaded pins", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageWithPerPinPerSiteSettingsObject_SameVoltageForcedAndCurrentLimitDividedEqually()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(FourPinsGangedGroup);

            var settings = new PinSiteData<DCPowerSourceSettings>(
                new string[]
                {
                    "VCC1",
                    "VCC2",
                    "VCC3",
                    "VCC4",
                    "VCC5"
                },
                new int[] { 0, 1 },
                new DCPowerSourceSettings
                {
                    Level = 6,
                    Limit = 1
                });
            sessionsBundle.ForceVoltage(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertVoltageSettings(channelOutput, expectedVoltageLevel: 6, expectedCurrentLimit: 1);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageWithPerPinPerSiteSettingsOnPinGroupName_SameVoltageForcedAndCurrentLimitDividedEqually()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(FourPinsGangedGroup);

            var settings = new PinSiteData<DCPowerSourceSettings>(
                new string[]
                {
                    FourPinsGangedGroup,
                    "VCC5"
                },
                new int[] { 0, 1 },
                new DCPowerSourceSettings
                {
                    Level = 6,
                    Limit = 3
                });
            sessionsBundle.ForceVoltage(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertVoltageSettings(sitePinInfo, channelOutput, 6, 0.75, 3);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceSameVoltageWithAsymmetricLimit_SameVoltageForcedAndCurrentLimitDividedEqually()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            sessionsBundle.ForceVoltageAsymmetricLimit(voltageLevel: 2.5, currentLimitHigh: 3, currentLimitLow: -1);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Asymmetric, channelOutput.Source.ComplianceLimitSymmetry);
                Assert.Equal(2.5, channelOutput.Source.Voltage.VoltageLevel);
                Assert.Equal(0.6, channelOutput.Source.Voltage.CurrentLimitHigh);
                Assert.Equal(-0.2, channelOutput.Source.Voltage.CurrentLimitLow);
                AssertTriggerSettings(sessionInfo.AssociatedSitePinList[0], channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageOnFilteredBundleWithFewPins_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var filteredBundle = sessionsBundle.FilterByPin(new string[] { "VCC1", "VCC2" });
            void ForceVoltageOnFilteredBundle()
            {
                filteredBundle.ForceVoltage(2.0, 3.0);
            }

            var exception = Assert.Throws<AggregateException>(ForceVoltageOnFilteredBundle);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("not present in the DCPowerSessionsBundle", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageOnSubsetBundleWithTwoPins_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var subsetBundle = sessionManager.DCPower(TwoPinsGangedGroup);
            void ForceVoltageOnSubsetBundle()
            {
                subsetBundle.ForceVoltage(2.0, 3.0);
            }

            var exception = Assert.Throws<AggregateException>(ForceVoltageOnSubsetBundle);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("not present in the DCPowerSessionsBundle", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_FilterBundleWithFewPinsAndUngangThenForceVoltage_CorrectVoltageForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var filteredBundle = sessionsBundle.FilterByPin(new string[] { "VCC1", "VCC2" });
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
            filteredBundle.ForceVoltage(new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                Level = 1,
                Limit = 1,
            });

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (sitePinInfo.PinName == "VCC1" || sitePinInfo.PinName == "VCC2")
                {
                    var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                    AssertVoltageSettings(channelOutput, 1, 1);
                }
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentWithSingleSettingsObject_CorrectCurrentForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(FourPinsGangedGroup);

            sessionsBundle.ForceCurrent(new DCPowerSourceSettings() { Level = 2, Limit = 2.6 });

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(sitePinInfo, channelOutput, 0.5, 2, 2.6);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageWithSymmetricLimit_CurrentLimitDividedCorrectly()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ForceVoltage(voltageLevel: 2, currentLimit: 1.5);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertVoltageSettings(sitePinInfo, channelOutput, 2, 0.5, 1.5);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForcePerPinVoltagesWithSymmetricLimit_VoltagesForcedAndCurrentLimitDividedCorrectly()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var voltageLevels = new Dictionary<string, double>() { ["VCC1"] = 2, ["VCC2"] = 2, ["VCC3"] = 2, ["VCC4"] = 2, ["VCC5"] = 2 };

            sessionsBundle.ForceVoltage(voltageLevels, currentLimit: 0.6);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertVoltageSettings(sitePinInfo, channelOutput, 2, 0.2, 0.6);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForcePerPinVoltagesWithSymmetricLimitOnPinGroupName_VoltagesForcedAndCurrentLimitDividedCorrectly()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var voltageLevels = new Dictionary<string, double>() { [ThreePinsGangedGroup] = 2, ["VCC4"] = 2, ["VCC5"] = 2 };

            sessionsBundle.ForceVoltage(voltageLevels, currentLimit: 0.6);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertVoltageSettings(sitePinInfo, channelOutput, 2, 0.2, 0.6);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceDifferentPerPinVoltagesWithSymmetricLimitOnGangedPins_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            void ForceVoltageTest()
            {
                sessionsBundle.ForceVoltage(voltageLevels: new Dictionary<string, double>() { ["VCC1"] = 1, ["VCC2"] = 2, ["VCC3"] = 3, ["VCC4"] = 2, ["VCC5"] = 2 }, currentLimit: 0.6);
            }

            var exception = Assert.Throws<AggregateException>(ForceVoltageTest);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different values for Cascaded pins", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForcePerSiteVoltagesWithSymmetricLimit_VoltagesForcedAndCurrentLimitDividedCorrectly()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var voltageLevels = new SiteData<double>(new double[] { 1, 3 });
            sessionsBundle.ForceVoltage(voltageLevels, currentLimit: 3);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                if (sitePinInfo.SiteNumber == 0)
                {
                    AssertVoltageSettings(channelOutput, expectedVoltageLevel: 1, expectedCurrentLimit: 0.6);
                }
                else
                {
                    AssertVoltageSettings(channelOutput, expectedVoltageLevel: 3, expectedCurrentLimit: 0.6);
                }
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForcePerPinPerSiteVoltagesWithSymmetricLimit_CorrectVoltagesForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var voltageLevels = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VCC1"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 },
                ["VCC2"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 },
                ["VCC3"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 },
                ["VCC4"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 },
                ["VCC5"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 }
            });
            sessionsBundle.ForceVoltage(voltageLevels, currentLimit: 4.5);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                AssertVoltageSettings(channelOutput, expectedVoltageLevel: sitePinInfo.SiteNumber == 0 ? 4.0 : 2.5, expectedCurrentLimit: 0.9);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceDifferentPerPinVoltagesOnGangedPins_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var voltageLevels = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VCC1"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 },
                ["VCC2"] = new Dictionary<int, double>() { [0] = 2, [1] = 2.5 },
                ["VCC3"] = new Dictionary<int, double>() { [0] = 2, [1] = 2.5 },
                ["VCC4"] = new Dictionary<int, double>() { [0] = 4, [1] = 4 },
                ["VCC5"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 }
            });
            void ForceVoltageTest()
            {
                sessionsBundle.ForceVoltage(voltageLevels, currentLimit: 4.5);
            }

            var exception = Assert.Throws<AggregateException>(ForceVoltageTest);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different values for Cascaded pins", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGangedinTwoPinGroups_ForceVoltageWithSingleSettingsObject_CorrectVoltageForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(TwoPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ForceVoltage(new DCPowerSourceSettings() { Level = 3.6, Limit = 3 });

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                if (sitePinInfo.PinName == "VCC4" || sitePinInfo.PinName == "VCC5")
                {
                    AssertVoltageSettings(channelOutput, 3.6, 1.5);
                    AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4147_C1_S11/2" : "SMU_4147_C2_S10/2");
                }
                else
                {
                    AssertVoltageSettings(channelOutput, 3.6, 1);
                    AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
                }
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageWithSingleSettingsObject_CorrectVoltageForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(FourPinsGangedGroup);

            sessionsBundle.ForceVoltage(new DCPowerSourceSettings() { Level = 2.6, Limit = 2 });

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                AssertVoltageSettings(sitePinInfo, channelOutput, 2.6, 0.5, 2);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentWithPerSiteSettingsObject_CorrectCurrentsForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var settings = new SiteData<DCPowerSourceSettings>(new[]
            {
                new DCPowerSourceSettings() { Level = 1, Limit = 3.6 },
                new DCPowerSourceSettings() { Level = 0.5, Limit = 5 },
            });
            sessionsBundle.ForceCurrent(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(channelOutput, expectedCurrentLevel: sitePinInfo.SiteNumber == 0 ? 0.2 : 0.1, expectedVoltageLimit: sitePinInfo.SiteNumber == 0 ? 3.6 : 5);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentWithPerPinSettingsObject_CorrectCurrentsForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var settings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC1"] = new DCPowerSourceSettings() { Level = 1, Limit = 4 },
                ["VCC2"] = new DCPowerSourceSettings() { Level = 1, Limit = 4 },
                ["VCC3"] = new DCPowerSourceSettings() { Level = 1, Limit = 4 },
                ["VCC4"] = new DCPowerSourceSettings() { Level = 1, Limit = 4 },
                ["VCC5"] = new DCPowerSourceSettings() { Level = 1, Limit = 4 }
            };
            sessionsBundle.ForceCurrent(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertCurrentSettings(channelOutput, expectedCurrentLevel: 1, expectedVoltageLimit: 4);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentWithDifferentPerPinSettingsObject_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var settings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC1"] = new DCPowerSourceSettings() { Level = 1, Limit = 4 },
                ["VCC2"] = new DCPowerSourceSettings() { Level = 1, Limit = 4 },
                ["VCC3"] = new DCPowerSourceSettings() { Level = 1, Limit = 3 },
                ["VCC4"] = new DCPowerSourceSettings() { Level = 1, Limit = 3 },
                ["VCC5"] = new DCPowerSourceSettings() { Level = 1, Limit = 3 }
            };

            void ForceCurrent()
            {
                sessionsBundle.ForceCurrent(settings);
            }

            var exception = Assert.Throws<AggregateException>(ForceCurrent);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different values for Cascaded pins", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentWithPerPinSettingsObjectOnPinGroupName_CorrectCurrentsForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var settings = new Dictionary<string, DCPowerSourceSettings>()
            {
                [ThreePinsGangedGroup] = new DCPowerSourceSettings() { Level = 3, Limit = 4 },
                ["VCC4"] = new DCPowerSourceSettings() { Level = 1, Limit = 4 },
                ["VCC5"] = new DCPowerSourceSettings() { Level = 1, Limit = 4 }
            };
            sessionsBundle.ForceCurrent(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertCurrentSettings(channelOutput, expectedCurrentLevel: 1, expectedVoltageLimit: 4);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentWithPerPinPerSiteSettingsObject_CorrectCurrentsForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(FourPinsGangedGroup);

            var settings = new PinSiteData<DCPowerSourceSettings>(
                new string[]
                {
                    "VCC1",
                    "VCC2",
                    "VCC3",
                    "VCC4",
                    "VCC5"
                },
                new int[] { 0, 1 },
                new DCPowerSourceSettings
                {
                    Level = 0.75,
                    Limit = 6
                });
            sessionsBundle.ForceCurrent(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertCurrentSettings(channelOutput, expectedCurrentLevel: 0.75, expectedVoltageLimit: 6);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentWithPerPinPerSiteSettingsObjectOnPinGroupName_CorrectCurrentsForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(FourPinsGangedGroup);

            var settings = new PinSiteData<DCPowerSourceSettings>(
                new string[]
                {
                    FourPinsGangedGroup,
                    "VCC5"
                },
                new int[] { 0, 1 },
                new DCPowerSourceSettings
                {
                    Level = 1,
                    Limit = 6
                });
            sessionsBundle.ForceCurrent(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                AssertCurrentSettings(sitePinInfo, channelOutput, 0.25, 1.0, 6);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceSameCurrentWithAsymmetricLimit_SameCurrentForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            sessionsBundle.ForceCurrentAsymmetricLimit(currentLevel: 2.5, voltageLimitHigh: 3, voltageLimitLow: -1);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Asymmetric, channelOutput.Source.ComplianceLimitSymmetry);
                Assert.Equal(0.5, channelOutput.Source.Current.CurrentLevel);
                Assert.Equal(3, channelOutput.Source.Current.VoltageLimitHigh);
                Assert.Equal(-1, channelOutput.Source.Current.VoltageLimitLow);
                AssertTriggerSettings(sessionInfo.AssociatedSitePinList[0], channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentOnFilteredBundleWithFewPins_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var filteredBundle = sessionsBundle.FilterByPin(new string[] { "VCC1", "VCC2" });
            void ForceCurrentOnFilteredBundle()
            {
                filteredBundle.ForceCurrent(1.0, 3.0);
            }

            var exception = Assert.Throws<AggregateException>(ForceCurrentOnFilteredBundle);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("not present in the DCPowerSessionsBundle", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentOnSubsetBundleWithTwoPins_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var subsetBundle = sessionManager.DCPower(TwoPinsGangedGroup);
            void ForceCurrentOnSubsetBundle()
            {
                subsetBundle.ForceCurrent(1.0, 3.0);
            }

            var exception = Assert.Throws<AggregateException>(ForceCurrentOnSubsetBundle);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("not present in the DCPowerSessionsBundle", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_FilterBundleWithFewPinsAndUngangThenForceCurrent_CorrectCurrentForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var filteredBundle = sessionsBundle.FilterByPin(new string[] { "VCC1", "VCC2" });
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
            filteredBundle.ForceCurrent(new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 1,
                Limit = 1,
            });

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (sitePinInfo.PinName == "VCC1" || sitePinInfo.PinName == "VCC2")
                {
                    var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                    AssertCurrentSettings(channelOutput, 1, 1);
                }
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequence = new[] { 0.000, 0.005, 0.010 };

            sessionsBundle.ForceCurrentSequence(sequence);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { -0.02, 0.03, 0.04, 0.05, 0.07 };
            sessionsBundle.ForceCurrentSequence(currentSequence: sequence, voltageLimit: 0.5, currentLevelRange: 0.1, voltageLimitRange: 1, sequenceLoopCount: 1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedVoltageLimit: 0.5, expectedSequenceLoopCount: 1, expectedCurrentLevelRange: 0.1, expectedVoltageLimitRange: 1));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceWithPerSiteSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new double[][]
            {
                new[] { -0.005, 0.010 },
                new[] { -0.006, 0.012 },
                new[] { 0.007, 0.014 },
                new[] { 0.008, 0.016 }
            });
            sessionsBundle.ForceCurrentSequence(currentSequence: sequence, voltageLimit: 0.5, currentLevelRange: 0.1, voltageLimitRange: 1, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedCurrentLevelRange: 0.1, expectedVoltageLimit: 0.5, expectedVoltageLimitRange: 1, expectedSequenceLoopCount: 2));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceWithPerPinPerSiteSequence_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>() { [0] = new[] { 0.005, 0.010 }, [1] = new[] { 0.006, 0.012 }, [2] = new[] { 0.007, 0.014 }, [3] = new[] { 0.008, 0.016 } }
            });
            sessionsBundle.ForceCurrentSequence(currentSequence: sequence, voltageLimit: 0.5, currentLevelRange: 0.1, voltageLimitRange: 1, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedCurrentLevelRange: 0.1, expectedVoltageLimit: 0.5, expectedVoltageLimitRange: 1, expectedSequenceLoopCount: 2));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentSequence_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { -1.0, 1.0, 1.5, 1.7, 2.0 };
            sessionsBundle.ForceCurrentSequence(currentSequence: sequence, voltageLimit: 0.5, currentLevelRange: 2.5, voltageLimitRange: 1, sequenceLoopCount: 1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedVoltageLimit: 0.5, expectedSequenceLoopCount: 1));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentSequenceWithPerSiteSequence_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new double[][]
            {
                new[] { -1.1, 0.5, 1.1, 1.7, 2.3 },
                new[] { -1.5, -1, 0.5, 1, 1.5 }
            });
            sessionsBundle.ForceCurrentSequence(currentSequence: sequence, voltageLimit: 0.5, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedVoltageLimit: 0.5, expectedSequenceLoopCount: 2));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentSequenceWithPerPinPerSiteSequence_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { -0.6, 0.9 }, [1] = new[] { -0.2, 0.6 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { -0.6, 0.9 }, [1] = new[] { -0.2, 0.6 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { -0.6, 0.9 }, [1] = new[] { -0.2, 0.6 } }
            });
            sessionsBundle.ForceCurrentSequence(currentSequence: sequence, voltageLimit: 0.5, currentLevelRange: 2, voltageLimitRange: 1, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedVoltageLimit: 0.5, expectedSequenceLoopCount: 2));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentSequenceOnPinGroupWithPerPinPerSiteSequence_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var pins = _tsmContext.GetPinsInPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["ThreePinsGangedGroup"] = new Dictionary<int, double[]>() { [0] = new[] { -1.5, 0.9 }, [1] = new[] { -1.2, 0.6 } }
            });
            sessionsBundle.ForceCurrentSequence(currentSequence: sequence, voltageLimit: 0.5, currentLevelRange: 2, voltageLimitRange: 1, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedVoltageLimit: 0.5, expectedSequenceLoopCount: 2));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureSourceSettings_CorrectValuesAreSetWithSameValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 0.2,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Asymmetric,
                LimitHigh = 3,
                LimitLow = -1,
                SourceDelayInSeconds = 0.02
            };
            sessionsBundle.ConfigureSourceSettings(settings);

            Assert.Equal(settings.OutputFunction, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Source.Output.Function);
            Assert.Equal(settings.Level, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.Current.CurrentLevel);
            Assert.Equal(settings.LimitSymmetry, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.ComplianceLimitSymmetry);
            Assert.Equal(settings.LimitHigh, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.Current.VoltageLimitHigh);
            Assert.Equal(settings.LimitLow, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.Current.VoltageLimitLow);
            Assert.Equal(settings.SourceDelayInSeconds.Value, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Source.SourceDelay.TotalSeconds);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureSourceSettings_CorrectValuesAreSetWithPerPinValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });
            var siteCount = GetActiveSites(sessionsBundle);

            var settingsForVCC = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 0.1,
                Limit = 3
            };
            var settingsForVDD = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 3.6,
                Limit = 0.1,
                TransientResponse = DCPowerSourceTransientResponse.Fast
            };
            var settingsForVDET = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 5,
                Limit = 0.2,
                SourceDelayInSeconds = 0.02
            };

            var perPinSettings = new PinSiteData<DCPowerSourceSettings>(
                new string[]
                {
                    "VCC",
                    "VDD",
                    "VDET"
                },
                new SiteData<DCPowerSourceSettings>[]
                {
                    new SiteData<DCPowerSourceSettings>(siteCount, settingsForVCC),
                    new SiteData<DCPowerSourceSettings>(siteCount, settingsForVDD),
                    new SiteData<DCPowerSourceSettings>(siteCount, settingsForVDET),
                });

            sessionsBundle.ConfigureSourceSettings(perPinSettings);

            if (pinMapWithChannelGroup)
            {
                Assert.Equal(settingsForVCC.Level, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"].Source.Current.CurrentLevel);
                Assert.Equal(DCPowerSourceTransientResponse.Normal, sessionsBundle.InstrumentSessions.ElementAt(4).Session.Outputs["SMU_4154_C3_S04/0"].Source.TransientResponse);
                Assert.Equal(settingsForVDD.Limit, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.Voltage.CurrentLimit);
                Assert.Equal(settingsForVDD.TransientResponse, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.TransientResponse);
                Assert.Equal(0.017, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.SourceDelay.TotalSeconds, 3);
                Assert.Equal(settingsForVDET.SourceDelayInSeconds, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"].Source.SourceDelay.TotalSeconds);
            }
            else
            {
                Assert.Equal(settingsForVCC.Level, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Source.Current.CurrentLevel);
                Assert.Equal(DCPowerSourceTransientResponse.Normal, sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput.Source.TransientResponse);
                Assert.Equal(settingsForVDD.Limit, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.Voltage.CurrentLimit);
                Assert.Equal(settingsForVDD.TransientResponse, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.TransientResponse);
                Assert.Equal(0.017, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.SourceDelay.TotalSeconds, 3);
                Assert.Equal(settingsForVDET.SourceDelayInSeconds, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.SourceDelay.TotalSeconds);
            }
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSourceSettings_CorrectValuesAreSetAndCurrentLevelDivided()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 5,
                Limit = 7
            };
            sessionsBundle.ConfigureSourceSettings(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(channelOutput, 1, 7);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSourceSettingsWithSiteData_CorrectValuesAreSetAndCurrentLevelDivided()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var settingsForSite1 = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 0.5,
                Limit = 7
            };
            var settingsForSite2 = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 1,
                Limit = 5
            };
            var perSiteSettings = new SiteData<DCPowerSourceSettings>(
                new int[] { 0, 1 },
                new DCPowerSourceSettings[]
                {
                    settingsForSite1,
                    settingsForSite2
                });
            sessionsBundle.ConfigureSourceSettings(perSiteSettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(channelOutput, expectedCurrentLevel: sitePinInfo.SiteNumber == 0 ? 0.1 : 0.2, expectedVoltageLimit: sitePinInfo.SiteNumber == 0 ? 7 : 5);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSourceSettingsWithPinSiteDataOnPinGroupName_CorrectValuesAreSetAndCurrentLevelDivided()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(FourPinsGangedGroup);

            var perPinPerSiteSettings = new PinSiteData<DCPowerSourceSettings>(
                new string[]
                {
                    FourPinsGangedGroup,
                    "VCC5"
                },
                new int[] { 0, 1 },
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 1,
                    Limit = 6
                });
            sessionsBundle.ConfigureSourceSettings(perPinPerSiteSettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(sitePinInfo, channelOutput, 0.25, 1, 6);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSourceSettingsWithPinSiteData_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(FourPinsGangedGroup);

            var perPinPerSiteSettings = new PinSiteData<DCPowerSourceSettings>(
                new string[]
                {
                    "VCC1",
                    "VCC2",
                    "VCC3",
                    "VCC4",
                    "VCC5"
                },
                new int[] { 0, 1 },
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 0.75,
                    Limit = 6
                });
            sessionsBundle.ConfigureSourceSettings(perPinPerSiteSettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(sitePinInfo, channelOutput, 0.75, 0.75, 6);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureDifferentPerPinCurrentSourceSettingsForSameSite_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 1,
                Limit = 7
            };
            var settingsForVcc4 = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 1,
                Limit = 6
            };

            var perPinsettings = new PinSiteData<DCPowerSourceSettings>(new int[] { 0, 1 }, new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC1"] = settings,
                ["VCC2"] = settings,
                ["VCC3"] = settings,
                ["VCC4"] = settingsForVcc4,
                ["VCC5"] = settings
            });
            void ConfigureSourceSettings()
            {
                sessionsBundle.ConfigureSourceSettings(perPinsettings);
            }

            var exception = Assert.Throws<AggregateException>(ConfigureSourceSettings);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different values for Cascaded pins", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigurePerPinCurrentSourceSettings_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var settingsForGangedChannels = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 1,
                Limit = 7
            };
            var settingsForNonGangedChannels = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 2,
                Limit = 7
            };
            var perPinsettings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC1"] = settingsForGangedChannels,
                ["VCC2"] = settingsForGangedChannels,
                ["VCC3"] = settingsForGangedChannels,
                ["VCC4"] = settingsForNonGangedChannels,
                ["VCC5"] = settingsForNonGangedChannels
            };
            sessionsBundle.ConfigureSourceSettings(perPinsettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(sitePinInfo, channelOutput, 1, 2, 7);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigurePerPinCurrentSourceSettingsOnPinGroupName_CorrectValuesAreSetAndCurrentLevelDivided()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var settingsForGangedChannels = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 3,
                Limit = 7
            };
            var settingsForNonGangedChannels = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 2,
                Limit = 7
            };
            var perPinsettings = new Dictionary<string, DCPowerSourceSettings>()
            {
                [ThreePinsGangedGroup] = settingsForGangedChannels,
                ["VCC4"] = settingsForNonGangedChannels,
                ["VCC5"] = settingsForNonGangedChannels
            };
            sessionsBundle.ConfigureSourceSettings(perPinsettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(sitePinInfo, channelOutput, 1, 2, 7);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureDifferentPerPinCurrentSourceSettings_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var currentSettings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 4,
                Limit = 0.6,
            };
            var currentSettingsForVcc5 = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 0.5,
                Limit = 4,
            };
            var perPinsettings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC1"] = currentSettings,
                ["VCC2"] = currentSettings,
                ["VCC3"] = currentSettings,
                ["VCC4"] = currentSettings,
                ["VCC5"] = currentSettingsForVcc5
            };
            void ConfigureSourceSettings()
            {
                sessionsBundle.ConfigureSourceSettings(perPinsettings);
            }

            var exception = Assert.Throws<AggregateException>(ConfigureSourceSettings);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different values for Cascaded pins", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSourceSettings_CorrectValuesAreSetAndCurrentLimitDivided()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                Level = 3,
                Limit = 1.5
            };
            sessionsBundle.ConfigureSourceSettings(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                AssertVoltageSettings(sitePinInfo, channelOutput, 3, 0.5, 1.5);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSourceSettingsWithSiteData_CorrectValuesAreSetAndCurrentLimitDivided()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(FourPinsGangedGroup);

            var perSiteSettings = new SiteData<DCPowerSourceSettings>(
                new int[] { 0, 1 },
                new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    Level = 4,
                    Limit = 1,
                });
            sessionsBundle.ConfigureSourceSettings(perSiteSettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                AssertVoltageSettings(sitePinInfo, channelOutput, 4, 0.25, 1);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSourceSettingsWithPinSiteData_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var perPinPerSiteSettings = new PinSiteData<DCPowerSourceSettings>(
                new string[]
                {
                    "VCC1",
                    "VCC2",
                    "VCC3",
                    "VCC4",
                    "VCC5"
                },
                new int[] { 0, 1 },
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    Level = 3,
                    Limit = 1
                });
            sessionsBundle.ConfigureSourceSettings(perPinPerSiteSettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                AssertVoltageSettings(sitePinInfo, channelOutput, 3, 1, 1);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSourceSettingsWithPinSiteDataOnPinGroupName_CorrectValuesAreSetAndCurrentLimitDivided()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            var perPinPerSiteSettings = new PinSiteData<DCPowerSourceSettings>(
                new string[]
                {
                    ThreePinsGangedGroup,
                    "VCC4",
                    "VCC5"
                },
                new int[] { 0, 1 },
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    Level = 3,
                    Limit = 3
                });
            sessionsBundle.ConfigureSourceSettings(perPinPerSiteSettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                AssertVoltageSettings(sitePinInfo, channelOutput, 3, 1, 3);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigurePerPinVoltageSourceSettings_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                Level = 4,
                Limit = 0.6,
            };
            var perPinsettings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC1"] = settings,
                ["VCC2"] = settings,
                ["VCC3"] = settings,
                ["VCC4"] = settings,
                ["VCC5"] = settings
            };
            sessionsBundle.ConfigureSourceSettings(perPinsettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                AssertVoltageSettings(channelOutput, 4, 0.6);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigurePerPinVoltageSourceSettingsOnPinGroupName_CorrectValuesAreSetAndCurrentLimitDivided()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                Level = 4,
                Limit = 3,
            };
            var perPinsettings = new Dictionary<string, DCPowerSourceSettings>()
            {
                [AllPinsGangedGroup] = settings
            };
            sessionsBundle.ConfigureSourceSettings(perPinsettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                AssertVoltageSettings(channelOutput, 4, 0.6);
                AssertSourceTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureDifferentOutputFunctionForPins_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var voltageSettings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                Level = 4,
                Limit = 0.6,
            };
            var currentSettings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 0.5,
                Limit = 4,
            };
            var perPinsettings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC1"] = voltageSettings,
                ["VCC2"] = voltageSettings,
                ["VCC3"] = voltageSettings,
                ["VCC4"] = currentSettings,
                ["VCC5"] = currentSettings
            };
            void ConfigureSourceSettings()
            {
                sessionsBundle.ConfigureSourceSettings(perPinsettings);
            }

            var exception = Assert.Throws<AggregateException>(ConfigureSourceSettings);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different output functions for Cascaded pins", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureSourceSettingsOnFilteredBundleWithFewPins_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var filteredBundle = sessionsBundle.FilterByPin(new string[] { "VCC1", "VCC2" });
            void ConfigureSourceSettings()
            {
                filteredBundle.ConfigureSourceSettings(new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    Level = 1,
                    Limit = 1,
                });
            }

            var exception = Assert.Throws<AggregateException>(ConfigureSourceSettings);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("not present in the DCPowerSessionsBundle", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureSourceSettingsOnSubsetBundleWithTwoPins_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var subsetBundle = sessionManager.DCPower(TwoPinsGangedGroup);
            void ConfigureSourceSettings()
            {
                subsetBundle.ConfigureSourceSettings(new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    Level = 1,
                    Limit = 1,
                });
            }

            var exception = Assert.Throws<AggregateException>(ConfigureSourceSettings);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("not present in the DCPowerSessionsBundle", exception.InnerException.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_FilterBundleWithFewPinsAndUngangThenConfigure_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var filteredBundle = sessionsBundle.FilterByPin(new string[] { "VCC1", "VCC2" });
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
            filteredBundle.ConfigureSourceSettings(new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 1,
                Limit = 1,
            });

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (sitePinInfo.PinName == "VCC1" || sitePinInfo.PinName == "VCC2")
                {
                    var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                    AssertCurrentSettings(channelOutput, 1, 1);
                }
            });
        }

        [Fact]
        public void DifferentSMU_ConfigureSourceSettingsWithMultipleChannelOutputAndValidSitePinInfo_ThrowsException()
        {
            var sessionManager = Initialize("DifferentSMUDevicesForEachSiteSeperateChannelGroupPerInstr.pinmap");
            var sessionsBundle = sessionManager.DCPower("VCC");
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 1,
                Limit = 1,
            };

            void ConfigureSourceSettingsWithMultipleChannelOutput()
            {
                var sessionInfo = sessionsBundle.InstrumentSessions.First();
                sessionInfo.ConfigureSourceSettings(settings, sessionInfo.AllChannelsOutput, sessionsBundle.AggregateSitePinList.First());
            }

            var exception = Assert.Throws<NISemiconductorTestException>(ConfigureSourceSettingsWithMultipleChannelOutput);
            Assert.Contains("only supports single channel operation", exception.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureOutputConnected_OutputConnectedThrowsExceptionWhenUnsupported(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });
            var expectedPhrases = new string[] { "An exception occurred while processing pins/sites:", "Invalid value for parameter or property." };

            var values = new SiteData<bool>(GetActiveSites(sessionsBundle), false);

            void ConfigureOutputConnected()
            {
                sessionsBundle.ConfigureOutputConnected(values);
            }

            var exception = Assert.Throws<NISemiconductorTestException>(ConfigureOutputConnected);
            foreach (var expectedPhrase in expectedPhrases)
            {
                Assert.Contains(expectedPhrase, exception.Message);
            }
        }

#pragma warning disable xUnit1013 // Public method should be marked as test
        public void DifferentSMUDevices_ConfigureOutputConnected_OutputConnectedWithPerPinValues()
#pragma warning restore xUnit1013 // Public method should be marked as test
        {
            // Cannot implement this test until we test against other pin map support or pin/site filtering.
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureOutputEnabled_OutputEnabled(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            sessionsBundle.ConfigureOutputEnabled(true);

            // Confirm which instruments do an don't have this.
            if (pinMapWithChannelGroup)
            {
                // 4110
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"].Source.Output.Enabled);
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"].Source.Output.Enabled);
                // 4130
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/0"].Source.Output.Enabled);
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/1"].Source.Output.Enabled);
                // 4154
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/0"].Source.Output.Enabled);
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/1"].Source.Output.Enabled);
                // 4112
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/0"].Source.Output.Enabled);
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/1"].Source.Output.Enabled);
            }
            else
            {
                // 4110
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Source.Output.Enabled);
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.Output.Enabled);
                // 4130
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.Output.Enabled);
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Source.Output.Enabled);
                // 4154
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(4).AllChannelsOutput.Source.Output.Enabled);
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsOutput.Source.Output.Enabled);
                // 4112
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput.Source.Output.Enabled);
                Assert.True(sessionsBundle.InstrumentSessions.ElementAt(7).AllChannelsOutput.Source.Output.Enabled);
            }

            sessionsBundle.ConfigureOutputEnabled(false);

            if (pinMapWithChannelGroup)
            {
                // 4110
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"].Source.Output.Enabled);
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"].Source.Output.Enabled);
                // 4130
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/0"].Source.Output.Enabled);
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4130_C2_S04/1"].Source.Output.Enabled);
                // 4154
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/0"].Source.Output.Enabled);
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(2).Session.Outputs["SMU_4154_C3_S04/1"].Source.Output.Enabled);
                // 4112
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/0"].Source.Output.Enabled);
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(3).Session.Outputs["SMU_4112_C4_S04/1"].Source.Output.Enabled);
            }
            else
            {
                // 4110
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Source.Output.Enabled);
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.Output.Enabled);
                // 4130
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.Output.Enabled);
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Source.Output.Enabled);
                // 4154
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(4).AllChannelsOutput.Source.Output.Enabled);
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsOutput.Source.Output.Enabled);
                // 4112
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput.Source.Output.Enabled);
                Assert.False(sessionsBundle.InstrumentSessions.ElementAt(7).AllChannelsOutput.Source.Output.Enabled);
            }
        }

        [Fact]
        public void SMUDevicesMerged_GetSourceDelayInSeconds_ValuesAreReturnedInPrimaryPinName()
        {
            var sessionManager = Initialize("MergedPinGroupTest_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsMergedGroupWithVCCPrimaryAsPrimaryPin");
            sessionsBundle.MergePinGroup("AllPinsMergedGroupWithVCCPrimaryAsPrimaryPin");

            var sourceDelays = sessionsBundle.GetSourceDelayInSeconds();

            Assert.Single(sourceDelays.PinNames);
            Assert.Equal("VCCPrimary", sourceDelays.PinNames[0]);
            Assert.DoesNotContain("AllPinsMergedGroupWithVCCPrimaryAsPrimaryPin", sourceDelays.PinNames);
        }

        [Fact]
        public void SMUDevicesGanged_GetSourceDelayInSeconds_ValuesDontHavePinGroupName()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup("AllPinsGangedGroup");

            var sourceDelays = sessionsBundle.GetSourceDelayInSeconds();

            Assert.Equal(5, sourceDelays.PinNames.Length);
            Assert.DoesNotContain("AllPinsGangedGroup", sourceDelays.PinNames);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_GetSourceDelayInSeconds_GetPerSiteValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");
            var testValue = 0.001;
            sessionsBundle.ConfigureSourceSettings(
                new DCPowerSourceSettings()
                {
                    SourceDelayInSeconds = testValue,
                });

            var values = sessionsBundle.GetSourceDelayInSeconds();

            Assert.Equal(testValue, values.ExtractSite(0)["VCC"]);
            Assert.Equal(testValue, values.ExtractSite(1)["VCC"]);
            Assert.Equal(testValue, values.ExtractSite(2)["VCC"]);
            Assert.Equal(testValue, values.ExtractSite(3)["VCC"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_GetSourceDelayInSeconds_GetPerDeviceValues(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });
            var activeSites = GetActiveSites(sessionsBundle);

            var settingsForVCC = new DCPowerSourceSettings()
            {
                SourceDelayInSeconds = 1,
            };
            var settingsForVDD = new DCPowerSourceSettings()
            {
                SourceDelayInSeconds = 0.2,
            };
            var settingsForVDET = new DCPowerSourceSettings()
            {
                SourceDelayInSeconds = 0.03,
            };

            var settings = new PinSiteData<DCPowerSourceSettings>(
                new string[]
                {
                    "VCC",
                    "VDD",
                    "VDET"
                },
                new SiteData<DCPowerSourceSettings>[]
                {
                    new SiteData<DCPowerSourceSettings>(activeSites, settingsForVCC),
                    new SiteData<DCPowerSourceSettings>(activeSites, settingsForVDD),
                    new SiteData<DCPowerSourceSettings>(activeSites, settingsForVDET)
                });

            sessionsBundle.ConfigureSourceSettings(settings);

            var values = sessionsBundle.GetSourceDelayInSeconds();

            Assert.Equal(1, values.ExtractSite(0)["VCC"]);
            Assert.Equal(0.2, values.ExtractSite(1)["VDD"]);
            Assert.Equal(0.03, values.ExtractSite(2)["VDET"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetSameCurrentLimitWithoutCurrentLimitRange_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            sessionsBundle.ConfigureCurrentLimit(0.1);

            Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Source.Voltage.CurrentLimit);
            Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.Voltage.CurrentLimit);
            Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.Voltage.CurrentLimit);
            Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Source.Voltage.CurrentLimit);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetPerPinCurrentLimitWithoutCurrentLimitRange_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });

            sessionsBundle.ConfigureCurrentLimits(new Dictionary<string, double>()
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
                Assert.Equal(1, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Source.Voltage.CurrentLimit);
                Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.Voltage.CurrentLimit);
                Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.Voltage.CurrentLimit);
            }
        }

        [Fact]
        public void SMUDevicesMerged_GetCurrentLimits_ValuesAreReturnedInPrimaryPinName()
        {
            var sessionManager = Initialize("MergedPinGroupTest_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsMergedGroupWithVCCPrimaryAsPrimaryPin");
            sessionsBundle.MergePinGroup("AllPinsMergedGroupWithVCCPrimaryAsPrimaryPin");

            var currentLimits = sessionsBundle.GetCurrentLimits();

            Assert.Single(currentLimits.PinNames);
            Assert.Equal("VCCPrimary", currentLimits.PinNames[0]);
            Assert.DoesNotContain("AllPinsMergedGroupWithVCCPrimaryAsPrimaryPin", currentLimits.PinNames);
        }

        [Fact]
        public void SMUDevicesGanged_GetCurrentLimits_ValuesDontHavePinGroupName()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup("AllPinsGangedGroup");

            var currentLimits = sessionsBundle.GetCurrentLimits();

            Assert.Equal(5, currentLimits.PinNames.Length);
            Assert.DoesNotContain("AllPinsGangedGroup", currentLimits.PinNames);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesSetSameCurrentLimit_GetCurrentLimits_GetTheSameValue(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");
            sessionsBundle.ConfigureCurrentLimit(0.1);

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
            sessionsBundle.ConfigureCurrentLimits(new Dictionary<string, double>()
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
        public void DifferentSMUDevices_SetVoltageSequence_CorrectVoltageMeasurementsFetchedClearAndDeleteAdvancedSequence(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequence = new double[] { 1, 2, 3, 4, 5 };
            // Cannot test sequence loop count because it's not supported in offline mode.
            sessionsBundle.ConfigureVoltageSequence(sequenceName, expectedSequence, sequenceLoopCount: 1, setAsActiveSequence: true);
            var results = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                output.Control.Initiate();
                // Cannot test fetch backlog because it's always 1 in offline mode.
                return sessionInfo.Session.Measurement.Fetch(sitePinInfo.IndividualChannelString, PrecisionTimeSpan.FromSeconds(1), 5);
            });

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                Assert.Equal(expectedSequence, results[0][0].VoltageMeasurements);
                Assert.Equal(expectedSequence, results[0][1].VoltageMeasurements);
                Assert.Equal(expectedSequence, results[0][2].VoltageMeasurements);
                Assert.Equal(expectedSequence, results[0][3].VoltageMeasurements);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetCurrentSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "CurrentSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequence = new double[] { 0.5, 1, 1.5, 2, 2.5 };
            // Cannot test sequence loop count because it's not supported in offline mode.
            sessionsBundle.ConfigureCurrentSequence(sequenceName, expectedSequence, sequenceLoopCount: 1, setAsActiveSequence: true);
            var results = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                output.Control.Initiate();
                // Cannot test fetch backlog because it's always 1 in offline mode.
                return sessionInfo.Session.Measurement.Fetch(sitePinInfo.IndividualChannelString, PrecisionTimeSpan.FromSeconds(1), 5);
            });

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                Assert.Equal(expectedSequence, results[0][0].CurrentMeasurements);
                Assert.Equal(expectedSequence, results[0][1].CurrentMeasurements);
                Assert.Equal(expectedSequence, results[0][2].CurrentMeasurements);
                Assert.Equal(expectedSequence, results[0][3].CurrentMeasurements);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetVoltageSequenceWithStepDeltaTime_SequenceStepDeltaTimeEnabled(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequenceWithDeltaTime";

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.False(sessionInfo.AllChannelsOutput.Source.SequenceStepDeltaTimeEnabled);
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence: new double[] { 1, 2, 3 }, sequenceLoopCount: 1, sequenceStepDeltaTimeInSeconds: 0.05, setAsActiveSequence: true);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.True(sessionInfo.AllChannelsOutput.Source.SequenceStepDeltaTimeEnabled);
                if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
                {
                    Assert.Equal(0.05, sessionInfo.AllChannelsOutput.Source.SequenceStepDeltaTime.TotalSeconds);
                }
            });
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureAdvanceSequence_ClearAdvancedSequences_ActiveSequencesCleared(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName = "TestSequence";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true);

            sessionsBundle.ClearActiveAdvancedSequence();

            var ex = Assert.Throws<NISemiconductorTestException>(() =>
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    var activeSequenceName = sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence;
                });
            });
            Assert.Contains("A sequence must have at least one step.", ex.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureAdvanceSequence_DeleteAdvancedSequences_SequenceDeletedSuccessfully(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName = "TestSequence";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true);

            sessionsBundle.DeleteAdvancedSequence(sequenceName);

            var ex = Assert.Throws<NISemiconductorTestException>(() =>
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = sequenceName;
                });
            });
            Assert.Contains("The active advanced sequence does not exist.", ex.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureAdvanceSequence_ClearThenDeleteAdvancedSequence_SequenceDeletedSuccessfully(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName = "TestSequence";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true);

            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);

            var ex = Assert.Throws<NISemiconductorTestException>(() =>
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = sequenceName;
                });
            });
            Assert.Contains("The active advanced sequence does not exist.", ex.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureMultipleAdvanceSequenceAsInactive_DeleteMultipleAdvancedSequences_AllSequencesDeleted(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName1 = "Sequence1";
            string sequenceName2 = "Sequence2";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            sessionsBundle.ConfigureAdvancedSequence(sequenceName1, stepProperties, setAsActiveSequence: false);
            sessionsBundle.ConfigureAdvancedSequence(sequenceName2, stepProperties, setAsActiveSequence: false);

            sessionsBundle.DeleteAdvancedSequence(sequenceName1, sequenceName2);

            var ex1 = Assert.Throws<NISemiconductorTestException>(() =>
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = sequenceName1;
                });
            });
            var ex2 = Assert.Throws<NISemiconductorTestException>(() =>
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = sequenceName1;
                });
            });
            Assert.Contains("The active advanced sequence does not exist.", ex1.Message);
            Assert.Contains("The active advanced sequence does not exist.", ex2.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequence_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "VoltageSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 1, setAsActiveSequence: true);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSiteData_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequenceWithSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                new[] { 2.0, 3.0, 4.0, 5.0, 6.0 },
                new[] { 2.5, 3.5, 4.5, 5.5, 6.5 }
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber), precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithPinSiteData_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "VoltageSequenceWithPinSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                    [1] = new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                    [2] = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 },
                    [3] = new[] { 2.5, 3.5, 4.5, 5.5, 6.5 }
                }
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => expectedSequences.GetValue(siteNumber, pinName), precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequence_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceTest";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 2, itemsToFetch: 5, checkForCurrentMeasurement: false);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithPerSiteSequence_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceWithSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => sequence.GetValue(siteNumber), precision: 2, itemsToFetch: 5, checkForCurrentMeasurement: false);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithPerPinPerSiteSequenceOnPinGroupName_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceWithPinSiteDataOnPinGroupName";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                [ThreePinsGangedGroup] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName), precision: 2, itemsToFetch: 2, checkForCurrentMeasurement: false, groupName: ThreePinsGangedGroup);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithPerPinPerSiteSequence_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceWithPinSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();

            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName), precision: 2, itemsToFetch: 2, checkForCurrentMeasurement: false), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithDifferentPerPinPerSiteSequence_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceWithDifferentPerPinPerSiteSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 1.0, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            void ConfigureVoltageSequenceTest()
            {
                sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 2);
            }

            sessionsBundle.Abort();
            var exception = Assert.Throws<AggregateException>(ConfigureVoltageSequenceTest);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different sequences for Cascaded pins", exception.InnerException.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "CurrentSequenceTest";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 0.2, 0.4, 0.6, 0.8 };
            sessionsBundle.ConfigureCurrentSequence(sequenceName, sequence, sequenceLoopCount: 1, setAsActiveSequence: true);

            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 1, itemsToFetch: 4), sessionsBundle, sequenceName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSiteData_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "CurrentSequenceWithSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 0.2, 0.3, 0.4, 0.5 },
                new[] { 0.3, 0.4, 0.5, 0.6 },
                new[] { 0.4, 0.5, 0.6, 0.7 },
                new[] { 0.5, 0.6, 0.7, 0.8 }
            });
            sessionsBundle.ConfigureCurrentSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true);

            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber), precision: 1, itemsToFetch: 4), sessionsBundle, sequenceName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithPinSiteData_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "CurrentSequenceWithPinSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.2, 0.3, 0.4, 0.5 },
                    [1] = new[] { 0.3, 0.4, 0.5, 0.6 },
                    [2] = new[] { 0.4, 0.5, 0.6, 0.7 },
                    [3] = new[] { 0.5, 0.6, 0.7, 0.8 }
                }
            });
            sessionsBundle.ConfigureCurrentSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true);

            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => expectedSequences.GetValue(siteNumber, pinName), precision: 1, itemsToFetch: 4), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequence_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceTest";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 0.2, 0.4, 0.6, 0.8, 1.0 };
            sessionsBundle.ConfigureCurrentSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence.Select(value => value / 3).ToArray(), precision: 2, itemsToFetch: 5), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithPerSiteSequence_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceWithSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 0.2, 0.3, 0.4, 0.5 },
                new[] { 0.3, 0.4, 0.5, 0.6 },
                new[] { 0.4, 0.5, 0.6, 0.7 },
                new[] { 0.5, 0.6, 0.7, 0.8 }
            });
            sessionsBundle.ConfigureCurrentSequence(sequenceName, expectedSequences, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber).Select(value => value / 3).ToArray(), precision: 2, itemsToFetch: 4), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithPerPinPerSiteSequenceOnPinGroupName_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceWithPinSiteDataOnPinGroupName";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                [ThreePinsGangedGroup] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            sessionsBundle.ConfigureCurrentSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName).Select(value => value / 3).ToArray(), precision: 2, itemsToFetch: 2, groupName: ThreePinsGangedGroup), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithPerPinPerSiteSequence_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceWithPinSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 0.5, 0.7 }, [1] = new[] { 0.8, 0.9 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 0.4, 0.7 }, [1] = new[] { 0.8, 0.9 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 0.5, 0.7 }, [1] = new[] { 0.8, 0.9 } }
            });
            sessionsBundle.ConfigureCurrentSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName), precision: 2, itemsToFetch: 2), sessionsBundle, sequenceName);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSourceDelays_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "VoltageSequenceWithSourceDelays";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var sourceDelayInSeconds = new double[] { 0.05, 0.05, 0.05, 0.05, 0.05 };
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays(sequenceName, sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 2, itemsToFetch: 5, checkForCurrentMeasurement: false), sessionsBundle, sequenceName);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSourceDelaysAndPerSiteSequence_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "VoltageSequenceWithSourceDelaysAndSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                new[] { 2.0, 2.5, 3.0, 3.5, 4.0 },
                new[] { 2.5, 3.0, 3.5, 4.0, 4.5 }
            });
            var expectedSourceDelayInSeconds = new SiteData<double[]>(new double[][]
            {
                new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                new[] { 0.05, 0.05, 0.05, 0.05, 0.05 }
            });
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays(sequenceName, expectedSequences, expectedSourceDelayInSeconds, sequenceLoopCount: 1, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber), precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false), sessionsBundle, sequenceName);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSourceDelaysAndPerPinPerSiteSequence_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "VoltageSequenceWithSourceDelaysAndPinSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                    [1] = new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                    [2] = new[] { 2.0, 2.5, 3.0, 3.5, 4.0 },
                    [3] = new[] { 2.5, 3.0, 3.5, 4.0, 4.5 }
                }
            });
            var expectedSourceDelayInSeconds = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                    [1] = new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                    [2] = new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                    [3] = new[] { 0.05, 0.05, 0.05, 0.05, 0.05 }
                }
            });
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays(sequenceName, expectedSequences, expectedSourceDelayInSeconds, sequenceLoopCount: 1, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => expectedSequences.GetValue(siteNumber, pinName), precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithSourceDelays_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceWithSourceDelays";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var sourceDelayInSeconds = new double[] { 3.0, 3.0, 3.0, 3.0, 3.0 };
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays(sequenceName, sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 2, itemsToFetch: 5, checkForCurrentMeasurement: false), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithSourceDelaysAndPerSiteSequence_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                new[] { 0.5, 1.5, 2.5, 3.5, 4.5 },
            });
            var sourceDelayInSeconds = new SiteData<double[]>(new double[][]
            {
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 }
            });
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays("VoltageSequenceWithSourceDelays", sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => sequence.GetValue(siteNumber), precision: 2, itemsToFetch: 5, checkForCurrentMeasurement: false), sessionsBundle, "VoltageSequenceWithSourceDelays");
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithSourceDelaysAndPerPinPerSiteSequence_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            var sourceDelayInSeconds = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } }
            });
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays("VoltageSequenceWithSourceDelaysAndPerSiteSequence", sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName), precision: 2, itemsToFetch: 2, checkForCurrentMeasurement: false), sessionsBundle, "VoltageSequenceWithSourceDelaysAndPerSiteSequence");
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureSequence_CorrectTriggersSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var sequence = new double[] { 0.2, 0.4, 0.6, 0.8, 1.0 };
            sessionsBundle.ConfigureVoltageSequence("VoltageSequence", sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                AssertTriggerSettings(sitePinInfo, output, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0", checkSequenceRelatedTriggers: true);
            });
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSourceDelays_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 0.2, 0.4, 0.6, 0.8, 1.0 };
            var sourceDelayInSeconds = new double[] { 2.0, 2.0, 2.0, 2.0, 2.0 };
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays("CurrentSequenceWithSourceDelays", sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 2, itemsToFetch: 5), sessionsBundle, "CurrentSequenceWithSourceDelays");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSourceDelaysAndPerSiteSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 0.2, 0.3, 0.4, 0.5 },
                new[] { 0.3, 0.4, 0.5, 0.6 },
                new[] { 0.4, 0.5, 0.6, 0.7 },
                new[] { 0.5, 0.6, 0.7, 0.8 }
            });
            var expectedSourceDelayInSeconds = new SiteData<double[]>(new double[][]
            {
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 }
            });
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays("CurrentSequenceWithSourceDelaysAndPerSiteSequence", expectedSequences, expectedSourceDelayInSeconds, sequenceLoopCount: 1, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber), precision: 1, itemsToFetch: 4), sessionsBundle, "CurrentSequenceWithSourceDelaysAndPerSiteSequence");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.2, 0.3, 0.4, 0.5 },
                    [1] = new[] { 0.3, 0.4, 0.5, 0.6 },
                    [2] = new[] { 0.4, 0.5, 0.6, 0.7 },
                    [3] = new[] { 0.5, 0.6, 0.7, 0.8 }
                }
            });
            var expectedSourceDelayInSeconds = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 1.0, 1.0, 1.0, 1.0 },
                    [1] = new[] { 1.0, 1.0, 1.0, 1.0 },
                    [2] = new[] { 1.0, 1.0, 1.0, 1.0 },
                    [3] = new[] { 1.0, 1.0, 1.0, 1.0 }
                }
            });
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays("CurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence", expectedSequences, expectedSourceDelayInSeconds, sequenceLoopCount: 1, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => expectedSequences.GetValue(siteNumber, pinName), precision: 1, itemsToFetch: 4), sessionsBundle, "CurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence");
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithSourceDelays_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 0.2, 0.4, 0.6, 0.8, 1.0 };
            var sourceDelayInSeconds = new double[] { 4.0, 4.0, 4.0, 4.0, 4.0 };
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays("CurrentSequenceWithSourceDelaysGanged", sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence.Select(value => value / 3).ToArray(), precision: 2, itemsToFetch: 5), sessionsBundle, "CurrentSequenceWithSourceDelaysGanged");
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithSourceDelaysAndPerSiteSequence_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceWithSourceDelaysAndPerSiteSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 0.2, 0.3, 0.4, 0.5 },
                new[] { 0.3, 0.4, 0.5, 0.6 },
                new[] { 0.4, 0.5, 0.6, 0.7 },
                new[] { 0.5, 0.6, 0.7, 0.8 }
            });
            var expectedSourceDelayInSeconds = new SiteData<double[]>(new double[][]
            {
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 }
            });
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays(sequenceName, expectedSequences, expectedSourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber).Select(value => value / 3).ToArray(), precision: 2, itemsToFetch: 4), sessionsBundle, "CurrentSequenceWithSourceDelaysAndPerSiteSequence");
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 0.3, 0.5 }, [1] = new[] { 0.4, 0.6 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 0.3, 0.5 }, [1] = new[] { 0.4, 0.6 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 0.3, 0.5 }, [1] = new[] { 0.4, 0.6 } }
            });
            var sourceDelayInSeconds = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } }
            });
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays(sequenceName, sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName), precision: 2, itemsToFetch: 2), sessionsBundle, "CurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence");
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndAllInVoltageMode_CheckVoltageModeAndLevels_ReturnsTrue(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            var result = sessionsBundle.CheckDCVoltageModeAndLevels(out _);

            Assert.True(result);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndTwoInCurrentMode_CheckVoltageModeAndLevels_ReturnsFalse(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            sessionsBundle.Do((sessionInfo, sessionIndex) =>
            {
                if (sessionIndex < 2)
                {
                    sessionInfo.AllChannelsOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                }
            });

            var result = sessionsBundle.CheckDCVoltageModeAndLevels(out var failedChannels);

            Assert.False(result);
            Assert.Equal(2, failedChannels.Count());
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsString, failedChannels);
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsString, failedChannels);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesForcedDifferentVoltageLevels_CheckVoltageModeAndLevelsWithMatchingValues_ReturnsTrue(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD" });
            var voltageLevels = new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 2 };
            sessionsBundle.ForceVoltage(voltageLevels, currentLimit: 0.1);

            var result = sessionsBundle.CheckDCVoltageModeAndLevels(out _, expectedVoltages: voltageLevels);

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
            sessionsBundle.ForceVoltage(voltageLevels, currentLimit: 0.1);

            var result = sessionsBundle.CheckDCVoltageModeAndLevels(out var failedChannels, expectedVoltages: new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 3 });

            Assert.False(result);
            Assert.Equal(4, failedChannels.Count());
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsString, failedChannels);
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsString, failedChannels);
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(5).AllChannelsString, failedChannels);
            Assert.Contains(sessionsBundle.InstrumentSessions.ElementAt(7).AllChannelsString, failedChannels);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureSameSourceSettings_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 0.2,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Asymmetric,
                LimitHigh = 3,
                LimitLow = -1,
                SourceDelayInSeconds = 0.02
            };
            sessionsBundle.ConfigureSourceSettings(settings);

            Assert.Equal(settings.OutputFunction, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Source.Output.Function);
            Assert.Equal(settings.Level, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.Current.CurrentLevel);
            Assert.Equal(settings.LimitSymmetry, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.ComplianceLimitSymmetry);
            Assert.Equal(settings.LimitHigh, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.Current.VoltageLimitHigh);
            Assert.Equal(settings.LimitLow, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.Current.VoltageLimitLow);
            Assert.Equal(settings.SourceDelayInSeconds.Value, sessionsBundle.InstrumentSessions.ElementAt(3).AllChannelsOutput.Source.SourceDelay.TotalSeconds);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureDifferentSourceSettings_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });

            var settingsForVCC = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 0.1,
                Limit = 3
            };
            var settingsForVDD = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 3.6,
                Limit = 0.1,
                TransientResponse = DCPowerSourceTransientResponse.Fast
            };
            var settingsForVDET = new DCPowerSourceSettings()
            {
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = 5,
                Limit = 0.2,
                SourceDelayInSeconds = 0.02
            };
            var settings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["VCC"] = settingsForVCC,
                ["VDD"] = settingsForVDD,
                ["VDET"] = settingsForVDET
            };
            sessionsBundle.ConfigureSourceSettings(settings);

            if (pinMapWithChannelGroup)
            {
                Assert.Equal(settingsForVCC.Level, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"].Source.Current.CurrentLevel);
                Assert.Equal(DCPowerSourceTransientResponse.Normal, sessionsBundle.InstrumentSessions.ElementAt(4).Session.Outputs["SMU_4154_C3_S04/0"].Source.TransientResponse);
                Assert.Equal(settingsForVDD.Limit, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.Voltage.CurrentLimit);
                Assert.Equal(settingsForVDD.TransientResponse, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.TransientResponse);
                Assert.Equal(0.017, sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"].Source.SourceDelay.TotalSeconds, 3);
                Assert.Equal(settingsForVDET.SourceDelayInSeconds, sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"].Source.SourceDelay.TotalSeconds);
            }
            else
            {
                Assert.Equal(settingsForVCC.Level, sessionsBundle.InstrumentSessions.ElementAt(0).AllChannelsOutput.Source.Current.CurrentLevel);
                Assert.Equal(DCPowerSourceTransientResponse.Normal, sessionsBundle.InstrumentSessions.ElementAt(6).AllChannelsOutput.Source.TransientResponse);
                Assert.Equal(settingsForVDD.Limit, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.Voltage.CurrentLimit);
                Assert.Equal(settingsForVDD.TransientResponse, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.TransientResponse);
                Assert.Equal(0.017, sessionsBundle.InstrumentSessions.ElementAt(1).AllChannelsOutput.Source.SourceDelay.TotalSeconds, 3);
                Assert.Equal(settingsForVDET.SourceDelayInSeconds, sessionsBundle.InstrumentSessions.ElementAt(2).AllChannelsOutput.Source.SourceDelay.TotalSeconds);
            }
        }

        private void AssertVoltageSettings(DCPowerOutput channelOutput, double expectedVoltageLevel, double expectedCurrentLimit, int precision = 6)
        {
            Assert.Equal(expectedVoltageLevel, channelOutput.Source.Voltage.VoltageLevel, precision);
            Assert.Equal(expectedCurrentLimit, channelOutput.Source.Voltage.CurrentLimit, precision);
        }

        private void AssertVoltageSettings(DCPowerOutput channelOutput, double? expectedVoltageLevel = null, double? expectedCurrentLimitHigh = null, double? expectedCurrentLimitLow = null, double? expectedCurrentLimit = null, double? expectedCurrentLimitRange = null)
        {
            if (expectedVoltageLevel.HasValue)
            {
                Assert.Equal(expectedVoltageLevel.Value, channelOutput.Source.Voltage.VoltageLevel);
            }
            if (expectedCurrentLimitHigh.HasValue)
            {
                Assert.Equal(expectedCurrentLimitHigh.Value, channelOutput.Source.Voltage.CurrentLimitHigh);
            }
            if (expectedCurrentLimitLow.HasValue)
            {
                Assert.Equal(expectedCurrentLimitLow.Value, channelOutput.Source.Voltage.CurrentLimitLow);
            }
            if (expectedCurrentLimit.HasValue)
            {
                Assert.Equal(expectedCurrentLimit.Value, channelOutput.Source.Voltage.CurrentLimit);
            }
            if (expectedCurrentLimitRange.HasValue)
            {
                Assert.Equal(expectedCurrentLimitRange.Value, channelOutput.Source.Voltage.CurrentLimitRange);
            }
        }

        private void AssertCurrentSettings(DCPowerOutput channelOutput, double expectedCurrentLevel, double expectedVoltageLimit)
        {
            Assert.Equal(expectedCurrentLevel, channelOutput.Source.Current.CurrentLevel);
            Assert.Equal(expectedVoltageLimit, channelOutput.Source.Current.VoltageLimit);
        }

        private void AssertCurrentSettings(DCPowerOutput channelOutput, double expectedVoltageLimit, double expectedSequenceLoopCount, double? expectedCurrentLevelRange = null, double? expectedVoltageLimitRange = null)
        {
            if (expectedCurrentLevelRange.HasValue)
            {
                Assert.Equal(expectedCurrentLevelRange, channelOutput.Source.Current.CurrentLevelRange);
            }
            if (expectedVoltageLimitRange.HasValue)
            {
                Assert.Equal(expectedVoltageLimitRange, channelOutput.Source.Current.VoltageLimitRange);
            }
            Assert.Equal(expectedVoltageLimit, channelOutput.Source.Current.VoltageLimit);
            Assert.Equal(expectedSequenceLoopCount, channelOutput.Source.SequenceLoopCount);
        }

        private void AssertCurrentSettings(DCPowerOutput channelOutput, double expectedCurrentLevel, double expectedVoltageLimitHigh, double expectedVoltageLimitLow)
        {
            Assert.Equal(expectedCurrentLevel, channelOutput.Source.Current.CurrentLevel);
            Assert.Equal(expectedVoltageLimitHigh, channelOutput.Source.Current.VoltageLimitHigh);
            Assert.Equal(expectedVoltageLimitLow, channelOutput.Source.Current.VoltageLimitLow);
        }

        private void AssertCurrentSettings(SitePinInfo sitePinInfo, DCPowerOutput channelOutput, double gangedChannelLevel, double normalChannelLevel, double voltageLimit)
        {
            var currentLevel = sitePinInfo.CascadingInfo is GangingInfo ? gangedChannelLevel : normalChannelLevel;
            AssertCurrentSettings(channelOutput, currentLevel, voltageLimit);
        }

        private void AssertVoltageSettings(SitePinInfo sitePinInfo, DCPowerOutput channelOutput, double voltageLevel, double gangedChannelLimit, double normalChannelLimit)
        {
            var channelLimit = sitePinInfo.CascadingInfo is GangingInfo ? gangedChannelLimit : normalChannelLimit;
            AssertVoltageSettings(channelOutput, voltageLevel, channelLimit);
        }

        private void AssertSourceTriggerSettings(SitePinInfo sitePinInfo, DCPowerOutput channelOutput, string leaderChannelString)
        {
            Assert.Equal(GetTriggerName(sitePinInfo, leaderChannelString), channelOutput.Triggers.SourceTrigger.DigitalEdge.InputTerminal);
        }

        private void AssertTriggerSettings(SitePinInfo sitePinInfo, DCPowerOutput channelOutput, string leaderChannelString, bool checkSequenceRelatedTriggers = false)
        {
            if (IsFollowerOfGangedChannels(sitePinInfo.CascadingInfo))
            {
                Assert.Equal(GetTriggerName(sitePinInfo, leaderChannelString), channelOutput.Triggers.SourceTrigger.DigitalEdge.InputTerminal);
                Assert.Equal(GetTriggerName(sitePinInfo, leaderChannelString, "Measure"), channelOutput.Triggers.MeasureTrigger.DigitalEdge.InputTerminal);
                Assert.Equal(DCPowerSourceTriggerType.DigitalEdge, channelOutput.Triggers.SourceTrigger.Type);
                Assert.Equal(DCPowerMeasureTriggerType.DigitalEdge, channelOutput.Triggers.MeasureTrigger.Type);
                if (checkSequenceRelatedTriggers)
                {
                    Assert.Equal(GetTriggerName(sitePinInfo, leaderChannelString, "Start"), channelOutput.Triggers.StartTrigger.DigitalEdge.InputTerminal);
                    Assert.Equal(DCPowerStartTriggerType.DigitalEdge, channelOutput.Triggers.StartTrigger.Type);
                    Assert.Equal(GetTriggerName(sitePinInfo, leaderChannelString, "SequenceAdvance"), channelOutput.Triggers.SequenceAdvanceTrigger.DigitalEdge.InputTerminal);
                    Assert.Equal(DCPowerSequenceAdvanceTriggerType.DigitalEdge, channelOutput.Triggers.SequenceAdvanceTrigger.Type);
                }
            }
        }

        private static int[] GetActiveSites(DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.AggregateSitePinList
                .Where(sitePinInfo => sitePinInfo.SiteNumber != -1)
                .Select(sitePinInfo => sitePinInfo.SiteNumber)
                .Distinct()
                .ToArray();
        }

        private void AssertEqualForDoubleArrays(double[] expected, double[] actual, int precision = 3)
        {
            Assert.Equal(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual[i], precision);
            }
        }

        private void AssertSequenceMeasurementsMatchExpected(
            DCPowerSessionsBundle sessionsBundle,
            Func<int, string, double[]> getExpectedSequence,
            double timeoutSeconds = 5.0,
            int precision = 3,
            int itemsToFetch = 2,
            bool checkForCurrentMeasurement = true,
            bool initiateChannel = true,
            string groupName = null)
        {
            var results = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                if (initiateChannel)
                {
                    sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Initiate();
                }
                return sessionInfo.Session.Measurement.Fetch(sitePinInfo.IndividualChannelString, PrecisionTimeSpan.FromSeconds(timeoutSeconds), itemsToFetch);
            });
            foreach (var siteNumber in results.SiteNumbers)
            {
                foreach (var pinName in results.PinNames)
                {
                    var expectedSequence = getExpectedSequence(siteNumber, groupName ?? pinName);
                    var actualSequence = checkForCurrentMeasurement ? results.GetValue(siteNumber, pinName).CurrentMeasurements : results.GetValue(siteNumber, pinName).VoltageMeasurements;
                    AssertEqualForDoubleArrays(expectedSequence, actualSequence, precision);
                }
            }
        }

        private void Assert_ClearAndDeleteConfigureAdvancedSequences(Action action, DCPowerSessionsBundle sessionsBundle, string sequenceName)
        {
            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                action();
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }
    }
}
