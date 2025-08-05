# SMUMergePinGroup Sequence Example

This example demonstrates how to use the Merge Pin Group feature of the Semiconductor Test Library.

There are two scenarios demonstrated by this example:

## 1. Merging and Unmerging Pin Groups at the Code Module level

### Case A: Merging Pin Group with 4 channels

Within the accompanying pin map file, there is one pin group named "Vcc4ch". The pins in this pin group map to four channels of a twenty four channel PXIe-4163 module. Note that the first pin in the pin group is considered the primary channel when performing the merge operation, in this case channel 0 for site 0 and channel 8 for site 1. The pin group name is then passed as parameter to the `MergePinsFIMVThenUnmerge` method by the MergeAndFIMV - 4 Channels step in the MainSequence of the sequence file.

### Case B: Merging Pin Group with 2 channels

Within the accompanying pin map file, there is one pin group named "Vcc2ch". The pins in this pin group map to two channels of a twenty four channel PXIe-4163 module. Note that the first pin in each of these pin groups is considered the primary channel when performing the merge operation, in this case channels 0 for site 0 and channels 8 for site 1 respectively. The pin group name is then passed as parameter to the same `MergePinsFIMVThenUnmerge` method as before, but by the MergeAndFIMV - 2 Channels step in the MainSequence of the sequence file.

## 2. Merging and Unmerging Pin Groups at the Sequence level

There are dedicated steps for each of the following operations:

- `SMUMergePinGroup`
- `SMUPowerDownAndUnmergePinGroup`

These two functions are called at different places within the test program to either perform merging dynamically within MainSequence (case C) or statically (case D) at the very start and end of testing within ProcessSetup and ProcessCleanup, respectively.

### Case C: Merging Pin Group Dynamically at the Sequence Level

The two functions are called before and after the step `Force Voltage Measure Current (FVMI)`. It uses the same pin group name "Vcc4ch0". This is considered dynamic merge as the merging operation is performed only for certain steps within MainSequence, where the pins in pin group can be individually utilized for other steps in MainSequence.

### Case D: Merging Pin Group Statically at the Sequence Level

The same two functions are called in `ProcessSetup` and `ProcessCleanup` sequences, respectively, but target a different pin group name "Vref8ch0". This demonstrates when the pins of the pin group are aways configured to be merged together throughout testing, thus they only need to be merged once at the very beginning of the test program and unmerged at the very end of test program. This is considered a static merge as the merging operation is performed only once and remain merged throughout testing.

### Prerequisites

1. If you want to use the example you must have the following software installed:
   - STS Software 24.5.0 or later
2. To run the example you must also have:
   - A NI-DCPower instrument named SMU_4163_C1_S04 as defined in MAX.
   - TestStand configured to use the Batch process model.

> **NOTE**
>
> You can view the example sequence file in the TestStand Sequence Editor and C# code source files in Visual Studio or any text editor without meeting the #2 requirement.
> To run the example though, you must have the required instruments physically installed in your system or simulated using Offline Mode.
>
> Complete the following steps to simulate the instruments in Offline Mode:
>
> 1. Open the sequence file (.seq) in the TestStand Sequence Editor.
> 2. Click the Enable Offline Mode button on the TSM toolbar. To run the test sequence, click the Start/Resume Lot button on the TSM toolbar.
> 3. Click the Disable Offline Mode button to return to the default TSM behavior.

## Using the Example

Complete the following steps to use this example. You can also run this example in offline mode to see it in action.

1. Select **Semiconductor Module -> Edit Pin Map** File or click the **Edit Pin Map File** button on the TSM toolbar to open the STLExample.MergePinGroup.pinmap file in the Pin Map Editor.
The pin map file defines the following information:
   - One NI-DCPower instrument named `SMU_4163_C1_S04`.
   - Four DUT pins named `Vcc0`, `Vcc1`, `Vcc2` and `Vcc3`.
   - Eight System pins named `Vref0`, `Vref1`, `Vref2`, `Vref3`, `Vref4`, `Vref5`, `Vref6` and `Vref7`.
   - Three pin groups named `Vcc2ch`, `Vcc4ch` and `Vref8ch`.
   - Two sites on the tester.
   - A series of connections for each site, in which each connection specifies a DUT pin, a site number, an instrument, and an instrument channel.
2. Complete the following steps to review the `MainSequence`, `ProcessSetup`, and `ProcessCleanup` sequences that this test program uses.
   1. On the `Sequences` pane, select the MainSequence sequence and review the objectives each step performs and optionally review the C#/.NET code associated with each step:
      - In the Setup & Cleanup step group, there are no steps.
      - In the Main step group, the example demonstrates:
         - Four channel merging for `Vcc4ch` pingroup & unmerging at code level.
         - Two channel merging for `Vcc2ch` & unmerging at code level.
         - Step level four channel merging of pin group `Vcc4ch`.
         - calling shipping step FVMI on the pingroup `Vcc4ch`.
         - Step level unmerging of pingroup.
         - calling shipping step FVMI on the pingroup  `Vref8ch`.
   2. On the `Sequences` pane, select the `ProcessSetup` sequence. TestStand calls this sequence once before starting testing.
      - Steps in the Setup step group initialize instruments and store the instrument sessions in the SemiconductorModuleContext.
      - In the Main step group, the example demonstrates:
         - Step level four channel merging of pin group `Vref8ch`.
   3. On the `Sequences` pane, select the `ProcessCleanup` sequence. TestStand calls this sequence once after testing completes.
      - In the Main step group, the example demonstrates:
         - Step level four channel unmerging of pin group `Vref8ch`.
      - Steps in the Cleanup step group close and reset the instruments.
3. You must meet all the [Prerequisites](#prerequisites) to run the test program. To run the test program, click the **Start/Resume Lot** button on the TSM toolbar.
