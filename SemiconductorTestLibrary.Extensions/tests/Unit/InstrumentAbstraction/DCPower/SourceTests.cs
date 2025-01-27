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
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
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
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageWithAsymmetricLimitAndRangesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceVoltageAsymmetricLimit(voltageLevel: 3.6, currentLimitHigh: 0.2, currentLimitLow: -0.1, voltageLevelRange: 5, currentLimitRange: 0.5);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
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
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentWithAsymmetricLimitAndRangesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceCurrentAsymmetricLimit(currentLevel: 0.1, voltageLimitHigh: 3, voltageLimitLow: -1, currentLevelRange: 0.5, voltageLimitRange: 5);
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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureOutputConnected_OutputConnectedThrowsExceptionWhenUnsupported(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            var values = new SiteData<bool>(GetActiveSites(sessionsBundle), false);

            void ConfigureOutputConnected()
            {
                sessionsBundle.ConfigureOutputConnected(values);
            }

            AggregateException aggregateException = Assert.Throws<AggregateException>(ConfigureOutputConnected);
            foreach (Exception innerExeption in aggregateException.InnerExceptions)
            {
                Assert.Contains("Invalid value for parameter or property.", innerExeption.InnerException.Message);
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
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
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

#pragma warning disable xUnit1013 // Public method should be marked as test
        public void DifferentSMUDevices_ForceVoltageSequenceSynchronized_VoltageSequenceForced()
#pragma warning restore xUnit1013 // Public method should be marked as test
        {
            // The measure unit is not enabled (i.e. MeasureWhen is not set to AutomaticallyAfterSourceComplete) when forcing the voltage sequence in the target method,
            // making testing not possible. It's also very hard to test the forcing is synchronized.
        }

        private void AssertVoltageSettings(DCPowerOutput channelOutput, double expectedVoltageLevel, double expectedCurrentLimit)
        {
            Assert.Equal(expectedVoltageLevel, channelOutput.Source.Voltage.VoltageLevel);
            Assert.Equal(expectedCurrentLimit, channelOutput.Source.Voltage.CurrentLimit);
        }

        private void AssertVoltageSettings(DCPowerOutput channelOutput, double expectedVoltageLevel, double expectedCurrentLimitHigh, double expectedCurrentLimitLow)
        {
            Assert.Equal(expectedVoltageLevel, channelOutput.Source.Voltage.VoltageLevel);
            Assert.Equal(expectedCurrentLimitHigh, channelOutput.Source.Voltage.CurrentLimitHigh);
            Assert.Equal(expectedCurrentLimitLow, channelOutput.Source.Voltage.CurrentLimitLow);
        }

        private void AssertCurrentSettings(DCPowerOutput channelOutput, double expectedCurrentLevel, double expectedVoltageLimit)
        {
            Assert.Equal(expectedCurrentLevel, channelOutput.Source.Current.CurrentLevel);
            Assert.Equal(expectedVoltageLimit, channelOutput.Source.Current.VoltageLimit);
        }

        private void AssertCurrentSettings(DCPowerOutput channelOutput, double expectedCurrentLevel, double expectedVoltageLimitHigh, double expectedVoltageLimitLow)
        {
            Assert.Equal(expectedCurrentLevel, channelOutput.Source.Current.CurrentLevel);
            Assert.Equal(expectedVoltageLimitHigh, channelOutput.Source.Current.VoltageLimitHigh);
            Assert.Equal(expectedVoltageLimitLow, channelOutput.Source.Current.VoltageLimitLow);
        }

        private static int[] GetActiveSites(DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.AggregateSitePinList
                .Where(sitePinInfo => sitePinInfo.SiteNumber != -1)
                .Select(sitePinInfo => sitePinInfo.SiteNumber)
                .Distinct()
                .ToArray();
        }
    }
}
