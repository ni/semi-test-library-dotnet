namespace NationalInstruments.Examples.SemiconductorTestLibrary.SPI.Common
{
    /// <summary>
    /// Internal class containing all constant values used within the SPI example project.
    /// </summary>
    internal static class Constants
    {
        // Pattern names referenced in the digital pattern project.
        // These must match the pattern names in the SPI Register Map digiproj.
        internal const string SPIWriteRegisterPatternName = "SPI_write_template";
        internal const string SPIReadRegisterPatternName = "SPI_read_template";

        // Source waveform names referenced in the digital pattern project.
        internal const string SPIReadWriteSourceWaveformName = "source_buffer";

        // Capture waveform names referenced in the digital pattern project.
        internal const string SPIReadCaptureWaveformName = "capture_buffer";
    }
}
