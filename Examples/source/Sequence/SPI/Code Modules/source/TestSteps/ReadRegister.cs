using NationalInstruments.Examples.SemiconductorTestLibrary.SPI.Helpers;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.Examples.SemiconductorTestLibrary.SPI.Common.Constants;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.SPI
{
    /// <summary>
    /// Contains the test step methods for the SPI example project.
    /// </summary>
    public static partial class TestStep
    {
        /// <summary>
        /// Writes a register value to the DUT via SPI, then reads the same register back
        /// and publishes the captured data. The write sends the address and data on MOSI,
        /// and the read sends the address on MOSI while capturing the DUT response on MISO.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="spiPins">The SPI port pin names.</param>
        /// <param name="registerAddress">The 7-bit register address to write and read.</param>
        /// <param name="data">The 8-bit data value to write.</param>
        /// <param name="publishedDataId">The published data identifier for the read-back result.</param>
        /// <returns>The per-site decoded register values read back from the DUT.</returns>
        public static SiteData<byte> WriteAndReadRegister(
            ISemiconductorModuleContext semiconductorModuleContext,
            string[] spiPins,
            byte registerAddress,
            byte data,
            string publishedDataId)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(semiconductorModuleContext);
            DigitalSessionsBundle spi = sessionManager.Digital(spiPins);

            // Write: encode address + data into source waveform and burst the write pattern.
            uint[] writeCommand = SPIWaveformHelper.EncodeWriteCommand(registerAddress, data);
            spi.WriteSourceWaveformBroadcast(SPIReadWriteSourceWaveformName, writeCommand);
            spi.BurstPattern(SPIWriteRegisterPatternName, timeoutInSeconds: 10);

            // Read: encode address into source waveform and burst the read pattern.
            // SPI requires the master to send the register address on MOSI to tell the DUT which register to return on MISO.
            uint[] readCommand = SPIWaveformHelper.EncodeReadCommand(registerAddress);
            spi.WriteSourceWaveformBroadcast(SPIReadWriteSourceWaveformName, readCommand);
            spi.BurstPattern(SPIReadRegisterPatternName, timeoutInSeconds: 10);

            // Fetch captured MISO data and decode.
            SiteData<uint[]> capturedWaveform = spi.FetchCaptureWaveform(
                SPIReadCaptureWaveformName, samplesToRead: 1, timeoutInSeconds: 10);
            SiteData<byte> registerData = SPIWaveformHelper.DecodeReadResponsePerSite(capturedWaveform);

            semiconductorModuleContext.PublishResults(registerData.Select(x => (double)x), publishedDataId);
            return registerData;
        }
    }
}