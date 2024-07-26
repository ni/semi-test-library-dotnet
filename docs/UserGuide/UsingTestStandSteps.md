# Using TestStand Steps

The Semiconductor Test Library provides a series of TestStand step types for you to use in your test sequences to perform common operations, such as setting up and closing instruments, powering up a DUT, and common tests methods such as continuity and leakage.

STS Software 24.5 includes the `NationalInstruments.SemiconductorTestLibrary.TestStandSteps` assembly and makes the high-level step methods available in the TestStand Insertion Pallet as drag-and-drop step types.

To use the step types of the Semiconductor Test Library in the TestStand Sequence Editor, navigate to Insertion Palette » Step Types, select Semiconductor Module » Common (.NET). Then drag and drop a step type to the target sequence and configure it as necessary.

> [!NOTE]
> The source code for these steps are available on [GitHub](https://github.com/ni/semi-test-library-dotnet) for you use as a reference.

## Instrument Setup and Cleanup with TestStandSteps

The basic setup and cleanup code is not intended to re-written and managed by each test program. Therefore, Users are encouraged to leverage drag-and-drop TestStand step types that are included in the STS Software to initialize and close instrument sessions in their test sequence.

> [!WARNING]
> If you use a upgrade to a newer version of nuget package, the `NationalInstruments.SemiconductorTestLibrary.Abstractions` used by your project may be newer than the one installed by STS Software. Since the `NationalInstruments.SemiconductorTestLibrary.TestStandSteps` assembly uses the `NationalInstruments.SemiconductorTestLibrary.Abstractions` assembly installed with the STS Software version, it will not share the same memory space as your project's output assembly and this will cause problems.

When using the Steps provided by the NationalInstruments.SemiconductorTestLibrary.TestStandSteps assembly, you should ensure you are also calling the `MapInitializedInstrumentSessions` method from the `Initialization` class. This will ensure the cached instrument session information is always consistent between the `NationalInstruments.SemiconductorTestLibrary.TestStandSteps` assembly and your project's output assembly.

>[!NOTE]
> The `MapInitializedInstrumentSessions` method should be called in the ProcessSetup sequence of the test program, immediately after all instrument sessions have been initialized. To see an example of this, use the STS Project Creation Tool to create a new project using the NI Default - C#/.NET template.
>
> Class: `Initialization` \
> Namespace: `NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction` \
> Assembly: `NationalInstruments.SemiconductorTestLibrary.Abstractions.dll`
>
> Refer to the API Reference for more details regarding the `MapInitializedInstrumentSessions` method.

## Published Data IDs of TestStandSteps

The following table shows the published data ids for the TestStandSteps provided by the Semiconductor Test Library that publish test results.

| Method Name                            | PublishedDataId(s)       | Data Type | Units                  |
| :------------------------------------- | :----------------------- | :-------- | :--------------------- |
| `AcquireAnalogInputWaveforms`          | Minimum                  | Numeric   | \*Depends on task type |
|                                        | Minimum                  | Numeric   | \*Depends on task type |
| `BurstPattern`                         | Pattern Pass/Fail Result | Boolean   | N/A                    |
|                                        | Pattern Fail Count       | Numeric   | N/A                    |
| `ContinuityTest`                       | Continuity               | Numeric   | Volts                  |
| `ForceCurrentMeasureVoltage`           | Voltage                  | Numeric   | Volts                  |
| `ForceVoltageMeasureCurrent`           | Current                  | Numeric   | Amperes                |
| `LeakageTest`                          | Leakage                  | Numeric   | Amperes                |

> [!NOTE]
> When dragging and dropping a step from the TestStand Insertion Pallet into a sequence, the resulting step be linked to the version of the TestStandSteps assembly that ships with the version of STS Software you are using. However, Test tab in the Step Settings pane will not automatically be populated, as this is dependent on the number of pins used by the test. You must populate the Test tab yourself using the PublishedDataIds listed above.
