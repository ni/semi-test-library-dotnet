using System.Diagnostics;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to perform measurements using the Digital Multimeter (DMM) instrument.
    /// This class, and it's methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have already initiated and configured prior.
    /// Additionally, they are intentionally marked as internal to prevent them from be directly invoked from code outside of this project.
    /// </summary>
    internal static class MeasureDMM
    {
        internal static void SinglePointMeasureDCVoltageDMM(ISemiconductorModuleContext tsmContext, string[] dmmPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dmmPins = sessionManager.DMM(dmmPinNames);

            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 7.5);
            PinSiteData<double> measurements = dmmPins.Read(maximumTimeInMilliseconds: 10);
        }

        internal static void SinglePointSinglePinMeasureDCVoltageWriteResultsToDebugDMM(ISemiconductorModuleContext tsmContext, string dmmPinName)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dmmPin = sessionManager.DMM(dmmPinName);

            dmmPin.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 5.5);
            PinSiteData<double> measurements = dmmPin.Read(10);

            // Print to debug console.
            Debug.WriteLine($"DMM Measurement Results for {dmmPinName}:");
            foreach (int site in tsmContext.SiteNumbers)
            {
                Debug.WriteLine($"Site{site}: {measurements.GetValue(site, dmmPinName)}");
            }
        }

        internal static void FastSinglePointMeasureDCVoltageDMM(ISemiconductorModuleContext tsmContext, string[] dmmPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dmmPins = sessionManager.DMM(dmmPinNames);

            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 5.5);
            dmmPins.ConfigureAutoZero(DmmAuto.Off);
            dmmPins.ConfigureADCCalibration(DmmAdcCalibration.Off);
            PinSiteData<double> measurements = dmmPins.Read(maximumTimeInMilliseconds: 10);
            tsmContext.PublishResults(measurements, publishedDataId: "dmmMeasurment");
        }
    }
}
