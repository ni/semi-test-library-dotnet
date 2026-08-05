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
        /// Specifies the standard waveform that you want the signal generator to produce (Sine, Square, Triangle, Ramp Up, Ramp Down, DC, Noise, User ).
        /// </summary>
        public StandardWaveform WaveformFunctionType { get; set; }

        /// <summary>
        /// Specifies the frequency of the standard waveform that you want the signal generator to produce (in hertz).
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Specifies the amplitude of the standard waveform that you want the signal generator to produce (in volts).
        /// </summary>
        public double Amplitude { get; set; }

        /// <summary>
        /// Specifies the DC offset of the standard waveform that you want the signal generator to produce (in volts).
        /// </summary>
        public double DcOffset { get; set; }

        /// <summary>
        /// Specifies the horizontal offset of the standard waveform that you want the signal generator to produce (in degrees).
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
            WaveformFunctionType = functionType;
            Frequency = frequency;
            Amplitude = amplitude;
            DcOffset = dcOffset;
            StartPhase = startPhase;
        }
    }
}
