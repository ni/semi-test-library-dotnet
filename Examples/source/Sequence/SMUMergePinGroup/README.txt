# SMUMergePinGroup Sequence Example:

This example demonstrates how to use the Merge Pin Group feature of the Semiconductor Test Library.
 
There are two scenarios demonstrated by this example:
## 1. Merging and Unmerging Pin Groups at the Code Module level.

### - Case A: Merging Pin Group with 4 channels.

Within the accompanying pin map file, there is one pin group named "Vcc4ch". The pins in this pin group map to four channels of a four channel PXIe-4147 module. Note that the first pin in the pin group is considered the primary channel when performing the merge operation, in this case channel 0. The pin group name is then passed as parameter to the same `MergePinsFIMVThenUnmerge` method as before, but by the MergeAndFIMV - 4 Channels step in the MainSequence of the sequence file.

### - Case B: Merging Pin Group with 2 channels.

Within the accompanying pin map file, there are two pin groups named "Vcc2ch0" and"Vcc2ch2", respectively. The pins in this pin group map to two channels of a four channel PXIe-4147 module. Note that the first pin in the pin group is considered the primary channel when performing the merge operation, in this case channel 0 and channel 2 respectively. The pin group name is then passed as parameter to the same `MergePinsFIMVThenUnmerge` method as before, but by the MergeAndFIMV - 2 Channels step in the MainSequence of the sequence file.


## 2. Merging and Unmerging Pin Groups at the Sequence level.
### We have created dedicated steps just for the following operations:
- `SMUMergePinGroup`
- `SMUPowerDownAndUnmergePinGroup`

These two functions are called before and after the standard shipping step `Force Voltage Measure Current (FVMI)`. It uses the same pin group name "Vcc4ch". This way standard steps can benefit from the merging. 

### Prerequisites

1. If you want to use the example you must have the following software installed:
   - STS Software 24.5.0 or later
2. To run the test program you must also have:
   - A NI-DCPower instrument named SMU_4147_C1_S04 as defined in MAX.
   - TestStand configured to use the Batch process model.

> **NOTE**
> You can view the test program in the TestStand Sequence Editor and code modules in a C# source code editor without meeting the #2 requirement.
> To run the example you must have the required instruments physically installed in your system, or simulated using Offline Mode. 
>
> Complete the following steps to simulate the instruments in Offline Mode:
> 1. Click the Enable Offline Mode button  on the TSM toolbar. To run the test program, click the Start/Resume Lot button on the TSM toolbar.
> 2. Launch the Test Program Editor and select the Offline Mode panel to view the path to the Offline Mode system configuration file TSM uses to create simulated instruments for STLExample.SMUMergePinGroup.seq.
> 3. Click the Disable Offline Mode button to return to the default TSM behavior.


## Using the Example

Complete the steps in the following sections to learn about the test program components. You can also run this example in offline mode to see it in action.

1. Select **Semiconductor Module » Edit Pin Map** File or click the **Edit Pin Map File** button on the TSM toolbar to open the STLExample.MergePinGroup.pinmap file in the Pin Map Editor.
The pin map file defines the following information:
   - One NI-DCPower instrument named `SMU_4147_C1_S04`.
   - Four DUT pins named `Vcc0`, `Vcc1`, `Vcc2` and `Vcc3`. 
   - Three pin groups named `Vcc2ch0`, `Vcc2ch2`, and `Vcc4ch0`.
   - One site on the tester.
   - A series of connections for each site, in which each connection specifies a DUT pin, a site number, an instrument, and an instrument channel.
2. Complete the following steps to review the Test Program Configurations that this test program uses.
   1. Select **Semiconductor Module » Edit Test Program: SMUMergePinGroup.seq** or click the **Edit Test Program: SMUMergePinGroup.seq** button on the TSM toolbar.
   2. Select the Configuration Definition panel.
3. Complete the following steps to review the `MainSequence`, `ProcessSetup`, and `ProcessCleanup` sequences that this test program uses.
   1. On the `Sequences` pane, select the MainSequence sequence and review the objectives each step performs and optionally review the LabVIEW code associated with each step:
      - In the Setup & Cleanup section, there are no steps.
      - In the Main section, the test program Performs 
         - Four channel merging for 10A & unmerging at code level.
         - Two channel merging for 5A & unmerging at code level.
         - Step level four channel merging.
         - calling shipping step FVMI, with current limit of 12A.
         - Step level unmerging of pingroup.
   2. On the `Sequences` pane, select the `ProcessSetup` sequence. TestStand calls this sequence once before starting testing. The steps in this sequence initialize the instruments and store the instrument sessions in the SemiconductorModuleContext.
   3. On the `Sequences` pane, select the `ProcessCleanup` sequence. TestStand calls this sequence once after testing completes. The steps in this sequence close and reset the instruments.

5. You must meet all the [Prerequisites](#prerequisites) to run the test program. To run the test program, click the **Start/Resume Lot** button on the TSM toolbar.
