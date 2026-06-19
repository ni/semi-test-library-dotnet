using System;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    internal class SampleExample
    {
        /// <summary>
        /// Example method to show generation of sinewaveform
        /// </summary>
        /// <param name="function">The standard function to configure.</param>
        public static void GenerateSineWaveform(StandardFunction function)
        {
            /*
             * ConfigureOutputMode
             * ConfigureStandardWaveform
             * Initiate
             * Poll for IsDone
             * Abort
             */
        }
    }
}
