using System;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.FGenStandardWaveformGeneration
{
    /// <summary>
    /// This class provides an example method to demonstrate how to configure and start a standard waveform
    /// (Sine, Square, Triangle, Ramp Up, Ramp Down, DC, or Noise) on pins mapped to a Function Generator (NI-FGEN),
    /// using the Fgen Instrument Abstraction methods from the Semiconductor Test Library.
    /// Assumptions:
    /// 1. The pin map maps the target pins to NI-FGEN channels.
    /// 2. The NI-FGEN sessions have already been initialized, for example by the Setup NI-FGEN Instrumentation step.
    /// The <see cref="StandardWaveform.User"/> waveform type is not supported.
    /// </summary>
    public static class TestStep
    {
        /// <summary>
        /// Generates a standard waveform on all the specified pins and/or pin groups for 10 seconds.
        /// </summary>
        /// <remarks>
        /// This method performs the following operations:
        /// <list type="number">
        /// <item>Configures the output mode of the FGen pins to enable Function generation</item>
        /// <item>Configures settings for the selected standard waveform type</item>
        /// <item>Initiates generation</item>
        /// <item>Waits for 10 seconds</item>
        /// <item>Aborts generation</item>
        /// <item>
        /// </list>
        /// To generate continuously, remove the wait and abort calls and abort generation in a subsequent code module.
        /// </remarks>
        public static void GenerateStandardWaveform(
            ISemiconductorModuleContext tsmContext,
            string[] pinsOrPinGroups,
            string standardWaveformType,
            double frequency,
            double amplitude,
            double offset = 0,
            double phase = 0)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            var fgenBundle = sessionManager.Fgen(pinsOrPinGroups);

            // The output mode must be set to Function before configuring a standard waveform.
            fgenBundle.ConfigureOutputMode(OutputMode.Function);

            // Configure the standard waveform settings for all channels in the bundle.
            var standardWaveform = (StandardWaveform)Enum.Parse(typeof(StandardWaveform), standardWaveformType, ignoreCase: true);
            var waveformSettings = new StandardWaveformSettings(
                waveformFunctionType: standardWaveform,
                frequency: frequency,
                amplitude: amplitude,
                dcOffset: offset,
                startPhase: phase);
            fgenBundle.ConfigureStandardWaveform(waveformSettings);

            // Enable the output to allow the waveform to be generated at the output connector.
            fgenBundle.ConfigureOutputEnabled(true);

            // Initiate signal generation. This does not wait for generation to complete.
            fgenBundle.Initiate();

            try
            {
                // Wait for 10 seconds to ensure the generation has started. Adjust as needed.
                fgenBundle.WaitUntilDone(10000);
            }
            catch (NISemiconductorTestException)
            {
                // If the wait times out, it means the generation is still running. This is expected for continuous waveforms.
                // You can handle this case as needed, for example by logging a message or ignoring the exception.
            }

            // Stop generation after the wait time. Remove this line if you want continuous generation.
            fgenBundle.Abort();
        }
    }
}
