# Using TestStand Steps

The Semiconductor Test Library provides a series of TestStand step types for you to use in your test sequences to execute common test methods such as continuity and leakage, and perform common operations, including setting up and closing instruments, and powering up a DUT.

STS Software 24.5 includes the `NationalInstruments.SemiconductorTestLibrary.TestStandSteps` assembly and makes the high-level step methods available in the TestStand Insertion Palette as drag-and-drop step types.

To use the step types of the Semiconductor Test Library in the TestStand Sequence Editor, navigate to Insertion Palette » Step Types and select Semiconductor Module » Common (.NET). Then drag and drop a step type to the target sequence and configure it based on your needs.

> [!NOTE]
> The source code for these steps is available on [GitHub](https://github.com/ni/semi-test-library-dotnet).

## Instrument Setup and Cleanup with TestStandSteps

The basic setup and cleanup code is not intended to be re-written and managed by each test program. Therefore, you are encouraged to leverage drag-and-drop TestStand step types included in the STS Software to initialize and close instrument sessions in the test sequence.

> [!WARNING]
> If you upgrade to a newer version of the NuGet package, the `NationalInstruments.SemiconductorTestLibrary.Abstractions` assembly used by your project may be newer than the one installed by STS Software. Since the `NationalInstruments.SemiconductorTestLibrary.TestStandSteps` assembly uses the `NationalInstruments.SemiconductorTestLibrary.Abstractions` assembly installed with the STS Software version, it will not share the same memory space as your project's output assembly, which will cause problems.

When using the steps provided by the NationalInstruments.SemiconductorTestLibrary.TestStandSteps assembly, you should call the `MapInitializedInstrumentSessions` method from the `Initialization` class. This ensures the cached instrument session information is consistent between the `NationalInstruments.SemiconductorTestLibrary.TestStandSteps` assembly and your project's output assembly.

>[!NOTE]
> You should call the `MapInitializedInstrumentSessions` method once at the start of your test program, immediately after all instrument sessions have been initialized. To see an example, use the STS Project Creation Tool to create a new project and select the NI Default - C#/.NET template.
>
> Class: `Initialization` \
> Namespace: `NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction` \
> Assembly: `NationalInstruments.SemiconductorTestLibrary.Abstractions.dll`
>
> Refer to the API Reference for more details regarding the `MapInitializedInstrumentSessions` method.

## Published Data IDs of TestStandSteps

The following table shows the Published Data Ids for the TestStandSteps provided by the Semiconductor Test Library with published test results.

| Method Name                            | PublishedDataId(s)       | Data Type | Units                  |
| :------------------------------------- | :----------------------- | :-------- | :--------------------- |
| `AcquireAnalogInputWaveforms`          | Minimum                  | Numeric   | \*Depends on task type |
|                                        | Maximum                  | Numeric   | \*Depends on task type |
| `BurstPattern`                         | Pattern Pass/Fail Result | Boolean   | N/A                    |
|                                        | Pattern Fail Count       | Numeric   | N/A                    |
| `ContinuityTest`                       | Continuity               | Numeric   | Volts                  |
| `ForceCurrentMeasureVoltage`           | Voltage                  | Numeric   | Volts                  |
| `ForceVoltageMeasureCurrent`           | Current                  | Numeric   | Amperes                |
| `LeakageTest`                          | Leakage                  | Numeric   | Amperes                |

> [!NOTE]
> When dragging and dropping a step from the TestStand Insertion Palette into a sequence, the resulting step is linked to the version of the TestStandSteps assembly that ships with the STS Software version you are using. However, the Test tab in the Step Settings pane will not automatically be populated, as it is dependent on the number of pins used in the test. You must populate the Test tab using the PublishedDataIds.
