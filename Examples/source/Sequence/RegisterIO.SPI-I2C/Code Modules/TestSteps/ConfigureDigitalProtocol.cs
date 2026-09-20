using System;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C
{
    /// <summary>
    /// Set of protocol-agnostic examples to demonstrate using the <see cref="SPI"/> class
    /// through the shared <see cref="IDigitalProtocol"/> interface.
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
        /// <remarks>
        /// Any change to <paramref name="writePatternName"/> or <paramref name="readPatternName"/> must be
        /// reflected in the corresponding <c>.digipat</c> pattern files. When either is left <c>null</c>, the
        /// selected protocol keeps its default pattern name (<c>SPI_*_template</c> or <c>I2C_*_template</c>).
        /// </remarks>
        /// <param name="communicationProtocol">The protocol whose parameters to configure.</param>
        /// <param name="addressBitWidth">The number of bits the register address is.</param>
        /// <param name="valueBitWidth">The number of bits the register value holds.</param>
        /// <param name="writePatternName">The digital pattern name used to write a register value. When null, the protocol default is kept.</param>
        /// <param name="readPatternName">The digital pattern name used to read a register value. When null, the protocol default is kept.</param>
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
            string writePatternName = null,
            string readPatternName = null,
            string sourceWaveformName = "source_buffer",
            string captureWaveformName = "capture_buffer",
            int sampleWidth = 8,
            string readWriteCountSequenceRegister = "reg0",
            string addressBitWidthSequenceRegister = "reg1",
            string valueBitWidthSequenceRegister = "reg2",
            string[] pinNames = null)
        {
            IDigitalProtocol protocol;
            switch (communicationProtocol)
            {
                case CommunicationProtocol.SPI:
                    protocol = SPI.Instance;
                    break;
                case CommunicationProtocol.I2C:
                    protocol = I2C.Instance;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(communicationProtocol), communicationProtocol, "Unsupported communication protocol.");
            }

            protocol.DefaultAddressBitWidth = (uint)addressBitWidth;
            protocol.DefaultValueBitWidth = (uint)valueBitWidth;
            protocol.SourceWaveformName = sourceWaveformName;
            protocol.CaptureWaveformName = captureWaveformName;
            protocol.SampleWidth = (uint)sampleWidth;
            protocol.ReadWriteCountSequenceRegister = readWriteCountSequenceRegister;
            protocol.AddressBitWidthSequenceRegister = addressBitWidthSequenceRegister;
            protocol.ValueBitWidthSequenceRegister = valueBitWidthSequenceRegister;
            if (writePatternName != null)
            {
                protocol.WritePatternName = writePatternName;
            }
            if (readPatternName != null)
            {
                protocol.ReadPatternName = readPatternName;
            }
            if (pinNames != null)
            {
                protocol.PinNames = pinNames;
            }
        }
    }
}
