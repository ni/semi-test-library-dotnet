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

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Fgen
{
    [Collection("NonParallelizable")]
    public sealed class OutputTests : IDisposable
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

        [Fact]
        public void InitializeBundleWithSinglePin_PerformConfigureOutputEnabledOperation_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ConfigureOutputEnabled(false);
            AssertOutputEnabledState(sessionsBundle, false);
            sessionsBundle.ConfigureOutputEnabled(true);
            AssertOutputEnabledState(sessionsBundle, true);
        }

        [Fact]
        public void InitializeBundleWithMultiplePin_PerformConfigureOutputEnabledOperation_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });

            sessionsBundle.ConfigureOutputEnabled(false);
            AssertOutputEnabledState(sessionsBundle, false);
            sessionsBundle.ConfigureOutputEnabled(true);
            AssertOutputEnabledState(sessionsBundle, true);
        }

        [Fact]
        public void InitializeBundleWithMultiplePin_PerformConfigureOutputEnabledOperationWithSiteData_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var pinNames = new string[] { "A", "B" };
            var siteNumbers = new int[] { 0, 1 };
            var siteDataArray = new bool[] { true, false };
            var siteData = new SiteData<bool>(siteNumbers, siteDataArray);

            sessionsBundle.ConfigureOutputEnabled(siteData);

            var arraySiteData = new SiteData<bool>[] { siteData, siteData };
            var pinSiteData = new PinSiteData<bool>(pinNames, arraySiteData);
            AssertOutputEnabledState(sessionsBundle, pinSiteData);
        }

        [Fact]
        public void InitializeBundleWithMultiplePin_PerformConfigureOutputEnabledOperationWithPinSiteData_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var pinNames = new string[] { "A", "B" };
            var siteNumbers = new int[] { 0, 1 };
            var perPinPerSiteData = new bool[][] { new bool[] { true, false }, new bool[] { false, true } };
            var pinSiteData = new PinSiteData<bool>(pinNames, siteNumbers, perPinPerSiteData);

            sessionsBundle.ConfigureOutputEnabled(pinSiteData);

            AssertOutputEnabledState(sessionsBundle, pinSiteData);
        }

        [Fact]
        public void InitializeBundleWithSinglePin_PerformConfigureOutputImpedanceOperation_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ConfigureOutputImpedance(50);
            AssertOutputImpedance(sessionsBundle, 50);
            sessionsBundle.ConfigureOutputImpedance();
            AssertOutputImpedance(sessionsBundle, 50);
        }

        [Fact]
        public void InitializeBundleWithMultiplePin_PerformConfigureOutputImpedanceOperation_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });

            sessionsBundle.ConfigureOutputImpedance(50);
            AssertOutputImpedance(sessionsBundle, 50);
            sessionsBundle.ConfigureOutputImpedance();
            AssertOutputImpedance(sessionsBundle, 50);
        }

        [Fact]
        public void InitializeBundleWithMultiplePin_PerformConfigureOutputImpedanceOperationWithSiteData_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var pinNames = new string[] { "A", "B" };
            var siteNumbers = new int[] { 0, 1 };
            var siteDataArray = new double[] { 50, 50 };
            var siteData = new SiteData<double>(siteNumbers, siteDataArray);

            sessionsBundle.ConfigureOutputImpedance(siteData);

            var arraySiteData = new SiteData<double>[] { siteData, siteData };
            var pinSiteData = new PinSiteData<double>(pinNames, arraySiteData);
            AssertOutputImpedance(sessionsBundle, pinSiteData);
        }

        [Fact]
        public void InitializeBundleWithMultiplePin_PerformConfigureOutputImpedanceOperationWithPinSiteData_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var pinNames = new string[] { "A", "B" };
            var siteNumbers = new int[] { 0, 1 };
            var perPinPerSiteData = new double[][] { new double[] { 50, 50 }, new double[] { 50, 50 } };
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers, perPinPerSiteData);

            sessionsBundle.ConfigureOutputImpedance(pinSiteData);

            AssertOutputImpedance(sessionsBundle, pinSiteData);
        }

        [Fact]
        public void InitializeBundleWithSinglePin_PerformConfigureOutputModeOperation_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
        }

        [Fact]
        public void InitializeBundleWithMultiplePin_PerformConfigureOutputModeOperation_Succeeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });

            sessionsBundle.ConfigureOutputMode(OutputMode.Function);
        }

        [Fact]
        public void InitializeBundleWithSinglePin_PerformConfigureUnsupportedOutputMode_ThrowsException()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen("A");

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureOutputMode(OutputMode.Arbitrary));
            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureOutputMode(OutputMode.FrequencyList));
            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureOutputMode(OutputMode.Sequence));
            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureOutputMode(OutputMode.Script));
        }

        [Fact]
        public void InitializeBundleWithMultiplePin_PerformConfigureUnsupportedOutputMode_ThrowsException()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureOutputMode(OutputMode.Arbitrary));
            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureOutputMode(OutputMode.FrequencyList));
            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureOutputMode(OutputMode.Sequence));
            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureOutputMode(OutputMode.Script));
        }

        // Helper methods to assert the output enabled state and impedance for the sessions bundle
        private void AssertOutputEnabledState(FgenSessionsBundle sessionsBundle, bool expectedValue)
        {
            sessionsBundle.Do((sessionInformation, sitePinInfo) =>
            {
                var actualValue = sessionInformation.Session.Output.GetEnabled(sitePinInfo.IndividualChannelString.Split('/').Last());
                Assert.Equal(expectedValue, actualValue);
            });
        }

        private void AssertOutputEnabledState(FgenSessionsBundle sessionsBundle, PinSiteData<bool> expectedValue)
        {
            sessionsBundle.Do((sessionInformation, sitePinInfo) =>
            {
                var actualValue = sessionInformation.Session.Output.GetEnabled(sitePinInfo.IndividualChannelString.Split('/').Last());
                Assert.Equal(expectedValue.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName), actualValue);
            });
        }

        private void AssertOutputImpedance(FgenSessionsBundle sessionsBundle, double expectedValue)
        {
            sessionsBundle.Do((sessionInformation, sitePinInfo) =>
            {
                var actualValue = sessionInformation.Session.Output.GetImpedance(sitePinInfo.IndividualChannelString.Split('/').Last());
                Assert.Equal(expectedValue, actualValue);
            });
        }

        private void AssertOutputImpedance(FgenSessionsBundle sessionsBundle, PinSiteData<double> expectedValue)
        {
            sessionsBundle.Do((sessionInformation, sitePinInfo) =>
            {
                var actualValue = sessionInformation.Session.Output.GetImpedance(sitePinInfo.IndividualChannelString.Split('/').Last());
                Assert.Equal(expectedValue.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName), actualValue);
            });
        }
    }
}
