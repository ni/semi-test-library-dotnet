# Extracting a Pin Subset from PinSiteData

You can get the pin information, the data for a single pin, or the data for a subset of pins from a `PinSiteData` object. The methods that return pin data create a new object, and the original object is not modified.

The following table outlines the methods and properties available for extracting pin data.

**Table of Methods and Properties for Extracting Pin Data:**

|Member|Description|
| :- | :- |
|[`PinNames`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.PinNames.html)|Returns the pin names in the `PinSiteData` object.|
|[`SiteNumbersByPin`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.SiteNumbersByPin.html)|Returns a read-only dictionary where each key is a pin name and each value is the list of site numbers for that pin, in ascending order. This property is applicable only to release 26.5.0 or later.|
|[`GetValue(int siteNumber, string pinName)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.GetValue.html)|Returns the value of the specified pin and site.|
|[`TryGetValue(int siteNumber, string pinName, out T value)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.TryGetValue.html)|Gets the value of the specified pin and site, if it exists. Returns `true` if the pin and site exist, and `false` otherwise.|
|[`ExtractPin(string pinName)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.ExtractPin.html)|Returns a new `SiteData` object that contains the data of the specified pin.|
|[`ExtractPins(string[] pinNames)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.ExtractPins.html)|Returns a new `PinSiteData` object that contains only the specified pins.|

## Usage Considerations for Extracting a Pin Subset

When extracting pin data from `PinSiteData` objects you should consider the following:

1. The `ExtractPin` and `ExtractPins` methods return a new object. Modifying the returned object does not modify the original object, and vice versa. However, when `T` is a reference type, such as an array, both objects refer to the same value instances.
2. Every specified pin must exist. Otherwise, an `NISemiconductorTestException` is thrown with the message `The pin name {pinName} does not exist.` Use the `TryGetValue` method or check the `PinNames` property first if a pin might not exist.
3. The `GetValue`, `ExtractPin`, and `ExtractPins` methods throw an `NISemiconductorTestException` if the `PinSiteData` object contains no pins.
4. The `GetValue` method throws an `NISemiconductorTestException` if the specified pin contains no sites, or if the specified site does not exist for the pin.
5. The `TryGetValue` method returns `false` if the specified pin or site does not exist. However, it throws an `NISemiconductorTestException` if the specified pin exists but contains no sites.
6. The `ExtractPin` method returns an empty `SiteData` object if the specified pin contains no sites. For a jagged `PinSiteData` object, the returned `SiteData` object contains only the sites of that pin.
7. The `pinNames` input of the `ExtractPins` method must contain unique values. Otherwise, an `NISemiconductorTestException` is thrown. An `ArgumentException` is thrown if the `pinNames` input is an empty array.
8. The `PinNames` and `SiteNumbersByPin` properties return a new array or dictionary each time they are accessed. Store the returned value in a local variable instead of accessing the property repeatedly. The `SiteNumbersByPin` property is applicable only to release 26.5.0 or later.
9. The order of the `PinNames` property is not guaranteed to match the order in which the pins were added.

## Example Usage for Extracting a Pin Subset

Example of extracting pin data from a `PinSiteData` object:

```csharp
// Pin          | Site -1 | Site 0 | Site 1
// VDET         |  ---    |  1.5   |  1.6
// VCC1         |  ---    |  3.3   |  3.4
// SystemSupply | -22.5   |  ---   |  ---

// Gets the pin names and the sites of each pin.
string[] pinNames = pinSiteData.PinNames;
// { "VDET", "VCC1", "SystemSupply" }

IReadOnlyDictionary<string, IReadOnlyList<int>> siteNumbersByPin = pinSiteData.SiteNumbersByPin;
// { ["VDET"] = { 0, 1 }, ["VCC1"] = { 0, 1 }, ["SystemSupply"] = { -1 } }

// Gets the value of VCC1 for site 1.
double vcc1Site1Value = pinSiteData.GetValue(siteNumber: 1, pinName: "VCC1");
// 3.4

// Gets the value of VDET for site -1, if it exists.
bool valueExists = pinSiteData.TryGetValue(siteNumber: -1, pinName: "VDET", out double value);
// valueExists = false, value = 0.0

// Creates a new SiteData object that contains the data of VCC1.
SiteData<double> vcc1 = pinSiteData.ExtractPin("VCC1");
// Site  |  0    |  1
// Value |  3.3  |  3.4

// Creates a new PinSiteData object that contains only the DUT pins.
PinSiteData<double> dutPins = pinSiteData.ExtractPins(new string[] { "VDET", "VCC1" });
// Pin    | Site 0 | Site 1
// VDET   |  1.5   |  1.6
// VCC1   |  3.3   |  3.4

// Since the DUT pins all contain site 0, a site subset can now be extracted.
// Calling ExtractSites on the original object throws an exception, because SystemSupply does not contain site 0.
PinSiteData<double> site0Data = dutPins.ExtractSites(new int[] { 0 });
// Pin    | Site 0
// VDET   |  1.5
// VCC1   |  3.3
```
