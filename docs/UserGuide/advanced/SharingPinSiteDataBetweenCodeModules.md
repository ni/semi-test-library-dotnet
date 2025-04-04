# Sharing SiteData and PinSiteData Between Code Modules

You can store site and pin data within the `SemiconductorModuleContext` in one code module, and retrieve it later in another code module within the same test program using an ID string. For more information, refer to [Sharing Data between Code Modules (TSM)](https://www.ni.com/docs/bundle/teststand-semiconductor-module/page/sharing-data-between-code-modules.html) topic in the TSM documentation.

> [!NOTE]
> The `SetSiteData` and `GetSiteData` methods provided by the TSM Code Module API do not support `SiteData` or `PinSiteData` objects directly.

Use the following methods from the Semiconductor Test Library to share `SiteData` and `PinSiteData` objects between code modules:
- `SetGlobalSiteData`
- `GetGlobalSiteData`
- `SetGlobalPinSiteData`
- `GetGlobalPinSiteData`

## `SetGlobalSiteData` and `SetGlobalPinSiteData` behavior

The `SetGlobalSiteData` and `SetGlobalPinSiteData` methods override the existing data by default. This occurs when data for a specific site already exists in the `SiteData` object that is associated with a given ID string, or data that is associated with a given ID string for a specific pin-site pair already exists in the `PinSiteData` object. To preserve existing data, pass `false` to the `overrideIfExisting` input parameter.

> [!NOTE]
> An `NISemiconductorTestException` is thrown when data already exists and `overrideIfExisting` is set to `false`.

## `GetGlobalSiteData` and `GetGlobalPinSiteData` behavior

The `GetGlobalSiteData` and `GetGlobalPinSiteData` methods filter for active sites in `SemiconductorModuleContext` by default. To retrieve the exact data you store with the `SetGlobalSiteData` and `SetGlobalPinSiteData` methods, pass `false` to the `filterForActiveSites` input parameter.

> [!NOTE]
> An `NISemiconductorTestException` is thrown when data has not been set for one or more active sites, and `filterForActiveSites` input parameter is set to `true` (default). To retrieve the exact data stored with the `SetGlobalSiteData` and `SetGlobalPinSiteData` methods regardless of the active sites, pass `false` to the `filterForActiveSites` input parameter.

## Sharing SiteData

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
    DigitalSessionsBundle digitalPin = sessionManager.Digital(pinName);
    digitalPin.BurstPattern(patternName);
    SiteData<uint[]> measurement = digitalPin.FetchCaptureWaveform(waveformName, samplesToRead);

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
    DigitalSessionsBundle digitalPin = sessionManager.Digital(pinName);
    digitalPin.BurstPattern(patternName);
    SiteData<uint[]> measurement = digitalPin.FetchCaptureWaveform(waveformName, samplesToRead);

    SiteData<bool[]> comparisonResults = measurement.Compare<uint[], bool[]>(ComparisonType.EqualTo, comparisonData);
    // Publish the Aggregate Comparison Result: whether or not all samples in the comparison result are found to be True.
    SiteData<bool> aggregateComparisonResult = comparisonResults.Select(result => result.All(value => value));
    semiconductorModuleContext.PublishResults(aggregateComparisonResult, "ComparisonResults");
}
```

## Sharing PinSiteData

The following example shows how to store per-site per-pin measurement data for comparison in a later test step:

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