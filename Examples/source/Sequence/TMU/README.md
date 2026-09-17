# TMU Example

This example demonstrates how to use the Semiconductor Test Library (STL) extension methods for the Time Measurement Unit (TMU) to perform different timing measurements with an NI Digital Pattern Instrument (PXIe-657X).

## Overview

The example showcases six different TMU measurement types on digital pins:

1. **MeasurePeriod**: Measures the period of a digital signal using rising edge detection and publishes the averaged result using the `"Period"` published data id.
2. **MeasurePulseWidth**: Measures the high pulse width of a digital signal, from the rising edge to the subsequent falling edge at Voh, and publishes the averaged result using the `"PulseWidth"` published data id.
3. **MeasureDutyCycle**: Measures the duration the signal spends in the high state and publishes it using the `"DutyCycleTime"` published data id, then performs a period measurement to convert that duration into a ratio, which is published using the `"DutyCycle"` published data id.
   > **Note:** The duty cycle configuration method returns a time duration, not a percentage. This step performs an additional period measurement to convert the result.
4. **MeasureRiseTime**: Measures the rise time of a digital signal, from the low voltage threshold (Vol) to the high voltage threshold (Voh), and publishes the averaged result using the `"RiseTime"` published data id.
5. **MeasureFallTime**: Measures the fall time of a digital signal, from the high voltage threshold (Voh) to the low voltage threshold (Vol), and publishes the averaged result using the `"FallTime"` published data id.
6. **MeasureSkew**: Measures the skew between the same edge type occurring on a reference pin and a target pin and publishes the averaged result using the `"Skew"` published data id. A positive result means the target edge occurs after the reference edge.

Each step follows the same general pattern: assign TMU resources, configure the measurement, initiate, fetch the averaged result using a 5 second timeout, publish the results, and finally disable the TMU and clear the resource assignment.

> **Note:** The TMU configuration methods enable the TMU resource internally, so no separate `EnableTMU` call is required.

## Prerequisites

### Software Requirements

- STS Software 26.5.0 or later

### Hardware Requirements

- An NI Digital Pattern Instrument (PXIe-657X) named `HSD_6571_C1_S03` (or update the pin map with your desired instrument name) as defined in NI MAX
- A signal source driving the measured pins `C0` and `C1`

> **NOTE**
> You can view the example sequence file in the TestStand Sequence Editor and C# source files in Visual Studio or any text editor without meeting the hardware requirements.
>
> You do not need the required instruments physically installed in your system to run the example, as it can be run with instruments simulated using Offline Mode. However, the TMU measures the timing characteristics of a real signal present on the pin under test. Therefore, to obtain actual measurement results from the test steps, you must have the required hardware installed and a continuously toggling digital signal driven onto the measured pins, with the pin levels (Vol/Voh) matching the amplitude of the incoming signal, since edge detection depends on these thresholds.
>
> **To simulate instruments in Offline Mode:**
>
> 1. Open the sequence file (.seq) in the TestStand Sequence Editor.
> 2. Click the **Enable Offline Mode** button on the TSM toolbar.
> 3. Click the **Start/Resume Lot** button on the TSM toolbar to run the test sequence.
> 4. Click the **Disable Offline Mode** button to return to the default TSM behavior.
>
> Not all digital pattern instruments support the TMU. Refer to the NI Digital Pattern Instruments (PXIe-657X) documentation for details.

## Key Files

### Sequence and Configuration Files

- `STLExample.TMU.seq`: Example TestStand sequence that demonstrates running the sample TMU test steps.
- `Supporting Materials/Pin Maps/STLExample.TMU.pinmap`: Pin map file containing device and pin information for the digital pattern instrument.
- `Supporting Materials/STLExample.TMU.digiproj`: Digital pattern project file containing the levels and timing used by the example.

### Code Modules

- `Code Modules/TestSteps/MeasurePeriodTMU.cs`: Configures the TMU for a rising edge period measurement on the `C0` pin and publishes the averaged period using the `"Period"` published data id.
- `Code Modules/TestSteps/MeasurePulseWidthTMU.cs`: Configures the TMU for a high pulse width measurement on the `C0` pin and publishes the averaged pulse width using the `"PulseWidth"` published data id.
- `Code Modules/TestSteps/MeasureDutyCycleTMU.cs`: Configures the TMU for a high duty cycle measurement on the `C0` pin, publishes the measured duration using the `"DutyCycleTime"` published data id, and publishes the duty cycle ratio calculated from an additional period measurement using the `"DutyCycle"` published data id.
- `Code Modules/TestSteps/MeasureRiseTimeTMU.cs`: Configures the TMU for a rise time measurement (Vol to Voh) on the `C0` pin and publishes the averaged rise time using the `"RiseTime"` published data id.
- `Code Modules/TestSteps/MeasureFallTime.cs`: Configures the TMU for a fall time measurement (Voh to Vol) on the `C0` pin and publishes the averaged fall time using the `"FallTime"` published data id.
- `Code Modules/TestSteps/MeasureSkewTMU.cs`: Configures the TMU for a skew measurement between the reference pin `C0` and the target pin `C1` and publishes the averaged skew using the `"Skew"` published data id.

## Using the Example

### Step 1: Open the Sequence File

1. Launch the **TestStand Sequence Editor**.
2. Open the sequence file `STLExample.TMU.seq` located in this example's directory.

### Step 2: Review MainSequence

1. In the TestStand Sequence Editor, select the **MainSequence** tab to view the test steps.
2. Observe the sequence of test steps that demonstrate the different TMU measurement types:
   - **MeasurePeriod** - Measures the signal period using rising edge detection.
   - **MeasurePulseWidth** - Measures the high pulse width of the signal.
   - **MeasureDutyCycle** - Measures the high duration and converts it to a ratio using the measured period.
   - **MeasureRiseTime** - Measures the rise time from Vol to Voh.
   - **MeasureFallTime** - Measures the fall time from Voh to Vol.
   - **MeasureSkew** - Measures the skew between the reference pin `C0` and target pin `C1`.
3. Note the order of execution and how the steps are organized. Pay attention to the step properties configured for each step by selecting a step and reviewing its settings in the **Step Settings** pane.

### Step 3: Review the Pin Map

1. From the TestStand Sequence Editor, open the pin map by selecting **Semiconductor Module -> Edit Pin Map File...** from the menu bar, or by clicking the **Edit Pin Map File** button on the TSM toolbar.
2. Review the instrument definitions and pin assignments. Note that the pin map is configured to use an NI Digital Pattern Instrument (PXIe-657X) named `HSD_6571_C1_S03`, with the digital pins `C0` and `C1`.
3. **If you are using a different digital pattern instrument:**
   - Verify that your instrument model supports the TMU.
   - Locate the instrument entry for `HSD_6571_C1_S03` in the Pin Map Editor.
   - Update the instrument name to match the name of your available instrument as it appears in **NI MAX** (Measurement & Automation Explorer).
   - Update the channel assignments for the `C0` and `C1` pins as needed.
   - Save the pin map file after making changes.

### Step 4: Review the Code Implementation

You can open the C# source code in one of two ways:

- **From TestStand:** In the MainSequence, **double-click** any test step to open its associated code module directly in Visual Studio.
- **From disk:** Navigate to the `Code Modules` folder within this example's directory and **double-click** the Visual Studio solution file (`.sln`) to open the full project in Visual Studio. Alternatively, open Visual Studio manually and use **File -> Open -> Project/Solution** to browse to and open the solution file.
- Once the code is open in Visual Studio, review each of the key files listed in the [code modules section](#code-modules) above.

> **TIP:** In Visual Studio, **hover over** any STL extension method name to view its inline documentation. This provides details on the method's parameters, expected behavior, and return values.

### Step 5: Run the Test Program

1. Return to the **TestStand Sequence Editor**.
2. Ensure all [Prerequisites](#prerequisites) are met, including having the correct digital pattern hardware installed (or Offline Mode enabled for simulation). A signal must be present on the measured pins to obtain actual measurement results.
3. Click the **Start Lot(F5)** or **Single Test(Ctrl + F5)** button on the TSM toolbar to execute the test sequence.
4. Monitor the execution in the TestStand Sequence Editor and review the results upon completion.
