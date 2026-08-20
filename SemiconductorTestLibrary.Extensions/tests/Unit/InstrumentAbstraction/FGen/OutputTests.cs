using System;
using System.Globalization;
using System.Linq;
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

        #region ConfigureOutputEnabled tests

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundleWithSinglePin_PerformConfigureOutputEnabledOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ConfigureOutputEnabled(false);

            AssertOutputEnabledState(sessionsBundle, false);
            sessionsBundle.ConfigureOutputEnabled(true);
            AssertOutputEnabledState(sessionsBundle, true);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentSharedAcrossPinsAndSites.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformConfigureOutputEnabledOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });

            sessionsBundle.ConfigureOutputEnabled(false);

            AssertOutputEnabledState(sessionsBundle, false);
            sessionsBundle.ConfigureOutputEnabled(true);
            AssertOutputEnabledState(sessionsBundle, true);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentSharedAcrossPinsAndSites.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformConfigureOutputEnabledOperationWithSiteData_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var pinNames = new string[] { "A", "B" };
            var sessionsBundle = sessionManager.Fgen(pinNames);
            var siteNumbers = new int[] { 0, 1 };
            var siteDataArray = new bool[] { true, false };
            var siteData = new SiteData<bool>(siteNumbers, siteDataArray);

            sessionsBundle.ConfigureOutputEnabled(siteData);

            var arraySiteData = new SiteData<bool>[] { siteData, siteData };
            var pinSiteData = new PinSiteData<bool>(pinNames, arraySiteData);
            AssertOutputEnabledState(sessionsBundle, pinSiteData);
        }
        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentSharedAcrossPinsAndSites.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformConfigureOutputEnabledOperationWithPinSiteData_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var pinNames = new string[] { "A", "B" };
            var siteNumbers = new int[] { 0, 1 };
            var perPinPerSiteData = new bool[][] { new bool[] { true, false }, new bool[] { false, true } };
            var pinSiteData = new PinSiteData<bool>(pinNames, siteNumbers, perPinPerSiteData);

            sessionsBundle.ConfigureOutputEnabled(pinSiteData);

            AssertOutputEnabledState(sessionsBundle, pinSiteData);
        }

        #endregion

        #region ConfigureOutputImpedance tests

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundleWithSinglePin_PerformConfigureOutputImpedanceOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ConfigureOutputImpedance(50);

            AssertOutputImpedance(sessionsBundle, 50);
            sessionsBundle.ConfigureOutputImpedance();
            AssertOutputImpedance(sessionsBundle, 50);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentSharedAcrossPinsAndSites.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformConfigureOutputImpedanceOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });

            sessionsBundle.ConfigureOutputImpedance(50);

            AssertOutputImpedance(sessionsBundle, 50);
            sessionsBundle.ConfigureOutputImpedance();
            AssertOutputImpedance(sessionsBundle, 50);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentSharedAcrossPinsAndSites.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformConfigureOutputImpedanceOperationWithSiteData_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
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

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentSharedAcrossPinsAndSites.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformConfigureOutputImpedanceOperationWithPinSiteData_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });
            var pinNames = new string[] { "A", "B" };
            var siteNumbers = new int[] { 0, 1 };
            var perPinPerSiteData = new double[][] { new double[] { 50, 50 }, new double[] { 50, 50 } };
            var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers, perPinPerSiteData);

            sessionsBundle.ConfigureOutputImpedance(pinSiteData);

            AssertOutputImpedance(sessionsBundle, pinSiteData);
        }

        #endregion

        #region ConfigureOutputMode tests

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundleWithSinglePin_PerformConfigureOutputModeOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ConfigureOutputMode(OutputMode.Function);

            AssertOutputMode(sessionsBundle, OutputMode.Function);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentSharedAcrossPinsAndSites.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformConfigureOutputModeOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });

            sessionsBundle.ConfigureOutputMode(OutputMode.Function);

            AssertOutputMode(sessionsBundle, OutputMode.Function);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        public void InitializeBundleWithSinglePin_PerformConfigureUnsupportedOutputModeThrowsException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");

            foreach (var outputMode in Enum.GetValues(typeof(OutputMode)))
            {
                if ((OutputMode)outputMode != OutputMode.Function)
                {
                    var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureOutputMode((OutputMode)outputMode));
                    Assert.Contains(string.Format(CultureInfo.InvariantCulture, ResourceStrings.FGen_InvalidOutputModeException, outputMode), exception.Message);
                }
            }
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentPerPinPerSite.pinmap")]
        [InlineData("FgenSingleInstrumentSharedAcrossPinsAndSites.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformConfigureUnsupportedOutputModeThrowsException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });

            foreach (var outputMode in Enum.GetValues(typeof(OutputMode)))
            {
                if ((OutputMode)outputMode != OutputMode.Function)
                {
                    var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureOutputMode((OutputMode)outputMode));
                    Assert.Contains(string.Format(CultureInfo.InvariantCulture, ResourceStrings.FGen_InvalidOutputModeException, outputMode), exception.Message);
                }
            }
        }
        #endregion

        #region HelperMethods
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
                Assert.Equal(expectedValue.GetValue(sitePinInfo), actualValue);
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
                Assert.Equal(expectedValue.GetValue(sitePinInfo), actualValue);
            });
        }

        private void AssertOutputMode(FgenSessionsBundle sessionsBundle, OutputMode expectedValue)
        {
            sessionsBundle.Do(sessionInformation =>
            {
                var actualValue = sessionInformation.Session.Output.OutputMode;
                Assert.Equal(expectedValue, actualValue);
            });
        }
        #endregion
    }
}
