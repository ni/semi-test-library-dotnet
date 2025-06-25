
# How to use MergePinGroup() and UnmergePinGroup() function:
    - We can execute this example with the help of offline config file located under "Supporting Materials\Offline Mode Configurations" in simuation mode. 
    - The PinMap file need to run this example is located under "Supporting Materials\Pin Maps"
    - In the PinMap file, we have created two pin groups are created with SMU pins that can be merged. This example uses pins from PXIe-4147. 
    - Please refer to the user manual for requirements of the channels that are mergeable as per the PXIe 4147 (or 4162 or 4163) card specification.

    - The example `SMUMergPinGroup` sequence file next to this file calls the `SMUMergePinGroup.cs` (C# code) inside the `Code Module` folder.
    - In the C# code,  
        - We create a `dcPowerBundle` with all the pins of the pingroup to be merged in it for performing the merge operation.
        - In this example we are creating a bundle with just the those pin groups. If required, the bundle can have other pins as well.
        - We assume that the relays to connect the channels for merging operation is taken care. And a dummy relay function call is placed and by default it is skipped as we pass empty string for the relays.
        - On the Bundle we Perform the merge operation.
        - After merging the typical workflow of measurement operations are performed. i.e. configure, force, and measure. These operations are internally performed on the primary pin of the merged channel only. 
        - Once those operations are completed we can unmerge and disconnect the channels. 

    - The example performs the same operation twice, in first step it perform 2 channel merging and in second step performs 4 channel merging by passing different Pingroup names to the same function. 
    - The example also show cases sperate merge and unmerge function calls and uses the `Force Current And Measure Voltage` Step in the middle to perform operation on merged channels.
