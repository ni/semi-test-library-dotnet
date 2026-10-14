using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower.Utilities;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DCPower
{
    public sealed partial class SourceTests : IDisposable
    {
        private const double DefaultTimeout = 5.0;

        private void AssertEqualForDoubleArrays(double[] expected, double[] actual, int precision = 3)
        {
            Assert.Equal(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual[i], precision);
            }
        }

        private void AssertSequenceMeasurementsMatchExpected(
            DCPowerSessionsBundle sessionsBundle,
            Func<int, string, double[]> getExpectedSequence,
            double timeoutSeconds = 5.0,
            int precision = 3,
            int itemsToFetch = 2,
            bool checkForCurrentMeasurement = true,
            bool initiateChannel = true,
            string groupName = null)
        {
            var results = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                if (initiateChannel)
                {
                    sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Initiate();
                }
                return sessionInfo.Session.Measurement.Fetch(sitePinInfo.IndividualChannelString, PrecisionTimeSpan.FromSeconds(timeoutSeconds), itemsToFetch);
            });
            foreach (var siteNumber in results.SiteNumbers)
            {
                foreach (var pinName in results.PinNames)
                {
                    var expectedSequence = getExpectedSequence(siteNumber, groupName ?? pinName);
                    var actualSequence = checkForCurrentMeasurement ? results.GetValue(siteNumber, pinName).CurrentMeasurements : results.GetValue(siteNumber, pinName).VoltageMeasurements;
                    AssertEqualForDoubleArrays(expectedSequence, actualSequence, precision);
                }
            }
        }

        private void Assert_ClearAndDeleteConfigureAdvancedSequences(Action action, DCPowerSessionsBundle sessionsBundle, string sequenceName)
        {
            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                action();
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequenceSynchronized_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { 0.000, 0.005, 0.010 };
            sessionsBundle.ForceVoltageSequenceSynchronized(sequence, DefaultTimeout, currentLimit: 0.5, voltageLevelRange: 1.0, currentLimitRange: 0.5);

            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: 0.5));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageSequenceSynchronized_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = Array.Empty<double>();
            void ForceVoltageSequenceSynchronizedTest() => sessionsBundle.ForceVoltageSequenceSynchronized(sequence, DefaultTimeout, currentLimit: 0.5, voltageLevelRange: 1.0, currentLimitRange: 0.5);

            var exception = Assert.Throws<NISemiconductorTestException>(ForceVoltageSequenceSynchronizedTest);
            var exceptionMessage = "This feature is not supported on a ganged pin group";
            Assert.Contains(exceptionMessage, exception.Message);
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequenceSynchronizedWithPerPinPerSiteValues_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            var sequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.001, 0.002, 0.003 },
                    [1] = new[] { 0.004, 0.005, 0.006 },
                    [2] = new[] { 0.007, 0.008, 0.009 },
                    [3] = new[] { 0.010, 0.011, 0.012 }
                }
            });
            var currentLimits = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 1.0, [1] = 1.1, [2] = 1.2, [3] = 1.3 }
            });
            var voltageLevelRanges = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 1.0, [1] = 1.0, [2] = 1.0, [3] = 1.0 }
            });
            var currentLimitRanges = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 1.5, [1] = 1.5, [2] = 1.5, [3] = 1.5 }
            });
            sessionsBundle.ForceVoltageSequenceSynchronized(
                sequences,
                DefaultTimeout,
                currentLimit: currentLimits,
                voltageLevelRanges,
                currentLimitRanges);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: currentLimits.GetValue(sitePinInfo.SiteNumber, "VDD"));
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequenceSynchronizedWithPerSiteValues_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            // Create per-site sequences
            var sequences = new SiteData<double[]>(new[]
            {
                new[] { 0.001, 0.002, 0.003 },
                new[] { 0.004, 0.005, 0.006 },
                new[] { 0.007, 0.008, 0.009 },
                new[] { 0.010, 0.011, 0.012 }
            });
            var currentLimits = new SiteData<double>(new double[] { 1.0, 1.1, 1.2, 1.3 });
            var voltageLevelRanges = new SiteData<double>(new double[] { 1.0, 1.0, 1.0, 1.0 });
            var currentLimitRanges = new SiteData<double>(new double[] { 1.5, 1.5, 1.5, 1.5 });
            sessionsBundle.ForceVoltageSequenceSynchronized(
                sequences,
                DefaultTimeout,
                currentLimit: currentLimits,
                voltageLevelRange: voltageLevelRanges,
                currentLimitRange: currentLimitRanges);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: currentLimits.GetValue(sitePinInfo.SiteNumber));
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedWithPerSiteValuesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new SiteData<DCPowerSourceSettings[]>(new[]
            {
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 2.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 2.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 3.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 3.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 4.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 4.5,
                        Limit = 0.1
                    }
                }
            });
            sessionsBundle.ForceAdvancedSequenceSynchronized(sequence, sequenceLoopCount: 1, sequenceTimeoutInSeconds: 5.0);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedWithPerPinPerSiteValuesSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var vddSequence = new DCPowerSourceSettings[]
            {
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 1.0,
                    Limit = 0.1
                },
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 2.0,
                    Limit = 0.1
                }
            };
            var sequence = new PinSiteData<DCPowerSourceSettings[]>(
                new[] { "VDD" },
                new[]
                {
                    new SiteData<DCPowerSourceSettings[]>(sites, vddSequence)
                });
            sessionsBundle.ForceAdvancedSequenceSynchronized(sequence, sequenceLoopCount: 1, sequenceTimeoutInSeconds: 10.0);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithInconsistenceProperties_ThrowsException(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new PinSiteData<DCPowerSourceSettings[]>(
                new[] { "VDD" },
                new[]
                {
                    new SiteData<DCPowerSourceSettings[]>(
                        sites,
                        new[]
                        {
                            new DCPowerSourceSettings
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                                Level = 1.0,
                                Limit = 0.1,
                                TransientResponse = DCPowerSourceTransientResponse.Fast
                            },
                            new DCPowerSourceSettings
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                                Level = 1.5
                            }
                        })
                });
            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0));

            Assert.Contains("Inconsistent advanced sequence properties. The following properties must be specified or omitted for all steps in the sequence", exception.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetch_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new[]
            {
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 1.0,
                    Limit = 0.1
                },
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 1.5,
                    Limit = 0.1
                },
                new DCPowerSourceSettings
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = 2.0,
                    Limit = 0.1
                }
            };
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 3,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithPerSiteSequence_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequencePerSite = new SiteData<DCPowerSourceSettings[]>(new[]
            {
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.5,
                        Limit = 0.1
                    }
                },
                new[]
                {
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.0,
                        Limit = 0.1
                    },
                    new DCPowerSourceSettings
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = 1.5,
                        Limit = 0.1
                    }
                }
            });
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequencePerSite,
                sequenceLoopCount: 1,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithPerPinPerSiteSequence_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new PinSiteData<DCPowerSourceSettings[]>(
                new[] { "VDD" },
                new[]
                {
                    new SiteData<DCPowerSourceSettings[]>(
                        sites,
                        new[]
                        {
                            new DCPowerSourceSettings
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                                Level = 1.0,
                                Limit = 0.1
                            },
                            new DCPowerSourceSettings
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                                Level = 1.5,
                                Limit = 0.1
                            }
                        })
                });
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithInconsistencesPerSiteDCPowerAdvancedSequenceStepProperties_ThrowsExceptions(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequencePerSite = new SiteData<DCPowerAdvancedSequenceStepProperties[]>(
                sites,
                sites.Select(_ => new[]
                {
                    new DCPowerAdvancedSequenceStepProperties
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        VoltageLevel = 1.0,
                        CurrentLimit = 0.1,
                        CurrentLevel = 0.05,
                        TransientResponse = DCPowerSourceTransientResponse.Slow,
                        CurrentLevelRange = 0.2,
                        VoltageLevelRange = 5.0
                    },
                    new DCPowerAdvancedSequenceStepProperties
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        VoltageLevel = 1.5,
                        CurrentLimit = 0.1
                    }
                }).ToArray());
            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequencePerSite,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0));

            Assert.Contains("Inconsistent advanced sequence properties. The following properties must be specified or omitted for all steps in the sequence", exception.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithPerSiteDCPowerAdvancedSequenceStepProperties_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequencePerSite = new SiteData<DCPowerAdvancedSequenceStepProperties[]>(
                sites,
                sites.Select(_ => new[]
                {
                    new DCPowerAdvancedSequenceStepProperties
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        VoltageLevel = 1.0,
                        CurrentLimit = 0.1
                    },
                    new DCPowerAdvancedSequenceStepProperties
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        VoltageLevel = 1.5,
                        CurrentLimit = 0.1
                    }
                }).ToArray());
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequencePerSite,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithDCPowerAdvancedSequenceStepProperties_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new[]
            {
                new DCPowerAdvancedSequenceStepProperties
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    VoltageLevel = 1.0,
                    CurrentLimit = 0.1
                },
                new DCPowerAdvancedSequenceStepProperties
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    VoltageLevel = 1.5,
                    CurrentLimit = 0.1
                },
                new DCPowerAdvancedSequenceStepProperties
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    VoltageLevel = 2.0,
                    CurrentLimit = 0.1
                }
            };
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 3,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceAdvancedSequenceSynchronizedAndFetchWithPerPinPerSiteDCPowerAdvancedSequenceStepProperties_CorrectResultFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sites = GetActiveSites(sessionsBundle);
            CreateDCPowerAdvancedSequencePropertyMappingsCache();

            var sequence = new PinSiteData<DCPowerAdvancedSequenceStepProperties[]>(
                new[] { "VDD" },
                new[]
                {
                    new SiteData<DCPowerAdvancedSequenceStepProperties[]>(
                        sites,
                        new[]
                        {
                            new DCPowerAdvancedSequenceStepProperties
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                VoltageLevel = 1.0,
                                CurrentLimit = 0.1
                            },
                            new DCPowerAdvancedSequenceStepProperties
                            {
                                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                                VoltageLevel = 1.5,
                                CurrentLimit = 0.1
                            }
                        })
                });
            var results = sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                waitForSequenceCompletion: true,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var measurement = results.GetValue(sitePinInfo);
                    for (int i = 0; i < measurement.Length; i++)
                    {
                        Assert.Equal(1.0 + 0.5 * i, measurement[i].VoltageMeasurement, precision: 2);
                    }
                });
            }
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceAdvancedSequenceSynchronizedWithPerSiteValues_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            var sequence = new SiteData<DCPowerSourceSettings[]>(Array.Empty<DCPowerSourceSettings[]>());
            void ForceAdvancedSequenceSynchronizedTest() => sessionsBundle.ForceAdvancedSequenceSynchronized(sequence, sequenceLoopCount: 1, sequenceTimeoutInSeconds: 5.0);

            var exception = Assert.Throws<NISemiconductorTestException>(ForceAdvancedSequenceSynchronizedTest);
            var exceptionMessage = "This feature is not supported on a ganged pin group";
            Assert.Contains(exceptionMessage, exception.Message);
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceAdvancedSequenceSynchronizedWithPerPinPerSiteValues_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);
            var sites = GetActiveSites(sessionsBundle);

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            var sequence = new PinSiteData<DCPowerSourceSettings[]>(Array.Empty<string>(), Array.Empty<int>(), data: null);
            void ForceAdvancedSequenceSynchronizedTest() => sessionsBundle.ForceAdvancedSequenceSynchronized(sequence, sequenceLoopCount: 1, sequenceTimeoutInSeconds: 10.0);

            var exception = Assert.Throws<NISemiconductorTestException>(ForceAdvancedSequenceSynchronizedTest);
            var exceptionMessage = "This feature is not supported on a ganged pin group";
            Assert.Contains(exceptionMessage, exception.Message);
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceAdvancedSequenceSynchronizedAndFetch_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            var sequence = Array.Empty<DCPowerSourceSettings>();
            void ForceAdvancedSequenceSynchronizedAndFetchTest() => sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 3,
                measurementTimeoutInSeconds: 10.0);

            var exception = Assert.Throws<NISemiconductorTestException>(ForceAdvancedSequenceSynchronizedAndFetchTest);
            var exceptionMessage = "This feature is not supported on a ganged pin group";
            Assert.Contains(exceptionMessage, exception.Message);
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceAdvancedSequenceSynchronizedAndFetchWithPerSiteSequence_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            var sequence = new SiteData<DCPowerSourceSettings[]>(Array.Empty<DCPowerSourceSettings[]>());
            void ForceAdvancedSequenceSynchronizedAndFetchTest() => sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0);

            var exception = Assert.Throws<NISemiconductorTestException>(ForceAdvancedSequenceSynchronizedAndFetchTest);
            var exceptionMessage = "This feature is not supported on a ganged pin group";
            Assert.Contains(exceptionMessage, exception.Message);
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceAdvancedSequenceSynchronizedAndFetchWithPerPinPerSiteSequence_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sites = GetActiveSites(sessionsBundle);

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            var sequence = new PinSiteData<DCPowerSourceSettings[]>(Array.Empty<string>(), Array.Empty<int>(), data: null);
            void ForceAdvancedSequenceSynchronizedAndFetchTest() => sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceLoopCount: 1,
                sequenceTimeoutInSeconds: 10.0,
                pointsToFetch: 2,
                measurementTimeoutInSeconds: 10.0);

            var exception = Assert.Throws<NISemiconductorTestException>(ForceAdvancedSequenceSynchronizedAndFetchTest);
            var exceptionMessage = "This feature is not supported on a ganged pin group";
            Assert.Contains(exceptionMessage, exception.Message);
            sessionsBundle.UngangPinGroup(ThreePinsGangedGroup);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureAdvancedSequence_CorrectValueAreSet(bool setAsActiveSequence)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var steps = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, ApertureTime = 0.016, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, SourceDelay = 0.5, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            const string sequenceName = "ScalarAdvancedSequence";
            sessionsBundle.ConfigureAdvancedSequence(
                sequenceName,
                steps,
                setAsActiveSequence: setAsActiveSequence,
                commitFirstElementAsInitialState: false);

            if (setAsActiveSequence && !_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => new double[] { 1.0, 2.0, 3.0 }, precision: 3, itemsToFetch: 3, checkForCurrentMeasurement: false);
            }
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                var adv = output.Source.AdvancedSequencing;
                if (setAsActiveSequence)
                {
                    Assert.Equal(sequenceName, adv.ActiveAdvancedSequence);
                }
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureAdvancedSequenceWithPerSiteDataPerStep_CorrectValueAreSet(bool setAsActiveSequence)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var perSiteSteps = new SiteData<IList<DCPowerAdvancedSequenceStepProperties>>(new[]
            {
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { CurrentLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCCurrent },
                    new DCPowerAdvancedSequenceStepProperties { CurrentLevel = 1.2, OutputFunction = DCPowerSourceOutputFunction.DCCurrent }
                },
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { CurrentLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCCurrent },
                    new DCPowerAdvancedSequenceStepProperties { CurrentLevel = 2.2, OutputFunction = DCPowerSourceOutputFunction.DCCurrent }
                },
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                },
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 4.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 4.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                }
            });
            const string sequenceName = "PerSiteAdvancedSequence";
            sessionsBundle.ConfigureAdvancedSequence(
                sequenceName,
                perSiteSteps,
                setAsActiveSequence: setAsActiveSequence,
                commitFirstElementAsInitialState: false);

            if (setAsActiveSequence && !_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                var siteData = new SiteData<double[]>(new[]
                {
                    new double[] { 1.0, 1.2 },
                    new double[] { 2.0, 2.2 }
                });
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => siteData.GetValue(siteNumber), precision: 3, itemsToFetch: 3, checkForCurrentMeasurement: true);
            }
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                var adv = output.Source.AdvancedSequencing;
                if (setAsActiveSequence)
                {
                    Assert.Equal(sequenceName, adv.ActiveAdvancedSequence);
                }
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureAdvancedSequenceWithPinSiteDataPerStep_CorrectValueAreSet(bool setAsActiveSequence)
        {
            var pinNames = new[] { "VDD" };
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower(pinNames);

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var perSiteSteps = new Dictionary<string, IDictionary<int, IList<DCPowerAdvancedSequenceStepProperties>>>
            {
                ["VDD"] = new Dictionary<int, IList<DCPowerAdvancedSequenceStepProperties>>
                {
                    [0] = new List<DCPowerAdvancedSequenceStepProperties>
                    {
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                    },
                    [1] = new List<DCPowerAdvancedSequenceStepProperties>
                    {
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                    },
                    [2] = new List<DCPowerAdvancedSequenceStepProperties>
                    {
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                    },
                    [3] = new List<DCPowerAdvancedSequenceStepProperties>
                    {
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 4.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                        new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 4.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                    }
                }
            };
            var pinSiteData = new PinSiteData<IList<DCPowerAdvancedSequenceStepProperties>>(perSiteSteps);
            const string sequenceName = "PerPinPerSiteAdvancedSequence";
            sessionsBundle.ConfigureAdvancedSequence(
                sequenceName,
                pinSiteData,
                setAsActiveSequence: setAsActiveSequence,
                commitFirstElementAsInitialState: true);

            if (setAsActiveSequence && !_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                var data = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
                {
                    ["VDD"] = new Dictionary<int, double[]>()
                    {
                        [0] = new double[] { 1.0, 1.2 },
                        [1] = new double[] { 2.0, 2.2 }
                    }
                });
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => data.GetValue(siteNumber, pinName), precision: 3, itemsToFetch: 3, checkForCurrentMeasurement: false);
            }
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                var adv = output.Source.AdvancedSequencing;
                if (setAsActiveSequence)
                {
                    Assert.Equal(sequenceName, adv.ActiveAdvancedSequence);
                }
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureAdvancedSequenceWithNotSupportedProperties_ThrowsException(bool setAsActiveSequence)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var steps = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, LcrVoltageRange = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            const string sequenceName = "AdvancedSequenceException";
            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.ConfigureAdvancedSequence(
                sequenceName,
                steps,
                setAsActiveSequence: setAsActiveSequence,
                commitFirstElementAsInitialState: false));

            Assert.Contains("The specified instrument (NI PXIe-4147) does not support one or more of the requested advanced sequence properties.", exception.Message);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureAdvancedSequence_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower("AllPinsGangedGroup");
            sessionsBundle.GangPinGroup("AllPinsGangedGroup");

            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName = "Sequence";
            var stepProperties = Array.Empty<DCPowerAdvancedSequenceStepProperties>().ToList();
            void ConfigureAdvancedSequenceTest() => sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true);

            var configureException = Assert.Throws<NISemiconductorTestException>(ConfigureAdvancedSequenceTest);
            var exceptionMessage = "This feature is not supported on a ganged pin group";
            Assert.Contains(exceptionMessage, configureException.Message);
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceSynchronized_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { 0.000, 0.005, 0.010 };
            sessionsBundle.ForceCurrentSequenceSynchronized(sequence, DefaultTimeout, voltageLimit: 0.5, currentLevelRange: 0.1, voltageLimitRange: 0.5);

            sessionsBundle.Abort();
            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                Assert.Equal(0.1, sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Source.Current.CurrentLevelRange);
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceSynchronizedWithPerPinPerSiteValues_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            var currentSequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.001, 0.002, 0.003 },
                    [1] = new[] { 0.004, 0.005, 0.006 },
                    [2] = new[] { 0.007, 0.008, 0.009 },
                    [3] = new[] { 0.010, 0.011, 0.012 }
                }
            });
            var voltageLimits = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 1.0, [1] = 1.1, [2] = 1.2, [3] = 1.3 }
            });
            var currentLevelRanges = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 0.1, [1] = 0.1, [2] = 0.1, [3] = 0.1 }
            });
            var voltageLimitRanges = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["VDD"] = new Dictionary<int, double>() { [0] = 1.5, [1] = 1.5, [2] = 1.5, [3] = 1.5 }
            });
            sessionsBundle.ForceCurrentSequenceSynchronized(
                currentSequence,
                DefaultTimeout,
                voltageLimit: voltageLimits,
                currentLevelRange: currentLevelRanges,
                voltageLimitRange: voltageLimitRanges);

            sessionsBundle.Abort();
            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                Assert.Equal(currentLevelRanges.GetValue(sitePinInfo.SiteNumber, "VDD"), sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Source.Current.CurrentLevelRange);
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceSynchronizedWithPerSiteValues_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            // Create per-site sequences
            var currentSequences = new SiteData<double[]>(new[]
            {
                new[] { 0.001, 0.002, 0.003 },
                new[] { 0.004, 0.005, 0.006 },
                new[] { 0.007, 0.008, 0.009 },
                new[] { 0.010, 0.011, 0.012 }
            });
            var voltageLimits = new SiteData<double>(new double[] { 1.0, 1.1, 1.2, 1.3 });
            var currentLevelRanges = new SiteData<double>(new double[] { 0.1, 0.1, 0.1, 0.1 });
            var voltageLimitRanges = new SiteData<double>(new double[] { 1.5, 1.5, 1.5, 1.5 });
            sessionsBundle.ForceCurrentSequenceSynchronized(
                currentSequences,
                DefaultTimeout,
                voltageLimit: voltageLimits,
                currentLevelRange: currentLevelRanges,
                voltageLimitRange: voltageLimitRanges);

            sessionsBundle.Abort();
            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                Assert.Equal(currentLevelRanges.GetValue(sitePinInfo.SiteNumber), sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Source.Current.CurrentLevelRange, 2);
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentSequenceSynchronized_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = Array.Empty<double>();
            void ForceCurrentSequenceSynchronizedTest() => sessionsBundle.ForceCurrentSequenceSynchronized(sequence, DefaultTimeout, voltageLimit: 0.5);

            var exception = Assert.Throws<NISemiconductorTestException>(ForceCurrentSequenceSynchronizedTest);
            var exceptionMessage = "This feature is not supported on a ganged pin group";
            Assert.Contains(exceptionMessage, exception.Message);
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequence_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { 1.0, 1.5, 2.0, 2.5, 3.0 };
            var currentLimit = 0.2;
            var currentLimitRange = 3.0;
            sessionsBundle.ForceVoltageSequence(
                sequence,
                DefaultTimeout,
                currentLimit,
                voltageLevelRange: 5.0,
                currentLimitRange: currentLimitRange,
                sequenceLoopCount: 1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: currentLimit, expectedCurrentLimitRange: currentLimitRange));
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequenceSiteData_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new[]
            {
                new[] { 1.0, 1.1, 1.2 },
                new[] { 2.0, 2.1, 2.2 },
                new[] { 3.0, 3.1, 3.2 },
                new[] { 4.0, 4.1, 4.2 }
            });
            var currentLimit = 0.3;
            var currentLimitRange = 3;
            sessionsBundle.ForceVoltageSequence(
                sequence,
                DefaultTimeout,
                currentLimit: currentLimit,
                voltageLevelRange: 6.0,
                currentLimitRange: currentLimitRange,
                sequenceLoopCount: 1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: currentLimit, expectedCurrentLimitRange: (double?)currentLimitRange));
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequencePinSiteData_CorrectValueAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>
                {
                    [0] = new[] { 1.0, 1.1, 1.2 },
                    [1] = new[] { 1.3, 1.4, 1.5 },
                    [2] = new[] { 1.6, 1.7, 1.8 },
                    [3] = new[] { 1.9, 2.0, 2.1 }
                }
            });
            var currentLimit = 0.25;
            var currentLimitRange = 3;
            sessionsBundle.ForceVoltageSequence(
                sequence,
                DefaultTimeout,
                currentLimit,
                7.0,
                currentLimitRange,
                1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: currentLimit, expectedCurrentLimitRange: (double?)currentLimitRange));
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceVoltageSequenceWithDefaultParametersSequenceAppliedSuccessfully(bool pinMapWithChannelGroup)
        {
            var pinNames = new string[] { "VDD" };
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower(pinNames);

            var sequence = new[] { 0.5, 1.0, 1.5 };
            sessionsBundle.ForceVoltageSequence(sequence, DefaultTimeout);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageSequence_CorrectValueAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { 0.5, 1.0, 1.5, 2.0 };
            sessionsBundle.ForceVoltageSequence(
                sequence,
                DefaultTimeout,
                1.0,
                5.0,
                3,
                1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: 0.2));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageSequenceWithPerSiteSequence_CorrectValueAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new double[][]
            {
                new[] { 0.4, 1.0, 1.6 },
                new[] { 0.6, 1.1, 1.6 }
            });
            sessionsBundle.ForceVoltageSequence(sequence, DefaultTimeout, currentLimit: 1.5, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                AssertVoltageSettings(output, expectedCurrentLimit: sitePinInfo.CascadingInfo is GangingInfo ? 0.5 : 1.5);
            });
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageSequenceWithSamePerPinPerSiteSequence_CorrectValueAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            sessionsBundle.ForceVoltageSequence(sequence, DefaultTimeout, 1.8, null, null, 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertVoltageSettings(sessionInfo.AllChannelsOutput, expectedCurrentLimit: 0.6));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceVoltageSequenceWithDifferentPerPinPerSiteSequence_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 1.0, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            void ForceVoltageTest()
            {
                sessionsBundle.ForceVoltageSequence(sequence, DefaultTimeout, 1.8, null, null, 2);
            }

            sessionsBundle.Abort();
            var exception = Assert.Throws<AggregateException>(ForceVoltageTest);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different sequences for cascaded pins", exception.InnerException.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceSucceeds(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequence = new[] { 0.000, 0.005, 0.010 };

            sessionsBundle.ForceCurrentSequence(sequence, DefaultTimeout);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { -0.02, 0.03, 0.04, 0.05, 0.07 };
            sessionsBundle.ForceCurrentSequence(sequence, DefaultTimeout, voltageLimit: 0.5, currentLevelRange: 0.1, voltageLimitRange: 1, sequenceLoopCount: 1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedVoltageLimit: 0.5, expectedSequenceLoopCount: 1, expectedCurrentLevelRange: 0.1, expectedVoltageLimitRange: 1));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceWithPerSiteSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new double[][]
            {
                new[] { -0.005, 0.010 },
                new[] { -0.006, 0.012 },
                new[] { 0.007, 0.014 },
                new[] { 0.008, 0.016 }
            });
            sessionsBundle.ForceCurrentSequence(sequence, DefaultTimeout, voltageLimit: 0.5, currentLevelRange: 0.1, voltageLimitRange: 1, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedCurrentLevelRange: 0.1, expectedVoltageLimit: 0.5, expectedVoltageLimitRange: 1, expectedSequenceLoopCount: 2));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ForceCurrentSequenceWithPerPinPerSiteSequence_CorrectValuesAreSet(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>() { [0] = new[] { 0.005, 0.010 }, [1] = new[] { 0.006, 0.012 }, [2] = new[] { 0.007, 0.014 }, [3] = new[] { 0.008, 0.016 } }
            });
            sessionsBundle.ForceCurrentSequence(sequence, DefaultTimeout, voltageLimit: 0.5, currentLevelRange: 0.1, voltageLimitRange: 1, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedCurrentLevelRange: 0.1, expectedVoltageLimit: 0.5, expectedVoltageLimitRange: 1, expectedSequenceLoopCount: 2));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentSequence_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new[] { -1.0, 1.0, 1.5, 1.7, 2.0 };
            sessionsBundle.ForceCurrentSequence(sequence, DefaultTimeout, voltageLimit: 0.5, currentLevelRange: 2.5, voltageLimitRange: 1, sequenceLoopCount: 1);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedVoltageLimit: 0.5, expectedSequenceLoopCount: 1));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentSequenceWithPerSiteSequence_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new double[][]
            {
                new[] { -1.1, 0.5, 1.1, 1.7, 2.3 },
                new[] { -1.5, -1, 0.5, 1, 1.5 }
            });
            sessionsBundle.ForceCurrentSequence(sequence, DefaultTimeout, voltageLimit: 0.5, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedVoltageLimit: 0.5, expectedSequenceLoopCount: 2));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentSequenceWithPerPinPerSiteSequence_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { -0.6, 0.9 }, [1] = new[] { -0.2, 0.6 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { -0.6, 0.9 }, [1] = new[] { -0.2, 0.6 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { -0.6, 0.9 }, [1] = new[] { -0.2, 0.6 } }
            });
            sessionsBundle.ForceCurrentSequence(sequence, DefaultTimeout, voltageLimit: 0.5, currentLevelRange: 2, voltageLimitRange: 1, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedVoltageLimit: 0.5, expectedSequenceLoopCount: 2));
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ForceCurrentSequenceOnPinGroupWithPerPinPerSiteSequence_CorrectValuesAreSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var pins = _tsmContext.GetPinsInPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["ThreePinsGangedGroup"] = new Dictionary<int, double[]>() { [0] = new[] { -1.5, 0.9 }, [1] = new[] { -1.2, 0.6 } }
            });
            sessionsBundle.ForceCurrentSequence(sequence, DefaultTimeout, voltageLimit: 0.5, currentLevelRange: 2, voltageLimitRange: 1, sequenceLoopCount: 2);

            sessionsBundle.Abort();
            sessionsBundle.Do(sessionInfo => AssertCurrentSettings(sessionInfo.AllChannelsOutput, expectedVoltageLimit: 0.5, expectedSequenceLoopCount: 2));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetVoltageSequence_CorrectVoltageMeasurementsFetchedClearAndDeleteAdvancedSequence(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequence = new double[] { 1, 2, 3, 4, 5 };
            // Cannot test sequence loop count because it's not supported in offline mode.
            sessionsBundle.ConfigureVoltageSequence(sequenceName, expectedSequence, sequenceLoopCount: 1, setAsActiveSequence: true);
            var results = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                output.Control.Initiate();
                // Cannot test fetch backlog because it's always 1 in offline mode.
                return sessionInfo.Session.Measurement.Fetch(sitePinInfo.IndividualChannelString, PrecisionTimeSpan.FromSeconds(1), 5);
            });

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                Assert.Equal(expectedSequence, results[0][0].VoltageMeasurements);
                Assert.Equal(expectedSequence, results[0][1].VoltageMeasurements);
                Assert.Equal(expectedSequence, results[0][2].VoltageMeasurements);
                Assert.Equal(expectedSequence, results[0][3].VoltageMeasurements);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetCurrentSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "CurrentSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequence = new double[] { 0.5, 1, 1.5, 2, 2.5 };
            // Cannot test sequence loop count because it's not supported in offline mode.
            sessionsBundle.ConfigureCurrentSequence(sequenceName, expectedSequence, sequenceLoopCount: 1, setAsActiveSequence: true);
            var results = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                output.Control.Initiate();
                // Cannot test fetch backlog because it's always 1 in offline mode.
                return sessionInfo.Session.Measurement.Fetch(sitePinInfo.IndividualChannelString, PrecisionTimeSpan.FromSeconds(1), 5);
            });

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                Assert.Equal(expectedSequence, results[0][0].CurrentMeasurements);
                Assert.Equal(expectedSequence, results[0][1].CurrentMeasurements);
                Assert.Equal(expectedSequence, results[0][2].CurrentMeasurements);
                Assert.Equal(expectedSequence, results[0][3].CurrentMeasurements);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequenceWithUpdateMode";
            var expectedSequence = new double[] { 1, 2, 3, 4, 5 };

            sessionsBundle.ConfigureVoltageSequence(sequenceName, expectedSequence, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSiteDataAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequenceWithSiteDataAndUpdateMode";
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                new[] { 2.0, 3.0, 4.0, 5.0, 6.0 },
                new[] { 2.5, 3.5, 4.5, 5.5, 6.5 }
            });

            sessionsBundle.ConfigureVoltageSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithPinSiteDataAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequenceWithPinSiteDataAndUpdateMode";
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                    [1] = new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                    [2] = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 },
                    [3] = new[] { 2.5, 3.5, 4.5, 5.5, 6.5 }
                }
            });

            sessionsBundle.ConfigureVoltageSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "CurrentSequenceWithUpdateMode";
            var expectedSequence = new double[] { 0.5, 1, 1.5, 2, 2.5 };

            sessionsBundle.ConfigureCurrentSequence(sequenceName, expectedSequence, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSiteDataAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "CurrentSequenceWithSiteDataAndUpdateMode";
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 0.5, 1.0, 1.5, 2.0, 2.5 },
                new[] { 0.6, 1.1, 1.6, 2.1, 2.6 },
                new[] { 0.7, 1.2, 1.7, 2.2, 2.7 },
                new[] { 0.8, 1.3, 1.8, 2.3, 2.8 }
            });

            sessionsBundle.ConfigureCurrentSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithPinSiteDataAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "CurrentSequenceWithPinSiteDataAndUpdateMode";
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.5, 1.0, 1.5, 2.0, 2.5 },
                    [1] = new[] { 0.6, 1.1, 1.6, 2.1, 2.6 },
                    [2] = new[] { 0.7, 1.2, 1.7, 2.2, 2.7 },
                    [3] = new[] { 0.8, 1.3, 1.8, 2.3, 2.8 }
                }
            });

            sessionsBundle.ConfigureCurrentSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureAdvancedSequenceWithUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName = "AdvancedSequenceWithUpdateMode";
            var stepProperties = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };

            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureAdvancedSequenceWithSiteDataAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName = "AdvancedSequenceWithSiteDataAndUpdateMode";
            var stepProperties = new SiteData<IList<DCPowerAdvancedSequenceStepProperties>>(new IList<DCPowerAdvancedSequenceStepProperties>[]
            {
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                },
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 4.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                },
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 5.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 6.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                },
                new List<DCPowerAdvancedSequenceStepProperties>
                {
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 7.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                    new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 8.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
                }
            });

            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureAdvancedSequenceWithPinSiteDataAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            CreateDCPowerAdvancedSequencePropertyMappingsCache();
            string sequenceName = "AdvancedSequenceWithPinSiteDataAndUpdateMode";
            IList<DCPowerAdvancedSequenceStepProperties> CreateSteps(double startLevel) => new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = startLevel, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = startLevel + 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            var stepProperties = new PinSiteData<IList<DCPowerAdvancedSequenceStepProperties>>(new Dictionary<string, IDictionary<int, IList<DCPowerAdvancedSequenceStepProperties>>>()
            {
                ["VDD"] = new Dictionary<int, IList<DCPowerAdvancedSequenceStepProperties>>()
                {
                    [0] = CreateSteps(1.0),
                    [1] = CreateSteps(2.0),
                    [2] = CreateSteps(3.0),
                    [3] = CreateSteps(4.0)
                }
            });

            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSourceDelaysAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequenceWithSourceDelaysAndUpdateMode";
            var expectedSequence = new double[] { 1, 2, 3 };
            var sourceDelays = new double[] { 0.01, 0.02, 0.03 };

            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays(sequenceName, expectedSequence, sourceDelays, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSourceDelaysAndSiteDataAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequenceWithSourceDelaysAndSiteDataAndUpdateMode";
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0 },
                new[] { 1.5, 2.5, 3.5 },
                new[] { 2.0, 3.0, 4.0 },
                new[] { 2.5, 3.5, 4.5 }
            });
            var sourceDelays = new SiteData<double[]>(new double[][]
            {
                new[] { 0.01, 0.02, 0.03 },
                new[] { 0.01, 0.02, 0.03 },
                new[] { 0.01, 0.02, 0.03 },
                new[] { 0.01, 0.02, 0.03 }
            });

            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays(sequenceName, expectedSequences, sourceDelays, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSourceDelaysAndPinSiteDataAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequenceWithSourceDelaysAndPinSiteDataAndUpdateMode";
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 1.0, 2.0, 3.0 },
                    [1] = new[] { 1.5, 2.5, 3.5 },
                    [2] = new[] { 2.0, 3.0, 4.0 },
                    [3] = new[] { 2.5, 3.5, 4.5 }
                }
            });
            var sourceDelays = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.01, 0.02, 0.03 },
                    [1] = new[] { 0.01, 0.02, 0.03 },
                    [2] = new[] { 0.01, 0.02, 0.03 },
                    [3] = new[] { 0.01, 0.02, 0.03 }
                }
            });

            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays(sequenceName, expectedSequences, sourceDelays, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSourceDelaysAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "CurrentSequenceWithSourceDelaysAndUpdateMode";
            var expectedSequence = new double[] { 0.5, 1, 1.5 };
            var sourceDelays = new double[] { 0.01, 0.02, 0.03 };

            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays(sequenceName, expectedSequence, sourceDelays, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSourceDelaysAndSiteDataAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "CurrentSequenceWithSourceDelaysAndSiteDataAndUpdateMode";
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 0.5, 1.0, 1.5 },
                new[] { 0.6, 1.1, 1.6 },
                new[] { 0.7, 1.2, 1.7 },
                new[] { 0.8, 1.3, 1.8 }
            });
            var sourceDelays = new SiteData<double[]>(new double[][]
            {
                new[] { 0.01, 0.02, 0.03 },
                new[] { 0.01, 0.02, 0.03 },
                new[] { 0.01, 0.02, 0.03 },
                new[] { 0.01, 0.02, 0.03 }
            });

            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays(sequenceName, expectedSequences, sourceDelays, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(UpdateMode.Deferred)]
        [InlineData(UpdateMode.Commit)]
        [InlineData(UpdateMode.Immediate)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSourceDelaysAndPinSiteDataAndUpdateMode_UpdateModeSetCorrectly(UpdateMode updateMode)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup: false);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "CurrentSequenceWithSourceDelaysAndPinSiteDataAndUpdateMode";
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.5, 1.0, 1.5 },
                    [1] = new[] { 0.6, 1.1, 1.6 },
                    [2] = new[] { 0.7, 1.2, 1.7 },
                    [3] = new[] { 0.8, 1.3, 1.8 }
                }
            });
            var sourceDelays = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.01, 0.02, 0.03 },
                    [1] = new[] { 0.01, 0.02, 0.03 },
                    [2] = new[] { 0.01, 0.02, 0.03 },
                    [3] = new[] { 0.01, 0.02, 0.03 }
                }
            });

            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays(sequenceName, expectedSequences, sourceDelays, sequenceLoopCount: 1, setAsActiveSequence: true, updateMode: updateMode);

            AssertInitiateBehaviorMatchesUpdateMode(sessionsBundle, updateMode);
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_SetVoltageSequenceWithStepDeltaTime_SequenceStepDeltaTimeEnabled(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequenceWithDeltaTime";

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.False(sessionInfo.AllChannelsOutput.Source.SequenceStepDeltaTimeEnabled);
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence: new double[] { 1, 2, 3 }, sequenceLoopCount: 1, sequenceStepDeltaTimeInSeconds: 0.05, setAsActiveSequence: true);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.True(sessionInfo.AllChannelsOutput.Source.SequenceStepDeltaTimeEnabled);
                if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
                {
                    Assert.Equal(0.05, sessionInfo.AllChannelsOutput.Source.SequenceStepDeltaTime.TotalSeconds);
                }
            });
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureAdvanceSequence_ClearAdvancedSequences_ActiveSequencesCleared(bool pinMapWithChannelGroup)
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
            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true);

            sessionsBundle.ClearActiveAdvancedSequence();

            var ex = Assert.Throws<NISemiconductorTestException>(() =>
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    var activeSequenceName = sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence;
                });
            });
            Assert.Contains("A sequence must have at least one step.", ex.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureAdvanceSequence_DeleteAdvancedSequences_SequenceDeletedSuccessfully(bool pinMapWithChannelGroup)
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
            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true);

            sessionsBundle.DeleteAdvancedSequence(sequenceName);

            var ex = Assert.Throws<NISemiconductorTestException>(() =>
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = sequenceName;
                });
            });
            Assert.Contains("The active advanced sequence does not exist.", ex.Message);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureAdvanceSequence_ClearThenDeleteAdvancedSequence_SequenceDeletedSuccessfully(bool pinMapWithChannelGroup)
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
            sessionsBundle.ConfigureAdvancedSequence(sequenceName, stepProperties, setAsActiveSequence: true);

            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);

            var ex = Assert.Throws<NISemiconductorTestException>(() =>
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = sequenceName;
                });
            });
            Assert.Contains("The active advanced sequence does not exist.", ex.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevicesAndConfigureMultipleAdvanceSequenceAsInactive_DeleteMultipleAdvancedSequences_AllSequencesDeleted(bool pinMapWithChannelGroup)
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

            sessionsBundle.DeleteAdvancedSequence(sequenceName1, sequenceName2);

            var ex1 = Assert.Throws<NISemiconductorTestException>(() =>
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = sequenceName1;
                });
            });
            var ex2 = Assert.Throws<NISemiconductorTestException>(() =>
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = sequenceName1;
                });
            });
            Assert.Contains("The active advanced sequence does not exist.", ex1.Message);
            Assert.Contains("The active advanced sequence does not exist.", ex2.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequence_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "VoltageSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 1, setAsActiveSequence: true);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSiteData_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            string sequenceName = "VoltageSequenceWithSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                new[] { 2.0, 3.0, 4.0, 5.0, 6.0 },
                new[] { 2.5, 3.5, 4.5, 5.5, 6.5 }
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber), precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithPinSiteData_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "VoltageSequenceWithPinSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                    [1] = new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                    [2] = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 },
                    [3] = new[] { 2.5, 3.5, 4.5, 5.5, 6.5 }
                }
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true);

            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => expectedSequences.GetValue(siteNumber, pinName), precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequence_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceTest";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 2, itemsToFetch: 5, checkForCurrentMeasurement: false);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithPerSiteSequence_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceWithSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => sequence.GetValue(siteNumber), precision: 2, itemsToFetch: 5, checkForCurrentMeasurement: false);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithPerPinPerSiteSequenceOnPinGroupName_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceWithPinSiteDataOnPinGroupName";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                [ThreePinsGangedGroup] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            if (!_tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName), precision: 2, itemsToFetch: 2, checkForCurrentMeasurement: false, groupName: ThreePinsGangedGroup);
            }
            sessionsBundle.ClearActiveAdvancedSequence();
            sessionsBundle.DeleteAdvancedSequence(sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithPerPinPerSiteSequence_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceWithPinSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();

            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName), precision: 2, itemsToFetch: 2, checkForCurrentMeasurement: false), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithDifferentPerPinPerSiteSequence_ThrowsException()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceWithDifferentPerPinPerSiteSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 1.0, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            void ConfigureVoltageSequenceTest()
            {
                sessionsBundle.ConfigureVoltageSequence(sequenceName, sequence, sequenceLoopCount: 2);
            }

            sessionsBundle.Abort();
            var exception = Assert.Throws<AggregateException>(ConfigureVoltageSequenceTest);
            Assert.IsType<NISemiconductorTestException>(exception.InnerException);
            Assert.Contains("The parameter contains different sequences for cascaded pins", exception.InnerException.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "CurrentSequenceTest";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 0.2, 0.4, 0.6, 0.8 };
            sessionsBundle.ConfigureCurrentSequence(sequenceName, sequence, sequenceLoopCount: 1, setAsActiveSequence: true);

            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 1, itemsToFetch: 4), sessionsBundle, sequenceName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSiteData_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "CurrentSequenceWithSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 0.2, 0.3, 0.4, 0.5 },
                new[] { 0.3, 0.4, 0.5, 0.6 },
                new[] { 0.4, 0.5, 0.6, 0.7 },
                new[] { 0.5, 0.6, 0.7, 0.8 }
            });
            sessionsBundle.ConfigureCurrentSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true);

            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber), precision: 1, itemsToFetch: 4), sessionsBundle, sequenceName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithPinSiteData_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "CurrentSequenceWithPinSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.2, 0.3, 0.4, 0.5 },
                    [1] = new[] { 0.3, 0.4, 0.5, 0.6 },
                    [2] = new[] { 0.4, 0.5, 0.6, 0.7 },
                    [3] = new[] { 0.5, 0.6, 0.7, 0.8 }
                }
            });
            sessionsBundle.ConfigureCurrentSequence(sequenceName, expectedSequences, sequenceLoopCount: 1, setAsActiveSequence: true);

            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => expectedSequences.GetValue(siteNumber, pinName), precision: 1, itemsToFetch: 4), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequence_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceTest";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 0.2, 0.4, 0.6, 0.8, 1.0 };
            sessionsBundle.ConfigureCurrentSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence.Select(value => value / 3).ToArray(), precision: 2, itemsToFetch: 5), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithPerSiteSequence_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceWithSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 0.2, 0.3, 0.4, 0.5 },
                new[] { 0.3, 0.4, 0.5, 0.6 },
                new[] { 0.4, 0.5, 0.6, 0.7 },
                new[] { 0.5, 0.6, 0.7, 0.8 }
            });
            sessionsBundle.ConfigureCurrentSequence(sequenceName, expectedSequences, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber).Select(value => value / 3).ToArray(), precision: 2, itemsToFetch: 4), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithPerPinPerSiteSequenceOnPinGroupName_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceWithPinSiteDataOnPinGroupName";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                [ThreePinsGangedGroup] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            sessionsBundle.ConfigureCurrentSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName).Select(value => value / 3).ToArray(), precision: 2, itemsToFetch: 2, groupName: ThreePinsGangedGroup), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithPerPinPerSiteSequence_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceWithPinSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 0.5, 0.7 }, [1] = new[] { 0.8, 0.9 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 0.4, 0.7 }, [1] = new[] { 0.8, 0.9 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 0.5, 0.7 }, [1] = new[] { 0.8, 0.9 } }
            });
            sessionsBundle.ConfigureCurrentSequence(sequenceName, sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName), precision: 2, itemsToFetch: 2), sessionsBundle, sequenceName);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSourceDelays_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "VoltageSequenceWithSourceDelays";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var sourceDelayInSeconds = new double[] { 0.05, 0.05, 0.05, 0.05, 0.05 };
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays(sequenceName, sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 2, itemsToFetch: 5, checkForCurrentMeasurement: false), sessionsBundle, sequenceName);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSourceDelaysAndPerSiteSequence_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "VoltageSequenceWithSourceDelaysAndSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                new[] { 2.0, 2.5, 3.0, 3.5, 4.0 },
                new[] { 2.5, 3.0, 3.5, 4.0, 4.5 }
            });
            var expectedSourceDelayInSeconds = new SiteData<double[]>(new double[][]
            {
                new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                new[] { 0.05, 0.05, 0.05, 0.05, 0.05 }
            });
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays(sequenceName, expectedSequences, expectedSourceDelayInSeconds, sequenceLoopCount: 1, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber), precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false), sessionsBundle, sequenceName);
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureVoltageSequenceWithSourceDelaysAndPerPinPerSiteSequence_CorrectVoltageMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");
            var sequenceName = "VoltageSequenceWithSourceDelaysAndPinSiteData";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                    [1] = new[] { 1.5, 2.5, 3.5, 4.5, 5.5 },
                    [2] = new[] { 2.0, 2.5, 3.0, 3.5, 4.0 },
                    [3] = new[] { 2.5, 3.0, 3.5, 4.0, 4.5 }
                }
            });
            var expectedSourceDelayInSeconds = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                    [1] = new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                    [2] = new[] { 0.05, 0.05, 0.05, 0.05, 0.05 },
                    [3] = new[] { 0.05, 0.05, 0.05, 0.05, 0.05 }
                }
            });
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays(sequenceName, expectedSequences, expectedSourceDelayInSeconds, sequenceLoopCount: 1, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => expectedSequences.GetValue(siteNumber, pinName), precision: 1, itemsToFetch: 5, checkForCurrentMeasurement: false), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithSourceDelays_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "VoltageSequenceWithSourceDelays";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var sourceDelayInSeconds = new double[] { 3.0, 3.0, 3.0, 3.0, 3.0 };
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays(sequenceName, sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 2, itemsToFetch: 5, checkForCurrentMeasurement: false), sessionsBundle, sequenceName);
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithSourceDelaysAndPerSiteSequence_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new SiteData<double[]>(new double[][]
            {
                new[] { 1.0, 2.0, 3.0, 4.0, 5.0 },
                new[] { 0.5, 1.5, 2.5, 3.5, 4.5 },
            });
            var sourceDelayInSeconds = new SiteData<double[]>(new double[][]
            {
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 }
            });
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays("VoltageSequenceWithSourceDelays", sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => sequence.GetValue(siteNumber), precision: 2, itemsToFetch: 5, checkForCurrentMeasurement: false), sessionsBundle, "VoltageSequenceWithSourceDelays");
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureVoltageSequenceWithSourceDelaysAndPerPinPerSiteSequence_CorrectVoltageMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 1.5, 2.1 }, [1] = new[] { 0.8, 1.4 } }
            });
            var sourceDelayInSeconds = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } }
            });
            sessionsBundle.ConfigureVoltageSequenceWithSourceDelays("VoltageSequenceWithSourceDelaysAndPerSiteSequence", sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName), precision: 2, itemsToFetch: 2, checkForCurrentMeasurement: false), sessionsBundle, "VoltageSequenceWithSourceDelaysAndPerSiteSequence");
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureSequence_CorrectTriggersSet()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(AllPinsGangedGroup);
            sessionsBundle.GangPinGroup(AllPinsGangedGroup);

            var sequence = new double[] { 0.2, 0.4, 0.6, 0.8, 1.0 };
            sessionsBundle.ConfigureVoltageSequence("VoltageSequence", sequence, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                AssertTriggerSettings(sitePinInfo, output, sitePinInfo.SiteNumber == 0 ? "SMU_4137_C5_S02/0" : "SMU_4137_C5_S03/0", checkSequenceRelatedTriggers: true);
            });
            sessionsBundle.UngangPinGroup(AllPinsGangedGroup);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSourceDelays_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 0.2, 0.4, 0.6, 0.8, 1.0 };
            var sourceDelayInSeconds = new double[] { 2.0, 2.0, 2.0, 2.0, 2.0 };
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays("CurrentSequenceWithSourceDelays", sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence, precision: 2, itemsToFetch: 5), sessionsBundle, "CurrentSequenceWithSourceDelays");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSourceDelaysAndPerSiteSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 0.2, 0.3, 0.4, 0.5 },
                new[] { 0.3, 0.4, 0.5, 0.6 },
                new[] { 0.4, 0.5, 0.6, 0.7 },
                new[] { 0.5, 0.6, 0.7, 0.8 }
            });
            var expectedSourceDelayInSeconds = new SiteData<double[]>(new double[][]
            {
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 }
            });
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays("CurrentSequenceWithSourceDelaysAndPerSiteSequence", expectedSequences, expectedSourceDelayInSeconds, sequenceLoopCount: 1, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber), precision: 1, itemsToFetch: 4), sessionsBundle, "CurrentSequenceWithSourceDelaysAndPerSiteSequence");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSMUDevices_ConfigureCurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence_CorrectCurrentMeasurementsFetched(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var sessionsBundle = sessionManager.DCPower("VDD");

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 0.2, 0.3, 0.4, 0.5 },
                    [1] = new[] { 0.3, 0.4, 0.5, 0.6 },
                    [2] = new[] { 0.4, 0.5, 0.6, 0.7 },
                    [3] = new[] { 0.5, 0.6, 0.7, 0.8 }
                }
            });
            var expectedSourceDelayInSeconds = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VDD"] = new Dictionary<int, double[]>()
                {
                    [0] = new[] { 1.0, 1.0, 1.0, 1.0 },
                    [1] = new[] { 1.0, 1.0, 1.0, 1.0 },
                    [2] = new[] { 1.0, 1.0, 1.0, 1.0 },
                    [3] = new[] { 1.0, 1.0, 1.0, 1.0 }
                }
            });
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays("CurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence", expectedSequences, expectedSourceDelayInSeconds, sequenceLoopCount: 1, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => expectedSequences.GetValue(siteNumber, pinName), precision: 1, itemsToFetch: 4), sessionsBundle, "CurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence");
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithSourceDelays_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new double[] { 0.2, 0.4, 0.6, 0.8, 1.0 };
            var sourceDelayInSeconds = new double[] { 4.0, 4.0, 4.0, 4.0, 4.0 };
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays("CurrentSequenceWithSourceDelaysGanged", sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (_, __) => sequence.Select(value => value / 3).ToArray(), precision: 2, itemsToFetch: 5), sessionsBundle, "CurrentSequenceWithSourceDelaysGanged");
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithSourceDelaysAndPerSiteSequence_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceWithSourceDelaysAndPerSiteSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var expectedSequences = new SiteData<double[]>(new double[][]
            {
                new[] { 0.2, 0.3, 0.4, 0.5 },
                new[] { 0.3, 0.4, 0.5, 0.6 },
                new[] { 0.4, 0.5, 0.6, 0.7 },
                new[] { 0.5, 0.6, 0.7, 0.8 }
            });
            var expectedSourceDelayInSeconds = new SiteData<double[]>(new double[][]
            {
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 },
                new[] { 5.0, 5.0, 5.0, 5.0, 5.0 }
            });
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays(sequenceName, expectedSequences, expectedSourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, _) => expectedSequences.GetValue(siteNumber).Select(value => value / 3).ToArray(), precision: 2, itemsToFetch: 4), sessionsBundle, "CurrentSequenceWithSourceDelaysAndPerSiteSequence");
        }

        [Fact]
        public void DifferentSMUDevicesGanged_ConfigureCurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence_CorrectCurrentMeasurementsFetched()
        {
            var sessionManager = Initialize("SMUGangPinGroup_SessionPerChannel.pinmap");
            var sessionsBundle = sessionManager.DCPower(ThreePinsGangedGroup);
            sessionsBundle.GangPinGroup(ThreePinsGangedGroup);
            var sequenceName = "CurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence";

            sessionsBundle.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);
            var sequence = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 0.3, 0.5 }, [1] = new[] { 0.4, 0.6 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 0.3, 0.5 }, [1] = new[] { 0.4, 0.6 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 0.3, 0.5 }, [1] = new[] { 0.4, 0.6 } }
            });
            var sourceDelayInSeconds = new PinSiteData<double[]>(new Dictionary<string, IDictionary<int, double[]>>()
            {
                ["VCC1"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } },
                ["VCC2"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } },
                ["VCC3"] = new Dictionary<int, double[]>() { [0] = new[] { 2.0, 2.0 }, [1] = new[] { 2.0, 2.0 } }
            });
            sessionsBundle.ConfigureCurrentSequenceWithSourceDelays(sequenceName, sequence, sourceDelayInSeconds, sequenceLoopCount: 2, setAsActiveSequence: true);

            sessionsBundle.Abort();
            Assert_ClearAndDeleteConfigureAdvancedSequences(() => AssertSequenceMeasurementsMatchExpected(sessionsBundle, (siteNumber, pinName) => sequence.GetValue(siteNumber, pinName), precision: 2, itemsToFetch: 2), sessionsBundle, "CurrentSequenceWithSourceDelaysAndPerPinPerSiteSequence");
        }

        [Fact(Skip="WorkFlow not supported with API changes. Need to modify the test for Timeout Exception")]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void GangedSMUBundle_ForceVoltageSequenceWithoutWaitForSequenceCompletion_ReturnsImmediately()
        {
            var sessionsBundle = GetGangedSMUBundle();
            sessionsBundle.ConfigureSourceDelay(0.1);
            var voltageSequence = new[] { 1.8, 3.3, 5.0 };
            var startTime = DateTime.Now;

            sessionsBundle.ForceVoltageSequence(
                voltageSequence,
                0.005,
                currentLimit: 0.05,
                sequenceLoopCount: 1);
            var elapsedTime = (DateTime.Now - startTime).TotalMilliseconds;

            // Should return quickly without waiting
            Assert.True(elapsedTime < 100, $"Expected quick return, but took {elapsedTime}ms which is more than source delay 100ms");
        }
    }
}
