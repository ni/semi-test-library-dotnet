using System;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    internal class SampleExample
    {
        /// <summary>
        /// Example method to show generation of sinewaveform
        /// </summary>
        /// <param name="function">The standard function to configure.</param>
        /// <param name="frequency">The frequency of the sine wave.</param>
        /// <param name="amplitude">The amplitude of the sine wave.</param>
        /// <param name="dcOffest">The DC offset of the sine wave.</param>
        /// <param name="startPhase">The start phase of the sine wave.</param>
        public static void GenerateSineWaveform(StandardFunction function, double frequency, double amplitude, double dcOffest = 0, double startPhase = 0)
        {
            /*
             * ConfigureOutputMode
             * ConfigureChannel
             * ConfigureStandardWaveform
             * Initiate
             * Poll for IsDone
             * Abort
             */
        }
    }
}
