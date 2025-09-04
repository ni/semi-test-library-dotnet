# Best Practices for Writing Extension Methods in STL

Extension methods provide a framework for adding capabilities to the core abstractions provided by the Semiconductor Test Library. For example, you can add instrument-specific functionality for configuration, control, or measurement. Extension methods reduce the complexity of multi-site instrument programming and simplify writing high-level test code.
The extension methods act as a bridge between low-level instrument control and high-level test program development. In addition to the extension methods provided by the Semiconductor Test Library, you can also create new extension methods to meet your specific needs while avoiding constraints of inheritance and direct dependency.

This page provides instructions for writing extension methods that meet Semiconductor Test Library requirements.

## Type of Extension Methods

This section describes common scenarios encountered when writing extension methods to add instrument specific capabilities, such as configuration, control, and measurement. It also discusses how to implement extension methods effectively for each scenario.

### Configure Methods

These methods configure an instrument state or settings (such as voltage settings, configuring trigger parameters).
Complete the following steps to create a new Configure Method while adhering to best practices:

#### Method Definition

1. **Method Name**:
    - Use a clear and descriptive name that reflects the method's functionality.
    - Begin the method name with `Configure`, followed by a description of the specific setting or state being configured. For example: [ConfigureAOTerminalConfiguration](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/Configure.cs#L23C9-L23C142)
1. **Parameters**:
    - The first parameter should include the `this` keyword followed by the concrete type being extended.
        - For Instrument Abstraction extensions the concrete type is the one implementing `ISessionsBundle`. For example, `this DCPowerSessionsBundle` in [ConfigureSourceSettings](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L26C52-L26C66).
    - Provide the mandatory parameters required by the driver to execute the function successfully.
    - Choose the parameter type based on the requirement (such as Scalar, SiteData, or PinSiteData). Refer to the [key considerations](#choosing-parameter-and-return-types) for more detailed information on choosing appropriate parameters.
1. **Method Functionality**:
    - Perform any calculations if applicable and use the appropriate instrument commands to set/configure the values.
    - Handle any exceptions or errors gracefully, providing meaningful error messages if the operation fails. Refer to [Exception Handling and Validation](#exception-handling-and-validation) for more details.
1. **Return Type**:
    - The method should not return any value and should have a return type of `void`.

### Get Methods

These methods are used to retrieve properties, data or states from an instrument (such as read the current, get voltage). To create a new Get Method while adhering to best practices, follow these steps:

#### Method Definition

1. **Method Name**:
    - Use a clear and descriptive name that reflects the method's functionality.
    - *Prefix with* `Get` when the methods needs to return a value, representing a *property, configuration or setting*. For example: [GetSampleClockRate](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/Configure.cs#L129)
    - *Avoid* `Read` or `Fetch` for property accesses. These prefixes are reserved for high-level operations involving *instrument interaction* to return results. For example: [ReadStatic](https://github.com/ni/semi-test-library-dotnet/blob/00772a0797b0522e23f27b880b181913bb84326a/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/StaticState.cs#L60)
1. **Parameters and Return Type**:
    - The first parameter should include the `this` keyword followed by the concrete type being extended.
        - For Instrument Abstraction extensions the concrete type is the one implementing `ISessionsBundle`. For example, `this DCPowerSessionsBundle` in [ConfigureSourceSettings](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L26C52-L26C66).
    - Choose the rest of the parameter and the return type based on the requirement (such as Scalar, SiteData, or PinSiteData). Refer to the [key considerations](#choosing-parameter-and-return-types) for more detailed information on choosing appropriate parameters and return type.
1. **Method Functionality**:
    - Use appropriate instrument commands to retrieve the required values.
    - Handle any exceptions or errors gracefully, providing meaningful error messages if the operation fails. Refer to [Exception Handling and Validation](#exception-handling-and-validation) for more details.

### High-Level Functions

- These methods are intended to simplify complex tasks that typically require multiple driver calls or other operations by encapsulating them into a single method.

#### Method Definition

1. **Method Name**:
    - Name the method based on the specific functionality the higher-level method is intended to perform. For example, [ForceVoltage](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L79).
1. **Parameters**:
    - The first parameter should include the `this` keyword followed by the concrete type being extended.
        - For Instrument Abstraction extensions the concrete type is the one implementing `ISessionsBundle`. For example, `this DCPowerSessionsBundle` in [ConfigureSourceSettings](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L26C52-L26C66).
    - Accept parameters that are necessary to perform the high-level function.
1. **Method Functionality**:
    - Combine multiple low-level or extension calls to perform the desired high-level action.
1. **Return Type**:
    - The method should return either void or a value in an appropriate type (such as double, int, bool, or a custom data structure like `SiteData<T>` or `PinSiteData<T>`).

## Do And DoAndReturnXXXResults Methods

- To invoke a low-level driver API call, use the `Do` methods in the `ParallelExecution` class within the `NationalInstruments.SemiconductorTestLibrary.Common` namespace.
- These methods are used inside the extension methods to perform low-level driver operations on the session bundle and its associated sessions. These methods help in executing actions and retrieving results in different formats.
- These methods are STL-provided parallel execution utilities that make multi-site and -pin programming with low-level driver APIs easier.

### Do Methods

1. **`Do<TSessionInformation>(this ISessionsBundle<TSessionInformation> sessionsBundle, Action<TSessionInformation> action)`**
    - Use this method when you need to perform an operation on all the sessions in parallel with the same inputs.
    - Example: [WriteAnalogSingleSample](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/AnalogOutput.cs#L20)
2. **`Do<TSessionInformation>(this ISessionsBundle<TSessionInformation> sessionsBundle, Action<TSessionInformation, int> action)`**
    - Use this method to perform an operation on all sessions in parallel with session-specific inputs, where the session index is required.
    - Example: [ApplyTDROffsets](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/LevelsAndTiming.cs#L415)
3. **`Do<TSessionInformation>(this ISessionsBundle<TSessionInformation> sessionsBundle, Action<TSessionInformation, SitePinInfo> action)`**
    - Use this method to perform an operation on all sessions and channels in parallel, where pin, site, or channel information is required. Refer to the [`SitePinInfo` class documentation](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.Common.SitePinInfo.html#properties) for all accessible properties.
    - Example: [ConfigureAOFunctionGeneration](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/AnalogOutputFunctionGeneration.cs#L25)
4. **`Do<TSessionInformation>(this ISessionsBundle<TSessionInformation> sessionsBundle, Action<TSessionInformation, int, SitePinInfo> action)`**
    - Use this method to perform an operation on all sessions and channels in parallel, and both the session index and channel information are required.
    - Example: [AcquireSynchronizedWaveforms](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Measure.cs#L330)

### DoAndReturnXXXResults Methods

1. **`DoAndReturnPerInstrumentPerChannelResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, TResult> function)`**
    - Use this method to perform an operation on all sessions in parallel and return per-instrument per-channel results.
    - Example: [ReadSequencerFlag](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/SequencerFlagsAndRegisters.cs#L20)
2. **`DoAndReturnPerInstrumentPerChannelResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, SitePinInfo, TResult> function)`**
    - Use this method to perform an operation on all sessions and channels in parallel and return per-instrument per-channel results, where pin, site, or channel information is required. Refer to the [`SitePinInfo` class documentation](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.Common.SitePinInfo.html#properties) for all accessible properties.
    - Example: [StartAOFunctionGeneration](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/AnalogOutputFunctionGeneration.cs#L64)
3. **`DoAndReturnPerInstrumentPerChannelResults<TSessionInformation, TResult1, TResult2>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, Tuple<TResult1, TResult2>> function)`**
    - Use this method to perform an operation on all sessions in parallel and return two sets of per-instrument per-channel results.
    - Example: [MeasureAndReturnPerSitePerPinResults](https://github.com/ni/semi-test-library-dotnet/blob/4bd77d53dca3b21e4a2110d774cc5555aeadcc1b/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Measure.cs#L199)
4. **`DoAndReturnPerSitePerPinResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, TResult[]> function)`**
    - Use this method to perform an operation on all sessions in parallel and return per-site per-pin results.
    - Example: [GetSampleClockRate](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/Configure.cs#L131)
5. **`DoAndReturnPerSitePerPinResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, SitePinInfo, TResult> function)`**
    - Use this method to perform an operation on all sessions and channels in parallel and return per-site per-pin results, where pin, site, or channel information is required. Refer to the [`SitePinInfo` class documentation](https://ni.github.io/semi-test-library-dotnet/SemiconductorTestLibrary/NationalInstruments.SemiconductorTestLibrary.Common.SitePinInfo.html#properties) for all accessible properties.
    - Example: [GetApertureTimeInSeconds](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Measure.cs#L131)
6. **`DoAndReturnPerSitePerPinResults<TSessionInformation, TResult1, TResult2>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, Tuple<TResult1[], TResult2[]>> function)`**
    - Use this method to perform an operation on all sessions in parallel and return two sets of per-site per-pin results.
    - Example: [MeasureAndReturnPerSitePerPinResults](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Measure.cs#L199)
7. **`DoAndReturnPerSitePerPinResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, TResult[,]> function)`**
    - Use this method to perform an operation on all sessions in parallel and return per-site per-pin results from a two-dimensional array.
8. **`DoAndReturnPerSitePerPinResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, int, TResult[]> function)`**
    - Use this method to perform an operation on all sessions in parallel and return per-site per-pin results, where the session index is required.

For more information, refer to the [How to Make Low Level Driver API Calls](https://ni.github.io/semi-test-library-dotnet/UserGuide/advanced/MakingLowLevelDriverCalls.html#how-to-make-low-level-driver-api-calls).

## General Considerations for All Extension Method

### Reusability and Modularity

- When multiple extension methods contain repeating code, extract the reusable code into a separate method and refactor your methods to use it.

- :heavy_check_mark: **Do**
  - If all of your methods are part of the same .cs file, place the new method within that same .cs file and mark it as a `private` method.
  - If your methods are spread across multiple .cs files, add the new method to an `internal static class Utilities` class within the namespace for the instrument type being worked on, and mark it as an `internal` method. If no such Class exists, create it (for example [`ExcludeSpecificChannel`](https://github.com/ni/semi-test-library-dotnet/blob/12282644789e1f03018b6e3e024829d405ddad1d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Utilities.cs#L7)).
- :x: **Don't**
  - STL Contributors Only: Never refactor existing methods to use the new method. Only focus on the methods being added.

### Exception Handling and Validation

The `ExceptionCollector` class collects and manages exceptions that occur during driver operations, allowing them to be aggregated and thrown as a single `NISemiconductorTestException`. This approach helps ensure that all relevant errors are reported together, improving troubleshooting and error visibility.

Ensure proper exception handling is in place. This could include validating input parameters, checking communication status with the instrument, and providing useful error messages if the operation fails.

**Example**: The [`VerifyTaskType`](https://github.com/ni/semi-test-library-dotnet/blob/12282644789e1f03018b6e3e024829d405ddad1d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/Utilities.cs#L18) method is used in DAQmx Digital Input and Analog Input functions to validate the task type and throw error accordingly.

When invoking `Do` and `DoAndReturnXXXResults` methods, provide the `caseDescription` parameter to add meaningful context to low-level error messages, making it easier for users to understand and troubleshoot issues.

#### How to name a `caseDescription`

- Be descriptive and concise.
  - Use a short phrase that clearly describes the operation (such as ***PPMU Force Voltage***).
- Use Title case.
  - Capitalize the first letter for better readability.
- Match the method purpose.
  - The description should match the intent of the extension method or the operation.
- Avoid using generic or excessively long names (keep character count low).

**Example**:

```cs
digitalSessionsBundle.Do((sessionInfo, sitePinInfo) =>
{
    var settings = new PPMUSettings
    {
        OutputFunction = PpmuOutputFunction.DCVoltage,
        VoltageLevel = voltageLevels[sitePinInfo.PinName],
        CurrentLimitRange = currentLimitRange,
        ApertureTime = apertureTime,
        SettlingTime = settlingTime
    };
    sessionInfo.Session.Force(sitePinInfo.SitePinString, settings);
}, "PPMU Force Voltage");  // <-- Case Description added as an optional parameter to the Do method.
```

### Determining the Scope of the Method: Channel, Model and Session

|| **Channel-based Methods** | **Session-based Methods** | **Module-based Methods** |
|----------|----------|----------|----------|
| **Functionality** | If the functionality is specific to individual channels, use the appropriate [`Do` or `DoAndReturnXXXResults` methods](#do-and-doandreturnxxxresults-methods) to iterate over each channel.| If the functionality is specific to the entire session, ensure that driver operations are only performed once per-session, and any returned values are returned consistently across across all channels using the appropriate [`Do` or `DoAndReturnXXXResults` methods](#do-and-doandreturnxxxresults-methods).  | If the functionality is applicable to an entire module, such as a hardware limited instrument capability, ensure that driver operations are performed only once per-session and choose the return type carefully using the appropriate [`Do` or `DoAndReturnXXXResults` methods](#do-and-doandreturnxxxresults-methods).|
|  **Documentation** | When appropriate, provide in-line code and method summary documentation to denote the channel-specific operation. | When appropriate, provide in-line code and method summary documentation to denote the session-specific operation. | When appropriate, provide in-line code and method summary documentation to denote the module-specific operation. |
|  **Example** | [GetTimeSetEdge](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/LevelsAndTiming.cs#L315) | [GetTimeSetPeriod](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/LevelsAndTiming.cs#L254)| [ReadSequencerFlag](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/SequencerFlagsAndRegisters.cs#L20) |

### Choosing Parameter and Return Types

The decision to use Scalar, SiteData, or PinSiteData depends on the level of requirement. Below are the guidelines for when to use each type:

#### Scalar Input/Return Type

***Use Case***: When the same value applies uniformly across all channels or pins in the session.

**Example**:

- ***Input Parameter***: `DCPowerSourceSettings settings` Represents a single value (such as voltage level) applied to all sessions.
  - [public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, DCPowerSourceSettings settings)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L26)

- ***Return Type***:`List<TResult> or TResult[]` Each value indicates the result of the specified function for an individual session.
  - [public static bool[] ReadSequencerFlag(this DigitalSessionsBundle sessionsBundle, string flag)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/SequencerFlagsAndRegisters.cs#L20)

#### SiteData Input/Return Type

***Use Case***: When each site requires a unique value, but the value is consistent across all pins within a site.

**Example**:

- ***Input Parameter***: `SiteData` object represents the value for each site.
  - [public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, SiteData\<DCPowerSourceSettings> settings)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L36)
- ***Return Type***: `SiteData<T>` object containing measurements for each site.
  - [public static SiteData\<bool> GetSitePassFail(this DigitalSessionsBundle sessionsBundle)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/Pattern.cs#L95)

#### PinSiteData Input/Return Type

***Use Case***: When each pin and site combination requires a unique value.

**Example**:

- ***Input Parameter***: `PinSiteData` object that specifies the value for each pin-site combination.
  - [public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, PinSiteData\<DCPowerSourceSettings> settings)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L46)
- ***Return Type***: `PinSiteData<T>` object that contains measurements for each pin-site combination.
  - [public static PinSiteData\<double> GetSourceDelayInSeconds(this DCPowerSessionsBundle sessionsBundle)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L803)

#### PerInstrumentPerChannel - Return type

***Use Case***

- Use this format when you need to retrieve measurements directly from the instrument channels without considering the site and pin context.
- This approach is useful when needing to work with raw instrument data, where mapping measurements to specific sites or pins is either not applicable or unnecessary.
  - Example: [MeasureAndReturnPerInstrumentPerChannelResults(this DCPowerSessionsBundle sessionsBundle)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Measure.cs#L187)

## Instrument Specific Guidelines

Some instruments may require additional considerations or have unique limitations. For instance:

### DCPower

- Closely pay attention to cases where different model sessions are in a single sessions bundle because different models may have varying capabilities, limitations, and configuration requirements.
- Ensure that your code handles different model sessions in a single sessions bundle effectively.
  - For Power Supplies specifically, you may need to handle voltage and current settings separately and always verify that both current and voltage are within their maximum and minimum ranges based on the instrument model. This is mainly to avoid unhandled exceptions.

### Digital

- In Digital, it is crucial to ensure the site details before proceeding with the development because site-specific operations can significantly impact the accuracy and reliability of the results.

### DAQmx

- Configure DAQmx sessions with homogeneous pin types. Avoid mixing analog and digital pin configurations within a single session. For example, when initializing an Analog Input (AI) session, include only pins designated for AI.
- Avoid combining digital extension methods and analog methods. Maintain clear separation between analog and digital operations.
- While accessing the read methods, note that the readings change smoothly over time, not instantly. For example, if an Analog Output (AO) is set to write 5 V, the corresponding Analog Input (AI) read shows the voltage ramping towards 5 V, rather than an immediate 5 V reading.

## Writing Tests for Extension Methods

When developing a new extension method it must be tested to validate functionality. This is especially important when developing extension method intended to be contributed to the [semi-test-library-dotnet](https://github.com/ni/semi-test-library-dotnet) project. Each extension should have associated unit tests to ensure it works as expected. These tests should be capable of testing different scenarios, such as:

- **Valid inputs**: Ensuring the method behaves as expected with valid parameters.
- **Invalid inputs**: Handling incorrect inputs.
- **Boundary conditions**: Testing edge cases, such as setting maximum or minimum values.
- **Exception handling**: Verifying that exceptions are properly thrown and handled when needed.

> [!NOTE]
> Unit tests should be written using [xUnit](https://xunit.net). Refer to the various test projects within the [semi-test-library-dotnet](https://github.com/ni/semi-test-library-dotnet) repository as a reference.
>
> Contributors should write tests using simulated hardware that can be run in offline mode only. If a test requires actual hardware to run, please reach out to NI for further discussion before proceeding.
>
> If support is needed, contributors are encouraged to [open a new issue](https://github.com/ni/semi-test-library-dotnet/issues/new?template=support_request.md) using the *Contributor Support* template.

### General Practices for Writing Tests

When writing unit tests, it's important to maintain clarity, readability, and consistency. Below are essential guidelines:

#### Test Naming Conventions

- Use clear, descriptive names for test methods to convey the intent of the test.
- Follow the naming convention: `PreparedStateThatIsRelevant_MethodToTestWithScenarioDescription_ExpectedBehavior` for easy understanding.

**Example:**

```csharp
  public void InitializeDAQmxTasks_ReadAnalogSamplesFromOneFilteredChannel_ResultsContainExpectedData()
```

#### Test Structure

Make sure that the tests are independent. Avoid creating dependencies between tests to ensure reliability in unit testing. Always use the Arrange-Act-Assert (AAA) Pattern and structure your tests in three clear phases:

- **Arrange**: Set up the required data or objects.
- **Act**: Execute the method or code under test.
- **Assert**: Verify the expected outcome using clear assertions. Ensure that assertions are specific, readable, and not overly generic.

**Example:**

```csharp
  public void SessionsInitialized_BurstPatternWithoutSpecifyingPins_Succeeds()
  {
      #Arrange
      var sessionManager = InitializeSessionsAndCreateSessionManager("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
      var sessionsBundle = sessionManager.Digital();
      
      #Act
      sessionsBundle.BurstPattern("TX_RF");
      sessionsBundle.WaitUntilDone();
      var results = sessionsBundle.GetSitePassFail();
      var failCountResults = sessionsBundle.GetFailCount();
  
      #Arrange
      Assert.Equal(2, results.SiteNumbers.Length);
      Assert.Equal(2, failCountResults.SiteNumbers.Length);
      Assert.Equal(5, failCountResults.ExtractSite(0).Count);
  }
```

#### Using Attributes

For tests requiring different input data, use required attribute such as **Theory**, **Fact** accordingly to define the test scenarios:

- **Theory**: This attribute allows running the same test with different sets of input data.
- **Fact**: This attribute is used for tests that run with a single, predefined set of data.

**Example:**
  
1. Using Theory:

```csharp
[Theory]
[InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
[InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
public void SessionsInitialized_WaitUntilDoneSucceeds(string pinMap, string digitalProject)
 ```

1. Using Fact:

``` csharp
[Fact]
public void SessionsInitialized_BurstPatternWithoutSpecifyingPins_Succeeds()
```
