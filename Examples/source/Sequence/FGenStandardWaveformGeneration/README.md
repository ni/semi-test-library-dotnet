# FGen Standard Waveform Generation Sequence Example

This example demonstrates how to use the Semiconductor Test Library (STL) extension methods for the Function Generator (NI-FGEN) to generate standard waveforms with an NI Function Generator (PXIe-543x).

## Overview

The example showcases three different standard waveform types generated on the `AI0` and `AI1` pins, which are mapped to NI-FGEN channels:

1. **Generate Sine Wave**: Generates a 100kHz sine wave with an amplitude of 5V, and zero offset and phase.
2. **Generate Square Wave**: Generates a 100kHz square wave with an amplitude of 5V, and zero offset and phase.
3. **Generate Triangle Wave**: Generates a 100kHz triangle wave with an amplitude of 5V, and zero offset and phase.

Each step calls the same `GenerateStandardWaveform` code module, which generates the specified waveform type for 10 seconds on the "AI0" and "AI1" pins using the configured standard waveform settings (frequency, amplitude, offset, start phase).

> **NOTE:** To generate continuously, remove the wait and abort calls from the GenerateStandardWaveform code module, and add a new code module to abort generation in a subsequent test step.

The supported waveform types are Sine, Square, Triangle, RampUp, RampDown, DC, and Noise.

> **NOTE:** The `User` waveform type is not supported.

## Prerequisites

### Software Requirements

- STS Software 26.5.0 or later

### Hardware Requirements

- Three NI Function Generators (PXIe-543x) named `FGEN_5433_C1_S03`, `FGEN_5433_C1_S04`, and `FGEN_5433_C1_S05` (or update the pin map to use the names of the NI Function Generators in your system, as defined in NI MAX)

> **NOTE:**
> You can view the example sequence file in the TestStand Sequence Editor and C# source files in Visual Studio or any text editor without meeting the hardware requirements.
>
> You do not need the required instruments physically installed in your system to run the example, as it can be run with simulated instruments using Offline Mode.
> However, to generate an actual waveform at the output connectors of the instruments, you must have the required hardware installed. To observe the generated waveforms, connect an oscilloscope or other measurement instrument to the instrument channels mapped to the `AI0` and `AI1` pins.
>
> **To simulate instruments in Offline Mode:**
>
> 1. Open the sequence file (.seq) in the TestStand Sequence Editor.
> 2. Click the **Enable Offline Mode** button on the TSM toolbar.
> 3. Click the **Start/Resume Lot** button on the TSM toolbar to run the test sequence.
> 4. Click the **Disable Offline Mode** button to return to the default TSM behavior.

## Key Files

### Sequence and Configuration Files

- `STLExample.FGenStandardWaveformGeneration.seq`: Example TestStand sequence that demonstrates running the sample standard waveform generation test steps.
- `STLExample.FGenStandardWaveformGeneration.pinmap`: Pin map file containing device and pin information for the function generators.
- `STLExample.FGenStandardWaveformGeneration.offlinecfg`: Offline Mode system configuration file containing the simulated function generators.

### Code Modules

- `Code Modules/TestStep/GenerateStandardWaveform.cs`: Configures the output mode to Function, configures the standard waveform settings for the specified waveform type, enables the output, and initiates signal generation on the specified pins. Waits for 10-seconds for the generation to complete and then aborts generation.

## Using the Example

### Step 1: Open the Sequence File

1. Launch the **TestStand Sequence Editor**.
2. Open the sequence file `STLExample.FGenStandardWaveformGeneration.seq` located in this example's directory.

### Step 2: Review MainSequence

1. In the TestStand Sequence Editor, select the **MainSequence** tab to view the test steps.
2. Observe the sequence of test steps that demonstrate standard waveform generation:
   - **Generate Sine Wave** - Generates a sine waveform on the `AI0` and `AI1` pins.
   - **Generate Square Wave** - Generates a square waveform on the `AI0` and `AI1` pins.
   - **Generate Triangle Wave** - Generates a triangle waveform on the `AI0` and `AI1` pins.
3. Select each step and review its settings in the **Step Settings** pane. Each waveform generation step is configured with a frequency of 100000 Hz, an amplitude of 5V peak-to-peak, a DC offset of 0V, and a start phase of 0 degrees.
4. To generate a different waveform type, update the `standardWaveformType` parameter of the step to one of the other supported waveform types ("RampUp", "RampDown", "DC", or "Noise").

### Step 3: Review the Pin Map

1. From the TestStand Sequence Editor, open the pin map by selecting **Semiconductor Module -> Edit Pin Map File...** from the menu bar, or by clicking the **Edit Pin Map File** button on the TSM toolbar.
2. Review the instrument definitions and pin assignments. Note that the pin map is configured to use three NI Function Generators (PXIe-543x) named `FGEN_5433_C1_S03`, `FGEN_5433_C1_S04`, and `FGEN_5433_C1_S05`, with the pins `AI0`, `AI1`, and `Clk`, respectively. Each pin is connected to channel 0 of its instrument for site 0, and to channel 1 for site 1. The **Generate Standard Waveform** steps use only the `AI0` and `AI1` pins.
3. **If you are using a different function generator:**
   - Locate the instrument entries for `FGEN_5433_C1_S03`, `FGEN_5433_C1_S04`, and `FGEN_5433_C1_S05` in the Pin Map Editor.
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
