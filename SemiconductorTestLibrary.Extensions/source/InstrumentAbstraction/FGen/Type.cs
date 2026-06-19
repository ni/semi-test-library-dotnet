using System;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Standard function. Will be moved to Abstractions.
    /// </summary>
    /// <remarks>
    /// This enum represents the standard waveform functions available in the FGen.
    /// </remarks>
    public enum StandardFunction // moved to Abstractions
    {
        Sine,
        Square,
        Triangle,
        Rampup,
        RampDown,
        DC,
        Noise,
        User
    }
    /// <summary>
    /// Output mode enum.
    /// </summary>
    /// <remarks>
    /// This enum represents the output modes available in the FGen.
    /// </remarks>
    public enum OutputMode // moved to Abstractions
    {
        StdWaveform,
        ArbWaveform,
        FrequencyList,
        ArbWaveformSequence.
    }

    /// <summary>
    /// ClockMode
    /// </summary>
    /// <remarks>
    /// This enum represents the clock modes available in the FGen.
    /// </remarks>
    public enum ClockMode // moved to Abstractions
    {
        HighResolutionSampling,
        DivideDownSampling,
        Automatic
    }

    /// <summary>
    /// Type class contains all type definitions needed for FGen operations.
    /// </summary>
    public class StdWaveformSettings
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
        public double DCOffset { get; set; }

        /// <summary>
        /// Specifies the start phase of the waveform to generate.
        /// </summary>
        public double StartPhase { get; set; }

        /// <summary>
        /// Standara Waveform function settings.
        /// </summary>
        /// <param name="functionType">functionType</param>
        /// <param name="frequency">frequency</param>
        /// <param name="amplitude">amplitude</param>
        /// <param name="dcOffset">dcOffset</param>
        /// <param name="startPhase">startPhase</param>
        public StdWaveformSettings(StandardFunction functionType, double frequency, double amplitude, double dcOffset = 0, double startPhase = 0)
        {
            FunctionType = functionType;
            Frequency = frequency;
            Amplitude = amplitude;
            DCOffset = dcOffset;
            StartPhase = startPhase;
        }
    }
}
