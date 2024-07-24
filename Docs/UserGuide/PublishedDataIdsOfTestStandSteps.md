# Published Data IDs of TestStandSteps

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
