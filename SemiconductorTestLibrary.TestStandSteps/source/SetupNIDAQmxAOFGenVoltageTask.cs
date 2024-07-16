using System;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Initializes an NI DAQmx Analog Output Function Generation Task associated with the pin map.
        /// The value of <paramref name="taskType"/> string must match that of the DAQmx task definition within the pin map (default ="AOFuncGen").
        /// Note that the task will only be configured upon exiting this step, it will not be running.
        /// This type of task is only supported by certain NI DAQmx hardware, such as the 4468.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="taskType">The NI DAQmx task type.</param>
        /// <param name="waveformType">The kind of waveform to generate.</param>
        /// <param name="frequency">The frequency value.</param>
        /// <param name="amplitude">The amplitude value.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="outputTerminalConfiguration">The terminal configuration of the analog output channel.</param>
        public static void SetupNIDAQmxAOFGenVoltageTask(
            ISemiconductorModuleContext tsmContext,
            string taskType = DefaultDAQmxTaskTypeStrings.AnalogOutputFunctionGeneration,
            AOFunctionGenerationType waveformType = AOFunctionGenerationType.Sine,
            double frequency = 1000.0,
            double amplitude = 5.0,
            double offset = 0.0,
            DAQmxTerminalConfiguration outputTerminalConfiguration = DAQmxTerminalConfiguration.Default)
        {
            try
            {
                InitializeAndClose.CreateDAQmxAOFunctionGenerationTasks(tsmContext, taskType, waveformType, frequency, amplitude, offset, outputTerminalConfiguration);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
