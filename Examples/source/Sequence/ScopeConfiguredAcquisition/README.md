# Scope Configured Acquisition Sequence Example

## Introduction

This example demonstrates how to use the Semiconductor Test Library specifically the Scope extension methods  for configured waveform acquisition. This example demonstrates the multi-site capability of scope extension methods using 2 sites in the pinmap file. 

### Highlighted Features

- Pin Map
- Multi Site
- Offline Mode

### Major APIs

- TSM Code Module API
- Semiconductor Test Library Scope extension methods

### Prerequisites

1. If you want to use the example you must have the following software installed:
   - STS Software 24.5.3 or later
2. To run the test program you must also have:
   - Two NI-Scope instruments named SCOPE_5172_C1_S02 and SCOPE_5172_C1_S03, respectively.
   - TestStand configured to use the Batch process model.

> **NOTE** \
> You can view the test program in the TestStand Sequence Editor and code modules in a C# source code editor without meeting the #2 requirement.
> To run the example you must have the required instruments physically installed in your system, or simulated using Offline Mode.
>
> Complete the following steps to simulate the instruments in Offline Mode:
>
> 1. Click the Enable Offline Mode button  on the TSM toolbar. To run the test program, click the Start/Resume Lot button on the TSM toolbar.
> 2. Launch the TestStand sequence Editor and select the Offline Mode panel to view the path to the Offline Mode system configuration file TSM uses to create simulated instruments for this example.
> 3. Click the Disable Offline Mode button to return to the default TSM behavior.

## Using the Example

Complete the steps in the following sections to learn about the test program components. You can also run this example in offline mode to see it in action.

1. Select **Semiconductor Module -> Edit Pin Map** File or click the **Edit Pin Map File** button on the TSM toolbar to open the ScopeConfiguredAcquisition pin map file in the Pin Map Editor.
The pin map file defines the following information:
   - Two NI-SCOPE instrument named `SCOPE_5172_C1_S02` and `SCOPE_5172_C1_S03`.
   - Four DUT pins named `Vsc0`, `Vsc1`, `Vsc2`, and `Vsc3`. The `Vsc0` DUT pin used to connect the signal out from DUT.
   - Two sites on the tester.
   - A series of connections for each site, in which each connection specifies a DUT pin, a site number, an instrument, and an instrument channel.

2. Complete the following steps to review the Test Program Configurations that this test program uses.
   1. Select **Semiconductor Module -> Edit Test Program: ScopeConfiguredAcquisition.seq** or click the **Edit Test Program: ScopeConfiguredAcquisition.seq** button on the TSM toolbar.

3. Complete the following steps to review the `MainSequence`, `ProcessSetup`, and `ProcessCleanup` sequences that this test program uses.
   1. On the `Sequences` pane, select the MainSequence sequence and review the objectives each step performs and optionally review the code associated with each step:
      - In the main section, the example configures vertical settings, electrical characteristics, trigger settings, then the program initiates the acquisition, and at the end it fetches waveform and prints in the html report.
      - In the `Variables` pane, observe the default values of the various settings used by the program under `Locals` section. The local variables are used to store the waveform object references and pass between the steps in the current sequence.  
   2. On the `Sequences` pane, select the `ProcessSetup` sequence. TestStand calls this sequence once before starting testing. The steps in this sequence initialize the scope instruments and store the instrument sessions in the SemiconductorModuleContext.
   3. On the `Sequences` pane, select the `ProcessCleanup` sequence. TestStand calls this sequence once after testing completes. The steps in this sequence close and reset the  instruments.
4. You must meet all the [Prerequisites](#prerequisites) to run the test program. To run the test program, click the **Start/Resume Lot** button on the TSM toolbar.
