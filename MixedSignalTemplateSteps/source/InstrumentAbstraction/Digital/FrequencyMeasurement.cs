using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.DataAbstraction;
using NationalInstruments.ModularInstruments.NIDigital;

namespace NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods for frequency measurements.
    /// </summary>
    public static class FrequencyMeasurement
    {
        /// <summary>
        /// Measures frequency and returns per-site per-pin results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <returns>The measurements in per-site per-pin format.</returns>
        public static PinSiteData<double> MeasureFrequency(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                sessionInfo.PinSet.SelectedFunction = SelectedFunction.Digital;
                return sessionInfo.PinSet.FrequencyCounter.MeasureFrequency();
            });
        }
    }
}
