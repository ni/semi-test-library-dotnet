# Working with PinSiteData

A `PinSiteData<T>` object holds values of type `T` for one or more pins, where each pin contains one value per site. You can create a `PinSiteData` object with all of its pins, sites, and values defined upfront, or you can create it empty and add pins, add sites, and set values later.

This topic describes how to create, populate, and modify `PinSiteData` objects.

>[!NOTE]
> The following members were introduced in the 26.5.0 release:
>
> - The `PinSiteData()`, `PinSiteData(string pinName, params int[] siteNumbers)`, and `PinSiteData(string[] pinNames, params int[] siteNumbers)` constructors.
> - The `AddPin`, `RemovePin`, `AddSite`, `RemoveSite`, and `SetValue` methods.
> - The `SiteNumbersByPin` property.
>
> Find examples for using the `PinSiteData` object [here on GitHub](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/CodeSnippets/DataAbstraction).

---

## Creating and Populating PinSiteData

The following table outlines the constructors available for `PinSiteData<T>`.

**Table of PinSiteData Constructors:**

|Constructor|Description|
| :- | :- |
|[`PinSiteData()`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.-ctor.html)|Initializes an empty `PinSiteData` object with no pins, no sites, and no values.|
|[`PinSiteData(string pinName, params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.-ctor.html)|Initializes a `PinSiteData` object with a single pin and the specified sites. The value of each site is the default value of `T`.|
|[`PinSiteData(string[] pinNames, params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.-ctor.html)|Initializes a `PinSiteData` object with the specified pins, where each pin contains the specified sites. The value of each site is the default value of `T`.|
|[`PinSiteData(string[] pinNames, SiteData<T>[] perPinSiteData)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.-ctor.html)|Initializes a `PinSiteData` object from an array of `SiteData` objects, where each element is the data for the pin at the same index in `pinNames`.|
|[`PinSiteData(Dictionary<string, IDictionary<int, T>> pinSiteData)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.-ctor.html)|Initializes a `PinSiteData` object from a dictionary of pin- and site-unique data.|
|[`PinSiteData(int[] siteNumbers, Dictionary<string, T> pinData)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.-ctor.html)|Initializes a `PinSiteData` object from a dictionary of pin-unique data. Each pin value is repeated across all the specified sites.|
|[`PinSiteData(string[] pinNames, int[] siteNumbers, T data)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.-ctor.html)|Initializes a `PinSiteData` object with the same value for all pins and sites.|
|[`PinSiteData(string[] pinNames, int[] siteNumbers, T[] perPinData)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.-ctor.html)|Initializes a `PinSiteData` object with pin-unique data. Each pin value is repeated across all sites.|
|[`PinSiteData(int[] siteNumbers, string[] pinNames, T[] perSiteData)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.-ctor.html)|Initializes a `PinSiteData` object with site-unique data. Each site value is repeated across all pins.|
|[`PinSiteData(string[] pinNames, int[] siteNumbers, T[][] perPinPerSiteData)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.-ctor.html)|Initializes a `PinSiteData` object with pin- and site-unique data, where the first dimension represents pins and the second dimension represents sites.|
|[`PinSiteData(int[] siteNumbers, string[] pinNames, T[][] perSitePerPinData)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.-ctor.html)|Initializes a `PinSiteData` object with site- and pin-unique data, where the first dimension represents sites and the second dimension represents pins.|

### Usage Considerations for Creating PinSiteData

When creating `PinSiteData` objects you should consider the following:

1. If all the pin names, site numbers, and values are known upfront, passing them to a constructor is the most concise approach.
2. If the pin names and site numbers are known upfront but the values are not, use the `PinSiteData(string[] pinNames, params int[] siteNumbers)` constructor. Then, set the values with the `SetValue` method. This is applicable to release 26.5.0 or later.
3. If the pin names or site numbers are determined at runtime, such as inside a loop or based on a condition, create the object with the `PinSiteData()` constructor. Then, populate the object with the `AddPin`, `AddSite`, and `SetValue` methods. This is only applicable to release 26.5.0 or later.
4. If the `siteNumbers` input of the `PinSiteData(string pinName, params int[] siteNumbers)` or `PinSiteData(string[] pinNames, params int[] siteNumbers)` constructor is not provided, `null`, or an empty array, each pin is created with no sites. Add sites later with the `AddSite` or `SetValue` methods. This is only applicable to release 26.5.0 or later.
5. Pin names must not be `null` or empty strings. An `ArgumentNullException` is thrown if the `pinName` or `pinNames` input, or any of the specified pin names, is `null`. An `ArgumentException` is thrown if the `pinNames` input is an empty array, or if any of the specified pin names is an empty string. This is only applicable to release 26.5.0 or later.
6. The `pinNames` and `siteNumbers` arrays passed to a constructor must each contain unique values. Otherwise, an `NISemiconductorTestException` is thrown.
7. The lengths of the data arrays must match the lengths of the corresponding `pinNames` and `siteNumbers` arrays. Otherwise, an `NISemiconductorTestException` is thrown.
8. The `PinSiteData(string[] pinNames, SiteData<T>[] perPinSiteData)` constructor copies each `SiteData` object. Changes made later to the original `SiteData` objects do not affect the `PinSiteData` object.
9. Use `-1` as the site number to represent site-agnostic data, such as data associated with a system pin.

### Example Usage for Creating PinSiteData

Example of creating a `PinSiteData` object with all pins, sites, and values defined upfront:

```csharp
var pinNames = new string[] { "VDET", "VCC1" };
var siteNumbers = new int[] { 2, 4, 3 };
var perPinData = new double[] { 42, 105 };

// Creates a PinSiteData object with pin-unique data, where each pin value is repeated across all sites.
var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers, perPinData);
// Pin    | Site 2 | Site 3 | Site 4
// VDET   |  42.0  |  42.0  |  42.0
// VCC1   | 105.0  | 105.0  | 105.0
```

Example of creating a `PinSiteData` object with known pins and sites, and setting the values afterwards (applicable only to release 26.5.0 or later):

```csharp
var pinNames = new string[] { "VDET", "VCC1" };
var siteNumbers = new int[] { 2, 4, 3 };

// Creates a PinSiteData object with all pins and sites. Each value is initialized with the default value of double (0.0).
var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers);
// Pin    | Site 2 | Site 3 | Site 4
// VDET   |  0.0   |  0.0   |  0.0
// VCC1   |  0.0   |  0.0   |  0.0

// Sets the same value for all pins and sites.
pinSiteData.SetValue(55);
// Pin    | Site 2 | Site 3 | Site 4
// VDET   |  55.0  |  55.0  |  55.0
// VCC1   |  55.0  |  55.0  |  55.0
```

Example of creating an empty `PinSiteData` object and populating it afterwards (applicable only to release 26.5.0 or later):

```csharp
// Creates an empty PinSiteData object.
var pinSiteData = new PinSiteData<double>();
// (empty)

// Adds pins. No sites exist yet, so each pin is created with no sites.
pinSiteData.AddPin("VDET", "VCC1");
// Pin    | (no sites)
// VDET   |
// VCC1   |

// Adds sites across all pins. Each site is initialized with the default value of double (0.0).
pinSiteData.AddSite(0, 1);
// Pin    | Site 0 | Site 1
// VDET   |  0.0   |  0.0
// VCC1   |  0.0   |  0.0

// Sets a value for each pin across all its sites.
pinSiteData.SetValue(value: 1.5, pinNames: "VDET");
pinSiteData.SetValue(value: 3.3, pinNames: "VCC1");
// Pin    | Site 0 | Site 1
// VDET   |  1.5   |  1.5
// VCC1   |  3.3   |  3.3
```

---

## Adding and Removing Pins from PinSiteData

The following table outlines the methods available for adding and removing pins from a `PinSiteData<T>` object.

>[!NOTE]
> Adding and removing pins is applicable only to release 26.5.0 or later.

**Table of Methods for Adding and Removing Pins:**

|Method|Description|
| :- | :- |
|[`AddPin(params string[] pinNames)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.AddPin.html)|Adds the specified pins. Each new pin contains all the sites that exist in the `PinSiteData` object, initialized with the default value of `T`.|
|[`RemovePin(params string[] pinNames)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.RemovePin.html)|Removes the specified pins and all their site values.|

### Usage Considerations for Adding and Removing Pins

When adding and removing pins from `PinSiteData` objects you should consider the following:

1. You can pass the pin names either as an array, for example `pinSiteData.AddPin(pinNames)`, or as separate arguments, for example `pinSiteData.AddPin("VCC1", "VCC2")`.
2. Each new pin contains all the site numbers returned by the `SiteNumbers` property, which is the union of the sites across all existing pins. If different pins contain different sites, the new pin contains every site that exists in any pin.
3. If the `PinSiteData` object contains no sites, each new pin is created with no sites. Add sites later with the `AddSite` or `SetValue` methods.
4. New sites are initialized with the default value of `T`. For example, the default value is `0` for `int`, `0.0` for `double`, `false` for `bool`, and `null` for reference types, such as arrays and strings.
5. The `AddPin` method skips pins that already exist.
6. The `RemovePin` method skips pins that do not exist.
7. An `ArgumentNullException` is thrown if the `pinNames` input, or any of the specified pin names, is `null`. An `ArgumentException` is thrown if the `pinNames` input is an empty array, if no pin name is provided, or if any of the specified pin names is an empty string.
8. Removing all pins leaves the `PinSiteData` object empty. You must add a pin with the `AddPin` method before calling the `AddSite(params int[] siteNumbers)` method again.

### Example Usage for Adding and Removing Pins

Example of adding and removing pins (applicable only to release 26.5.0 or later):

```csharp
var pinSiteData = new PinSiteData<double>("VDET", 0, 1, 2);
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  0.0   |  0.0   |  0.0

// Adds pins VCC1 and VCC2. The new pins contain the existing sites 0, 1, and 2.
pinSiteData.AddPin("VCC1", "VCC2");
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  0.0   |  0.0   |  0.0
// VCC1   |  0.0   |  0.0   |  0.0
// VCC2   |  0.0   |  0.0   |  0.0

// Removes pins VCC1 and VCC2, including all their site values.
pinSiteData.RemovePin("VCC1", "VCC2");
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  0.0   |  0.0   |  0.0
```

Example of adding a pin to a `PinSiteData` object where pins contain different sites (applicable only to release 26.5.0 or later):

```csharp
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  1.0   |  1.0   |  ---
// VCC1   |  2.0   |  ---   |  2.0

// The new pin contains the union of the sites across all existing pins (0, 1, and 2).
pinSiteData.AddPin("VCC2");
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  1.0   |  1.0   |  ---
// VCC1   |  2.0   |  ---   |  2.0
// VCC2   |  0.0   |  0.0   |  0.0
```

---

## Adding and Removing Sites from PinSiteData



The following table outlines the methods available for adding and removing sites from a `PinSiteData<T>` object.

>[!NOTE]
> Adding and removing sites is applicable only to release 26.5.0 or later.

**Table of Methods for Adding and Removing Sites:**

|Method|Description|
| :- | :- |
|[`AddSite(params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.AddSite.html)|Adds the specified sites to all existing pins.|
|[`AddSite(string pinName, params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.AddSite.html)|Adds the specified sites to the specified pin. The pin is added first if it does not exist.|
|[`AddSite(string[] pinNames, params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.AddSite.html)|Adds the specified sites to the specified pins. Any pin that does not exist is added first.|
|[`RemoveSite(params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.RemoveSite.html)|Removes the specified sites from all pins.|
|[`RemoveSite(string pinName, params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.RemoveSite.html)|Removes the specified sites from the specified pin.|
|[`RemoveSite(string[] pinNames, params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.RemoveSite.html)|Removes the specified sites from the specified pins.|

### Usage Considerations for Adding and Removing Sites

When adding and removing sites from `PinSiteData` objects you should consider the following:

1. New sites are initialized with the default value of `T`. The `AddSite` methods skip sites that already exist for a pin. The values of existing sites are not changed.
2. The `AddSite(params int[] siteNumbers)` method requires at least one pin. If the `PinSiteData` object contains no pins, an `NISemiconductorTestException` is thrown. Call the `AddPin` method first when you create the object with the `PinSiteData()` constructor, or after you remove all pins with the `RemovePin` method.
3. The `AddSite(string pinName, params int[] siteNumbers)` and `AddSite(string[] pinNames, params int[] siteNumbers)` methods add any of the specified pins that do not exist. They do not require the `PinSiteData` object to contain any pins.
4. The `RemoveSite` methods skip sites that do not exist for a pin. The `RemoveSite(string pinName, params int[] siteNumbers)` and `RemoveSite(string[] pinNames, params int[] siteNumbers)` methods also skip pins that do not exist.
5. Removing all sites from a pin does not remove the pin. Use the `RemovePin` method to remove the pin.
6. An `ArgumentNullException` is thrown if the `siteNumbers` input is `null`. An `ArgumentException` is thrown if the `siteNumbers` input is an empty array or if no site number is provided. For the `AddSite` methods that accept pin names, the same exceptions are thrown for `null` or empty pin names. The `RemoveSite` methods throw these exceptions only when at least one of the targeted pins exists.
7. The `AddSite` and `RemoveSite` methods that accept pin names can result in a jagged `PinSiteData` object, where different pins contain different sites. Use these methods with caution.
8. The `SiteNumbers` property returns the union of sites across all pins. For a jagged `PinSiteData` object, use the `SiteNumbersByPin` property to get the sites of each pin.
9. The `GetValue` method throws an `NISemiconductorTestException` if the specified pin does not exist or does not contain the specified site. The [math operations](../MathOperations/MathOperationsWithPinSiteData.md) throw an `NISemiconductorTestException` if the `PinSiteData` object is empty or if any pin contains no sites.

### Example Usage for Adding and Removing Sites

Example of adding and removing sites across all pins (applicable only to release 26.5.0 or later):

```csharp
var pinSiteData = new PinSiteData<double>(new string[] { "VDET", "VCC1" }, 0, 1);
// Pin    | Site 0 | Site 1
// VDET   |  0.0   |  0.0
// VCC1   |  0.0   |  0.0

// Adds sites 2 and 3 to all pins.
pinSiteData.AddSite(2, 3);
// Pin    | Site 0 | Site 1 | Site 2 | Site 3
// VDET   |  0.0   |  0.0   |  0.0   |  0.0
// VCC1   |  0.0   |  0.0   |  0.0   |  0.0

// Removes sites 2 and 3 from all pins.
pinSiteData.RemoveSite(2, 3);
// Pin    | Site 0 | Site 1
// VDET   |  0.0   |  0.0
// VCC1   |  0.0   |  0.0
```

Example of adding and removing sites for specific pins, which results in a jagged `PinSiteData` object (applicable only to release 26.5.0 or later):

```csharp
var pinSiteData = new PinSiteData<double>(new string[] { "VDET", "VCC1", "VCC2" }, 0, 1);
// Pin    | Site 0 | Site 1
// VDET   |  0.0   |  0.0
// VCC1   |  0.0   |  0.0
// VCC2   |  0.0   |  0.0

// Adds site 2 to VDET and VCC1 only.
pinSiteData.AddSite(pinNames: new string[] { "VDET", "VCC1" }, siteNumbers: 2);
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  0.0   |  0.0   |  0.0
// VCC1   |  0.0   |  0.0   |  0.0
// VCC2   |  0.0   |  0.0   |  ---

// Removes site 0 from VCC1 only.
pinSiteData.RemoveSite(pinName: "VCC1", siteNumbers: 0);
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  0.0   |  0.0   |  0.0
// VCC1   |  ---   |  0.0   |  0.0
// VCC2   |  0.0   |  0.0   |  ---

// Returns the union of the sites across all pins: { 0, 1, 2 }.
int[] allSiteNumbers = pinSiteData.SiteNumbers;

// Returns the sites of each pin:
// { ["VDET"] = { 0, 1, 2 }, ["VCC1"] = { 1, 2 }, ["VCC2"] = { 0, 1 } }
IReadOnlyDictionary<string, IReadOnlyList<int>> siteNumbersByPin = pinSiteData.SiteNumbersByPin;
```

Example of adding a site-agnostic site for a system pin (applicable only to release 26.5.0 or later):

```csharp
var pinSiteData = new PinSiteData<double>(new string[] { "VCC1", "VCC2" }, 0, 1);

// Adds the SystemSupply pin with the site-agnostic site -1. The pin does not exist, so it is added first.
pinSiteData.AddSite("SystemSupply", -1);
// Pin          | Site -1 | Site 0 | Site 1
// VCC1         |  ---    |  0.0   |  0.0
// VCC2         |  ---    |  0.0   |  0.0
// SystemSupply |  0.0    |  ---   |  ---
```

---

## Setting Data Values in PinSiteData

The following table outlines the methods available for setting values in a `PinSiteData<T>` object.

>[!NOTE]
> Setting data values is applicable only to release 26.5.0 or later.

**Table of Methods for Setting Data Values:**

|Method|Description|
| :- | :- |
|[`SetValue(T value)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.SetValue.html)|Sets the value for all sites of all pins.|
|[`SetValue(T value, params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.SetValue.html)|Sets the value for the specified sites of all pins. Sites that do not exist are added first.|
|[`SetValue(T value, params string[] pinNames)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.SetValue.html)|Sets the value for all sites of the specified pins. Pins that do not exist are added first.|
|[`SetValue(T value, string pinName, params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.SetValue.html)|Sets the value for the specified sites of the specified pin. The pin and sites are added first if they do not exist.|
|[`SetValue(T value, string[] pinNames, params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.SetValue.html)|Sets the value for the specified sites of the specified pins. Pins and sites are added first if they do not exist.|

### Usage Considerations for Setting Data Values

When setting values in `PinSiteData` objects you should consider the following:

1. The `SetValue` methods overwrite any existing value for the affected pin and site combinations. Pins that are not specified are not changed.
2. To set the same value for a pin across all its sites, use `SetValue(T value, params string[] pinNames)`. To set a site-specific value, use `SetValue(T value, string pinName, params int[] siteNumbers)` or `SetValue(T value, string[] pinNames, params int[] siteNumbers)`.
3. Use named arguments, such as `pinName:`, `pinNames:`, and `siteNumbers:`, to make it clear which overload is called.
4. The `SetValue(T value)` method throws an `NISemiconductorTestException` if the `PinSiteData` object contains no pins, or if any pin contains no sites.
5. The `SetValue(T value, params int[] siteNumbers)` method adds the specified sites to every pin if they do not exist. It throws an `NISemiconductorTestException` if the `PinSiteData` object contains no pins.
6. The `SetValue(T value, params string[] pinNames)` method behaves as follows:
   1. For each specified pin that exists, the value is set for all sites of that pin.
   2. Each specified pin that does not exist is added with all the site numbers returned by the `SiteNumbers` property, and the value is set for those sites.
   3. An `NISemiconductorTestException` is thrown if the `PinSiteData` object contains no sites, or if any of the specified pins that exist contains no sites.
7. The `SetValue(T value, string pinName, params int[] siteNumbers)` and `SetValue(T value, string[] pinNames, params int[] siteNumbers)` methods add any of the specified pins and sites that do not exist, and then set the value. You do not need to call the `AddPin` or `AddSite` methods first.
8. If the `siteNumbers` input is `null` or an empty array:
   1. The `SetValue(T value, params int[] siteNumbers)` method sets the value for all existing sites of each pin. An `NISemiconductorTestException` is thrown if any pin contains no sites.
   2. The `SetValue(T value, string pinName, params int[] siteNumbers)` and `SetValue(T value, string[] pinNames, params int[] siteNumbers)` methods set the value for all existing sites of each specified pin. Each specified pin that does not exist is added with all the site numbers returned by the `SiteNumbers` property. An `NISemiconductorTestException` is thrown if a specified pin that exists contains no sites, or if a specified pin does not exist and the `PinSiteData` object contains no sites.
9. An `ArgumentNullException` is thrown if the `pinName` or `pinNames` input, or any of the specified pin names, is `null`. An `ArgumentException` is thrown if the `pinNames` input is an empty array, or if any of the specified pin names is an empty string.
10. The `SetValue(T value, string[] pinNames, params int[] siteNumbers)` method processes the pins in order. If an exception is thrown for a pin, the values already set for the preceding pins remain.
11. When `T` is a reference type, such as an array, the `SetValue` methods assign the same instance to every affected pin and site combination. Modifying that instance later changes the value for all of those combinations. Assign a separate instance to each combination if the values must be independent.
12. The `ExtractPin` method returns a copy of the data for the specified pin. Changing the returned `SiteData` object does not change the `PinSiteData` object. Use the `SetValue` methods to change the values of the `PinSiteData` object.

>[!CAUTION]
> Take care when setting values in a `PinSiteData` object that contains system pins with site-agnostic data (site `-1`):
>
> - The `SetValue(T value)` method sets the value for all sites, including site `-1`.
> - The `SetValue(T value, params int[] siteNumbers)` method adds the specified sites to all pins, including system pins.
> - Pins added by the `SetValue(T value, params string[] pinNames)` method contain all the site numbers in the `PinSiteData` object, including site `-1`.
>
> Instead, specify both the pin name and the site numbers explicitly with the `SetValue(T value, string pinName, params int[] siteNumbers)` or `SetValue(T value, string[] pinNames, params int[] siteNumbers)` method.

### Example Usage for Setting Data Values

Example of setting a value for all sites of a pin, and setting site-specific values (applicable only to release 26.5.0 or later):

```csharp
var pinSiteData = new PinSiteData<double>(new string[] { "VDET", "VCC1", "VCC2" }, 0, 1, 2);
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  0.0   |  0.0   |  0.0
// VCC1   |  0.0   |  0.0   |  0.0
// VCC2   |  0.0   |  0.0   |  0.0

// Sets the same value for all pins and sites.
pinSiteData.SetValue(1.5);
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  1.5   |  1.5   |  1.5
// VCC1   |  1.5   |  1.5   |  1.5
// VCC2   |  1.5   |  1.5   |  1.5

// Sets a value for site 1 of all pins.
pinSiteData.SetValue(value: 3.3, siteNumbers: 1);
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  1.5   |  3.3   |  1.5
// VCC1   |  1.5   |  3.3   |  1.5
// VCC2   |  1.5   |  3.3   |  1.5

// Sets a value for all sites of VDET.
pinSiteData.SetValue(value: 0.9, pinNames: "VDET");
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  0.9   |  0.9   |  0.9
// VCC1   |  1.5   |  3.3   |  1.5
// VCC2   |  1.5   |  3.3   |  1.5

// Sets a value for site 0 of VCC1 only.
pinSiteData.SetValue(value: 5.0, pinName: "VCC1", siteNumbers: 0);
// Pin    | Site 0 | Site 1 | Site 2
// VDET   |  0.9   |  0.9   |  0.9
// VCC1   |  5.0   |  3.3   |  1.5
// VCC2   |  1.5   |  3.3   |  1.5

// Sets a value for sites 2 and 3 of VCC1 and VCC2. Site 3 does not exist, so it is added to VCC1 and VCC2 only.
pinSiteData.SetValue(value: 2.2, pinNames: new string[] { "VCC1", "VCC2" }, siteNumbers: new int[] { 2, 3 });
// Pin    | Site 0 | Site 1 | Site 2 | Site 3
// VDET   |  0.9   |  0.9   |  0.9   |  ---
// VCC1   |  5.0   |  3.3   |  2.2   |  2.2
// VCC2   |  1.5   |  3.3   |  2.2   |  2.2
```

Example of building a `PinSiteData` object only with `SetValue`, including a system pin (applicable only to release 26.5.0 or later):

```csharp
var siteNumbers = new int[] { 0, 1 };

var pinSiteData = new PinSiteData<double>();
// Adds VCC1 with sites 0 and 1, and sets the value.
pinSiteData.SetValue(value: 1.5, pinName: "VCC1", siteNumbers: siteNumbers);
// Adds SystemSupply with the site-agnostic site -1, and sets the value.
pinSiteData.SetValue(value: -22.5, pinName: "SystemSupply", siteNumbers: -1);
// Pin          | Site -1 | Site 0 | Site 1
// VCC1         |  ---    |  1.5   |  1.5
// SystemSupply | -22.5   |  ---   |  ---
```

---

**Table of Pin and Site Properties:**

The following table outlines the properties available for getting the pins and sites of a `PinSiteData<T>` object.

|Property|Description|
| :- | :- |
|[`PinNames`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.PinNames.html)|Returns the pin names in the `PinSiteData` object.|
|[`SiteNumbers`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.SiteNumbers.html)|Returns the union of the site numbers across all pins, in ascending order.|
|[`SiteNumbersByPin`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.SiteNumbersByPin.html)|Returns a read-only dictionary where each key is a pin name and each value is the list of site numbers for that pin, in ascending order.|This property is available only in release 26.5.0 or later.

---

## Working with PinSiteData in Loops

You can use loops to build a `PinSiteData` object when the pin names, site numbers, or values come from another data structure, or are computed at runtime. How you do this depends on the release you use:

- Prior to release 26.5.0, the `PinSiteData` class does not provide methods to add pins, add sites, or set values. Collect the values in a .NET collection inside the loop, and then create the `PinSiteData` object after the loop.
- In release 26.5.0 or later, you can add pins, add sites, and set values on a `PinSiteData` object inside the loop.

When working with `PinSiteData` in loops in any release you should consider the following:

1. The order of the `PinNames` property is not guaranteed to match the order in which the pins were passed to the constructor or added. When you index into a data array by pin, iterate over the same `pinNames` array that is used to arrange the data.
2. To transform every value of an existing `PinSiteData` object into a new `PinSiteData` object, the `Select` method can be more concise than a loop.

### Working with PinSiteData in Loops Prior to Release 26.5.0

#### Usage Considerations Prior to Release 26.5.0

When working with `PinSiteData` in loops prior to release 26.5.0 you should consider the following:

1. If the pin names or site numbers are determined at runtime, or if different pins contain different sites, collect the values in a `Dictionary<string, IDictionary<int, T>>` inside the loop. Then, pass the dictionary to the `PinSiteData(Dictionary<string, IDictionary<int, T>> pinSiteData)` constructor after the loop.
2. If all the pin names and site numbers are known before the loop, collect the values in an array inside the loop. Then, pass the array to the matching constructor after the loop, such as the `PinSiteData(string[] pinNames, int[] siteNumbers, T[][] perPinPerSiteData)` constructor.
3. To iterate over the sites of each pin in an existing `PinSiteData` object, iterate over the `PinNames` property and use the `ExtractPin` method to get the `SiteData` object of each pin. Then, iterate over the `SiteNumbers` property of that `SiteData` object. This approach also works when different pins contain different sites.

#### Example Usage Prior to Release 26.5.0

Example of calculating a pin- and site-unique value inside nested loops, and creating the `PinSiteData` object after the loops:

```csharp
var pinNames = new string[] { "VDET", "VCC1" };
var siteNumbers = new int[] { 2, 4, 3 };
// The first dimension represents pins and the second dimension represents sites,
// in the same order as the pinNames and siteNumbers arrays.
var perPinPerSiteData = new double[pinNames.Length][];
for (int pinIndex = 0; pinIndex < pinNames.Length; pinIndex++)
{
    perPinPerSiteData[pinIndex] = new double[siteNumbers.Length];
    for (int siteIndex = 0; siteIndex < siteNumbers.Length; siteIndex++)
    {
        // CalculateValue represents your own calculation.
        perPinPerSiteData[pinIndex][siteIndex] = CalculateValue(pinNames[pinIndex], siteNumbers[siteIndex]);
    }
}

// Creates the PinSiteData object after the loops.
var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers, perPinPerSiteData);
```

Example of iterating over the sites of each pin in an existing `PinSiteData` object to build a new `PinSiteData` object:

```csharp
PinSiteData<double> measurement = dcPowerPins.MeasureVoltage();
double upperLimit = 1.2;

var isAboveLimitData = new Dictionary<string, IDictionary<int, bool>>();
foreach (string pinName in measurement.PinNames)
{
    SiteData<double> perPinMeasurement = measurement.ExtractPin(pinName);
    var perSiteResults = new Dictionary<int, bool>();
    foreach (int siteNumber in perPinMeasurement.SiteNumbers)
    {
        perSiteResults.Add(siteNumber, perPinMeasurement.GetValue(siteNumber) > upperLimit);
    }
    isAboveLimitData.Add(pinName, perSiteResults);
}

// Creates the PinSiteData object after the loops.
var isAboveLimit = new PinSiteData<bool>(isAboveLimitData);
```

### Working with PinSiteData in Loops in Release 26.5.0 or Later

#### Usage Considerations for Release 26.5.0 or Later

When adding pins and sites and setting values inside loops in release 26.5.0 or later you should consider the following:

1. If all the pin names and site numbers are known before the loop, pass them to a constructor, or add them with a single call to the `AddPin` and `AddSite` methods. Then, set only the values inside the loop.
2. Calling `SetValue(T value, string pinName, params int[] siteNumbers)` inside a loop adds each pin and site the first time it is encountered. You do not need to call the `AddPin` or `AddSite` methods separately.
3. The `PinNames`, `SiteNumbers`, and `SiteNumbersByPin` properties return a new array or dictionary each time they are accessed. Therefore, you can add or remove pins and sites while iterating over the returned values. Store the returned value in a local variable before the loop instead of accessing the property in every iteration.
4. To iterate over a jagged `PinSiteData` object, use the `SiteNumbersByPin` property to get the sites of each pin. The `SiteNumbers` property returns the union of the sites across all pins, so not every pin contains every site in it.

>[!CAUTION]
> `PinSiteData` objects are not thread-safe. Do not call the `AddPin`, `RemovePin`, `AddSite`, `RemoveSite`, or `SetValue` methods on the same `PinSiteData` object from multiple threads at the same time, such as inside a `Parallel.For` or `Parallel.ForEach` loop, or in the actions passed to the `InvokeInParallel` method. This applies even when each thread accesses a different pin or site. Concurrent modifications can corrupt the object or lose values without throwing an exception. Reading values from multiple threads is safe only when no thread modifies the object at the same time.
>
> Instead, store the result of each parallel iteration in its own element of a preallocated array. After the parallel loop completes, populate the `PinSiteData` object from the array, either by passing the array to a constructor or by calling the `SetValue` method in a sequential loop.

#### Example Usage for Release 26.5.0 or Later

Example of adding pins and sites before the loop, and setting a pin-unique value inside the loop:

```csharp
var pinNames = new string[] { "VDET", "VCC1" };
var siteNumbers = new int[] { 2, 4, 3 };
var perPinData = new double[] { 42, 105 };

var pinSiteData = new PinSiteData<double>();
pinSiteData.AddPin(pinNames);
pinSiteData.AddSite(siteNumbers);
// Sets the value for each pin across all its sites.
for (int i = 0; i < pinNames.Length; i++)
{
    pinSiteData.SetValue(value: perPinData[i], pinNames: pinNames[i]);
}
// Pin    | Site 2 | Site 3 | Site 4
// VDET   |  42.0  |  42.0  |  42.0
// VCC1   | 105.0  | 105.0  | 105.0
```

Example of setting a site-unique value across all pins inside the loop:

```csharp
var pinNames = new string[] { "VDET", "VCC1" };
var siteNumbers = new int[] { 2, 4, 3 };
var perSiteData = new double[] { 42, 105, 55 };

var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers);
// Sets the value for each site across all pins.
for (int i = 0; i < siteNumbers.Length; i++)
{
    pinSiteData.SetValue(value: perSiteData[i], siteNumbers: siteNumbers[i]);
}
// Pin    | Site 2 | Site 3 | Site 4
// VDET   |  42.0  |  55.0  | 105.0
// VCC1   |  42.0  |  55.0  | 105.0
```

Example of setting a pin- and site-unique value inside nested loops:

```csharp
var pinNames = new string[] { "VDET", "VCC1" };
var siteNumbers = new int[] { 2, 4, 3 };
// The first dimension represents pins and the second dimension represents sites,
// in the same order as the pinNames and siteNumbers arrays.
var perPinPerSiteData = new double[][]
{
    new double[] { 42, 105, 206 }, // VDET data for sites 2, 4, 3.
    new double[] { 55, 2048, 0.5 } // VCC1 data for sites 2, 4, 3.
};

var pinSiteData = new PinSiteData<double>(pinNames);
pinSiteData.AddSite(siteNumbers);
for (int pinIndex = 0; pinIndex < pinNames.Length; pinIndex++)
{
    for (int siteIndex = 0; siteIndex < siteNumbers.Length; siteIndex++)
    {
        pinSiteData.SetValue(perPinPerSiteData[pinIndex][siteIndex], pinNames[pinIndex], siteNumbers[siteIndex]);
    }
}
// Pin    | Site 2 | Site 3 | Site 4
// VDET   |  42.0  | 206.0  | 105.0
// VCC1   |  55.0  |   0.5  | 2048.0
```

Example of building a `PinSiteData` object from a dictionary, where `SetValue` adds each pin and site as it is encountered:

```csharp
var pinAndSiteUniqueData = new Dictionary<string, IDictionary<int, double>>
{
    ["VCC1"] = new Dictionary<int, double> { [0] = 1.5, [1] = 11.5 },
    ["SystemSupply"] = new Dictionary<int, double> { [-1] = -22.5 }
};

var pinSiteData = new PinSiteData<double>();
foreach (var pinEntry in pinAndSiteUniqueData)
{
    foreach (var siteEntry in pinEntry.Value)
    {
        // Pins and sites are added automatically if they do not exist.
        pinSiteData.SetValue(value: siteEntry.Value, pinName: pinEntry.Key, siteNumbers: siteEntry.Key);
    }
}
// Pin          | Site -1 | Site 0 | Site 1
// VCC1         |  ---    |  1.5   |  11.5
// SystemSupply | -22.5   |  ---   |  ---
```

Example of iterating over the sites of each pin in a jagged `PinSiteData` object to build a new `PinSiteData` object:

```csharp
PinSiteData<double> measurement = dcPowerPins.MeasureVoltage();
double upperLimit = 1.2;

var isAboveLimit = new PinSiteData<bool>();
IReadOnlyDictionary<string, IReadOnlyList<int>> siteNumbersByPin = measurement.SiteNumbersByPin;
foreach (var pinAndSites in siteNumbersByPin)
{
    string pinName = pinAndSites.Key;
    foreach (int siteNumber in pinAndSites.Value)
    {
        bool value = measurement.GetValue(siteNumber, pinName) > upperLimit;
        isAboveLimit.SetValue(value, pinName, siteNumber);
    }
}
```

Example of calculating values inside a `Parallel.For` loop, and setting the values after the parallel loop completes:

```csharp
var pinNames = new string[] { "VDET", "VCC1" };
var siteNumbers = new int[] { 2, 4, 3 };
var perSiteData = new double[siteNumbers.Length];

// Each iteration writes only to its own element of the perSiteData array.
// Do not call the SetValue method on a shared PinSiteData object inside the parallel loop.
Parallel.For(0, siteNumbers.Length, i =>
{
    // CalculateValue represents your own calculation.
    perSiteData[i] = CalculateValue(siteNumbers[i]);
});

var pinSiteData = new PinSiteData<double>(pinNames, siteNumbers);
// Sets the values sequentially, after the parallel loop completes.
for (int i = 0; i < siteNumbers.Length; i++)
{
    pinSiteData.SetValue(value: perSiteData[i], siteNumbers: siteNumbers[i]);
}
```
