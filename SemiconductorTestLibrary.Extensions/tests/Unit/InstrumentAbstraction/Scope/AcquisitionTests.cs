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

        [Fact]
        public void SessionsBundle_ConfigureAcquisitionTypeWithNormal_AcquisitionTypeCorrectlyConfigured()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");

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

        #endregion

        #region ReadWaveform Tests

        [Fact]
        public void SessionsBundle_ReadWaveform_ReturnsWaveformForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            var waveforms = sessionsBundle.ReadWaveform(DefaultTimeout, recordLength: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
        }

        [Fact]
        public void SessionsBundle_ReadWaveformWithMultiplePins_ReturnsWaveformForAllSitePins()
        {
            var sessionsBundle = GetSessionsBundle(new[] { "SCP_5172_Pin_1", "SCP_5172_Pin_2" });
            ConfigureTimingForAcquisition(sessionsBundle);

            var waveforms = sessionsBundle.ReadWaveform(DefaultTimeout, recordLength: 1000);

            Assert.Equal(2, waveforms.PinNames.Length);
        }

        [Fact]
        public void SessionsBundle_ReadWaveformWithZeroTimeout_ThrowsNISemiconductorTestExceptionWithChannelDetails()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            void ReadWaveform() => sessionsBundle.ReadWaveform(PrecisionTimeSpan.Zero, recordLength: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(ReadWaveform);
            Assert.Contains("channel", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region FetchWaveform Tests

        [Fact]
        public void SessionsBundle_InitiateThenFetchWaveform_ReturnsWaveformForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);
            sessionsBundle.Initiate();

            var waveforms = sessionsBundle.FetchWaveform(DefaultTimeout, pointsToFetch: 1000);

            Assert.All(waveforms.SiteNumbers, site => Assert.NotNull(waveforms.ExtractSite(site).Values.First()));
        }

        [Fact]
        public void SessionsBundle_FetchWaveformWithZeroTimeoutAndNoPendingAcquisition_ThrowsNISemiconductorTestExceptionWithChannelDetails()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            void FetchWaveform() => sessionsBundle.FetchWaveform(PrecisionTimeSpan.Zero, pointsToFetch: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(FetchWaveform);
            Assert.Contains("channel", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Read Tests

        [Fact]
        public void SessionsBundle_Read_ReturnsSamplesForEachSitePin()
        {
            var sessionsBundle = GetSessionsBundle("SCP_5172_Pin_1");
            ConfigureTimingForAcquisition(sessionsBundle);

            var samples = sessionsBundle.Read(DefaultTimeout, numberOfSamples: 1000);

            Assert.All(samples.SiteNumbers, site => Assert.NotEmpty(samples.ExtractSite(site).Values.First()));
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

            void Read() => sessionsBundle.Read(PrecisionTimeSpan.Zero, numberOfSamples: 1000);

            var exception = Assert.Throws<NISemiconductorTestException>(Read);
            Assert.Contains("channel", exception.Message, StringComparison.OrdinalIgnoreCase);
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
