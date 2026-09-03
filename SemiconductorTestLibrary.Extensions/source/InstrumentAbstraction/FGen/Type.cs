using NationalInstruments.ModularInstruments.NIFgen;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    /// <summary>
    /// The class is used to configure the settings for standard waveform generation in a signal generator.
    /// It encapsulates parameters such as the type of waveform, frequency, amplitude, DC offset, and start phase.
    /// </summary>
    public class StandardWaveformSettings
    {
        /// <summary>
        /// The standard waveform that you want the signal generator to produce (Sine, Square, Triangle, Ramp Up, Ramp Down, DC, Noise, User ).
        /// </summary>
        public StandardWaveform WaveformFunctionType { get; set; }

        /// <summary>
        /// The frequency of the standard waveform that you want the signal generator to produce (in hertz).
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// The peak-to-peak amplitude of the standard waveform that you want the signal generator to produce (in volts).
        /// </summary>
        public double Amplitude { get; set; }

        /// <summary>
        /// The DC offset of the standard waveform that you want the signal generator to produce (in volts).
        /// </summary>
        public double DcOffset { get; set; }

        /// <summary>
        /// The horizontal offset, in degrees of one waveform cycle, of the standard waveform that you want the signal generator to produce (in degrees).
        /// </summary>
        public double StartPhase { get; set; }

        /// <summary>
        /// Standard Waveform function settings.
        /// </summary>
        /// <param name="functionType">The standard waveform that you want the signal generator to produce (Sine, Square, Triangle, Ramp Up, Ramp Down, DC, Noise, User ).</param>
        /// <param name="frequency">The frequency of the standard waveform that you want the signal generator to produce (in hertz).</param>
        /// <param name="amplitude">The peak-to-peak amplitude of the standard waveform that you want the signal generator to produce (in volts).</param>
        /// <param name="dcOffset">The DC offset of the standard waveform that you want the signal generator to produce (in volts).</param>
        /// <param name="startPhase">The horizontal offset, in degrees of one waveform cycle, of the standard waveform that you want the signal generator to produce (in degrees).</param>
        public StandardWaveformSettings(StandardWaveform functionType, double frequency, double amplitude, double dcOffset = 0, double startPhase = 0)
        {
            WaveformFunctionType = functionType;
            Frequency = frequency;
            Amplitude = amplitude;
            DcOffset = dcOffset;
            StartPhase = startPhase;
        }
    }
}
