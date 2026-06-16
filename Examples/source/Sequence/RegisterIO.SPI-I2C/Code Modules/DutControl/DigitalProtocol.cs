using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl
{
    /// <summary>
    /// Abstract digital protocol class, used as a base for more specific digital protocol implementations.
    /// </summary>
    public abstract class DigitalProtocol : IDigitalProtocol
    {
        protected DigitalSessionsBundle _digitalSessionsBundle;

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalProtocol"/> class.
        /// </summary>
        protected DigitalProtocol() { }

        /// <inheritdoc/>
        public string WritePatternName { get; set; }

        /// <inheritdoc/>
        public string ReadPatternName { get; set; }

        /// <inheritdoc/>
        public string SourceWaveformName { get; set; } = "source_buffer";

        /// <inheritdoc/>
        public string CaptureWaveformName { get; set; } = "capture_buffer";

        /// <inheritdoc/>
        public uint SampleWidth { get; set; } = 8;

        /// <inheritdoc/>
        public string ReadWriteCountSequenceRegister { get; set; } = "reg0";

        /// <inheritdoc/>
        public string AddressBitWidthSequenceRegister { get; set; } = "reg1";

        /// <inheritdoc/>
        public string ValueBitWidthSequenceRegister { get; set; } = "reg2";

        /// <inheritdoc/>
        public string[] PinNames { get; set; } = new[] { "SDI", "SDO" };

        /// <inheritdoc/>
        public uint DefaultAddressBitWidth { get; set; } = 16;

        /// <inheritdoc/>
        public uint DefaultValueBitWidth { get; set; } = 16;

        /// <inheritdoc/>
        public BitOrder BitOrder { get; set; } = BitOrder.MsbFirst;

        /// <inheritdoc/>
        public void SetBundle(DigitalSessionsBundle digitalSessionsBundle)
        {
            // It is important that the pins used by the bundle contain the pins used within the pattern.
            // This validation can only be done once we have a session to operate on.
            ValidateBundle(digitalSessionsBundle);
            _digitalSessionsBundle = digitalSessionsBundle;
        }

        #region Single Register Operations

        /// <inheritdoc/>
        public SiteData<long> ReadRegister(uint address)
        {
            return ReadRegister(address, DefaultAddressBitWidth, DefaultValueBitWidth);
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
            WriteRegister(address, value, DefaultAddressBitWidth, DefaultValueBitWidth);
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
            WriteRegister(address, value, DefaultAddressBitWidth, DefaultValueBitWidth);
        }

        /// <inheritdoc cref="WriteRegister(uint, SiteData{long})"/>
        /// <param name="addressBitWidth">The bit width for the register address.</param>
        /// <param name="valueBitWidth">The bit width for the register value.</param>
        public void WriteRegister(uint address, SiteData<long> value, uint addressBitWidth, uint valueBitWidth)
        {
            int[] siteNumbers = value.SiteNumbers;
            uint[] addressSamples = LongToU32Samples(address, addressBitWidth, SampleWidth, BitOrder);
            uint[][] perSiteWfmSamples = new uint[siteNumbers.Length][];
            for (int i = 0; i < siteNumbers.Length; i++)
            {
                uint[] valueSamples = LongToU32Samples(value.GetValue(siteNumbers[i]), valueBitWidth, SampleWidth, BitOrder);
                perSiteWfmSamples[i] = ConcatArrays(addressSamples, valueSamples);
            }
            SiteData<uint[]> sourceWaveforms = new SiteData<uint[]>(siteNumbers, perSiteWfmSamples);

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
            return ReadRegisters(addresses, DefaultAddressBitWidth, DefaultValueBitWidth);
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
            WriteRegisters(addresses, values, DefaultAddressBitWidth, DefaultValueBitWidth);
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
            WriteRegisters(addresses, values, DefaultAddressBitWidth, DefaultValueBitWidth);
        }

        /// <inheritdoc cref="WriteRegisters(uint[], SiteData{long}[])"/>
        /// <param name="addressBitWidth">The bit width for each register address.</param>
        /// <param name="valueBitWidth">The bit width for each register value.</param>
        public void WriteRegisters(uint[] addresses, SiteData<long>[] values, uint addressBitWidth, uint valueBitWidth)
        {
            int[] siteNumbers = values[0].SiteNumbers;
            uint[][] perSiteWfmSamples = new uint[siteNumbers.Length][];
            for (int siteIndex = 0; siteIndex < siteNumbers.Length; siteIndex++)
            {
                long[] siteValues = new long[addresses.Length];
                for (int regIndex = 0; regIndex < addresses.Length; regIndex++)
                {
                    siteValues[regIndex] = values[regIndex].GetValue(siteNumbers[siteIndex]);
                }
                perSiteWfmSamples[siteIndex] = BuildInterleavedSourceWaveform(
                    addresses, siteValues, addressBitWidth, valueBitWidth);
            }
            SiteData<uint[]> sourceWaveforms = new SiteData<uint[]>(siteNumbers, perSiteWfmSamples);

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

        /// <summary>
        /// Validates that all pins in <see cref="IDigitalProtocol.PinNames"/> are present in the bundle.
        /// A superset of pins in the bundle is acceptable.
        /// </summary>
        /// <param name="digitalSessionsBundle">The <see cref="DigitalSessionsBundle"/> object to validate.</param>
        /// <exception cref="ArgumentException">Thrown when required pins are missing from the bundle.</exception>
        private void ValidateBundle(DigitalSessionsBundle digitalSessionsBundle)
        {
            if (PinNames == null || PinNames.Length == 0)
            {
                return;
            }

            // Check that all expected pins are contained within the bundle pins.
            // It is acceptable for there to be a superset of pins within the bundle.
            var bundlePins = new HashSet<string>(
                digitalSessionsBundle.AggregateSitePinList.Select(sp => sp.PinName),
                StringComparer.Ordinal);
            if (!PinNames.All(bundlePins.Contains))
            {
                throw new ArgumentException(
                    $"The DigitalSessionsBundle does not contain the required pins defined by " +
                    $"PinNames: [{string.Join(", ", PinNames)}].");
            }
        }
    }
}
