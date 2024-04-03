using System;
using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines operations on analog output channels configured under the Analog Output: Function Generation task type.
    /// </summary>
    public static class AnalogOutputFunctionGeneration
    {
        /// <summary>
        /// Configures the analog output function generation for the task (also known as PureTune).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="functionGenerationSettings">T</param>
        /// <exception cref="DaqException"></exception>
        public static void ConfigureAOFunctionGeneration(this DAQmxTasksBundle tasksBundle, AOFunctionGenerationSettings functionGenerationSettings)
        {
            tasksBundle.Verify();
            tasksBundle.Do((taskInfo, sitePinInfo) =>
            {
                var targetChannel = sitePinInfo.IndividualChannelString;
                if (functionGenerationSettings.FunctionType.HasValue)
                {
                    taskInfo.Task.AOChannels[targetChannel].FunctionGenerationType = functionGenerationSettings.FunctionType.Value;
                }
                if (functionGenerationSettings.Frequency.HasValue)
                {
                    taskInfo.Task.AOChannels[targetChannel].FunctionGenerationFrequency = functionGenerationSettings.Frequency.Value;
                }
                if (functionGenerationSettings.Amplitude.HasValue)
                {
                    taskInfo.Task.AOChannels[targetChannel].FunctionGenerationAmplitude = functionGenerationSettings.Amplitude.Value;
                }
                if (functionGenerationSettings.Offset.HasValue)
                {
                    taskInfo.Task.AOChannels[targetChannel].FunctionGenerationOffset = functionGenerationSettings.Offset.Value;
                }
                if (functionGenerationSettings.StartPhase.HasValue)
                {
                    taskInfo.Task.AOChannels[targetChannel].FunctionGenerationStartPhase = functionGenerationSettings.StartPhase.Value;
                }
                if (functionGenerationSettings.DutyCycle.HasValue)
                {
                    taskInfo.Task.AOChannels[targetChannel].FunctionGenerationSquareDutyCycle = functionGenerationSettings.DutyCycle.Value;
                }
            });
            tasksBundle.Commit();
        }
    }
}
