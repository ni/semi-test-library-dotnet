using System;
using System.Linq;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Fgen
{
    [Collection("NonParallelizable")]
    public sealed class StandardWaveformGenerationTests : IDisposable
    {
        private const double Tolerance = 0.001;
        private static readonly string[] PinNames = new string[] { "A", "B" };
        private static readonly int[] SiteNumbers = new int[] { 0, 1 };

        private ISemiconductorModuleContext _tsmContext;

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

        public static TheoryData<string, StandardWaveform> PinMapAndSupportedWaveformFunctionTypes()
        {
            const string PinMap = "FgenSingleInstrumentPerPin.pinmap";
            var waveformTypes = Enum.GetValues(typeof(StandardWaveform))
                .Cast<StandardWaveform>()
                .Where(waveform => waveform != StandardWaveform.User);

            var data = new TheoryData<string, StandardWaveform>();
            foreach (var waveform in waveformTypes)
            {
                data.Add(PinMap, waveform);
            }
            return data;
        }

        [Theory]
        [MemberData(nameof(PinMapAndSupportedWaveformFunctionTypes))]
        public void InitializeBundleWithMultiplePins_ConfigureStandardWaveformWithAllSupportedFunctionType_AllChannelsConfiguredCorrectly(string pinmap, StandardWaveform waveformFunctionType)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(PinNames);
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var standardWaveformSettings = new StandardWaveformSettings(
                functionType: waveformFunctionType,
                amplitude: 5.0,
                frequency: 1000.0,
                dcOffset: 2.0,
                startPhase: 90.0);

            sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings);

            var configuredSettings = GetStandardWaveformSettings(sessionsBundle);
            foreach (var site in SiteNumbers)
            {
                foreach (var pin in PinNames)
                {
                    AssertStandardWaveformSettings(standardWaveformSettings, configuredSettings.GetValue(site, pin));
                }
            }
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_ConfigureStandardWaveformWithScalarData_AllChannelsConfiguredCorrectly(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(PinNames);
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var standardWaveformSettings = new StandardWaveformSettings(
                functionType: StandardWaveform.Sine,
                amplitude: 5.0,
                frequency: 1000.0,
                dcOffset: -1.0,
                startPhase: 90.0);

            sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings);

            var configuredSettings = GetStandardWaveformSettings(sessionsBundle);
            foreach (var site in SiteNumbers)
            {
                foreach (var pin in PinNames)
                {
                    AssertStandardWaveformSettings(standardWaveformSettings, configuredSettings.GetValue(site, pin));
                }
            }
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_ConfigureStandardWaveformWithSiteData_AllChannelsConfiguredCorrectly(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(PinNames);
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var siteData = new SiteData<StandardWaveformSettings>(new[]
            {
                new StandardWaveformSettings(functionType: StandardWaveform.Sine, amplitude: 5.0, frequency: 1000.0),  // Site 0
                new StandardWaveformSettings(functionType: StandardWaveform.Square, amplitude: 3.5, frequency: 5000.0) // Site 1
            });

            sessionsBundle.ConfigureStandardWaveform(siteData);

            var configuredSettings = GetStandardWaveformSettings(sessionsBundle);
            foreach (var site in SiteNumbers)
            {
                foreach (var pin in PinNames)
                {
                    AssertStandardWaveformSettings(siteData.GetValue(site), configuredSettings.GetValue(site, pin));
                }
            }
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_ConfigureStandardWaveformWithPinSiteData_AllChannelsConfiguredCorrectly(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(PinNames);
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var pinSiteData = new PinSiteData<StandardWaveformSettings>(PinNames, SiteNumbers, new[]
            {
                // Pin A: Site 0, Site 1
                new[] { new StandardWaveformSettings(StandardWaveform.Sine, frequency: 1000.0, amplitude: 5.0), new StandardWaveformSettings(StandardWaveform.Sine, frequency: 5000.0, amplitude: 3.5) },
                // Pin B: Site 0, Site 1
                new[] { new StandardWaveformSettings(StandardWaveform.Triangle, frequency: 1000.0, amplitude: 5.0), new StandardWaveformSettings(StandardWaveform.RampUp, frequency: 5000.0, amplitude: 3.5) }
            });

            sessionsBundle.ConfigureStandardWaveform(pinSiteData);

            var configuredSettings = GetStandardWaveformSettings(sessionsBundle);
            foreach (var site in SiteNumbers)
            {
                foreach (var pin in PinNames)
                {
                    AssertStandardWaveformSettings(pinSiteData.GetValue(site, pin), configuredSettings.GetValue(site, pin));
                }
            }
        }

        [Fact]
        public void InitializeBundleWithMultiplePins_ConfigureStandardWaveformWithUnsupportedFunctionType_ThrowsException()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var pinNames = new string[] { "A", "B" };
            var sessionsBundle = sessionManager.Fgen(pinNames);
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var standardWaveformSettings = new StandardWaveformSettings(
                functionType: StandardWaveform.User,
                amplitude: 5.0,
                frequency: 1000.0);

            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings));

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("The 'User' Waveform Function Type is not supported in STL", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        [Fact]
        public void InitializeBundleWithMultiplePins_ConfigureStandardWaveformWithInvalidDCOffset_AllChannelsConfiguredCorrectly()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen(PinNames);
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var standardWaveformSettings = new StandardWaveformSettings(
                functionType: StandardWaveform.Sine,
                amplitude: 5.0,
                frequency: 1000.0,
                dcOffset: 8.0, // Invalid DC offset for the given amplitude.
                startPhase: 90.0);

            var exception = Record.Exception(() =>
            {
                sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings);
                sessionsBundle.Commit();
            });

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("Invalid value for parameter or property", exception.Message); // Ensure correct error message is reported in the exception message.
            Assert.Contains("Property: DC Offset", exception.Message); // Ensure correct property is reported in the exception message.
        }

        [Fact]
        public void InitializeBundleWithMultiplePins_ConfigureStandardWaveformWithInvalidFrequency_AllChannelsConfiguredCorrectly()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen(PinNames);
            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
            var standardWaveformSettings = new StandardWaveformSettings(
                functionType: StandardWaveform.Sine,
                amplitude: -5.0,
                frequency: -1000.0,
                dcOffset: 0.0,
                startPhase: 90); // Invalid start phase for the given waveform.

            var exception = Record.Exception(() =>
            {
                sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings);
                sessionsBundle.Commit();
            });

            Assert.IsType<NISemiconductorTestException>(exception);
            Assert.Contains("Invalid value for parameter or property", exception.Message); // Ensure correct error message is reported in the exception message.
            Assert.Contains("Property: Frequency", exception.Message); // Ensure correct property is reported in the exception message.
        }

        #region Helper Methods

        private static PinSiteData<StandardWaveformSettings> GetStandardWaveformSettings(FgenSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, siepinInfo) =>
            {
                // Get channel name from the individual channel string
                var channelName = GetChannelNameFromIndividualChannelString(siepinInfo.IndividualChannelString);

                // Retrieve the standard waveform settings from the session
                var waveformFunctionType = sessionInfo.Session.StandardWaveform.GetWaveformFunction(channelName);
                var amplitude = sessionInfo.Session.StandardWaveform.GetAmplitude(channelName);
                var dcOffset = sessionInfo.Session.StandardWaveform.GetDCOffset(channelName);
                var frequency = sessionInfo.Session.StandardWaveform.GetFrequency(channelName);
                var startPhase = sessionInfo.Session.StandardWaveform.GetStartPhase(channelName);

                // Create a new StandardWaveformSettings object with the retrieved values and return it.
                return new StandardWaveformSettings(waveformFunctionType, frequency, amplitude, dcOffset, startPhase);
            });
        }

        private static void AssertStandardWaveformSettings(StandardWaveformSettings expected, StandardWaveformSettings actual)
        {
            Assert.Equal(expected.WaveformFunctionType, actual.WaveformFunctionType);
            AssertWithinTolerance(expected.Frequency, actual.Frequency);
            AssertWithinTolerance(expected.Amplitude, actual.Amplitude);
            AssertWithinTolerance(expected.DcOffset, actual.DcOffset);
            AssertWithinTolerance(expected.StartPhase, actual.StartPhase);
        }

        private static void AssertWithinTolerance(double expected, double actual)
        {
            Assert.InRange(actual, expected - Tolerance, expected + Tolerance);
        }

        #endregion
    }
}
