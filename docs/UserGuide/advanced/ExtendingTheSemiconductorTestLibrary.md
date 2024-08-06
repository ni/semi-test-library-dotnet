# Extending the Semiconductor Test Library

The Semiconductor Test Library provides extension methods for abstracting common, high-level instrument operations.

Extension Methods are static methods that can be called as if they were instance methods for the object type they extend. For tests code being written, there's no apparent difference between calling an extension method and calling the methods defined in a type.

- Extension methods enable you to "add" methods to existing types without creating a new derived type, recompiling, or modifying the original type.
- An extension method cannot be called if it has the same signature as a method defined in the extended type. All name collisions are resolved in favor of the instance or static method defined by the type itself. Therefore, extension methods cannot be used to imitate existing methods of the extended type.
- Extension methods cannot access any private data in the extended class.
- Learn more at: [https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods).

## Writing Your Own Extension Method

NI recommends the following steps to get started writing your own extension methods:

1. Create a separate .cs file to write your project-specific extensions with its own unique namespace. This file must contain a static class, which should use the same name as the file name.
2. Write out a new static method, with the first parameter being the target class to extend, to be proceeded by the `this` keyword.
3. You can the add other parameters to the method signature and your desired logic within the method body.

Refer to the following example to see how the `PinSiteData` class can be extended to have a `MaxByPin()` method that calculates the per-site maximum value across each pin.

```C#
// FileName: MyProjectExtensions.cs
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.MyProject.Extensions
{
    /// <summary>
    /// Class containing custom extension methods for MyProject.
    /// </summary>
    public static class MyProjectExtensions
    {
        /// <summary>
        /// Calculates the per-site maximum value across each pin.
        /// </summary>
        /// <remarks>
        /// <example>
        /// Example usage:
        /// <code>
        /// var measurements = dcPins.MeasureVoltage();
        /// var maxValueAcrossPins = measurements.MaxByPin();
        /// </code>
        /// </example>
        /// </remarks>
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
