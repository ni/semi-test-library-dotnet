using System;
using System.Collections.Generic;
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
        private const string _SCP_5186_Pin = "SCP_5186_Pin";
        private const string _SCP_5162_Pin = "SCP_5162_Pin";
        private const string _SCP_5172_Pin = "SCP_5172_Pin";

        private readonly ISemiconductorModuleContext _tsmContext;
        // Bandwidth values for NI 5162 device
        private const double Ni5162Z50OhmFullBandwidth = 1500000000.0;
        private const double Ni5162Z50Ohm20MHzFilter = 20000000.0;
        private const double Ni5162Z50Ohm175MHzFilter = 175000000.0;
        private const double Ni5162Z1MOhmMaxBandwidth = 300000000.0;

        // Bandwidth values for NI 5172 device
        private const double Ni5172Z50OhmMaxBandwidth = 100000000.0;
        private const double Ni5172Z1MOhmMaxBandwidth = 98000000.0;

        // Impedance constants
        private const double Z50Ohm = 50.0;
        private const double Z1MOhm = 1000000.0;
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
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            sessionsBundle.ConfigureVertical(new VerticalSettings());

            AssertVerticalSettings(sessionsBundle, new VerticalSettings());
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithCustomSettings_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var settings = new VerticalSettings
            {
                Range = 1.0,
                Offset = 0.25,
                ProbeAttenuation = 1.0,
                Enabled = true
            };

            sessionsBundle.ConfigureVertical(settings);
            AssertVerticalSettings(sessionsBundle, settings);
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithChannelDisabled_ChannelIsDisabled()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
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
            var sessionsBundle = GetSessionsBundle(_SCP_5162_Pin);
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
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var finalSettings = new VerticalSettings { Range = 0.5, Offset = 0.25 };

            sessionsBundle.ConfigureVertical(new VerticalSettings { Range = 1.0, Offset = 0.5 });
            sessionsBundle.ConfigureVertical(finalSettings);

            AssertVerticalSettings(sessionsBundle, finalSettings);
        }

        [Fact]
        public void InitiatedAcquisition_ConfigureVertical_Succeeds()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            sessionsBundle.Initiate();

            sessionsBundle.ConfigureVertical(new VerticalSettings { Range = 1.0 });

            sessionsBundle.Abort();
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithInvalidRange_ThrowsException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);

            var exception = Assert.Throws<NISemiconductorTestException>(
                () => sessionsBundle.ConfigureVertical(new VerticalSettings { Range = -1.0 }));

            Assert.NotNull(exception);
        }

        [Fact]
        public void SessionsBundle_ConfigureVerticalWithPerSiteSettings_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
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
            var sessionsBundle = GetSessionsBundle(new[] { _SCP_5186_Pin });
            var perPinPerSiteSettings = new PinSiteData<VerticalSettings>(
                new[] { _SCP_5186_Pin },
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
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var perSiteSettings = new SiteData<VerticalSettings>(new Dictionary<int, VerticalSettings>
            {
                [0] = new VerticalSettings()
            });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureVertical(perSiteSettings));
        }

        [Fact]
        public void SessionsBundle_ConfigureCharacteristicsWithDefaultValues_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5162_Pin);
            sessionsBundle.ConfigureVertical(new VerticalSettings());
            var electricalCharacteristics = new ElectricalCharacteristics();

            sessionsBundle.ConfigureElectricalCharacteristics(electricalCharacteristics);

            var expectedBW = GetExpectedBandwidthNi5162(electricalCharacteristics.InputImpedance, electricalCharacteristics.InputFrequencyMax);
            AssertElectricalCharacteristics(sessionsBundle, electricalCharacteristics.InputImpedance, expectedBW);
        }

        [Fact]
        public void SessionsBundle_ConfigureElectricalCharacteristicsWithCustomValues_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5172_Pin);
            sessionsBundle.ConfigureVertical(new VerticalSettings());
            var characteristics = new ElectricalCharacteristics
            {
                InputImpedance = Z50Ohm
            };

            sessionsBundle.ConfigureElectricalCharacteristics(characteristics);
            var expectedBW = GetExpectedBandwidthNi5172(characteristics.InputImpedance, characteristics.InputFrequencyMax);
            AssertElectricalCharacteristics(sessionsBundle, 50, expectedBW);
        }

        [Fact]
        public void SessionsBundle_ConfigureElectricalCharacteristicsCalledMultipleTimes_LastSettingsApplied()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5172_Pin);
            sessionsBundle.ConfigureVertical(new VerticalSettings());
            var finalCharacteristics = new ElectricalCharacteristics { InputImpedance = Z50Ohm };

            sessionsBundle.ConfigureElectricalCharacteristics(new ElectricalCharacteristics { InputImpedance = Z1MOhm });
            sessionsBundle.ConfigureElectricalCharacteristics(finalCharacteristics);

            var expectedBW = GetExpectedBandwidthNi5172(finalCharacteristics.InputImpedance, finalCharacteristics.InputFrequencyMax);
            AssertElectricalCharacteristics(sessionsBundle, 50, expectedBW);
        }

        [Fact]
        public void InitiatedAcquisition_ConfigureElectricalCharacteristics_Succeeds()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            sessionsBundle.Initiate();

            sessionsBundle.ConfigureElectricalCharacteristics(new ElectricalCharacteristics { InputImpedance = Z50Ohm, InputFrequencyMax = 1000 });

            sessionsBundle.Abort();
        }

        [Fact]
        public void SessionsBundle_ConfigureElectricalCharacteristicsWithInvalidInputImpedance_ThrowsException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5172_Pin);
            sessionsBundle.ConfigureVertical(new VerticalSettings());

            var exception = Assert.Throws<NISemiconductorTestException>(
                () => sessionsBundle.ConfigureElectricalCharacteristics(new ElectricalCharacteristics { InputImpedance = -1 }));

            Assert.NotNull(exception);
        }

        [Fact]
        public void SessionsBundle_ConfigureElectricalCharacteristicsWithPerSiteSettings_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5172_Pin);
            sessionsBundle.ConfigureVertical(new VerticalSettings());
            var siteZeroSettings = new ElectricalCharacteristics { InputImpedance = 50 };
            var siteOneSettings = new ElectricalCharacteristics { InputImpedance = 1000000 };
            var perSiteSettings = new SiteData<ElectricalCharacteristics>(new Dictionary<int, ElectricalCharacteristics>
            {
                [0] = siteZeroSettings,
                [1] = siteOneSettings
            });

            sessionsBundle.ConfigureElectricalCharacteristics(perSiteSettings);

            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                var expected = perSiteSettings.GetValue(sitePinInfo.SiteNumber);
                var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
                Assert.Equal(expected.InputImpedance, channel.InputImpedance, 3);
            });
        }

        [Fact]
        public void SessionsBundle_ConfigureElectricalCharacteristicsWithPerPinPerSiteSettings_ValuesApplied()
        {
            var sessionsBundle = GetSessionsBundle(new[] { _SCP_5172_Pin });
            sessionsBundle.ConfigureVertical(new VerticalSettings());
            var perPinPerSiteSettings = new PinSiteData<ElectricalCharacteristics>(
                new[] { _SCP_5172_Pin },
                new[] { 0, 1 },
                new[]
                {
                    new[] { new ElectricalCharacteristics { InputImpedance = 50 }, new ElectricalCharacteristics { InputImpedance = 1000000 } }
                });

            sessionsBundle.ConfigureElectricalCharacteristics(perPinPerSiteSettings);

            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                var expected = perPinPerSiteSettings.GetValue(sitePinInfo);
                var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
                Assert.Equal(expected.InputImpedance, channel.InputImpedance, 3);
            });
        }

        [Fact]
        public void SessionsBundle_ConfigureElectricalCharacteristicsWithPerSiteSettingsMissingSite_ThrowsException()
        {
            var sessionsBundle = GetSessionsBundle(_SCP_5186_Pin);
            var perSiteSettings = new SiteData<ElectricalCharacteristics>(new Dictionary<int, ElectricalCharacteristics>
            {
                [0] = new ElectricalCharacteristics()
            });

            Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureElectricalCharacteristics(perSiteSettings));
        }

        private static void AssertElectricalCharacteristics(ScopeSessionsBundle sessionsBundle, double inputImpedance, double inputFrequencyMax)
        {
            sessionsBundle.Do((ScopeSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
            {
                var channel = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString];
                Assert.Equal(inputImpedance, channel.InputImpedance, 3);
                Assert.Equal(inputFrequencyMax, channel.InputFrequencyMax, 3);
            });
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

        // Private helper methods for NI 5162 coercion logic
        private static double GetExpectedBandwidthNi5162(double inputImpedance, double configuredBandwidth)
        {
            if (inputImpedance == Z1MOhm)
            {
                // 1 MΩ path always coerces to 300 MHz regardless of input
                return Ni5162Z1MOhmMaxBandwidth;
            }
            else
            {
                // 50 Ω path has conditional coercion based on configured bandwidth
                if (configuredBandwidth <= 0.0 || configuredBandwidth > Ni5162Z50Ohm175MHzFilter)
                {
                    // Full bandwidth: -1.0, 0.0, or anything above 175 MHz
                    return Ni5162Z50OhmFullBandwidth;
                }
                else if (configuredBandwidth <= 8000000.0)
                {
                    // 20 MHz filter: 1.0 to 8 MHz (including exactly 8 MHz)
                    return Ni5162Z50Ohm20MHzFilter;
                }
                else
                {
                    // 175 MHz filter: > 8 MHz to 175 MHz
                    return Ni5162Z50Ohm175MHzFilter;
                }
            }
        }

        // Private helper methods for NI 5172 coercion logic
        private static double GetExpectedBandwidthNi5172(double inputImpedance, double configuredBandwidth)
        {
            if (inputImpedance == Z1MOhm)
            {
                // 1 MΩ path always coerces to 98 MHz
                return Ni5172Z1MOhmMaxBandwidth;
            }
            else
            {
                // 50 Ω path always coerces to 100 MHz
                return Ni5172Z50OhmMaxBandwidth;
            }
        }
    }
}
