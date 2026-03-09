using System;
using System.Collections.Generic;
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
        public void SourceSettingsAndSequenceConfigured_ConfigureAndDisablePulseTriggerDigitalEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
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
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void SourceSettingsAndSequenceConfigured_ConfigureAndDisablePulseTriggerSOftwareEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
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
        public void SourceSettingsAndSequenceConfigured_ConfigureAndDisableStartTriggerDigitalEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
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
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void SourceSettingsAndSequenceConfigured_ConfigureAndDisableSequenceAdvanceTriggerSoftwareEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
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
        public void SourceSettingsAndSequenceConfigured_ConfigureAndDisableSequenceAdvanceTriggerDigitalEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
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
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void SourceSettingsAndSequenceConfigured_ConfigureStartAndPulseTriggerAndDisableStartTrigger_TriggerConfiguredAndOnlySoftwareTriggerIsDisabled(string pinMapFileName)
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

            sessionsBundle.ConfigureTriggerSoftwareEdge(TriggerType.StartTrigger);
            sessionsBundle.ConfigureTriggerSoftwareEdge(TriggerType.SourceTrigger);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceTriggerType.SoftwareEdge, output.Triggers.SourceTrigger.Type);
                Assert.Equal(DCPowerStartTriggerType.SoftwareEdge, output.Triggers.StartTrigger.Type);
            });
            // Test Clear Trigger
            sessionsBundle.DisableTriggers(new List<TriggerType> { TriggerType.StartTrigger });
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                Assert.Equal(DCPowerSourceTriggerType.SoftwareEdge, output.Triggers.SourceTrigger.Type);
                Assert.Equal(DCPowerStartTriggerType.None, output.Triggers.StartTrigger.Type);
            });
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void SourceSettingsAndSequenceConfigured_ConfigureAndDisableSourceTriggerDigitalEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
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
        }

        [Theory]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        public void SourceSettingsAndSequenceConfigured_ConfigureAndDisableSourceTriggerSoftwareEdge_TriggerConfiguredAndDisabled(string pinMapFileName)
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
        public void Initialize_ConfigureMeasureTriggerSoftwareEdge_MeasureTriggerConfigured(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VDET" });

            // Test Software Trigger
            sessionsBundle.ConfigureTriggerSoftwareEdge(TriggerType.MeasureTrigger);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Measurement.Configuration.MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger;
                sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Triggers.MeasureTrigger.Type = DCPowerMeasureTriggerType.SoftwareEdge;
                sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Commit();

                AssertMeasureTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerMeasureTriggerType.SoftwareEdge);
            });
        }

        [Theory]
        [InlineData("Mixed Signal Tests.pinmap")]
        [InlineData("SMUsSupportingPulsing.pinmap")]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
        public void Initialize_ConfigureMeasureTriggerDigitalEdge_MeasureTriggerConfigured(string pinMapFileName)
        {
            var sessionManager = Initialize(pinMapFileName);
            var sessionsBundle = sessionManager.DCPower(new string[] { "VDD", "VDET" });
            var triggerLine = "PXI_Trig0";

            sessionsBundle.ConfigureTriggerDigitalEdge(TriggerType.MeasureTrigger, triggerLine);

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Measurement.Configuration.MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger;
                sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Commit();
                var inputTerminal = $"/{sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Name.Split('/')[0]}/{triggerLine}";
                AssertMeasureTriggerSettings(sessionInfo, sitePinInfo.IndividualChannelString, DCPowerMeasureTriggerType.DigitalEdge, inputTerminal, DCPowerTriggerEdge.Rising);
            });
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
