# SMUGangPinGroup Sequence Example

This example demonstrates how to use the Gang Pin Group feature of the Semiconductor Test Library.

There are two scenarios demonstrated by this example:

## 1. Ganging and Unganging Pin Groups at the Code Module level

### Case A: Ganging Pin Group with 4 channels

Within the accompanying pin map file, there is one pin group named "Vcc4ch". The pins in this pin group map to four channels, each channel belonging to a different single-channel PXIe-4137 module. The pin group name is then passed as a parameter to the `SMUGangPinsFIMVThenUngang` method by the GangAndFIMV - 4 Channels step in the MainSequence of the sequence file.

### Case B: Ganging Pin Group with 2 channels

Within the accompanying pin map file, there is one pin group named "Vcc2ch". The pins in this pin group map to two channels, each channel belonging to a different single-channel PXIe-4137 module. The pin group name is then passed as a parameter to the same `SMUGangPinsFIMVThenUngang` method as before, but by the GangAndFIMV - 2 Channels step in the MainSequence of the sequence file.

## 2. Ganging and Unganging Pin Groups at the Sequence level

There are dedicated steps for each of the following operations:

- `SMUGangPinGroup`
- `SMUPowerDownAndUngangPinGroup`

These two functions are called at different places within the test program to either perform ganging dynamically within MainSequence (case C) or statically (case D) at the very start and end of testing within ProcessSetup and ProcessCleanup, respectively.

### Case C: Ganging Pin Group Dynamically at the Sequence Level

The two functions are called before and after the step `Force Voltage Measure Current (FVMI)`. It uses the same pin group name "Vcc4ch". This is considered a dynamic gang, as the ganging operation is performed only for certain steps within MainSequence, where the pins in the pin group can be individually utilized for other steps in MainSequence.

### Case D: Ganging Pin Group Statically at the Sequence Level

The same two functions are called in `ProcessSetup` and `ProcessCleanup` sequences, respectively, but target a different pin group name "Vref8ch". This demonstrates when the pins of the pin group are always configured to be ganged together throughout testing; thus, they only need to be ganged once at the very beginning of the test program and unganged at the very end of the test program. This is considered a static gang, as the ganging operation is performed only once and remains ganged throughout testing.

### Prerequisites

1. If you want to use the example, you must have the following software installed:
   - STS Software 24.5.0 or later
2. To run the example, you should also have:
   - 16 NI-DCPower instruments (NI-PXIe 4137) available, named sequentially from `SMU_4137_C1_S02` to `SMU_4137_C1_S17`, as defined in NI-MAX.
   - TestStand configured to use the Batch process model.

> **NOTE**
>
> You can view the example sequence file in the TestStand Sequence Editor and C# code source files in Visual Studio or any text editor without meeting the #2 requirement.
> To run the example, however, you must have the required instruments physically installed in your system or simulated using Offline Mode.
>
> Complete the following steps to simulate the instruments in Offline Mode:
>
> 1. Open the sequence file (SMUGangPinGroup.seq) in the TestStand Sequence Editor.
> 2. Click the Enable Offline Mode button on the TSM toolbar. To run the test sequence, click the Start/Resume Lot button on the TSM toolbar.
> 3. Click the Disable Offline Mode button to return to the default TSM behavior.

## Using the Example

Complete the following steps to use this example. You can also run this example in offline mode to see it in action.

1. Select **Semiconductor Module -> Edit Pin Map** File or click the **Edit Pin Map File** button on the TSM toolbar to open the STLExample.SMUGangPinGroup.pinmap file in the Pin Map Editor.
The pin map file defines the following:
   - 16 NI-DCPower instruments (NI-PXIe 4137) named sequentially from `SMU_4137_C1_S02` to `SMU_4137_C1_S17`.
   - Four DUT pins named `Vcc0`, `Vcc1`, `Vcc2`, and `Vcc3`.
   - Eight System pins named `Vref0`, `Vref1`, `Vref2`, `Vref3`, `Vref4`, `Vref5`, `Vref6`, and `Vref7`.
   - Three pin groups named `Vcc2ch`, `Vcc4ch`, and `Vref8ch`.
   - Two sites on the tester.
   - A series of connections for each site, in which each connection specifies a DUT pin, a site number, an instrument, and an instrument channel.
2. Complete the following steps to review the `MainSequence`, `ProcessSetup`, and `ProcessCleanup` sequences that this test program uses.
   1. On the `Sequences` pane, select the `MainSequence` sequence and review the objectives each step performs and optionally review the C#/.NET code associated with each step:
      - In the Setup & Cleanup step group, there are no steps.
      - In the Main step group, the example demonstrates:
         - Ganging & unganging at code level for the `Vcc4ch` pin group.
         - Ganging & unganging at code level for the `Vcc2ch` pin group.
         - Ganging at the sequence level for the `Vcc4ch` pin group.
         - Performing a Force Voltage and Measure Current (FVMI) operation on the ganged `Vcc4ch` pin group.
         - Unganging at the sequence level for the `Vcc4ch` pin group.
         - Performing a Force Voltage and Measure Current (FVMI) operation on the ganged `Vref8ch` pin group.
   2. On the `Sequences` pane, select the `ProcessSetup` sequence. TestStand calls this sequence once before starting testing.
      - Steps in the Setup step group initialize instruments and store the instrument sessions in the SemiconductorModuleContext.
      - In the Main step group, the example demonstrates:
         - Step level four channel ganging of pin group `Vref8ch`.
   3. On the `Sequences` pane, select the `ProcessCleanup` sequence. TestStand calls this sequence once after testing completes.
      - In the Main step group, the example demonstrates:
         - Step level four channel unganging of pin group `Vref8ch`.
      - Steps in the Cleanup step group close and reset the instruments.
3. You must meet all the [Prerequisites](#prerequisites) to run the test program. To run the test program, click the **Start/Resume Lot** button on the TSM toolbar.
