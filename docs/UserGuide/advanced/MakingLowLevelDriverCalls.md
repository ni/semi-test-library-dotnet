# Making Low-Level Driver Calls

When developing your test program, you may require use a low-level driver function or capability not exposed as a high-level extension method by the Semiconductor Test Library. In this case, the [`ParallelExecution`](ParallelizationMethods.md) methods provided by the Semiconductor Test Library can be leveraged to directly access to the low-level driver sessions, which enables you to implement ad-hoc driver calls in-line with other high-level methods in your test code.

## How to Make Low-Level Driver API Calls

To invoke a low-level driver API call, use the `Do` methods in the `ParallelExecution` class within the `NationalInstruments.SemiconductorTestLibrary.Common` namespace.

The following example highlights using the `Do` method with an existing `DigitalSessionsBundle` object to start generating a clock signal by invoking the `ClockGenerator.GenerateClock` method from the niDigital driver API on the underlying PinSet of each niDigital driver session.

> [!NOTE]
> If you need to invoke a method on the driver session or access a driver-specific property object, you must also include the using declaration for the namespace of the driver API at the beginning of your code file. In the following example, the using directive `using NationalInstruments.ModularInstruments.NIDigital;` is assumed.

```C#
public static void GenerateClock(DigitalSessionsBundle sessionsBundle, double frequency)
{
    sessionsBundle.Do((DigitalSessionInformation sessionInfo) =>
    {
        sessionInfo.PinSet.ClockGenerator.GenerateClock(frequency, selectDigitalFunction: true);
    });
};
```

Similarly, you may want to return values from low-level driver properties.

The following example demonstrates how to get the configured voltage level for the target DCPower instruments by using the `DoAndReturnPerPinPerSiteData` method to access the `VoltageLevel` property of the low-level niDCPower driver session.

```C#
public static PinSiteData<double> GetVoltageLevel(DCPowerSessionsBundle sessionsBundle)
{
    return sessionsBundle.DoAndReturnPerSitePerPinResults(
        (DCPowerSessionInformation sessionInfo, SitePinInfo sitePinInfo) =>
        {
            return sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Source.Voltage.VoltageLevel;
        });
}
```

**Related Concepts:**

- [Extending the Semiconductor Test Library](ExtendingTheSemiconductorTestLibrary.md)
- [Parallelization Methods](ParallelizationMethods.md)
