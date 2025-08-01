# Semiconductor Test Library - SMU Merge PinGroup Example 

## What does this example show?
Demonstrates the use of the SMU MergePinGroup and UnmergePinGroup methods provided by the Semiconductor Test Library to merge and unmerge smu pins defined as pingroup in pinmap file. After merging we can source more current than individual channel can provide from a dcpower instrument.

## How to run this Example with Simulated hardware. 
Simulate PXIe-4147 in NIMax and rename the simulated card as per the resource name in the pinmap file. After that build and run this example. When prompted for merging press number 4 for four channel merging or any other key for 2 channel merging. 

## What are the prerequisites for running this example?
- STS Software 24.5.0 or later
- .NET Framework 4.0 or 4.5.
- A compatible multichannel SMU instrument (e.g., NI PXIe-4147, NI PXIe-4162, or NI PXIe-4163).

Where can I find more information?
<Public Documents>\National Instruments\NI_SemiconductorTestLibrary\Documentation