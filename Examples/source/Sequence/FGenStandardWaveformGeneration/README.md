# FGen Standard Waveform Generation Sequence Example

This example demonstrates how to use the Semiconductor Test Library (STL) extension methods for the Function Generator (NI-FGEN) to generate a standard waveform with an NI Function Generator (PXIe-543x).

## Overview

The example showcases a single test step that generates a standard waveform on pins mapped to NI-FGEN channels:

1. **GenerateStandardWaveform**: Configures the output mode to Function, configures the standard waveform settings (waveform type, frequency, amplitude, DC offset, and start phase) on all channels mapped to the specified pins (`AI0` and `AI1`), enables the output, and initiates signal generation. The step then waits for generation to complete using a 10-second timeout, and aborts generation before returning. If the wait times out because generation is still running, which is expected for continuous waveforms, the timeout exception is caught and the step continues to abort generation.
   > **NOTE:** To generate continuously, remove the wait and abort calls, and abort generation in a subsequent code module.

The supported waveform types are Sine, Square, Triangle, RampUp, RampDown, DC, and Noise.

> **NOTE:** The `User` waveform type is not supported.

## Prerequisites

### Software Requirements

- STS Software 26.5.0 or later

### Hardware Requirements

- Three NI Function Generators (PXIe-543x) named `FGEN_5433_C1_S13`, `FGEN_5433_C2_S05`, and `FGEN_5433_C3_S03` (or update the pin map to use the names of the NI Function Generators in your system, as defined in NI MAX)

> **NOTE:**
> You can view the example sequence file in the TestStand Sequence Editor and C# source files in Visual Studio or any text editor without meeting the hardware requirements.
>
> You do not need the required instruments physically installed in your system to run the example, as it can be run with simulated instruments using Offline Mode.
> However, to generate an actual waveform at the output connectors of the instruments, you must have the required hardware installed. To observe the generated waveform, connect an oscilloscope or other measurement instrument to the instrument channels mapped to the pins.
>
> **To simulate instruments in Offline Mode:**
>
> 1. Open the sequence file (.seq) in the TestStand Sequence Editor.
> 2. Click the **Enable Offline Mode** button on the TSM toolbar.
> 3. Click the **Start/Resume Lot** button on the TSM toolbar to run the test sequence.
> 4. Click the **Disable Offline Mode** button to return to the default TSM behavior.

## Key Files

### Sequence and Configuration Files

- `STLExample.FGenStandardWaveformGeneration.seq`: Example TestStand sequence that demonstrates running the sample standard waveform generation test step.
- `STLExample.FGenStandardWaveformGeneration.pinmap`: Pin map file containing device and pin information for the function generators.
- `STLExample.FGenStandardWaveformGeneration.offlinecfg`: Offline Mode system configuration file containing the simulated function generators.

### Code Modules

- `Code Modules/TestStep/GenerateStandardWaveform.cs`: Configures the output mode to Function, configures the standard waveform settings, enables the output, and initiates signal generation on the specified pins. Waits for generation to complete using a 10-second timeout, catches the timeout exception if generation is still running, and then aborts generation.

## Using the Example

### Step 1: Open the Sequence File

1. Launch the **TestStand Sequence Editor**.
2. Open the sequence file `STLExample.FGenStandardWaveformGeneration.seq` located in this example's directory.

### Step 2: Review MainSequence

1. In the TestStand Sequence Editor, select the **MainSequence** tab to view the test steps.
2. Observe the test step that demonstrates standard waveform generation:
   - **Generate Standard Waveform** - Generates a standard waveform on the `AI0` and `AI1` pins.
3. Select the step and review its settings in the **Step Settings** pane. The step is configured to generate a sine waveform on the `AI0` and `AI1` pins with a frequency of 100000 Hz, an amplitude of 5 V peak-to-peak, a DC offset of 1 V, and a start phase of 0 degrees.
4. The waveform type is passed using the `Locals.Sine` variable. To generate a different waveform type, update the `standardWaveformType` parameter to use one of the other waveform type variables defined under **Locals** (`Square`, `Triangle`, `RampUp`, `RampDown`, `DC`, or `Noise`).

### Step 3: Review the Pin Map

1. From the TestStand Sequence Editor, open the pin map by selecting **Semiconductor Module -> Edit Pin Map File...** from the menu bar, or by clicking the **Edit Pin Map File** button on the TSM toolbar.
2. Review the instrument definitions and pin assignments. Note that the pin map is configured to use three NI Function Generators (PXIe-543x) named `FGEN_5433_C1_S13`, `FGEN_5433_C2_S05`, and `FGEN_5433_C3_S03`, with the pins `AI0`, `AI1`, and `Clk`, respectively. Each pin is connected to channel 0 of its instrument for site 0, and to channel 1 for site 1. The **Generate Standard Waveform** step uses only the `AI0` and `AI1` pins.
3. **If you are using a different function generator:**
   - Locate the instrument entries for `FGEN_5433_C1_S13`, `FGEN_5433_C2_S05`, and `FGEN_5433_C3_S03` in the Pin Map Editor.
   - Update the instrument names to match the names of your available instruments as they appear in **NI MAX** (Measurement & Automation Explorer).
   - Update the channel assignments for the `AI0`, `AI1`, and `Clk` pins as needed.
   - Save the pin map file after making changes.

### Step 4: Review the Code Implementation

You can open the C# source code in one of two ways:

- **From TestStand:** In the MainSequence, **double-click** any test step to open its associated code module directly in Visual Studio.
- **From disk:** Navigate to the `Code Modules` folder within this example's directory and **double-click** the Visual Studio solution file (`.sln`) to open the full project in Visual Studio. Alternatively, open Visual Studio manually and use **File -> Open -> Project/Solution** to browse to and open the solution file.
- Once the code is open in Visual Studio, review each of the key files listed in the [code modules section](#code-modules) above.

> **TIP:** In Visual Studio, **hover over** any STL extension method name to view its inline documentation. This provides details on the method's parameters, expected behavior, and return values.

### Step 5: Run the Test Program

1. Return to the **TestStand Sequence Editor**.
2. Ensure all [Prerequisites](#prerequisites) are met, including having the correct function generator hardware installed (or Offline Mode enabled for simulation).
3. Click the **Start Lot (F5)** or **Single Test (Ctrl + F5)** button on the TSM toolbar to execute the test sequence.
4. Monitor the execution in the TestStand Sequence Editor and review the results upon completion.
