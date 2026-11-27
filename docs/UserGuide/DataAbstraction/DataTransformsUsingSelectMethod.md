# Data Transforms Using Select Method

The `Select` method performs an operation on each value of a `SiteData` or `PinSiteData` object, and returns a new object that contains the results. The returned object contains the same sites, and for `PinSiteData` objects, the same pins as the original object. The operation can return a different data type, which makes the `Select` method useful for converting data from one type to another.

The following table outlines the `Select` methods.

**Table of Select Methods:**

|Method|Description|
| :- | :- |
|[`SiteData<T>.Select<TResult>(Func<T, TResult> operation)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.SiteData-1.Select.html)|Performs the operation on the value of each site, and returns a new `SiteData<TResult>` object.|
|[`PinSiteData<T>.Select<TResult>(Func<T, TResult> operation)`](../../SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.DataAbstraction.PinSiteData-1.Select.html)|Performs the operation on the value of each pin and site, and returns a new `PinSiteData<TResult>` object.|

## Usage Considerations for the Select Method

When using the `Select` method, you should consider the following:

1. The `Select` method does not modify the original object.
2. The operation can be a lambda expression, a method, or a local function. In each case, the operation must accept a value of type `T` and return a value of type `TResult`. `TResult` can be any data type.
3. The underlying data type of the original object, `T`, must be one of the following types, or an array type:
    - `double`
    - `decimal`
    - `float`
    - `int`
    - `uint`
    - `long`
    - `ulong`
    - `byte`
    - `sbyte`
    - `short`
    - `ushort`

    Otherwise, an `NISemiconductorTestException` is thrown with the message `Math operations not supported on the {type} type data.` For example, the `Select` method cannot be called on a `SiteData<bool>` or `SiteData<string>` object. For these types, iterate over the sites and create a new object from the results instead. For more information, refer to [Working with SiteData in Loops](SiteData.md#working-with-sitedata-in-loops).
4. The `Select` method of `SiteData` throws an `NISemiconductorTestException` if the `SiteData` object is empty.
5. The `Select` method of `PinSiteData` throws an `NISemiconductorTestException` if the `PinSiteData` object contains no pins, or if any pin contains no sites. Jagged `PinSiteData` objects are supported, and the returned object has the same jagged structure.
6. If the operation throws an exception for any value, the `Select` method stops and throws an `NISemiconductorTestException`. The exception message includes the site, and for `PinSiteData` objects, the pin where the exception occurred, along with the original exception details.
7. For basic arithmetic, the [math operations](../MathOperations/MathOperationsWithSiteData.md) such as `Add` and `Multiply` are more concise. Use the `Select` method for custom operations and type conversions.
8. The `Select` method was introduced in the 24.5.1 release.

## Example Usage for the Select Method

Example of converting the data type:

```csharp
var intData = new SiteData<int>(new int[] { 1, 2 });
// Casts each site value to a double.
SiteData<double> doubleData = intData.Select(value => (double)value);
// Site  |  0    |  1
// Value |  1.0  |  2.0
```

Example of reducing array data to a single value per pin and site:

```csharp
PinSiteData<double[]> multiPointMeasurement = dmmPins.ReadMultiPoint(sampleCount, maximumTimeInMilliseconds: 5000);
// Selects the maximum value of the samples for each pin and site.
PinSiteData<double> maxValue = multiPointMeasurement.Select(samples => samples.Max());
tsmContext.PublishResults(maxValue, publishedDataId: "MaxMultiPointValue");
```

Example of aggregating per-sample comparison results into a single pass/fail result per site:

```csharp
SiteData<bool[]> comparisonResults = measurement.Compare<uint[], bool[]>(ComparisonType.EqualTo, expectedData);
// Returns true for a site only if all the samples of that site match.
SiteData<bool> aggregateResult = comparisonResults.Select(result => result.All(value => value));
tsmContext.PublishResults(aggregateResult, publishedDataId: "ComparisonResult");
```

Example of using a local function to transform array data into a string:

```csharp
var captureData = new SiteData<uint[]>(new uint[][]
{
    new uint[] { 192, 3 }, // Site 0 data
    new uint[] { 32, 128 } // Site 1 data
});
SiteData<string> hexData = captureData.Select(SumSamplesAndRepresentAsHexString);
// Site  |  0    |  1
// Value | "C3"  | "A0"

string SumSamplesAndRepresentAsHexString(uint[] samples)
{
    var sum = samples.Sum(sample => sample);
    return $"{sum:X}";
}
```

Example of a `Select` operation :x: **NOT ALLOWED**:

```csharp
SiteData<bool> passFail = sdo.GetSitePassFail();

var passFailAsInt = passFail.Select(value => value ? 1 : 0);
// The above operation will throw an NISemiconductorTestException with the following message:
// "Math operations not supported on the System.Boolean type data."
```

>[!NOTE]
> The following example uses the parameterless `SiteData` constructor and the `SetValue` method. Both are only availabe in release 26.5.0 or later.

The same transform can be performed by iterating over the sites instead:

```csharp
SiteData<bool> passFail = sdo.GetSitePassFail();

var passFailAsInt = new SiteData<int>();
int[] siteNumbers = passFail.SiteNumbers;
foreach (int siteNumber in siteNumbers)
{
    passFailAsInt.SetValue(passFail.GetValue(siteNumber) ? 1 : 0, siteNumber);
}
```
