using System;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    [Collection("NonParallelizable")]
    public sealed class PPMUShharedTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager InitializeSessionsAndCreateSessionManager(string pinMapFileName, string digitalPatternProjectFileName)
        {
            return InitializeSessionsAndCreateSessionManager(pinMapFileName, digitalPatternProjectFileName, out _);
        }

        public TSMSessionManager InitializeSessionsAndCreateSessionManager(string pinMapFileName, string digitalPatternProjectFileName, out IPublishedDataReader publishedDataReader)
        {
            _tsmContext = CreateTSMContext(pinMapFileName, out publishedDataReader, digitalPatternProjectFileName);
            Initialize(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        [Theory]
        [InlineData("DigiPin")]
        [InlineData("DigiPinSharedSameSessionSameInstrument")]
        [InlineData("DigiPinSharedSameSessionDifferentInstruments")]
        [InlineData("DigiPinSharedSeperateSessionsDifferentInstruments")]
        public void SharedChannelsMeasureAfterSourceComplete_ForceVoltageMeasureCurrent_AllChannelsMeasureSameValue(string pinName)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("SharedPinTests2.pinmap", "SharedPinTests2.digiproj");
            var sessionsBundle = sessionManager.Digital(pinName);

            sessionsBundle.ForceVoltage(voltageLevel: 3.6);
            var results = sessionsBundle.MeasureCurrent();

            AssertAllChannelsHaveSameResult(results);
        }

        [Theory]
        [InlineData("DigiPin")]
        [InlineData("DigiPinSharedSameSessionSameInstrument")]
        [InlineData("DigiPinSharedSameSessionDifferentInstruments")]
        [InlineData("DigiPinSharedSeperateSessionsDifferentInstruments")]
        public void SharedChannelsMeasureOnDemand_ForceVoltageMeasureCurrent_AllChannelsMeasureSameValue(string pinName)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("SharedPinTests2.pinmap", "SharedPinTests2.digiproj");
            var sessionsBundle = sessionManager.Digital(pinName);

            sessionsBundle.ForceVoltage(voltageLevel: 3.6);
            var results = sessionsBundle.MeasureCurrent();

            AssertAllChannelsHaveSameResult(results);
        }

        [Theory]
        [InlineData("DigiPin")]
        [InlineData("DigiPinSharedSameSessionSameInstrument")]
        [InlineData("DigiPinSharedSameSessionDifferentInstruments")]
        [InlineData("DigiPinSharedSeperateSessionsDifferentInstruments")]
        public void SharedChannelsMeasureAfterSourceComplete_ForceCurrentMeasureVoltage_AllChannelsMeasureSameValue(string pinName)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("SharedPinTests2.pinmap", "SharedPinTests2.digiproj");
            var sessionsBundle = sessionManager.Digital(pinName);

            sessionsBundle.ForceCurrent(currentLevel: .010);
            var results = sessionsBundle.MeasureVoltage();

            AssertAllChannelsHaveSameResult(results);
        }

        [Theory]
        [InlineData("DigiPin")]
        [InlineData("DigiPinSharedSameSessionSameInstrument")]
        [InlineData("DigiPinSharedSameSessionDifferentInstruments")]
        [InlineData("DigiPinSharedSeperateSessionsDifferentInstruments")]
        public void SharedChannelsMeasureOnDemand_ForceCurrentMeasureVoltage_AllChannelsMeasureSameValue(string pinName)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("SharedPinTests2.pinmap", "SharedPinTests2.digiproj");
            var sessionsBundle = sessionManager.Digital(pinName);

            sessionsBundle.ForceCurrent(currentLevel: .010);
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
