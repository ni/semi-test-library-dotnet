using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;

using NIDigitalBitOrder = NationalInstruments.ModularInstruments.NIDigital.BitOrder;
using NIDigitalSourceDataMapping = NationalInstruments.ModularInstruments.NIDigital.SourceDataMapping;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl
{
    /// <summary>
    /// Abstract digital protocol class, used as a base for more specific digital protocol implementations.
    /// </summary>
    /// <remarks>
    /// Protocol instances (for example, <see cref="SPI.Instance"/>) hold shared configuration and the active
    /// <see cref="DigitalSessionsBundle"/>, so they are not thread-safe. Do not use one instance from multiple
    /// code modules or test sites at the same time.
    /// </remarks>
    public abstract class DigitalProtocol : IDigitalProtocol
    {
        /// <summary>
        /// The maximum number of registers that can be read or written in a single burst.
        /// </summary>
        public const int MaxRegistersPerBurst = 32;

        private const uint MaxSampleWidth = 32;
        private const uint MaxBitWidth = 64;

        // Tracks the waveforms created on each instrument session so they are created exactly once per session.
        private static readonly ConditionalWeakTable<object, Dictionary<string, string>> _createdWaveforms =
            new ConditionalWeakTable<object, Dictionary<string, string>>();

        private DigitalSessionsBundle _digitalSessionsBundle;
        private uint _sampleWidth = 8;
        private uint _defaultAddressBitWidth = 16;
        private uint _defaultValueBitWidth = 16;

        /// <inheritdoc/>
        public string WritePatternName { get; set; }

        /// <inheritdoc/>
        public string ReadPatternName { get; set; }

        /// <inheritdoc/>
        public string SourceWaveformName { get; set; } = "SPI_source_buffer";

        /// <inheritdoc/>
        public string CaptureWaveformName { get; set; } = "SPI_capture_buffer";

        /// <inheritdoc/>
        public string SourcePinName { get; set; } = "SDI";

        /// <inheritdoc/>
        public string CapturePinName { get; set; } = "SDO";

        /// <inheritdoc/>
        public uint SampleWidth
        {
            get => _sampleWidth;
            set
            {
                if (value < 1 || value > MaxSampleWidth)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"The sample width must be between 1 and {MaxSampleWidth}.");
                }
                _sampleWidth = value;
            }
        }

        /// <inheritdoc/>
        public string ReadWriteCountSequenceRegister { get; set; } = "reg0";

        /// <inheritdoc/>
        public string AddressBitWidthSequenceRegister { get; set; } = "reg1";

        /// <inheritdoc/>
        public string ValueBitWidthSequenceRegister { get; set; } = "reg2";

        /// <inheritdoc/>
        public IReadOnlyList<string> PinNames { get; set; } = new[] { "SDI", "SDO" };

        /// <inheritdoc/>
        public uint DefaultAddressBitWidth
        {
            get => _defaultAddressBitWidth;
            set
            {
                ValidateBitWidth(value, nameof(value));
                _defaultAddressBitWidth = value;
            }
        }

        /// <inheritdoc/>
        public uint DefaultValueBitWidth
        {
            get => _defaultValueBitWidth;
            set
            {
                ValidateBitWidth(value, nameof(value));
                _defaultValueBitWidth = value;
            }
        }

        /// <inheritdoc/>
        public BitOrder BitOrder { get; set; } = BitOrder.MsbFirst;

        private DigitalSessionsBundle Bundle => _digitalSessionsBundle
            ?? throw new InvalidOperationException(
                $"No {nameof(DigitalSessionsBundle)} is set. Call {nameof(SetBundle)} (or tsmContext.DutControl(...)) before accessing registers.");

        /// <inheritdoc/>
        public void SetBundle(DigitalSessionsBundle digitalSessionsBundle)
        {
            if (digitalSessionsBundle == null)
            {
                throw new ArgumentNullException(nameof(digitalSessionsBundle));
            }

            // It is important that the pins used by the bundle contain the pins used within the pattern.
            // This validation can only be done once we have a session to operate on.
            ValidateBundle(digitalSessionsBundle);
            CreateSerialWaveforms(digitalSessionsBundle);
            _digitalSessionsBundle = digitalSessionsBundle;
        }

        /// <inheritdoc/>
        public void ClearBundle()
        {
            _digitalSessionsBundle = null;
        }

        #region Single Register Operations

        /// <inheritdoc/>
        public SiteData<long> ReadRegister(uint address)
        {
            return ReadRegister(address, DefaultAddressBitWidth, DefaultValueBitWidth);
        }

        /// <inheritdoc cref="IDigitalProtocol.ReadRegister(uint)"/>
        /// <param name="address"/>
        /// <param name="addressBitWidth">The bit width for the register address.</param>
        /// <param name="valueBitWidth">The bit width for the register value.</param>
        public SiteData<long> ReadRegister(uint address, uint addressBitWidth, uint valueBitWidth)
        {
            ValidateRegisterAccess(address, addressBitWidth, valueBitWidth);

            // The driver defines a source waveform as a buffer of U32 samples, where each sample holds only as many
            // bits as the sample width the waveform was created with. The address must therefore be unpacked into
            // as many samples as are needed to send all of its bits. Only the address is sourced, because the read
            // pattern captures the register value from the DUT.
            uint[] srcWfmSamples = LongToU32Samples(address, addressBitWidth, SampleWidth, BitOrder);

            Bundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, 1);
            Bundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            Bundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            WriteSourceWaveformToAllSites(srcWfmSamples);
            Bundle.BurstPattern(ReadPatternName, timeoutInSeconds: 10);

            // The driver returns a capture waveform as a buffer of U32 samples,
            // with each sample containing only the number of bits specified by the waveform's sample width.
            // As a result, each register value may span multiple samples.
            // The samples must be packed together to reconstruct a single register value.
            uint samplesPerValue = GetSampleCount(valueBitWidth, SampleWidth);
            SiteData<uint[]> captureWaveforms = Bundle.FetchCaptureWaveform(
                CaptureWaveformName, samplesToRead: (int)samplesPerValue, timeoutInSeconds: 10);

            return captureWaveforms.Select(x => U32SamplesToLong(x, SampleWidth, BitOrder));
        }

        /// <inheritdoc/>
        public void WriteRegister(uint address, long value)
        {
            WriteRegister(address, value, DefaultAddressBitWidth, DefaultValueBitWidth);
        }

        /// <inheritdoc cref="IDigitalProtocol.WriteRegister(uint, long)"/>
        /// <param name="address"/>
        /// <param name="value"/>
        /// <param name="addressBitWidth">The bit width for the register address.</param>
        /// <param name="valueBitWidth">The bit width for the register value.</param>
        public void WriteRegister(uint address, long value, uint addressBitWidth, uint valueBitWidth)
        {
            ValidateRegisterAccess(address, addressBitWidth, valueBitWidth);
            ValidateValue(value, valueBitWidth);

            // The driver defines a source waveform as a buffer of U32 samples, where each sample holds only as many
            // bits as the sample width the waveform was created with. Both the address and the value must therefore
            // be unpacked into as many samples as are needed to send all of their bits. They are concatenated
            // because the write pattern sources the address and then the value from the same waveform.
            uint[] addressSamples = LongToU32Samples(address, addressBitWidth, SampleWidth, BitOrder);
            uint[] valueSamples = LongToU32Samples(value, valueBitWidth, SampleWidth, BitOrder);
            uint[] srcWfmSamples = ConcatArrays(addressSamples, valueSamples);

            Bundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, 1);
            Bundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            Bundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            WriteSourceWaveformToAllSites(srcWfmSamples);
            Bundle.BurstPattern(WritePatternName, timeoutInSeconds: 10);
        }

        /// <inheritdoc/>
        public void WriteRegister(uint address, SiteData<long> value)
        {
            WriteRegister(address, value, DefaultAddressBitWidth, DefaultValueBitWidth);
        }

        /// <inheritdoc cref="IDigitalProtocol.WriteRegister(uint, SiteData{long})"/>
        /// <param name="address"/>
        /// <param name="value"/>
        /// <param name="addressBitWidth">The bit width for the register address.</param>
        /// <param name="valueBitWidth">The bit width for the register value.</param>
        public void WriteRegister(uint address, SiteData<long> value, uint addressBitWidth, uint valueBitWidth)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            ValidateRegisterAccess(address, addressBitWidth, valueBitWidth);

            int[] siteNumbers = value.SiteNumbers;

            // The address and value must be unpacked into U32 samples for the same reason as the overload above.
            // Because the value can differ per site, a separate source waveform must be built for each site and
            // written site-unique.
            uint[] addressSamples = LongToU32Samples(address, addressBitWidth, SampleWidth, BitOrder);
            uint[][] perSiteWfmSamples = new uint[siteNumbers.Length][];
            for (int i = 0; i < siteNumbers.Length; i++)
            {
                long siteValue = value.GetValue(siteNumbers[i]);
                ValidateValue(siteValue, valueBitWidth);
                uint[] valueSamples = LongToU32Samples(siteValue, valueBitWidth, SampleWidth, BitOrder);
                perSiteWfmSamples[i] = ConcatArrays(addressSamples, valueSamples);
            }
            SiteData<uint[]> sourceWaveforms = new SiteData<uint[]>(siteNumbers, perSiteWfmSamples);

            Bundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, 1);
            Bundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            Bundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            WriteSourceWaveform(sourceWaveforms);
            Bundle.BurstPattern(WritePatternName, timeoutInSeconds: 10);
        }

        #endregion Single Register Operations

        #region Multi Register Operations

        /// <inheritdoc/>
        public SiteData<long[]> ReadRegisters(uint[] addresses)
        {
            return ReadRegisters(addresses, DefaultAddressBitWidth, DefaultValueBitWidth);
        }

        /// <inheritdoc cref="IDigitalProtocol.ReadRegisters(uint[])"/>
        /// <param name="addresses"/>
        /// <param name="addressBitWidth">The bit width for each register address.</param>
        /// <param name="valueBitWidth">The bit width for each register value.</param>
        public SiteData<long[]> ReadRegisters(uint[] addresses, uint addressBitWidth, uint valueBitWidth)
        {
            ValidateMultiRegisterAccess(addresses, addressBitWidth, valueBitWidth);

            // The driver defines a source waveform as a buffer of U32 samples, where each sample holds only as many
            // bits as the sample width the waveform was created with, so each address must be unpacked into as many
            // samples as are needed to send all of its bits. The addresses are placed back to back because the read
            // pattern loops once per register within a single burst, sourcing the next address on each iteration.
            foreach (uint address in addresses)
            {
                ValidateAddress(address, addressBitWidth);
            }
            uint[] srcWfmSamples = LongArrayToU32Samples(
                addresses.Select(address => (long)address).ToArray(), addressBitWidth, SampleWidth, BitOrder);

            Bundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, addresses.Length);
            Bundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            Bundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            WriteSourceWaveformToAllSites(srcWfmSamples);
            Bundle.BurstPattern(ReadPatternName, timeoutInSeconds: 10);

            // The driver returns a capture waveform as a buffer of U32 samples,
            // with each sample containing only the number of bits specified by the waveform's sample width.
            // As a result, each register value may span multiple samples.
            // The samples must be packed together to reconstruct a single register value.
            // Since register values are captured back-to-back, with one set of samples per loop iteration,
            // the aggregate samples captured during the pattern burst represent multiple register values
            // and must first be divided into groups by register before packing.
            uint samplesPerValue = GetSampleCount(valueBitWidth, SampleWidth);
            int totalSamples = (int)(samplesPerValue * (uint)addresses.Length);
            SiteData<uint[]> captureWaveforms = Bundle.FetchCaptureWaveform(
                CaptureWaveformName, samplesToRead: totalSamples, timeoutInSeconds: 10);

            return captureWaveforms.Select(x => U32SamplesToLongArray(x, SampleWidth, addresses.Length, BitOrder));
        }

        /// <inheritdoc/>
        public void WriteRegisters(uint[] addresses, long[] values)
        {
            WriteRegisters(addresses, values, DefaultAddressBitWidth, DefaultValueBitWidth);
        }

        /// <inheritdoc cref="IDigitalProtocol.WriteRegisters(uint[], long[])"/>
        /// <param name="addresses"/>
        /// <param name="values"/>
        /// <param name="addressBitWidth">The bit width for each register address.</param>
        /// <param name="valueBitWidth">The bit width for each register value.</param>
        public void WriteRegisters(uint[] addresses, long[] values, uint addressBitWidth, uint valueBitWidth)
        {
            ValidateMultiRegisterAccess(addresses, addressBitWidth, valueBitWidth);
            ValidateValuesMatchAddresses(addresses, values);

            // The driver defines a source waveform as a buffer of U32 samples, where each sample holds only as many
            // bits as the sample width the waveform was created with, so each address and value must be unpacked into
            // as many samples as are needed to send all of their bits. The samples are interleaved because the write
            // pattern loops once per register within a single burst, sourcing an address and then its value on each
            // iteration.
            uint[] srcWfmSamples = BuildInterleavedSourceWaveform(addresses, values, addressBitWidth, valueBitWidth);

            Bundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, addresses.Length);
            Bundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            Bundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            WriteSourceWaveformToAllSites(srcWfmSamples);
            Bundle.BurstPattern(WritePatternName, timeoutInSeconds: 10);
        }

        /// <inheritdoc/>
        public void WriteRegisters(uint[] addresses, SiteData<long>[] values)
        {
            WriteRegisters(addresses, values, DefaultAddressBitWidth, DefaultValueBitWidth);
        }

        /// <inheritdoc cref="IDigitalProtocol.WriteRegisters(uint[], SiteData{long}[])"/>
        /// <param name="addresses"/>
        /// <param name="values"/>
        /// <param name="addressBitWidth">The bit width for each register address.</param>
        /// <param name="valueBitWidth">The bit width for each register value.</param>
        public void WriteRegisters(uint[] addresses, SiteData<long>[] values, uint addressBitWidth, uint valueBitWidth)
        {
            ValidateMultiRegisterAccess(addresses, addressBitWidth, valueBitWidth);
            ValidateValuesMatchAddresses(addresses, values);
            ValidateSameSites(values);

            // The addresses and values must be unpacked into interleaved U32 samples for the same reason as the
            // overload above. Because the values can differ per site, a separate source waveform must be built for
            // each site and written site-unique.
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

            Bundle.WriteSequencerRegister(ReadWriteCountSequenceRegister, addresses.Length);
            Bundle.WriteSequencerRegister(AddressBitWidthSequenceRegister, (int)addressBitWidth);
            Bundle.WriteSequencerRegister(ValueBitWidthSequenceRegister, (int)valueBitWidth);
            WriteSourceWaveform(sourceWaveforms);
            Bundle.BurstPattern(WritePatternName, timeoutInSeconds: 10);
        }

        #endregion Multi Register Operations

        #region Sample Conversion Helpers

        // Writes the same samples to every active site using the SiteUnique source waveform. This lets the
        // broadcast-style operations share the single SiteUnique waveform used by the site-unique operations.
        private void WriteSourceWaveformToAllSites(uint[] samples)
        {
            int[] siteNumbers = Bundle.AggregateSitePinList
                .Select(sitePinInfo => sitePinInfo.SiteNumber)
                .Distinct()
                .ToArray();
            uint[][] perSiteSamples = new uint[siteNumbers.Length][];
            for (int i = 0; i < siteNumbers.Length; i++)
            {
                perSiteSamples[i] = samples;
            }
            WriteSourceWaveform(new SiteData<uint[]>(siteNumbers, perSiteSamples));
        }

        // The driver fixes the size of a source waveform on its first write in a session and rejects later writes
        // of a different size. Every write is therefore padded to the same size, which is large enough for a burst
        // of the maximum number of registers at the maximum address and value bit widths. The patterns only
        // consume the samples they need, so the padding is never sourced. For the same reason, the instrument
        // setup must not apply the waveform file data (applySourceWaveformData = false), as that would fix the
        // waveform to the size of the file data before the first register operation.
        private void WriteSourceWaveform(SiteData<uint[]> perSiteSamples)
        {
            Bundle.WriteSourceWaveformSiteUnique(
                SourceWaveformName, perSiteSamples, expandToMinimumSize: true, minimumSize: GetSourceWaveformSize(SampleWidth));
        }

        internal static int GetSourceWaveformSize(uint sampleBitWidth)
        {
            return MaxRegistersPerBurst * 2 * (int)GetSampleCount(MaxBitWidth, sampleBitWidth);
        }

        private uint[] BuildInterleavedSourceWaveform(uint[] addresses, long[] values, uint addressBitWidth, uint valueBitWidth)
        {
            var allSamples = new List<uint>();
            for (int i = 0; i < addresses.Length; i++)
            {
                ValidateAddress(addresses[i], addressBitWidth);
                ValidateValue(values[i], valueBitWidth);
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
            if (valueCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(valueCount), valueCount, "The value count must be greater than zero.");
            }
            if (samples.Length % valueCount != 0)
            {
                throw new ArgumentException($"The {samples.Length} samples cannot be split evenly into {valueCount} values.", nameof(samples));
            }

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
            if (sampleBitWidth < 1 || sampleBitWidth > MaxSampleWidth)
            {
                throw new ArgumentOutOfRangeException(nameof(sampleBitWidth), sampleBitWidth, $"The sample width must be between 1 and {MaxSampleWidth}.");
            }
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

        #region Validation Helpers

        internal static void ValidateBitWidth(uint bitWidth, string paramName)
        {
            if (bitWidth < 1 || bitWidth > MaxBitWidth)
            {
                throw new ArgumentOutOfRangeException(paramName, bitWidth, $"The bit width must be between 1 and {MaxBitWidth}.");
            }
        }

        internal static void ValidateAddress(uint address, uint addressBitWidth)
        {
            // Addresses are 32 bits wide, so any wider field always fits.
            if (addressBitWidth < 32 && (address >> (int)addressBitWidth) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(address), address, $"The address 0x{address:X} does not fit in {addressBitWidth} bits.");
            }
        }

        internal static void ValidateValue(long value, uint valueBitWidth)
        {
            // Negative values are rejected unless the field spans all 64 bits.
            if (valueBitWidth < MaxBitWidth && ((ulong)value >> (int)valueBitWidth) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"The value {value} does not fit in {valueBitWidth} bits.");
            }
        }

        internal static void ValidateAddresses(uint[] addresses)
        {
            if (addresses == null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }
            if (addresses.Length == 0)
            {
                throw new ArgumentException("At least one register address is required.", nameof(addresses));
            }
            if (addresses.Length > MaxRegistersPerBurst)
            {
                throw new ArgumentException(
                    $"At most {MaxRegistersPerBurst} registers can be accessed in a single burst, but {addresses.Length} addresses were provided.",
                    nameof(addresses));
            }
        }

        internal static void ValidateValuesMatchAddresses(uint[] addresses, Array values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            if (values.Length != addresses.Length)
            {
                throw new ArgumentException(
                    $"Addresses and values must have equal lengths, but {addresses.Length} addresses and {values.Length} values were provided.",
                    nameof(values));
            }
        }

        internal static void ValidateSameSites(SiteData<long>[] values)
        {
            if (values.Any(v => v == null))
            {
                throw new ArgumentException("The values must not contain null entries.", nameof(values));
            }
            var expectedSites = new HashSet<int>(values[0].SiteNumbers);
            if (values.Any(v => !expectedSites.SetEquals(v.SiteNumbers)))
            {
                throw new ArgumentException("All site-unique values must contain the same set of sites.", nameof(values));
            }
        }

        private static void ValidateRegisterAccess(uint address, uint addressBitWidth, uint valueBitWidth)
        {
            ValidateBitWidth(addressBitWidth, nameof(addressBitWidth));
            ValidateBitWidth(valueBitWidth, nameof(valueBitWidth));
            ValidateAddress(address, addressBitWidth);
        }

        private static void ValidateMultiRegisterAccess(uint[] addresses, uint addressBitWidth, uint valueBitWidth)
        {
            ValidateBitWidth(addressBitWidth, nameof(addressBitWidth));
            ValidateBitWidth(valueBitWidth, nameof(valueBitWidth));
            ValidateAddresses(addresses);
        }

        #endregion Validation Helpers

        /// <summary>
        /// Creates the serial source and capture waveforms on each session in the bundle, as they must exist on the
        /// instrument before they can be written or bursted. The waveforms may already exist on the session, for
        /// example when they are loaded from the waveform files in the digital pattern project during instrument
        /// initialization. Creating a waveform that already exists with the same settings has no effect, whereas
        /// creating it with different settings (pin, sample width, or bit order) causes the driver to throw an
        /// exception, as waveform settings cannot change after creation. Creation is tracked per session to avoid
        /// repeating it on every call; a fresh session (for example, a second sequence run after instrument cleanup)
        /// gets its own waveforms.
        /// </summary>
        /// <param name="digitalSessionsBundle">The <see cref="DigitalSessionsBundle"/> to create the waveforms on.</param>
        private void CreateSerialWaveforms(DigitalSessionsBundle digitalSessionsBundle)
        {
            NIDigitalBitOrder bitOrder = BitOrder == BitOrder.MsbFirst
                ? NIDigitalBitOrder.MostSignificantBitFirst
                : NIDigitalBitOrder.LeastSignificantBitFirst;
            string sourceKey = "source:" + SourceWaveformName;
            string sourceSettings = $"pin={SourcePinName}, sampleWidth={SampleWidth}, bitOrder={bitOrder}";
            string captureKey = "capture:" + CaptureWaveformName;
            string captureSettings = $"pin={CapturePinName}, sampleWidth={SampleWidth}, bitOrder={bitOrder}";

            digitalSessionsBundle.Do(sessionInfo =>
            {
                Dictionary<string, string> created = _createdWaveforms.GetOrCreateValue(sessionInfo.Session);
                lock (created)
                {
                    if (!IsWaveformCreated(created, sourceKey, sourceSettings))
                    {
                        // SiteUnique supports both broadcast-style writes (same data to every site) and
                        // site-unique writes with a single waveform name; a Broadcast waveform cannot be
                        // written with WriteSourceWaveformSiteUnique.
                        sessionInfo.Session.SourceWaveforms.CreateSerial(
                            SourcePinName, SourceWaveformName, NIDigitalSourceDataMapping.SiteUnique, SampleWidth, bitOrder);
                        created[sourceKey] = sourceSettings;
                    }
                    if (!IsWaveformCreated(created, captureKey, captureSettings))
                    {
                        sessionInfo.Session.CaptureWaveforms.CreateSerial(
                            CapturePinName, CaptureWaveformName, SampleWidth, bitOrder);
                        created[captureKey] = captureSettings;
                    }
                }
            });
        }

        private static bool IsWaveformCreated(Dictionary<string, string> created, string key, string settings)
        {
            if (!created.TryGetValue(key, out string existingSettings))
            {
                return false;
            }
            if (existingSettings != settings)
            {
                throw new InvalidOperationException(
                    $"The waveform '{key}' already exists on the session with different settings ({existingSettings}). " +
                    $"Waveform settings cannot change after creation; use a different waveform name for ({settings}).");
            }
            return true;
        }

        /// <summary>
        /// Validates that the pins in <see cref="IDigitalProtocol.PinNames"/>, <see cref="IDigitalProtocol.SourcePinName"/>
        /// and <see cref="IDigitalProtocol.CapturePinName"/> are present in the bundle.
        /// A superset of pins in the bundle is acceptable.
        /// </summary>
        /// <param name="digitalSessionsBundle">The <see cref="DigitalSessionsBundle"/> object to validate.</param>
        /// <exception cref="ArgumentException">Thrown when required pins are missing from the bundle.</exception>
        private void ValidateBundle(DigitalSessionsBundle digitalSessionsBundle)
        {
            // It is acceptable for there to be a superset of pins within the bundle.
            var bundlePins = new HashSet<string>(
                digitalSessionsBundle.AggregateSitePinList.Select(sp => sp.PinName),
                StringComparer.Ordinal);
            string[] missingPins = (PinNames ?? Array.Empty<string>())
                .Concat(new[] { SourcePinName, CapturePinName })
                .Where(pin => !bundlePins.Contains(pin))
                .Distinct()
                .ToArray();
            if (missingPins.Length > 0)
            {
                throw new ArgumentException(
                    $"The bundle must include a source pin named: {SourcePinName}, and a capture pin named: {CapturePinName}.");
            }
        }
    }
}
