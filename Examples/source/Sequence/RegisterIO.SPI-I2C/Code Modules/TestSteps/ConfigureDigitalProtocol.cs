using System;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C
{
    /// <summary>
    /// Set of protocol-agnostic examples to demonstrate using the <see cref="SPI"/> and <see cref="I2C"/> classes
    /// through the shared <see cref="IDigitalProtocol"/> interface.
    /// The <see cref="CommunicationProtocol"/> parameter selects which protocol is used
    /// at runtime.
    /// </summary>
    public static partial class TestStep
    {
        private const int UseProtocolDefault = -1;

        /// <summary>
        /// Only required if needing to override default protocol parameters.
        /// Call this once per test program before any protocol instances are used.
        /// Override only the values you need. Any parameter left at its default value (an empty string ("") for strings,
        /// -1 for numbers, or an empty array or null for pin names) uses the protocol's default value for that setting.
        /// </summary>
        /// <param name="communicationProtocol">The protocol whose parameters to configure.</param>
        /// <param name="addressBitWidth">The number of bits the register address is. If specified as -1, the protocol's default address bit width is used.</param>
        /// <param name="valueBitWidth">The number of bits the register value holds. If specified as -1, the protocol's default value bit width is used.</param>
        /// <param name="writePatternName">The digital pattern name used to write a register value. If specified as an empty string (""), the protocol's default write pattern name is used.</param>
        /// <param name="readPatternName">The digital pattern name used to read a register value. If specified as an empty string (""), the protocol's default read pattern name is used.</param>
        /// <param name="sourceWaveformName">The digital source waveform name. If specified as an empty string (""), the protocol's default source waveform name is used.</param>
        /// <param name="captureWaveformName">The digital capture waveform name. If specified as an empty string (""), the protocol's default capture waveform name is used.</param>
        /// <param name="sampleWidth">The number of bits each digital waveform sample is. If specified as -1, the protocol's default sample width is used.</param>
        /// <param name="readWriteCountSequenceRegister">The sequencer register controlling the transaction count. If specified as an empty string (""), the protocol's default register is used.</param>
        /// <param name="addressBitWidthSequenceRegister">The sequencer register controlling the address bit width. If specified as an empty string (""), the protocol's default register is used.</param>
        /// <param name="valueBitWidthSequenceRegister">The sequencer register controlling the value bit width. If specified as an empty string (""), the protocol's default register is used.</param>
        /// <param name="pinNames">The protocol pin names expected by the template patterns. Must include the protocol's source and capture pins. If specified as an empty array or null, the protocol's default pin names are used.</param>
        public static void ConfigureDigitalProtocol(
            CommunicationProtocol communicationProtocol = CommunicationProtocol.SPI,
            int addressBitWidth = UseProtocolDefault,
            int valueBitWidth = UseProtocolDefault,
            string writePatternName = "",
            string readPatternName = "",
            string sourceWaveformName = "",
            string captureWaveformName = "",
            int sampleWidth = UseProtocolDefault,
            string readWriteCountSequenceRegister = "",
            string addressBitWidthSequenceRegister = "",
            string valueBitWidthSequenceRegister = "",
            string[] pinNames = null)
        {
            IDigitalProtocol protocol = SemiconductorModuleContextExtensions.GetProtocol(communicationProtocol);
            IDigitalProtocol defaults = SemiconductorModuleContextExtensions.CreateDefaultProtocol(communicationProtocol);

            // The numeric settings are validated by their setters, so apply them first so invalid values throw before any names change.
            protocol.DefaultAddressBitWidth = ValueOrDefault(addressBitWidth, defaults.DefaultAddressBitWidth);
            protocol.DefaultValueBitWidth = ValueOrDefault(valueBitWidth, defaults.DefaultValueBitWidth);
            protocol.SampleWidth = ValueOrDefault(sampleWidth, defaults.SampleWidth);
            protocol.WritePatternName = ValueOrDefault(writePatternName, defaults.WritePatternName);
            protocol.ReadPatternName = ValueOrDefault(readPatternName, defaults.ReadPatternName);
            protocol.SourceWaveformName = ValueOrDefault(sourceWaveformName, defaults.SourceWaveformName);
            protocol.CaptureWaveformName = ValueOrDefault(captureWaveformName, defaults.CaptureWaveformName);
            protocol.ReadWriteCountSequenceRegister = ValueOrDefault(readWriteCountSequenceRegister, defaults.ReadWriteCountSequenceRegister);
            protocol.AddressBitWidthSequenceRegister = ValueOrDefault(addressBitWidthSequenceRegister, defaults.AddressBitWidthSequenceRegister);
            protocol.ValueBitWidthSequenceRegister = ValueOrDefault(valueBitWidthSequenceRegister, defaults.ValueBitWidthSequenceRegister);
            protocol.PinNames = pinNames == null || pinNames.Length == 0 ? defaults.PinNames : pinNames;
        }

        private static uint ValueOrDefault(int value, uint defaultValue)
        {
            return value == UseProtocolDefault ? defaultValue : (uint)value;
        }

        private static string ValueOrDefault(string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
    }
}
