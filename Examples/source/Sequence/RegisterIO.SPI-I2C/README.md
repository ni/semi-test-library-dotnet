# Register IO (SPI and I2C) Sequence Example

This example demonstrates how to access DUT registers over SPI and I2C using NI Digital Pattern instruments with the Semiconductor Test Library (STL).

The example implementation encapsulates the following protocol-specific details behind a single protocol-agnostic interface, `IDigitalProtocol`:

- Pattern bursting
- Source and capture waveforms
- Bit packing

The same test steps work with either protocol. A `CommunicationProtocol` parameter selects the protocol (`SPI` or `I2C`) that the test steps use at runtime. This is particularly useful for DUTs that support both I2C and SPI communication.

The example demonstrates three common register access scenarios:

| Scenario                                      | Description                                                                                                                                                                      |
| --------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Single Register - Site-Agnostic Values        | Write a value to a single register and compare the value read back from that register. All sites receive the same value.                                                         |
| Single Register - Site-Unique Values          | Write a site-unique value to a single register and compare the value read back from that register. Each site receives a unique value.                                            |
| Multiple Registers - Register-Specific Values | Write register-specific values to multiple registers in a single burst and compare the values read back from each register. All sites receive the same register-specific values. |

> **NOTE**: Each scenario uses a single pattern burst for the write operation and a second pattern burst for the readback operation.

The register addresses and values used by the example sequences target a Bosch BME280 sensor, as it supports both SPI and I2C communication and is widely available.

## Key Files

The example contains the following key files:

- `STLExample.RegisterIO.SPI.seq`: Example TestStand sequence file demonstrating the use of SPI communication.
- `STLExample.RegisterIO.I2C.seq`: Example TestStand sequence file demonstrating the use of I2C communication.
- `Code Modules/STLExample.RegisterIO.SPIAndI2C.csproj`: Code module project for both sequences.
- `Code Modules/DutControl/`: Protocol abstraction.
  - `IDigitalProtocol.cs`: Protocol-agnostic interface for accessing DUT registers.
  - `DigitalProtocol.cs`: Abstract base class that implements the core register access logic.
  - `SPI.cs` and `I2C.cs`: Protocol-specific implementations.
  - `CommunicationProtocol.cs`: Enumeration for selecting the SPI or I2C protocol.
  - `SemiconductorModuleContextExtensions.cs`: `DutControl` extension method that gets the selected protocol instance for the current code module.
- `Code Modules/TestSteps/`: Test step methods that the sequences call.
- `Supporting Materials/`: Pin map, digital project, and other supporting files for both sequences.
  - `STLExample.RegisterIO.SPIAndI2C.offlinecfg`: Offline configuration file for instrument simulation in Offline Mode.
  - `STLExample.RegisterIO.SPIAndI2C.pinmap`: Pin map.
  - `STLExample.RegisterIO.SPIAndI2C.digiproj`: Digital Pattern Editor project.
  - `STLExample.RegisterIO.SPIAndI2C.specs`: Specifications for the pin levels and the SPI and I2C timing.
  - `STLExample.RegisterIO.SPIAndI2C.digilevels`: Pin levels.
  - `STLExample.RegisterIO.SPIAndI2C.digitiming`: Digital time sets for both protocols.
  - `SPI/` and `I2C/`: Read and write digital patterns for each protocol, and source and capture waveform files for each protocol.

## Prerequisites

### Software Requirements

- STS Software 24.5.1 or later.
- Semiconductor Test Library (STL) 24.5.1 or later.

### Hardware Requirements

- One NI PXIe-6571 Digital Pattern instrument with the `HSD_6571_C1_S02` alias.
- One DUT for each site with registers that you can access over SPI and I2C, such as the Bosch BME280 sensor, connected to the instrument as defined in the pin map.

> **NOTE**
>
> If you cannot meet the hardware requirements, you can review the example sequence files in the TestStand Sequence Editor, and the C# code source files in Visual Studio or any text editor.
> To run the example, you must physically install the required instrument or simulate it using Offline Mode. The alias of the instrument in NI MAX or NI Hardware Manager must match the alias defined in `STLExample.RegisterIO.SPIAndI2C.pinmap`.

### Offline Mode

Complete the following steps to simulate the instrument in Offline Mode:

1. Open `STLExample.RegisterIO.SPI.seq` or `STLExample.RegisterIO.I2C.seq` in the TestStand Sequence Editor.
2. Click the **Enable Offline Mode** button on the TSM toolbar to load `Supporting Materials/STLExample.RegisterIO.SPIAndI2C.offlinecfg`.

> **NOTE**: A simulated instrument has no DUT to respond, so you cannot verify the register readback values in Offline Mode.

## How It Works

### Pin Map

The pin map defines one NI PXIe-6571 Digital Pattern instrument (`HSD_6571_C1_S02`) and two sites (site 0 and site 1). The pin map has the following DUT pins:

| Pin   | Site 0 Channel | Site 1 Channel | SPI Use                             | I2C Use                                                                                         |
| ----- | -------------- | -------------- | ----------------------------------- | ----------------------------------------------------------------------------------------------- |
| `CS`  | 1              | 29             | Chip select (active low)            | The read pattern holds it high to keep the DUT in I2C mode; the write pattern does not drive it |
| `SDI` | 2              | 28             | Serial data into the DUT            | Serial data line (SDA) for both writing and reading                                             |
| `SDO` | 3              | 27             | Serial data out of the DUT          | The patterns hold it high to select the I2C device address of the DUT (`0x77`)                  |
| `SCK` | 4              | 26             | Serial clock                        | Serial clock line (SCL)                                                                         |
| `VO3` | 5              | 25             | Not used                            | Not used                                                                                        |
| `VIN` | 6              | 24             | DUT supply from the instrument PPMU | DUT supply from the instrument PPMU                                                             |

The pin group `DigitalPins` groups the following pins:

- `CS`
- `SCK`
- `SDI`
- `SDO`

> **NOTE**
>
> For I2C, the `SDI` (SDA) and `CS` pins require pull-up resistors, unless your DUT board already provides them:
>
> - `SDI` (SDA): The patterns release this pin to check the ACK and NACK bits and to capture the register data.
> - `CS`: The write pattern does not drive this pin. If `CS` goes low, the BME280 disables its I2C interface until the next power-on reset.

### Levels, Timing, and Specifications

**Levels**: `DigitalPins` drive 0 V (Vil) and 3.3 V (Vih), with a compare threshold of 0.66 V (Vol) and 2.64 V (Voh), and a termination mode of High-Z. The levels reference the `DC` specifications.

**Specifications**: The clock period (`t.SCK`) is `t.LOW + t.HIGH`, where each half is 5 µs, so the clock runs at 100 kHz for both protocols. The specifications also define the following times:

- SPI data input setup time (`t.setup_mosi`)
- SPI data output delay time (`t.delay_miso`)
- SPI chip select setup time (`t.setup_cs`)
- I2C START hold and setup times (`t.HD_STA` and `t.SU_STA`)
- I2C data hold and setup times (`t.HD_DAT` and `t.SU_DAT`)
- I2C STOP setup time (`t.SU_STO`)
- I2C bus free time between a STOP and a START (`t.BUF`)

**Time sets**: The SPI and I2C patterns use the following time sets:

| SPI Time Set           | I2C Time Set           |
| ---------------------- | ---------------------- |
| `SPI_Idle`             | `I2C_Idle`             |
| `SPI_SourceStartSetup` | `I2C_SourceStartSetup` |
| `SPI_Start`            | `I2C_Start`            |
| `SPI_Busy`             | `I2C_Busy`             |
| `SPI_Stop`             | `I2C_Stop`             |

The `SPI_SourceStartSetup` and `I2C_SourceStartSetup` time sets provide the setup delay that the `source_start` opcode requires. The other time sets use the clock period from the specifications, and `I2C_Stop` adds the bus free time.

### Patterns

Each protocol has a read pattern and a write pattern:

- `SPI_read_template`
- `SPI_write_template`
- `I2C_read_template`
- `I2C_write_template`

The patterns are generic. Sequencer registers control the number of transactions and the bit widths instead of fixed values in the patterns, so the same pattern can serve different register sizes:

| Sequencer Register | Purpose                                          |
| ------------------ | ------------------------------------------------ |
| `reg0`             | Number of transactions in the burst (loop count) |
| `reg1`             | Number of address bits                           |
| `reg2`             | Number of value bits                             |

The write pattern uses the source engine to dynamically source the register address and the value bits on the `SDI` pin from a source waveform. The read pattern uses the source engine to dynamically source only the register address on the `SDI` pin. The read pattern also uses the capture engine to capture the value bits into a capture waveform from the `SDO` pin for SPI and from the `SDI` pin (SDA) for I2C.

Each pattern repeats the following transaction `reg0` times:

- `SPI_write_template`: `CS` low, write bit (`0`), register address, value, `CS` high.
- `SPI_read_template`: `CS` low, read bit (`1`), register address, captured value, `CS` high.
- `I2C_write_template`: START, device address (`0x77`) with the write bit, register address, value, STOP. The DUT acknowledges each byte.
- `I2C_read_template`: START, device address (`0x77`) with the write bit, register address, repeated START, device address with the read bit, captured value, NACK, STOP.

The patterns generate or check the following protocol framing bits, so the source waveform does not contain them:

- The SPI read or write bit
- The I2C device address
- The I2C read or write bit
- The I2C ACK and NACK bits

Therefore, the address bit width counts only the register address bits. The I2C patterns expect an ACK after every 8 bits, so use an 8-bit address and an 8-bit value with I2C.

Each protocol uses its own source and capture waveforms because both protocols share the same instrument session:

| Protocol | Source Waveform (Pin)       | Capture Waveform (Pin)       |
| -------- | --------------------------- | ---------------------------- |
| SPI      | `SPI_source_buffer` (`SDI`) | `SPI_capture_buffer` (`SDO`) |
| I2C      | `I2C_source_buffer` (`SDI`) | `I2C_capture_buffer` (`SDI`) |

The waveform files in the digital project define serial waveforms with a sample width of 1 bit and the most significant bit first. The source waveforms use site-unique data mapping. The protocol settings in the code modules must match these waveform definitions.

> **NOTE**: You can find the pattern source files (`.digipatsrc`) in `Supporting Materials/SPI` and `Supporting Materials/I2C`.

### Code Modules

The `DutControl` folder contains the protocol abstraction.

- `IDigitalProtocol` defines the following high-level register access operations:

  - `ReadRegister`
  - `WriteRegister`
  - `ReadRegisters`
  - `WriteRegisters`

  The write operations support broadcast and site-unique values. The interface also defines required properties for the following settings:

  - Pattern names
  - Waveform names
  - Sequencer register names
  - Bit widths
  - Bit order

- `DigitalProtocol` is an abstract base class that implements the core logic of the `IDigitalProtocol` interface:
  1. It converts the address and value into waveform samples. By default, it uses a sample width of 8 bits with the most significant bit first, a 16-bit address, and a 16-bit value. You can override these defaults with the `ConfigureDigitalProtocol` test step or with the register access overloads that accept the address and value bit widths.
  2. It writes `reg0`, `reg1`, and `reg2` on the sequencer.
  3. It writes the source waveform, either with the same samples for all sites or with unique samples per site.
  4. It bursts the read or write pattern with a 10-second timeout.
  5. For reads, it fetches the capture waveform and converts the samples into a single value for each register.
  6. It creates the necessary serial source and capture waveforms on the instrument. The source waveform must always be site-unique to support both broadcast and site-unique writes.
- `SPI` and `I2C` are singleton classes derived from `DigitalProtocol`. Each is responsible for setting the properties that the specific digital protocol requires, such as the read and write pattern names.
- `SemiconductorModuleContextExtensions.DutControl(CommunicationProtocol communicationProtocol)` provides a high-level method to get a selected protocol instance (`SPI` or `I2C`):
  1. It gets the appropriate protocol instance based on the selected `CommunicationProtocol`.
  2. It creates a `DigitalSessionsBundle` using the `TSMSessionManager` for the pins associated with the selected protocol instance.
  3. It stores the created `DigitalSessionsBundle` by calling the `IDigitalProtocol.SetBundle` method.
  4. Finally, it returns the instance as `IDigitalProtocol`.

The following example assumes that `ConfigureDigitalProtocol` has already configured the protocol, as the **Configure Communication Protocol** step does in the example sequences:

```C#
IDigitalProtocol protocol = tsmContext.DutControl(CommunicationProtocol.SPI);
protocol.WriteRegister(address: 0x48, value: 4);
SiteData<long> readBack = protocol.ReadRegister(address: 0x48);
```

> **NOTE**
>
> - Call `DutControl` in each code module that accesses the DUT registers. Do not store the returned `IDigitalProtocol` instance for reuse in a subsequent code module because the instance is bound to the bundle of the code module that called `DutControl`.
> - The protocol instances are process-wide singletons, and they are not thread-safe. Do not invoke them in parallel.

#### Test Steps

The `TestSteps` folder contains the test step methods that the sequences call:

1. `ConfigureDigitalProtocol` overrides the default settings of the selected protocol.
   - Parameters: `communicationProtocol`, `addressBitWidth`, `valueBitWidth`, `writePatternName`, `readPatternName`, `sourceWaveformName`, `captureWaveformName`, `sampleWidth`, `readWriteCountSequenceRegister`, `addressBitWidthSequenceRegister`, `valueBitWidthSequenceRegister`, and `pinNames`.
   - Any parameter that you leave at its default value (an empty string, `-1`, or an empty pin list) uses the default setting of the protocol.
   - Call it once per test program before you use any protocol instance.
   - It is required only if you need to change a default. The example sequences require it because the default settings do not match the waveform files in the digital project or the BME280.
2. `WriteValueToRegisterAndCompareReadbackValue` writes the same value to a single register on all sites.
   - Parameters: `tsmContext`, `protocol`, `registerAddress`, and `valueToWrite`.
   - It reads the register back and compares the value read back on each site with the written value.
   - It publishes `RegisterValueReadback` and `ComparisonResult`.
3. `WriteSiteUniqueValueToRegisterAndCompareReadbackValues` writes a unique value to a single register on each site.
   - Parameters: `tsmContext`, `protocol`, `registerAddress`, and `perSiteValuesToWrite` (one value per site, in site order).
   - It reads the register back and compares the value read back on each site with the value written to that site.
   - It publishes `RegisterValueReadback` and `ComparisonResult`.
4. `WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues` writes register-specific values to multiple registers in a single burst.
   - Parameters: `tsmContext`, `protocol`, `registerAddresses`, and `valuesToWrite`.
   - It reads all the registers back in a second burst and compares the value read back from each register with the value written to it.
   - It publishes `Register_0x<address>_Readback` and `Register_0x<address>_Comparison` for each register, where `<address>` is the register address in hexadecimal, for example, `Register_0x74_Readback`.
   - A single burst can access up to 32 registers.
5. `CleanupDigitalProtocol` clears the reference of the protocol instance to the instrument sessions.
   - Parameters: `communicationProtocol`.
   - Call it once per test program from `ProcessCleanup` before the **Cleanup Instrumentation** step closes the instrument sessions. Without this step, TestStand reports unreleased object references when it shuts down.

### Sequence Files

Both sequence files have the same structure:

- `ProcessSetup`: The **Setup NI Digital Instrumentation** step initializes the digital instrument, loads the files in the digital project, and applies the `STLExample.RegisterIO.SPIAndI2C` levels and timing sheets. The **Cache Session Mapping** step keeps the cached instrument session information consistent between the installed STL TestStandSteps assembly and the example code modules assembly.
- `MainSequence`:
  - The `Setup` group powers up the DUT on the `VIN` pin (3.3 V with a 2 mA current limit) with the **DUT Power Up** step. Then, the **Configure Communication Protocol** step calls `ConfigureDigitalProtocol`. This step configures the selected protocol for the DUT by setting the bit widths, sample width, and pins.
  - The `Main` group contains three Multi Test steps, one for each register access scenario.
  - The `Cleanup` group powers down the DUT with the **DUT Power Down** step.
- `ProcessCleanup`: The **Cleanup Communication Protocol** step calls `CleanupDigitalProtocol`. Then, the **Cleanup Instrumentation** step closes the instrument sessions.

The **Configure Communication Protocol** step uses the following settings. The other parameters use the default values of the protocol.

| Parameter               | SPI Sequence              | I2C Sequence              |
| ----------------------- | ------------------------- | ------------------------- |
| `communicationProtocol` | `SPI`                     | `I2C`                     |
| `addressBitWidth`       | 7                         | 8                         |
| `valueBitWidth`         | 8                         | 8                         |
| `sampleWidth`           | 1                         | 1                         |
| `pinNames`              | `CS`, `SCK`, `SDI`, `SDO` | `CS`, `SCK`, `SDI`, `SDO` |

The sample width of 1 bit matches the waveform files in the digital project. The address bit width is the number of register address bits in the source waveform. It differs between the protocols because each protocol places the read or write bit differently:

- **SPI**: The read or write bit and the register address share one byte. The pattern sends the read or write bit, and the source waveform supplies the remaining 7 bits.
- **I2C**: The read or write bit shares a byte with the 7-bit device address of the DUT (`0x77`), which the patterns send. The register address follows in its own byte, so the source waveform supplies all 8 bits, and you pass `0xF4` to access register `0xF4`.

The register addresses, the values to write, and the expected readback values are `MainSequence` locals, such as:

- `WriteSingleRegisterAddress`
- `WriteSingleRegisterValue`
- `ReadSingleRegisterValue`

The expected readback values are the test limits.

## Using the Example

Complete the following steps to use this example:

1. Open `Code Modules/STLExample.RegisterIO.SPIAndI2C.csproj` in Visual Studio and build it. The sequences call the code modules in `Code Modules/bin/NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.dll`, so you must build the project before you run the sequences.
2. Open the Digital Pattern project `Supporting Materials/STLExample.RegisterIO.SPIAndI2C.digiproj` in the Digital Pattern Editor.
3. Review the pin levels, timing, specifications, patterns, and waveforms.
4. Open `STLExample.RegisterIO.SPI.seq` or `STLExample.RegisterIO.I2C.seq` in the TestStand Sequence Editor. When the sequence file loads, the **About This Example** dialog box describes the example. Click **Help** in the dialog box to open this README.
5. Select **Semiconductor Module -> Edit Pin Map File** or click the **Edit Pin Map File** button on the TSM toolbar to open `STLExample.RegisterIO.SPIAndI2C.pinmap` in the Pin Map Editor. If you are using physical hardware, update the instrument name, DUT pins, and site connections as needed to match your system setup.
6. Review the `ProcessSetup`, `MainSequence`, and `ProcessCleanup` sequences, as described in [Sequence Files](#sequence-files).
7. If you are using the BME280 or a similar DUT, ensure that the appropriate physical connections exist between the NI Digital Pattern instrument and the DUT pins, as defined by the pin map. Additionally, when you are not using the BME280, verify that the register addresses, values, and bit widths passed to each step are appropriate for your DUT. Note that the default address and value bit width is 16 bits, which the **Configure Communication Protocol** step overrides. If you change the register addresses of the multiple-register step, also update the measurement IDs of its test evaluations because the published data IDs contain the register addresses.
8. Run the test program by clicking the **Start/Resume Lot** button on the TSM toolbar.
9. Set a breakpoint, launch the TSM Runtime Data Viewer, or open the TSM data logs at the end of the lot to review the test results.
