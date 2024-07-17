# Extending The Semiconductor Test Library

The Semiconductor Test Library provides extension methods for abstracting common, high-level instrument operations.

If you are unfamiliar Extension Methods, Extension methods are static methods that are called as if they were instance methods on the extended type. For client code written in C#, there's no apparent difference between calling an extension method and calling the methods defined in a type.

- Extension methods enable the developer to "add" methods to existing types without creating a new derived type, recompiling, or otherwise modifying the original type.
- An extension method will never be called if it has the same signature as a method defined in the type. So extension method can never be used to impersonate existing methods on a type, because all name collisions are resolved in favor of the instance or static method defined by the type itself.
- Extension methods cannot access any private data in the extended class.
- Learn more at: [https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods).

## Writing Your Own Extension Method

It is recommended that you start by creating a separate .cs file to write your project-specific extensions in, with it's own unique namespace. It must contain a static class, which should use the same name as the file name. Once you have this setup, you then then write out a new static method, with the first parameter being the target class you wish to extend proceeded by the `this` keyword. You can then add any other parameters and your desired logic within the method. Refer to the example below, which extends `PinSiteData` to have a `MaxByPin()` method that calculates the per-site maximum value across each pin.

```C#
// FileName: MyProjectExtensions.cs
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace MyProject.Extensions
{
    /// <summary>
    /// Class containing custom extension methods for MyProject.
    /// </summary>
    public static class MyProjectExtensions
    {
        /// <summary>
        /// Calculates the per-site maximum value across each pin.
        /// </summary>
        /// <example>
        /// Example usage:
        /// <code>
        /// var measurements = dcPins.MeasureVoltage();
        /// var measurementMaxAcrossPins = measurements.MaxByPin();
        /// </code>
        /// </example>
        /// <typeparam name="T">The base type for the per-site per-pin data</typeparam>
        /// <param name="data">The <see cref="PinSiteData{T}"/> object</param>
        /// <returns>
        /// Returns a new <see cref="SiteData{T}"/> object,
        /// containing the per-site maximum values across all pins.
        /// </returns>
        public static SiteData<T> MaxByPin<T>(this PinSiteData<T> data)
        {
            var perSiteMax = new Dictionary<int, T>();
            for (int siteIndex = 0; siteIndex < data.SiteNumbers.Length; siteIndex++)
            {
                var perPinSingleSiteValues = new T[data.PinNames.Length];
                for (int pinIndex = 0; pinIndex < data.PinNames.Length; pinIndex++)
                {
                    perPinSingleSiteValues[pinIndex] =
                    data.GetValue(data.SiteNumbers[siteIndex], data.PinNames[pinIndex]);
                }
                perSiteMax.Add(data.SiteNumbers[siteIndex], perPinSingleSiteValues.Max());
            }
            return new SiteData<T>(perSiteMax);
        }
    }
}
```
