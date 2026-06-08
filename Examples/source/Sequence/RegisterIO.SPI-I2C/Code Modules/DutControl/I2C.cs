using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl
{
    /// <summary>
    /// Implements <see cref="IDigitalProtocol"/> for I2C register access using
    /// NI Digital Pattern instruments with source and capture waveforms.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Create a new instance per code module (similar to <see cref="TSMSessionManager"/>).
    /// The class supports configurable address and data bit widths, single and multi-register
    /// operations, and both broadcast and site-unique data writes.
    /// </para>
    /// <para>
    /// Example usage:
    /// <code>
    /// IDigitalProtocol protocol = new I2C(tsmContext);
    /// protocol.WriteRegister(address: 0x48, value: 4);
    /// SiteData&lt;long&gt; readBack = protocol.ReadRegister(address: 0x48);
    /// </code>
    /// </para>
    /// </remarks>
    public class I2C : IDigitalProtocol
    {
        private readonly DigitalSessionsBundle _digitalSessionsBundle;
        private readonly int[] _activeSiteNumbers;

        /// <summary>
        /// Gets the digital pattern name used to read a value from a DUT register.
        /// </summary>
        public string ReadPatternName { get; }

        /// <summary>
        /// Gets the digital pattern name used to write a value to a DUT register.
        /// </summary>
        public string WritePatternName { get; }

        /// <summary>
        /// Gets the digital capture waveform name used by the read pattern.
        /// </summary>
        public string CaptureWaveformName { get; }

        /// <summary>
        /// Gets the digital source waveform name used by both the read and write patterns.
        /// </summary>
        public string SourceWaveformName { get; }

        /// <summary>
        /// Gets the number of bits each digital waveform sample is.
        /// </summary>
        public uint SampleWidth { get; }

        /// <summary>
        /// Gets the digital sequencer register used in the pattern to dynamically specify the number of registers to read/write.
        /// </summary>
        public string ReadWriteCountSequenceRegister { get; }

        /// <summary>
        /// Gets the digital sequencer register used in the pattern to dynamically specify the register address size.
        /// </summary>
        public string AddressBitWidthSequenceRegister { get; }

        /// <summary>
        /// Gets the digital sequencer register used in the pattern to dynamically specify the register value size.
        /// </summary>
        public string ValueBitWidthSequenceRegister { get; }

        /// <summary>
        /// Gets the number of bits the register address is.
        /// </summary>
        public uint AddressBitWidth { get; }

        /// <summary>
        /// Gets the number of bits the register data holds.
        /// </summary>
        public uint ValueBitWidth { get; }

        /// <summary>
        /// Gets the bit ordering used when packing and unpacking waveform samples.
        /// </summary>
        public BitOrder BitOrder { get; }

        /// <summary>
        /// Constructs an <see cref="I2C"/> object to read and write to DUT registers via I2C.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="addressBitWidth">The number of bits the register address is.</param>
        /// <param name="valueBitWidth">The number of bits the register data holds.</param>
        /// <param name="readPatternName">The digital pattern name used to read a value from a DUT register.</param>
        /// <param name="writePatternName">The digital pattern name used to write a value to a DUT register.</param>
        /// <param name="sampleWidth">The number of bits each digital waveform sample is.</param>
        /// <param name="captureWaveformName">The digital capture waveform name used by the read pattern.</param>
        /// <param name="sourceWaveformName">The digital source waveform name used by both the read and write patterns.</param>
        /// <param name="readWriteCountSequenceRegister">The digital sequencer register used in the patterns to dynamically specify the number of registers to read/write.</param>
        /// <param name="addressBitWidthSequenceRegister">The digital sequencer register used in the patterns to dynamically specify the register address size.</param>
        /// <param name="valueBitWidthSequenceRegister">The digital sequencer register used in the patterns to dynamically specify the register value size.</param>
        /// <param name="bitOrder">The bit ordering used when packing and unpacking waveform samples. Defaults to MSB-first.</param>
        public I2C(
            ISemiconductorModuleContext semiconductorModuleContext,
            uint addressBitWidth = 8,
            uint valueBitWidth = 8,
            string readPatternName = "I2C_read_template",
            string writePatternName = "I2C_write_template",
            uint sampleWidth = 8,
            string captureWaveformName = "capture_buffer",
            string sourceWaveformName = "source_buffer",
            string readWriteCountSequenceRegister = "reg0",
            string addressBitWidthSequenceRegister = "reg1",
            string valueBitWidthSequenceRegister = "reg2",
            BitOrder bitOrder = BitOrder.MsbFirst)
        {
            ReadPatternName = readPatternName;
            WritePatternName = writePatternName;
            CaptureWaveformName = captureWaveformName;
            SourceWaveformName = sourceWaveformName;
            SampleWidth = sampleWidth;
            ReadWriteCountSequenceRegister = readWriteCountSequenceRegister;
            AddressBitWidthSequenceRegister = addressBitWidthSequenceRegister;
            ValueBitWidthSequenceRegister = valueBitWidthSequenceRegister;

            AddressBitWidth = addressBitWidth;
            ValueBitWidth = valueBitWidth;
            BitOrder = bitOrder;

            var sm = new TSMSessionManager(semiconductorModuleContext);
            _digitalSessionsBundle = sm.Digital();
            _activeSiteNumbers = semiconductorModuleContext.SiteNumbers.ToArray();
        }

        #region Single Register Operations

        /// <inheritdoc/>
        public SiteData<long> ReadRegister(uint address)
        {
            return ReadRegister(address, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="ReadRegister(uint)"/>
        /// <param name="addressBitWidth">The bit width for the register address.</param>
        /// <param name="valueBitWidth">The bit width for the register value.</param>
        public SiteData<long> ReadRegister(uint address, uint addressBitWidth, uint valueBitWidth)
        {
            uint[] addressSamples = LongToU32Samples(address, addressBitWidth, SampleWidth, BitOrder);
            uint[] valuePadding = new uint[GetSampleCount(valueBitWidth, SampleWidth)];
            uint[] srcWfmSamples = ConcatArrays(addressSamples, valuePadding);

            _digitalSessionsBundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, 1);
            _digitalSessionsBundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            _digitalSessionsBundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            _digitalSessionsBundle.WriteSourceWaveformBroadcast(SourceWaveformName, srcWfmSamples);
            _digitalSessionsBundle.BurstPattern(ReadPatternName, timeoutInSeconds: 10);

            uint samplesPerValue = GetSampleCount(valueBitWidth, SampleWidth);
            SiteData<uint[]> captureWaveforms = _digitalSessionsBundle.FetchCaptureWaveform(
                CaptureWaveformName, samplesToRead: (int)samplesPerValue, timeoutInSeconds: 10);

            return captureWaveforms.Select(x => U32SamplesToLong(x, SampleWidth, BitOrder));
        }

        /// <inheritdoc/>
        public void WriteRegister(uint address, long value)
        {
            WriteRegister(address, value, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="WriteRegister(uint, long)"/>
        /// <param name="addressBitWidth">The bit width for the register address.</param>
        /// <param name="valueBitWidth">The bit width for the register value.</param>
        public void WriteRegister(uint address, long value, uint addressBitWidth, uint valueBitWidth)
        {
            uint[] addressSamples = LongToU32Samples(address, addressBitWidth, SampleWidth, BitOrder);
            uint[] valueSamples = LongToU32Samples(value, valueBitWidth, SampleWidth, BitOrder);
            uint[] srcWfmSamples = ConcatArrays(addressSamples, valueSamples);

            _digitalSessionsBundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, 1);
            _digitalSessionsBundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            _digitalSessionsBundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            _digitalSessionsBundle.WriteSourceWaveformBroadcast(SourceWaveformName, srcWfmSamples);
            _digitalSessionsBundle.BurstPattern(WritePatternName, timeoutInSeconds: 10);
        }

        /// <inheritdoc/>
        public void WriteRegister(uint address, SiteData<long> value)
        {
            WriteRegister(address, value, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="WriteRegister(uint, SiteData{long})"/>
        /// <param name="addressBitWidth">The bit width for the register address.</param>
        /// <param name="valueBitWidth">The bit width for the register value.</param>
        public void WriteRegister(uint address, SiteData<long> value, uint addressBitWidth, uint valueBitWidth)
        {
            uint[] addressSamples = LongToU32Samples(address, addressBitWidth, SampleWidth, BitOrder);
            uint[][] perSiteWfmSamples = new uint[_activeSiteNumbers.Length][];
            for (int i = 0; i < _activeSiteNumbers.Length; i++)
            {
                uint[] valueSamples = LongToU32Samples(value.GetValue(_activeSiteNumbers[i]), valueBitWidth, SampleWidth, BitOrder);
                perSiteWfmSamples[i] = ConcatArrays(addressSamples, valueSamples);
            }
            SiteData<uint[]> sourceWaveforms = new SiteData<uint[]>(_activeSiteNumbers, perSiteWfmSamples);

            _digitalSessionsBundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, 1);
            _digitalSessionsBundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            _digitalSessionsBundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            _digitalSessionsBundle.WriteSourceWaveformSiteUnique(SourceWaveformName, sourceWaveforms);
            _digitalSessionsBundle.BurstPattern(WritePatternName, timeoutInSeconds: 10);
        }

        #endregion Single Register Operations

        #region Multi Register Operations

        /// <inheritdoc/>
        public SiteData<long[]> ReadRegisters(uint[] addresses)
        {
            return ReadRegisters(addresses, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="ReadRegisters(uint[])"/>
        /// <param name="addressBitWidth">The bit width for each register address.</param>
        /// <param name="valueBitWidth">The bit width for each register value.</param>
        public SiteData<long[]> ReadRegisters(uint[] addresses, uint addressBitWidth, uint valueBitWidth)
        {
            long[] dummyValues = new long[addresses.Length];
            uint[] srcWfmSamples = BuildInterleavedSourceWaveform(addresses, dummyValues, addressBitWidth, valueBitWidth);

            _digitalSessionsBundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, addresses.Length);
            _digitalSessionsBundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            _digitalSessionsBundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            _digitalSessionsBundle.WriteSourceWaveformBroadcast(SourceWaveformName, srcWfmSamples);
            _digitalSessionsBundle.BurstPattern(ReadPatternName, timeoutInSeconds: 10);

            uint samplesPerValue = GetSampleCount(valueBitWidth, SampleWidth);
            int totalSamples = (int)(samplesPerValue * (uint)addresses.Length);
            SiteData<uint[]> captureWaveforms = _digitalSessionsBundle.FetchCaptureWaveform(
                CaptureWaveformName, samplesToRead: totalSamples, timeoutInSeconds: 10);

            return captureWaveforms.Select(x => U32SamplesToLongArray(x, SampleWidth, addresses.Length, BitOrder));
        }

        /// <inheritdoc/>
        public void WriteRegisters(uint[] addresses, long[] values)
        {
            WriteRegisters(addresses, values, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="WriteRegisters(uint[], long[])"/>
        /// <param name="addressBitWidth">The bit width for each register address.</param>
        /// <param name="valueBitWidth">The bit width for each register value.</param>
        public void WriteRegisters(uint[] addresses, long[] values, uint addressBitWidth, uint valueBitWidth)
        {
            uint[] srcWfmSamples = BuildInterleavedSourceWaveform(addresses, values, addressBitWidth, valueBitWidth);

            _digitalSessionsBundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, addresses.Length);
            _digitalSessionsBundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            _digitalSessionsBundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            _digitalSessionsBundle.WriteSourceWaveformBroadcast(SourceWaveformName, srcWfmSamples);
            _digitalSessionsBundle.BurstPattern(WritePatternName, timeoutInSeconds: 10);
        }

        /// <inheritdoc/>
        public void WriteRegisters(uint[] addresses, SiteData<long>[] values)
        {
            WriteRegisters(addresses, values, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="WriteRegisters(uint[], SiteData{long}[])"/>
        /// <param name="addressBitWidth">The bit width for each register address.</param>
        /// <param name="valueBitWidth">The bit width for each register value.</param>
        public void WriteRegisters(uint[] addresses, SiteData<long>[] values, uint addressBitWidth, uint valueBitWidth)
        {
            uint[][] perSiteWfmSamples = new uint[_activeSiteNumbers.Length][];
            for (int siteIndex = 0; siteIndex < _activeSiteNumbers.Length; siteIndex++)
            {
                long[] siteValues = new long[addresses.Length];
                for (int regIndex = 0; regIndex < addresses.Length; regIndex++)
                {
                    siteValues[regIndex] = values[regIndex].GetValue(_activeSiteNumbers[siteIndex]);
                }
                perSiteWfmSamples[siteIndex] = BuildInterleavedSourceWaveform(
                    addresses, siteValues, addressBitWidth, valueBitWidth);
            }
            SiteData<uint[]> sourceWaveforms = new SiteData<uint[]>(_activeSiteNumbers, perSiteWfmSamples);

            _digitalSessionsBundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, addresses.Length);
            _digitalSessionsBundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            _digitalSessionsBundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            _digitalSessionsBundle.WriteSourceWaveformSiteUnique(SourceWaveformName, sourceWaveforms);
            _digitalSessionsBundle.BurstPattern(WritePatternName, timeoutInSeconds: 10);
        }

        #endregion Multi Register Operations

        #region Sample Conversion Helpers

        private uint[] BuildInterleavedSourceWaveform(uint[] addresses, long[] values, uint addressBitWidth, uint valueBitWidth)
        {
            var allSamples = new List<uint>();
            for (int i = 0; i < addresses.Length; i++)
            {
                allSamples.AddRange(LongToU32Samples(addresses[i], addressBitWidth, SampleWidth, BitOrder));
                allSamples.AddRange(LongToU32Samples(values[i], valueBitWidth, SampleWidth, BitOrder));
            }
            return allSamples.ToArray();
        }

        internal static uint[] LongToU32Samples(long value, uint valueBitWidth, uint sampleBitWidth, BitOrder bitOrder = BitOrder.MsbFirst)
        {
            uint numSamples = GetSampleCount(valueBitWidth, sampleBitWidth);
            uint[] samplesArray = new uint[numSamples];
            uint sampleMask = sampleBitWidth >= 32 ? uint.MaxValue : (1u << (int)sampleBitWidth) - 1;
            for (int i = 0; i < samplesArray.Length; i++)
            {
                int sampleIndex = bitOrder == BitOrder.MsbFirst ? (int)numSamples - 1 - i : i;
                int shiftAmount = (int)(sampleBitWidth * (uint)sampleIndex);
                samplesArray[i] = (uint)((value >> shiftAmount) & sampleMask);
            }
            return samplesArray;
        }

        internal static uint[] LongArrayToU32Samples(long[] values, uint valueBitWidth, uint sampleBitWidth, BitOrder bitOrder = BitOrder.MsbFirst)
        {
            uint samplesPerValue = GetSampleCount(valueBitWidth, sampleBitWidth);
            uint[] result = new uint[values.Length * samplesPerValue];
            for (int v = 0; v < values.Length; v++)
            {
                uint[] samples = LongToU32Samples(values[v], valueBitWidth, sampleBitWidth, bitOrder);
                Array.Copy(samples, 0, result, v * (int)samplesPerValue, samples.Length);
            }
            return result;
        }

        internal static long U32SamplesToLong(uint[] samples, uint sampleBitWidth, BitOrder bitOrder = BitOrder.MsbFirst)
        {
            long value = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                int sampleIndex = bitOrder == BitOrder.MsbFirst ? samples.Length - 1 - i : i;
                int shiftAmount = (int)(sampleBitWidth * (uint)sampleIndex);
                value |= (long)samples[i] << shiftAmount;
            }
            return value;
        }

        internal static long[] U32SamplesToLongArray(uint[] samples, uint sampleBitWidth, int valueCount, BitOrder bitOrder = BitOrder.MsbFirst)
        {
            int samplesPerValue = samples.Length / valueCount;
            long[] values = new long[valueCount];
            for (int i = 0; i < valueCount; i++)
            {
                uint[] subset = new uint[samplesPerValue];
                Array.Copy(samples, i * samplesPerValue, subset, 0, samplesPerValue);
                values[i] = U32SamplesToLong(subset, sampleBitWidth, bitOrder);
            }
            return values;
        }

        private static uint GetSampleCount(uint valueBitWidth, uint sampleBitWidth)
        {
            return (valueBitWidth + sampleBitWidth - 1) / sampleBitWidth;
        }

        private static uint[] ConcatArrays(uint[] first, uint[] second)
        {
            uint[] result = new uint[first.Length + second.Length];
            first.CopyTo(result, 0);
            second.CopyTo(result, first.Length);
            return result;
        }

        #endregion Sample Conversion Helpers
    }
}
