using System;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Fgen
{
    [Collection("NonParallelizable")]
    public sealed class StandardWaveformGenerationTests : IDisposable
    {
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

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.Sine)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.Square)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.Triangle)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.DC)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.RampUp)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.RampDown)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.Noise)]
        public void InitializeBundleWithSinglePin_PerformConfigureStandardWaveformOperation_Succeeds(string pinmap, ModularInstruments.NIFgen.StandardWaveform waveformFunctionType)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");
            var standardWaveformSettings = new StandardWaveformSettings(
                functionType: waveformFunctionType,
                amplitude: 5.0,
                dcOffset: 1.0,
                frequency: 1000.0,
                startPhase: 90.0);

            sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.Sine)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.Square)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.Triangle)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.DC)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.RampUp)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.RampDown)]
        [InlineData("FgenSingleInstrumentPerPin.pinmap", ModularInstruments.NIFgen.StandardWaveform.Noise)]
        [InlineData("FgenSingleInstrumentPerSite.pinmap", ModularInstruments.NIFgen.StandardWaveform.Sine)]
        [InlineData("FgenSingleInstrumentPerSite.pinmap", ModularInstruments.NIFgen.StandardWaveform.Square)]
        [InlineData("FgenSingleInstrumentPerSite.pinmap", ModularInstruments.NIFgen.StandardWaveform.Triangle)]
        [InlineData("FgenSingleInstrumentPerSite.pinmap", ModularInstruments.NIFgen.StandardWaveform.DC)]
        [InlineData("FgenSingleInstrumentPerSite.pinmap", ModularInstruments.NIFgen.StandardWaveform.RampUp)]
        [InlineData("FgenSingleInstrumentPerSite.pinmap", ModularInstruments.NIFgen.StandardWaveform.RampDown)]
        [InlineData("FgenSingleInstrumentPerSite.pinmap", ModularInstruments.NIFgen.StandardWaveform.Noise)]
        public void InitializeBundleWithMultiplePins_PerformConfigureStandardWaveformOperation_Succeeds(string pinmap, ModularInstruments.NIFgen.StandardWaveform waveformFunctionType)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var standardWaveformSettings = new StandardWaveformSettings(
                functionType: waveformFunctionType,
                amplitude: 5.0,
                frequency: 1000.0,
                dcOffset: -1.0,
                startPhase: -90.0);

            sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformConfigureStandardWaveformOperationWithSiteData_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var standardWaveformSettings1 = new StandardWaveformSettings(
                functionType: ModularInstruments.NIFgen.StandardWaveform.Sine,
                amplitude: 5.0,
                frequency: 1000.0);
            var standardWaveformSettings2 = new StandardWaveformSettings(
                functionType: ModularInstruments.NIFgen.StandardWaveform.Square,
                amplitude: 3.5,
                frequency: 5000.0);
            var siteNumbers = new int[] { 0, 1 };
            var siteDataArray = new StandardWaveformSettings[] { standardWaveformSettings1, standardWaveformSettings2 };
            var siteData = new SiteData<StandardWaveformSettings>(siteNumbers, siteDataArray);

            sessionsBundle.ConfigureStandardWaveform(siteData);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformConfigureStandardWaveformOperationWithPinSiteData_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var pinNames = new string[] { "A", "B" };
            var sessionsBundle = sessionManager.Fgen(pinNames);
            var standardWaveformSettings1 = new StandardWaveformSettings(
                functionType: ModularInstruments.NIFgen.StandardWaveform.Sine,
                amplitude: 5.0,
                frequency: 1000.0);
            var standardWaveformSettings2 = new StandardWaveformSettings(
                functionType: ModularInstruments.NIFgen.StandardWaveform.Sine,
                amplitude: 3.5,
                frequency: 5000.0);
            var standardWaveformSettings3 = new StandardWaveformSettings(
                functionType: ModularInstruments.NIFgen.StandardWaveform.Triangle,
                amplitude: 5.0,
                frequency: 1000.0);
            var standardWaveformSettings4 = new StandardWaveformSettings(
                functionType: ModularInstruments.NIFgen.StandardWaveform.RampUp,
                amplitude: 3.5,
                frequency: 5000.0);
            var siteNumbers = new int[] { 0, 1 };
            var perPinPerSiteData = new StandardWaveformSettings[][] { new[] { standardWaveformSettings1, standardWaveformSettings2 }, new[] { standardWaveformSettings3, standardWaveformSettings4 } };
            var pinSiteData = new PinSiteData<StandardWaveformSettings>(pinNames, siteNumbers, perPinPerSiteData);

            sessionsBundle.ConfigureStandardWaveform(pinSiteData);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformConfigureStandardWaveformOperationWithUnsupportedFunctionType_ThrowsException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var pinNames = new string[] { "A", "B" };
            var sessionsBundle = sessionManager.Fgen(pinNames);
            var standardWaveformSettings = new StandardWaveformSettings(
                functionType: ModularInstruments.NIFgen.StandardWaveform.User,
                amplitude: 5.0,
                frequency: 1000.0);

            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureStandardWaveform(standardWaveformSettings));
            Assert.Contains(ResourceStrings.FGen_InvalidFunctionType, exception.Message);
        }
    }
}