using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Defines methods for frequency measurements.
    /// </summary>
    public static class FrequencyCounter
    {
        /// <summary>
        /// Measures frequency and returns per-site per-pin results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="measurementMode">The frequency measurement mode.</param>
        /// <param name="measurementTime">The measurement time in seconds.</param>
        /// <returns>The measurements in per-site per-pin format.</returns>
        public static PinSiteData<double> MeasureFrequency(this DigitalSessionsBundle sessionsBundle, FrequencyMeasurementMode? measurementMode = null, double? measurementTime = null)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                sessionInfo.PinSet.SelectedFunction = SelectedFunction.Digital;
                if (measurementMode.HasValue)
                {
                    sessionInfo.PinSet.FrequencyCounter.MeasurementMode = measurementMode.Value;
                }
                if (measurementTime.HasValue)
                {
                    sessionInfo.PinSet.FrequencyCounter.MeasurementTime = Ivi.Driver.PrecisionTimeSpan.FromSeconds(measurementTime.Value);
                }
                return sessionInfo.PinSet.FrequencyCounter.MeasureFrequency();
            });
        }
    }
}
