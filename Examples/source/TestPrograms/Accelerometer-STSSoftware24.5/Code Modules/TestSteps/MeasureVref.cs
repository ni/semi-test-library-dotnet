using System.Linq;
using NationalInstruments;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Measures the analog output signal from the DUT's Vref pin using the Scope instrument.
        /// The signal is measured as a waveform of multiple samples.
        /// The min, max, and average values of the resulting waveform are published,
        /// using Min, Max, and DCValue as the respective published data ids.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="scopePinName">The scope pin name, as defined in the loaded pin map file.</param>
        public static void MeasureVref(ISemiconductorModuleContext semiconductorModuleContext, string scopePinName = "Vref_OScope")
        {
            var sessionManger = new TSMSessionManager(semiconductorModuleContext);
            ScopeSessionsBundle vref = sessionManger.Scope(scopePinName);

            PinSiteData<double[]> vrefMeasurements = vref.DoAndReturnPerSitePerPinResults((sessionInfo, pinSiteInfo) =>
            {
                AnalogWaveformCollection<double> waveforms;
                string channel = pinSiteInfo.IndividualChannelString;
                waveforms = sessionInfo.Session.Channels[channel].Measurement.Read(PrecisionTimeSpan.FromSeconds(5), -1, null);
                // Waveforms are returned as a collection of multiple records and channels.
                // Indexing the first waveform, as only one record and one channel being read at a time.
                double[] singleChannelSamples = waveforms[0].Samples.Select(sample => sample.Value).ToArray();
                return singleChannelSamples;
            });

            PinSiteData<double> average = vrefMeasurements.Select(x => x.Average());
            PinSiteData<double> min = vrefMeasurements.Select(x => x.Min());
            PinSiteData<double> max = vrefMeasurements.Select(x => x.Max());

            semiconductorModuleContext.PublishResults(average, "DCValue");
            semiconductorModuleContext.PublishResults(min, "Min");
            semiconductorModuleContext.PublishResults(average, "Max");
        }
    }
}