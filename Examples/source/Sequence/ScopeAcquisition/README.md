# Scope Acquisition Sequence Example

This example demonstrates how to use the Semiconductor Test Library (STL) extension methods to acquire a signal with an NI Scope instrument (PXIe-5172).

## Overview

The example showcases five different trigger types for waveform acquisition on scope pins:

1. **Immediate Trigger** (`ConfigureTriggerImmediate`): Acquisition starts immediately after the acquisition is initiated, without waiting for a trigger condition.
2. **Edge Trigger** (`ConfigureEdgeTrigger`): Acquisition starts when the signal crosses the specified `triggerLevel` with the specified `triggerSlope` (positive or negative), using the specified `triggerCoupling`, `holdOff`, and `delay`.
3. **Digital Trigger** (`ConfigureTriggerDigital`): Acquisition starts when a rising or falling edge (`triggerSlope`) of a digital trigger signal is detected, using the specified `holdOff` and `delay`.
4. **Hysteresis Trigger** (`ConfigureTriggerHysteresis`): Acquisition starts when the signal crosses the `triggerLevel` with the specified `triggerSlope`, but only after first crossing the hysteresis band defined by `triggerHysteresis`. This rejects noise around the trigger level.
5. **Window Trigger** (`ConfigureTriggerWindow`): Acquisition starts when the signal enters or leaves (`triggerWindowMode`) the voltage window defined by `windowLowLevel` and `windowHighLevel`.

Each trigger type is represented by a step in the MainSequence. Each step is configured using parameters defined by sequence variables. The code modules for each step follow the same general pattern:

1. Creates a `TSMSessionManager` and gets the `ScopeSessionsBundle` for the `pinOrPinGroup` parameter.
2. Configures the vertical settings (`range`, `offset`, `probeAttenuation`, `enableTimeInterleavedSampling`) and the electrical characteristics (`inputImpedance`, `inputFrequencyMax`), then commits the settings to the instrument.
3. Configures the trigger-specific settings to the scope.
4. Initiates the acquisition and fetches the waveform for each pin and site (1000 samples, 50 second timeout).
5. Computes the frequency and amplitude of each waveform using `Utilities.CalculateWaveformFrequencyAndPeakToPeakAmplitude`.
6. Publishes the per-pin, per-site results using the "Frequency" and "Amplitude" Published Data Ids.

## Prerequisites

### Software Requirements

- STS Software 26.5.0 or later

### Hardware Requirements

- Two NI Scope instruments named SCOPE_5172_C1_S02 and SCOPE_5172_C1_S03 (or update the pin map to use the name of the NI Scope cards in your system, as defined in NI MAX).

> **NOTE:**
> You can view the example sequence file in the TestStand Sequence Editor and C# source files in Visual Studio or any text editor without meeting the hardware requirements.
>
> You do not need the required instruments physically installed in your system to run the example, as it can be run with simulated instruments using Offline Mode.
> However, the Scope acquires a real signal present on the pin under test. Therefore, to obtain actual measurement results from the test steps, you must have the required hardware installed and feed signal onto the measured pins, with one of the trigger settings based on signal type.
>
> **To simulate instruments in Offline Mode:**
>
> 1. Open the sequence file (.seq) in the TestStand Sequence Editor.
> 2. Click the **Enable Offline Mode** button on the TSM toolbar.
> 3. Click the **Start/Resume Lot** button on the TSM toolbar to run the test sequence.
> 4. Click the **Disable Offline Mode** button to return to the default TSM behavior.

## Key Files

### Sequence and Configuration Files

- `STLExample.ScopeAcquisition.seq`: Example TestStand sequence that demonstrates running the sample Scope test steps.
- `STLExample.ScopeAcquisition.pinmap`: Pin map file containing device and pin information for the Scope instrument.
- `STLExample.ScopeAcquisition.offlinecfg`: Offline configuration file containing the Instruments used by the example for simulation.

### Code Modules

- `Code Modules/TestSteps/ConfigureInitiateAndFetchScopeAcquisitionUsingImmediateTrigger.cs`: Configures the specified scope pins to use an immediate trigger, initiates acquisition and fetches the acquired waveform. Then calculates and publishes the measured frequency and amplitude of the acquired waveform.
- `Code Modules/TestSteps/ConfigureInitiateAndFetchScopeAcquisitionUsingEdgeTrigger.cs`: Configures the specified scope pins to use an edge trigger, initiates acquisition and fetches the acquired waveform. Then calculates and publishes the measured frequency and amplitude of the acquired waveform.
- `Code Modules/TestSteps/ConfigureInitiateAndFetchScopeAcquisitionUsingDigitalTrigger.cs`: Configures the specified scope pins to use a digital trigger, initiates acquisition and fetches the acquired waveform. Then calculates and publishes the measured frequency and amplitude of the acquired waveform.
- `Code Modules/TestSteps/ConfigureInitiateAndFetchScopeAcquisitionUsingHysteresisTrigger.cs`: Configures the specified scope pins to use a hysteresis trigger, initiates acquisition and fetches the acquired waveform. Then calculates and publishes the measured frequency and amplitude of the acquired waveform.
- `Code Modules/TestSteps/ConfigureInitiateAndFetchScopeAcquisitionUsingWindowTrigger.cs`: Configures the specified scope pins to use a window trigger, initiates acquisition and fetches the acquired waveform. Then calculates and publishes the measured frequency and amplitude of the acquired waveform.

## Using the Example

### Step 1: Open the Sequence File

1. Launch the **TestStand Sequence Editor**.
2. Open the sequence file `STLExample.ScopeAcquisition.seq` located in this example's directory.

### Step 2: Review ProcessSetup and ProcessCleanup

1. In the TestStand Sequence Editor, select the **ProcessSetup** tab to view the test steps.
2. In the TestStand Sequence Editor, select the **ProcessCleanup** tab to view the test steps.

### Step 3: Review MainSequence

1. In the TestStand Sequence Editor, select the **MainSequence** tab to view the test steps.
2. Observe the sequence of test steps that demonstrate the different trigger-based waveform acquisitions. Each step calls a code module in `Code Modules/TestSteps` that configures the scope, initiates the acquisition, fetches the waveform, and publishes the measured "Frequency" and "Amplitude" results:
   - **Immediate** (`Scope Acquisition Using Immediate Trigger`) - Configures an immediate trigger, so the waveform is acquired as soon as the acquisition is initiated.
   - **Edge Trigger** (`Scope Acquisition Using Edge Trigger`) - Configures an edge trigger using the trigger level, slope, coupling, hold-off, and delay step parameters.
   - **Digital Trigger** (`Scope Acquisition Using Digital Trigger`) - Configures a digital trigger using the trigger slope, hold-off, and delay step parameters.
   - **Hysteresis Trigger** (`Scope Acquisition Using Hysteresis Trigger`) - Configures a hysteresis trigger using the trigger level, hysteresis, slope, coupling, hold-off, and delay step parameters.
   - **Window Trigger** (`Scope Acquisition Using Window Trigger`) - Configures a window trigger using the window low level, window high level, window mode, coupling, hold-off, and delay step parameters.

   Each code module configures the scope, initiates acquisition, fetches waveforms, and publishes results inline. All modules use `Utilities.CalculateWaveformFrequencyAndPeakToPeakAmplitude` to calculate the frequency and peak-to-peak amplitude per pin and site.

3. Note the order of execution and how the steps are organized. Pay attention to the step properties configured for each step by selecting a step and reviewing its settings in the **Step Settings** pane.

### Step 4: Review the Pin Map

1. From the TestStand Sequence Editor, open the pin map by selecting **Semiconductor Module -> Edit Pin Map File...** from the menu bar, or by clicking the **Edit Pin Map File** button on the TSM toolbar.
2. Review the instrument definitions and pin assignments. Note that the pin map is configured to use NI Scope Instruments (PXIe-5172) named `SCOPE_5172_C1_S02` and `SCOPE_5172_C1_S03`, with the scope pins `Vsc0`, `Vsc1`, `Vsc2`, and `Vsc3`.
3. **If you are using a different scope instrument:**
   - Locate the instrument entry for `SCOPE_5172_C1_S02` and `SCOPE_5172_C1_S03` in the Pin Map Editor.
   - Update the instrument name to match the name of your available instrument as it appears in **NI MAX** (Measurement & Automation Explorer).
   - Update the channel assignments for the `Vsc0`, `Vsc1`, `Vsc2`, and `Vsc3` pins as needed.
   - Save the pin map file after making changes.

### Step 5: Review the Offline Configuration

1. Complete the following steps to review the Test Program Configurations that this test program uses.
   1. Select **Semiconductor Module -> Edit Test Program: STLExample.ScopeAcquisition.seq** or click the **Edit Test Program: STLExample.ScopeAcquisition.seq** button on the TSM toolbar.
   2. Review the Offline Mode section to see which two scope instruments the STS software simulates when you run this example.

### Step 6: Review the Code Implementation

You can open the C# source code in the following ways:

- **From TestStand:** In the MainSequence, **double-click** any test step to open its associated code module directly in Visual Studio.
- **From disk:** Navigate to the `Code Modules` folder within this example's directory and **double-click** the Visual Studio solution file (`.sln`) to open the full project in Visual Studio. Alternatively, open Visual Studio manually and use **File -> Open -> Project/Solution** to browse to and open the solution file.

Once the code is open in Visual Studio, review each of the key files listed in the [code modules section](#code-modules) above.

> **TIP:** In Visual Studio, **hover over** any STL extension method name to view its inline documentation. This provides details on the method's parameters, expected behavior, and return values.

### Step 7: Run the Test Program

1. Return to the **TestStand Sequence Editor**.
2. Ensure all [Prerequisites](#prerequisites) are met, including having the correct scope hardware installed (or Offline Mode enabled for simulation). A signal must be present on the measured pins to obtain actual measurement results.
3. If you are running the test on STS hardware, update the values in the `ExpectedValues` container under **FileGlobals** to the expected values for the signal on the measured pins, so that the test passes accordingly.
4. Click the **Start Lot (F5)** or **Single Test (Ctrl + F5)** button on the TSM toolbar to execute the test sequence.
5. Monitor the execution in the TestStand Sequence Editor and review the results upon completion.
