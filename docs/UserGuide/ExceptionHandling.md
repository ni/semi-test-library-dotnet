# Exception Handling

## NISemiconductorTestException

The `NISemiconductorTestException` is the primary exception thrown by the Semiconductor Test Library. It acts as a wrapper for all types of exceptions that can occur during the operations, providing a consistent interface for error handling.

> [!NOTE]
> Class: `NISemiconductorTestException`\
> Namespace: `NationalInstruments.SemiconductorTestLibrary.Common` \
> Assembly: `NationalInstruments.SemiconductorTestLibrary.Abstractions.dll`
>

### Exception Wrapping Behavior

•	If the original exception is an `AggregateException`, the `NISemiconductorTestException` flattens it and uses the first inner exception.
•	The error code (*HResult*) is extracted from the original exception, or parsed from the exception message if available.
•	The original exception is set as the inner exception, preserving the full stack trace and context.

### Exception Details

- The exception message will include the message from the original exception.
- The InnerException property will contain the original exception object.
- The `HResult` property will be set to the error code, if available.
    - Example: If the error code is present in the message (e.g., "Error code: -200077"), it will be parsed and set on the exception.
This approach ensures that when you catch a `NISemiconductorTestException`, you have access to both a high-level error message and the full details of the underlying cause, making it easier to diagnose and handle errors in your test code.

### Exception Constructor Usage

The `NISemiconductorTestException` provides multiple constructors for different scenarios:

```csharp
// Basic exception with message
throw new NISemiconductorTestException("Operation failed");

// Exception with message and inner exception
throw new NISemiconductorTestException("Operation failed", innerException);

// Exception with error code and inner exception (used internally)
throw new NISemiconductorTestException(errorCode, innerException);
```