# SMU Hardware Level Sequencing Example

This example demonstrates how to use the Semiconductor Test Library (STL) extension methods for Hardware Level Sequencing (HLS) to run different force voltage sequences with an NI Source Measurement Unit (SMU).

## Overview

The example showcases four different approaches for forcing voltage ramp sequences on SMU pins:

1. **ForceVoltageRamp**: Forces a basic voltage ramp sequence on specified SMU pins.
2. **ConfigureVoltageRampSequenceInitiateAndFetchCurrentMeasurements**: Configure and initiate voltage ramp sequence and publishes the max current measured during sequence execution.
3. **ForceSynchronizedVoltageRamp**: Forces a voltage ramp sequence synchronized across pins.
4. **ConfigureSMUAdvancedSequence**: Configures an advanced sequence upfront without initializing it, allowing later initiation in the test flow.
5. **InitiateSMUAdvancedSequence**: Initiates a previously configured advanced sequence for the specified SMU pins.

## Prerequisites

### Software Requirements

- STS Software 24.5.0 or later .

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
>
>Not all devices are supported by Hardware Level Sequencing, Please refer to [NI-DCPower Supported Functions by Device](https://www.ni.com/docs/en-US/bundle/ni-dcpower-c-api-ref/page/group____root__nidcpower__supported__functions__by__device.html) for details.

## Key Files

### Sequence and Configuration Files

- `STLExample.ConfigureAndForceSynchronizedAndAsynchronizedVoltageRamp.seq`: Example TestStand sequence that demonstrates running the sample HLS test steps.
- `STLExample.ConfigureAndForceSynchronizedAndAsynchronizedVoltageRamp.pinmap`: Pin map file containing device and pin information for the SMU.

### Code Modules

- `Code Modules/TestSteps/ForceVoltageRamp.cs`: Forces a hardware-timed voltage ramp sequence on the specified SMU pins using `ForceVoltageSequence`.
- `Code Modules/TestSteps/ConfigureVoltageRampSequenceInitiateAndFetchCurrentMeasurements.cs`: Configure and initiate hardware-timed voltage ramp sequence on the specified SMU pins and fetches current measurements taken during each step of the sequence. Publishes the max current value across the steps using the "MaxCurrent" published data id.
- `Code Modules/TestSteps/ForceSynchronizedVoltageRamp.cs`: Forces a hardware-timed voltage ramp sequence that is synchronized across the specified SMU pins.
- `Code Modules/TestSteps/ConfigureSMUAdvancedSequence.cs`: Configures an advanced sequence for the specified SMU pins without setting it as the active sequence, allowing it to be initiated later in the test flow.
- `Code Modules/TestSteps/InitiateSMUAdvancedSequence.cs`: Initiates a previously configured advanced sequence for the specified SMU pins.

## Using the Example

### Step 1: Open the Sequence File

1. Launch the **TestStand Sequence Editor**.
2. Open the sequence file `STLExample.ConfigureAndForceSynchronizedAndAsynchronizedVoltageRamp.seq` located in this example's directory.

### Step 2: Review the MainSequence

1. In the TestStand Sequence Editor, select the **MainSequence** tab to view the test steps.
2. Observe the sequence of test steps that demonstrate the different hardware level sequencing approaches:
   - **ForceVoltageRamp** — Forces a basic voltage ramp on SMU pins.
   - **ConfigureVoltageRampSequenceInitiateAndFetchCurrentMeasurements** — Configures, initiates, and fetches current measurements from a voltage ramp sequence.
   - **ForceSynchronizedVoltageRamp** — Forces a synchronized voltage ramp across multiple SMU pins.
   - **ConfigureSMUAdvancedSequence** — Configures an advanced sequence without immediately initiating it.
   - **InitiateSMUAdvancedSequence** — Initiates the previously configured advanced sequence.
3. Note the order of execution and how the steps are organized. Pay attention to the step properties (such as pin names and parameter values) configured for each step by selecting a step and reviewing its settings in the **Step Settings** pane.

### Step 3: Review the Pin Map

1. From the TestStand Sequence Editor, open the pin map by selecting **Semiconductor Module → Edit Pin Map File...** from the menu bar, or by clicking the **Edit Pin Map File** button on the TSM toolbar.
2. The pin map file `STLExample.ConfigureAndForceSynchronizedAndAsynchronizedVoltageRamp.pinmap` will open in the **Pin Map Editor**.
3. Review the pin definitions and the instrument assignments. Note that the pin map is configured to use an NI-DCPower instrument named `SMU_4147_C2_S03`.
4. **If you are using a different SMU that supports Hardware Level Sequencing:**
   - Locate the instrument entry for `SMU_4147_C2_S03` in the Pin Map Editor.
   - Update the instrument name to match the name of your available SMU as it appears in **NI MAX** (Measurement & Automation Explorer).
   - Save the pin map file after making changes.
   - Refer to [NI-DCPower Supported Functions by Device](https://www.ni.com/docs/en-US/bundle/ni-dcpower-c-api-ref/page/group____root__nidcpower__supported__functions__by__device.html) to verify that your SMU model supports the advanced sequencing functions used in this example.

### Step 4: Review the Code Implementation

You can open the C# source code in one of two ways:

- **From TestStand:** In the MainSequence, **double-click** any test step to open its associated code module directly in Visual Studio.
- **From disk:** Navigate to the `Code Modules` folder within this example's directory and **double-click** the Visual Studio solution file (`.sln`) to open the full project in Visual Studio. Alternatively, open Visual Studio manually and use **File -> Open -> Project/Solution** to browse to and open the solution file.
Once the code is open in Visual Studio, review each of the following files to understand how the STL extension methods are used.

> **TIP:** In Visual Studio, **hover over** any STL extension method name to view its inline documentation. This provides details on the method's parameters, expected behavior, and return values.

### Step 5: Run the Test Program

1. Return to the **TestStand Sequence Editor**.
2. Ensure all [Prerequisites](#prerequisites) are met, including having the correct SMU hardware installed (or Offline Mode enabled for simulation).
3. Click the **Start Lot(F5)** or **Single Test(Ctrl + F5)** button on the TSM toolbar to execute the test sequence.
4. Monitor the execution in the TestStand Sequence Editor and review the results upon completion.

## Related Documentation

- [NI-DCPower Supported Functions by Device](https://www.ni.com/docs/en-US/bundle/ni-dcpower-c-api-ref/page/group____root__nidcpower__supported__functions__by__device.html) - For advanced sequence support by instrument model
