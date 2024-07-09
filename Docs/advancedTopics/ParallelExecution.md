# Parallelization Methods

The Semiconductor Test Library provides extension methods for abstracting parallel for loops that are required to iterate of each of the various instrument sessions within a given pin sessions bundle.

- Note that the Parallelization methods are simply extension methods for objects that are inherited from the ISessionsBundle interface.
- These methods can be passed to any driver operation(s) to be performed across each instrument session associated with the pin(s) queried.
- There are overloads to allow the user to target an operation to be performed across each session or across each pin and site.
- These methods should only be used when needing to write low-level driver calls to implement instrument capabilities not yet exposed by the Semiconductor Test Library.
Namespace: Name NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution.
