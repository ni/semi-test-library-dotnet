# **Best Practices for Writing Extension Methods in STL**

Extension methods collectively provide a comprehensive framework for additional capabilities built on top of the core abstractions provided by the Semiconductor Test Library, such as adding instrument-specific functionality for configuring, controlling, and measuring. They simplify the complexity of instrument communication, offering a smooth interface that makes it easier to perform tests.
The extension methods act as a bridge between low-level instrument control and high-level test program development. Extension methods allow the Semiconductor Test Library to include predefined methods for supporting various instrument types and capabilities while also enabling users to create their own extension methods for their specific needs, without the constraints of inheritance or direct dependency.

This page provides guidance on writing an extension method in alignment with the established practices of the Semiconductor Test Library, to ensure it meets the required standards.

## **Type of Extension Methods**

This section explains the different scenarios that are likely to be encountered when writing extension methods to add instrument specific capabilities, such as configuration, control, and measurement. It also discusses when and how to implement an effective extension method for each scenario.

### Configure Methods

These methods configure an instrument’s state or settings (e.g., voltage settings, configuring trigger parameters). To create a new Configure Method while adhering to best practices, follow these steps:

#### **Method Definition:**

1. **Method Name**:
    - Use a clear and descriptive name that reflects the method's functionality.
    - Begin the method name with `Configure`, followed by a description of the specific setting or state being configured. Eg: [ConfigureAOTerminalConfiguration](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/Configure.cs#L23C9-L23C142)
1. **Parameters**:
    - The first parameter should include the `this` keyword followed by the concrete type being extended.
        - For Instrument Abstraction extensions the concrete type will be the one implementing `ISessionsBundle`. For example, [DCPowerSessionsBundle](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L26C52-L26C93)
    - The user should provide the mandatory parameters required by the driver to execute the function successfully.
    - Choose the parameter type based on the requirement (e.g., Scalar, SiteData, or PinSiteData). Refer to the [key considerations](#choosing-parameter-and-return-types) for detailed information.
1. **Method Functionality**:
    - Perform any calculations if applicable and use the appropriate instrument commands to set/configure the values.
    - Handle any exceptions or errors gracefully, providing meaningful error messages if the operation fails.
1. **Return Type**:
    - The method should not return any value and should have a return type of `void`.

### Get Methods

These methods are used to retrieve properties, data or states from an instrument (e.g., read the current, get voltage). To create a new Get Method while adhering to best practices, follow these steps:

#### **Method Definition:**

1. **Method Name**:
    - Use a clear and descriptive name that reflects the method's functionality.
    - *Prefix with* `Get` when the methods needs to return a value, representing a *property, configuration or setting*. Eg: [GetSampleClockRate](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/Configure.cs#L129)
    - *Avoid* `Read` or `Fetch` for property accesses. These prefixes are are reserved for high-level operations involving *instrument interaction* to return results. Eg: [ReadStatic](https://github.com/ni/semi-test-library-dotnet/blob/00772a0797b0522e23f27b880b181913bb84326a/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/StaticState.cs#L60)
1. **Parameters and Return Type**:
    - The first parameter should include the `this` keyword followed by the concrete type being extended.
        - For Instrument Abstraction extensions the concrete type will be the one implementing `ISessionsBundle`. For example, [DCPowerSessionsBundle](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L26C52-L26C93)
    - Choose the rest of the parameter and the return type based on the requirement (e.g., Scalar, SiteData, or PinSiteData). Refer to the [key considerations](#choosing-parameter-and-return-types) for detailed information on choosing parameter.
1. **Method Functionality**:
    - Use appropriate instrument commands to retrieve the required values.
    - Handle any exceptions or errors gracefully, providing meaningful error messages if the operation fails.

### High-Level Functions

- These methods are intended to simplify complex tasks that typically require multiple driver calls or other operations by encapsulating them into a single method.

#### **Method Definition:**

1. **Method Name**:
    - Name the method based on the specific functionality the higher-level method is intended to perform. Eg, [PowerDown](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L579).
1. **Parameters**:
    - The first parameter should include the `this` keyword followed by the concrete type being extended.
        - For Instrument Abstraction extensions the concrete type will be the one implementing `ISessionsBundle`. For example, [DCPowerSessionsBundle](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L26C52-L26C93).
    - Accept parameters that are necessary to perform the high-level function.
1. **Method Functionality**:
    - Combine multiple low-level or extension calls to perform the desired high-level action.
1. **Return Type**:
    - The method should return either void or a value in an appropriate type (e.g., double, int, bool, or a custom data structure like `SiteData<T>` or `PinSiteData<T>`).

## **Do And DoAndReturnXXXResults Methods**

- To invoke a low-level driver API call, use the Do methods in the ParallelExecution class within the NationalInstruments.SemiconductorTestLibrary.Common namespace.
- These methods are used inside the extension methods to perform low-level driver operations on the session bundle and its associated sessions. These methods help in executing actions and retrieving results in different formats.
- These methods are STL-provided parallel execution utilities that makes the creation of extension methods simpler.

### **Do Methods**

1. **`Do<TSessionInformation>(this ISessionsBundle<TSessionInformation> sessionsBundle, Action<TSessionInformation> action)`**
    - Use this method when you need to perform an operation on all the sessions in parallel with the same inputs.
    - Example: [WriteAnalogSingleSample](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/AnalogOutput.cs#L20)
2. **`Do<TSessionInformation>(this ISessionsBundle<TSessionInformation> sessionsBundle, Action<TSessionInformation, int> action)`**
    - Use this method to perform an operation on all sessions in parallel with session-specific inputs, where the session index is required.
    - Example: [ApplyTDROffsets](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/LevelsAndTiming.cs#L415)
3. **`Do<TSessionInformation>(this ISessionsBundle<TSessionInformation> sessionsBundle, Action<TSessionInformation, SitePinInfo> action)`**
    - Use this method to perform an operation on all sessions and channels in parallel.
    - Example: [ConfigureAOFunctionGeneration](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/AnalogOutputFunctionGeneration.cs#L25)
4. **`Do<TSessionInformation>(this ISessionsBundle<TSessionInformation> sessionsBundle, Action<TSessionInformation, int, SitePinInfo> action)`**
    - Use this method to perform an operation on all sessions and channels in parallel, and both the session index and channel information are required.
    - Example: [AcquireSynchronizedWaveforms](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Measure.cs#L330)

### **DoAndReturnXXXResults Methods**

1. **`DoAndReturnPerInstrumentPerChannelResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, TResult> function)`**
    - Use this method to perform an operation on all sessions in parallel and return per-instrument per-channel results.
    - Example: [ReadSequencerFlag](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/SequencerFlagsAndRegisters.cs#L20)
2. **`DoAndReturnPerInstrumentPerChannelResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, SitePinInfo, TResult> function)`**
    - Use this method to perform an operation on all sessions and channels in parallel and return per-instrument per-channel results.
    - Example: [StartAOFunctionGeneration](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/AnalogOutputFunctionGeneration.cs#L64)
3. **`DoAndReturnPerInstrumentPerChannelResults<TSessionInformation, TResult1, TResult2>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, Tuple<TResult1, TResult2>> function)`**
    - Use this method to perform an operation on all sessions in parallel and return two sets of per-instrument per-channel results.
    - Example: [MeasureAndReturnPerInstrumentPerChannelResults](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Measure.cs#L189)
4. **`DoAndReturnPerSitePerPinResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, TResult[]> function)`**
    - Use this method to perform an operation on all sessions in parallel and return per-site per-pin results.
    - Example: [GetSampleClockRate](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/Configure.cs#L131)
5. **`DoAndReturnPerSitePerPinResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, SitePinInfo, TResult> function)`**
    - Use this method to perform an operation on all sessions and channels in parallel and return per-site per-pin results.
    - Example: [GetApertureTimeInSeconds](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Measure.cs#L131)
6. **`DoAndReturnPerSitePerPinResults<TSessionInformation, TResult1, TResult2>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, Tuple<TResult1[], TResult2[]>> function)`**
    - Use this method to perform an operation on all sessions in parallel and return two sets of per-site per-pin results.
    - Example: [MeasureAndReturnPerSitePerPinResults](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Measure.cs#L199)
7. **`DoAndReturnPerSitePerPinResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, TResult[,]> function)`**
    - Use this method to perform an operation on all sessions in parallel and return per-site per-pin results from a two-dimensional array.
8. **`DoAndReturnPerSitePerPinResults<TSessionInformation, TResult>(this ISessionsBundle<TSessionInformation> sessionsBundle, Func<TSessionInformation, int, TResult[]> function)`**
    - Use this method to perform an operation on all sessions in parallel and return per-site per-pin results, where the session index is required.

Navigate to this [page](https://ni.github.io/semi-test-library-dotnet/UserGuide/advanced/MakingLowLevelDriverCalls.html#how-to-make-low-level-driver-api-calls) for further details.

## **Writing Tests for Extension Methods**

Each extension should have associated unit tests to ensure it works as expected. These tests should be capable of testing different scenarios, such as:

- **Valid inputs**: Ensuring the method behaves as expected with valid parameters.
- **Invalid inputs**: Handling incorrect inputs.
- **Boundary conditions**: Testing edge cases, such as setting maximum or minimum values.
- **Exception handling**: Verifying that exceptions are properly thrown and handled when needed.

> [!NOTE]
> Contributors should write tests using simulated hardware that can be run in offline mode only. If a test requires actual hardware to run, please reach out to NI for further discussion before proceeding.
>
> If support is needed, contributors are encouraged to [open a new issue](https://github.com/ni/semi-test-library-dotnet/issues/new?template=support_request.md) using the *Contributor Support* template.

### **General Practices for Writing Tests**

When writing unit tests, it's important to maintain clarity, readability, and consistency. Below are essential guidelines:

#### **Test Naming Conventions**

- Use clear, descriptive names for test methods to convey the intent of the test.
- Follow the naming convention: `PreparedStateThatIsRelevant_MethodToTestWithScenarioDescription_ExpectedBehavior` for easy understanding.

**Example:**

```csharp
  public void InitializeDAQmxTasks_ReadAnalogSamplesFromOneFilteredChannel_ResultsContainExpectedData()
```

#### **Test Structure**

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

#### **Using Attributes**

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

#### **Adding Platform-Specific Traits (INTERNAL-ONLY)**

Based on the platform, add Trait attribute followed by the Tester's Name.
**Example:**

```csharp
[Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
```

## **General Considerations for All Extension Method**

### **Reusability and Modularity**

- When adding multiple high-level extension methods for a certain instrument with common, repeated code that can be reused between methods, extract that code into a separate method and refactor your methods to use it.

- :heavy_check_mark: **Do**
  - If all of your methods are part of the same .cs file, then place the new method within that same .cs file and mark it as a `private` method.
  - If your methods are spread across multiple .cs files, then add the new method to an `internal static class Utilities` class within the namespace for the instrument type being worked on, and mark it as an `internal` method. If no such Class exists, create it (e.g. [ExcludeSpecificChannel](https://github.com/ni/semi-test-library-dotnet/blob/12282644789e1f03018b6e3e024829d405ddad1d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Utilities.cs#L7))
- :x: **Don't**
  - Never refactor existing methods to use the new method. Only focus on the methods being added.

### **Error Handling and Validation**

Ensure proper error handling is in place. This could include validating input parameters, checking communication status with the instrument, and providing useful error messages if the operation fails.

When invoking `Do` and `DoAndReturnXXXResults` methods, provide the `caseDescription` parameter to add meaningful context to low-level error messages, making it easier for users to understand and troubleshoot issues.

**Example**: This [example](https://github.com/ni/semi-test-library-dotnet/blob/12282644789e1f03018b6e3e024829d405ddad1d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DAQmx/Utilities.cs#L18) method is used in DAQmx Digital Input and Analog Input functions for validating the task type and throw error accordingly.

### **Determining the Scope of the Method: Channel, Model and Session**

|| **Channel-based Methods** | **Session-based Methods** | **Module-based Methods** |
|----------|----------|----------|----------|
| **Functionality** | If the functionality is specific to individual channels, use the appropriate `Do` or `DoAndReturnXXXResults` methods to iterate over each channel. For more details on how to use these methods, refer to the [Do And DoAndReturnXXXResults Methods](#do-and-doandreturnxxxresults-methods) section.| If the functionality is specific to the entire session, ensure the value is retrieved once per session and applied consistently across all sessions using the appropriate `Do` or `DoAndReturnXXXResults` methods. For more details on how to use these methods, refer to the [Do And DoAndReturnXXXResults Methods](#do-and-doandreturnxxxresults-methods) section  | If the functionality is applicable to an entire module (e.g., an instrument), ensure the value is retrieved once per module and applied consistently across all channels within that module using the appropriate `Do` or `DoAndReturnXXXResults` methods. For more details on how to use these methods, refer to the [Do And DoAndReturnXXXResults Methods](#do-and-doandreturnxxxresults-methods) section .|
|  **Naming a method** | Use a descriptive name that reflects the channel-specific operation. | Use a descriptive name that reflects the session-specific operation. | Use a descriptive name that reflects the module-specific operation. |
|  **Example** | [GetTimeSetEdge](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/LevelsAndTiming.cs#L315) | [GetTimeSetPeriod](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/LevelsAndTiming.cs#L254)| [ReadSequencerFlag](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/SequencerFlagsAndRegisters.cs#L20) |

> While choosing the *return type*, refer the [key considerations](#choosing-parameter-and-return-types).

### **Choosing Parameter and Return Types**

The decision to use Scalar, SiteData, or PinSiteData depends on the level of requirement. Below are the guidelines for when to use each type:

#### **Scalar Input/Return Type**

***Use Case***: When the same value applies uniformly across all channels or pins in the session.

**Example**:

- ***Input Parameter***: `DCPowerSourceSettings settings` Represents a single value (e.g., voltage level) applied to all sessions.
  - [public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, DCPowerSourceSettings settings)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L26)

- ***Return Type***:`List<TResult> or TResult[]` Each value indicates the result of the specified function for an individual session.
  - [public static bool[] ReadSequencerFlag(this DigitalSessionsBundle sessionsBundle, string flag)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/SequencerFlagsAndRegisters.cs#L20)

#### **SiteData Input/Return Type**

***Use Case***: When each site requires a unique value, but the value is consistent across all pins within a site.

**Example**:

- ***Input Parameter***: `SiteData` object represents the value for each site.
  - [public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, SiteData\<DCPowerSourceSettings> settings)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L36)
- ***Return Type***: `SiteData<T>` object containing measurements for each site.
  - [public static SiteData\<bool> GetSitePassFail(this DigitalSessionsBundle sessionsBundle)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/Digital/Pattern.cs#L95)

#### **PinSiteData Input/Return Type**

***Use Case***: When each pin and site combination requires a unique value.

**Example**:

- ***Input Parameter***: `PinSiteData` object that specifies the value for each pin-site combination.
  - [public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, PinSiteData\<DCPowerSourceSettings> settings)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L46)
- ***Return Type***: `PinSiteData<T>` object that contains measurements for each pin-site combination.
  - [public static PinSiteData\<double> GetSourceDelayInSeconds(this DCPowerSessionsBundle sessionsBundle)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Source.cs#L803)

#### **PerInstrumentPerChannel - Return type**

***Use Case***

- Use this format when you need to retrieve measurements directly from the instrument channels without considering the site and pin context.
- This approach is useful when needing to work with raw instrument data, where mapping measurements to specific sites or pins is either not applicable or unnecessary.
  - Example: [MeasureAndReturnPerInstrumentPerChannelResults(this DCPowerSessionsBundle sessionsBundle)](https://github.com/ni/semi-test-library-dotnet/blob/87f9ebe52c1eba721fda454b5c1712bb6bdae77d/SemiconductorTestLibrary.Extensions/source/InstrumentAbstraction/DCPower/Measure.cs#L187)

## **Instrument Specific Guidelines**

Some instruments may require additional considerations or have unique limitations. For instance:

### **Power Supplies**

- You may need to handle voltage and current settings separately and always verify that both current and voltage are within their maximum and minimum ranges based on the instrument model. This is mainly to prevent damage to the instrument and to avoid unhandled exceptions.

### **DC Power**

- Closely pay attention to cases where different model sessions are in a single sessions bundle because different models may have varying capabilities, limitations, and configuration requirements.
- Ensure that your code handles different model sessions in a single sessions bundle effectively.

### **Digital**

- In Digital, it is crucial to ensure the site details before proceeding with the development because site-specific operations can significantly impact the accuracy and reliability of the results.

### **DAQmx**

- Configure DAQmx sessions with homogeneous pin types. Avoid mixing analog and digital pin configurations within a single session. For example, when initializing an Analog Input (AI) session, include only pins designated for AI.
- Avoid combining digital extension methods and analog methods. Maintain clear separation between analog and digital operations.
- While accessing the read methods, note that the readings change smoothly over time, not instantly. For example, if an Analog Output (AO) is set to write 5 V, the corresponding Analog Input (AI) read will show the voltage ramping towards 5 V, rather than an immediate 5 V reading.
