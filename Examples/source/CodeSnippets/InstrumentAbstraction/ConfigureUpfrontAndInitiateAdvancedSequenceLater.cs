using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to measure current for pins mapped to DCPower Instruments.
    /// Note that DCPower Instruments include both Source Measurement Units (SMUs) and Programmable Power Supplies (PPS) devices.
    /// This class, and it's methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have been already initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class ConfigureUpfrontAndInitiateAdvancedSequenceLater
    {
        internal static void ConfigureUpfrontAndInitiateAdvancedSequenceLaterExample(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Configure the measure settings upfront
            dcPowerPins.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });

            // configure the advanced sequence upfront, but do not set it as the active sequence. This allows you to initiate the advanced sequence later in your test flow without needing to reconfigure it.
            var advanceSequenceName = "MyAdvancedSequence";
            var advanceSequenceSettings = new List<DCPowerAdvancedSequenceStepProperties>
            {
                [0] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, ApertureTime = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                [1] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, ApertureTime = 2.1, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                [2] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, ApertureTime = 2.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };

            dcPowerPins.ConfigureAdvancedSequence(advanceSequenceName, advanceSequenceSettings, setAsActiveSequence: false);

            // Configure the source settings upfront
            dcPowerPins.ConfigureSourceSettings(new DCPowerSourceSettings() { SourceDelayInSeconds = 10, TransientResponse = DCPowerSourceTransientResponse.Normal });

            dcPowerPins.Commit();
            // Initiate the advanced sequence that was configured earlier
            dcPowerPins.InitiateAdvancedSequence(advanceSequenceName);
        }

        internal static void ConfigureMultipleAdvanceSequencesUpfrontAndInitiateAdvancedSequencesLaterExample(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Configure the measure settings upfront
            dcPowerPins.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });

            // configure the advanced sequence upfront, but do not set it as the active sequence. This allows you to initiate the advanced sequence later in your test flow without needing to reconfigure it.
            var firstAdvanceSequence = "MyAdvancedSequence1";

            var advanceSequenceSettingsForFirstAdvancedSequences = new List<DCPowerAdvancedSequenceStepProperties>
            {
                [0] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, ApertureTime = 2.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                [1] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, ApertureTime = 2.1, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                [2] = new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, ApertureTime = 2.2, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };

            // configure another advanced sequence with different settings
            var secondAdvanceSequence = "MyAdvancedSequence2";

            var voltages = new double[] { 0, 2, 3, 4 };
            var apertureTimes = new double[] { 2, 3.1, 4.3, 2.4 };
            var advanceSequenceSettingsForSecondAdvancedSequences = new DCPowerAdvancedSequenceStepProperties[4];
            for (int i = 0; i < voltages.Length; i++)
            {
                var stepSetting = new DCPowerAdvancedSequenceStepProperties();
                stepSetting.VoltageLevel = voltages[i];
                stepSetting.ApertureTime = apertureTimes[i];
                advanceSequenceSettingsForSecondAdvancedSequences[i] = stepSetting;
            }

            // Configure both advanced sequences upfront without setting either of them as the active sequence. This allows you to initiate either of the advanced sequences later in your test flow without needing to reconfigure them.
            dcPowerPins.ConfigureAdvancedSequence(firstAdvanceSequence, advanceSequenceSettingsForFirstAdvancedSequences, setAsActiveSequence: false);
            dcPowerPins.ConfigureAdvancedSequence(secondAdvanceSequence, advanceSequenceSettingsForSecondAdvancedSequences, setAsActiveSequence: false);

            // Configure the source settings upfront
            dcPowerPins.ConfigureSourceSettings(new DCPowerSourceSettings() { SourceDelayInSeconds = 10, TransientResponse = DCPowerSourceTransientResponse.Normal });

            dcPowerPins.Commit();

            // Initiate the advanced sequence that was configured earlier
            dcPowerPins.InitiateAdvancedSequence(firstAdvanceSequence);
        }
    }
}
