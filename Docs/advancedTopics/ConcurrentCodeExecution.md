# Concurrent Code Execution (Advanced)

Namespace: NationalInstruments.SemiconductorTestLibrary.Common.Utilities
Use to implement Concurrent programming when separate lines of code should execute in parallel.

- For example, it is typical to use this method when performing operations on pins of different instrument types.

  > **Note** that this method calls the Parallel.Invoke method to execute multiple methods in parallel. However, it wraps the Parallel.Invoke method in a try-catch statement such that if an expectation occurs, only the first exception that is encountered will be returned to the call. This allows the exception to be more easily bubbled up and displayed properly by the TestStand  Semiconductor Module runtime error dialog.

The following example demonstrates how to use the InvokeInParallel method from the NationalInstruments.SemiconductorTestLibrary.Common.Utilities class:

```C#
// It is more efficient to invoke the following steps in parallel, as they are independent of each other.
Utilities.InvokeInParallel(
    () =>
    {
        // Publish results collected.
        PinSiteData<double> currentDifference = 
        currentBefore.Subtract(currentAfter).Abs();
        semiconductorModuleContext.PublishResults(currentDifference, publishedDataId: "CurrentDifference");
    },
    () =>
    {
        // Clean up relay configurations and place the instrument in a safe state, as it makes sense for any proceeding test.
        smuPins.ForceVoltage(voltageLevel: 0, currentLimit: 0.001);
        smuPins.ConfigureOutputEnabled(false);
        PreciseWait(settlingTime);
        semiconductorModuleContext.ApplyRelayConfiguration(relayConfigurationCleanup, settlingTime);
    });
```
