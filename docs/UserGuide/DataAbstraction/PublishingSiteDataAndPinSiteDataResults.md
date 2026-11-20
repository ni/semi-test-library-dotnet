# Publishing SiteData and PinSiteData Results

The Semiconductor Test Library provides extension methods for the TSM `ISemiconductorModuleContext` object to publish `SiteData` and `PinSiteData` objects as test results. These methods publish one result for each active site, and for `PinSiteData` objects, one result for each pin. For more information about how published results are evaluated, refer to [Publishing Results](../PublishingResults.md).

Class: `Publish` \
Namespace: `NationalInstruments.SemiconductorTestLibrary.Common`

The following table outlines the methods available for publishing results.

**Table of Methods for Publishing Results:**

|Method|Description|
| :- | :- |
|[`PublishResults<T>(PinSiteData<T> results, string publishedDataId)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.Common.Publish.PublishResults.html)|Publishes the value of each pin for each active site. Each result is associated with its pin.|
|[`PublishResults<T>(SiteData<T> results, string publishedDataId, string pin = "")`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.Common.Publish.PublishResults.html)|Publishes the value of each active site. The results are associated with the specified pin, if one is provided.|
|[`PublishResult<T>(T result, string publishedDataId, string pin = "")`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.Common.Publish.PublishResult.html)|Publishes the same value for all active sites.|
|[`PublishSingleSiteResult<T>(T result, string publishedDataId, string pin = "")`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.Common.Publish.PublishSingleSiteResult.html)|Publishes a value for a single site. Use this method only with a single-site `ISemiconductorModuleContext` object.|

## Usage Considerations for Publishing Results

When publishing `SiteData` and `PinSiteData` objects you should consider the following:

1. The `publishedDataId` input must match a published data ID that is defined for the test step in TestStand.
2. Results are published only for the active sites of the `ISemiconductorModuleContext` object. Values for sites that are not active are ignored.
3. Each active site must have a value. If the object does not contain a value for an active site, the value of the site-agnostic site `-1` is published for that site instead. If neither exists, an `NISemiconductorTestException` is thrown with the message `Measurement data for site {siteNumber} is missing.`
4. The `PublishResults<T>(PinSiteData<T> results, string publishedDataId)` method publishes each pin separately. Therefore, each pin must have a value for every active site, or a value for site `-1`. This applies to jagged `PinSiteData` objects as well.
5. The `PublishResults<T>(PinSiteData<T> results, string publishedDataId)` method associates each result with its pin name. If the pin name is a pin group, the results are associated with the first pin in the pin group.
6. If a `PinSiteData` object contains no pins, nothing is published and no exception is thrown.
7. The `PublishResults<T>(SiteData<T> results, string publishedDataId, string pin = "")` method does not associate the results with a pin by default. Specify the `pin` input when the results must be associated with a specific pin.
8. Only the following data types are supported for publishing: `double`, `int`, `uint`, `long`, `bool`, and `string`. Any other data type, including arrays, causes an `NISemiconductorTestException` to be thrown with the message `Cannot publish measurement data. The {type} data type is invalid.` Use the [`Select`](DataTransformsUsingSelectMethod.md) method to convert the data to a supported type before publishing.
9. Results are published one site at a time. If an exception is thrown for a site, the results already published for the preceding sites and pins remain published.
10. Many instrument abstraction methods also provide variants that measure and publish in a single call, such as `MeasureAndPublishCurrent` and `BurstPatternAndPublishResults`.

## Example Usage for Publishing Results

Example of publishing per-pin, per-site results and per-site results:

```csharp
TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
DCPowerSessionsBundle vdd = sessionManager.DCPower("VDD");
DigitalSessionsBundle sdo = sessionManager.Digital("SDO");

// Publishes one result for each pin and each active site.
PinSiteData<double> currentMeasurements = vdd.MeasureCurrent();
tsmContext.PublishResults(currentMeasurements, publishedDataId: "VDD_Current");

// Publishes one result for each active site.
SiteData<bool> passFail = sdo.GetSitePassFail();
tsmContext.PublishResults(passFail, publishedDataId: "SDO_PassFail");
```

Example of publishing the results of a single pin, associated with that pin:

```csharp
PinSiteData<double> currentMeasurements = vdd.MeasureCurrent();
SiteData<double> vddCurrent = currentMeasurements.ExtractPin("VDD");
tsmContext.PublishResults(vddCurrent, publishedDataId: "VDD_Current", pin: "VDD");
```

Example of converting the data to a supported type before publishing:

```csharp
// Arrays cannot be published directly.
// Select the maximum value of the multi-point measurement for each pin and site, and then publish it.
PinSiteData<double[]> multiPointMeasurement = dmmPins.ReadMultiPoint(sampleCount, maximumTimeInMilliseconds: 5000);
PinSiteData<double> maxValue = multiPointMeasurement.Select(samples => samples.Max());
tsmContext.PublishResults(maxValue, publishedDataId: "MaxMultiPointValue");
```

>[!NOTE]
> The `SetValue` method used in the following example is available only in release 26.5.0 or later.

Example of publishing a site-agnostic value for all active sites:

```csharp
// SystemSupply is a system pin, so its value is stored for the site-agnostic site -1.
var pinSiteData = new PinSiteData<double>();
pinSiteData.SetValue(value: -22.5, pinName: "SystemSupply", siteNumbers: -1);

// The value of site -1 is published for each active site, associated with the SystemSupply pin.
tsmContext.PublishResults(pinSiteData, publishedDataId: "SystemSupplyVoltage");
```
