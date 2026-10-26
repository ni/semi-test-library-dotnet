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

        private const double ConfiguredVoltageLevel = 1.5;
        private const double DefaultVoltageLevel = 0.0;
        private const double GangedCurrentLimit = 0.02;
        private const string GangedPinGroup = "GangedPins";
        private const string SiteZeroVccChannel = "SMU_4147_C1_S11/0";
        private const string SiteZeroChannels = "SMU_4147_C1_S11/0, SMU_4147_C2_S10/0";
        private static readonly string[] _resetDevicePins = new string[] { "VCC", "VDD" };
        private static readonly string[] _gangedPins = new string[] { "VCC1", "VCC2" };
        private static readonly int[] _sitesOtherThanZero = new int[] { 1, 2, 3 };

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
        [InlineData("DCPowerResetDevice_SessionPerChannel.pinmap")]
        [InlineData("DCPowerResetDevice_SessionPerInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_SharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_SharedSessionsAcrossInstruments.pinmap")]
        [InlineData("DCPowerResetDevice_CommonSession.pinmap")]
        public void AllSitesBundle_ResetSiteZeroChannels_OnlySiteZeroChannelsResetAndSessionsKept(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            var allSitesBundle = sessionManager.DCPower(_resetDevicePins);
            allSitesBundle.ConfigureVoltageLevel(ConfiguredVoltageLevel, UpdateMode.Commit);
            var sessionsBeforeReset = GetSessions(allSitesBundle);

            allSitesBundle.Reset(SiteZeroChannels);

            AssertVoltageLevels(allSitesBundle.FilterBySite(0), DefaultVoltageLevel);
            AssertVoltageLevels(allSitesBundle.FilterBySite(_sitesOtherThanZero), ConfiguredVoltageLevel);
            AssertSameSessions(sessionsBeforeReset, GetSessions(allSitesBundle));
            AssertSessionsAreUsable(allSitesBundle);
        }

        [Theory]
        [InlineData("DCPowerResetDevice_SessionPerChannel.pinmap")]
        [InlineData("DCPowerResetDevice_SharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_CommonSession.pinmap")]
        public void SiteZeroBundle_ResetSingleChannel_OtherChannelsInSameSessionKeepConfiguration(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(ConfiguredVoltageLevel, UpdateMode.Commit);
            var siteZeroBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);

            siteZeroBundle.Reset(SiteZeroVccChannel);

            AssertVoltageLevels(siteZeroBundle.FilterByPin("VCC"), DefaultVoltageLevel);
            AssertVoltageLevels(siteZeroBundle.FilterByPin("VDD"), ConfiguredVoltageLevel);
            AssertVoltageLevels(sessionManager.DCPower(_resetDevicePins).FilterBySite(_sitesOtherThanZero), ConfiguredVoltageLevel);
        }

        [Fact]
        public void SiteZeroBundle_ResetWithUnorderedSpacedAndDuplicatedChannels_SiteZeroChannelsReset()
        {
            var sessionManager = Initialize("DCPowerResetDevice_SharedSessionsWithinInstrument.pinmap");
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(ConfiguredVoltageLevel, UpdateMode.Commit);
            var siteZeroBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);

            siteZeroBundle.Reset(" SMU_4147_C2_S10/0 ,SMU_4147_C1_S11/0, SMU_4147_C1_S11/0 ");

            AssertVoltageLevels(siteZeroBundle, DefaultVoltageLevel);
            AssertVoltageLevels(sessionManager.DCPower(_resetDevicePins).FilterBySite(_sitesOtherThanZero), ConfiguredVoltageLevel);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(" , ")]
        public void SiteZeroBundle_ResetWithNullOrEmptyChannelString_ThrowsException(string channelString)
        {
            var sessionManager = Initialize("DCPowerResetDevice_SessionPerChannel.pinmap");
            var siteZeroBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);

            void Reset() => siteZeroBundle.Reset(channelString);

            var exception = Assert.Throws<NISemiconductorTestException>(Reset);
            Assert.Contains("channelString", exception.Message);
            Assert.Contains("cannot be null or empty", exception.Message);
        }

        [Theory]
        [InlineData("SMU_4147_C1_S11/1")]
        [InlineData("SMU_4147_C1_S11/0, SMU_4147_C2_S10/3")]
        [InlineData("UnknownInstrument/0")]
        public void SiteZeroBundle_ResetChannelNotInBundle_ThrowsExceptionAndNoChannelReset(string channelString)
        {
            var sessionManager = Initialize("DCPowerResetDevice_SessionPerChannel.pinmap");
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(ConfiguredVoltageLevel, UpdateMode.Commit);
            var siteZeroBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);

            void Reset() => siteZeroBundle.Reset(channelString);

            var exception = Assert.Throws<NISemiconductorTestException>(Reset);
            Assert.Contains("are not part of the sessions bundle", exception.Message);
            AssertVoltageLevels(sessionManager.DCPower(_resetDevicePins), ConfiguredVoltageLevel);
        }

        [Theory]
        [InlineData("DCPowerResetDevice_Ganged_SessionPerChannel.pinmap")]
        [InlineData("DCPowerResetDevice_Ganged_SharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_Ganged_AcrossInstruments.pinmap")]
        [InlineData("DCPowerResetDevice_Ganged_SessionsSpanningInstruments.pinmap")]
        public void GangedPinGroup_ResetAllGangedChannels_GangedChannelsResetAndBundleUsable(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            var gangedBundle = sessionManager.DCPower(_gangedPins);
            gangedBundle.GangPinGroup(GangedPinGroup);
            gangedBundle.ForceVoltage(voltageLevel: ConfiguredVoltageLevel, currentLimit: GangedCurrentLimit);

            gangedBundle.Reset(GetChannelString(gangedBundle));

            AssertVoltageLevels(gangedBundle, DefaultVoltageLevel);
            Assert.True(gangedBundle.HasGangedChannels);
            gangedBundle.ForceVoltage(voltageLevel: ConfiguredVoltageLevel, currentLimit: GangedCurrentLimit);
            gangedBundle.UngangPinGroup(GangedPinGroup);
        }

        [Theory]
        [InlineData("DCPowerResetDevice_SessionPerChannel.pinmap", false)]
        [InlineData("DCPowerResetDevice_SessionPerChannel.pinmap", true)]
        [InlineData("DCPowerResetDevice_SessionPerInstrument.pinmap", false)]
        [InlineData("DCPowerResetDevice_SessionPerInstrument.pinmap", true)]
        [InlineData("DCPowerResetDevice_SharedSessionsWithinInstrument.pinmap", false)]
        [InlineData("DCPowerResetDevice_SharedSessionsWithinInstrument.pinmap", true)]
        [InlineData("DCPowerResetDevice_SharedSessionsAcrossInstruments.pinmap", false)]
        [InlineData("DCPowerResetDevice_SharedSessionsAcrossInstruments.pinmap", true)]
        [InlineData("DCPowerResetDevice_CommonSession.pinmap", false)]
        [InlineData("DCPowerResetDevice_CommonSession.pinmap", true)]
        public void SitesSplitAcrossBundles_ResetDeviceOnSiteZeroBundle_SiteZeroChannelsResetAndOtherChannelsRecovered(string pinMap, bool restoreConfiguration)
        {
            /// Arrange
            var sessionManager = Initialize(pinMap);
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(ConfiguredVoltageLevel, UpdateMode.Commit);
            var bundleA = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);
            var bundleB = sessionManager.DCPower(_resetDevicePins).FilterBySite(1);
            var bundleC = sessionManager.DCPower(_resetDevicePins).FilterBySite(_sitesOtherThanZero);
            var bundleD = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);
            var expectedOtherChannelsVoltageLevel = restoreConfiguration ? ConfiguredVoltageLevel : DefaultVoltageLevel;

            /// Act
            bundleA.ResetDevice(restoreConfiguration);

            /// Assert
            AssertVoltageLevels(bundleA, DefaultVoltageLevel);
            AssertVoltageLevels(bundleD, DefaultVoltageLevel);
            AssertVoltageLevels(bundleB, expectedOtherChannelsVoltageLevel);
            AssertVoltageLevels(bundleC, expectedOtherChannelsVoltageLevel);
            AssertSessionsAreUsable(bundleA, bundleB, bundleC, bundleD);
        }

        [Theory]
        [InlineData("DCPowerResetDevice_SessionPerChannel.pinmap")]
        [InlineData("DCPowerResetDevice_SessionPerInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_SharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_SharedSessionsAcrossInstruments.pinmap")]
        [InlineData("DCPowerResetDevice_CommonSession.pinmap")]
        public void SitesSplitAcrossBundles_ResetDeviceWithDefaultRestoreConfiguration_OtherChannelsRestored(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(ConfiguredVoltageLevel, UpdateMode.Commit);
            var bundleA = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);
            var otherSitesBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(_sitesOtherThanZero);

            bundleA.ResetDevice();

            AssertVoltageLevels(bundleA, DefaultVoltageLevel);
            AssertVoltageLevels(otherSitesBundle, ConfiguredVoltageLevel);
        }

        [Theory]
        [InlineData("DCPowerResetDevice_SessionPerChannel.pinmap")]
        [InlineData("DCPowerResetDevice_SessionPerInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_SharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_SharedSessionsAcrossInstruments.pinmap")]
        [InlineData("DCPowerResetDevice_CommonSession.pinmap")]
        public void SitesSplitAcrossBundles_ResetDeviceOnSiteZeroBundle_SessionsOfSiteZeroBundleAreNotReinitialized(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            var bundleA = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);
            var sessionsBeforeReset = GetSessions(bundleA);

            bundleA.ResetDevice();

            AssertSameSessions(sessionsBeforeReset, GetSessions(bundleA));
        }

        [Fact]
        public void SessionPerChannel_ResetDeviceOnSiteZeroBundle_SessionsOfOtherSitesReinitialized()
        {
            var sessionManager = Initialize("DCPowerResetDevice_SessionPerChannel.pinmap");
            var bundleA = sessionManager.DCPower(_resetDevicePins).FilterBySite(0);
            var otherSitesBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(_sitesOtherThanZero);
            var otherSitesSessionsBeforeReset = GetSessions(otherSitesBundle);

            bundleA.ResetDevice();

            var otherSitesSessionsAfterReset = GetSessions(otherSitesBundle);
            Assert.Equal(otherSitesSessionsBeforeReset.Count, otherSitesSessionsAfterReset.Count);
            Assert.All(otherSitesSessionsAfterReset, session => Assert.DoesNotContain(otherSitesSessionsBeforeReset, sessionBeforeReset => ReferenceEquals(sessionBeforeReset, session)));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SessionPerChannel_ResetDeviceOnBundleWithTwoSessionsOnSameInstrument_AllChannelsOfBundleReset(bool restoreConfiguration)
        {
            var sessionManager = Initialize("DCPowerResetDevice_SessionPerChannel.pinmap");
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(ConfiguredVoltageLevel, UpdateMode.Commit);
            var resetBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(new[] { 0, 1 });
            var otherSitesBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(new[] { 2, 3 });

            resetBundle.ResetDevice(restoreConfiguration);

            AssertVoltageLevels(resetBundle, DefaultVoltageLevel);
            AssertVoltageLevels(otherSitesBundle, restoreConfiguration ? ConfiguredVoltageLevel : DefaultVoltageLevel);
            AssertSessionsAreUsable(resetBundle, otherSitesBundle);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SessionPerChannel_ResetDeviceOnPinFilteredBundle_ChannelsOnOtherInstrumentUntouched(bool restoreConfiguration)
        {
            var sessionManager = Initialize("DCPowerResetDevice_SessionPerChannel.pinmap");
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(ConfiguredVoltageLevel, UpdateMode.Commit);
            var vccBundle = sessionManager.DCPower(_resetDevicePins).FilterByPin("VCC");
            var vddBundle = sessionManager.DCPower(_resetDevicePins).FilterByPin("VDD");
            var vddSessionsBeforeReset = GetSessions(vddBundle);

            vccBundle.ResetDevice(restoreConfiguration);

            AssertVoltageLevels(vccBundle, DefaultVoltageLevel);
            AssertVoltageLevels(vddBundle, ConfiguredVoltageLevel);
            AssertSameSessions(vddSessionsBeforeReset, GetSessions(vddBundle));
            AssertSessionsAreUsable(vccBundle, vddBundle);
        }

        [Theory]
        [InlineData("DCPowerResetDevice_SessionPerChannel.pinmap", false)]
        [InlineData("DCPowerResetDevice_SessionPerChannel.pinmap", true)]
        [InlineData("DCPowerResetDevice_SharedSessionsWithinInstrument.pinmap", false)]
        [InlineData("DCPowerResetDevice_SharedSessionsWithinInstrument.pinmap", true)]
        [InlineData("DCPowerResetDevice_CommonSession.pinmap", false)]
        [InlineData("DCPowerResetDevice_CommonSession.pinmap", true)]
        public void OtherSitesDisabled_ResetDeviceOnSiteZeroBundle_ChannelsOfDisabledSitesRecovered(string pinMap, bool restoreConfiguration)
        {
            var sessionManager = Initialize(pinMap);
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(ConfiguredVoltageLevel, UpdateMode.Commit);
            var siteZeroSessionManager = new TSMSessionManager(_tsmContext.GetSemiconductorModuleContextWithSites(new[] { 0 }));
            var bundleA = siteZeroSessionManager.DCPower(_resetDevicePins);

            bundleA.ResetDevice(restoreConfiguration);

            var disabledSitesBundle = sessionManager.DCPower(_resetDevicePins).FilterBySite(_sitesOtherThanZero);
            AssertVoltageLevels(bundleA, DefaultVoltageLevel);
            AssertVoltageLevels(disabledSitesBundle, restoreConfiguration ? ConfiguredVoltageLevel : DefaultVoltageLevel);
            AssertSessionsAreUsable(bundleA, disabledSitesBundle);
        }

        [Theory]
        [InlineData("DCPowerResetDevice_SessionPerChannel.pinmap")]
        [InlineData("DCPowerResetDevice_SessionPerInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_SharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_SharedSessionsAcrossInstruments.pinmap")]
        [InlineData("DCPowerResetDevice_CommonSession.pinmap")]
        public void OneThreadPerSite_ResetDeviceOnEachSiteBundle_AllChannelsResetAndUsable(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            sessionManager.DCPower(_resetDevicePins).ConfigureVoltageLevel(ConfiguredVoltageLevel, UpdateMode.Commit);

            Parallel.ForEach(_tsmContext.SiteNumbers, siteNumber =>
            {
                var siteSessionManager = new TSMSessionManager(_tsmContext.GetSemiconductorModuleContextWithSites(new[] { siteNumber }));
                siteSessionManager.DCPower(_resetDevicePins).ResetDevice();
            });

            var allSitesBundle = sessionManager.DCPower(_resetDevicePins);
            AssertVoltageLevels(allSitesBundle, DefaultVoltageLevel);
            AssertSessionsAreUsable(allSitesBundle);
        }

        [Theory]
        [InlineData("DCPowerResetDevice_Ganged_SessionPerChannel.pinmap")]
        [InlineData("DCPowerResetDevice_Ganged_SharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_Ganged_AcrossInstruments.pinmap")]
        [InlineData("DCPowerResetDevice_Ganged_SessionsSpanningInstruments.pinmap")]
        public void GangedPinGroup_ResetDeviceOnSiteOneBundle_SiteZeroGangedChannelsRestoredAndUsable(string pinMap)
        {
            /// Arrange
            var sessionManager = Initialize(pinMap);
            var gangedBundle = sessionManager.DCPower(_gangedPins);
            gangedBundle.GangPinGroup(GangedPinGroup);
            gangedBundle.ForceVoltage(voltageLevel: ConfiguredVoltageLevel, currentLimit: GangedCurrentLimit);
            var siteZeroBundle = gangedBundle.FilterBySite(0);
            var siteOneBundle = gangedBundle.FilterBySite(1);
            var siteZeroVoltageLevelsBeforeReset = GetVoltageLevelPerChannel(siteZeroBundle);

            /// Act
            siteOneBundle.ResetDevice();

            /// Assert
            var siteZeroVoltageLevelsAfterReset = GetVoltageLevelPerChannel(siteZeroBundle);
            Assert.Equal(siteZeroVoltageLevelsBeforeReset.Count, siteZeroVoltageLevelsAfterReset.Count);
            foreach (var channelVoltageLevel in siteZeroVoltageLevelsBeforeReset)
            {
                Assert.Equal(channelVoltageLevel.Value, siteZeroVoltageLevelsAfterReset[channelVoltageLevel.Key], 3);
            }
            AssertVoltageLevels(siteOneBundle, DefaultVoltageLevel);
            Assert.True(gangedBundle.HasGangedChannels);
            gangedBundle.ForceVoltage(voltageLevel: ConfiguredVoltageLevel, currentLimit: GangedCurrentLimit);
            gangedBundle.UngangPinGroup(GangedPinGroup);
        }

        [Theory]
        [InlineData("DCPowerResetDevice_SessionPerChannel.pinmap")]
        [InlineData("DCPowerResetDevice_SessionPerInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_SharedSessionsWithinInstrument.pinmap")]
        [InlineData("DCPowerResetDevice_SharedSessionsAcrossInstruments.pinmap")]
        [InlineData("DCPowerResetDevice_CommonSession.pinmap")]
        public void OneThreadForAllSites_ResetDeviceOnAllSitesBundle_AllChannelsResetAndUsable(string pinMap)
        {
            var sessionManager = Initialize(pinMap);
            var allSitesBundle = sessionManager.DCPower(_resetDevicePins);
            allSitesBundle.ConfigureVoltageLevel(ConfiguredVoltageLevel, UpdateMode.Commit);

            allSitesBundle.ResetDevice();

            AssertVoltageLevels(allSitesBundle, DefaultVoltageLevel);
            AssertSessionsAreUsable(allSitesBundle);
        }

        [Fact]
        public void SessionsNotInitializedWithInitializeAndClose_ResetDevice_ThrowsException()
        {
            _tsmContext = CreateTSMContext("DCPowerResetDevice_SessionPerChannel.pinmap");
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
