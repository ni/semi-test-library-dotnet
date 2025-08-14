using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer.Common.Constants;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Trims the DUT's Vref value.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="spiPortPins">The SPI port pin names, as defined in the SetVrefValue pattern.</param>
        public static void TrimVrefValue(ISemiconductorModuleContext semiconductorModuleContext, string[] spiPortPins)
        {
            double[] vrefValues = semiconductorModuleContext.GetInputDataAsDoubles(inputDataId: "Vref Value");
            double maxVrefValue = semiconductorModuleContext.GetSpecificationsValue("DC.Max_Vref_Value");

            uint[] perSiteRegisterValues = new uint[vrefValues.Length];
            // Compute register value based on vrefValues (one per site).
            for (int i = 0; i < vrefValues.Length; i++)
            {
                double vrefValue = vrefValues[i];
                double normalizedValue = (vrefValue / maxVrefValue) * byte.MaxValue;
                uint registerValue = ConvertDoubleToByte(normalizedValue);

                perSiteRegisterValues[i] = registerValue;
            }
            SiteData<uint> registerValues = semiconductorModuleContext.NewSiteData(perSiteRegisterValues);

            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            DigitalSessionsBundle spi = sessionManager.Digital(spiPortPins);

            // Source waveforms must be an array of 32-bit samples.
            // In this case, we only expect to provide one same, which represents a single byte.
            SiteData<uint[]> sourceWaveforms = registerValues.Select(x => new[] { x });

            spi.WriteSourceWaveformSiteUnique(SetVrefValueWaveformName, sourceWaveforms);
            spi.BurstPattern(SetVrefValuePatternName, timeoutInSeconds: 10);

            SiteData<uint[]> captureWaveforms = spi.FetchCaptureWaveform(GetVrefValueWaveformName, samplesToRead: 1, timeoutInSeconds: 10);

            // Compares each site's waveform values.
            // We only expect to compare one waveform sample against the value calculated above
            // to confirm the register value written out is the same value read back in.
            // This could be written as a single line, but is being broken-up for readability and easier debug.
            SiteData<uint> capturedRegisterValue = captureWaveforms.Select(x => x[0]);
            SiteData<bool> comparsionResult = capturedRegisterValue.Compare(ComparisonType.EqualTo, registerValues);

            semiconductorModuleContext.PublishResults(comparsionResult, "Trim Verify");
        }

        private static byte ConvertDoubleToByte(double doubleValue)
        {
            byte byteValue;
            if (doubleValue > byte.MaxValue)
            {
                byteValue = byte.MaxValue;
            }
            else if (doubleValue < byte.MinValue)
            {
                byteValue = byte.MinValue;
            }
            else
            {
                byteValue = (byte)doubleValue;
            }
            return byteValue;
        }
    }
}