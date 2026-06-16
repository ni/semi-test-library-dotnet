using System;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c
{
    /// <summary>
    /// Set of protocol-agnostic examples to demonstrate using the <see cref="SPI"/> and
    /// <see cref="I2C"/> classes through the shared <see cref="IDigitalProtocol"/> interface.
    /// The <see cref="CommunicationProtocol"/> parameter selects which implementation is used
    /// at runtime.
    /// </summary>
    public static partial class TestStep
    {
        /// <summary>
        /// Only required if needing to override default protocol parameters.
        /// Call this once per test program before any protocol instances are used.
        /// Override only the values you need.
        /// </summary>
        /// <param name="communicationProtocol">The protocol whose parameters to configure.</param>
        /// <param name="addressBitWidth">The number of bits the register address is.</param>
        /// <param name="valueBitWidth">The number of bits the register value holds.</param>
        /// <param name="writePatternName">The digital pattern name used to write a register value.</param>
        /// <param name="readPatternName">The digital pattern name used to read a register value.</param>
        /// <param name="sourceWaveformName">The digital source waveform name.</param>
        /// <param name="captureWaveformName">The digital capture waveform name.</param>
        /// <param name="sampleWidth">The number of bits each digital waveform sample is.</param>
        /// <param name="readWriteCountSequenceRegister">The sequencer register controlling the transaction count.</param>
        /// <param name="addressBitWidthSequenceRegister">The sequencer register controlling the address bit width.</param>
        /// <param name="valueBitWidthSequenceRegister">The sequencer register controlling the value bit width.</param>
        /// <param name="pinNames">The protocol pin names expected by the template patterns.</param>
        public static void ConfigureDigitalProtocol(
            CommunicationProtocol communicationProtocol = CommunicationProtocol.SPI,
            int addressBitWidth = 16,
            int valueBitWidth = 16,
            string writePatternName = "SPI_write_template",
            string readPatternName = "SPI_read_template",
            string sourceWaveformName = "source_buffer",
            string captureWaveformName = "capture_buffer",
            int sampleWidth = 8,
            string readWriteCountSequenceRegister = "reg0",
            string addressBitWidthSequenceRegister = "reg1",
            string valueBitWidthSequenceRegister = "reg2",
            string[] pinNames = null)
        {
            if (communicationProtocol == CommunicationProtocol.I2C
                && (writePatternName == "SPI_write_template" || readPatternName == "SPI_read_template"))
            {
                throw new ArgumentException(
                    "The writePatternName and readPatternName parameters must be explicitly set when communicationProtocol is CommunicationProtocol.I2C.");
            }

            IDigitalProtocol protocol = communicationProtocol == CommunicationProtocol.SPI
                ? (IDigitalProtocol)SPI.Instance
                : I2C.Instance;

            protocol.DefaultAddressBitWidth = (uint)addressBitWidth;
            protocol.DefaultValueBitWidth = (uint)valueBitWidth;
            protocol.WritePatternName = writePatternName;
            protocol.ReadPatternName = readPatternName;
            protocol.SourceWaveformName = sourceWaveformName;
            protocol.CaptureWaveformName = captureWaveformName;
            protocol.SampleWidth = (uint)sampleWidth;
            protocol.ReadWriteCountSequenceRegister = readWriteCountSequenceRegister;
            protocol.AddressBitWidthSequenceRegister = addressBitWidthSequenceRegister;
            protocol.ValueBitWidthSequenceRegister = valueBitWidthSequenceRegister;
            if (pinNames != null)
            {
                protocol.PinNames = pinNames;
            }
        }
    }
}
