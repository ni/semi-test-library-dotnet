# Concurrent Code Execution

Code statements that are independent of each other can be written to execute in concurrently. You can use the `InvokeInParallel` method from the `Utilities` class to allow separate lines of code to execute concurrently. For example, to perform operations on pins of different instrument types at the same time.

  > **Note** this method uses the [`Parallel.Invoke`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel.invoke?view=net-8.0) method to execute multiple methods in parallel, and can be [invoked in the exact same way](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-use-parallel-invoke-to-execute-parallel-operations). However, note that it also wraps the Parallel.Invoke method in a try-catch statement such that if an expectation occurs, only the first exception that is encountered will be returned to the call. This allows the exception to  bubble-up and display properly by the TestStand runtime error dialog.

Namespace: `NationalInstruments.SemiconductorTestLibrary.Common`

Assembly: `NationalInstruments.SemiconductorTestLibrary.Abstractions.dll`

The following example demonstrates how to use the `InvokeInParallel` method from the `Utilities` class:

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
