using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to perform analog output task operations for pins mapped to DAQmx instruments.
    /// This class, and it's methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have already initiated and configured prior.
    /// Additionally, they are intentionally marked as internal to prevent them from be directly invoked from code outside of this project.
    /// </summary>
    internal static class AnalogOutputDAQmx
    {
        internal static void OutputAnalog(ISemiconductorModuleContext tsmContext, string[] aoPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var aoPins = sessionManager.DAQmx(aoPinNames);
            var outputLevel = 5.0;

            // Assumes all aoPins passed in are indeed of task type: "AO" in Pin Map.
            aoPins.WriteAnalogSingleSample(outputLevel);
        }

        internal static void OutputPureToneAoFgen(ISemiconductorModuleContext tsmContext, string[] aoPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var aoPins = sessionManager.DAQmx(aoPinNames);

            // Assumes all aoPins passed in are indeed of task type: "AOFGEN" in Pin Map.
            aoPins.ConfigureAOFunctionGeneration(new AOFunctionGenerationSettings
            {
                FunctionType = AOFunctionGenerationType.Sine,
                Frequency = 1000,
                Amplitude = 1,
                Offset = 0,
            });

            aoPins.StartAOFunctionGeneration();

            // Wait 2 seconds, then stop generation.
            Utilities.PreciseWait(2);
            aoPins.Stop();
        }

        internal static void OutputAnalogToOutputPureToneAoFgen(ISemiconductorModuleContext tsmContext, string[] aoPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Assumes all aoPinNames passed in are mapped to DAQmx channels of task type: "AO" in Pin Map.
            var aoPins = sessionManager.DAQmx(aoPinNames);

            // Assumes the aoPinNames passed in also have a complementary pin defined in the pin map with the same name,
            // but using a suffix of "_AOFgen". These complementary pins are mapped to the same DAQmx channels,
            // but of task type: "AOFGEN".
            var aoFGenPins = sessionManager.DAQmx(aoPinNames.Select(x => $"{x}_AOFGen").ToArray());

            var voltageLevel = 5.0;

            // Configure and start AO Generation.
            aoPins.WriteAnalogSingleSample(voltageLevel, autoStart: true);

            // Wait 2 seconds, then stop generation.
            Utilities.PreciseWait(timeInSeconds: 2);

            // Switch from AO to AOFGen on same instrument channels.
            // The AO tasks must first be stopped, then the hardware must be unreserved.
            aoPins.Stop();
            aoPins.Unreserve();

            // Configure AOFGen Generation.
            aoFGenPins.ConfigureAOFunctionGeneration(new AOFunctionGenerationSettings
            {
                FunctionType = AOFunctionGenerationType.Sine,
                Frequency = 1000,
                Amplitude = voltageLevel,
                Offset = 0,
            });

            // Start AOFGen Generation.
            aoFGenPins.StartAOFunctionGeneration();

            // Wait 2 seconds, then stop generation.
            Utilities.PreciseWait(timeInSeconds: 2);

            // To switch back to AO to AOFGen on same instrument channels.
            // the AO tasks must again be stopped, and the hardware must be unreserved.
            aoFGenPins.Stop();
            aoFGenPins.Unreserve();
        }
    }
}
