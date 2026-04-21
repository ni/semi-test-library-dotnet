using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.SPI
{
    /// <summary>
    /// Defines methods for reading and writing to DUT registers via digital patterns
    /// using source and capture waveforms. This class abstracts the low-level waveform
    /// encoding/decoding so that callers only need to specify register addresses and data values.
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
    /// var dutDigiIO = new DUTRegisterIO(tsmContext);
    /// dutDigiIO.WriteRegister(address: 0x48, value: 4);
    /// SiteData&lt;long&gt; readBack = dutDigiIO.ReadRegister(address: 0x48);
    /// </code>
    /// </para>
    /// </remarks>
    public class DUTRegisterIO
    {
        /// <summary>
        /// The digital sessions bundle, containing the digital pattern instrument sessions,
        /// for each of the digital pattern pins and sites in the current context.
        /// </summary>
        private readonly DigitalSessionsBundle _digitalSessionsBundle;

        /// <summary>
        /// The active site numbers for the current context.
        /// </summary>
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
        /// Constructs a <see cref="DUTRegisterIO"/> object to read and write to DUT registers.
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
        public DUTRegisterIO(
            ISemiconductorModuleContext semiconductorModuleContext,
            uint addressBitWidth = 16,
            uint valueBitWidth = 16,
            string readPatternName = "SPI_read_template",
            string writePatternName = "SPI_read_template",
            uint sampleWidth = 8,
            string captureWaveformName = "capture_buffer",
            string sourceWaveformName = "source_buffer",
            string readWriteCountSequenceRegister = "reg0",
            string addressBitWidthSequenceRegister = "reg1",
            string valueBitWidthSequenceRegister = "reg2")
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

            var sm = new TSMSessionManager(semiconductorModuleContext);
            _digitalSessionsBundle = sm.Digital();
            _activeSiteNumbers = semiconductorModuleContext.SiteNumbers.ToArray();
        }

        #region Single Register Operations

        /// <summary>
        /// Reads the value at the specified DUT register address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>The per-site value read from the register.</returns>
        public SiteData<long> ReadRegister(uint address)
        {
            return ReadRegister(address, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="ReadRegister(uint)"/>
        /// <remarks>
        /// Use this overload to specify a specific bit size for the register's address and value,
        /// instead of using the values configured when the <see cref="DUTRegisterIO"/> object was created.
        /// </remarks>
        /// <param name="addressBitWidth">The bit width for the register address.</param>
        /// <param name="valueBitWidth">The bit width for the register value.</param>
        public SiteData<long> ReadRegister(uint address, uint addressBitWidth, uint valueBitWidth)
        {
            // Pad source waveform with dummy value samples so the total size matches WriteRegister.
            // The driver requires all writes to the same source waveform to use the same size.
            uint[] addressSamples = LongToU32Samples(address, addressBitWidth, SampleWidth);
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

            SiteData<long> capturedRegisterValue = captureWaveforms.Select(x => U32SamplesToLong(x, SampleWidth));
            return capturedRegisterValue;
        }

        /// <summary>
        /// Writes the specified value to the specified DUT register address.
        /// </summary>
        /// <param name="address">The address of the register to write the value to.</param>
        /// <param name="value">The value to write to the register.</param>
        public void WriteRegister(uint address, long value)
        {
            WriteRegister(address, value, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="WriteRegister(uint, long)"/>
        /// <remarks>
        /// Use this overload to specify a specific bit size for the register's address and value,
        /// instead of using the values configured when the <see cref="DUTRegisterIO"/> object was created.
        /// </remarks>
        /// <param name="addressBitWidth">The bit width for the register address.</param>
        /// <param name="valueBitWidth">The bit width for the register value.</param>
        public void WriteRegister(uint address, long value, uint addressBitWidth, uint valueBitWidth)
        {
            uint[] addressSamples = LongToU32Samples(address, addressBitWidth, SampleWidth);
            uint[] valueSamples = LongToU32Samples(value, valueBitWidth, SampleWidth);
            uint[] srcWfmSamples = ConcatArrays(addressSamples, valueSamples);

            _digitalSessionsBundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, 1);
            _digitalSessionsBundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            _digitalSessionsBundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            _digitalSessionsBundle.WriteSourceWaveformBroadcast(SourceWaveformName, srcWfmSamples);
            _digitalSessionsBundle.BurstPattern(WritePatternName, timeoutInSeconds: 10);
        }

        /// <summary>
        /// Writes the specified site-unique value to the specified DUT register address.
        /// </summary>
        /// <param name="address">The address of the register to write the value to.</param>
        /// <param name="value">The per-site value to write to the register.</param>
        public void WriteRegister(uint address, SiteData<long> value)
        {
            WriteRegister(address, value, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="WriteRegister(uint, SiteData{long})"/>
        /// <remarks>
        /// Use this overload to specify a specific bit size for the register's address and value,
        /// instead of using the values configured when the <see cref="DUTRegisterIO"/> object was created.
        /// </remarks>
        /// <param name="addressBitWidth">The bit width for the register address.</param>
        /// <param name="valueBitWidth">The bit width for the register value.</param>
        public void WriteRegister(uint address, SiteData<long> value, uint addressBitWidth, uint valueBitWidth)
        {
            uint[] addressSamples = LongToU32Samples(address, addressBitWidth, SampleWidth);
            uint[][] perSiteWfmSamples = new uint[_activeSiteNumbers.Length][];
            for (int i = 0; i < _activeSiteNumbers.Length; i++)
            {
                uint[] valueSamples = LongToU32Samples(value.GetValue(_activeSiteNumbers[i]), valueBitWidth, SampleWidth);
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

        /// <summary>
        /// Reads the values from the specified DUT register addresses.
        /// </summary>
        /// <param name="addresses">The addresses of the registers to read.</param>
        /// <returns>The per-site values read from the registers.</returns>
        public SiteData<long[]> ReadRegisters(uint[] addresses)
        {
            return ReadRegisters(addresses, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="ReadRegisters(uint[])"/>
        /// <remarks>
        /// Use this overload to specify register-unique bit sizes for the register's addresses and values,
        /// instead of using the values configured when the <see cref="DUTRegisterIO"/> object was created.
        /// </remarks>
        /// <param name="addressBitWidth">The bit width for each register address.</param>
        /// <param name="valueBitWidth">The bit width for each register value.</param>
        public SiteData<long[]> ReadRegisters(uint[] addresses, uint addressBitWidth, uint valueBitWidth)
        {
            // Build interleaved source waveform with dummy value samples to match WriteRegisters size.
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

            SiteData<long[]> capturedRegisterValues = captureWaveforms.Select(
                x => U32SamplesToLongArray(x, SampleWidth, addresses.Length));
            return capturedRegisterValues;
        }

        /// <summary>
        /// Writes the specified register-unique values to the specified register addresses.
        /// </summary>
        /// <param name="addresses">The addresses of each register to write the values to.</param>
        /// <param name="values">The register-unique values to write to each register.</param>
        public void WriteRegisters(uint[] addresses, long[] values)
        {
            WriteRegisters(addresses, values, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="WriteRegisters(uint[], long[])"/>
        /// <remarks>
        /// Use this overload to specify register-unique bit sizes for the register's addresses and values,
        /// instead of using the values configured when the <see cref="DUTRegisterIO"/> object was created.
        /// </remarks>
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

        /// <summary>
        /// Writes the specified site-unique values to the specified register addresses.
        /// </summary>
        /// <param name="addresses">The addresses of each register to write the values to.</param>
        /// <param name="values">The per-site values to write to each register.</param>
        public void WriteRegisters(uint[] addresses, SiteData<long>[] values)
        {
            WriteRegisters(addresses, values, AddressBitWidth, ValueBitWidth);
        }

        /// <inheritdoc cref="WriteRegisters(uint[], SiteData{long}[])"/>
        /// <remarks>
        /// Use this overload to specify register-unique bit sizes for the register's addresses and values,
        /// instead of using the values configured when the <see cref="DUTRegisterIO"/> object was created.
        /// </remarks>
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

        /// <summary>
        /// Builds an interleaved source waveform for a multi-register write operation.
        /// The output format is: [addr0_samples, val0_samples, addr1_samples, val1_samples, ...].
        /// </summary>
        private uint[] BuildInterleavedSourceWaveform(uint[] addresses, long[] values, uint addressBitWidth, uint valueBitWidth)
        {
            var allSamples = new List<uint>();
            for (int i = 0; i < addresses.Length; i++)
            {
                allSamples.AddRange(LongToU32Samples(addresses[i], addressBitWidth, SampleWidth));
                allSamples.AddRange(LongToU32Samples(values[i], valueBitWidth, SampleWidth));
            }
            return allSamples.ToArray();
        }

        /// <summary>
        /// Unpacks a long value into an array of uint samples, where each sample
        /// holds <paramref name="sampleBitWidth"/> bits of the value (LSB first).
        /// </summary>
        /// <param name="value">The value to unpack.</param>
        /// <param name="valueBitWidth">The actual number of bits the value occupies.</param>
        /// <param name="sampleBitWidth">The number of bits each uint sample holds.</param>
        /// <returns>An array of uint samples.</returns>
        internal static uint[] LongToU32Samples(long value, uint valueBitWidth, uint sampleBitWidth)
        {
            uint numSamples = GetSampleCount(valueBitWidth, sampleBitWidth);
            uint[] samplesArray = new uint[numSamples];
            uint sampleMask = sampleBitWidth >= 32 ? uint.MaxValue : (1u << (int)sampleBitWidth) - 1;
            for (int i = 0; i < samplesArray.Length; i++)
            {
                int shiftAmount = (int)(sampleBitWidth * (uint)i);
                samplesArray[i] = (uint)((value >> shiftAmount) & sampleMask);
            }
            return samplesArray;
        }

        /// <summary>
        /// Unpacks multiple long values into a single array of uint samples.
        /// Each value is unpacked using <see cref="LongToU32Samples"/> and the results are concatenated.
        /// </summary>
        /// <param name="values">The values to unpack.</param>
        /// <param name="valueBitWidth">The actual number of bits each value occupies.</param>
        /// <param name="sampleBitWidth">The number of bits each uint sample holds.</param>
        /// <returns>A concatenated array of uint samples for all values.</returns>
        internal static uint[] LongArrayToU32Samples(long[] values, uint valueBitWidth, uint sampleBitWidth)
        {
            uint samplesPerValue = GetSampleCount(valueBitWidth, sampleBitWidth);
            uint[] result = new uint[values.Length * samplesPerValue];
            for (int v = 0; v < values.Length; v++)
            {
                uint[] samples = LongToU32Samples(values[v], valueBitWidth, sampleBitWidth);
                Array.Copy(samples, 0, result, v * (int)samplesPerValue, samples.Length);
            }
            return result;
        }

        /// <summary>
        /// Packs an array of uint samples into a single long value.
        /// Each sample contributes <paramref name="sampleBitWidth"/> bits (LSB first).
        /// </summary>
        /// <param name="samples">The array of uint samples to pack.</param>
        /// <param name="sampleBitWidth">The number of bits each uint sample holds.</param>
        /// <returns>The packed long value.</returns>
        internal static long U32SamplesToLong(uint[] samples, uint sampleBitWidth)
        {
            long value = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                int shiftAmount = (int)(sampleBitWidth * (uint)i);
                value |= (long)samples[i] << shiftAmount;
            }
            return value;
        }

        /// <summary>
        /// Packs an array of uint samples into multiple long values.
        /// The samples are divided evenly among the specified number of values.
        /// </summary>
        /// <param name="samples">The array of uint samples to pack.</param>
        /// <param name="sampleBitWidth">The number of bits each uint sample holds.</param>
        /// <param name="valueCount">The number of values contained within the samples array.</param>
        /// <returns>An array of packed long values.</returns>
        internal static long[] U32SamplesToLongArray(uint[] samples, uint sampleBitWidth, int valueCount)
        {
            int samplesPerValue = samples.Length / valueCount;
            long[] values = new long[valueCount];
            for (int i = 0; i < valueCount; i++)
            {
                uint[] subset = new uint[samplesPerValue];
                Array.Copy(samples, i * samplesPerValue, subset, 0, samplesPerValue);
                values[i] = U32SamplesToLong(subset, sampleBitWidth);
            }
            return values;
        }

        /// <summary>
        /// Gets the number of samples needed to represent a value of the given bit width.
        /// </summary>
        private static uint GetSampleCount(uint valueBitWidth, uint sampleBitWidth)
        {
            return (valueBitWidth + sampleBitWidth - 1) / sampleBitWidth;
        }

        /// <summary>
        /// Concatenates two uint arrays.
        /// </summary>
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
