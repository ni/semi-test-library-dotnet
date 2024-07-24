# Making Low-level Driver Calls

During test program development, you may be a low-level driver function or capability that is not directly exposed as a high-level extension method by the Semiconductor Test Library. In such cases, it may be necessary to use the low-level driver API to implement the desired test code. The [`ParallelExecution`](ParallelExecution.md) methods provided by the Semiconductor Test Library can be leveraged to directly access to the low-level driver sessions enabling you to implement ad-hoc driver calls inline with other high-level methods in your test code.

## How-to Make Low-level Driver API Calls

To invoke a low-level driver API call, use the `Do` methods provided within the `ParallelExecution` class within the `NationalInstruments.SemiconductorTestLibrary.Common` namespace.

The following example highlights how use the `Do` method with an existing `DigitalSessionsBundle` object to start clock generation by invoking the `ClockGenerator.GenerateClock` method from the niDigital driver API on the underlying PinSet of the niDigital driver session.

> Note that if you need to invoke a method on the driver session object, rather than simply access a property, you must also include a using declaration for the namespace of the driver API at the top of your code file. In the example below requires the following using directive: `using NationalInstruments.ModularInstruments.NIDigital;`

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

The following example demonstrates how to get the configured the voltage level for the target DCPower instruments by accessing the `VoltageLevel` property of the low-level niDCPower driver session by using the `DoAndReturnPerPinPerSiteData` method.

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

**Related concepts:**

- [Extending The Semiconductor Test Library](ExtendingTheSemiconductorTestLibrary.md)
