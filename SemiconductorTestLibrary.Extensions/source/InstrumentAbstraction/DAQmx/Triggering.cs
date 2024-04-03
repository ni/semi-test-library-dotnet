using System;
using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines operations for configuring triggers.
    /// </summary>
    public static class Triggering
    {
        /// <summary>
        /// Configures the start trigger for the task.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public static void ConfigureStartTriggerDigitalEdge()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disables the start trigger for the task.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public static void DisableStartTrigger()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Exports the signal to the specified output terminal.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public static void ExportSignal()
        {
            throw new NotImplementedException();
        }
    }
}
