# Hardware Level Sequencing (HLS) Voltage Ramp Example

This example demonstrates how to use the Semiconductor Test Library (STL) extension methods for Hardware Level Sequencing (HLS) to run different force voltage sequences with an NI Source Measurement Unit (SMU).

## Overview

The example showcases four different approaches for forcing voltage ramp sequences on SMU pins:

1. **ForceVoltageRamp**: Forces a basic voltage ramp sequence on specified SMU pins using `ForceVoltageSequence`.
2. **ForceVoltageRampMeasureCurrent**: Forces a voltage ramp sequence and measures the resulting current after source completion.
3. **ForceSynchronizedVoltageRampFetchMeasurementsAndPublishMaxCurrent**: Forces a synchronized voltage ramp with hardware triggering across multiple pins, then fetches measurements.
4. **ConfigureSMUAdvancedSequence**: Configures an advanced sequence upfront without activation, allowing later initiation in the test flow.
5. **InitiateSMUAdvancedSequence** : Initiates a previously configured advanced sequence for the specified SMU pins.

## Prerequisites

### Software Requirements

- NI-DCPower driver

### Hardware Requirements

- A NI-DCPower instrument named `SMU_4147_C2_S03` (or update the pin map with your desired SMU name) as defined in NI MAX

> **NOTE**
> You can view the example sequence file in the TestStand Sequence Editor and C# source files in Visual Studio or any text editor without meeting the hardware requirements.
>
> To run the example, you must have the required instruments physically installed in your system or simulated using Offline Mode.
>
> **To simulate instruments in Offline Mode:**
>
> 1. Open the sequence file (.seq) in the TestStand Sequence Editor.
> 2. Click the **Enable Offline Mode** button on the TSM toolbar.
> 3. Click the **Start/Resume Lot** button on the TSM toolbar to run the test sequence.
> 4. Click the **Disable Offline Mode** button to return to the default TSM behavior.

## Key Files

### Sequence and Configuration Files

- `STLExample.ConfigureAndForceSynchronizedAndAsynchronizedVoltageRamp.seq`: Example TestStand sequence that demonstrates running the sample HLS test steps.
- `STLExample.ConfigureAndForceSynchronizedAndAsynchronizedVoltageRamp.pinmap`: Pin map file containing device and pin information for the SMU.

### Code Modules

- `Code Modules/TestSteps/ForceVoltageRamp.cs`: Demonstrates the simplest approach to forcing a voltage ramp (0V to 3V, 10 points) on specified SMU pins using `ForceVoltageSequence`.
- `Code Modules/TestSteps/ForceVoltageRampMeasureCurrent.cs`: Applies a voltage ramp to specified SMU pins and measures the resulting current. Configures `MeasureWhen` to `AutomaticallyAfterSourceComplete` and publishes the measurement results.
- `Code Modules/TestSteps/ForceSynchronizedVoltageRampFetchMeasurementsAndPublishMaxCurrent.cs`: Forces a synchronized voltage ramp using `ForceVoltageSequenceSynchronized` with hardware triggering. Waits for sequence completion, measures voltage, and cleans up the Start Trigger configuration.
- `Code Modules/TestSteps/ConfigureSMUAdvancedSequence.cs`: Demonstrates advanced sequence configuration using `ConfigureAdvancedSequence` with `setAsActiveSequence: false`, allowing deferred initiation. Useful for scenarios where sequence setup should be separated from execution.
- - `Code Modules/TestSteps/InitiateSMUAdvancedSequence.cs`: Initiates a previously configured advanced sequence for the specified SMU pins.

## Using the Example

1. **Update the Pin Map (if needed):**
   If the hardware `SMU_4147_C2_S03` is not available, update the pin map:
   - Review the pin map from the TestStand Sequence Editor by selecting **Semiconductor Module -> Edit Pin Map File...** or clicking the Edit Pin Map File button on the TSM toolbar.
   - Open `STLExample.ConfigureAndForceSynchronizedAndAsynchronizedVoltageRamp.pinmap` in the Pin Map Editor
   - Update the instrument name to match your available hardware

2. **Run the Test Program:**
   - Ensure all [Prerequisites](#prerequisites) are met
   - Click the **Start/Resume Lot** button on the TSM toolbar

## Related Documentation

- [NI-DCPower Supported Functions by Device](https://www.ni.com/docs/en-US/bundle/ni-dcpower-c-api-ref/page/group____root__nidcpower__supported__functions__by__device.html) - For advanced sequence support by instrument model
