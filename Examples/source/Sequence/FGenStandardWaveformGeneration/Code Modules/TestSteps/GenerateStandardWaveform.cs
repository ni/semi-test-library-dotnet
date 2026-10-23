using System;
using System.Linq;
using NationalInstruments.ModularInstruments.NIFgen;
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
    public static class TestSteps
    {
        /// <summary>
        /// Configures the output mode to Function, configures the standard waveform settings,
        /// enables the output, and initiates signal generation on all the specified pins and/or pin groups.
        /// The method waits 3 seconds after generation starts, and then aborts generation before returning.
        /// To generate continuously, remove the wait and abort calls and abort generation in a subsequent code module.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object reference.</param>
        /// <param name="pinsOrPinGroups">The pins or pin groups mapped to function generator channels.</param>
        /// <param name="standardWaveformType">The standard waveform type to generate. "User" waverform type is not supported.</param>
        /// <param name="frequency">The frequency of the waveform, in hertz.</param>
        /// <param name="amplitude">The peak-to-peak amplitude of the waveform, in volts.</param>
        /// <param name="offset">The DC offset of the waveform, in volts.</param>
        /// <param name="phase">The start phase of the waveform, in degrees.</param>
        public static void GenerateStandardWaveform(
            ISemiconductorModuleContext tsmContext,
            string[] pinsOrPinGroups,
            string standardWaveformType,
            double frequency,
            double amplitude,
            double offset,
            double phase)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var fgenPins = tsmContext.FilterPinsByInstrumentType(pinsOrPinGroups, InstrumentTypeIdConstants.NIFgen);

            if (fgenPins.Any())
            {
                var fgenBundle = sessionManager.Fgen(fgenPins);

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

                // Wait for 3 seconds to ensure the generation has started. Adjust as needed.
                fgenBundle.WaitUntilDone(3000);

                // Stop generation after the wait time. Remove this line if you want continuous generation.
                fgenBundle.Abort();
            }
        }
    }
}
