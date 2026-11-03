# Scope Acquisition Sequence Example

This example demonstrates how to use the Semiconductor Test Library (STL) extension methods to acquire a signal with an NI Scope instrument (PXIe-5172). This example demonstrates the multi-site capability of scope extension methods using 4 sites in the pinmap file. 


## Overview

The example showcases five different trigger types for waveform acquisition on scope pins:

1. **Immediate Trigger** (`ConfigureTriggerImmediate`): Waveform acquisition starts immediately after the acquisition is initiated, without waiting for a trigger condition.
2. **Edge Trigger** (`ConfigureEdgeTrigger`): Acquisition starts when the signal crosses the specified `triggerLevel` with the specified `triggerSlope` (positive or negative), using the specified `triggerCoupling`, `holdOff`, and `delay`.
3. **Digital Trigger** (`ConfigureTriggerDigital`): Acquisition starts when a rising or falling edge (`triggerSlope`) of a digital trigger signal is detected, using the specified `holdOff` and `delay`.
4. **Hysteresis Trigger** (`ConfigureTriggerHysteresis`): Acquisition starts when the signal crosses the `triggerLevel` with the specified `triggerSlope`, but only after first crossing the hysteresis band defined by `triggerHysteresis`. This rejects noise around the trigger level.
5. **Window Trigger** (`ConfigureTriggerWindow`): Acquisition starts when the signal enters or leaves (`triggerWindowMode`) the voltage window defined by `windowLowLevel` and `windowHighLevel`.

Each test step follows the same general pattern:

1. Creates a `TSMSessionManager` and gets the `ScopeSessionsBundle` for the `pinOrPinGroup` parameter.
2. Configures the vertical settings (`range`, `offset`, `probeAttenuation`, `enableTimeInterleavedSampling`) and the electrical characteristics (`inputImpedance`, `inputFrequencyMax`), then commits the settings to the instrument.
3. Configures the trigger specific to the step.
4. Initiates the acquisition and fetches the waveform for each pin and site (1000 samples, 50 second timeout).
5. Computes the frequency and amplitude of each waveform using `Utilities.MeasureTone`.
6. Publishes the per-pin, per-site results using the "Frequency" and "Amplitude" Published Data Ids.


### Prerequisites

### Software Requirements

- STS Software 26.5.0 or later

### Hardware Requirements

- Two NI-Scope instruments named `SCOPE_5172_C1_S02` and `SCOPE_5172_C1_S03` (or update the pin map to use the name of the NI Scope cards in your system, as defined in NI MAX).

> **NOTE:**
> You can view the example sequence file in the TestStand Sequence Editor and C# source files in Visual Studio or any text editor without meeting the hardware requirements.
>
> You do not need the required instruments physically installed in your system to run the example, as it can be run with simulated instruments using Offline Mode.
> However, the Scope acquires real signal present on the pin under test. Therefore, to obtain actual measurement results from the test steps, you must have the required hardware installed and feed signal onto the measured pins, with one of the trigger settings based on signal type.
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
- `STLExample.ScopeAcquisition.offlinecfg`: Offline configuration file containing the Instruements used by the example for simulation.

### Code Modules

- `Code Modules/TestSteps/ConfigureEdgeTriggerInitiateAndFetchScopeAcquisition.cs`: Configures the scope for Edge trigger based waveform acquisition on the `pinOrPinGroup` parameter and publishes the computed frequency and amplitude using the "Frequency" and "Amplitude" Published Data Ids.
- `Code Modules/TestSteps/ConfigureInitiateAndFetchScopeAcquisition.cs`: Configures the scope for Immediate waveform acquisition on the `pinOrPinGroup` parameter and publishes the computed frequency and amplitude using the "Frequency" and "Amplitude" Published Data Ids.
- `Code Modules/TestSteps/ConfigureTriggerDigitalInitiateAndFetchScopeAcquisition.cs`: Configures the scope for Digital Triggered waveform acquisition on the `pinOrPinGroup` parameter and publishes the computed frequency and amplitude using the "Frequency" and "Amplitude" Published Data Ids.
- `Code Modules/TestSteps/ConfigureTriggerHysteresisInitiateAndFetchScopeAcquisition.cs`: Configures the scope for Hysteresis Triggered waveform acquisition on the `pinOrPinGroup` parameter and publishes the computed frequency and amplitude using the "Frequency" and "Amplitude" Published Data Ids.
- `Code Modules/TestSteps/ConfigureTriggerWindowInitiateAndFetchScopeAcquisition.cs`: Configures the scope for Window Triggered waveform acquisition on the `pinOrPinGroup` parameter and publishes the computed frequency and amplitude using the "Frequency" and "Amplitude" Published Data Ids.


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
   - **Immediate** (`ConfigureInitiateAndFetchScopeAcquisition`) - Configures an immediate trigger, so the waveform is acquired as soon as the acquisition is initiated.
   - **Edge Trigger** (`ConfigureEdgeTriggerInitiateAndFetchScopeAcquisition`) - Configures an edge trigger using the trigger level, slope, coupling, hold-off, and delay step parameters.
   - **Digital Trigger** (`ConfigureTriggerDigitalInitiateAndFetchScopeAcquisition`) - Configures a digital trigger using the trigger slope, hold-off, and delay step parameters.
   - **Hysteresis Trigger** (`ConfigureTriggerHysteresisInitiateAndFetchScopeAcquisition`) - Configures a hysteresis trigger using the trigger level, hysteresis, slope, coupling, hold-off, and delay step parameters.
   - **Window Trigger** (`ConfigureTriggerWindowInitiateAndFetchScopeAcquisition`) - Configures a window trigger using the window low level, window high level, window mode, coupling, hold-off, and delay step parameters.

   All steps share the common helper methods `ConfigureScope`, which applies the vertical settings and electrical characteristics and commits them, and `InitiateFetchAndPublishScopeAcquisition`, which initiates the acquisition, fetches the waveforms, computes the frequency and amplitude per pin and site, and publishes the results.

4. Note the order of execution and how the steps are organized. Pay attention to the step properties configured for each step by selecting a step and reviewing its settings in the **Step Settings** pane.

Complete the steps in the following sections to learn about the test program components. You can also run this example in offline mode to see it in action.

### Step 4: Review the Pin Map File
1. Select **Semiconductor Module -> Edit Pin Map** File or click the **Edit Pin Map File** button on the TSM toolbar to open the ScopeConfiguredAcquisition pin map file in the Pin Map Editor.
The pin map file defines the following information:
   - Two NI-SCOPE instrument named `SCOPE_5172_C1_S02` and `SCOPE_5172_C1_S03`.
   - Four DUT pins named `Vsc0`, `Vsc1`, `Vsc2`, and `Vsc3`. The `Vsc0` DUT pin used to connect the signal out from DUT.
   - Four sites on the tester.
   - A series of connections for each site, in which each connection specifies a DUT pin, a site number, an instrument, and an instrument channel.

2. Complete the following steps to review the Test Program Configurations that this test program uses.
   1. Select **Semiconductor Module -> Edit Test Program: ScopeAcquisition.seq** or click the **Edit Test Program: ScopeAcquisition.seq** button on the TSM toolbar.
   2. Review the Offline Mode section to see which two scope instruments the STS software simulates when you run this example.

3. Complete the following steps to review the `MainSequence`, `ProcessSetup`, and `ProcessCleanup` sequences that this test program uses.
   1. On the `Sequences` pane, select the MainSequence sequence and review the objectives each step performs and optionally review the code associated with each step:
      - In the main section, the example configures vertical settings, electrical characteristics, trigger settings, then the program initiates the acquisition, and at the end it fetches waveform and prints in the html report.
      - In the `Variables` pane, observe the default values of the various settings used by the program under `Locals` section. The local variables are used to store the waveform object references and pass between the steps in the current sequence.  
   2. On the `Sequences` pane, select the `ProcessSetup` sequence. TestStand calls this sequence once before starting testing. The steps in this sequence initialize the scope instruments and store the instrument sessions in the SemiconductorModuleContext.
   3. On the `Sequences` pane, select the `ProcessCleanup` sequence. TestStand calls this sequence once after testing completes. The steps in this sequence close and reset the  instruments.
4. You must meet all the [Prerequisites](#prerequisites) to run the test program. To run the test program, click the **Start/Resume Lot** button on the TSM toolbar.
