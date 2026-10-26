using System;
using System.Linq;
using Ivi.Driver;
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
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
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

        [Theory]
        [InlineData("SCP_5172_Pin_1")]
        [InlineData("SCP_5622_Pin")]
        [InlineData("SCP_5186_Pin")]
        public void SessionsBundle_ConfigureAcquisitionTypeWithNormal_AcquisitionTypeCorrectlyConfigured(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);

            sessionsBundle.ConfigureAcquisitionType(ScopeAcquisitionType.Normal);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeAcquisitionType.Normal, sessionInfo.Session.Acquisition.Type));
        }

        [Fact]
        public void SessionsBundle_ConfigureAcquisitionTypeWithFlexibleResolution_AcquisitionTypeCorrectlyConfigured()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");

            sessionsBundle.ConfigureAcquisitionType(ScopeAcquisitionType.FlexibleResolution);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeAcquisitionType.FlexibleResolution, sessionInfo.Session.Acquisition.Type));
        }

        [Fact]
        public void SessionsBundle_ConfigureAcquisitionTypeWithMultiplePins_AcquisitionTypeCorrectlyConfiguredForAllSessions()
        {
            var sessionsBundle = GetSessionsBundle(new[] { "SCP_5172_Pin_1", "SCP_5172_Pin_2", "SCP_5172_Pin_3" });

            sessionsBundle.ConfigureAcquisitionType(ScopeAcquisitionType.Normal);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeAcquisitionType.Normal, sessionInfo.Session.Acquisition.Type));
        }

        [Fact]
        public void SessionsBundle_ConfigureAcquisitionTypeCalledMultipleTimes_LastValueApplied()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");

            sessionsBundle.ConfigureAcquisitionType(ScopeAcquisitionType.FlexibleResolution);
            sessionsBundle.ConfigureAcquisitionType(ScopeAcquisitionType.Normal);

            sessionsBundle.Do(sessionInfo => Assert.Equal(ScopeAcquisitionType.Normal, sessionInfo.Session.Acquisition.Type));
        }

        [Fact]
        public void SessionsBundle_ConfigureAcquisitionTypeWithInvalidEnumValue_ThrowsNISemiconductorTestException()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");

            // An out-of-range ScopeAcquisitionType value is not validated by this method and is passed directly
            // to the driver, which rejects it with an IviCDriverException. Since this call happens within a Do
            // operation, the exception is caught and wrapped in an NISemiconductorTestException.
            void ConfigureAcquisitionType() => sessionsBundle.ConfigureAcquisitionType((ScopeAcquisitionType)(-1));

            Assert.Throws<NISemiconductorTestException>(ConfigureAcquisitionType);
        }

        #endregion

        #region ReadWaveform Tests

        [Theory]
        [InlineData("SCP_5172_Pin_1")]
        [InlineData("SCP_5622_Pin")]
        [InlineData("SCP_5186_Pin")]
        public void SessionsBundle_ReadWaveform_ReturnsWaveformForEachSitePin(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);
            ConfigureTimingForAcquisition(sessionsBundle);

            var waveforms = sessionsBundle.ReadWaveform(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Fact]
        public void SessionsBundle_ReadWaveformWithMultiplePins_ReturnsWaveformForAllSitePins()
        {
            var sessionsBundle = GetSessionsBundle(new[] { "SCP_5172_Pin_1", "SCP_5172_Pin_2" });
            ConfigureTimingForAcquisition(sessionsBundle);

            var waveforms = sessionsBundle.ReadWaveform(DefaultTimeout, numberOfSamples: 1000);

            Assert.Equal(2, waveforms.PinNames.Length);
        }

        [Fact]
        public void SessionsBundle_ReadWaveformWithZeroTimeout_ThrowsNISemiconductorTestExceptionWithChannelDetails()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // A timeout of 0 tells the driver to return whatever samples are currently available without waiting.
            // Since no acquisition has started yet, no samples are available, so the driver still reports a timeout.
            void ReadWaveform() => sessionsBundle.ReadWaveform(PrecisionTimeSpan.Zero, numberOfSamples: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(ReadWaveform);
            Assert.Contains("channel", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
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
        public void SessionsBundle_ReadWaveformWithNegativeOneTimeout_WaitsIndefinitelyAndReturnsWaveform()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // Specifying -1 for timeout tells the driver to wait indefinitely for the acquisition to complete.
            var waveforms = sessionsBundle.ReadWaveform(PrecisionTimeSpan.FromSeconds(-1), numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public void SessionsBundle_ReadWaveformWithInvalidNumberOfSamples_ThrowsNISemiconductorTestException(long numberOfSamples)
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // A numberOfSamples value of 0, or a negative value other than -1, is an invalid parameter value for the
            // underlying driver, which rejects it with an IviCDriverException. Since this call happens within a
            // DoAndReturnPerSitePerPinResults operation, the exception is caught and wrapped in an
            // NISemiconductorTestException.
            void ReadWaveform() => sessionsBundle.ReadWaveform(DefaultTimeout, numberOfSamples);

            Assert.Throws<NISemiconductorTestException>(ReadWaveform);
        }

        #endregion

        #region Read Tests

        [Theory]
        [InlineData("SCP_5172_Pin_1")]
        [InlineData("SCP_5622_Pin")]
        [InlineData("SCP_5186_Pin")]
        public void SessionsBundle_Read_ReturnsSamplesForEachSitePin(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);
            ConfigureTimingForAcquisition(sessionsBundle);

            var samples = sessionsBundle.Read(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(samples.SiteNumbers, site => Assert.NotEmpty(samples.ExtractSite(site).Values.First()));
            Assert.All(samples.SiteNumbers, site => Assert.Equal(1000, samples.ExtractSite(site).Values.First().Length));
        }

        [Fact]
        public void SessionsBundle_ReadWithMultiplePins_ReturnsSamplesForAllSitePins()
        {
            var sessionsBundle = GetSessionsBundle(new[] { "SCP_5172_Pin_1", "SCP_5172_Pin_2" });
            ConfigureTimingForAcquisition(sessionsBundle);

            var samples = sessionsBundle.Read(DefaultTimeout, numberOfSamples: 1000);

            Assert.Equal(2, samples.PinNames.Length);
        }

        [Fact]
        public void SessionsBundle_ReadWithZeroTimeout_ThrowsNISemiconductorTestExceptionWithChannelDetails()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // A timeout of 0 tells the driver to return whatever samples are currently available without waiting.
            // Since no acquisition has started yet, no samples are available, so the driver still reports a timeout.
            void Read() => sessionsBundle.Read(PrecisionTimeSpan.Zero, numberOfSamples: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(Read);
            Assert.Contains("channel", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
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
        public void SessionsBundle_ReadWithNegativeOneTimeout_WaitsIndefinitelyAndReturnsSamples()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // Specifying -1 for timeout tells the driver to wait indefinitely for the acquisition to complete.
            var samples = sessionsBundle.Read(PrecisionTimeSpan.FromSeconds(-1), numberOfSamples: 1000);

            Assert.All(samples.SiteNumbers, site => Assert.NotEmpty(samples.ExtractSite(site).Values.First()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public void SessionsBundle_ReadWithInvalidNumberOfSamples_ThrowsNISemiconductorTestException(long numberOfSamples)
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            // A numberOfSamples value of 0, or a negative value other than -1, is an invalid parameter value for the
            // underlying driver, which rejects it with an IviCDriverException. Since this call happens within a
            // DoAndReturnPerSitePerPinResults operation, the exception is caught and wrapped in an
            // NISemiconductorTestException.
            void Read() => sessionsBundle.Read(DefaultTimeout, numberOfSamples);

            Assert.Throws<NISemiconductorTestException>(Read);
        }

        [Fact]
        public void SessionsBundle_ReadWithZeroTimeoutOnMultiChannelCardWithPartialPinSelection_ThrowsNISemiconductorTestExceptionWithEntireCardChannelDetails()
        {
            // SCP_5172_Pin_1, SCP_5172_Pin_2, and SCP_5172_Pin_3 all reside on the same SCP_5172_C8_S04 card.
            // Only SCP_5172_Pin_1 is explicitly selected for this acquisition, but since a single site losing its
            // trigger signal prevents the entire card from acquiring data, the timeout message must still include
            // the site-pin-channel details for SCP_5172_Pin_2 and SCP_5172_Pin_3, which were not explicitly selected.
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            void Read() => sessionsBundle.Read(PrecisionTimeSpan.Zero, numberOfSamples: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(Read);
            Assert.Contains("site0/SCP_5172_Pin_1", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("site1/SCP_5172_Pin_1", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("site0/SCP_5172_Pin_2", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("site1/SCP_5172_Pin_2", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("site0/SCP_5172_Pin_3", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("site1/SCP_5172_Pin_3", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SessionsBundle_ReadWithZeroTimeoutOnMultiSessionMultiCardBundle_ThrowsNISemiconductorTestExceptionWithPinSiteDetailsForBothCards()
        {
            // SCP_5622_Pin is connected to a different physical card per site (SCP_5622_C3_S05 for site0, SCP_5622_C3_S06 for site1),
            // so selecting this pin across both sites produces a bundle with 2 sessions on 2 same-type cards.
            var sessionsBundle = GetSessionsBundle("SCP_5622_Pin");
            ConfigureTimingForAcquisition(sessionsBundle);

            void Read() => sessionsBundle.Read(PrecisionTimeSpan.Zero, numberOfSamples: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(Read);
            Assert.Contains("site0/SCP_5622_Pin", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("site1/SCP_5622_Pin", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SessionsBundle_ReadOnMultiSessionMultiCardBundle_ReturnsSamplesForBothSites()
        {
            // SCP_5622_Pin is connected to a different physical card per site (SCP_5622_C3_S05 for site0, SCP_5622_C3_S06 for site1),
            // so selecting this pin across both sites produces a bundle with 2 sessions on 2 same-type cards.
            var sessionsBundle = GetSessionsBundle("SCP_5622_Pin");
            ConfigureTimingForAcquisition(sessionsBundle);

            var samples = sessionsBundle.Read(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(samples.SiteNumbers, site => Assert.NotEmpty(samples.ExtractSite(site).Values.First()));
        }

        #endregion

        #region FetchWaveform Tests

        [Theory]
        [InlineData("SCP_5172_Pin_1")]
        [InlineData("SCP_5622_Pin")]
        [InlineData("SCP_5186_Pin")]
        public void SessionsBundle_InitiateThenFetchWaveform_ReturnsWaveformForEachSitePin(string pin)
        {
            var sessionsBundle = GetSessionsBundle(pin);
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            var waveforms = sessionsBundle.FetchWaveform(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
            Assert.All(waveforms.SiteNumbers, site => Assert.Equal(1000, waveforms.ExtractSite(site).Values.First().SampleCount));
        }

        [Fact]
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
        public void SessionsBundle_InitiateThenFetchWaveformWithNegativeOneTimeout_WaitsIndefinitelyAndReturnsWaveform()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            // Specifying -1 for timeout tells the driver to wait indefinitely for the acquisition to complete.
            var waveforms = sessionsBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(-1), numberOfSamples: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public void SessionsBundle_InitiateThenFetchWaveformWithInvalidPointsToFetch_ThrowsNISemiconductorTestException(int numberOfSamples)
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            // A numberOfSamples value of 0, or a negative value other than -1, is an invalid parameter value for the
            // underlying driver, which rejects it with an IviCDriverException. Since this call happens within a
            // DoAndReturnPerSitePerPinResults operation, the exception is caught and wrapped in an
            // NISemiconductorTestException.
            void FetchWaveform() => sessionsBundle.FetchWaveform(DefaultTimeout, numberOfSamples);

            Assert.Throws<NISemiconductorTestException>(FetchWaveform);
        }

        #endregion

        #region Helper Methods

        private static void ConfigureTimingForAcquisition(ScopeSessionsBundle sessionsBundle)
        {
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
