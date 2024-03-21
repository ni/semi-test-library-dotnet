using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DCPower
{
    [Collection("NonParallelizable")]
    public sealed class SourceTests : IDisposable
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
        public void DifferentSMUDevices_ForceSameVoltageWithSymmetricLimit_SameVoltageForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            sessionsBundle.ForceVoltageSymmetricLimit(voltageLevel: 3.6, currentLimit: 0.1);

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => AssertVoltageSettings(sessionInfo.ChannelOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1));
            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, sessionInfo.ChannelOutput.Source.ComplianceLimitSymmetry));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForcePerPinVoltagesWithSymmetricLimit_CorrectVoltagesForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET" });

            sessionsBundle.ForceVoltageSymmetricLimit(voltageLevels: new Dictionary<string, double>() { ["VCC"] = 1, ["VDD"] = 2, ["VDET"] = 3 }, currentLimit: 0.1);

            if (pinMapWithChannelGroup)
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/0"], expectedVoltageLevel: 1, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).Session.Outputs["SMU_4147_C1_S11/0"], expectedVoltageLevel: 2, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).Session.Outputs["SMU_4110_C1_S04/1"], expectedVoltageLevel: 3, expectedCurrentLimit: 0.1);
            }
            else
            {
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput, expectedVoltageLevel: 1, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput, expectedVoltageLevel: 2, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput, expectedVoltageLevel: 3, expectedCurrentLimit: 0.1);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForcePerSiteVoltagesWithSymmetricLimit_CorrectVoltagesForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            var voltageLevels = new Dictionary<int, Dictionary<string, double>>()
            {
                [0] = new Dictionary<string, double>() { ["VCC"] = 1, ["VDET"] = 1.5 },
                [1] = new Dictionary<string, double>() { ["VCC"] = 2, ["VDET"] = 2.5 },
                [2] = new Dictionary<string, double>() { ["VCC"] = 3, ["VDET"] = 3.5 },
                [3] = new Dictionary<string, double>() { ["VCC"] = 4, ["VDET"] = 4.5 }
            };
            sessionsBundle.ForceVoltageSymmetricLimit(voltageLevels, currentLimit: 0.1);

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
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput, expectedVoltageLevel: 1, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput, expectedVoltageLevel: 1.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput, expectedVoltageLevel: 2, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).ChannelOutput, expectedVoltageLevel: 2.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(4).ChannelOutput, expectedVoltageLevel: 3, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(5).ChannelOutput, expectedVoltageLevel: 3.5, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(6).ChannelOutput, expectedVoltageLevel: 4, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(7).ChannelOutput, expectedVoltageLevel: 4.5, expectedCurrentLimit: 0.1);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageWithSymmetricLimitWithSettingsObject_SameVoltageForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            var settings = new Dictionary<string, DCPowerSettings>()
            {
                ["VCC"] = new DCPowerSettings() { SourceSettings = new DCPowerSourceSettings() { Level = 3.6, Limit = 0.1 } },
                ["VDET"] = new DCPowerSettings() { SourceSettings = new DCPowerSourceSettings() { Level = 5.0, Limit = 0.2 } }
            };
            sessionsBundle.ForceVoltageSymmetricLimit(settings);

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
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput, expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(3).ChannelOutput, expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(4).ChannelOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(5).ChannelOutput, expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(6).ChannelOutput, expectedVoltageLevel: 3.6, expectedCurrentLimit: 0.1);
                AssertVoltageSettings(sessionsBundle.InstrumentSessions.ElementAt(7).ChannelOutput, expectedVoltageLevel: 5, expectedCurrentLimit: 0.2);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageWithSymmetricLimitAndRangesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            sessionsBundle.ForceVoltageSymmetricLimit(voltageLevel: 3.6, currentLimit: 0.1, voltageLevelRange: 5, currentLimitRange: 0.5);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceSameVoltageWithAsymmetricLimit_SameVoltageForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceVoltageAsymmetricLimit(voltageLevel: 3.6, currentLimitHigh: 0.2, currentLimitLow: -0.1);

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => AssertVoltageSettings(sessionInfo.ChannelOutput, expectedVoltageLevel: 3.6, expectedCurrentLimitHigh: 0.2, expectedCurrentLimitLow: -0.1));
            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(DCPowerComplianceLimitSymmetry.Asymmetric, sessionInfo.ChannelOutput.Source.ComplianceLimitSymmetry));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageWithAsymmetricLimitAndRangesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceVoltageAsymmetricLimit(voltageLevel: 3.6, currentLimitHigh: 0.2, currentLimitLow: -0.1, voltageLevelRange: 5, currentLimitRange: 0.5);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceSameCurrentWithSymmetricLimit_SameCurrentForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            sessionsBundle.ForceCurrentSymmetricLimit(currentLevel: 0.1, voltageLimit: 5);

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => AssertCurrentSettings(sessionInfo.ChannelOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 5));
            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(DCPowerComplianceLimitSymmetry.Symmetric, sessionInfo.ChannelOutput.Source.ComplianceLimitSymmetry));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentWithSymmetricLimitWithSettingsObject_SameCurrentForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDET" });

            var settings = new Dictionary<string, DCPowerSettings>()
            {
                ["VCC"] = new DCPowerSettings() { SourceSettings = new DCPowerSourceSettings() { Level = 0.1, Limit = 3 } },
                ["VDET"] = new DCPowerSettings() { SourceSettings = new DCPowerSourceSettings() { Level = 0.2, Limit = 5 } }
            };
            sessionsBundle.ForceCurrentSymmetricLimit(settings);

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
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(0).ChannelOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(1).ChannelOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(2).ChannelOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(3).ChannelOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(4).ChannelOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(5).ChannelOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(6).ChannelOutput, expectedCurrentLevel: 0.1, expectedVoltageLimit: 3);
                AssertCurrentSettings(sessionsBundle.InstrumentSessions.ElementAt(7).ChannelOutput, expectedCurrentLevel: 0.2, expectedVoltageLimit: 5);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentWithSymmetricLimitAndRangesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VCC");

            sessionsBundle.ForceCurrentSymmetricLimit(currentLevel: 0.1, voltageLimit: 5, currentLevelRange: 0.5, voltageLimitRange: 5);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceSameCurrentWithAsymmetricLimit_SameCurrentForced(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceCurrentAsymmetricLimit(currentLevel: 0.1, voltageLimitHigh: 3, voltageLimitLow: -1);

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => AssertCurrentSettings(sessionInfo.ChannelOutput, expectedCurrentLevel: 0.1, expectedVoltageLimitHigh: 3, expectedVoltageLimitLow: -1));
            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(DCPowerComplianceLimitSymmetry.Asymmetric, sessionInfo.ChannelOutput.Source.ComplianceLimitSymmetry));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentWithAsymmetricLimitAndRangesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ForceCurrentAsymmetricLimit(currentLevel: 0.1, voltageLimitHigh: 3, voltageLimitLow: -1, currentLevelRange: 0.5, voltageLimitRange: 5);
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
    }
}
