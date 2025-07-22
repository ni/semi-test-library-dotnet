
# MergePinGroup Sequence Example:

This example demonstrates how to use the Merge Pin Group feature of the Semiconductor Test Library.
 
There are two scenarios demonstrated by this example:
## 1. Merging and Unmerging Pin Groups at the Code Module level.

### - Case A: Merging Pin Group with 4 channels.

This is achived by the pingroup "Vcc4ch" defined in the pinmap file. This pingroup pins maps to all four channels of PXIe-4147 module. First pin must map to channel 0. This group name is passed as parameter to the function `MergePinsFIMVThenUnmerge`. 

### - Case B: Merging Pin Group with 2 channels.

This is achived by the pingroup "Vcc2ch0" or "Vcc2ch2" defined in the pinmap file. This pingroup pins maps to two mergeable channels of PXIe-4147 module. First pin must map to even number channels like 0,2. Second pin must be its immediate next channel of the SMU. This group name is passed as parameter to the function `MergePinsFIMVThenUnmerge`.


## 2. Merging and Unmerging Pin Groups at the Sequence level.
### We have created dedicated steps just for the following operations:
- `Merge`
- `Powerdown and unmerge`

These two functions are called before and after the standard shipping step `Force Voltage Measure Current (FVMI)`. It uses the same pin group name "Vcc4ch". This way standard steps can benefit from the merging. 


 
## Prerequisites: 
You must have the following software installed if you want
to use the example with real instruments or in Offline Mode:
- STS Software 24.5.0 or later
- This example be default makes use of the offline config file located next to the sequence file. 