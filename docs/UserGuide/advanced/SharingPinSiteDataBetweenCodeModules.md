# Sharing SiteData and PinSiteData Between Code Modules

As you develop your test program, it may be necessary to store data in one code module for later use in the same test program by another code module. To achieve this, you can store the data within the SemiconductorModuleContext in one code module and retrieve it later in another code module using an ID string. For more information, refer to the [Sharing Data between Code Modules (TSM)](https://www.ni.com/docs/bundle/teststand-semiconductor-module/page/sharing-data-between-code-modules.html)  topic in the TSM documentation.

However, the `SetSiteData` and `GetSiteData` methods provided by the TSM Code Module API do not support `SiteData` or `PinSiteData` objects directly. Instead, use the following methods from the Semiconductor Test library to share `SiteData` and `PinSiteData` objects between code modules:
- `SetGlobalSiteData`
- `GetGlobalSiteData`
- `SetGlobalPinSiteData`
- `GetGlobalPinSiteData`

> [!NOTE]
>When data for a specific site already exists in the `SiteData` object that is associated with a given ID string, or data for a specific pin-site pair already exists in the `PinSiteData` object that is associated with a given ID string, the `SetGlobalSiteData` and `SetGlobalPinSiteData` methods override the existing data by default. If you don't want existing data to be overwritten, make sure to pass `false` to the `overrideIfExisting` input parameter. An `NISemiconductorTestException` will be thrown when data already exists and `overrideIfExisting` is set to `false`.

> [!NOTE]
> The `GetGlobalSiteData` and `GetGlobalPinSiteData` methods filter for active sites in SemiconductorModuleContext by default. If you want to retrieve the exact data you store with the `SetGlobalSiteData` and `SetGlobalPinSiteData` methods, make sure to pass `false` to the `filterForActiveSites` input parameter.

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
    TSMSessionManager sessionManager = new TSMSessionManager(semiconductorModuleContext);
    DigitalSessionsBundle digitalPins = sessionManager.Digital(pinName);
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
    SiteData<uint[]> comparisonData = semiconductorModuleContext.GetGlobalSiteData<uint[]>("ComparisonData");

    TSMSessionManager sessionManager = new TSMSessionManager(semiconductorModuleContext);
    DigitalSessionsBundle digitalPins = sessionManager.Digital(pinName);
    digitalPins.BurstPattern(patternName);
    SiteData<uint[]> measurement = digitalPins.FetchCaptureWaveform(waveformName, samplesToRead);

    SiteData<bool[]> comparisonResults = measurement.Compare<uint[], bool[]>(ComparisonType.EqualTo, comparisonData);
    // Publish the Aggregate Comparison Result: whether or not all samples in the comparison result are found to be True.
    SiteData<bool> aggregateComparisonResult = comparisonResults.Select(result => result.All(value => value));
    semiconductorModuleContext.PublishResults(aggregateComparisonResult, "ComparisonResults");
}
```

## Sharing PinSiteData Example

The following example shows how to store per-pin per-site measurement data for comparison in a later test step:

``` C#
public static void FirstCodeModule(ISemiconductorModuleContext semiconductorModuleContext, string pinName)
{
    TSMSessionManager sessionManager = new TSMSessionManager(semiconductorModuleContext);
    DCPowerSessionsBundle dcPowerPin = sessionManager.DCPower(pinName);
    PinSiteData<double> measurement = dcPowerPin.MeasureVoltage();

    semiconductorModuleContext.SetGlobalPinSiteData("ComparisonData", measurement);
}

public static void SecondCodeModule(ISemiconductorModuleContext semiconductorModuleContext, string pinName)
{
    PinSiteData<double> comparisonData = semiconductorModuleContext.GetGlobalPinSiteData<double>("ComparisonData");

    TSMSessionManager sessionManager = new TSMSessionManager(semiconductorModuleContext);
    DCPowerSessionsBundle dcPowerPin = sessionManager.DCPower(pinName);
    PinSiteData<double> measurement = dcPowerPin.MeasureVoltage();

    PinSiteData<double> comparisonResults = measurement.Subtract(comparisonData);
    semiconductorModuleContext.PublishResults(comparisonResults, "ComparisonResults");
}
```