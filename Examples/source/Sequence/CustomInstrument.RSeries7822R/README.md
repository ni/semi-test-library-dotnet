# Custom Instrument RSeries Sequence Example

This example demonstrates how to use the **Custom Instrument** feature of the Semiconductor Test Library to interact with an R Series device through the STL.

R-Series devices are FPGA-based instruments whose functionality is defined by custom LabVIEW FPGA code deployed via a compiled FPGA bitfile. Since they are not natively supported in TSM, these devices must be specified as a Custom Instrument in the PinMap file and accessed through the STL Custom Instrument interface.

This example uses a **PXIe-7822R** R Series device to illustrate simple digital read and write operations. The **PXIe-7822R** R Series device provides **128 digital I/O physical channels**, organized into four connectors. Each connector is further divided into **four digital ports**, with each port offering 8-bit width.

For site 0, Connector 0 is used:

- Port 0 → _DigitalInput_A_
- Port 1 → _DigitalInput_B_
- Port 2 → _DigitalOutput_A_
- Port 3 → _DigitalOutput_B_

For site 1, Connector 1 is configured in the same way.

The FPGA code logic which will be deployed on to the RIO device (**PXIe-7822R**) gives a provision to enable loopback mode. When loopback mode is enabled, data written to input digital ports will be copied to output digital ports.

### Sample Test: _Digital Read Write_

The Digital Read Write Test provided by the example sources different values to the digital input port and measures the values at digital output port to ensure the measured values match the input values provided. When loopback mode is enabled, the test will pass without the need for external connections.

> **NOTE**:For demonstrations purposes this example is expected to be run with Loopback mode enabled by default

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
  - MyCustomInstrument/RSeries7822RDriverAPI.cs

### Driver code

These files represent the driver used to control the PXIe-7822R R Series device and are placed under Imports directory. The driver consists of a C API (`RSeries7822RDriverAPI.dll`) for interacting with a PXIe-7822R device via a compiled FPGA bit file (`RSeries7822R_ReadWriteDigitalPorts.lvbitx`).

- RSeries7822RDriverAPI.dll – A DLL exposing the C API used to interact with the deployed FPGA bitfile. It is built from the FPGA host API sources located under `Imports/source/APIs/*`
- RSeries7822RDriverAPI.h - header file containing method signatures.
- RSeries7822R_ReadWriteDigitalPorts.lvbitx – The FPGA bitfile deployed to the PXIe-7822R RIO device. It contains the digital read/write logic generated from the FPGA source VI (`FPGACode_ReadWriteDigitalPorts.vi`).

- Source
  - RSeries7822RDriverAPI.lvproj
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

1. If you want to use the example you must have the following software installed:
    - STS Software 24.5.0 or later
1. To run the example you must also have:
    - A PXIe-7822R instrument with an alias of 'RIO_7822R_C1_S06' defined in NI MAX.
    - TestStand configured to use the Batch process model.
1. To open, view, and compile the LabVIEW source files (GitHub only), you must have:
    - LabVIEW FPGA Module
    - Xilinx compiler / Compile worker with Xilinx cloud server

> **NOTE**:  
> You can view the example sequence file in the TestStand Sequence Editor and C# code source files in Visual Studio or any text editor without meeting the #2 requirement. To run the example though, you must have the required instruments physically installed in your system.

## Using this Example

Open the STLExample.CustomInstrument.RSeries7822R.seq file in TestStand and complete the following steps to use this example. You can also run this example in offline mode to see it in action.

- **Select Semiconductor Module -> Edit Pin Map File...** or click the Edit Pin Map File button on the TSM toolbar. The pin map file defines the following information:
  - R Series instrument `PXIe-7822R` with alias name: `RIO_7822R_C1_S06` Instrument Type Id: `PXI_RSeries_7822R`, Instrument Type:`Custom Instrument`
  - No of sites: 2; 0 & 1.
  - DUT pins: `DigitalInput_A`, `DigitalInput_B`, `DigitalOutput_A`, `DigitalOutput_B`
  - Connections:  
  For site '0', connector 1 ports are used for site '1' connector 2 ports are used.
    - `DigitalInput_A` --> `DIOPORT0`
    - `DigitalInput_B` --> `DIOPORT1`
    - `DigitalOutput_A`--> `DIOPORT2`
    - `DigitalOutput_B`--> `DIOPORT2`
- On the Sequences pane, select the **ProcessSetup sequence**. TestStand calls this sequence at the start of the testing.
  - Select `Setup RSeries7822R Instrumentation` step and ensure `enableLoopBackConfiguration` is set to default (true). If you are running with external physical loopback connections, then select 'false'.
- On the Sequences pane, select the **ProcessCleanup sequence**. TestStand calls this sequence at the end of the testing.
  - Ensure `Cleanup RSeries7822R Instrumentation` step is configured.
- On the Sequences pane, select the **MainSequence sequence**. This contains the actual test steps.
  - Ensure `Digital Read Write Test` is configured with following arguments.
    - `digitalInputPins`
    - `digitalOutputPins`
    - `pinData` {'127', '255'}
    - `publishedDataID`
  - Navigate to **Tests** tab and ensure data on the output are validated.
    - DigitalOutput_A validated against '127'.
    - DigitalOutput_B validated against '255'.
- To run the test program, click the **Start/Resume Lot** button on the TSM toolbar. You must meet all the [Prerequisites](#prerequisites) before running the test program.
