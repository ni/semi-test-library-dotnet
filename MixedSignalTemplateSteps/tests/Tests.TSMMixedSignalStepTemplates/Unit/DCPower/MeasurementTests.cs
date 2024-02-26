using System;
using System.Collections.Generic;
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
    public sealed class MeasurementTests : IDisposable
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
            var sessionsBundle = sessionManager.GetDCPowerSessionsBundle(new string[] { "VDD", "VCC", "VDET" });
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
            var sessionsBundle = sessionManager.GetDCPowerSessionsBundle(new string[] { "VDD", "VCC", "VDET" });
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
            var sessionsBundle = sessionManager.GetDCPowerSessionsBundle(new string[] { "VCC", "VDD", "VDET" });
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
            var sessionsBundle = sessionManager.GetDCPowerSessionsBundle(new string[] { "VCC", "VDD" });
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
            var sessionsBundle = sessionManager.GetDCPowerSessionsBundle(new string[] { "VCC", "VDD" });
            var currentLevels = new Dictionary<string, DCPowerSettings>()
            {
                ["VCC"] = new DCPowerSettings() { SourceSettings = new DCPowerSourceSettings() { Level = 0.1, Limit = 5 } },
                ["VDD"] = new DCPowerSettings() { SourceSettings = new DCPowerSourceSettings() { Level = 0.2, Limit = 5 } }
            };
            sessionsBundle.ForceCurrentSymmetricLimit(currentLevels);

            var results = sessionsBundle.MeasureAndReturnPerSitePerPinResults();

            Assert.Equal(0.1, results.Item2.Values[0]["VCC"]);
            Assert.Equal(0.2, results.Item2.Values[0]["VDD"]);
            Assert.Equal(0.1, results.Item2.Values[1]["VCC"]);
            Assert.Equal(0.2, results.Item2.Values[1]["VDD"]);
            Assert.Equal(0.1, results.Item2.Values[2]["VCC"]);
            Assert.Equal(0.2, results.Item2.Values[2]["VDD"]);
            Assert.Equal(0.1, results.Item2.Values[3]["VCC"]);
            Assert.Equal(0.2, results.Item2.Values[3]["VDD"]);
        }
    }
}
