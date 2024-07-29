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
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have been already initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class MeasureDMM
    {
        internal static void SinglePointMeasureDCVoltageDMM(ISemiconductorModuleContext tsmContext, string[] dmmPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Retrieves the Dmm sessions associated to the pin name(s).
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // Configures the settings used to take the measurements.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 7.5);

            // Performs a reading and returns the measurements.
            // Data is returned in a per-site per-pin format.
            PinSiteData<double> measurements = dmmPins.Read(maximumTimeInMilliseconds: 10);
        }

        internal static void SinglePointSinglePinMeasureDCVoltageWriteResultsToDebugDMM(ISemiconductorModuleContext tsmContext, string dmmPinName)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Retrieves the Dmm sessions associated to the pin name(s).
            var dmmPin = sessionManager.DMM(dmmPinName);

            // Configures the settings used to take the measurements.
            dmmPin.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 5.5);

            // Performs a reading and returns the measurements.
            // Data is returned in a per-site per-pin format.
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

            // Retrieves the Dmm sessions associated to the pin name(s).
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // Configures different settings taht will be used to take the measurements.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 5.5);
            dmmPins.ConfigureAutoZero(DmmAuto.Off);
            dmmPins.ConfigureADCCalibration(DmmAdcCalibration.Off);

            // Performs a reading and publish the measurements in a single step.
            dmmPins.ReadAndPublish(maximumTimeInMilliseconds: 10, publishedDataId: "dmmMeasurement");
        }

        internal static void SinglePointMeasureACVoltageDMM(ISemiconductorModuleContext tsmContext, string[] dmmPinNames, double voltageRange, double minFreq, double maxFreq)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Retrieves the Dmm sessions associated to the pin name(s).
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // Configures different settings taht will be used to take the measurements.
            // Setting up the measurement function to capture an AC voltage.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.ACVolts, range: voltageRange, 5.5);

            // Configures the frequency bandwidth for the AC measurements
            dmmPins.ConfigureACBandwidth(minFreq, maxFreq);

            // The measurements are returned in a per-site per-pin format.
            PinSiteData<double> measurements = dmmPins.Read(maximumTimeInMilliseconds: 10);
        }

        internal static void MultiPointMeasureDCVoltageDMM(ISemiconductorModuleContext tsmContext, string[] dmmPinNames, int samples)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Retrieves the Dmm sessions associated to the pin name(s).
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // Configures different settings taht will be used to take the measurements.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 7.5);

            // Configures the trigger count, sample counts per trigger, trigger source and trigger sample interval.
            // Note that this step is only required if more than 1 sample is desired.
            // If no external triggering is required set the sample trigger value to "Immediate".
            dmmPins.ConfigureMultiPoint(1, samples, "Immediate", 0.001);

            // Reads multiple sample points for each pin of each site, data is returned in a per-instrument, per-value format.
            var measurements = dmmPins.ReadMultiPoint(samples, 5000);
        }

        internal static void TriggeredMultiPointMeasurementDMM(ISemiconductorModuleContext tsmContext, string[] dmmPinNames, int samples)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Retrieves the Dmm sessions associated to the pin name(s).
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // Configures different settings taht will be used to take the measurements.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 7.5);

            // Configures the trigger count, sample counts per trigger, trigger source and trigger sample interval.
            // In this case a software trigger was chosen, measurements will start upon detecting this trigger.
            dmmPins.ConfigureMultiPoint(1, samples, "Software Trigger", 0.001);

            // Puts the Dmm sessions in a initiated state to wait for the trigger.
            dmmPins.Initiate();

            // Once initiated the Dmm instruments will wait for the trigger to start the acquisition, when the trigger occurs
            // the Dmm will capture the data and store it in its buffer. This data can be then retrieved with the Fetch method.

            // Send a software trigger to start the measurements.
            dmmPins.SendSoftwareTrigger();

            // Fetches multiple sample measurement points for an already started acquisition, data is returned in a per-instrument,
            // per-value format. Note that unlike the Read method (which starts the acquisition and returns the measurements when
            // called), the Fetch method expects to retrieve data from an already started acquisition.
            var measurements = dmmPins.FetchMultiPoint(samples, maximumTimeInMilliseconds: 5000);
        }
    }
}
