# Custom Instrument RSeries Sequence Example

This example demonstrates how to use the **Custom Instrument** feature of the Semiconductor Test Library (STL) to interact with an R Series device.

R-Series devices are FPGA-based instruments whose functionality is defined by custom LabVIEW FPGA code deployed via a compiled FPGA bitfile. Since they are not natively supported by TSM, these devices must be specified as a Custom Instrument in the Pin Map file and accessed through an STL Custom Instrument interface.

This example uses a **PXIe‑7822R** R Series device to demonstrate basic digital read and write operations. The **PXIe‑7822R** provides **128 digital I/O physical channels** organized across four connectors. Each connector is further divided into **four digital ports**, with each port containing eight channels (8-bit width). Two of the four connectors are used by the example, one for each site.

For site 0, Connector 0 is used:

| Port # | Channels | Port Function  | DUT Pins                          |
| ------ | -------- | -------------- | --------------------------------- |
| Port 0 | 0:7      | Digital Output | DigitalInput_A0:DigitalInput_A7   |
| Port 1 | 8:15     | Digital Output | DigitalInput_B0:DigitalInput_B7   |
| Port 2 | 16:23    | Digital Input  | DigitalOutput_A0:DigitalOutput_A7 |
| Port 3 | 24:31    | Digital Input  | DigitalOutput_B0:DigitalOutput_B7 |

For site 1, Connector 1 is used and configured the same way.

The FPGA code logic, which is deployed onto the RIO device (**PXIe-7822R**), provides the ability to enable loopback mode. When loopback mode is enabled, data written to device's digital output ports is internally routed to its digital input ports.

### Sample Test: Digital Read Write

The **Digital Read Write Test** provided by the example writes different values to a DUT's digital input pins and reads the values from DUT's digital output pins to verify that the values match.

> **NOTE**: For demonstration purposes, this example is expected to be run with loopback mode enabled by default.

## Key Files

Below are the key files along with their purpose.

### Example files

These files represent the main code that demonstrates the custom instrument implementation for this example.

- STLExample.CustomInstrument.RSeries.pinmap
- STLExample.CustomInstrument.RSeries.seq
- Code Modules
  - STLExample.CustomInstrument.RSeries.csproj
  - STLExample.CustomInstrument.RSeries.sln
  - SetupAndCleanupSteps.cs
  - TestStep.cs
  - RSeries7822RCustomInstrument/RSeries7822R.cs
  - RSeries7822RCustomInstrument/RSeries7822RFactory.cs
  - RSeries7822RCustomInstrument/HighLevelDriverOperations.cs
  - RSeries7822RCustomInstrument/RSeries7822RDriverAPI.cs

### Driver code

These files represent the driver used to control the PXIe-7822R R Series device and are placed under the Imports directory. The driver consists of a C API (`RSeries7822RDriverAPI.dll`) for interacting with a PXIe-7822R device via a compiled FPGA bit file (`RSeries7822R_ReadWriteDigital.lvbitx`).

- RSeries7822RDriverAPI.dll – A DLL exposing the C API used to interact with the deployed FPGA bitfile.
- RSeries7822RDriverAPI.h - header file containing method signatures.
- RSeries7822R_ReadWriteDigital.lvbitx – The FPGA bitfile deployed to the PXIe-7822R RIO device.

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
> You can view the example sequence file in the TestStand Sequence Editor, and C# code source files in Visual Studio or any text editor, without meeting the #2 requirement. To run the example, though, you must have the required instruments physically installed in your system.
> Users can also run the example using physical connections made externally, but the sample test will only pass if the output ports are physically looped back to the input ports.

## Using this Example

Open the STLExample.CustomInstrument.RSeries7822R.seq file in TestStand and complete the following steps to use this example.

- Review the pin map from the TestStand Sequence Editor by selecting **Semiconductor Module -> Edit Pin Map File...** or clicking the Edit Pin Map File button on the TSM toolbar. The pin map file defines the following information:
  - R Series instrument `PXIe-7822R` with alias name: `RIO_7822R_C1_S06` Instrument Type Id: `PXI_RSeries_7822R`, Instrument Type: `Custom Instrument`
  - Two sites: 0 & 1
  - DUT pin groups: `DigitalInput_A`, `DigitalInput_B`, `DigitalOutput_A`, `DigitalOutput_B`
    - Each pin group contains 8 DUT pins and represents a specific digital port of the DUT.
  - Connections:  
    - Channels from each digital port of the DUT are mapped to corresponding channels of each digital port of the R series device.
    - For site 0, all four ports from connector 0 are used. For site 1, all four ports from connector 1 are used.
      - DUT pins in the `DigitalInput_A` pin group map directly to the instrument channels of the R series device's `Port0`.
      - DUT pins in the `DigitalInput_B` pin group map directly to the instrument channels of the R series device's `Port1`.
      - DUT pins in the `DigitalOutput_A` pin group map directly to the instrument channels of the R series device's `Port2`.
      - DUT pins in the `DigitalOutput_B` pin group map directly to the instrument channels of the R series device's `Port3`.
- On the Sequences pane, select the **ProcessSetup** sequence. TestStand calls this sequence once at the start of testing.
  - Select `Setup RSeries7822R Instrumentation` step and note that the `enableLoopBackConfiguration` parameter is set to default (true). If you are using external physical connections, select `false`.
- On the Sequences pane, select the **ProcessCleanup** sequence. TestStand calls this sequence once at the end of testing.
  - Note that the `Cleanup RSeries7822R Instrumentation`  step is called from within this sequence.
- On the Sequences pane, select the **MainSequence** sequence. TestStand calls this sequence in separate threads for each site, and loops over it for each batch in the lot or until testing ends.
  - This is where the `Digital Read Write Test` step is called from.
  - Select the `Digital Read Write Test` step to view the Module tab of step's Step Settings pane. Note that the step parameters are configured with the following arguments, which are defined by local variables declared in the Variables tab.
    - `dutDigitalInputPorts = Locals.DutDigitalInputPorts` -> `"{DigitalInput_A, DigitalInput_B}"`
    - `dutDigitalOutputPorts = Locals.DutDigitalOutputPorts` -> `"{DigitalOutput_A, DigitalOutput_B}"`
    - `portData = Locals.PortData` -> `"{127, 255}"`
  - Navigate to Tests tab from within the Step Settings pane. Note that there are two test items, one for each digital output port, evaluated against their respective limits.
    - The `ValidateDigitalOutput A` test is evaluated against limits surrounding the expected '127' value.
    - The `ValidateDigitalOutput B` test is evaluated against limits surrounding the expected '255' value.
- To run the test program, click the **Start/Resume Lot** button on the TSM toolbar. You must meet all the [Prerequisites](#prerequisites) before running the test program.
