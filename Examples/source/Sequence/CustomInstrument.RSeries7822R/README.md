# [Draft]Custom Instrument RSeries sequence Example

This example demonstrates how to use the **Custom Instrument** feature to interact with an **RSeries** card through the STL.
R Series cards are FPGA-based NI instruments whose functionality can be customized by deploying an FPGA bit file generated from your custom FPGA code.
Because R Series cards are not natively supported in TSM, you must define them as **Custom Instruments** and access them using STL’s Custom Instrument functionality.

This example demonstrate Interacting with PXIe-7822R RSeries card to do Simple digital read write operation.

## Prerequisites

- Software requirement:
STS 25.5 or later
LV FPGA (Required only for customizing FPGA host code)
Xilinx compiler / Compile worker with Xilinx cloud server (Required only if FPGA source is updated)

- Hardware Requirements:
STS hardware or PXI system with PXI-7822R

## Key Files

This example has two parts

1. Driver code for RSeries7822R (RSeries7822 driver API)
2. Example code for integrating RSeries7822 driver API

### Driver code for RSeries7822R

- FPGARSeriesExample.lvbitx - FPGA bit file generated from Custom FPGA code.
- RSeries7822R_ReadWriteDigitalPorts_CAPI.dll - Contains cAPI methods to interact with deployed FPGA code.
- RSeries7822RDriverAPI.dll - C# wrappers for the cAPI.
- RSeries Driver API source:
  - RSeries7822RDriverAPI.csproj
  - RSeriesDriver.cs
  - ImportRSeriesCAPI.cs
    - Custom FPGA Code source:
    - RSeries Example.lvproj
    - APIs/OpenFPGA.vi
    - APIs/CloseFPGA.vi
    - APIs/EnableLoopBack.vi
    - APIs/WriteData.vi
    - APIs/ReadData.vi
    - APIs/Utility/*

### Example code

## Using this Example

Open STLExample.CustomInstrument.RSeries7822R.seq file in TestStand
Verify Setup and Cleanup instruments are configured in ProcessSetup and ProcessCleanup callback sequences'
Verify that main sequence contains `SemiConductor Multi Test` step for doing simple Digital Read/Write operations.
Verify the PinMap
Run the example and ensure validation tests pass.
