using System;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DCPower
{
    [Collection("NonParallelizable")]
    public sealed class TriggerAndEventsTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(bool pinMapWithChannelGroup)
        {
            string pinMapFileName = pinMapWithChannelGroup ? "DifferentSMUDevicesWithChannelGroup.pinmap" : "DifferentSMUDevices.pinmap";
            _tsmContext = CreateTSMContext(pinMapFileName);
            InitializeAndClose.Initialize(_tsmContext);
            return new TSMSessionManager(_tsmContext);
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
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void ConfigureTrigger_PulseTrigger_DigitalEdgeAndDisable(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VDET" });
            var triggerLine = "PXI_Trig0";

            // Setup Source Settings
            var settings = new DCPowerSourceSettings()
            {
                Level = 1.0,
                Limit = 0.1,
                OutputFunction = DCPowerSourceOutputFunction.PulseVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
            };
            sessionsBundle.ConfigureSourceSettings(settings);

            // Test Digital Edge Trigger - Raising (default)
            sessionsBundle.ConfigureTriggerDigitalEdge(TriggerType.PulseTrigger, triggerLine);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var inputTerminal = $"/{sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Name.Split('/')[0]}/{triggerLine}";
                AssertPulseTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerPulseTriggerType.DigitalEdge, inputTerminal);
            });

            // Test Clear Trigger
            sessionsBundle.DisableTriggers();
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertPulseTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerPulseTriggerType.None);
            });

            // Test Digital Edge Trigger - Falling (Cannot get this to work. May not be supported?)
            // sessionsBundle.DisableTriggers();
            // sessionsBundle.ConfigureTriggerDigitalEdge(TriggerType.PulseTrigger, triggerLine, DCPowerTriggerEdge.Falling);

            // sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            // {
            //     var inputTerminal = $"/{sessionInfo.Session.Outputs[sitePinInfo.InstrumentChannelString].Name.Split('/')[0]}/{triggerLine}";
            //     AssertPulseTriggerSettings(sessionInfo, sitePinInfo.InstrumentChannelString, DCPowerPulseTriggerType.DigitalEdge, inputTerminal, DCPowerTriggerEdge.Falling);
            // });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void ConfigureTrigger_PulseTrigger_SoftwarelEdgeAndDisable(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VDET" });

            // Setup Source Settings
            var settings = new DCPowerSourceSettings()
            {
                Level = 1.0,
                Limit = 0.1,
                OutputFunction = DCPowerSourceOutputFunction.PulseVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
            };
            sessionsBundle.ConfigureSourceSettings(settings);

            // Test Software Edge Trigger
            sessionsBundle.ConfigureTriggerSoftwareEdge(TriggerType.PulseTrigger);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertPulseTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerPulseTriggerType.SoftwareEdge);
            });

            // Test Clear Trigger
            sessionsBundle.DisableTriggers();
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertPulseTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerPulseTriggerType.None);
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void ConfigureTrigger_StartTrigger_DigitalEdgeAndDisable(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VDET" });
            var triggerLine = "PXI_Trig0";

            // Setup Source Settings
            var settings = new DCPowerSourceSettings()
            {
                Level = 1.0,
                Limit = 0.1,
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
            };
            sessionsBundle.ConfigureSourceSettings(settings);
            sessionsBundle.ConfigureSequence(new double[] { 0, .1, .2, .3 }, 1);

            // Test Digital Edge Trigger - Raising (default)
            sessionsBundle.ConfigureTriggerDigitalEdge(TriggerType.StartTrigger, triggerLine);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var inputTerminal = $"/{sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Name.Split('/')[0]}/{triggerLine}";
                AssertStartTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerStartTriggerType.DigitalEdge, inputTerminal);
            });

            // Test Clear Trigger
            sessionsBundle.DisableTriggers();
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertStartTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerStartTriggerType.None);
            });

            // Test Digital Edge Trigger - Falling (Cannot get this to work. May not be supported?)
            // sessionsBundle.DisableTriggers();
            // sessionsBundle.ConfigureTriggerDigitalEdge(TriggerType.StartTrigger, triggerLine, DCPowerTriggerEdge.Falling);

            // sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            // {
            //     var inputTerminal = $"/{sessionInfo.Session.Outputs[sitePinInfo.InstrumentChannelString].Name.Split('/')[0]}/{triggerLine}";
            //     AssertStartTriggerSettings(sessionInfo, sitePinInfo.InstrumentChannelString, DCPowerStartTriggerType.DigitalEdge, inputTerminal, DCPowerTriggerEdge.Falling);
            // });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void ConfigureTrigger_SequenceAdvanceTrigger_SoftwarelEdgeAndDisable(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VDET" });

            // Setup Source Settings
            var settings = new DCPowerSourceSettings()
            {
                Level = 1.0,
                Limit = 0.1,
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
            };
            sessionsBundle.ConfigureSourceSettings(settings);
            sessionsBundle.ConfigureSequence(new double[] { 0, .1, .2, .3 }, 1);

            // Test Software Edge Trigger
            sessionsBundle.ConfigureTriggerSoftwareEdge(TriggerType.SequenceAdvanceTrigger);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertSequenceAdvanceTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerSequenceAdvanceTriggerType.SoftwareEdge);
            });

            // Test Clear Trigger
            sessionsBundle.DisableTriggers();
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertSequenceAdvanceTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerSequenceAdvanceTriggerType.None);
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void ConfigureTrigger_SequenceAdvanceTrigger_DigitalEdgeAndDisable(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VDET" });
            var triggerLine = "PXI_Trig0";

            // Setup Source Settings
            var settings = new DCPowerSourceSettings()
            {
                Level = 1.0,
                Limit = 0.1,
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
            };
            sessionsBundle.ConfigureSourceSettings(settings);
            sessionsBundle.ConfigureSequence(new double[] { 0, .1, .2, .3 }, 1);

            // Test Digital Edge Trigger - Raising (default)
            sessionsBundle.ConfigureTriggerDigitalEdge(TriggerType.SequenceAdvanceTrigger, triggerLine);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var inputTerminal = $"/{sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Name.Split('/')[0]}/{triggerLine}";
                AssertSequenceAdvanceTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerSequenceAdvanceTriggerType.DigitalEdge, inputTerminal);
            });

            // Test Clear Trigger
            sessionsBundle.DisableTriggers();
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertSequenceAdvanceTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerSequenceAdvanceTriggerType.None);
            });

            // Test Digital Edge Trigger - Falling (Cannot get this to work. May not be supported?)
            // sessionsBundle.DisableTriggers();
            // sessionsBundle.ConfigureTriggerDigitalEdge(TriggerType.SequenceAdvanceTrigger, triggerLine, DCPowerTriggerEdge.Falling);

            // sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            // {
            //     var inputTerminal = $"/{sessionInfo.Session.Outputs[sitePinInfo.InstrumentChannelString].Name.Split('/')[0]}/{triggerLine}";
            //     AssertSequenceAdvanceTriggerSettings(sessionInfo, sitePinInfo.InstrumentChannelString, DCPowerSequenceAdvanceTriggerType.DigitalEdge, inputTerminal, DCPowerTriggerEdge.Falling);
            // });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void ConfigureTrigger_StartTrigger_SoftwarelEdgeAndDisable(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VDET" });

            // Setup Source Settings
            var settings = new DCPowerSourceSettings()
            {
                Level = 1.0,
                Limit = 0.1,
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
            };
            sessionsBundle.ConfigureSourceSettings(settings);
            sessionsBundle.ConfigureSequence(new double[] { 0, .1, .2, .3 }, 1);

            // Test Software Edge Trigger
            sessionsBundle.ConfigureTriggerSoftwareEdge(TriggerType.StartTrigger);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertStartTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerStartTriggerType.SoftwareEdge);
            });

            // Test Clear Trigger
            sessionsBundle.DisableTriggers();
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertStartTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerStartTriggerType.None);
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void ConfigureTrigger_SourceTrigger_DigitalEdgeAndDisable(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VDET" });
            var triggerLine = "PXI_Trig0";

            // Setup Source Settings
            var settings = new DCPowerSourceSettings()
            {
                Level = 1.0,
                Limit = 0.1,
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
            };
            sessionsBundle.ConfigureSourceSettings(settings);
            sessionsBundle.ConfigureSequence(new double[] { 0, .1, .2, .3 }, 1);

            // Test Digital Edge Trigger - Raising (default)
            sessionsBundle.ConfigureTriggerDigitalEdge(TriggerType.SourceTrigger, triggerLine);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var inputTerminal = $"/{sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Name.Split('/')[0]}/{triggerLine}";
                AssertSourceTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerSourceTriggerType.DigitalEdge, inputTerminal);
            });

            // Test Clear Trigger
            sessionsBundle.DisableTriggers();
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertSourceTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerSourceTriggerType.None);
            });

            // Test Digital Edge Trigger - Falling (Cannot get this to work. May not be supported?)
            // sessionsBundle.DisableTriggers();
            // sessionsBundle.ConfigureTriggerDigitalEdge(TriggerType.SourceTrigger, triggerLine, DCPowerTriggerEdge.Falling);

            // sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            // {
            //     var inputTerminal = $"/{sessionInfo.Session.Outputs[sitePinInfo.InstrumentChannelString].Name.Split('/')[0]}/{triggerLine}";
            //     AssertSourceTriggerSettings(sessionInfo, sitePinInfo.InstrumentChannelString, DCPowerSourceTriggerType.DigitalEdge, inputTerminal, DCPowerTriggerEdge.Falling);
            // });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void ConfigureTrigger_SourceTrigger_SoftwarelEdgeAndDisable(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VDET" });

            // Setup Source Settings
            var settings = new DCPowerSourceSettings()
            {
                Level = 1.0,
                Limit = 0.1,
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
            };
            sessionsBundle.ConfigureSourceSettings(settings);
            sessionsBundle.ConfigureSequence(new double[] { 0, .1, .2, .3 }, 1);

            // Test Software Edge Trigger
            sessionsBundle.ConfigureTriggerSoftwareEdge(TriggerType.SourceTrigger);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertSourceTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerSourceTriggerType.SoftwareEdge);
            });

            // Test Clear Trigger
            sessionsBundle.DisableTriggers();
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                AssertSourceTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerSourceTriggerType.None);
            });
        }

        [Theory]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void ConfigureTrigger_MeasureTrigger(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VDET" });
            var triggerLine = "PXI_Trig0";

            // Test Software Trigger
            sessionsBundle.ConfigureTriggerSoftwareEdge(TriggerType.MeasureTrigger);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Measurement.Configuration.MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger;
                sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Triggers.MeasureTrigger.Type = DCPowerMeasureTriggerType.SoftwareEdge;
                sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Commit();

                AssertMeasureTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerMeasureTriggerType.SoftwareEdge);
            });

            // Test Digital Edge Trigger - Raising (default)
            sessionsBundle.ConfigureTriggerDigitalEdge(TriggerType.MeasureTrigger, triggerLine);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var inputTerminal = $"/{sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Name.Split('/')[0]}/{triggerLine}";
                AssertMeasureTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerMeasureTriggerType.DigitalEdge, inputTerminal, DCPowerTriggerEdge.Rising);
            });

            // Test Digital Edge Trigger - Falling (Cannot get this to work. May not be supported?)
            // sessionsBundle.ConfigureTriggerDigitalEdge(TriggerType.MeasureTrigger, triggerLine, DCPowerTriggerEdge.Falling);
            // sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            // {
            //     var inputTerminal = $"/{sessionInfo.Session.Outputs[sitePinInfo.InstrumentChannelString].Name.Split('/')[0]}/{triggerLine}";
            //     AssertMeasureTriggerSettings(sessionInfo, sitePinInfo.InstrumentChannelString, DCPowerMeasureTriggerType.DigitalEdge, inputTerminal, DCPowerTriggerEdge.Falling);
            // });
        }

        [Theory]
        [InlineData(TriggerType.MeasureTrigger)]
        [InlineData(TriggerType.PulseTrigger)]
        [InlineData(TriggerType.SequenceAdvanceTrigger)]
        [InlineData(TriggerType.SourceTrigger)]
        [InlineData(TriggerType.StartTrigger)]
        public void PinMapWithMultipleSMUDevices_ConfigureTriggerSoftwareEdge_DoesNotThrowExceptionOnClearAndDisableTrigger(TriggerType triggerType)
        {
            var sessionManager = Initialize("MultipleSMUDevices.pinmap");
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET", "VEE" });

            sessionsBundle.ConfigureTriggerSoftwareEdge(triggerType);

            sessionsBundle.ClearTriggers();
            sessionsBundle.DisableTriggers();
        }

        [Theory]
        [InlineData(TriggerType.MeasureTrigger)]
        [InlineData(TriggerType.PulseTrigger)]
        [InlineData(TriggerType.SequenceAdvanceTrigger)]
        [InlineData(TriggerType.SourceTrigger)]
        [InlineData(TriggerType.StartTrigger)]
        public void PinMapWithMultipleSMUDevices_ConfigureTriggerDigitalEdge_DoesNotThrowExceptionOnClearAndDisableTrigger(TriggerType triggerType)
        {
            var sessionManager = Initialize("MultipleSMUDevices.pinmap");
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET", "VEE" });
            var triggerLine = "PXI_Trig0";

            sessionsBundle.ConfigureTriggerDigitalEdge(triggerType, triggerLine);

            sessionsBundle.ClearTriggers();
            sessionsBundle.DisableTriggers();
        }

        [Theory]
        [InlineData(TriggerType.MeasureTrigger)]
        [InlineData(TriggerType.PulseTrigger)]
        [InlineData(TriggerType.SequenceAdvanceTrigger)]
        [InlineData(TriggerType.SourceTrigger)]
        [InlineData(TriggerType.StartTrigger)]
        public void PinMapWithMultipleSMUDevicesAndInitiate_SendSoftwareEdgeTrigger_DoesNotThrowExceptionOnClearAndDisableTrigger(TriggerType triggerType)
        {
            var sessionManager = Initialize("MultipleSMUDevices.pinmap");
            var sessionsBundle = sessionManager.DCPower(new string[] { "VCC", "VDD", "VDET", "VEE" });
            if (triggerType == TriggerType.MeasureTrigger)
            {
                var settings = new DCPowerMeasureSettings()
                {
                    MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger
                };
                sessionsBundle.ConfigureMeasureSettings(settings);
            }
            sessionsBundle.Initiate();

            sessionsBundle.SendSoftwareEdgeTrigger(triggerType);

            sessionsBundle.ClearTriggers();
            sessionsBundle.DisableTriggers();
        }

        private void AssertPulseTriggerSettings(DCPowerSessionInformation sessionInfo, string channelString, DCPowerPulseTriggerType expectedType, string expectedInputTerminal = "", DCPowerTriggerEdge expectedEdge = DCPowerTriggerEdge.Rising)
        {
            Assert.Equal(expectedType, sessionInfo.Session.Outputs[channelString].Triggers.PulseTrigger.Type);
            if (expectedType == DCPowerPulseTriggerType.DigitalEdge)
            {
                Assert.Equal(expectedInputTerminal, sessionInfo.Session.Outputs[channelString].Triggers.PulseTrigger.DigitalEdge.InputTerminal);
                Assert.Equal(expectedEdge, sessionInfo.Session.Outputs[channelString].Triggers.PulseTrigger.DigitalEdge.Edge);
            }
        }

        private void AssertStartTriggerSettings(DCPowerSessionInformation sessionInfo, string channelString, DCPowerStartTriggerType expectedType, string expectedInputTerminal = "", DCPowerTriggerEdge expectedEdge = DCPowerTriggerEdge.Rising)
        {
            Assert.Equal(expectedType, sessionInfo.Session.Outputs[channelString].Triggers.StartTrigger.Type);
            if (expectedType == DCPowerStartTriggerType.DigitalEdge)
            {
                Assert.Equal(expectedInputTerminal, sessionInfo.Session.Outputs[channelString].Triggers.StartTrigger.DigitalEdge.InputTerminal);
                Assert.Equal(expectedEdge, sessionInfo.Session.Outputs[channelString].Triggers.StartTrigger.DigitalEdge.Edge);
            }
        }
        private void AssertSourceTriggerSettings(DCPowerSessionInformation sessionInfo, string channelString, DCPowerSourceTriggerType expectedType, string expectedInputTerminal = "", DCPowerTriggerEdge expectedEdge = DCPowerTriggerEdge.Rising)
        {
            Assert.Equal(expectedType, sessionInfo.Session.Outputs[channelString].Triggers.SourceTrigger.Type);
            if (expectedType == DCPowerSourceTriggerType.DigitalEdge)
            {
                Assert.Equal(expectedInputTerminal, sessionInfo.Session.Outputs[channelString].Triggers.SourceTrigger.DigitalEdge.InputTerminal);
                Assert.Equal(expectedEdge, sessionInfo.Session.Outputs[channelString].Triggers.SourceTrigger.DigitalEdge.Edge);
            }
        }
        private void AssertSequenceAdvanceTriggerSettings(DCPowerSessionInformation sessionInfo, string channelString, DCPowerSequenceAdvanceTriggerType expectedType, string expectedInputTerminal = "", DCPowerTriggerEdge expectedEdge = DCPowerTriggerEdge.Rising)
        {
            Assert.Equal(expectedType, sessionInfo.Session.Outputs[channelString].Triggers.SequenceAdvanceTrigger.Type);
            if (expectedType == DCPowerSequenceAdvanceTriggerType.DigitalEdge)
            {
                Assert.Equal(expectedInputTerminal, sessionInfo.Session.Outputs[channelString].Triggers.SequenceAdvanceTrigger.DigitalEdge.InputTerminal);
                Assert.Equal(expectedEdge, sessionInfo.Session.Outputs[channelString].Triggers.SequenceAdvanceTrigger.DigitalEdge.Edge);
            }
        }
        private void AssertMeasureTriggerSettings(DCPowerSessionInformation sessionInfo, string channelString, DCPowerMeasureTriggerType expectedType, string expectedInputTerminal = "", DCPowerTriggerEdge expectedEdge = DCPowerTriggerEdge.Rising)
        {
            Assert.Equal(expectedType, sessionInfo.Session.Outputs[channelString].Triggers.MeasureTrigger.Type);
            if (expectedType == DCPowerMeasureTriggerType.DigitalEdge)
            {
                Assert.Equal(expectedInputTerminal, sessionInfo.Session.Outputs[channelString].Triggers.MeasureTrigger.DigitalEdge.InputTerminal);
                Assert.Equal(expectedEdge, sessionInfo.Session.Outputs[channelString].Triggers.MeasureTrigger.DigitalEdge.Edge);
            }
        }
    }
}
