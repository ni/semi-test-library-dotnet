# Extracting a Site Subset from SiteData/PinSiteData

You can get the data for a single site or a subset of sites from a `SiteData` or `PinSiteData` object. The methods that return a subset of sites create a new object, and the original object is not modified.

The following table outlines the methods and properties available for extracting site data.

**Table of Methods and Properties for Extracting Site Data:**

|Type|Member|Description|
| :- | :- | :- |
|`SiteData<T>`|[`SiteNumbers`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.SiteNumbers.html)|Returns the site numbers in the `SiteData` object, in ascending order.|
|`SiteData<T>`|[`GetValue(int siteNumber)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.GetValue.html)|Returns the value of the specified site.|
|`SiteData<T>`|[`TryGetValue(int siteNumber, out T value)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.TryGetValue.html)|Gets the value of the specified site, if it exists. Returns `true` if the site exists, and `false` otherwise.|
|`SiteData<T>`|[`ExtractSites(int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.ExtractSites.html)|Returns a new `SiteData` object that contains only the specified sites.|
|`PinSiteData<T>`|[`SiteNumbers`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.SiteNumbers.html)|Returns the union of the site numbers across all pins, in ascending order.|
|`PinSiteData<T>`|[`ExtractSite(int siteNumber)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.ExtractSite.html)|Returns a dictionary that contains the value of each pin for the specified site, where each key is a pin name.|
|`PinSiteData<T>`|[`ExtractSites(int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.ExtractSites.html)|Returns a new `PinSiteData` object that contains all pins, but only the specified sites.|

## Usage Considerations for Extracting a Site Subset

When extracting site data from `SiteData` and `PinSiteData` objects you should consider the following:

1. The `ExtractSites` methods return a new object. Modifying the returned object does not modify the original object, and vice versa. However, when `T` is a reference type, such as an array, both objects refer to the same value instances.
2. Every specified site must exist. Otherwise, an `NISemiconductorTestException` is thrown with the message `The site number {siteNumber} does not exist.` Use the `TryGetValue` method or check the `SiteNumbers` property first if a site might not exist.
3. The `GetValue` and `ExtractSites` methods of `SiteData` throw an `NISemiconductorTestException` if the `SiteData` object is empty.
4. The `ExtractSite` and `ExtractSites` methods of `PinSiteData` throw an `NISemiconductorTestException` if the `PinSiteData` object contains no pins or no sites.
5. The `ExtractSite` and `ExtractSites` methods of `PinSiteData` require every pin to contain each of the specified sites. For a jagged `PinSiteData` object, or an object that contains system pins with only the site-agnostic site `-1`, first use the `ExtractPins` method to select the pins that contain the sites. For more information, refer to [Extracting a Pin Subset from PinSiteData](ExtractingPinSubset.md).
6. The `siteNumbers` input must contain unique values. If the `siteNumbers` input is an empty array, the returned object contains no sites.
7. The `SiteNumbers` property returns a new array each time it is accessed. Store the array in a local variable instead of accessing the property repeatedly.

## Example Usage for Extracting a Site Subset

Example of extracting site data from a `SiteData` object:

```csharp
var siteData = new SiteData<double>(new int[] { 0, 1, 2, 3 }, new double[] { 1.1, 2.2, 3.3, 4.4 });
// Site  |  0    |  1    |  2    |  3
// Value |  1.1  |  2.2  |  3.3  |  4.4

// Gets the value of site 2.
double site2Value = siteData.GetValue(2);
// 3.3

// Gets the value of site 5, if it exists.
bool site5Exists = siteData.TryGetValue(5, out double site5Value);
// site5Exists = false, site5Value = 0.0

// Creates a new SiteData object that contains only sites 1 and 3.
SiteData<double> subset = siteData.ExtractSites(new int[] { 1, 3 });
// Site  |  1    |  3
// Value |  2.2  |  4.4
```

Example of extracting site data from a `PinSiteData` object:

```csharp
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  1.5   |  1.6   |  1.7
// VCC1   |  3.3   |  3.4   |  3.5

// Gets the value of each pin for site 1.
IDictionary<string, double> site1Values = pinSiteData.ExtractSite(1);
// { ["VDET"] = 1.6, ["VCC1"] = 3.4 }

// Creates a new PinSiteData object that contains all pins, but only sites 0 and 2.
PinSiteData<double> subset = pinSiteData.ExtractSites(new int[] { 0, 2 });
// Pin    | Site 0 | Site 2
// VDET   |  1.5   |  1.7
// VCC1   |  3.3   |  3.5
```

Example of filtering data that is stored for all sites down to the active sites of the current code module:

```csharp
// Retrieves the data exactly as it was stored, regardless of the active sites.
PinSiteData<double> allSitesData = tsmContext.GetGlobalPinSiteData<double>("ReferenceData", filterForActiveSites: false);

// Creates a new PinSiteData object that contains only the active sites.
int[] activeSites = tsmContext.SiteNumbers.ToArray();
PinSiteData<double> activeSitesData = allSitesData.ExtractSites(activeSites);
```
