# Accelerometer Example Test Program (modified for STL)

## Introduction

This example is a modified version of the accelerometer example that ships with the NI TestStand Semiconductor Module (TSM), and has been updated to use the Semiconductor Test Library (STL).
The example demonstrates several features of TSM and STL in the context of a test program designed to test an imagined accelerometer part, up to four sites.

### Highlighted Features

- Pin Map
- Multi Site
- Binning
- Specifications
- Limits Files
- Test Program Configurations
- Virtual Pins
- Offline Mode

### Major APIs

- TSM Code Module API
- Semiconductor Test Library

### Prerequisites

1. If you want to use the example you must have the following software installed:
   - STS Software 24.5.x
2. To run the test program you must also have:
   - Two NI-Digital Pattern instruments named HSD_6570_C1_S02 and HSD_6570_C1_S04, respectively, as defined in Measurement & Automation Explorer (MAX).
   - Two NI-DCPower instruments named SMU_4143_C1_S06 and SMU_4143_C1_S07, respectively, as defined in MAX.
   - An NI-SCOPE instrument named SCOPE_5105_C1_S08 as defined in MAX.
   - A PXI-2567 relay driver module named RELAY_2567_C1_S09, as defined in MAX.
   - TestStand configured to use the Batch process model.

> **NOTE** \
> You can view the test program in the TestStand Sequence Editor and code modules in a C# source code editor without meeting the #2 requirement.
> To run the example you must have the required instruments physically installed in your system, or simulated using Offline Mode.
>
> Complete the following steps to simulate the instruments in Offline Mode:
>
> 1. Click the Enable Offline Mode button  on the TSM toolbar. To run the test program, click the Start/Resume Lot button on the TSM toolbar.
> 2. Launch the Test Program Editor and select the Offline Mode panel to view the path to the Offline Mode system configuration file TSM uses to create simulated instruments for Accelerometer.seq.
> 3. Click the Disable Offline Mode button to return to the default TSM behavior.

## Using the Example

Complete the steps in the following sections to learn about the test program components. You can also run this example in offline mode to see it in action.

1. Select **Semiconductor Module » Edit Pin Map** File or click the **Edit Pin Map File** button on the TSM toolbar to open the Accelerometer pin map file in the Pin Map Editor.
The pin map file defines the following information:
   - Two NI-Digital Pattern instruments named `HSD_6570_C1_S02` and `HSD_6570_C1_S04`. Both instruments belong to the same group so that code modules can access all digital pins on the tester using a single instrument session.
   - Two NI-DCPower instruments named `SMU_4143_C1_S06` and `SMU_4143_C1_S07`.
   - One NI-SCOPE instrument named `SCOPE_5105_C1_S08`.
   - One PXI-2567 relay driver module named `RELAY_2567_C1_S09`.
   - Ten DUT pins named `Vcc`, `Gnd`, `SCLK`, `MOSI`, `MISO`, `CS`, `RST`, `MODE`, `Vref_DIO`, and `Vref_OScope`. The `Vref_DIO` and `Vref_OScope` pins are virtual pins that refer to a single `Vref` DUT pin and are used to connect the pin to two different types of instruments, NI-Digital Pattern and NI-SCOPE.
   - One relay named `SCOPE_ENABLE_RELAY`per site. The test program uses the `SCOPE_ENABLE_RELAY` relay to control a physical relay that connects the `Vref` DUT pin to the NI-Digital Pattern instrument or to the NI-SCOPE instrument.
   - One relay named `NOISE_ENABLE_RELAY` per site. The test program uses the `NOISE_ENABLE_RELAY` relay to control a physical relay that connects the `Vref` DUT pin to a noise source, rather than to the NI-Digital Pattern or NI-SCOPE instruments.
   - Three pin groups named `SPI_Port`, `Digital`, and `AllDUTPins`.
   - One system relay named `POWER_RELAY`. The test program uses the `POWER_RELAY` relay to control a physical relay that controls a power source.
   - Four sites on the tester.
   - A series of connections for each site, in which each connection specifies a DUT pin, a site number, an instrument, and an instrument channel.
   - Site relay connections that specify to which control line of a relay driver module the `SCOPE_ENABLE_RELAY` relay is connected for a given site.
   - Site relay connections that specify to which control line of a relay driver module the `NOISE_ENABLE_RELAY` relay is connected for a given site.
   - A system relay connection that specifies whether the power source connected to the `POWER_RELAY` relay is enabled.
2. Complete the following steps to review the Test Program Configurations that this test program uses.
   1. Select **Semiconductor Module » Edit Test Program: Accelerometer.seq** or click the **Edit Test Program: Accelerometer.seq** button on the TSM toolbar.
   2. Select the Configuration Definition panel.
   3. This test program specifies two test conditions that each test program configuration must define:
       - `TestFlowId`—Defines an identifying name for the test flow.
       - `TestTemperature`—Defines the temperature at which to perform the tests.
   4. Select each of the individual Configuration panels to review the values each test program configuration gives to the specified test conditions.
3. Use the TestStand Sequence Editor to review the bin definitions file associated with the test program. Select **Semiconductor Module » Edit Bin Definitions File** or click the **Edit Bin Definitions File** button on the TSM toolbar.
   - The bin definitions file defines software bins that the test program uses and the hardware bins associated with the software bins.
4. Complete the following steps to review the `MainSequence`, `ProcessSetup`, and `ProcessCleanup` sequences that this test program uses.
   1. On the `Sequences` pane, select the MainSequence sequence and review the objectives each step performs and optionally review the LabVIEW code associated with each step:
      - In the Setup section, if the current test program configuration uses the Hot test temperature setting, the test program waits until the temperature controller reaches the specified temperature.
      - To prepare for digital tests, the test program configures the relay for the `Vref` pin to the Digital Pattern instrument.
      - The test program tests continuity, leakage, and idle power consumption on all digital pins.
      - The test program resets the DUT in preparation for SPI port communication.
      - The test program enables test mode on the DUT using the SPI port.
      - The test program checks the part number of the DUT by reading a register through the SPI port.
      - If the current TestFlowId is set to `Quality` the test program checks the part number at different Vcc levels.
      - To prepare for analog tests, the test program configures the relay for the `Vref` pin to the NI-SCOPE instrument.
      - The test program checks the minimum, maximum, and RMS voltage value on the Vref and uses the value to trim each DUT.
      - The test program sets and verifies the Vref register on the DUT based on the previous Vref measurement.
      - In the Cleanup section, the test program turns off all instrument output to the DUT in preparation for physical binning by the handler.
   2. On the `Sequences` pane, select the `ProcessSetup` sequence. TestStand calls this sequence once before starting testing. The steps in this sequence initialize the instruments and store the instrument sessions in the SemiconductorModuleContext. There are other steps in this sequence to configure a temperature controller, and to toggle the power source.
   3. On the `Sequences` pane, select the `ProcessCleanup` sequence. TestStand calls this sequence once after testing completes. The steps in this sequence close and reset the instruments.
5. Select **Semiconductor Module » Launch Digital Pattern Editor** or click the **Launch Digital Pattern Editor** button on the TSM toolbar to open the Digital Pattern Editor. Open the `<TestStand Public>\Examples\NI_SemiconductorModule\Accelerometer\DotNET\Accelerometer.digiproj` digital pattern project file in the Digital Pattern Editor. Use the Digital Pattern Editor to review the following files the Accelerometer test program uses:
   - `Accelerometer.specs` — Defines a set of variables and associated numeric values that you can reference in pin levels, time sets, other specifications files, and Shmoo operations.
   - `Accelerometer.digitiming` — Defines configuration components of the time sets, including the format and edge placement that shape the digital waveform on a per-pin basis.
   - `Accelerometer.digilevels` — Defines voltage levels for digital pins and pin groups connected to a Digital Pattern Instrument and for pins and pin groups connected to an NI-DCPower instrument.
   - `SPI - Read Part Number.digipat` — Pattern that reads the part number register from the DUT.
   - `SPI - Set Test Mode.digipat` — Pattern that sets the test mode by setting a register on the DUT.
   - `SPI - Set Vref Value.digipat` — Pattern that sets the Vref register on the DUT using a source waveform and reads it back using a capture waveform.
   - `Set Vref Value Waveform.tdms` — Source waveform configuration used to set the Vref register value.
   - `Get Vref Value Waveform.digicapture` — Capture waveform configuration used to obtain the Vref register value.
6. Use a text editor or spreadsheet software to open and review the `<TestStand Public>\Examples\NI_SemiconductorModule\Accelerometer\DotNET\Limits\Production Limits.txt` file, which is the tests limits file for the Production configuration of this test program. The test limits file is loaded at run time based on the current test configuration. This file specifies the values to use to evaluate whether a measurement passes or fails. The test program stores the limits loaded from the file with the results.
7. You must meet all the [Prerequisites](#prerequisites) to run the test program. To run the test program, click the **Start/Resume Lot** button on the TSM toolbar.
