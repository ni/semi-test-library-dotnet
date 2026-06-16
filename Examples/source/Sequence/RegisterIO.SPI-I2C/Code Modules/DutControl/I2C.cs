using System;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl
{
    /// <summary>
    /// Implements <see cref="IDigitalProtocol"/> for I2C register access using
    /// NI Digital Pattern instruments with source and capture waveforms.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Obtain the singleton instance via <see cref="Instance"/>, then call
    /// <see cref="DigitalProtocol.SetBundle"/> once per code module before performing
    /// any register operations. Optionally call
    /// <see cref="TestStep.ConfigureDigitalProtocol"/> once per test program to override
    /// default protocol parameters.
    /// </para>
    /// <para>
    /// Example usage:
    /// <code>
    /// IDigitalProtocol protocol = new TSMSessionManager(tsmContext).DutControl(CommunicationProtocol.I2C);
    /// protocol.WriteRegister(address: 0x48, value: 4);
    /// SiteData&lt;long&gt; readBack = protocol.ReadRegister(address: 0x48);
    /// </code>
    /// </para>
    /// </remarks>
    public sealed class I2C : DigitalProtocol
    {
        private static readonly Lazy<I2C> _instance = new Lazy<I2C>(() => new I2C());

        /// <summary>Gets the singleton I2C protocol instance.</summary>
        public static I2C Instance => _instance.Value;

        private I2C()
        {
            ReadPatternName = "I2C_read_template";
            WritePatternName = "I2C_write_template";
        }
    }
}
