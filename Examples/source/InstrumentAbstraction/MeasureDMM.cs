using System.Diagnostics;
using System.Linq;
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

            // Retrieves the DMM sessions associated to the pin name(s).
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // Configures the settings used to take a measurement.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 7.5);

            // Performs a reading and returns the measurements as a PinSiteData object.
            PinSiteData<double> measurements = dmmPins.Read(maximumTimeInMilliseconds: 10);
        }

        internal static void SinglePointSinglePinMeasureDCVoltageWriteResultsToDebugDMM(ISemiconductorModuleContext tsmContext, string dmmPinName)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Retrieves the DMM sessions associated to the pin name(s).
            var dmmPin = sessionManager.DMM(dmmPinName);

            // Configures the settings used to take a measurement.
            dmmPin.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 5.5);

            // Performs a reading and returns the measurements as a PinSiteData object.
            PinSiteData<double> measurements = dmmPin.Read(maximumTimeInMilliseconds: 10);

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

            // Retrieves the DMM sessions associated to the pin name(s).
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // Configures different settings used to take the measurement.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 5.5);
            dmmPins.ConfigureAutoZero(DmmAuto.Off);
            dmmPins.ConfigureADCCalibration(DmmAdcCalibration.Off);

            // Performs a measurement reading and immediately publish the results in a single step.
            dmmPins.ReadAndPublish(maximumTimeInMilliseconds: 10, publishedDataId: "dmmMeasurement");
        }

        internal static void SinglePointMeasureACVoltageDMM(ISemiconductorModuleContext tsmContext, string[] dmmPinNames, double voltageRange, double minFreq, double maxFreq)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Retrieves the DMM sessions associated to the pin name(s).
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // Configures different settings used to take the measurement.
            // Setting up the measurement function to take an AC voltage reading.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.ACVolts, range: voltageRange, resolutionDigits: 5.5);
            // Configures the frequency bandwidth for the AC measurements
            dmmPins.ConfigureACBandwidth(minFreq, maxFreq);

            // Performs a reading and returns the measurements as a PinSiteData object.
            PinSiteData<double> measurements = dmmPins.Read(maximumTimeInMilliseconds: 10);
        }

        internal static void TriggeredSinglePointMeasurementDMM(ISemiconductorModuleContext tsmContext, string[] dmmPinNames, int sampleCount)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Retrieves the DMM sessions associated to the pin name(s).
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // Configures different settings used to take the measurement.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 7.5);
            // Configures the trigger count, sample counts per trigger, trigger source and trigger sample interval.
            // In this case a software trigger was chosen, measurements will start upon detecting this trigger.
            dmmPins.ConfigureTrigger(DmmTriggerSource.SoftwareTrigger, triggerDelayInSeconds: 0);

            // Initiated the DMM session, putting it in a committed state await for the trigger before starting the acquisition.
            dmmPins.Initiate();

            // Send a software trigger to start the measurements.
            // When the trigger occurs, the DMM will capture the data and store it in its buffer.
            // This data can be then retrieved with the Fetch method.
            dmmPins.SendSoftwareTrigger();

            // Fetches the single point measurement sample from the already started acquisition.
            // Data is returned as a PinSiteData object.
            // Note that unlike the Read method (which starts the acquisition and returns the measurements when called),
            // the Fetch method expects to retrieve data from an already started acquisition.
            var measurements = dmmPins.Fetch(maximumTimeInMilliseconds: 5000);
        }

        internal static void MultiPointMeasureDCVoltageDMM(ISemiconductorModuleContext tsmContext, string[] dmmPinNames, int sampleCount)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Retrieves the DMM sessions associated to the pin name(s).
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // Configures different settings used to take the measurement.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 7.5);
            // Configures the trigger count, sample counts per trigger, trigger source and trigger sample interval.
            // Note that this step is only required if more than 1 sample is desired.
            // If no external triggering is required set the sample trigger value to "Immediate".
            dmmPins.ConfigureMultiPoint(triggerCount: 1, sampleCount, "Immediate", sampleIntervalInSeconds: 0.001);

            // Fetches multiple measurement points/samples for an already started acquisition.
            // Data is returned as a PinSiteData<T> object, where T is an 1D array of doubles with a length equal to the specified sampleCount,
            // and each element in the array represents a single sample.
            PinSiteData<double[]> multiPointMeasurement = dmmPins.ReadMultiPoint(sampleCount, maximumTimeInMilliseconds: 5000);

            // Gets the maximum value of all the measured samples, on a per-pin and per-site basis, then publishes the results.
            PinSiteData<double> maxValue = multiPointMeasurement.Select(x => x.Max());
            tsmContext.PublishResults(maxValue, "MaxMultiPointValue");
        }

        internal static void TriggeredMultiPointMeasurementDMM(ISemiconductorModuleContext tsmContext, string[] dmmPinNames, int sampleCount)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Retrieves the DMM sessions associated to the pin name(s).
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // Configures different settings used to take the measurement.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 0.02, resolutionDigits: 7.5);
            // Configures the trigger count, sample counts per trigger, trigger source and trigger sample interval.
            // In this case a software trigger was chosen, measurements will start upon detecting this trigger.
            dmmPins.ConfigureMultiPoint(triggerCount: 1, sampleCount, "Software Trigger", sampleIntervalInSeconds: 0.001);

            // Initiated the DMM session, putting it in a committed state await for the trigger before starting the acquisition.
            dmmPins.Initiate();

            // Send a software trigger to start the measurements.
            // When the trigger occurs, the DMM will capture the data and store it in its buffer.
            // This data can be then retrieved with the Fetch method.
            dmmPins.SendSoftwareTrigger();

            // Fetches multiple measurement points/samples for an already started acquisition.
            // Data is returned as a PinSiteData<T> object, where T is an 1D array of doubles with a length equal to the specified sampleCount,
            // and each element in the array represents a single sample.
            // Note that unlike the Read method (which starts the acquisition and returns the measurements when called),
            // the Fetch method expects to retrieve data from an already started acquisition.
            PinSiteData<double[]> multiPointMeasurement = dmmPins.FetchMultiPoint(sampleCount, maximumTimeInMilliseconds: 5000);

            // Gets the maximum value of all the measured samples, on a per-pin and per-site basis, then publishes the results.
            PinSiteData<double> maxValue = multiPointMeasurement.Select(x => x.Max());
            tsmContext.PublishResults(maxValue, "MaxMultiPointValue");
        }
    }
}
