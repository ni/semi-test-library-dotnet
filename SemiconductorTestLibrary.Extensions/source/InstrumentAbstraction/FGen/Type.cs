using System;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Standard function.
    /// </summary>
    public enum StandardFunction
    {
        Sine,
        Square,
        Triangle,
        Rampup,
        RamDown,
        DC,
        Noise,
        User
    }

    /// <summary>
    /// Type class contains all type definitions needed for FGen operations.
    /// </summary>
    public class WaveformType
    {
        /// <summary>
        /// Specifies the kind of the waveform to generate (sine, square, triangle, etc).
        /// </summary>
        public StandardFunction FunctionType { get; set; }

        /// <summary>
        /// Specifies the frequency of the waveform to generate.
        /// </summary>
        public double? Frequency { get; set; }

        /// <summary>
        /// Specifies the amplitude of the waveform to generate.
        /// </summary>
        public double? Amplitude { get; set; }

        /// <summary>
        /// Specifies the offset of the waveform to generate.
        /// </summary>
        public double? DCOffset { get; set; }

        /// <summary>
        /// Specifies the start phase of the waveform to generate.
        /// </summary>
        public double? StartPhase { get; set; }

        /// <summary>
        /// Waveform type.
        /// </summary>
        public WaveformType()
        { }
    }
}
