using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower.Utilities;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DCPower
{
    [Collection("NonParallelizable")]
    public sealed class ControlTests : IDisposable
    {
        /* Connections defined in pinmap:
         * VCC - Chassis:1 - PXI4110 - Channel:0
         * VCC - Chassis:2 - PXI4130 - Channel:0
         * VCC - Chassis:3 - PXIe4154 - Channel:0
         * VCC - Chassis:4 - PXIe4112 - Channel:0
         * VDD - Chassis:1 - PXIe4147 - Channel:0
         * VDD - Chassis:2 - PXIe4147 - Channel:0
         * VDD - Chassis:3 - PXIe4147 - Channel:0
         * VDD - Chassis:4 - PXIe4147 - Channel:0
         * VDET - Chassis:1 - PXI4110 - Channel:1
         * VDET - Chassis:2 - PXI4130 - Channel:1
         * VDET - Chassis:3 - PXIe4154 - Channel:1
         * VDET - Chassis:4 - PXIe4112 - Channel:1
         */

        private ISemiconductorModuleContext _tsmContext;

        private readonly double _configuredVoltageLevel = 1.5;
        private readonly double _defaultVoltageLevel = 0.0;
        private readonly double _dangedCurrentLimit = 0.02;
        private readonly string _gangedPinGroup = "GangedPins";
        private readonly string[] _resetDevicePins = new string[] { "VCC", "VDD" };
        private readonly string[] _gangedPins = new string[] { "VCC1", "VCC2" };
        private readonly int[] _sitesOtherThanZero = new int[] { 1, 2, 3 };

        public TSMSessionManager Initialize(bool pinMapWithChannelGroup)
        {
            string pinMapFileName = pinMapWithChannelGroup ? "DifferentSMUDevicesWithChannelGroup.pinmap" : "DifferentSMUDevices.pinmap";
            return Initialize(pinMapFileName);
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
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap", DCPowerMeasurementWhen.OnDemand)]
        [InlineData("Mixed Signal Tests.pinmap", DCPowerMeasurementWhen.OnMeasureTrigger)]
        [InlineData("Mixed Signal Tests.pinmap", DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete)]
        [InlineData("Mixed Signal Tests Common Session.pinmap", DCPowerMeasurementWhen.OnDemand)]
        [InlineData("Mixed Signal Tests Common Session.pinmap", DCPowerMeasurementWhen.OnMeasureTrigger)]
        [InlineData("Mixed Signal Tests Common Session.pinmap", DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete)]
        public void ConfigureMeasureWhen_Initiate_DataMeasurementDoesNotTimeOut(string pinmap, DCPowerMeasurementWhen measureWhen)
        {
            var sessionManager = Initialize(pinmap);
            var dcPower = sessionManager.DCPower(new[] { "PowerPins" });
            dcPower.ConfigureMeasureWhen(measureWhen);
            if (measureWhen == DCPowerMeasurementWhen.OnMeasureTrigger)
            {
                dcPower.ConfigureTriggerSoftwareEdge(TriggerType.MeasureTrigger);
            }

            dcPower.Initiate();

            dcPower.MeasureVoltage();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureAdvanceSequenceAsInactive_InitiateAdvancedSequences_AdvanceSequenceActivated(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName = "TestSequence";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: false);

            sessionsBundle.InitiateAdvancedSequence(sequenceName);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(sequenceName, sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence);
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureMultipleAdvanceSequenceAsInactive_InitiateSingleAdvancedSequenceOutofMultipleConfiguredAdvanceSequence_AdvanceSequenceActivated(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName1 = "Sequence1";
            string sequenceName2 = "Sequence2";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            sessionsBundle.ConfigureAdvancedSequence(sequenceName1, stepProperties, setAsActiveSequence: false);
            sessionsBundle.ConfigureAdvancedSequence(sequenceName2, stepProperties, setAsActiveSequence: false);

            sessionsBundle.InitiateAdvancedSequence(sequenceName1);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(sequenceName1, sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence);
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureAdvanceSequence_InitiateClearedAdvancedSequence_AdvanceSequenceActivated(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName = "Sequence";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true);

            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.InitiateAdvancedSequence(sequenceName);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(sequenceName, sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence);
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void ConfigureAdvanceSequence_ForceVoltageAndInitiateAdvancedSequence_AdvanceSequenceActivated(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName = "Sequence";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.5, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: false);

            sessionsBundle.ForceVoltage(1.5);
            sessionsBundle.InitiateAdvancedSequence(sequenceName, waitForSequenceCompletion: true, sequenceTimeoutInSeconds: 10);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(sequenceName, sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence);
            });
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap", DCPowerMeasurementWhen.OnDemand)]
        [InlineData("Mixed Signal Tests.pinmap", DCPowerMeasurementWhen.OnMeasureTrigger)]
        [InlineData("Mixed Signal Tests.pinmap", DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete)]
        [InlineData("Mixed Signal Tests Common Session.pinmap", DCPowerMeasurementWhen.OnDemand)]
        [InlineData("Mixed Signal Tests Common Session.pinmap", DCPowerMeasurementWhen.OnMeasureTrigger)]
        [InlineData("Mixed Signal Tests Common Session.pinmap", DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete)]
        public void GangPinGroupAndConfigureMeasureSettings_Initiate_DataMeasurementDoesNotTimeOut(string pinmap, DCPowerMeasurementWhen measureWhen)
        {
            var sessionManager = Initialize(pinmap);
            var dcPower = sessionManager.DCPower(new[] { "PowerPins" });
            dcPower.GangPinGroup("MergedPowerPins");
            var dcpowerMeasureSettings = new DCPowerMeasureSettings() { MeasureWhen = measureWhen };
            dcPower.ConfigureMeasureSettings(dcpowerMeasureSettings);
            if (measureWhen == DCPowerMeasurementWhen.OnMeasureTrigger)
            {
                dcPower.ConfigureTriggerSoftwareEdge(TriggerType.MeasureTrigger);
            }

            dcPower.Initiate();

            dcPower.MeasureVoltage();
            dcPower.UngangPinGroup("MergedPowerPins");
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("DCPowerSessionPerChannel.pinmap")]
        [InlineData("DCPowerSessionPerInstrument.pinmap")]
        [InlineData("DCPowerSharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerSharedSessionsAcrossInstruments.pinmap")]
        [InlineData("DCPowerCommonSession.pinmap")]
        public void BundleWithAllSites_ResetChannelZero_ResetsChannelZeroAndKeepsOtherChannelsConfigured(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            var allSitesBundle = sessionManager.DCPower(_resetDevicePins);
            allSitesBundle.ConfigureVoltageLevel(_configuredVoltageLevel, UpdateMode.Commit);
            var sessionsBeforeReset = GetSessions(allSitesBundle);

            allSitesBundle.Reset(GetChannelString(allSitesBundle.FilterBySite(0)));

            AssertVoltageLevels(allSitesBundle.FilterBySite(0), _defaultVoltageLevel);
            AssertVoltageLevels(allSitesBundle.FilterBySite(_sitesOtherThanZero), _configuredVoltageLevel);
            AssertSameSessions(sessionsBeforeReset, GetSessions(allSitesBundle));
            AssertSessionsAreUsable(allSitesBundle);
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("DCPowerSessionPerChannel.pinmap")]
        [InlineData("DCPowerSharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerCommonSession.pinmap")]
        public void BundleWithSiteZero_ResetChannelWithVCCPin_ResetsChannelsWithVCCAndKeepsOtherchannelsConfigured(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(_configuredVoltageLevel, UpdateMode.Commit);
            var siteZeroBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);

            siteZeroBundle.Reset(GetChannelString(siteZeroBundle.FilterByPin("VCC")));

            AssertVoltageLevels(siteZeroBundle.FilterByPin("VCC"), _defaultVoltageLevel);
            AssertVoltageLevels(siteZeroBundle.FilterByPin("VDD"), _configuredVoltageLevel);
            AssertVoltageLevels(sessionManager.DCPower(_resetDevicePins).FilterBySite(_sitesOtherThanZero), _configuredVoltageLevel);
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(" , ")]
        public void BundleWithSiteZero_ResetNullOrEmptyChannelString_ThrowsException(string channelString)
        {
            var sessionManager = Initialize("DCPowerSessionPerChannel.pinmap");
            var siteZeroBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);

            void Reset() => siteZeroBundle.Reset(channelString);

            var exception = Assert.Throws<NISemiconductorTestException>(Reset);
            Assert.Contains("channelString", exception.Message);
            Assert.Contains("cannot be null or empty", exception.Message);
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void BundleWithSiteZero_ResetChannelOfOtherSite_ThrowsExceptionAndResetsNoChannel(bool includeSiteZeroChannel)
        {
            var sessionManager = Initialize("DCPowerSessionPerChannel.pinmap");
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(_configuredVoltageLevel, UpdateMode.Commit);
            var siteZeroBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);
            var otherSiteChannelString = GetChannelString(sessionManager.DCPower("VCC").FilterBySite(1));
            var channelString = includeSiteZeroChannel ? $"{GetChannelString(siteZeroBundle.FilterByPin("VCC"))}, {otherSiteChannelString}" : otherSiteChannelString;

            void Reset() => siteZeroBundle.Reset(channelString);

            var exception = Assert.Throws<NISemiconductorTestException>(Reset);
            Assert.Contains("are not part of the sessions bundle", exception.Message);
            AssertVoltageLevels(sessionManager.DCPower(_resetDevicePins), _configuredVoltageLevel);
        }

        [Fact]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void BundleWithSiteZero_ResetChannelOfUnknownInstrument_ThrowsExceptionAndResetsNoChannel()
        {
            var sessionManager = Initialize("DCPowerSessionPerChannel.pinmap");
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(_configuredVoltageLevel, UpdateMode.Commit);
            var siteZeroBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);

            void Reset() => siteZeroBundle.Reset("UnknownInstrument/0");

            var exception = Assert.Throws<NISemiconductorTestException>(Reset);
            Assert.Contains("are not part of the sessions bundle", exception.Message);
            AssertVoltageLevels(sessionManager.DCPower(_resetDevicePins), _configuredVoltageLevel);
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("DCPowerGanged_SessionPerChannel.pinmap")]
        [InlineData("DCPowerGanged_SharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerGanged_AcrossInstruments.pinmap")]
        [InlineData("DCPowerGanged_SessionsSpanningInstruments.pinmap")]
        [InlineData("DCPowerGanged_CommonSession.pinmap")]
        public void BundleWithGangedPinGroup_ResetGangedChannels_ResetsGangedChannelsAndKeepsGanging(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            var gangedBundle = sessionManager.DCPower(_gangedPins);
            gangedBundle.GangPinGroup(_gangedPinGroup);
            gangedBundle.ForceVoltage(voltageLevel: _configuredVoltageLevel, currentLimit: _dangedCurrentLimit);

            gangedBundle.Reset(GetChannelString(gangedBundle));

            AssertVoltageLevels(gangedBundle, _defaultVoltageLevel);
            Assert.True(gangedBundle.HasGangedChannels);
            gangedBundle.ForceVoltage(voltageLevel: _configuredVoltageLevel, currentLimit: _dangedCurrentLimit);
            gangedBundle.UngangPinGroup(_gangedPinGroup);
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("DCPowerSessionPerChannel.pinmap", false)]
        [InlineData("DCPowerSessionPerChannel.pinmap", true)]
        [InlineData("DCPowerSessionPerInstrument.pinmap", false)]
        [InlineData("DCPowerSessionPerInstrument.pinmap", true)]
        [InlineData("DCPowerSharedSessionsWithinInstrument.pinmap", false)]
        [InlineData("DCPowerSharedSessionsWithinInstrument.pinmap", true)]
        [InlineData("DCPowerSharedSessionsAcrossInstruments.pinmap", false)]
        [InlineData("DCPowerSharedSessionsAcrossInstruments.pinmap", true)]
        [InlineData("DCPowerCommonSession.pinmap", false)]
        [InlineData("DCPowerCommonSession.pinmap", true)]
        public void BundlesWithOverlappingChannels_ResetDeviceWithSiteZeroBundle_ResetsSiteZeroChannelsAndReinitializesOtherChannels(string pinMap, bool restoreConfiguration)
        {
            var sessionManager = Initialize(pinMap);
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(_configuredVoltageLevel, UpdateMode.Commit);
            var bundleA = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);
            var bundleB = sessionManager.DCPower(_resetDevicePins).FilterBySite(1);
            var bundleC = sessionManager.DCPower(_resetDevicePins).FilterBySite(_sitesOtherThanZero);
            var bundleD = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);
            var expectedOtherChannelsVoltageLevel = restoreConfiguration ? _configuredVoltageLevel : _defaultVoltageLevel;

            bundleA.ResetDevice(restoreConfiguration);

            AssertVoltageLevels(bundleA, _defaultVoltageLevel);
            AssertVoltageLevels(bundleD, _defaultVoltageLevel);
            AssertVoltageLevels(bundleB, expectedOtherChannelsVoltageLevel);
            AssertVoltageLevels(bundleC, expectedOtherChannelsVoltageLevel);
            AssertSessionsAreUsable(bundleA, bundleB, bundleC, bundleD);
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("DCPowerSessionPerChannel.pinmap")]
        [InlineData("DCPowerSessionPerInstrument.pinmap")]
        [InlineData("DCPowerSharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerSharedSessionsAcrossInstruments.pinmap")]
        [InlineData("DCPowerCommonSession.pinmap")]
        public void BundleWithSiteZero_ResetDevice_DoesNotReinitializeSessionsOfBundle(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            var bundleA = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);
            var sessionsBeforeReset = GetSessions(bundleA);

            bundleA.ResetDevice();

            AssertSameSessions(sessionsBeforeReset, GetSessions(bundleA));
        }

        [Fact]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void InitializeSessionPerChannel_ResetDeviceChannelsInSiteZero_ReinitializesSessionsOfOtherChannels()
        {
            var sessionManager = Initialize("DCPowerSessionPerChannel.pinmap");
            var bundleA = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);
            var otherSitesBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(_sitesOtherThanZero);
            var otherSitesSessionsBeforeReset = GetSessions(otherSitesBundle);

            bundleA.ResetDevice();

            var otherSitesSessionsAfterReset = GetSessions(otherSitesBundle);
            Assert.Equal(otherSitesSessionsBeforeReset.Count, otherSitesSessionsAfterReset.Count);
            Assert.All(otherSitesSessionsAfterReset, session => Assert.DoesNotContain(otherSitesSessionsBeforeReset, sessionBeforeReset => ReferenceEquals(sessionBeforeReset, session)));
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void InitializeSessions_ResetDeviceInBundleWithVCCPin_ResetsChannelsWithVCCPinsAndKeepsOtherChannelsSame(bool restoreConfiguration)
        {
            var sessionManager = Initialize("DCPowerSessionPerChannel.pinmap");
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(_configuredVoltageLevel, UpdateMode.Commit);
            var vccBundle = sessionManager.DCPower(_resetDevicePins).FilterByPin("VCC");
            var vddBundle = sessionManager.DCPower(_resetDevicePins).FilterByPin("VDD");
            var vddSessionsBeforeReset = GetSessions(vddBundle);

            vccBundle.ResetDevice(restoreConfiguration);

            AssertVoltageLevels(vccBundle, _defaultVoltageLevel);
            AssertVoltageLevels(vddBundle, _configuredVoltageLevel);
            AssertSameSessions(vddSessionsBeforeReset, GetSessions(vddBundle));
            AssertSessionsAreUsable(vccBundle, vddBundle);
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("DCPowerSessionPerChannel.pinmap", false)]
        [InlineData("DCPowerSessionPerChannel.pinmap", true)]
        [InlineData("DCPowerSharedSessionsWithinInstrument.pinmap", false)]
        [InlineData("DCPowerSharedSessionsWithinInstrument.pinmap", true)]
        [InlineData("DCPowerCommonSession.pinmap", false)]
        [InlineData("DCPowerCommonSession.pinmap", true)]
        public void OtherSitesDisabled_ResetDeviceWithSiteZeroBundle_ResetsSiteZeroChannelsAndReinitializesChannelsOfDisabledSites(string pinMap, bool restoreConfiguration)
        {
            var sessionManager = Initialize(pinMap);
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(_configuredVoltageLevel, UpdateMode.Commit);
            var siteZeroSessionManager = new TSMSessionManager(_tsmContext.GetSemiconductorModuleContextWithSites(new[] { 0 }));
            var bundleA = siteZeroSessionManager.DCPower(_resetDevicePins);

            bundleA.ResetDevice(restoreConfiguration);

            var disabledSitesBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(_sitesOtherThanZero);
            AssertVoltageLevels(bundleA, _defaultVoltageLevel);
            AssertVoltageLevels(disabledSitesBundle, restoreConfiguration ? _configuredVoltageLevel : _defaultVoltageLevel);
            AssertSessionsAreUsable(bundleA, disabledSitesBundle);
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("DCPowerSessionPerChannel.pinmap")]
        [InlineData("DCPowerSessionPerInstrument.pinmap")]
        [InlineData("DCPowerSharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerSharedSessionsAcrossInstruments.pinmap")]
        [InlineData("DCPowerCommonSession.pinmap")]
        public void BundlePerSiteOnParallelThreads_ResetDevice_ResetsAllChannelsAndKeepsSessionsUsable(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(_configuredVoltageLevel, UpdateMode.Commit);

            Parallel.ForEach(_tsmContext.SiteNumbers, siteNumber =>
            {
                var siteSessionManager = new TSMSessionManager(_tsmContext.GetSemiconductorModuleContextWithSites(new[] { siteNumber }));
                siteSessionManager.DCPower(_resetDevicePins).ResetDevice();
            });

            var allSitesBundle = sessionManager.DCPower(_resetDevicePins);
            AssertVoltageLevels(allSitesBundle, _defaultVoltageLevel);
            AssertSessionsAreUsable(allSitesBundle);
        }

        [Theory]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("DCPowerGanged_SessionPerChannel.pinmap")]
        [InlineData("DCPowerGanged_SharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerGanged_AcrossInstruments.pinmap")]
        [InlineData("DCPowerGanged_SessionsSpanningInstruments.pinmap")]
        [InlineData("DCPowerGanged_CommonSession.pinmap")]
        public void BundleWithSiteOneGangedPinGroup_ResetDevice_ResetsSiteOneAndRestoresSiteZeroGangedChannels(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            var gangedBundle = sessionManager.DCPower(_gangedPins);
            gangedBundle.GangPinGroup(_gangedPinGroup);
            gangedBundle.ForceVoltage(voltageLevel: _configuredVoltageLevel, currentLimit: _dangedCurrentLimit);
            var siteZeroBundle = gangedBundle.FilterBySite(0);
            var siteOneBundle = gangedBundle.FilterBySite(1);
            var siteZeroVoltageLevelsBeforeReset = GetVoltageLevelPerChannel(siteZeroBundle);

            siteOneBundle.ResetDevice();

            var siteZeroVoltageLevelsAfterReset = GetVoltageLevelPerChannel(siteZeroBundle);
            Assert.Equal(siteZeroVoltageLevelsBeforeReset.Count, siteZeroVoltageLevelsAfterReset.Count);
            foreach (var channelVoltageLevel in siteZeroVoltageLevelsBeforeReset)
            {
                Assert.Equal(channelVoltageLevel.Value, siteZeroVoltageLevelsAfterReset[channelVoltageLevel.Key], 3);
            }
            AssertVoltageLevels(siteOneBundle, _defaultVoltageLevel);
            Assert.True(gangedBundle.HasGangedChannels);
            gangedBundle.ForceVoltage(voltageLevel: _configuredVoltageLevel, currentLimit: _dangedCurrentLimit);
            gangedBundle.UngangPinGroup(_gangedPinGroup);
        }

        [Fact]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void BundleWithSessionsNotInitializedByInitializeAndClose_ResetDevice_ThrowsException()
        {
            _tsmContext = CreateTSMContext("DCPowerSessionPerChannel.pinmap");
            foreach (var resourceString in _tsmContext.GetNIDCPowerResourceStrings())
            {
                _tsmContext.SetNIDCPowerSession(resourceString, new NIDCPower(resourceString, false, string.Empty));
            }
            DCPowerSessionInformation.GenerateInstrumentChannelToSitePinDictionary(_tsmContext);
            var sessionsBundle = new TSMSessionManager(_tsmContext).DCPower(_resetDevicePins).FilterBySite(0);

            void ResetDevice() => sessionsBundle.ResetDevice();

            var exception = Assert.Throws<NISemiconductorTestException>(ResetDevice);
            Assert.Contains("InitializeAndClose.Initialize", exception.Message);
        }

        private static IList<NIDCPower> GetSessions(DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.InstrumentSessions.Select(sessionInfo => sessionInfo.Session).ToList();
        }

        private static void AssertSameSessions(IList<NIDCPower> expectedSessions, IList<NIDCPower> actualSessions)
        {
            Assert.Equal(expectedSessions.Count, actualSessions.Count);
            for (int i = 0; i < expectedSessions.Count; i++)
            {
                Assert.Same(expectedSessions[i], actualSessions[i]);
            }
        }

        private static void AssertVoltageLevels(DCPowerSessionsBundle sessionsBundle, double expectedVoltageLevel)
        {
            var voltageLevelPerChannel = GetVoltageLevelPerChannel(sessionsBundle);
            Assert.NotEmpty(voltageLevelPerChannel);
            Assert.All(voltageLevelPerChannel.Values, voltageLevel => Assert.Equal(expectedVoltageLevel, voltageLevel, 3));
        }

        // Reads the driver property per channel, so ganged channels are checked individually instead of per pin group.
        private static IDictionary<string, double> GetVoltageLevelPerChannel(DCPowerSessionsBundle sessionsBundle)
        {
            var voltageLevelPerChannel = new Dictionary<string, double>();
            foreach (var sessionInfo in sessionsBundle.InstrumentSessions)
            {
                foreach (var sitePinInfo in sessionInfo.AssociatedSitePinList)
                {
                    var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    voltageLevelPerChannel[sitePinInfo.IndividualChannelString] = channelOutput.Source.Voltage.VoltageLevel;
                }
            }
            return voltageLevelPerChannel;
        }

        private static string GetChannelString(DCPowerSessionsBundle sessionsBundle)
        {
            return string.Join(",", sessionsBundle.AggregateSitePinList.Select(sitePinInfo => sitePinInfo.IndividualChannelString));
        }

        // A session that was invalidated by a device reset fails on the first hardware call, which is the measurement.
        private static void AssertSessionsAreUsable(params DCPowerSessionsBundle[] sessionsBundles)
        {
            foreach (var sessionsBundle in sessionsBundles)
            {
                sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.OnDemand);
                sessionsBundle.Initiate();
                var voltageMeasurements = sessionsBundle.MeasureVoltage();
                sessionsBundle.Abort();
                var siteNumbers = sessionsBundle.AggregateSitePinList.Select(sitePinInfo => sitePinInfo.SiteNumber);
                var expectedSiteNumbers = siteNumbers.Distinct().OrderBy(siteNumber => siteNumber);
                Assert.Equal(expectedSiteNumbers, voltageMeasurements.SiteNumbers);
            }
        }
    }
}
