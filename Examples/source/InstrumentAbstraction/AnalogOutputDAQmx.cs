using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to perform analog output task operations for pins mapped to DAQmx instruments.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have already been initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
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

            // Waits 2 seconds, then stops generation.
            PreciseWait(timeInSeconds: 2);
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

            // Configures and starts AO Generation.
            aoPins.WriteAnalogSingleSample(voltageLevel, autoStart: true);

            // Waits 2 seconds, then stops generation.
            PreciseWait(timeInSeconds: 2);

            // Switches from AO to AOFGen on same instrument channels.
            // The AO tasks must first be stopped, then the hardware must be unreserved.
            aoPins.Stop();
            aoPins.Unreserve();

            // Configures AOFGen Generation.
            aoFGenPins.ConfigureAOFunctionGeneration(new AOFunctionGenerationSettings
            {
                FunctionType = AOFunctionGenerationType.Sine,
                Frequency = 1000,
                Amplitude = voltageLevel,
                Offset = 0,
            });

            // Starts AOFGen Generation.
            aoFGenPins.StartAOFunctionGeneration();

            // Waits 2 seconds, then stops generation.
            PreciseWait(timeInSeconds: 2);

            // To switch back to AO to AOFGen on same instrument channels.
            // the AO tasks must again be stopped, and the hardware must be unreserved.
            aoFGenPins.Stop();
            aoFGenPins.Unreserve();
        }

        internal static void ConfiguringCommonModeOffset(ISemiconductorModuleContext tsmContext, string[] aoPinNames, double commonModeValue)
        {
            // Creates an instance of the TSMSessionManager to handle the sessions.
            var sessionManager = new TSMSessionManager(tsmContext);

            // Assumes all aoPinNames passed in are mapped to DAQmx channels of task type: "AO" in Pin Map.
            var aoPins = sessionManager.DAQmx(aoPinNames);

            // Sets the terminal Configuration to Differential
            // Common mode offset value is only applicable when the terminal configuration is set to differential.
            aoPins.ConfigureAOTerminalConfiguration(AOTerminalConfiguration.Differential);

            // Configures the Common Mode Offset value for all the tasks.
            aoPins.Do(taskInfo =>
            {
                taskInfo.Task.AOChannels.All.CommonModeOffset = commonModeValue;
            });
        }
    }
}
