using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Scope
{
    [Collection("NonParallelizable")]
    public sealed class ConfigureTests : IDisposable
    {
        private const string SinglePin = "DUTPin4";

        private readonly ISemiconductorModuleContext _tsmContext;

        public ConfigureTests()
        {
            _tsmContext = CreateTSMContext("ScopeTests.pinmap");
            Initialize(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithDefaultSettings_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);

            sessionsBundle.ConfigureVertical(new VerticalSettings());

            AssertVerticalSettings(sessionsBundle, new VerticalSettings());
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithCustomSettings_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);
            var settings = new VerticalSettings
            {
                Range = 0.5,
                Offset = 0.25,
                ProbeAttenuation = 10.0,
                Enabled = true
            };

            sessionsBundle.ConfigureVertical(settings);
            AssertVerticalSettings(sessionsBundle, settings);
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithChannelDisabled_ChannelIsDisabled()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);
            var settings = new VerticalSettings { Enabled = false };

            sessionsBundle.ConfigureVertical(settings);

            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                Assert.False(sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString].Enabled);
            });
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithTimeInterleavedSamplingEnabled_ValueApplied()
        {
            var sessionsBundle = GetSessionsBundle("DUTPin2");
            var settings = new VerticalSettings { EnableTimeInterleavedSampling = true };

            sessionsBundle.ConfigureVertical(settings);

            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                Assert.True(sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString].EnableTimeInterleavedSampling);
            });
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalCalledMultipleTimes_LastSettingsApplied()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);
            var finalSettings = new VerticalSettings { Range = 0.5, Offset = 0.25 };

            sessionsBundle.ConfigureVertical(new VerticalSettings { Range = 1.0, Offset = 0.5 });
            sessionsBundle.ConfigureVertical(finalSettings);

            AssertVerticalSettings(sessionsBundle, finalSettings);
        }

        [Fact]
        public void InitiatedAcquisition_ConfigureVertical_Succeeds()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);
            sessionsBundle.Initiate();

            sessionsBundle.ConfigureVertical(new VerticalSettings { Range = 1.0 });

            sessionsBundle.Abort();
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithInvalidRange_ThrowsException()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);

            var exception = Assert.Throws<NISemiconductorTestException>(
                () => sessionsBundle.ConfigureVertical(new VerticalSettings { Range = -1.0 }));

            Assert.NotNull(exception);
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithPerSiteSettings_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);
            var siteZeroSettings = new VerticalSettings { Range = 1, Offset = 0.1 };
            var siteOneSettings = new VerticalSettings { Range = 0.5, Offset = 0.2 };
            var perSiteSettings = new SiteData<VerticalSettings>(new Dictionary<int, VerticalSettings>
            {
                [0] = siteZeroSettings,
                [1] = siteOneSettings
            });

            sessionsBundle.ConfigureVertical(perSiteSettings);

            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                var expected = perSiteSettings.GetValue(sitePinInfo.SiteNumber);
                var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
                Assert.Equal(expected.Range, channel.Range, 3);
                Assert.Equal(expected.Offset, channel.Offset, 3);
            });
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithPerPinPerSiteSettings_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(new[] { "DUTPin4" });
            var perPinPerSiteSettings = new PinSiteData<VerticalSettings>(
                new[] { "DUTPin4" },
                new[] { 0, 1 },
                new[]
                {
                    new[] { new VerticalSettings { Range = 0.5 }, new VerticalSettings { Range = 1.0 } }
                });

            sessionsBundle.ConfigureVertical(perPinPerSiteSettings);

            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                var expected = perPinPerSiteSettings.GetValue(sitePinInfo);
                var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
                Assert.Equal(expected.Range, channel.Range, 3);
            });
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithPerSiteSettingsMissingSite_ThrowsException()
        {
            var sessionsBundle = GetSessionsBundle(SinglePin);
            var perSiteSettings = new SiteData<VerticalSettings>(new Dictionary<int, VerticalSettings>
            {
                [0] = new VerticalSettings()
            });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureVertical(perSiteSettings));
        }

        private static void AssertVerticalSettings(ScopeSessionsBundle sessionsBundle, VerticalSettings expected)
        {
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
                Assert.Equal(expected.Range, channel.Range, 3);
                Assert.Equal(expected.Offset, channel.Offset, 3);
                Assert.Equal(expected.Coupling, channel.Coupling);
                Assert.Equal(expected.ProbeAttenuation, channel.ProbeAttenuation, 3);
                Assert.Equal(expected.Enabled, channel.Enabled);
            });
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
    }
}
