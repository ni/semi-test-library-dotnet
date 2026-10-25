# Register IO (SPI and I2C) Sequence Example

This example demonstrates how to access DUT registers over SPI and I2C using NI Digital Pattern instruments with the Semiconductor Test Library (STL).

The example implementation encapsulates protocol-specific details, such as pattern bursting, source and capture waveforms, and bit packing, behind a single protocol-agnostic interface, `IDigitalProtocol`. The same test steps work with either protocol. A `CommunicationProtocol` parameter selects which protocol (`SPI` or `I2C`) is used at runtime.

Three common register access scenarios are demonstrated:

1. **Single Register - Site-Agnostic Values**: Write a value to a single register and compare the value read back from that register; the same value is written to all sites.
2. **Single Register - Site-Unique Values**: Write a site-unique value to a single register and compare the value read back from that register; a unique value is written to each site.
3. **Multiple Registers - Register-Specific Values**: Write register-specific values to multiple registers in a single burst and compare the values read back from each register; the same register-specific values are written to all sites.

> **NOTE**
>
> In each scenario, a single pattern burst is used for the write operation, and a second pattern burst is used for the readback operation.

The register addresses and values used by the example sequences target the Bosch BME280 sensor, which supports both SPI and I2C.

## Key Files

The example contains the following key files:

- `STLExample.RegisterIO.SPI.seq`: Example TestStand sequence file demonstrating use of SPI communication.
- `STLExample.RegisterIO.I2C.seq`: Example TestStand sequence file demonstrating use of I2C communication.
- `Code Modules/STLExample.RegisterIO.SPIAndI2C.csproj`: Code module project shared by both sequences.
- `Code Modules/DutControl/`: The protocol abstraction.
  - `IDigitalProtocol.cs`: The protocol-agnostic interface for accessing DUT registers.
  - `DigitalProtocol.cs`: Abstract base class that implements the core register access logic.
  - `SPI.cs` and `I2C.cs`: Protocol-specific implementations.
  - `CommunicationProtocol.cs`: Enumeration used to select the SPI or I2C protocol.
  - `SemiconductorModuleContextExtensions.cs`: The `DutControl` extension method that gets the selected protocol instance for the current code module.
- `Code Modules/TestSteps/`: The test step methods called from the sequences.
- `Supporting Materials/STLExample.RegisterIO.SPIAndI2C.digiproj`: Digital Pattern Editor project used by both sequences. It groups the following files:
  - `STLExample.RegisterIO.SPIAndI2C.pinmap`: Pin map.
  - `STLExample.RegisterIO.SPIAndI2C.digilevels`: Pin levels.
  - `STLExample.RegisterIO.SPIAndI2C.digitiming`: Time sets for both protocols.
  - `STLExample.RegisterIO.SPIAndI2C.specs`: Specifications for the pin levels and the SPI and I2C timing.
  - `SPI/` and `I2C/`: Read and write patterns, and source and capture waveform files, for each protocol.
- `Supporting Materials/STLExample.RegisterIO.SPIAndI2C.offlinecfg`: Offline configuration file for instrument simulation in Offline Mode.

## Prerequisites

1. To use this example, your system must meet the following requirements:
   - STS Software 25.5.0 or later
   - Semiconductor Test Library (STL) 25.5.0 or later
2. To run the example, you must also have:
   - One NI PXIe-6571 Digital Pattern instrument with the `HSD_6571_C1_S02` alias.
   - A DUT with registers that you can access over SPI and I2C, such as the Bosch BME280 sensor, connected to the instrument as defined in the pin map.

> **NOTE**
>
> If you cannot meet the second set of requirements, you can review the example sequence files in the TestStand Sequence Editor, and the C# code source files in Visual Studio or any text editor.
> To run the example, the required instrument must be physically installed or simulated using Offline Mode, and its alias in NI MAX (or the Hardware Configuration Tool) must match the alias defined in `STLExample.RegisterIO.SPIAndI2C.pinmap`.

### Offline Mode

Complete the following steps to simulate the instrument in Offline Mode:

1. Open `STLExample.RegisterIO.SPI.seq` or `STLExample.RegisterIO.I2C.seq` in the TestStand Sequence Editor.
2. Enable Offline Mode from the TSM toolbar.
3. Load `Supporting Materials/STLExample.RegisterIO.SPIAndI2C.offlinecfg`.

> **NOTE**
>
> A simulated instrument has no DUT to respond, so the register readback values cannot be verified in Offline Mode.

## How It Works

### Pin Map

The pin map defines one NI PXIe-6571 Digital Pattern instrument (`HSD_6571_C1_S02`) and one site (site 0). It has the following DUT pins:

| Pin | Channel | SPI Use | I2C Use |
|---|---|---|---|
| `CS` | 1 | Chip select (active low) | Held high by the read pattern to keep the DUT in I2C mode; not driven by the write pattern |
| `SDI` | 2 | Serial data into the DUT | Serial data line (SDA), used for both writing and reading |
| `SDO` | 3 | Serial data out of the DUT | Held high to select the I2C device address of the DUT (`0x77`) |
| `SCK` | 4 | Serial clock | Serial clock line (SCL) |
| `VO3` | 5 | Not used | Not used |
| `VIN` | 6 | DUT supply, forced by the instrument PPMU | DUT supply, forced by the instrument PPMU |

The pins `CS`, `SCK`, `SDI`, and `SDO` are also grouped as the pin group `DigitalPins`.

### Levels, Timing, and Specifications

- **Levels**: `DigitalPins` drive 0 V (Vil) and 3.3 V (Vih), with a compare threshold of 0.66 V (Vol) and 2.64 V (Voh), and a termination mode of High-Z. The levels reference the `DC` specifications.
- **Specifications**: The clock period (`t.SCK`) is `t.LOW + t.HIGH`, where each half is 5 µs, so the clock runs at 100 kHz for both protocols. The specifications also define the SPI data setup, chip select setup, and data output delay times, and the I2C START and STOP setup and hold, data setup and hold, and bus free times.
- **Time sets**: The SPI patterns use `SPI_Idle`, `SourceStartSetup`, `SPI_Start`, `SPI_Busy`, and `SPI_Stop`. The I2C patterns use `I2C_Idle`, `I2C_SourceStartSetup`, `I2C_Start`, `I2C_Busy`, and `I2C_Stop`. The `SourceStartSetup` and `I2C_SourceStartSetup` time sets provide the setup delay that the `source_start` opcode requires. The other time sets use the clock period from the specifications, and `I2C_Stop` adds the bus free time.

### Patterns

Each protocol has a read pattern and a write pattern (`SPI_read_template`, `SPI_write_template`, `I2C_read_template`, and `I2C_write_template`). The patterns are generic. The number of transactions and the bit widths are controlled by sequencer registers rather than being fixed in the pattern, so the same pattern can serve different register sizes:

| Sequencer Register | Purpose |
|---|---|
| `reg0` | Number of transactions in the burst (the loop count) |
| `reg1` | Number of address bits |
| `reg2` | Number of value bits |

The write pattern uses the source engine to dynamically source the register address and value bits on the `SDI` pin from a source waveform. The read pattern uses the source engine to dynamically source only the register address on the `SDI` pin, while also using the capture engine to capture the value bits into a capture waveform, from the `SDO` pin for SPI and from the `SDI` pin (SDA) for I2C.

Each pattern repeats the following transaction `reg0` times:

- `SPI_write_template`: `CS` low, write bit (`0`), register address, value, `CS` high.
- `SPI_read_template`: `CS` low, read bit (`1`), register address, captured value, `CS` high.
- `I2C_write_template`: START, device address (`0x77`) with the write bit, register address, value, STOP. The DUT acknowledges each byte.
- `I2C_read_template`: START, device address (`0x77`) with the write bit, register address, repeated START, device address with the read bit, captured value, NACK, STOP.

The patterns send the protocol framing bits, such as the SPI read or write bit, and the I2C device address, read or write bit, acknowledge, and NACK bits. Therefore, the address bit width counts only the register address bits. The I2C patterns expect an acknowledge after every 8 bits, so use an 8-bit address and an 8-bit value with I2C.

Each protocol uses its own source and capture waveforms, because both protocols share the same instrument session:

| Protocol | Source Waveform (Pin) | Capture Waveform (Pin) |
|---|---|---|
| SPI | `spi_source_buffer` (`SDI`) | `spi_capture_buffer` (`SDO`) |
| I2C | `i2c_source_buffer` (`SDI`) | `i2c_capture_buffer` (`SDI`) |

The waveform files in the digital project define serial waveforms with a sample width of 1 bit and the most significant bit first, and the source waveforms use site-unique data mapping. The protocol settings in the code modules must match these waveform definitions.

> **NOTE**
>
> The pattern source files (`.digipatsrc`) are available in `Supporting Materials/SPI` and `Supporting Materials/I2C`.

### Code Modules

The `DutControl` folder contains the protocol abstraction.

- `IDigitalProtocol` defines the high-level register access operations: `ReadRegister`, `WriteRegister`, `ReadRegisters`, and `WriteRegisters`. The write operations support broadcast and site-unique values. The interface also defines required properties for pattern names, waveform names, sequencer register names, bit widths, and bit order.
- `DigitalProtocol` is an abstract base class that implements the core logic of the `IDigitalProtocol` interface:
  1. It converts the address and value into waveform samples. By default, it uses a sample width of 8 bits, most significant bit first, a 16-bit address, and a 16-bit value. You can specify otherwise with the `ConfigureDigitalProtocol` test step, or with the register access overloads that accept the address and value bit widths.
  2. It writes `reg0`, `reg1`, and `reg2` on the sequencer.
  3. It writes the source waveform, either with the same samples for all sites or with unique samples per site. Every write is expanded to the same size, because the driver fixes the size of a source waveform on its first write.
  4. It bursts the read or write pattern with a 10-second timeout.
  5. For reads, it fetches the capture waveform and converts the samples into a single value for each register.
  6. It creates the necessary serial source and capture waveforms on the instrument. The source waveform must always be created as site-unique to allow support for both broadcast and site-unique writes.
- `SPI` and `I2C` are singleton classes derived from `DigitalProtocol`. Each is responsible for setting the properties required by the specific digital protocol, such as the read and write pattern names and the pins.
- `SemiconductorModuleContextExtensions.DutControl(CommunicationProtocol communicationProtocol)` provides a high-level method to get a selected protocol instance (`SPI` or `I2C`):
  1. It gets the appropriate protocol instance based on the selected `CommunicationProtocol`.
  2. It creates a `DigitalSessionsBundle` using the `TSMSessionManager` for the pins associated with the selected protocol instance.
  3. It stores the created `DigitalSessionsBundle` by calling the `IDigitalProtocol.SetBundle` method.
  4. Finally, it returns the instance as `IDigitalProtocol`.

Example usage:

```C#
IDigitalProtocol protocol = tsmContext.DutControl(CommunicationProtocol.SPI);
protocol.WriteRegister(address: 0x48, value: 4);
SiteData<long> readBack = protocol.ReadRegister(address: 0x48);
```

> **NOTE**
>
> - Call `DutControl` in each code module that accesses the DUT registers. Do not store the returned `IDigitalProtocol` instance and reuse it in a subsequent code module, because the instance is bound to the bundle of the code module that called `DutControl`.
> - The protocol instances are process-wide singletons and are not thread-safe. Do not invoke them in parallel.

#### Test Steps

The `TestSteps` folder contains the test step methods that the sequences call.

1. `ConfigureDigitalProtocol(communicationProtocol, addressBitWidth, valueBitWidth, writePatternName, readPatternName, sourceWaveformName, captureWaveformName, sampleWidth, readWriteCountSequenceRegister, addressBitWidthSequenceRegister, valueBitWidthSequenceRegister, pinNames)` overrides the default settings of the selected protocol. It is only required if you need to change a default. Call it once per test program before any protocol instance is used. If you omit the pattern names, waveform names, or pin names, the protocol keeps its current values. The bit widths, sample width, and sequencer registers are always set, so any that you omit revert to their defaults.
2. `WriteValueToRegisterAndCompareReadbackValue(tsmContext, protocol, registerAddress, valueToWrite)` writes the same value to a single register on all sites, reads the register back, compares the value read back on each site with the written value, and publishes `RegisterValueReadback` and `ComparisonResult`.
3. `WriteSiteUniqueValueToRegisterAndCompareReadbackValues(tsmContext, protocol, registerAddress, perSiteValuesToWrite)` writes a unique value to a single register on each site (one value per site, in site order), reads the register back, compares, and publishes the same two results.
4. `WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(tsmContext, protocol, registerAddresses, valuesToWrite)` writes register-specific values to multiple registers in a single burst, reads all the registers back in a second burst, and compares each register. It publishes `Register_0x<address>_Readback` and `Register_0x<address>_Comparison` for each register, where `<address>` is the register address in hexadecimal, for example `Register_0x74_Readback`. A single burst can access up to 32 registers.
5. `CleanupDigitalProtocol(communicationProtocol)` clears the reference of the protocol instance to the instrument sessions. Call it once per test program from `ProcessCleanup`, before the instrument sessions are closed. Without this step, TestStand reports unreleased object references when it shuts down.

### Sequence Files

Both sequence files have the same structure:

- `ProcessSetup`: The **Setup NI Digital Instrumentation** step initializes the digital instrument, loads the files in the digital project, and applies the `STLExample.RegisterIO.SPIAndI2C` levels and timing sheets. The **Cache Session Mapping** step keeps the cached instrument session information consistent between the STL TestStandSteps assembly and the example code modules assembly.
- `MainSequence`:
  - The `Setup` group powers up the DUT on the `VIN` pin (3.3 V with a 2 mA current limit) with the **DUT Power Up** step. Then, the **Configure Communication Protocol** step calls `ConfigureDigitalProtocol` to select the protocol to use at runtime and to set the bit widths, sample width, and pins for the DUT.
  - The `Main` group contains three Multi Test steps, one for each register access scenario.
  - The `Cleanup` group powers down the DUT with the **DUT Power Down** step.
- `ProcessCleanup`: The **Cleanup Communication Protocol** step calls `CleanupDigitalProtocol`, and then the **Cleanup Instrumentation** step closes the instrument sessions.

The **Configure Communication Protocol** step uses the following settings. The other parameters use the default values of the protocol.

| Parameter | SPI Sequence | I2C Sequence |
|---|---|---|
| `communicationProtocol` | `SPI` | `I2C` |
| `addressBitWidth` | 7 | 8 |
| `valueBitWidth` | 8 | 8 |
| `sampleWidth` | 1 | 1 |
| `pinNames` | `CS`, `SCK`, `SDI`, `SDO` | `CS`, `SCK`, `SDI`, `SDO` |

The sample width of 1 bit matches the waveform files in the digital project. The address bit widths differ because SPI sends a read or write bit followed by a 7-bit register address, whereas I2C sends the full 8-bit register address. For example, the BME280 register `0xF4` is addressed as `0x74` over SPI and as `0xF4` over I2C.

The register addresses, the values to write, and the expected readback values are defined as `MainSequence` locals, such as `WriteSingleRegisterAddress`, `WriteRegisterValues`, and `ReadRegisterValues`. The expected readback values are used as the test limits.

## Using the Example

Complete the following steps to use this example:

1. Open `Code Modules/STLExample.RegisterIO.SPIAndI2C.csproj` in Visual Studio and build it. The sequences call the code modules in `Code Modules/bin/NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.dll`, so you must build the project before you run the sequences.
2. Open the Digital Pattern project `Supporting Materials/STLExample.RegisterIO.SPIAndI2C.digiproj` in the Digital Pattern Editor to review the pin levels, timing, specifications, patterns, and waveforms.
3. Open `STLExample.RegisterIO.SPI.seq` or `STLExample.RegisterIO.I2C.seq` in the TestStand Sequence Editor. When the sequence file loads, the **About This Example** dialog box describes the example. Click **Help** in the dialog box to open this README.
4. Select **Semiconductor Module -> Edit Pin Map File** or click the **Edit Pin Map File** button on the TSM toolbar to open `STLExample.RegisterIO.SPIAndI2C.pinmap` in the Pin Map Editor. If using with physical hardware, update the instrument name, DUT pins, and site connections as needed to match your system setup.
5. Review the `ProcessSetup`, `MainSequence`, and `ProcessCleanup` sequences, as described in [Sequence Files](#sequence-files).
6. If using the BME280, or a similar DUT, ensure that the appropriate physical connections have been made between the NI Digital Pattern instrument and the DUT pins, as defined by the pin map. Additionally, when not using the BME280, verify that the register addresses, values, and bit widths passed to each step are appropriate for your DUT. Note that the default address and value bit width is 16 bits, which the **Configure Communication Protocol** step overrides. If you change the register addresses of the multiple-register step, also update the measurement IDs of its test evaluations, because the published data IDs contain the register addresses.
7. Run the test program by clicking the **Start/Resume Lot** button on the TSM toolbar. Set a breakpoint, launch the TSM Runtime Data Viewer, or open the TSM data logs at the end of the lot to review the test results.
