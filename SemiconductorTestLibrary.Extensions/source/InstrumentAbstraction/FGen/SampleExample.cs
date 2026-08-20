using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Sample example class to illustrate how the extensions are used in a typical waveform generation.
    /// </summary>
    public static class SampleExample
    {
        /// <summary>
        /// Example method to show generation of sinewaveform
        /// </summary>
        /// <param name="tsmContext">tsmcontext</param>
        public static void GenerateSineWaveform(ISemiconductorModuleContext tsmContext)
        {
            // Set the output mode to Function for generating standard waveforms.
            OutputMode outputMode = OutputMode.Function;

            // Define parameters for the waveform generation.
            StandardWaveform function1 = StandardWaveform.Sine;
            double frequency1 = 1000; // 1 KHz
            double amplitude1 = 2.0; // 2 Volts
            double dcOffset1 = 0;
            double startPhase1 = 0;
            var standardWaveformSettings1 = new StandardWaveformSettings(function1, frequency1, amplitude1, dcOffset1, startPhase1);

            // Define parameters for the waveform generation.
            StandardWaveform function2 = StandardWaveform.Square;
            double frequency2 = 2000; // 1 KHz
            double amplitude2 = 4.0; // 2 Volts
            double dcOffset2 = 1;
            double startPhase2 = 0;
            var standardWaveformSettings2 = new StandardWaveformSettings(function2, frequency2, amplitude2, dcOffset2, startPhase2);

            int[] siteData = new int[] { 0, 1 };
            string[] pinNames = new string[] { "A" };
            StandardWaveformSettings[] pinsiteData = new StandardWaveformSettings[] { standardWaveformSettings1, standardWaveformSettings2 };

            var standardWaveformSettings = new PinSiteData<StandardWaveformSettings>(siteData, pinNames, pinsiteData);

            // Create a TSMContext for the test session. The pinmap file path is specified in the TSMContext.
            // var tsmContext = TSMContext.CreateTSMContext("FgenTests.pinmap");
            // string path = tsmContext.PinMapFilePath;

            // Create a session manager and get the FGen sessions bundle for the specified pin group "A".
            var sessionManager = new TSMSessionManager(tsmContext);
            var fgenSessionsBundle = sessionManager.Fgen("A");

            // Abort any existing sessions to ensure a clean start.
            fgenSessionsBundle.Abort();

            // Configure the output mode and standard waveform settings for the FGen sessions.
            fgenSessionsBundle.ConfigureOutputMode(outputMode);
            fgenSessionsBundle.ConfigureStandardWaveform(standardWaveformSettings);
            fgenSessionsBundle.ConfigureOutputEnable(true);

            // Initiate the FGen sessions to start generating the waveform.
            fgenSessionsBundle.Initiate();
            System.Threading.Thread.Sleep(60000); // Wait for 60 seconds to observe the waveform

            // Stop the FGen sessions after the observation period.
            fgenSessionsBundle.Abort();
        }
    }
}
