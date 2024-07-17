# Concurrent Code Execution (Advanced)

Code statements that are independent of each other can be written to execute in concurrently. For example, to perform operations on pins of different instrument types. You can use the `InvokeInParallel` method from the `Utilities` class to allow separate lines of code to execute concurrently.

Applicable namespace: `NationalInstruments.SemiconductorTestLibrary.Common.Utilities`

  > **Note** this method calls the Parallel.Invoke method to execute multiple methods in parallel. However, it wraps the Parallel.Invoke method in a try-catch statement such that if an expectation occurs, only the first exception that is encountered will be returned to the call. This allows the exception to easily bubble-up and display properly by the TestStand runtime error dialog.

The following example demonstrates how to use the `InvokeInParallel` method from the `NationalInstruments.SemiconductorTestLibrary.Common.Utilities` class :

```C#
public static void ConcurrentCodeExample(ISemiconductorModuleContext semiconductorModuleContext, string pinNames)
{
    var sessionManager = new TSMSessionManager(semiconductorModuleContext);
    var publishDataID = "Measurement";
    var filteredPinNamesDmm = semiconductorModuleContext.FilterPinsByInstrumentType(pinNames, InstrumentTypeIdConstants.NIDmm);
    var filteredPinNamesPpmu = semiconductorModuleContext.FilterPinsByInstrumentType(pinNames, InstrumentTypeIdConstants.NIDigitalPattern);
    var filteredPinNamesSmu = semiconductorModuleContext.FilterPinsByInstrumentType(pinNames, InstrumentTypeIdConstants.NIDCPower);
    var ppmuPins = sessionManager.DCPower(filteredPinNamesSmu);
    var smuPins = sessionManager.Digital(filteredPinNamesPpmu);
    var dmmPins = sessionManager.DMM(filteredPinNamesPpmu);

    // Assumes that the instrumentation is already configured.
    Utilities.InvokeInParallel(
        () => ppmuPins.MeasureAndPublishCurrent(publishDataID),
        () => smuPins.MeasureAndPublishCurrent(publishDataID),
        () =>
        {
            var measurements = dmmPins.Read(maximumTimeInMilliseconds: 2000);
            semiconductorModuleContext.PublishResults(measurements, publishDataID);
        });
}
}
```
