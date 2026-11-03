using System;
using System.Linq;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Scope
{
    [Collection("NonParallelizable")]
    public sealed class AcquisitionTests : IDisposable
    {
        private static readonly PrecisionTimeSpan DefaultTimeout = PrecisionTimeSpan.FromSeconds(5);

        private readonly ISemiconductorModuleContext _tsmContext;

        public AcquisitionTests()
        {
            _tsmContext = CreateTSMContext("ScopeExtensionsTests.pinmap");
            Initialize(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        #region ConfigureAcquisitionType Tests

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ConfigureAcquisitionTypeWithNormal_AcquisitionTypeCorrectlyConfigured()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");

            sessionsBundle.ConfigureAcquisitionType(ScopeAcquisitionType.Normal);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeAcquisitionType.Normal, sessionInfo.Session.Acquisition.Type));
        }

        [Theory]
        [InlineData("SCP_5622_Pin")]
        [InlineData("SCP_5186_Pin")]
        public void SessionsBundleWithDifferentInstrumentModels_ConfigureAcquisitionTypeWithNormal_AcquisitionTypeCorrectlyConfigured(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);

            sessionsBundle.ConfigureAcquisitionType(ScopeAcquisitionType.Normal);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeAcquisitionType.Normal, sessionInfo.Session.Acquisition.Type));
        }

        [Fact]
        public void SessionsBundle_ConfigureAcquisitionTypeWithFlexibleResolution_AcquisitionTypeCorrectlyConfigured()
        {
            // Flexible resolution acquisition is only supported by the PXIe-5922, which is not available on the STSNIBCauvery.
            var sessionsBundle = GetSessionsBundle("SCP_5922_Pin");

            sessionsBundle.ConfigureAcquisitionType(ScopeAcquisitionType.FlexibleResolution);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeAcquisitionType.FlexibleResolution, sessionInfo.Session.Acquisition.Type));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ConfigureAcquisitionTypeWithMultiplePins_AcquisitionTypeCorrectlyConfiguredForAllSessions()
        {
            var sessionsBundle = GetSessionsBundle(new[] { "SCP_5172_Pin_1", "SCP_5172_Pin_2", "SCP_5172_Pin_3" });

            sessionsBundle.ConfigureAcquisitionType(ScopeAcquisitionType.Normal);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeAcquisitionType.Normal, sessionInfo.Session.Acquisition.Type));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ConfigureAcquisitionTypeCalledMultipleTimes_LastValueApplied()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");

            sessionsBundle.ConfigureAcquisitionType(ScopeAcquisitionType.FlexibleResolution);
            sessionsBundle.ConfigureAcquisitionType(ScopeAcquisitionType.Normal);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeAcquisitionType.Normal, sessionInfo.Session.Acquisition.Type));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ConfigureAcquisitionTypeWithInvalidEnumValue_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");

            // ConfigureAcquisitionType does not validate the ScopeAcquisitionType value; it is passed to the driver,
            // which rejects the out-of-range value. Because the driver call is made inside a Do operation,
            // the driver exception is wrapped in an NISemiconductorTestException.
            void ConfigureAcquisitionType()
            {
                sessionsBundle.ConfigureAcquisitionType((ScopeAcquisitionType)(-1));
                sessionsBundle.Commit();
            }

            Assert.Throws<NISemiconductorTestException>(ConfigureAcquisitionType);
        }

        #endregion

        #region ReadWaveform Tests

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWaveform_ReturnsWaveformForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            var waveforms = sessionsBundle.ReadWaveform(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Theory]
        [InlineData("SCP_5622_Pin")]
        [InlineData("SCP_5186_Pin")]
        public void SessionsBundleWithDifferentInstrumentModels_ReadWaveform_ReturnsWaveformForEachSitePin(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);
            ConfigureTimingForAcquisition(sessionsBundle);

            var waveforms = sessionsBundle.ReadWaveform(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWaveformWithMultiplePins_ReturnsWaveformForAllSitePins()
        {
            var sessionsBundle = GetSessionsBundle(new[] { "SCP_5172_Pin_1", "SCP_5172_Pin_2" });
            ConfigureTimingForAcquisition(sessionsBundle);

            var waveforms = sessionsBundle.ReadWaveform(DefaultTimeout, numberOfSamples: 1000);

            Assert.Equal(2, waveforms.PinNames.Length);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        public void SessionsBundle_ReadWaveformWithTriggerNeverReceived_ThrowsNISemiconductorTestExceptionWithChannelDetails()
        {
            // A software trigger that is never sent prevents the acquisition from completing,
            // so the driver reports a timeout once the specified timeout elapses.
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Do(sessionInfo => sessionInfo.Session.Trigger.ConfigureTriggerSoftware(PrecisionTimeSpan.Zero, PrecisionTimeSpan.Zero));
            sessionsBundle.Commit();

            void ReadWaveform() => sessionsBundle.ReadWaveform(PrecisionTimeSpan.FromMilliseconds(100), numberOfSamples: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(ReadWaveform);
            Assert.Contains("channel", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWaveformWithNegativeOneNumberOfSamples_ReturnsAllAvailableSamplesInRecord()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // Specifying -1 for numberOfSamples retrieves all samples currently available in the record,
            // rather than a specific fixed count.
            var waveforms = sessionsBundle.ReadWaveform(DefaultTimeout, numberOfSamples: -1);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotEmpty(waveforms.ExtractSite(site).Values.First().GetRawData()));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWaveformWithNegativeOneTimeout_WaitsIndefinitelyAndReturnsWaveform()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // Specifying -1 for timeout tells the driver to wait indefinitely for the acquisition to complete.
            var waveforms = sessionsBundle.ReadWaveform(PrecisionTimeSpan.FromSeconds(-1), numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWaveformWithZeroNumberOfSamples_ReturnsEmptyWaveformForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // The driver only validates that numberOfSamples fits within a 32-bit integer,
            // so a value of 0 is passed through and results in a waveform with no samples.
            var waveforms = sessionsBundle.ReadWaveform(DefaultTimeout, numberOfSamples: 0);

            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(0, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWaveformWithNegativeNumberOfSamplesOtherThanNegativeOne_ReturnsAllAvailableSamplesInRecord()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // The driver only validates that numberOfSamples fits within a 32-bit integer,
            // so a negative value other than -1 is passed through and treated the same as -1.
            var waveforms = sessionsBundle.ReadWaveform(DefaultTimeout, numberOfSamples: -2);

            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        public void SessionsBundle_ReadWaveformWithZeroTimeoutAndNegativeOneNumberOfSamples_ThrowsNISemiconductorTestExceptionWithPinSiteDetails()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Do(sessionInfo => sessionInfo.Session.Trigger.ConfigureTriggerSoftware(PrecisionTimeSpan.Zero, PrecisionTimeSpan.Zero));
            sessionsBundle.Commit();

            // A timeout of 0 fetches whatever is currently available without waiting. A software trigger that is never sent
            // guarantees the acquisition cannot have completed when the fetch occurs, so the driver always reports an error.
            // Simulated sessions ignore the missing software trigger and do not throw, so this test only runs on the tester.
            void ReadWaveform() => sessionsBundle.ReadWaveform(PrecisionTimeSpan.Zero, numberOfSamples: -1);

            var exception = Assert.Throws<NISemiconductorTestException>(ReadWaveform);
            Assert.Contains("SCP_5172_Pin_1", exception.Message);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWaveformWithNegativeOneTimeoutAndNegativeOneNumberOfSamples_ReturnsAllSamplesInRecord()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // A timeout of -1 waits indefinitely for the acquisition to complete, and -1 samples retrieves the entire record.
            var waveforms = sessionsBundle.ReadWaveform(PrecisionTimeSpan.FromSeconds(-1), numberOfSamples: -1);

            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWaveformWithNegativeTimeoutOtherThanNegativeOne_ReturnsWaveformForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // The driver does not reject negative timeout values other than -1; they are passed through and the acquisition completes.
            var waveforms = sessionsBundle.ReadWaveform(PrecisionTimeSpan.FromSeconds(-2), numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        #endregion

        #region Read Tests

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_Read_ReturnsSamplesForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            var samples = sessionsBundle.Read(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(samples.SiteNumbers, site => Assert.NotEmpty(samples.ExtractSite(site).Values.First()));
            Assert.All(samples.SiteNumbers, site => Assert.Equal(1000, samples.ExtractSite(site).Values.First().Length));
        }

        [Theory]
        [InlineData("SCP_5622_Pin")]
        [InlineData("SCP_5186_Pin")]
        public void SessionsBundleWithDifferentInstrumentModels_Read_ReturnsSamplesForEachSitePin(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);
            ConfigureTimingForAcquisition(sessionsBundle);

            var samples = sessionsBundle.Read(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(samples.SiteNumbers, site => Assert.NotEmpty(samples.ExtractSite(site).Values.First()));
            Assert.All(samples.SiteNumbers, site => Assert.Equal(1000, samples.ExtractSite(site).Values.First().Length));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWithMultiplePins_ReturnsSamplesForAllSitePins()
        {
            var sessionsBundle = GetSessionsBundle(new[] { "SCP_5172_Pin_1", "SCP_5172_Pin_2" });
            ConfigureTimingForAcquisition(sessionsBundle);

            var samples = sessionsBundle.Read(DefaultTimeout, numberOfSamples: 1000);

            Assert.Equal(2, samples.PinNames.Length);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        public void SessionsBundle_ReadWithTriggerNeverReceived_ThrowsNISemiconductorTestExceptionWithChannelDetails()
        {
            // A software trigger that is never sent prevents the acquisition from completing,
            // so the driver reports a timeout once the specified timeout elapses.
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Do(sessionInfo => sessionInfo.Session.Trigger.ConfigureTriggerSoftware(PrecisionTimeSpan.Zero, PrecisionTimeSpan.Zero));
            sessionsBundle.Commit();

            void Read() => sessionsBundle.Read(PrecisionTimeSpan.FromMilliseconds(100), numberOfSamples: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(Read);
            Assert.Contains("channel", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWithNegativeOneNumberOfSamples_ReturnsAllAvailableSamplesInRecord()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // Specifying -1 for numberOfSamples retrieves all samples currently available in the record,
            // rather than a specific fixed count.
            var samples = sessionsBundle.Read(DefaultTimeout, numberOfSamples: -1);

            Assert.All(samples.SiteNumbers, site => Assert.NotEmpty(samples.ExtractSite(site).Values.First()));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWithNegativeOneTimeout_WaitsIndefinitelyAndReturnsSamples()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // Specifying -1 for timeout tells the driver to wait indefinitely for the acquisition to complete.
            var samples = sessionsBundle.Read(PrecisionTimeSpan.FromSeconds(-1), numberOfSamples: 1000);

            Assert.All(samples.SiteNumbers, site => Assert.NotEmpty(samples.ExtractSite(site).Values.First()));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWithZeroNumberOfSamples_ReturnsEmptySamplesForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // The driver only validates that numberOfSamples fits within a 32-bit integer,
            // so a value of 0 is passed through and results in no samples being returned.
            var samples = sessionsBundle.Read(DefaultTimeout, numberOfSamples: 0);

            Assert.All(samples.SiteNumbers, site => Assert.Empty(samples.ExtractSite(site).Values.First()));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWithNegativeNumberOfSamplesOtherThanNegativeOne_ReturnsAllAvailableSamplesInRecord()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // The driver only validates that numberOfSamples fits within a 32-bit integer,
            // so a negative value other than -1 is passed through and treated the same as -1.
            var samples = sessionsBundle.Read(DefaultTimeout, numberOfSamples: -2);

            Assert.All(samples.SiteNumbers, site => Assert.Equal(1000, samples.ExtractSite(site).Values.First().Length));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        public void SessionsBundle_ReadWithTriggerNeverReceivedOnMultiChannelCardWithPartialPinSelection_ThrowsNISemiconductorTestExceptionWithEntireCardChannelDetails()
        {
            // On the STSNIBCauvery tester pinmap, site0/SCP_5172_Pin_1, site1/SCP_5172_Pin_1, and site0/SCP_5172_Pin_3
            // reside on the same card, while site0/SCP_5172_Pin_2, site1/SCP_5172_Pin_2, and site1/SCP_5172_Pin_3
            // reside on a different card. Only SCP_5172_Pin_1 is explicitly selected for this acquisition, but since
            // a lost trigger signal prevents the entire card from acquiring data, the timeout message must include
            // the site-pin-channel details for every channel on that card, including site0/SCP_5172_Pin_3, which was
            // not explicitly selected. Channels on the other card are not affected and must not be reported.
            // A software trigger that is never sent simulates a lost trigger signal, so the acquisition cannot
            // complete and the driver reports a timeout once the specified timeout elapses.
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Do(sessionInfo => sessionInfo.Session.Trigger.ConfigureTriggerSoftware(PrecisionTimeSpan.Zero, PrecisionTimeSpan.Zero));
            sessionsBundle.Commit();

            void Read() => sessionsBundle.Read(PrecisionTimeSpan.FromMilliseconds(100), numberOfSamples: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(Read);
            Assert.Contains("site0/SCP_5172_Pin_1", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("site1/SCP_5172_Pin_1", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("site0/SCP_5172_Pin_3", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("site0/SCP_5172_Pin_2", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("site1/SCP_5172_Pin_2", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("site1/SCP_5172_Pin_3", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(Skip = "Requires a missing trigger to cause an acquisition timeout, which only occurs on physical hardware. The PXIe-5622 is not available on STSNIBCauvery.")]
        public void SessionsBundle_ReadWithTriggerNeverReceivedOnMultiSessionMultiCardBundle_ThrowsNISemiconductorTestExceptionWithPinSiteDetailsForBothCards()
        {
            // SCP_5622_Pin is connected to a different physical card per site (SCP_5622_C3_S05 for site0, SCP_5622_C3_S06 for site1),
            // so selecting this pin across both sites produces a bundle with 2 sessions on 2 same-type cards.
            // A software trigger that is never sent prevents the acquisition from completing on either card.
            var sessionsBundle = GetSessionsBundle("SCP_5622_Pin");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Do(sessionInfo => sessionInfo.Session.Trigger.ConfigureTriggerSoftware(PrecisionTimeSpan.Zero, PrecisionTimeSpan.Zero));
            sessionsBundle.Commit();

            void Read() => sessionsBundle.Read(PrecisionTimeSpan.FromMilliseconds(100), numberOfSamples: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(Read);
            Assert.Contains("site0/SCP_5622_Pin", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("site1/SCP_5622_Pin", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SessionsBundle_ReadOnMultiSessionMultiCardBundle_ReturnsSamplesForBothSites()
        {
            // The PXIe-5622 is not available on the STSNIBCauvery, so this test only runs offline.
            // SCP_5622_Pin is connected to a different physical card per site (SCP_5622_C3_S05 for site0, SCP_5622_C3_S06 for site1),
            // so selecting this pin across both sites produces a bundle with 2 sessions on 2 same-type cards.
            var sessionsBundle = GetSessionsBundle("SCP_5622_Pin");
            ConfigureTimingForAcquisition(sessionsBundle);

            var samples = sessionsBundle.Read(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(samples.SiteNumbers, site => Assert.NotEmpty(samples.ExtractSite(site).Values.First()));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        public void SessionsBundle_ReadWithZeroTimeoutAndNegativeOneNumberOfSamples_ThrowsNISemiconductorTestExceptionWithPinSiteDetails()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Do(sessionInfo => sessionInfo.Session.Trigger.ConfigureTriggerSoftware(PrecisionTimeSpan.Zero, PrecisionTimeSpan.Zero));
            sessionsBundle.Commit();

            // A timeout of 0 fetches whatever is currently available without waiting. A software trigger that is never sent
            // guarantees the acquisition cannot have completed when the fetch occurs, so the driver always reports an error.
            // Simulated sessions ignore the missing software trigger and do not throw, so this test only runs on the tester.
            void Read() => sessionsBundle.Read(PrecisionTimeSpan.Zero, numberOfSamples: -1);

            var exception = Assert.Throws<NISemiconductorTestException>(Read);
            Assert.Contains("SCP_5172_Pin_1", exception.Message);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWithNegativeOneTimeoutAndNegativeOneNumberOfSamples_ReturnsAllSamplesInRecord()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // A timeout of -1 waits indefinitely for the acquisition to complete, and -1 samples retrieves the entire record.
            var samples = sessionsBundle.Read(PrecisionTimeSpan.FromSeconds(-1), numberOfSamples: -1);

            Assert.All(samples.SiteNumbers, site => Assert.Equal(1000, samples.ExtractSite(site).Values.First().Length));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_ReadWithNegativeTimeoutOtherThanNegativeOne_ReturnsSamplesForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // The driver does not reject negative timeout values other than -1; they are passed through and the acquisition completes.
            var samples = sessionsBundle.Read(PrecisionTimeSpan.FromSeconds(-2), numberOfSamples: 1000);

            Assert.All(samples.SiteNumbers, site => Assert.Equal(1000, samples.ExtractSite(site).Values.First().Length));
        }

        #endregion

        #region FetchWaveform Tests

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_InitiateThenFetchWaveform_ReturnsWaveformForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            var waveforms = sessionsBundle.FetchWaveform(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Theory]
        [InlineData("SCP_5622_Pin")]
        [InlineData("SCP_5186_Pin")]
        public void SessionsBundleWithDifferentInstrumentModels_InitiateThenFetchWaveform_ReturnsWaveformForEachSitePin(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            var waveforms = sessionsBundle.FetchWaveform(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_FetchWaveformWithZeroTimeoutAndNoPendingAcquisition_ThrowsNISemiconductorTestExceptionWithChannelDetails()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // A timeout of 0 tells the driver to return whatever samples are currently available without waiting.
            // Since no acquisition has been initiated, no samples are available, so the driver still reports a timeout.
            void FetchWaveform() => sessionsBundle.FetchWaveform(PrecisionTimeSpan.Zero, numberOfSamples: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(FetchWaveform);
            Assert.Contains("channel", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_InitiateThenFetchWaveformWithNegativeOnePointsToFetch_ReturnsAllAvailableSamplesInRecord()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            // Specifying -1 for numberOfSamples retrieves all samples currently available in the record,
            // rather than a specific fixed count.
            var waveforms = sessionsBundle.FetchWaveform(DefaultTimeout, numberOfSamples: -1);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotEmpty(waveforms.ExtractSite(site).Values.First().GetRawData()));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_InitiateThenFetchWaveformWithNegativeOneTimeout_WaitsIndefinitelyAndReturnsWaveform()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            // Specifying -1 for timeout tells the driver to wait indefinitely for the acquisition to complete.
            var waveforms = sessionsBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(-1), numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_InitiateThenFetchWaveformWithZeroNumberOfSamples_ReturnsEmptyWaveformForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            // The driver only validates that numberOfSamples fits within a 32-bit integer,
            // so a value of 0 is passed through and results in a waveform with no samples.
            var waveforms = sessionsBundle.FetchWaveform(DefaultTimeout, numberOfSamples: 0);

            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(0, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_InitiateThenFetchWaveformWithNegativeNumberOfSamplesOtherThanNegativeOne_ReturnsAllAvailableSamplesInRecord()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            // The driver only validates that numberOfSamples fits within a 32-bit integer,
            // so a negative value other than -1 is passed through and treated the same as -1.
            var waveforms = sessionsBundle.FetchWaveform(DefaultTimeout, numberOfSamples: -2);

            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_InitiateThenFetchWaveformWithZeroTimeoutAndNegativeOneNumberOfSamples_ReturnsWithoutThrowing()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            // A timeout of 0 with -1 samples fetches whatever samples are currently available without waiting, so no exception is thrown.
            var waveforms = sessionsBundle.FetchWaveform(PrecisionTimeSpan.Zero, numberOfSamples: -1);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_InitiateThenFetchWaveformWithNegativeOneTimeoutAndNegativeOneNumberOfSamples_ReturnsAllSamplesInRecord()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            // A timeout of -1 waits indefinitely for the acquisition to complete, and -1 samples retrieves the entire record.
            var waveforms = sessionsBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(-1), numberOfSamples: -1);

            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Fact]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void SessionsBundle_InitiateThenFetchWaveformWithNegativeTimeoutOtherThanNegativeOne_ReturnsWaveformForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            // The driver does not reject negative timeout values other than -1; they are passed through and the fetch completes.
            var waveforms = sessionsBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(-2), numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        #endregion

        #region Helper Methods

        private static void ConfigureTimingForAcquisition(ScopeSessionsBundle sessionsBundle)
        {
            // Channels must be explicitly enabled before an acquisition can be initiated and fetched;
            // they are not enabled by default.
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Channels[sessionInfo.AllChannelsString].Enabled = true;
            });
            sessionsBundle.ConfigureTiming(new TimingSettings
            {
                MinimumSampleRate = 1000000,
                MinimumNumberOfPoints = 1000,
                ReferencePosition = 50,
                NumberOfRecords = 1,
                EnforceRealtime = true
            });
            sessionsBundle.Commit();
        }

        private ScopeSessionsBundle GetSessionsBundle(string pin)
        {
            return GetSessionsBundle(new[] { pin });
        }

        private ScopeSessionsBundle GetSessionsBundle(string[] pins)
        {
            var sessionManager = new TSMSessionManager(_tsmContext);
            return sessionManager.Scope(pins);
        }

        #endregion
    }
}
