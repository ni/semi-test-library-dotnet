using System;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DCPower
{
    [Collection("NonParallelizable")]
    public sealed class MeasureSharedPinsTests : IDisposable
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
        [InlineData("SmuPin")]
        [InlineData("SmuPinSharedSameSessionSameInstrument")]
        [InlineData("SmuPinSharedSeperateSessionsSameInstrument")]
        [InlineData("SmuPinPinSharedSameSessionDifferentInstruments")]
        [InlineData("SmuPinPinSharedSeperateSessionsDifferentInstruments")]
        public void SharedChannelsMeasureAfterSourceComplete_ForceVoltageMeasureCurrent_AllChannelsMeasureSameValue(string pinName)
        {
            var sessionManager = Initialize("SharedPinTests2.pinmap");
            var sessionsBundle = sessionManager.DCPower(pinName);
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            sessionsBundle.ForceVoltage(voltageLevel: 3.6, waitForSourceCompletion: true);
            var results = sessionsBundle.MeasureCurrent();

            AssertAllChannelsHaveSameResult(results);
        }

        [Theory]
        [InlineData("SmuPin")]
        [InlineData("SmuPinSharedSameSessionSameInstrument")]
        [InlineData("SmuPinSharedSeperateSessionsSameInstrument")]
        [InlineData("SmuPinPinSharedSameSessionDifferentInstruments")]
        [InlineData("SmuPinPinSharedSeperateSessionsDifferentInstruments")]
        public void SharedChannelsMeasureOnDemand_ForceVoltageMeasureCurrent_AllChannelsMeasureSameValue(string pinName)
        {
            var sessionManager = Initialize("SharedPinTests2.pinmap");
            var sessionsBundle = sessionManager.DCPower(pinName);
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.OnDemand);

            sessionsBundle.ForceVoltage(voltageLevel: 3.6, waitForSourceCompletion: true);
            var results = sessionsBundle.MeasureCurrent();

            AssertAllChannelsHaveSameResult(results);
        }

        [Theory]
        [InlineData("SmuPin")]
        [InlineData("SmuPinSharedSameSessionSameInstrument")]
        [InlineData("SmuPinSharedSeperateSessionsSameInstrument")]
        [InlineData("SmuPinPinSharedSameSessionDifferentInstruments")]
        [InlineData("SmuPinPinSharedSeperateSessionsDifferentInstruments")]
        public void SharedChannelsMeasureAfterSourceComplete_ForceCurrentMeasureVoltage_AllChannelsMeasureSameValue(string pinName)
        {
            var sessionManager = Initialize("SharedPinTests2.pinmap");
            var sessionsBundle = sessionManager.DCPower(pinName);
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            sessionsBundle.ForceCurrent(currentLevel: .100, waitForSourceCompletion: true);
            var results = sessionsBundle.MeasureVoltage();

            AssertAllChannelsHaveSameResult(results);
        }

        [Theory]
        [InlineData("SmuPin")]
        [InlineData("SmuPinSharedSameSessionSameInstrument")]
        [InlineData("SmuPinSharedSeperateSessionsSameInstrument")]
        [InlineData("SmuPinPinSharedSameSessionDifferentInstruments")]
        [InlineData("SmuPinPinSharedSeperateSessionsDifferentInstruments")]
        public void SharedChannelsMeasureOnDemand_ForceCurrentMeasureVoltage_AllChannelsMeasureSameValue(string pinName)
        {
            var sessionManager = Initialize("SharedPinTests2.pinmap");
            var sessionsBundle = sessionManager.DCPower(pinName);
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.OnDemand);

            sessionsBundle.ForceCurrent(currentLevel: .100, waitForSourceCompletion: true);
            var results = sessionsBundle.MeasureVoltage();

            AssertAllChannelsHaveSameResult(results);
        }

        private void AssertAllChannelsHaveSameResult(PinSiteData<double> results)
        {
            var firstVal = results.GetValue(0, results.PinNames[0]);
            foreach (var siteNumber in results.SiteNumbers)
            {
                foreach (var pin in results.PinNames)
                {
                    Assert.Equal(firstVal, results.GetValue(siteNumber, pin));
                }
            }
        }
    }
}
