# Parallelization Methods

The Semiconductor Test Library provides extension methods for abstracting parallel for loops that are required to iterate over each of the various instrument sessions within a given pin sessions bundle.

- Note that you can use the Parallelization methods with any class that inherits from the `ISessionsBundle` interface, such as the `DCPowerSessionsBundle` class.
- You can use these methods to perform one or more driver operations across each instrument session associated with the pin(s) queried.
- Overloads allow you to specify whether an operation is to be performed across each session or across each pin and site.
- You should use these methods only when writing low-level driver calls to implement instrument capabilities not yet exposed by the Semiconductor Test Library.

> [!NOTE]
> Class: `ParallelExecution`\
> Namespace: `NationalInstruments.SemiconductorTestLibrary.Common` \
> Assembly: `NationalInstruments.SemiconductorTestLibrary.Abstractions.dll`

**Related Concepts:**

- [Making Low-level Driver Calls](MakingLowLevelDriverCalls.md)
- [NI TSM: Parallel For Loops](https://www.ni.com/docs/en-US/bundle/teststand-semiconductor-module/page/parallel-for-loops.html)
