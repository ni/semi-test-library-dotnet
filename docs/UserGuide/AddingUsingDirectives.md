# Adding Using Directives

Depending on your Visual Studio IDE configuration, the required using directives for accessing the Semiconductor Test Library may or may not automatically populate when you write code. Therefore, you should always add the appropriate namespaces as the using directives at the beginning of your code. This is required for the code to compile and for certain IDE features to work properly, such as Visual Studio's IntelliSense.

Example 1: When working with the NI-DCPower instruments, make sure you are using the following using directives in your code:

```C#
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
```

Example 2: When working with the `PinSiteData` or `SiteData` objects, make sure you are using the following using directives in your code:

```C#
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
```

> [!TIP]
> Consider a static using directive when working methods in the `Utilities` class, such as `PreciseWait`. This will allow you to write `PreciseWait()` in your code, instead of `Utilities.PreciseWait()`.
>
> ```C#
> using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
>```

**Related Information:**

- [Microsoft Learn: using directive](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive)
- [Microsoft Learn: IntelliSense in Visual Studio](https://learn.microsoft.com/en-us/visualstudio/ide/using-intellisense?view=vs-2022)
