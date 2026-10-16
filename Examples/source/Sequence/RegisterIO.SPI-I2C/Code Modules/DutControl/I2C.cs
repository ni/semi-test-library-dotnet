using System;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl
{
    /// <summary>
    /// Implements <see cref="IDigitalProtocol"/> for I2C register access using
    /// NI Digital Pattern instruments with SDI source, SDO capture, and SCK clock waveforms.
    /// </summary>
    /// <remarks>
    /// <see cref="Instance"/> is a process-wide singleton that holds shared configuration and the active bundle.
    /// It is not thread-safe: do not use it from multiple code modules, contexts, or tests in parallel.
    /// </remarks>
    public sealed class I2C : DigitalProtocol
    {
        private static readonly Lazy<I2C> _instance = new Lazy<I2C>(() => new I2C());

        /// <summary>Gets the singleton I2C protocol instance.</summary>
        public static I2C Instance => _instance.Value;

        internal I2C()
        {
            WritePatternName = "I2C_write_pattern";
            ReadPatternName = "I2C_read_pattern";
            SourcePinName = "SDI";
            CapturePinName = "SDO";
            PinNames = new[] { "SDI", "SDO", "SCK" };
        }
    }
}