# TMU (Time Measurement Unit) Example

This example demonstrates how to use the Semiconductor Test Library (STL) extension methods for the Time Measurement Unit (TMU) to perform timing measurements on digital signals with an NI Digital Pattern Instrument.

## Overview

The example showcases six different TMU measurement types on digital pins:

1. **MeasurePeriodTMU**: Measures the period of a digital signal by detecting rising edges and publishes the averaged result.
2. **MeasurePulseWidthTMU**: Measures the pulse width of a digital signal (high pulse at Voh or low pulse at Vol). Requires 1 comparator per pin.
3. **MeasureDutyCycleTMU**: Measures the high duty cycle duration and divides it by the measured period to publish the duty cycle as a ratio. Requires 1 comparator per pin.
   > **Note:** The duty cycle configuration method returns a time duration, not a percentage. This step performs an additional period measurement to convert the result.
4. **MeasureRiseTimeTMU**: Measures the rise time of a digital signal, from the low voltage threshold (Vol) to the high voltage threshold (Voh). Requires 2 comparators per pin.
5. **MeasureFallTime**: Measures the fall time of a digital signal, from the high voltage threshold (Voh) to the low voltage threshold (Vol). Requires 2 comparators per pin.
6. **MeasureSkewTMU**: Measures the skew (time difference) between the same edge type on a reference pin and a target pin. A positive result means the target edge occurs after the reference edge.
   > **Note:** `ConfigureTMUSkewMeasurement` enables the TMU internally, so no separate `EnableTMU` call is required.

Each step follows the same general pattern: assign TMU resources, configure the measurement (with `TmuArmType.Immediate`), enable the TMU (where required), initiate, fetch the averaged result over a 5 second timeout, publish results using the `"res"` published data id, and finally disable the TMU and clear the resource assignment.

## ⚠️ Important: A Live Signal Is Required

> **IMPORTANT**
> The TMU measures timing characteristics of a **real signal** present on the pin under test. To run this example successfully, the pins being measured (in this example, `C0`, and additionally `C1` for the skew measurement) **must be receiving a valid, continuously toggling digital signal** at the time the measurement is initiated.
>
> If no signal (or an unsuitable signal) is present on the pin:
>
> - The TMU will never detect the required edges.
> - The `FetchAveragedTMUMeasurement` call will wait until the configured timeout (5 seconds) expires.
> - A timeout exception will be thrown and the **TestStand sequence will error out**.
>
> Before running the example, ensure that one of the following is true:
>
> - The DUT (or a loopback/signal source) is actively driving the measured pins with the expected waveform, **or**
> - A digital pattern is being burst on the instrument that drives/stimulates the measured pins, **or**
> - The measured pins are otherwise externally connected to a running signal source.
>
> Also verify that the pin's voltage levels (Vol/Voh comparator thresholds) are configured appropriately for the incoming signal, since edge detection depends on these thresholds.

## Prerequisites

### Software Requirements

- STS Software 24.5.0 or later

### Hardware Requirements

- An NI Digital Pattern Instrument with TMU support named `HSD_6571_C1_S02` (or update the pin map with your desired instrument name) as defined in NI MAX
- Digital pins `C0` and `C1` mapped in the pin map (`C1` is required for the skew measurement)
- A valid signal source driving the measured pins (see [Important: A Live Signal Is Required](#️-important-a-live-signal-is-required))

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
> Not all digital pattern instruments support the TMU, and the number of available comparators per pin varies by model. Refer to the NI Digital Pattern Instruments documentation for details.

## Key Files

### Sequence and Configuration Files

- `STLExample.TMU.seq`: Example TestStand sequence that demonstrates running the sample TMU test steps.
- `STLExample.TMU.pinmap`: Pin map file containing device and pin information for the digital pattern instrument.

### Code Modules

- `Code Modules/TestSteps/MeasurePeriodTMU.cs`: Configures the TMU for a rising-edge period measurement on the `C0` pin, enables the TMU, and publishes the averaged period using the `"res"` published data id.
- `Code Modules/TestSteps/MeasurePulseWidthTMU.cs`: Configures the TMU for a pulse width measurement on the `C0` pin, enables the TMU, and publishes the averaged pulse width using the `"res"` published data id.
- `Code Modules/TestSteps/MeasureDutyCycleTMU.cs`: Configures the TMU for a high duty cycle measurement on the `C0` pin, performs an additional period measurement, and publishes the duty cycle ratio using the `"res"` published data id.
- `Code Modules/TestSteps/MeasureRiseTimeTMU.cs`: Configures the TMU for a rise time measurement (Vol to Voh) on the `C0` pin, enables the TMU, and publishes the averaged rise time using the `"res"` published data id.
- `Code Modules/TestSteps/MeasureFallTime.cs`: Configures the TMU for a fall time measurement (Voh to Vol) on the `C0` pin, enables the TMU, and publishes the averaged fall time using the `"res"` published data id.
- `Code Modules/TestSteps/MeasureSkewTMU.cs`: Configures the TMU for a skew measurement between the reference pin `C0` and the target pin `C1`, and publishes the averaged skew using the `"res"` published data id.

## Using the Example

### Step 1: Open the Sequence File

1. Launch the **TestStand Sequence Editor**.
2. Open the sequence file `STLExample.TMU.seq` located in this example's directory.

### Step 2: Review MainSequence

1. In the TestStand Sequence Editor, select the **MainSequence** tab to view the test steps.
2. Observe the sequence of test steps that demonstrate the different TMU measurement types:
   - **MeasurePeriod** - Measures the signal period using rising edge detection.
   - **MeasurePulseWidth** - Measures the pulse width of the signal.
   - **MeasureDutyCycle** - Measures the high duty cycle duration and converts it to a ratio using the measured period.
   - **MeasureRiseTime** - Measures the rise time from Vol to Voh.
   - **MeasureFallTime** - Measures the fall time from Voh to Vol.
   - **MeasureSkew** - Measures the skew between the reference pin `C0` and target pin `C1`.
3. Note the order of execution and how the steps are organized. Pay attention to the step properties (such as pin names and parameter values) configured for each step by selecting a step and reviewing its settings in the **Step Settings** pane.

### Step 3: Review the Pin Map

1. From the TestStand Sequence Editor, open the pin map by selecting **Semiconductor Module -> Edit Pin Map File...** from the menu bar, or by clicking the **Edit Pin Map File** button on the TSM toolbar.
2. Review the instrument definitions and pin assignments. Note that the pin map is configured to use an NI Digital Pattern Instrument named `HSD_6571_C1_S02`, with the digital pins `C0` and `C1`.
3. **If you are using a different digital pattern instrument:**
   - Verify that your instrument model supports the TMU and provides enough comparators per pin for the measurements used in this example (rise time and fall time each require 2 comparators per pin).
   - Locate the instrument entry for `HSD_6571_C1_S02` in the Pin Map Editor.
   - Update the instrument name to match the name of your available instrument as it appears in **NI MAX** (Measurement & Automation Explorer).
   - Update the channel assignments for the `C0` and `C1` pins as needed.
   - Save the pin map file after making changes.

### Step 4: Verify the Signal Under Test

1. Confirm that the pins being measured (`C0`, and `C1` for skew) are connected to an active signal source.
2. Confirm that the pin levels (Vol/Voh) configured for those pins match the amplitude of the incoming signal so the TMU comparators can detect edges.
3. If no signal is available, the measurement steps will time out after 5 seconds and the sequence will error out. See [Important: A Live Signal Is Required](#️-important-a-live-signal-is-required).

### Step 5: Review the Code Implementation

You can open the C# source code in one of two ways:

- **From TestStand:** In the MainSequence, **double-click** any test step to open its associated code module directly in Visual Studio.
- **From disk:** Navigate to the `Code Modules` folder within this example's directory and **double-click** the Visual Studio solution file (`.sln`) to open the full project in Visual Studio. Alternatively, open Visual Studio manually and use **File -> Open -> Project/Solution** to browse to and open the solution file.
- Once the code is open in Visual Studio, review each of the key files listed in the [code modules section](#code-modules) above.

> **TIP:** In Visual Studio, **hover over** any STL extension method name (such as `AssignTMUResources`, `ConfigurePeriodMeasurement`, or `FetchAveragedTMUMeasurement`) to view its inline documentation. This provides details on the method's parameters, expected behavior, and return values.

### Step 6: Run the Test Program

1. Return to the **TestStand Sequence Editor**.
2. Ensure all [Prerequisites](#prerequisites) are met, including having the correct digital pattern hardware installed (or Offline Mode enabled for simulation) and a valid signal present on the measured pins.
3. Click the **Start Lot(F5)** or **Single Test(Ctrl + F5)** button on the TSM toolbar to execute the test sequence.
4. Monitor the execution in the TestStand Sequence Editor and review the results upon completion.

## Troubleshooting

| Symptom | Likely Cause | Resolution |
| --- | --- | --- |
| Measurement times out after 5 seconds and the sequence errors out | No signal, or an unsuitable signal, is present on the measured pin | Ensure a valid, continuously toggling signal is driven onto `C0` (and `C1` for skew) before the step runs |
| Edges are not detected even though a signal is present | Vol/Voh comparator thresholds do not match the signal amplitude | Adjust the pin levels so the thresholds fall within the signal's voltage swing |
| Error when configuring rise/fall time measurements | Insufficient comparators available on the pin | Rise time and fall time require 2 comparators per pin; verify your instrument model supports this |
| Error when assigning TMU resources | TMU resource not released by a previous step | Ensure each step calls `DisableTMU` and `ClearTMUAssignment` on completion |

## Related Documentation

- [Semiconductor Test Library Documentation](https://github.com/ni/semi-test-library-dotnet)