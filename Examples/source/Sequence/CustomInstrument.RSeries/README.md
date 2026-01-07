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

> **NOTE**: For demonstration purposes, this example is expected to be run with loopback mode enabled by default.

## Key Files

Below are the key files along with their purpose.

### Example files

These files represent the main code that demonstrates the custom instrument implementation for this example.

- STLExample.CustomInstrument.RSeriesDevice.pinmap
- STLExample.CustomInstrument.RSeriesDevice.seq
- Code Modules
  - STLExample.CustomInstrument.RSeriesDevice.csproj
  - STLExample.CustomInstrument.RSeriesDevice.sln
  - SetupAndCleanupSteps.cs
  - TestStep.cs
  - RSeries7822RCustomInstrument/RSeries7822R.cs
  - RSeries7822RCustomInstrument/RSeries7822RFactory.cs
  - RSeries7822RCustomInstrument/HighLevelDriverOperations.cs
  - RSeries7822RCustomInstrument/RSeries7822RDriverAPI.cs

### Driver code

These files represent the driver used to control the PXIe-7822R R Series device and are placed under Imports directory. The driver consists of a C API (`RSeries7822RDriverAPI.dll`) for interacting with a PXIe-7822R device via a compiled FPGA bit file (`RSeries7822R_ReadWriteDigitalPorts.lvbitx`).

- RSeries7822RDriverAPI.dll – A DLL exposing the C API used to interact with the deployed FPGA bitfile.
- RSeries7822RDriverAPI.h - header file containing method signatures.
- RSeries7822R_ReadWriteDigitalPorts.lvbitx – The FPGA bitfile deployed to the PXIe-7822R RIO device.

- Source
  - RSeries7822RDriverAPI.lvproj
  - FPGACode_ReadWriteDigitalPorts.vi (LabVIEW FPGA source VI implementing the core digital read/write logic)
  - Debug C API.vi (VI to debug and test C API)
  - Debug LV API.vi (VI to debug and test LV API)
  - APIs (LabVIEW source VIs used to build the FPGA host C API, `RSeries7822RDriverAPI.dll`)
    - CloseFPGA.vi
    - EnableLoopBack.vi
    - OpenFPGA.vi
    - ReadData.vi
    - WriteData.vi

## Prerequisites

1. If you want to use the example you must have the following software installed:
    - STS Software 24.5.0 or later
1. To run the example you must also have:
    - A physical PXIe-7822R instrument with an alias of 'RIO_7822R_C1_S06' defined in NI MAX.
    - TestStand configured to use the Batch process model.
1. To open, view, and compile the LabVIEW source files (GitHub only), you must have:
    - LabVIEW FPGA Module
    - Xilinx compiler / Compile worker with Xilinx cloud server

> **NOTE**:  
> You can view the example sequence file in the TestStand Sequence Editor and C# code source files in Visual Studio or any text editor without meeting the #2 requirement. To run the example though, you must have the required instruments physically installed in your system.
> Users can also run the example using physical connections made externally, but the sample test will only pass if the output ports are physically looped back to the input ports.

## Using this Example

Open the STLExample.CustomInstrument.RSeries7822R.seq file in TestStand and complete the following steps to use this example.

- Review the pin map from the TestStand Sequence Editor by selecting **Semiconductor Module -> Edit Pin Map File...** or clicking the Edit Pin Map File button on the TSM toolbar. The pin map file defines the following information:
  - R Series instrument `PXIe-7822R` with alias name: `RIO_7822R_C1_S06` Instrument Type Id: `PXI_RSeries_7822R`, Instrument Type:`Custom Instrument`
  - Two sites: 0 & 1
  - DUT pins: `DigitalInput_A`, `DigitalInput_B`, `DigitalOutput_A`, `DigitalOutput_B`
  - Connections:  
  For site '0', all four ports from connector 1 are used. For site '1', all four ports from connector 2 are used."
    - `DigitalInput_A` --> `DIOPORT0`
    - `DigitalInput_B` --> `DIOPORT1`
    - `DigitalOutput_A`--> `DIOPORT2`
    - `DigitalOutput_B`--> `DIOPORT3`
- On the Sequences pane, select the **ProcessSetup** sequence. TestStand calls this sequence once at the start of testing.
  - Select `Setup RSeries7822R Instrumentation` step and note that the `enableLoopBackConfiguration` parameter is set to default (true). If you are using external physical connections, select `false`.
- On the Sequences pane, select the **ProcessCleanup** sequence. TestStand calls this sequence once at the end of testing
  - Note that the `Cleanup RSeries7822R Instrumentation`  step is called from within this sequence.
- On the Sequences pane, select the **MainSequence** sequence. TestStand calls this sequence in separate threads for each site, and loops over it for each batch in the lot or until testing ends.
  - This is where the `Digital Read Write Test` step is called from.
  - Select the `Digital Read Write Test` step to view the Module tab of step's Step Settings pane. Note that the step parameters are configured with the following arguments, which are defined by Local variables declared in the Variables tab.
    - `digitalInputPins = Locals.DigitalInputPins` -> `"{DigitalInput_A, DigitalInput_B}"`
    - `digitalOutputPins = Locals.DigitalOutputPins` -> `"{DigialOutput_A, DigitalOuput_B}"`
    - `pinData = Locals.PinData` -> `"{127, 255}"`
    - `publishedDataId = Locals.PublishedDataId` -> `"DigitalOutData"`
  - Navigate to Tests tab from within the Step Settings pane. Note that there are two test items, one for each digital output pin, validated against their separate limits.
    - DigitalOutput_A validated against '127'.
    - DigitalOutput_B validated against '255'.
- To run the test program, click the **Start/Resume Lot** button on the TSM toolbar. You must meet all the [Prerequisites](#prerequisites) before running the test program.
