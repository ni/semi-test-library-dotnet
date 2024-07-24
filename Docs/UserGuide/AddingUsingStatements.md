# Adding Using Directives

Depending on how your Visual Studio IDE is configured, the required using directives for accessing the Semiconductor Test Library may or may not automatically populate for as you write code. Therefore, you should always ensure that you have added the appropriate namespaces as using directives to the top of your code. This is required for the code to compile and for certain IDE features to work properly, such as Visual Studio's [IntelliSense](https://learn.microsoft.com/en-us/visualstudio/ide/using-intellisense?view=vs-2022).

Example 1: When working with the NI DCPower instruments, make sure you are using the following using directives in your code:

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

**Related information:**

- [Microsoft Learn: Using Directive](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive)
