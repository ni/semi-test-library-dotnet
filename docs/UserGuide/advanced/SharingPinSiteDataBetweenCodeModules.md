# Sharing SiteData and PinSiteData Between Code Modules

As you develop your test program, it may be necessary to store results measured in one code module for later use in the same test sequence by another code module. 
The Semiconductor Test Library provides the following extension methods for you to achieve this.
- `SetGlobalSiteData`
- `GetGlobalSiteData`
- `SetGlobalPinSiteData`
- `GetGlobalPinSiteData`

## Sharing SiteData Example

The following example shows how to store per-site measurement data for comparison in a later test step::

```C#
public static void FirstCodeModule(
    ISemiconductorModuleContext semiconductorModuleContext,
    string pinName,
    string patternName,
    string waveformName,
    int samplesToRead)
{
    var sessionManager = new TSMSessionManager(semiconductorModuleContext);
    var digitalPins = sessionManager.Digital(pinName);
    digitalPins.BurstPattern(patternName);
    SiteData<uint[]> measurement = digitalPins.FetchCaptureWaveform(waveformName, samplesToRead);

    semiconductorModuleContext.SetGlobalSiteData("ComparisonData", measurement);
}

public static void SecondCodeModule(
    ISemiconductorModuleContext semiconductorModuleContext,
    string pinName,
    string patternName,
    string waveformName,
    int samplesToRead)
{
    var comparisonData = semiconductorModuleContext.GetGlobalSiteData<uint[]>("ComparisonData");

    var sessionManager = new TSMSessionManager(semiconductorModuleContext);
    var digitalPins = sessionManager.Digital(pinName);
    digitalPins.BurstPattern(patternName);
    SiteData<uint[]> measurement = digitalPins.FetchCaptureWaveform(waveformName, 1);

    var comparisonResults = measurement.Compare(ComparisonType.EqualTo, comparisonData);
    semiconductorModuleContext.PublishResults(comparisonResults, "ComparisonResults");
}
```

## Sharing PinSiteData Example

The following example shows how to store per-pin per-site measurement data for comparison in a later test step:

``` C#
public static void FirstCodeModule(ISemiconductorModuleContext semiconductorModuleContext, string pinName)
{
    var sessionManager = new TSMSessionManager(semiconductorModuleContext);
    var dcPowerPin = sessionManager.DCPower(pinName);
    PinSiteData<double> measurement = dcPowerPin.MeasureVoltage();

    semiconductorModuleContext.SetGlobalPinSiteData("ComparisonData", measurement);
}

public static void SecondCodeModule(ISemiconductorModuleContext semiconductorModuleContext, string pinName)
{
    var comparisonData = semiconductorModuleContext.GetGlobalPinSiteData<double>("ComparisonData");

    var sessionManager = new TSMSessionManager(semiconductorModuleContext);
    var dcPowerPin = sessionManager.DCPower(pinName);
    PinSiteData<double> measurement = dcPowerPin.MeasureVoltage();

    var comparisonResults = measurement.Subtract(comparisonData);
    semiconductorModuleContext.PublishResults(comparisonResults, "ComparisonResults");
}
```

**Related Concepts:**

- [NI TSM: Sharing Data between Code Modules](https://www.ni.com/docs/bundle/teststand-semiconductor-module/page/sharing-data-between-code-modules.html)