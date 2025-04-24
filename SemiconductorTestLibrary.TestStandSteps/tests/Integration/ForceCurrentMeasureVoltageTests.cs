using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class ForceCurrentMeasureVoltageTests
    {
        [Fact]
        public void Initialize_RunForceCurrentMeasureVoltageWithPositiveInRangeVoltageLimit_VoltageLimitsCorrectlySet()
        {
           // var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            var tsmContext = CreateTSMContext("SharedPinTests.pinmap", "SharedPinTests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

             ForceCurrentMeasureVoltage(
                tsmContext,
               pinsOrPinGroups: new[] { "VCC1", "PA_EN" },
               currentLevel: 0.005,
               voltageLimit: 3.3,
                apertureTime: 5e-5,
               settlingTime: 5e-5);

            var dcPower = new TSMSessionManager(tsmContext).DCPower("VCC1").FilterBySite(new int[] { 2, 3 });
            dcPower.Do(sessionInfo =>
            {
                var dummy = sessionInfo.AllChannelsOutput.Source.Current.VoltageLimit;
                Assert.Equal(3.3, sessionInfo.AllChannelsOutput.Source.Current.VoltageLimit, 1);
            });
            var digital = new TSMSessionManager(tsmContext).Digital("PA_EN");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-2, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow);
                Assert.Equal(3.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });

            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunForceCurrentMeasureVoltageWithPositiveOutRangeVoltageLimit_VoltageLimitsCorrectlySet()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: 1.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var dcPower = new TSMSessionManager(tsmContext).DCPower("VCC1").FilterBySite(1);
            dcPower.Do(sessionInfo =>
            {
                Assert.Equal(1.3, sessionInfo.AllChannelsOutput.Source.Current.VoltageLimit, 1);
            });
            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow, 1);
                Assert.Equal(1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunForceCurrentMeasureVoltageWithNegativeInRangeVoltageLimit_VoltageLimitsCorrectlySet()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: -1.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var dcPower = new TSMSessionManager(tsmContext).DCPower("VCC1");
            dcPower.Do(sessionInfo =>
            {
                Assert.Equal(1.3, sessionInfo.AllChannelsOutput.Source.Current.VoltageLimit, 1);
            });
            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow, 1);
                Assert.Equal(1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunForceCurrentMeasureVoltageWithNegativeOutRangeVoltageLimit_VoltageLimitsCorrectlySet()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: -3.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var dcPower = new TSMSessionManager(tsmContext).DCPower("VCC1");
            dcPower.Do(sessionInfo =>
            {
                Assert.Equal(3.3, sessionInfo.AllChannelsOutput.Source.Current.VoltageLimit, 1);
            });
            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-2, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow);
                Assert.Equal(3.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void IntegrationtestDMM()
        {
          /*  var tsmContext = CreateTSMContext("SharedPinTests.pinmap");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDMMInstrumentation(tsmContext, resetDevice: false);
            // Step 1: Configure DC Power
            var dcPowerBundle = new TSMSessionManager(tsmContext).DCPower("VCC1");
            dcPowerBundle.Do(sessionInfo =>
            {
                // Set the output voltage and enable the channel
                sessionInfo.Session.Outputs[0].VoltageLevel = 3.3; // Set to 3.3V
                sessionInfo.Session.Outputs[0].Enabled = true;
            });

            var dmmBundle = new TSMSessionManager(tsmContext).DMM("DMM1");
            dmmBundle.Initiate();
            var measurementResults = dmmBundle.Fetch(1000);

            foreach (var pinName in measurementResults.PinNames)
            {
                foreach (var siteNumber in measurementResults.SiteNumbers)
                {
                    if (measurementResults.TryGetValue(siteNumber, pinName, out var value))
                    {
                        Console.WriteLine($"Pin: {pinName}, Site: {siteNumber}, Measured Voltage: {value} V");
                    }
                }
            }

            dmmBundle.Abort();
            dcPowerBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Outputs.Enabled = false;
            });*/
        }

        [Fact]
        public void Initialize_RunForceCurrentMeasureVoltageWithOutHighRangeVoltageLimit_ThrowsException()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);

            void ForceCurrentMeasureVoltageMethod() => ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: 8,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var exception = Assert.Throws<NISemiconductorTestException>(ForceCurrentMeasureVoltageMethod);
            Assert.Contains("Maximum Value: 6", exception.Message);
            CleanupInstrumentation(tsmContext);
        }
    }
}
