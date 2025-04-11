# Writing Test Code

Complete the following steps to use Semiconductor Test Library in your test code:

1. Create a new TSMSessionManager object and any other local variables that are necessary for your test.
2. Use the TSMSessionManager object to query the session for the target pins.
3. Configure the instrumentation connected to the target pins and configure the relay modules.
4. Source and/or measure the signals.
5. Burst the patterns required to configure the DUT.
6. Calculate and/or publish the required test results.
7. Repeat steps 4 through 6 as necessary for your test.
8. Clean up and restore the state of the instrumentation after finishing the test.

> [!NOTE]
> These steps only capture the typical operations found in a test code module. Some may not be required and other steps could be necessary depending on the test methodology being implemented.
>
> These steps and the example below assume that the .csproj file and .cs file that you are writing test code within have been setup with the appropriate assembly/package references and using directives, respectively. If otherwise, refer to the related information linked below.

**Related Information:**

- [Getting Started](../index.md#getting-started)
- [Adding Using Directives](AddingUsingDirectives.md)

## Example C# Code Snippet of Workflow

```C#
public static void WorkFlowExample(
    ISemiconductorModuleContext semiconductorModuleContext,
    string[] smuPinNames,
    string[] digitalPinNames,
    string relayConfigBeforeTest,
    string relayConfigAfterTest,
    string patternName)
{
    var sessionManager = new TSMSessionManager(semiconductorModuleContext);
    double voltageLevel = 3.3;
    double currentLimit = 0.01;
    double settlingTime = 0.001;
    var measureSettings = new DCPowerMeasureSettings()
    {
        ApertureTime = 0.001,
        Sense = DCPowerMeasurementSense.Remote
    };

    var smuPins = sessionManager.DCPower(smuPinNames);
    var digitalPins = sessionManager.Digital(digitalPinNames);

    smuPins.ConfigureMeasureSettings(measureSettings);
    semiconductorModuleContext.ApplyRelayConfiguration(relayConfigBeforeTest, waitSeconds: settlingTime);

    smuPins.ForceVoltage(voltageLevel, currentLimit);
    PreciseWait(timeInSeconds: settlingTime);
    var currentBefore = smuPins.MeasureAndPublishCurrent(publishedDataId: "CurrentBefore");

    digitalPins.BurstPatternAndPublishResults(patternName, publishedDataId: "PatternResults");
    PreciseWait(timeInSeconds: settlingTime);
    var currentAfter = smuPins.MeasureAndPublishCurrent(publishedDataId: "CurrentAfter");

    var currentDifference = currentBefore.Subtract(currentAfter).Abs();
    semiconductorModuleContext.PublishResults(currentDifference, publishedDataId: "CurrentDifference");

    smuPins.ForceVoltage(voltageLevel: 0, currentLimit: 0.001);
    PreciseWait(timeInSeconds: settlingTime);
    semiconductorModuleContext.ApplyRelayConfiguration(relayConfigAfterTest, waitSeconds: settlingTime);
}
```
