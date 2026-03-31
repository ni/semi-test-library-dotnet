# Hardware Level Sequencing (HLS) Voltage Ramp Example

This example demonstrates how to use the Semiconductor Test Library (STL) extension methods for Hardware Level Sequencing (HLS) to run different force voltage sequences with an NI Source Measurement Unit (SMU).

## Overview

The example showcases four different approaches for forcing voltage ramp sequences on SMU pins:

1. **ForceVoltageRamp**: Forces a basic voltage ramp sequence on specified SMU pins using `ForceVoltageSequence`.
2. **ForceVoltageRampMeasureCurrent**: Forces a voltage ramp sequence and measures the resulting current after source completion.
3. **ForceSynchronizedVoltageRampFetchMeasurementsAndPublishMaxCurrent**: Forces a synchronized voltage ramp with hardware triggering across multiple pins, then fetches measurements.
4. **ConfigureSMUAdvancedSequence**: Configures an advanced sequence upfront without activation, allowing later initiation in the test flow.
5. **InitiateSMUAdvancedSequence** : Initiates a previously configured advanced sequence for the specified SMU pins.

> **Note** : When using `ForceVoltageSequence`/`ForceVoltageSequenceSynchronized` or `ConfigureAdvancedSequence`/`ForceAdvancedSequenceSynchronized` within the same sequence, each step **must** use different pins. Using the same pins across these methods in a single sequence is not supported.

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

- `Code Modules/TestSteps/ForceVoltageRamp.cs`: Forces a hardware-timed voltage ramp sequence on the specified SMU pins using `ForceVoltageSequence`.
- `Code Modules/TestSteps/ForceVoltageRampMeasureCurrent.cs`: Forces a hardware-timed voltage ramp sequence on the specified SMU pins and fetches current measurements taken during each step of the sequence. Publishes the max current value across the steps using the "MaxCurrent" published data id.
- `Code Modules/TestSteps/ForceSynchronizedVoltageRampFetchMeasurementsAndPublishMaxCurrent.cs`: Forces a hardware-timed voltage ramp sequence that is synchronized across the specified SMU pins and fetches measurements taken during each step of the sequence. Publishes the max current value across the steps using the "MaxCurrent" published data id.
- `Code Modules/TestSteps/ConfigureSMUAdvancedSequence.cs`: Configures an advanced sequence for the specified SMU pins without setting it as the active sequence, allowing it to be initiated later in the test flow.
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
