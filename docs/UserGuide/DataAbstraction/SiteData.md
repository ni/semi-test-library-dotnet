# Working with SiteData

A `SiteData<T>` object holds one value of type `T` for each site. You can create a `SiteData` object with all of its sites and values defined upfront, or you can create it empty and add sites and set values later.

This topic describes how to create, populate, and modify `SiteData` objects.

>[!NOTE]
> The following members were introduced in the 26.5.0 release:
>
> - The `SiteData()` and `SiteData(int siteNumber)` constructors.
> - The `AddSite`, `RemoveSite`, and `SetValue` methods.
>
> Find examples for using the `SiteData` object [here on GitHub](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/CodeSnippets/DataAbstraction).

---

## Creating and Populating SiteData

The following table outlines the constructors available for `SiteData<T>`.

**Table of SiteData Constructors:**

|Constructor|Description|
| :- | :- |
|[`SiteData()`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.-ctor.html)|Initializes an empty `SiteData` object with no sites and no values.|
|[`SiteData(int siteNumber)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.-ctor.html)|Initializes a `SiteData` object with a single site. The value of the site is the default value of `T`.|
|[`SiteData(T[] data)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.-ctor.html)|Initializes a `SiteData` object from a per-site data array, where the array index represents the site number.|
|[`SiteData(IDictionary<int, T> data)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.-ctor.html)|Initializes a `SiteData` object from a dictionary of site-unique data, where each key is a site number.|
|[`SiteData(int[] siteNumbers, T data)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.-ctor.html)|Initializes a `SiteData` object with the same value for all the specified sites.|
|[`SiteData(int[] siteNumbers, T[] perSiteData)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.-ctor.html)|Initializes a `SiteData` object with site-unique data, where each element in `perSiteData` is the value for the site at the same index in `siteNumbers`.|

### Usage Considerations for Creating SiteData

When creating `SiteData` objects you should consider the following:

1. If all the site numbers and values are known upfront, passing them to a constructor is the most concise approach.
2. If the site numbers or values are determined at runtime, such as inside a loop or based on a condition, create the object with the `SiteData()` or `SiteData(int siteNumber)` constructor. Then, populate the object with the `AddSite` and `SetValue` methods. This is only applicable to release 26.5.0 or later.
3. Use the `SiteData(T[] data)` constructor only for niche scenarios. The active site numbers do not always start at 0 and are not always sequential. For example, the active sites can be 4, 9 and 12.
4. The `siteNumbers` array passed to a constructor must contain unique values. Otherwise, an `NISemiconductorTestException` is thrown.
5. An `ArgumentNullException` is thrown if the `siteNumbers` array passed to a constructor is `null`. An `ArgumentException` is thrown if the array is empty. This is only applicable to release 26.5.0 or later.
6. The `siteNumbers` and `perSiteData` arrays must have equal lengths. Otherwise, an `NISemiconductorTestException` is thrown.
7. The site numbers do not need to be in sequential order. The `SiteNumbers` property always returns the site numbers in ascending order.
8. Use `-1` as the site number to represent site-agnostic data, such as data associated with a system resource. A `SiteData` object can contain only one site-agnostic value.

### Example Usage for Creating SiteData

Example of creating a `SiteData` object with all sites and values defined upfront:

```csharp
var siteNumbers = new int[] { 2, 4, 5, 6 };

// Creates a SiteData object with the same value for all sites.
var siteData = new SiteData<double>(siteNumbers, 567);
// Site  |  2    |  4    |  5    |  6
// Value | 567.0 | 567.0 | 567.0 | 567.0
```

Example of creating an empty `SiteData` object and populating it afterwards (applicable only to release 26.5.0 or later):

```csharp
var siteNumbers = new int[] { 2, 4, 5, 6 };

// Creates an empty SiteData object.
var siteData = new SiteData<double>();
// Site  | (empty)

// Adds all the sites. Each site is initialized with the default value of double (0.0).
siteData.AddSite(siteNumbers);
// Site  |  2    |  4    |  5    |  6
// Value |  0.0  |  0.0  |  0.0  |  0.0

// Sets the same value for all sites.
siteData.SetValue(567);
// Site  |  2    |  4    |  5    |  6
// Value | 567.0 | 567.0 | 567.0 | 567.0
```

Example of creating a `SiteData` object with a single site and extending it afterwards (applicable only to release 26.5.0 or later):

```csharp
// Creates a SiteData object with site 0. The site is initialized with the default value of double (0.0).
var siteData = new SiteData<double>(siteNumber: 0);
// Site  |  0
// Value |  0.0

// Adds sites 1 and 2. Site 0 already exists, so it is skipped.
siteData.AddSite(0, 1, 2);
// Site  |  0    |  1    |  2
// Value |  0.0  |  0.0  |  0.0
```

---

## Adding and Removing Sites from SiteData

The following table outlines the methods available for adding and removing sites from a `SiteData<T>` object.

>[!NOTE]
> Adding and removing sites is applicable only to release 26.5.0 or later.

**Table of Methods for Adding and Removing Sites:**

|Method|Description|
| :- | :- |
|[`AddSite(params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.AddSite.html)|Adds the specified sites. Each new site is initialized with the default value of `T`.|
|[`RemoveSite(params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.RemoveSite.html)|Removes the specified sites and their values.|

### Usage Considerations for Adding and Removing Sites

When adding and removing sites from `SiteData` objects you should consider the following:

1. You can pass the site numbers either as an array, for example `siteData.AddSite(siteNumbers)`, or as separate arguments, for example `siteData.AddSite(0, 1, 2)`.
2. New sites are initialized with the default value of `T`. For example, the default value is `0` for `int`, `0.0` for `double`, `false` for `bool`, and `null` for reference types, such as arrays and strings.
3. The `AddSite` method skips sites that already exist. The values of existing sites are not changed.
4. The `RemoveSite` method skips sites that do not exist.
5. An `ArgumentNullException` is thrown if the `siteNumbers` input is `null`. An `ArgumentException` is thrown if the `siteNumbers` input is an empty array or if no site number is provided.
6. Removing all sites leaves the `SiteData` object empty. Methods such as `GetValue` and the [math operations](../MathOperations/MathOperationsWithSiteData.md) throw an `NISemiconductorTestException` when called on an empty `SiteData` object.

### Example Usage for Adding and Removing Sites

Example of adding and removing sites (applicable only to release 26.5.0 or later):

```csharp
var siteData = new SiteData<double>();
// Site  | (empty)

// Adds sites 0, 1, and 2.
siteData.AddSite(0, 1, 2);
// Site  |  0    |  1    |  2
// Value |  0.0  |  0.0  |  0.0

// Sets a value for all sites, then adds sites 1 and 3.
// Site 1 already exists, so it is skipped and its value is not changed.
// Site 3 is initialized with the default value.
siteData.SetValue(2.5);
siteData.AddSite(1, 3);
// Site  |  0    |  1    |  2    |  3
// Value |  2.5  |  2.5  |  2.5  |  0.0

// Removes sites 0 and 1. Site 5 does not exist, so it is skipped.
siteData.RemoveSite(0, 1, 5);
// Site  |  2    |  3
// Value |  2.5  |  0.0
```

---

## Setting Data Values in SiteData

The following table outlines the methods available for setting values in a `SiteData<T>` object.

>[!NOTE]
> Setting data values is applicable only to release 26.5.0 or later.

**Table of Methods for Setting Data Values:**

|Method|Description|
| :- | :- |
|[`SetValue(T value)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.SetValue.html)|Sets the value for all existing sites.|
|[`SetValue(T value, params int[] siteNumbers)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.SetValue.html)|Sets the value for the specified sites. Sites that do not exist are added first.|

### Usage Considerations for Setting Data Values

When setting values in `SiteData` objects you should consider the following:

1. The `SetValue` method overwrites any existing value for the affected sites.
2. The `SetValue(T value)` method throws an `NISemiconductorTestException` if the `SiteData` object contains no sites.
3. The `SetValue(T value, params int[] siteNumbers)` method adds any of the specified sites that do not exist, and then sets their value. You do not need to call the `AddSite` method first.
4. If the `siteNumbers` input is `null` or an empty array, the `SetValue(T value, params int[] siteNumbers)` method sets the value for all existing sites. In this case, an `NISemiconductorTestException` is thrown if the `SiteData` object contains no sites.
5. When `T` is a reference type, such as an array, the `SetValue` method assigns the same instance to every affected site. Modifying that instance later changes the value for all of those sites. Assign a separate instance to each site if the values must be independent.

### Example Usage for Setting Data Values

Example of setting a value for all sites and for specific sites (applicable only to release 26.5.0 or later):

```csharp
var siteData = new SiteData<double>();
siteData.AddSite(0, 1, 2);
// Site  |  0    |  1    |  2
// Value |  0.0  |  0.0  |  0.0

// Sets the same value for all sites.
siteData.SetValue(3.3);
// Site  |  0    |  1    |  2
// Value |  3.3  |  3.3  |  3.3

// Sets a value for site 1 only. Sites 0 and 2 are not changed.
siteData.SetValue(value: 1.8, siteNumbers: 1);
// Site  |  0    |  1    |  2
// Value |  3.3  |  1.8  |  3.3

// Sets a value for sites 3 and 4. These sites do not exist, so they are added first.
siteData.SetValue(value: 5.0, siteNumbers: new int[] { 3, 4 });
// Site  |  0    |  1    |  2    |  3    |  4
// Value |  3.3  |  1.8  |  3.3  |  5.0  |  5.0
```

Example of setting a site-agnostic value, where site `-1` is added because it does not exist (applicable only to release 26.5.0 or later):

```csharp
var siteData = new SiteData<double>(new int[] { 0, 1 }, 11);
siteData.SetValue(value: 33, siteNumbers: -1);
// Site  | -1    |  0    |  1
// Value | 33.0  | 11.0  | 11.0
```

---

## Working with SiteData in Loops

You can use loops to build a `SiteData` object when the site numbers or values come from another data structure, or are computed at runtime. How you do this depends on the release you use:

- Prior to release 26.5.0, the `SiteData` class does not provide methods to add sites or set values. Collect the values in a .NET collection inside the loop, and then create the `SiteData` object after the loop.
- In release 26.5.0 or later, you can add sites and set values on a `SiteData` object inside the loop.

When working with `SiteData` in loops in any release you should consider the following:

1. The `SiteNumbers` property returns the site numbers in ascending order, which might not match the order of the data you are iterating over. When you index into a data array by site, iterate over the same `siteNumbers` array that is used to arrange the data.
2. To transform every value of an existing `SiteData` object into a new `SiteData` object, the `Select` method can be more concise than a loop.

### Working with SiteData in Loops Prior to Release 26.5.0

#### Usage Considerations Prior to Release 26.5.0

When working with `SiteData` in loops prior to release 26.5.0 you should consider the following:

1. If the site numbers are determined at runtime, such as based on a condition, collect the values in a `Dictionary<int, T>` inside the loop. Then, pass the dictionary to the `SiteData(IDictionary<int, T> data)` constructor after the loop.
2. If all the site numbers are known before the loop, collect the values in an array inside the loop. Then, pass the site numbers and the array to the `SiteData(int[] siteNumbers, T[] perSiteData)` constructor after the loop.

#### Example Usage Prior to Release 26.5.0

Example of calculating a unique value for each site inside the loop, and creating the `SiteData` object after the loop:

```csharp
var siteNumbers = new int[] { 2, 4, 5 };
var perSiteData = new double[siteNumbers.Length];
for (int i = 0; i < siteNumbers.Length; i++)
{
    // CalculateValue represents your own calculation.
    perSiteData[i] = CalculateValue(siteNumbers[i]);
}

// Creates the SiteData object after the loop.
var siteData = new SiteData<double>(siteNumbers, perSiteData);
```

Example of building a new `SiteData` object that contains only the sites where a measurement exceeds a limit:

```csharp
SiteData<double> measurement = dcPowerPin.MeasureVoltage().ExtractPin("VDD");
double upperLimit = 1.2;

var sitesAboveLimitData = new Dictionary<int, double>();
foreach (int siteNumber in measurement.SiteNumbers)
{
    double value = measurement.GetValue(siteNumber);
    if (value > upperLimit)
    {
        sitesAboveLimitData.Add(siteNumber, value);
    }
}

// Creates the SiteData object after the loop.
var sitesAboveLimit = new SiteData<double>(sitesAboveLimitData);
```

### Working with SiteData in Loops in Release 26.5.0 or Later

#### Usage Considerations for Release 26.5.0 or Later

When adding sites and setting values inside loops in release 26.5.0 or later you should consider the following:

1. If all the site numbers are known before the loop, add them in a single call to the `AddSite` method, or pass them to a constructor. Then, set only the values inside the loop.
2. Calling `SetValue(T value, params int[] siteNumbers)` inside a loop adds each site the first time it is encountered. You do not need to call `AddSite` separately.
3. The `SiteNumbers` property returns a new array each time it is accessed. Therefore, you can add or remove sites while iterating over that array. Store the array in a local variable before the loop instead of accessing the property in every iteration.

>[!CAUTION]
> `SiteData` objects are not thread-safe. Do not call the `AddSite`, `RemoveSite`, or `SetValue` methods on the same `SiteData` object from multiple threads at the same time, such as inside a `Parallel.For` or `Parallel.ForEach` loop, or in the actions passed to the `InvokeInParallel` method. This applies even when each thread accesses a different site. Concurrent modifications can corrupt the object or lose values without throwing an exception. Reading values from multiple threads is safe only when no thread modifies the object at the same time.
>
> Instead, store the result of each parallel iteration in its own element of a preallocated array. After the parallel loop completes, populate the `SiteData` object from the array, either by passing the array to a constructor or by calling the `SetValue` method in a sequential loop.

#### Example Usage for Release 26.5.0 or Later

Example of adding sites and setting a unique value for each site from a data array, where the array index represents the site number:

```csharp
var data = new double[] { 1, 2, 3 };

var siteData = new SiteData<double>();
// Adds all the sites in a single call, before the loop.
siteData.AddSite(0, 1, 2);
// Sets a unique value for each site.
for (int i = 0; i < data.Length; i++)
{
    siteData.SetValue(data[i], i);
}
// Site  |  0    |  1    |  2
// Value |  1.0  |  2.0  |  3.0
```

Example of building a `SiteData` object from a dictionary, where `SetValue` adds each site as it is encountered:

```csharp
var perSiteDataDictionary = new Dictionary<int, double> { [1] = 11, [2] = 22, [3] = 33 };

var siteData = new SiteData<double>();
foreach (var item in perSiteDataDictionary)
{
    // Site numbers are added automatically if they do not exist.
    siteData.SetValue(value: item.Value, siteNumbers: item.Key);
}
// Site  |  1    |  2    |  3
// Value | 11.0  | 22.0  | 33.0
```

Example of building a new `SiteData` object that contains only the sites where a measurement exceeds a limit:

```csharp
SiteData<double> measurement = dcPowerPin.MeasureVoltage().ExtractPin("VDD");
double upperLimit = 1.2;

var sitesAboveLimit = new SiteData<double>();
int[] siteNumbers = measurement.SiteNumbers;
foreach (int siteNumber in siteNumbers)
{
    double value = measurement.GetValue(siteNumber);
    if (value > upperLimit)
    {
        sitesAboveLimit.SetValue(value, siteNumber);
    }
}
```

Example of calculating values inside a `Parallel.For` loop, and creating the `SiteData` object after the parallel loop completes:

```csharp
var siteNumbers = new int[] { 2, 4, 5 };
var perSiteData = new double[siteNumbers.Length];

// Each iteration writes only to its own element of the perSiteData array.
// Do not call the SetValue method on a shared SiteData object inside the parallel loop.
Parallel.For(0, siteNumbers.Length, i =>
{
    // CalculateValue represents your own calculation.
    perSiteData[i] = CalculateValue(siteNumbers[i]);
});

// Creates the SiteData object after the parallel loop completes.
var siteData = new SiteData<double>(siteNumbers, perSiteData);
```
