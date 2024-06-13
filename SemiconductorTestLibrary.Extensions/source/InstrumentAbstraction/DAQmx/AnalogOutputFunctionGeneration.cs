﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

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
        /// <param name="functionGenerationSettings">The function generation settings.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void ConfigureAOFunctionGeneration(this DAQmxTasksBundle tasksBundle, AOFunctionGenerationSettings functionGenerationSettings)
        {
            tasksBundle.Verify();
            tasksBundle.Do((taskInfo, sitePinInfo) =>
            {
                var targetChannel = taskInfo.Task.AOChannels[sitePinInfo.IndividualChannelString];
                if (functionGenerationSettings.FunctionType.HasValue)
                {
                    targetChannel.FunctionGenerationType = functionGenerationSettings.FunctionType.Value;
                }
                if (functionGenerationSettings.Frequency.HasValue)
                {
                    targetChannel.FunctionGenerationFrequency = functionGenerationSettings.Frequency.Value;
                }
                if (functionGenerationSettings.Amplitude.HasValue)
                {
                    targetChannel.FunctionGenerationAmplitude = functionGenerationSettings.Amplitude.Value;
                }
                if (functionGenerationSettings.Offset.HasValue)
                {
                    targetChannel.FunctionGenerationOffset = functionGenerationSettings.Offset.Value;
                }
                if (functionGenerationSettings.StartPhase.HasValue)
                {
                    targetChannel.FunctionGenerationStartPhase = functionGenerationSettings.StartPhase.Value;
                }
                if (functionGenerationSettings.DutyCycle.HasValue)
                {
                    targetChannel.FunctionGenerationSquareDutyCycle = functionGenerationSettings.DutyCycle.Value;
                }
            });
            tasksBundle.Commit();
        }

        /// <summary>
        /// Transitions the task to the running state to begin the analog output function generation.
        /// This method will wait for the appropriate amount of settling time required by the instrument.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void StartAOFunctionGeneration(this DAQmxTasksBundle tasksBundle)
        {
            var settlingTimes = tasksBundle.DoAndReturnPerInstrumentPerChannelResults((taskInfo, sitePinInfo) =>
            {
                return taskInfo.GetAOFunctionGenerationSettlingTime(sitePinInfo.IndividualChannelString);
            });
            // Want to ensure that all tasks are started at the same time.
            // This is important for the outputs to start as synchronized as possible.
            tasksBundle.Start();
            // We also want to ensure we wait only once for the max settling time required.
            PreciseWait(settlingTimes.Max().Max());
        }

        /// <summary>
        /// This method calculates the time required after starting a AO FuncGen task for the PXIe-4467 and PXIe-4468 for the output signal to settle.
        /// If the task contains a DAQ instrument that does not support AO FuncGen, such as the PXIe-6368, this method throws an exception.
        /// </summary>
        /// <param name="taskInfo">The <see cref="DAQmxTaskInformation"/> object.</param>
        /// <param name="individualChannelString">The individual channel string</param>
        /// <returns>The max settling time across all devices, given the current channel's frequency.</returns>
        /// <exception cref="NISemiconductorTestException"></exception>
        internal static double GetAOFunctionGenerationSettlingTime(this DAQmxTaskInformation taskInfo, string individualChannelString)
        {
            var period = 1 / taskInfo.Task.AOChannels[individualChannelString].FunctionGenerationFrequency;
            var perDeviceSettlingTimes = new List<double>();
            foreach (var device in taskInfo.Task.Devices)
            {
                var productType = DaqSystem.Local.LoadDevice(device).ProductType;
                var settlingTime = 0.0;
                switch (productType)
                {
                    case "PXIe-4467":
                        // The PXIe-4467 has a settling time that is calculated as: 80 cycles + 30 ms.
                        settlingTime = (period * 80.0) + 30e-3;
                        break;
                    case "PXIe-4468":
                        // The PXIe-4467 has a settling time that is calculated as: 90 cycles + 1 ms.
                        settlingTime = (period * 90.0) + 1e-3;
                        break;
                    default:
                        throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DAQmx_FuncGenNotSupported, productType));
                }
                perDeviceSettlingTimes.Add(settlingTime);
            }
            return perDeviceSettlingTimes.Max();
        }
    }
}
