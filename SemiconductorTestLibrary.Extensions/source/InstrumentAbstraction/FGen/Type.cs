using NationalInstruments.ModularInstruments.NIFgen;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Type class contains all type definitions needed for FGen operations.
    /// </summary>
    public class StandardWaveformSettings
    {
        /// <summary>
        /// Specifies the kind of the waveform to generate (sine, square, triangle, etc).
        /// </summary>
        public StandardWaveform FunctionType { get; set; }

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
        /// Standard Waveform function settings.
        /// </summary>
        /// <param name="functionType">functionType</param>
        /// <param name="frequency">frequency</param>
        /// <param name="amplitude">amplitude</param>
        /// <param name="dcOffset">dcOffset</param>
        /// <param name="startPhase">startPhase</param>
        public StandardWaveformSettings(StandardWaveform functionType, double frequency, double amplitude, double dcOffset = 0, double startPhase = 0)
        {
            FunctionType = functionType;
            Frequency = frequency;
            Amplitude = amplitude;
            DCOffset = dcOffset;
            StartPhase = startPhase;
        }
    }
}
