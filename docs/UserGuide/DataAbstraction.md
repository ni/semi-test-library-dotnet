# Data Abstraction

The Semiconductor Test Library simplifies how the test developer interacts with configuring instrumentation and managing returned measurement results by providing Pin- and Site- Aware Data Types. the library’s high-level extension methods both return and accept these types as input parameters. This abstracts the need for test developers to manage confusing array manipulations or translate between instrument and channel formatted to pin and site formatted data. Additionally, basic math functions are also supported to be operated on these types.

## Pin- and Site-Aware Data Types

**`SiteData<T>`** \
Namespace: `NationalInstruments.SemiconductorTestLibrary.DataAbstraction`

Defines an object containing values for one or more sites, where `T` can be passed as any data type.

This type is returned from methods in the Semiconductor Test Library where there may be a unique per-site value. This type can also be passed as an input parameter to some Semiconductor Test Library methods when there is a unique per-site value to operate with.

The `SiteData` object exposes basic methods for extracting a single site value or subset of values. Additionally, frequently used mathematical operations can be performed on a `SiteData` object for the most commonly used data types.

>[!NOTE]
> Refer to the API Reference for more details regarding the properties and methods exposed by the `SiteData`.
>
> Examples for using the `SiteData` can be found [here on GitHub](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/DataAbstraction).

---

**`PinSiteData<T>`** \
Namespace: `NationalInstruments.SemiconductorTestLibrary.DataAbstraction`

Defines an object containing values for one or more sites that is associated with a particular pin or set of pins, where `T` can be passed as any data type.

This type is returned from methods in the Semiconductor Test Library, such as measurement methods, where there may be a unique value for each pin, regardless of if the value is the same across all sites and/or pins. This type can also be passed as an input parameter to some Semiconductor Test Library methods when there is a unique per-site, per-pin value to operate with.

The `PinSiteData` object exposes basic methods for extracting a single site value or subset of values. Additionally, frequently used mathematical operations can be performed on a `PinSiteData` object for the most commonly used data types.

>[!NOTE]
> Refer to the API Reference for more details regarding the properties and methods exposed by the `PinSiteData`.
>
> Examples for using the `PinSiteData` can be found [here on GitHub](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/DataAbstraction).
