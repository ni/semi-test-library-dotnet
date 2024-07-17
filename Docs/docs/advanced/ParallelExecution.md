# Parallelization Methods

The Semiconductor Test Library provides extension methods for abstracting parallel for loops that are required to iterate of each of the various instrument sessions within a given pin sessions bundle.

Applicable namespace: `NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution`

- Note that the Parallelization methods can be used with any class that inherits from the `ISessionsBundle` interface, such as the `DCPowerSessionsBundle` class.
- These methods can be used to perform one or more driver operation(s) across each instrument session associated with the pin(s) queried.
- There are overloads to allow you to specify if an operation is to be performed across each session or across each pin and site.
- These methods should only be used when needing to write low-level driver calls to implement instrument capabilities not yet exposed by the Semiconductor Test Library.

Related concepts:

- [Making Low-level Driver Calls (Advanced)](LowLevelDriverCalls.md)
