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
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

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
        public void DifferentSMUDevices_ForcePerSiteVoltagesWithSymmetricLimit_CorrectVoltagesForced(string pinMapFileName, bool pinMapWithChannelGroup)
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
        public void DifferentSMUDevices_ForcePerPinPerSiteVoltagesWithSymmetricLimit_CorrectVoltagesForced(bool pinMapWithChannelGroup)
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

            AssertSequenceMeasurementsMatchExpected(sessionsBundle, _ => sequence, precision: 3, itemsToFetch: 3, checkForCurrentMeasurement: false, initiateChannel: false);
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
                voltageSequences: sequences,
                currentLimits: currentLimits,
                voltageLevelRanges: voltageLevelRanges,
                currentLimitRanges: currentLimitRanges);

            AssertSequenceMeasurementsMatchExpected(sessionsBundle, siteIndex => sequences.GetValue(siteIndex, "VDD"), precision: 3, itemsToFetch: 3, checkForCurrentMeasurement: false, initiateChannel: false);
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
                voltageSequences: sequences,
                currentLimits: currentLimits,
                voltageLevelRanges: voltageLevelRanges,
                currentLimitRanges: currentLimitRanges);

            AssertSequenceMeasurementsMatchExpected(sessionsBundle, siteIndex => sequences.GetValue(siteIndex), precision: 3, itemsToFetch: 3, checkForCurrentMeasurement: false, initiateChannel: false);
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
            AssertSequenceMeasurementsMatchExpected(sessionsBundle, _ => sequence, precision: 3, itemsToFetch: 3);
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
                currentSequences: currentSequence,
                voltageLimits: voltageLimits,
                currentLevelRanges: currentLevelRanges,
                voltageLimitRanges: voltageLimitRanges);

            sessionsBundle.Abort();
            AssertSequenceMeasurementsMatchExpected(sessionsBundle, siteIndex => currentSequence.GetValue(siteIndex, "VDD"), precision: 3, itemsToFetch: 3);
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
                currentSequences: currentSequences,
                voltageLimits: voltageLimits,
                currentLevelRanges: currentLevelRanges,
                voltageLimitRanges: voltageLimitRanges);

            sessionsBundle.Abort();
            AssertSequenceMeasurementsMatchExpected(sessionsBundle, siteIndex => currentSequences.GetValue(siteIndex), precision: 3, itemsToFetch: 3);
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
            AssertSequenceMeasurementsMatchExpected(sessionsBundle, _ => sequence, precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false);
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
            AssertSequenceMeasurementsMatchExpected(sessionsBundle, siteIndex => sequence.GetValue(siteIndex), precision: 1, itemsToFetch: 3, checkForCurrentMeasurement: false);
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
            AssertSequenceMeasurementsMatchExpected(sessionsBundle, siteIndex => sequence.GetValue(siteIndex, "VDD"), precision: 1, itemsToFetch: 3, checkForCurrentMeasurement: false);
            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: currentLimit, expectedCurrentLimitRange: (double?)currentLimitRange));
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequenceWithDefaultParameters_SequenceAppliedSuccessfully(bool pinMapWithChannelGroup)
        {
            var pinNames = new string[] { "VDD" };
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(pinNames);

            var sequence = new[] { 0.5, 1.0, 1.5 };
            sessionsBundle.ForceVoltageSequence(voltageSequence: sequence);

            sessionsBundle.Abort();
            AssertSequenceMeasurementsMatchExpected(sessionsBundle, _ => sequence, precision: 1, itemsToFetch: 3, checkForCurrentMeasurement: false);
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
                if (sitePinInfo.CascadingInfo is GangingInfo)
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 0.5, expectedVoltageLimit: 2);
                }
                else
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 1.5, expectedVoltageLimit: 2);
                }
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForcePerPinCurrentsWithSymmetricLimit_CorrectCurrentsForced()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ForceCurrent(currentLevels: new Dictionary<string, double>() { ["VCC1"] = 3, ["VCC2"] = 3, ["VCC3"] = 3, ["VCC4"] = 1, ["VCC5"] = 1 }, voltageLimit: 5);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                if (sitePinInfo.CascadingInfo is GangingInfo)
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 1, expectedVoltageLimit: 5);
                }
                else
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 1, expectedVoltageLimit: 5);
                }
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
                if (sitePinInfo.SiteNumber == 0)
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 3);
                }
                else
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 0.6, expectedVoltageLimit: 3);
                }
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
                ["VCC1"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 },
                ["VCC2"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 },
                ["VCC3"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 },
                ["VCC4"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 },
                ["VCC5"] = new Dictionary<int, double>() { [0] = 4, [1] = 2.5 }
            });
            sessionsBundle.ForceCurrent(currentLevels, voltageLimit: 4.5);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                if (sitePinInfo.SiteNumber == 0)
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 0.8, expectedVoltageLimit: 4.5);
                }
                else
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 0.5, expectedVoltageLimit: 4.5);
                }
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
                if (sitePinInfo.CascadingInfo is GangingInfo)
                {
                    AssertCurrentSettings(channelOutput, 0.5, 2.6);
                }
                else
                {
                    AssertCurrentSettings(channelOutput, 2, 2.6);
                }
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
                if (sitePinInfo.CascadingInfo is GangingInfo)
                {
                    AssertVoltageSettings(channelOutput, expectedVoltageLevel: 2, expectedCurrentLimit: 0.5);
                }
                else
                {
                    AssertVoltageSettings(channelOutput, expectedVoltageLevel: 2, expectedCurrentLimit: 1.5);
                }
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForcePerPinVoltagesWithSymmetricLimit_VoltagesForcedAndCurrentLimitDividedCorrectly()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ForceVoltage(voltageLevels: new Dictionary<string, double>() { ["VCC1"] = 2, ["VCC2"] = 2, ["VCC3"] = 2, ["VCC4"] = 2, ["VCC5"] = 2 }, currentLimit: 0.6);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                if (sitePinInfo.CascadingInfo is GangingInfo)
                {
                    AssertVoltageSettings(channelOutput, expectedVoltageLevel: 2, expectedCurrentLimit: 0.2, 1);
                }
                else
                {
                    AssertVoltageSettings(channelOutput, expectedVoltageLevel: 2, expectedCurrentLimit: 0.6);
                }
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
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
                if (sitePinInfo.SiteNumber == 0)
                {
                    AssertVoltageSettings(channelOutput, expectedVoltageLevel: 4.0, expectedCurrentLimit: 0.9);
                }
                else
                {
                    AssertVoltageSettings(channelOutput, expectedVoltageLevel: 2.5, expectedCurrentLimit: 0.9);
                }
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
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
                if (sitePinInfo.CascadingInfo is GangingInfo)
                {
                    AssertVoltageSettings(channelOutput, 2.6, 0.5);
                }
                else
                {
                    AssertVoltageSettings(channelOutput, 2.6, 2);
                }
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
                new DCPowerSourceSettings() { Level = 5, Limit = 3.6 },
                new DCPowerSourceSettings() { Level = 2, Limit = 5 },
            });
            sessionsBundle.ForceCurrent(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                if (sitePinInfo.SiteNumber == 0)
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 1, expectedVoltageLimit: 3.6);
                }
                else
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 0.4, expectedVoltageLimit: 5);
                }
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
                ["VCC1"] = new DCPowerSourceSettings() { Level = 3, Limit = 4 },
                ["VCC2"] = new DCPowerSourceSettings() { Level = 3, Limit = 4 },
                ["VCC3"] = new DCPowerSourceSettings() { Level = 3, Limit = 4 },
                ["VCC4"] = new DCPowerSourceSettings() { Level = 2, Limit = 4 },
                ["VCC5"] = new DCPowerSourceSettings() { Level = 2, Limit = 4 }
            };
            sessionsBundle.ForceCurrent(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                if (sitePinInfo.CascadingInfo is GangingInfo)
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 1, expectedVoltageLimit: 4);
                }
                else
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 2, expectedVoltageLimit: 4);
                }
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
                    Level = 3,
                    Limit = 6
                });
            sessionsBundle.ForceCurrent(settings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, channelOutput.Source.ComplianceLimitSymmetry);
                if (sitePinInfo.CascadingInfo is GangingInfo)
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 0.75, expectedVoltageLimit: 6);
                }
                else
                {
                    AssertCurrentSettings(channelOutput, expectedCurrentLevel: 3, expectedVoltageLimit: 6);
                }
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
        public void DifferentSMUDevices_ForceCurrentSequence_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { -0.02, 0.03, 0.04, 0.05, 0.07 };
            sessionsBundle.ForceCurrentSequence(currentSequence: sequence, voltageLimit: 0.5, currentLevelRange: 0.1, voltageLimitRange: 1, sequenceLoopCount: 1);

            sessionsBundle.Abort();
            AssertSequenceMeasurementsMatchExpected(sessionsBundle, _ => sequence, precision: 2, itemsToFetch: 5);
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedCurrentLevelRange: 0.1, expectedVoltageLimit: 0.5, expectedVoltageLimitRange: 1, expectedSequenceLoopCount: 1));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceWithPerSiteSequence_CorrectValueAreSet(bool pinMapWithChannelGroup)
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
            sessionsBundle.ForceCurrentSequence(currentSequences: sequence, voltageLimit: 0.5, currentLevelRange: 0.1, voltageLimitRange: 1, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            AssertSequenceMeasurementsMatchExpected(sessionsBundle, siteIndex => sequence.GetValue(siteIndex), precision: 3, itemsToFetch: 2);
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedCurrentLevelRange: 0.1, expectedVoltageLimit: 0.5, expectedVoltageLimitRange: 1, expectedSequenceLoopCount: 2));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceWithPerPinPerSiteSequence_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>() { [0] = new[] { 0.005, 0.010 }, [1] = new[] { 0.006, 0.012 }, [2] = new[] { 0.007, 0.014 }, [3] = new[] { 0.008, 0.016 } }
            });
            sessionsBundle.ForceCurrentSequence(currentSequences: sequence, voltageLimit: 0.5, currentLevelRange: 0.1, voltageLimitRange: 1, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            AssertSequenceMeasurementsMatchExpected(sessionsBundle, siteIndex => sequence.GetValue(siteIndex, "VDD"), precision: 2, itemsToFetch: 2);
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedCurrentLevelRange: 0.1, expectedVoltageLimit: 0.5, expectedVoltageLimitRange: 1, expectedSequenceLoopCount: 2));
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
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
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
                Level = 2,
                Limit = 7
            };
            var settingsForSite2 = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                Level = 3,
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
                AssertCurrentSettings(channelOutput, expectedCurrentLevel: sitePinInfo.SiteNumber == 0 ? 0.4 : 0.6, expectedVoltageLimit: sitePinInfo.SiteNumber == 0 ? 7 : 5);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSourceSettingsWithPinSiteData_CorrectValuesAreSetAndCurrentLevelDivided()
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
                    Level = 3,
                    Limit = 6
                });
            sessionsBundle.ConfigureSourceSettings(perPinPerSiteSettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCCurrent, channelOutput.Source.Output.Function);
                AssertCurrentSettings(sitePinInfo, channelOutput, 0.75, 3, 6);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigurePerPinCurrentSourceSettings_CorrectValuesAreSetAndCurrentLevelDivided()
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
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
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
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
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
                    Limit = 3,
                });
            sessionsBundle.ConfigureSourceSettings(perSiteSettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                AssertVoltageSettings(sitePinInfo, channelOutput, 4, 0.75, 3);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSourceSettingsWithPinSiteData_CorrectValuesAreSetAndCurrentLimitDivided()
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
                    Limit = 3
                });
            sessionsBundle.ConfigureSourceSettings(perPinPerSiteSettings);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceOutputFunction.DCVoltage, channelOutput.Source.Output.Function);
                AssertVoltageSettings(sitePinInfo, channelOutput, 3, 1, 3);
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigurePerPinVoltageSourceSettings_CorrectValuesAreSetAndCurrentLimitDivided()
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
                AssertTriggerSettings(sitePinInfo, channelOutput, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0");
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
        public void DifferentSMUDevices_SetSequence_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequence = new double[] { 1, 2, 3, 4, 5 };
            // Cannot test sequence loop count because it's not supported in offline mode.
            sessionsBundle.ConfigureSequence(expectedSequence, sequenceLoopCount: 1);
            var results = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Initiate();
                // Cannot test fetch backlog because it's always 1 in offline mode.
                return sessionInfo.Session.Measurement.Fetch(sessionInfo.AllChannelsString, PrecisionTimeSpan.FromSeconds(1), 5);
            });

            Assert.Equal(expectedSequence, results[0].VoltageMeasurements);
            Assert.Equal(expectedSequence, results[1].VoltageMeasurements);
            Assert.Equal(expectedSequence, results[2].VoltageMeasurements);
            Assert.Equal(expectedSequence, results[3].VoltageMeasurements);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetSequenceWithStepDeltaTime_SequenceStepDeltaTimeEnabled(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.False(sessionInfo.AllChannelsOutput.Source.SequenceStepDeltaTimeEnabled);
            });
            sessionsBundle.ConfigureSequence(sequence: new double[] { 1, 2, 3 }, sequenceLoopCount: 1, sequenceStepDeltaTimeInSeconds: 0.05);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.True(sessionInfo.AllChannelsOutput.Source.SequenceStepDeltaTimeEnabled);
                Assert.Equal(0.05, sessionInfo.AllChannelsOutput.Source.SequenceStepDeltaTime.TotalSeconds);
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureSequenceWithSiteData_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                new[] { 2.0, 3.0, 4.0, 5.0, 6.0 },
                new[] { 2.5, 3.5, 4.5, 5.5, 6.5 }
            });
            sessionsBundle.ConfigureSequence(expectedSequences, sequenceLoopCount: 1);

            AssertSequenceMeasurementsMatchExpected(sessionsBundle, siteIndex => expectedSequences.GetValue(siteIndex), precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureSequenceWithPinSiteData_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

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
            sessionsBundle.ConfigureSequence(expectedSequences, sequenceLoopCount: 1);

            AssertSequenceMeasurementsMatchExpected(sessionsBundle, siteIndex => expectedSequences.GetValue(siteIndex, "VDD"), precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureSequenceWithSourceDelaySucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                new[] { 2.0, 3.0, 4.0, 5.0, 6.0 },
                new[] { 2.5, 3.5, 4.5, 5.5, 6.5 }
            });
            var expectedSourceDelayInSeconds = new SiteData<double[]>(new double[][]
            {
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 }
            });
            sessionsBundle.ConfigureSequenceWithSourceDelays(expectedSequences, expectedSourceDelayInSeconds, sequenceLoopCount: 1);
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

        private void AssertCurrentSettings(DCPowerOutput channelOutput, double expectedCurrentLevelRange, double expectedVoltageLimit, double expectedVoltageLimitRange, double expectedSequenceLoopCount)
        {
            Assert.Equal(expectedCurrentLevelRange, channelOutput.Source.Current.CurrentLevelRange);
            Assert.Equal(expectedVoltageLimit, channelOutput.Source.Current.VoltageLimit);
            Assert.Equal(expectedVoltageLimitRange, channelOutput.Source.Current.VoltageLimitRange);
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
            if (sitePinInfo.CascadingInfo is GangingInfo)
            {
                AssertCurrentSettings(channelOutput, gangedChannelLevel, voltageLimit);
            }
            else
            {
                AssertCurrentSettings(channelOutput, normalChannelLevel, voltageLimit);
            }
        }

        private void AssertVoltageSettings(SitePinInfo sitePinInfo, DCPowerOutput channelOutput, double voltageLevel, double gangedChannelLimit, double normalChannelLimit)
        {
            if (sitePinInfo.CascadingInfo is GangingInfo)
            {
                AssertVoltageSettings(channelOutput, voltageLevel, gangedChannelLimit);
            }
            else
            {
                AssertVoltageSettings(channelOutput, voltageLevel, normalChannelLimit);
            }
        }

        private void AssertTriggerSettings(SitePinInfo sitePinInfo, DCPowerOutput channelOutput, string leaderChannelString)
        {
            Assert.Equal(GetTriggerName(sitePinInfo, leaderChannelString), channelOutput.Triggers.SourceTrigger.DigitalEdge.InputTerminal);
        }

        private string GetTriggerName(SitePinInfo sitePinInfo, string leaderChannelString)
        {
            var channel = sitePinInfo.IndividualChannelString;
            var leaderChannel = leaderChannelString.Split('/');
            var leaderChannelSlot = leaderChannel[0];
            var leaderChannelNumber = leaderChannel[leaderChannel.Length - 1];

            if (sitePinInfo.CascadingInfo is GangingInfo gangingInfo && gangingInfo.IsFollower)
            {
                return $"/{leaderChannelSlot}/Engine{leaderChannelNumber}/SourceTrigger";
            }
            if (channel.Contains("SMU_4147"))
            {
                return $"/{channel.Remove(channel.Length - 2)}/Immediate";
            }
            return string.Empty;
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
            Func<int, double[]> getExpectedSequence,
            int siteCount = 2,
            double timeoutSeconds = 5.0,
            int precision = 3,
            int itemsToFetch = 2,
            bool checkForCurrentMeasurement = true,
            bool initiateChannel = true)
        {
            var results = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults((sessionInfo, sitePinInfo) =>
            {
                if (initiateChannel)
                {
                    sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Initiate();
                }
                return sessionInfo.Session.Measurement.Fetch(sitePinInfo.IndividualChannelString, PrecisionTimeSpan.FromSeconds(timeoutSeconds), itemsToFetch);
            });

            for (int i = 0; i < siteCount; i++)
            {
                var expectedSequence = getExpectedSequence(i);
                var actualSequence = checkForCurrentMeasurement ? results[i][0].CurrentMeasurements : results[i][0].VoltageMeasurements;
                AssertEqualForDoubleArrays(expectedSequence, actualSequence, precision);
            }
        }
    }
}
