# Common Utility Methods

The Semiconductor Test Library provides utility methods commonly required for writing test code.

---

**`PreciseWait(double timeInSeconds)`**

Namespace: `NationalInstruments.SemiconductorTestLibrary.Common.Utilities`

Use this method to implement a software-timed wait to ensure a set amount of settling time before the next operation. It blocks the current thread and waits for the specified amount of time.

> [!NOTE]
> This method uses the [`Stopwatch`](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=netframework-4.8) class to support sub-millisecond settling times required by test code.
>
> NI does not recommend using the [`Thread.Sleep()`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread.sleep?view=netframework-4.8) method in test code due to its 10+ms resolution.

---
  
**`InvokeInParallel(params Action[] actions)`**

Namespace: `NationalInstruments.SemiconductorTestLibrary.Common.Utilities`

Use this method to implement Concurrent programming when separate lines of code should execute in parallel. Refer to the [Concurrent Code Execution](advanced/ConcurrentCodeExecution.md) topic for more details.
