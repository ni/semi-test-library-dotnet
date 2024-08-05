# Sharing SiteData and PinSiteData Between Code Modules

As you develop your test program, it may be necessary to store results measured in one code module for later use in the same test sequence by another code module. This can be achieved by storing the data within in the SemiconductorModuleContext in one code module and retrieving later in another code module using an ID string. For more information, refer the [Sharing Data between Code Modules (TSM)](https://www.ni.com/docs/bundle/teststand-semiconductor-module/page/sharing-data-between-code-modules.html) topic in the TSM help documentation.

The `SetSiteData` and `GetSiteData` .NET methods, provided by TSM, cannot be passed SiteData or PinSiteData objects directly. You must first convert SiteData and PinSiteData into a 1D array of per-site values, where each element in the array represents a given site value. In the case of PinSiteData, this will be a 1D array of per-pin dictionaries, where each dictionary element in the array represents a given site and each item in the dictionary represents a pin's value for the given site.

> [!NOTE]
> The data must be ordered to match the order of sites in the Semiconductor Module context. This order might not be sequential. Use the `SiteNumbers` property in the `ISemiconductorModuleContext` .NET object to determine the order of the sites in the Semiconductor Module context and arrange the data manually.

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

    var perSiteDataArray = new uint[semiconductorModuleContext.SiteNumbers.Count][];
    for (int i = 0; i < perSiteDataArray.Length; i++)
    {
        perSiteDataArray[i] = measurement.GetValue(semiconductorModuleContext.SiteNumbers.ElementAt(i));
    }
    semiconductorModuleContext.SetSiteData("ComparisonData", perSiteDataArray);
}

public static void SecondCodeModule(
    ISemiconductorModuleContext semiconductorModuleContext,
    string pinName,
    string patternName,
    string waveformName,
    int samplesToRead)
{
    var perSiteComparisonDataArray = semiconductorModuleContext.GetSiteData<uint[]>("ComparisonData");
    var comparisonData = new SiteData<uint[]>(perSiteComparisonDataArray);

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

    var perSiteDataArray = new IDictionary<string, double>[semiconductorModuleContext.SiteNumbers.Count];
    for (int i = 0; i < perSiteDataArray.Length; i++)
    {
        perSiteDataArray[i] = measurement.ExtractSite(semiconductorModuleContext.SiteNumbers.ElementAt(i));
    }
    semiconductorModuleContext.SetSiteData("ComparisonData", perSiteDataArray);
}

public static void SecondCodeModule(ISemiconductorModuleContext semiconductorModuleContext, string pinName)
{
    var perSitePinDict = semiconductorModuleContext.GetSiteData<IDictionary<string, double>>("ComparisonData");
    var pinSiteDictionary = new Dictionary<string, IDictionary<int, double>>();
    for (int i = 0; i < semiconductorModuleContext.SiteNumbers.Count; i++)
    {
        var siteNumber = semiconductorModuleContext.SiteNumbers.ElementAt(i);
        foreach (var pin in perSitePinDict[i].Keys)
        {
            var singlePinSiteValue = perSitePinDict[i][pin];
            if (!pinSiteDictionary.TryGetValue(pin, out IDictionary<int, double> perSitePinValues))
            {
                perSitePinValues = new Dictionary<int, double>() { [siteNumber] = singlePinSiteValue };
                pinSiteDictionary.Add(pin, perSitePinValues);
                continue;
            }
            if (!perSitePinValues.ContainsKey(siteNumber))
            {
                perSitePinValues.Add(siteNumber, singlePinSiteValue);
                continue;
            }
            perSitePinValues[siteNumber] = singlePinSiteValue;
        }
    }
    var comparisonData = new PinSiteData<double>(pinSiteDictionary);

    var sessionManager = new TSMSessionManager(semiconductorModuleContext);
    var dcPowerPin = sessionManager.DCPower(pinName);
    PinSiteData<double> measurement = dcPowerPin.MeasureVoltage();

    var comparisonResults = measurement.Subtract(comparisonData);
    semiconductorModuleContext.PublishResults(comparisonResults, "ComparisonResults");
}
```
