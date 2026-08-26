using System;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Fgen
{
    [Collection("NonParallelizable")]
    public sealed class UtilitiesTests : IDisposable
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
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundleWithSinglePinAndSetNonDefaultProperties_Reset_PropertiesRestoredToDefault(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");
            var outputModeDefault = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);
            sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode = OutputMode.Sequence);
            var outputModeNonDefault = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);

            sessionsBundle.Reset();

            var outputModeDefaultAfterReset = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);
            Assert.NotEqual(outputModeNonDefault, outputModeDefault);
            Assert.Equal(outputModeDefaultAfterReset, outputModeDefault);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundleWithSinglePinAndSetNonDefaultProperties_ResetDevice_PropertiesRestoredToDefault(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");
            var outputModeDefault = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);
            sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode = OutputMode.Sequence);
            var outputModeNonDefault = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);

            sessionsBundle.ResetDevice();

            var outputModeDefaultAfterResetDevice = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);
            Assert.NotEqual(outputModeNonDefault, outputModeDefault);
            Assert.Equal(outputModeDefaultAfterResetDevice, outputModeDefault);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentSharedAcrossPinsAndSites.pinmap")]
        public void InitializeBundleWithMultiplePinsAndSetNonDefaultProperties_Reset_PropertiesRestoredToDefault(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B", "SystemPin" });
            var outputModeDefault = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);
            sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode = OutputMode.Sequence);
            var outputModeNonDefault = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);

            sessionsBundle.Reset();

            var outputModeDefaultAfterReset = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);
            Assert.NotEqual(outputModeNonDefault, outputModeDefault);
            Assert.Equal(outputModeDefaultAfterReset, outputModeDefault);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentSharedAcrossPinsAndSites.pinmap")]
        public void InitializeBundleWithMultiplePinsAndSetNonDefaultProperties_ResetDevice_PropertiesRestoredToDefault(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B", "SystemPin" });
            var outputModeDefault = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);
            sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode = OutputMode.Sequence);
            var outputModeNonDefault = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);

            sessionsBundle.ResetDevice();

            var outputModeDefaultAfterResetDevice = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Output.OutputMode);
            Assert.NotEqual(outputModeNonDefault, outputModeDefault);
            Assert.Equal(outputModeDefaultAfterResetDevice, outputModeDefault);
        }
    }
}