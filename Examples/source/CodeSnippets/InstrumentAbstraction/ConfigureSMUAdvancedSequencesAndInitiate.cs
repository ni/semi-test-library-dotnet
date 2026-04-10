using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions.
    /// Specifically, how to force a voltage sequence on pins mapped to Source Measurement Unit (SMU) devices.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program with any dependent instrument sessions have already been initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class ConfigureSMUAdvancedSequencesAndInitiate
    {
        /// <summary>
        /// Configures measurement and source settings for specified SMU pins and sets up an advanced sequence that can be initiated later in the test flow.
        /// </summary>
        /// <remarks>
        /// This example demonstrates how to configure an advanced sequence upfront without immediately initiating the sequence.
        /// The configured advanced sequence can be recalled by name and initiated at a later point in the test program,
        /// allowing for flexible test execution and reducing the need for repeated configuration.
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">SMU pin names to be configured.</param>
        internal static void ConfigureSMUAdvancedSequenceAndInitiate(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Measurements can be taken during sequence execution, with exactly one sample for each step,
            // but to enable this, the MeasureWhen property must be set to AutomaticallyAfterSourceComplete.
            dcPowerPins.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            // Note that ConfigureSourceSettings should be called before ConfigureAdvancedSequence as ConfigureSourceSettings sets the Source Mode to SinglePoint.
            dcPowerPins.ConfigureSourceSettings(new DCPowerSourceSettings() { SourceDelayInSeconds = 10, TransientResponse = DCPowerSourceTransientResponse.Normal });

            dcPowerPins.Commit();

            // Configure the advanced sequence upfront, but do not set it as the active sequence.
            // This allows you to initiate the advanced sequence later in your test flow without needing to reconfigure it.
            var advancedSequenceName = "MyAdvancedSequence";
            var advancedSequenceSettings = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, ApertureTime = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, ApertureTime = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, ApertureTime = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };
            dcPowerPins.ConfigureAdvancedSequence(advancedSequenceName, advancedSequenceSettings, setAsActiveSequence: false);

            // Initiate the advanced sequence that was configured earlier
            dcPowerPins.InitiateAdvancedSequence(advancedSequenceName, waitForSequenceCompletion: true);

            // Clearing the active advanced sequence after use.
            dcPowerPins.ClearActiveAdvancedSequence();
            // Deleting the advanced sequence after use to free up available sequences (limited to 100 per session).
            dcPowerPins.DeleteAdvancedSequence(advancedSequenceName);
        }

        /// <summary>
        /// Configures multiple advanced sequences for specified SMU pins and initiates one of the sequences later in the test flow.
        /// </summary>
        /// <remarks>
        /// This example demonstrates how to configure multiple advanced sequences upfront without immediately initiating them.
        /// The configured advanced sequences can be recalled by name and initiated at a later point in the test program,
        /// allowing for flexible test execution and reducing the need for repeated configuration.
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">SMU pin names to be configured.</param>
        internal static void ConfigureMultipleSMUAdvancedSequencesAndInitiate(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Measurements can be taken during sequence execution, with exactly one sample for each step,
            // but to enable this, the MeasureWhen property must be set to AutomaticallyAfterSourceComplete.
            dcPowerPins.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            // Note that ConfigureSourceSettings should be called before ConfigureAdvancedSequence as ConfigureSourceSettings sets the Source Mode to SinglePoint.
            dcPowerPins.ConfigureSourceSettings(new DCPowerSourceSettings() { SourceDelayInSeconds = 10, TransientResponse = DCPowerSourceTransientResponse.Normal });

            dcPowerPins.Commit();

            // Configure both advanced sequences upfront without setting either of them as the active sequence.
            // This allows you to initiate either of the advanced sequences later in your test flow without needing to reconfigure them.
            var firstAdvancedSequence = "MyAdvancedSequence1";
            var advanceSequenceSettingsForFirstAdvancedSequences = new List<DCPowerAdvancedSequenceStepProperties>
            {
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 1.0, ApertureTime = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 2.0, ApertureTime = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage },
                new DCPowerAdvancedSequenceStepProperties { VoltageLevel = 3.0, ApertureTime = 1.0, OutputFunction = DCPowerSourceOutputFunction.DCVoltage }
            };

            // Configure another advanced sequence with different settings
            var secondAdvancedSequence = "MyAdvancedSequence2";
            var voltages = new double[] { 0, 2, 3, 4 };
            var apertureTimes = new double[] { 2, 3.1, 4.3, 2.4 };
            var advancedSequenceSettingsForSecondAdvancedSequences = new DCPowerAdvancedSequenceStepProperties[4];
            for (int i = 0; i < voltages.Length; i++)
            {
                var stepSetting = new DCPowerAdvancedSequenceStepProperties
                {
                    VoltageLevel = voltages[i],
                    ApertureTime = apertureTimes[i]
                };
                advancedSequenceSettingsForSecondAdvancedSequences[i] = stepSetting;
            }

            // Configure both advanced sequences upfront without setting either of them as the active sequence.
            // This allows you to initiate either of the advanced sequences later in your test flow without needing to reconfigure them.
            dcPowerPins.ConfigureAdvancedSequence(firstAdvancedSequence, advanceSequenceSettingsForFirstAdvancedSequences, setAsActiveSequence: false);
            dcPowerPins.ConfigureAdvancedSequence(secondAdvancedSequence, advancedSequenceSettingsForSecondAdvancedSequences, setAsActiveSequence: false);

            // Initiate the advanced sequences that was configured earlier
            dcPowerPins.InitiateAdvancedSequence(firstAdvancedSequence, waitForSequenceCompletion: true);
            dcPowerPins.InitiateAdvancedSequence(secondAdvancedSequence, waitForSequenceCompletion: true);
            dcPowerPins.InitiateAdvancedSequence(firstAdvancedSequence, waitForSequenceCompletion: true);

            // Clearing the active advanced sequence after use.
            dcPowerPins.ClearActiveAdvancedSequence();
            // Deleting the advanced sequence after use to free up available sequences (limited to 100 per session).
            // Note that you can pass the DeleteAdvancedSequence method one or more sequence names.
            dcPowerPins.DeleteAdvancedSequence(firstAdvancedSequence, secondAdvancedSequence);
        }
    }
}
