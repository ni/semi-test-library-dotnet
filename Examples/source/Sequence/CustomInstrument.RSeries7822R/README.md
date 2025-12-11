# Custom Instrument RSeries Sequence Example

This example demonstrates how to use the **Custom Instrument** feature of the Semiconductor Test Library to interact with an R Series card through the STL.

R-Series devices are FPGA-based instruments whose functionality is defined by custom LabVIEW FPGA code deployed via a compiled FPGA bitfile. Since they are not natively supported in TSM, these devices must be specified as a Custom Instrument in the PinMap file and accessed through the STL Custom Instrument interface.

This example uses a **PXIe-7822R** R Series device to illustrate simple digital read and write operations.

## More Details

The **PXIe-7822R** R Series device provides **128 digital I/O physical channels**, organized into four connectors. Each connector is further divided into **four digital ports**, with each port offering 8-bit width.

For site 0, Connector 0 is used:

- Port 0 → _DigitalInput_A_
- Port 1 → _DigitalInput_B_
- Port 2 → _DigitalOutput_A_
- Port 3 → _DigitalOutput_B_

For site 1, Connector 1 is configured in the same way.

When **Loopback** is enabled, any data written to _DigitalInput_A_ and _DigitalInput_B_ is automatically copied to _DigitalOutput_A_ and _DigitalOutput_B_, respectively.

- `FPGARSeriesExample.lvbitx` – The FPGA bitfile deployed to the PXIe-7822R RIO device. It contains the digital read/write logic generated from the FPGA source VI (`FPGACode_ReadWriteDigitalPorts.vi`).

- `RSeries7822R_ReadWriteDigitalPorts_CAPI.dll` – A DLL exposing the C API used to interact with the deployed FPGA bitfile. It is built from the FPGA host API sources located under `Imports/source/APIs/*`

## Key Files

Below are the key files along with its purpose.

### Example files

These files represent the main code that demonstrates the custom instrument implementation for this example.

- STLExample.CustomInstrument.RSeries7822R.pinmap
- STLExample.CustomInstrument.RSeries7822R.seq
- Code Modules
  - STLExample.CustomInstrument.RSeries7822R.csproj
  - STLExample.CustomInstrument.RSeries7822R.sln
  - SetupAndCleanupSteps.cs
  - TestStep.cs
  - MyCustomInstrument/RSeries7822R.cs
  - MyCustomInstrument/RSeries7822RFactory.cs
  - MyCustomInstrument/HighLevelDriverOperations.cs
  - MyCustomInstrument/ImportRSeriesCAPI.cs

### Driver code

Driver code `RSeries7822RDriverAPI` is placed under `Imports` directory. Driver code contains FPGA bit file, dynamic linked library containing C APIs to interact with deployed bit file.

- RSeries7822R_ReadWriteDigitalPorts_CAPI.dll (C API)
- RSeries7822R_ReadWriteDigitalPorts_CAPI.h
- FPGARSeriesExample.lvbitx (bit file)
- Source
  - RSeries Example.lvproj
  - FPGACode_ReadWriteDigitalPorts.vi (FPGA source VI)
  - Debug C API.vi (VI to debug and test C APIs)
  - Debug LV API.vi (VI to debug and test LV APIs)
  - APIs (Contains source VIs built in to C API dll)
    - CloseFPGA.vi
    - EnableLoopBack.vi
    - OpenFPGA.vi
    - ReadData.vi
    - WriteData.vi

## Prerequisites

- Software Requirements:
STS 25.5 or later.
LV FPGA (Required only for customizing FPGA source code).
Xilinx compiler / Compile worker with Xilinx cloud server (Required only if FPGA source is updated).

- Hardware Requirements:
STS hardware or PXI system with PXI-7822R instrument.

## Using this Example

Open the STLExample.CustomInstrument.RSeries7822R.seq file in TestStand.

- Verify that the Setup and Cleanup instruments are correctly configured in the ProcessSetup and ProcessCleanup callback sequences.
- Confirm that the MainSequence contains a Semiconductor Multi Test step that performs basic digital read/write operations.
- Ensure that the digital write operation is configured for the digital ports (pins) DigitalInput_A and DigitalInput_B, with values 127 and 255, respectively.
- Ensure that the digital read operation is configured for the digital ports (pins) DigitalOutput_A and DigitalOutput_B, and that the returned values are validated against the expected values 127 and 255, respectively.
- Verify the PinMap configuration for all pin-to-channel connections.
- Run the example sequence and confirm that all validation tests passes for both sites '0' &'1'.
