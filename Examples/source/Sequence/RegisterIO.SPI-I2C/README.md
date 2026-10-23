# Register IO (SPI and I2C) Sequence Example

This example demonstrates how to read and write DUT registers over SPI and I2C using NI Digital Pattern instruments and the Semiconductor Test Library (STL).

The example wraps the protocol-specific details (pattern bursting, source and capture waveforms, and bit packing) behind a single protocol-agnostic interface, `IDigitalProtocol`. The same test steps work for either protocol. A `CommunicationProtocol` parameter selects which implementation (`SPI` or `I2C`) is used at runtime.

Three register access scenarios are demonstrated:

1. **Write a broadcast value to one register and compare the readback**: the same value is written to every site.
2. **Write a site-unique value to one register and compare the readback**: each site gets its own value.
3. **Write unique values to multiple registers in a single burst and compare the readbacks**: several registers are written and read back with one pattern burst each.

## Key Files

The example contains the following key files:

- `Code Modules/STLExample.RegisterIO.SPI.seq`: Example TestStand sequence file for SPI.
- `Code Modules/STLExample.RegisterIO.I2C.seq`: Example TestStand sequence file for I2C.
- `Code Modules/STLExample.RegisterIO.SPIAndI2C.csproj` and `.sln`: Code module project and solution shared by both sequences.
- `Code Modules/DutControl/`: The protocol abstraction.
  - `IDigitalProtocol.cs`: The protocol-agnostic contract for reading and writing registers.
  - `DigitalProtocol.cs`: Abstract base class that implements the register read and write logic.
  - `SPI.cs` and `I2C.cs`: Protocol implementations. Each is a singleton that sets its pattern names and pins.
  - `CommunicationProtocol.cs`: Enumeration used to select SPI or I2C.
  - `SemiconductorModuleContextExtensions.cs`: The `DutControl` extension method that creates a protocol instance bound to the current context.
- `Code Modules/TestSteps/`: The test step methods called from the sequences.
- `Supporting Materials/DigitalProject/STLExample.RegisterIO.SPIAndI2C.digiproj`: Digital Pattern Editor project that groups the files below.
  - `STLExample.RegisterIO.SPIAndI2C.pinmap`: Pin map used by the sequences.
  - `STLExample.RegisterIO.SPIAndI2C.digilevels`: Pin levels.
  - `STLExample.RegisterIO.SPIAndI2C.digitiming`: Time sets.
  - `SPI Characteristic.specs`: Specifications for the SPI clock and timing.
  - `SPI/` and `I2C/`: Read and write patterns for each protocol.

> **NOTE**
>
> This example does not include an offline configuration file (`.offlinecfg`).

## Prerequisites

1. To use this example, your system must meet the following requirements:
   - STS Software 25.0.0 or later
   - Semiconductor Test Library (STL) 25.0.0 or later
   - TestStand 2023 or later
   - NI-Digital Pattern Driver and the Digital Pattern Editor (to open or rebuild the digital project)
2. To run the example, you must also have:
   - One NI Digital Pattern instrument (32 channels) with the alias `PXI1Slot2`, as defined in the pin map.
   - A DUT connected to that instrument that responds to the register protocol described below, or a loopback arrangement that returns the written data on `SDO`.

> **NOTE**
>
> You can view the example sequence files in the TestStand Sequence Editor, and the C# code source files in Visual Studio or any text editor.
> To run the example, however, the required instrument must be physically installed in your system, and its alias in NI MAX (or the Hardware Configuration Tool) must match the alias defined in `STLExample.RegisterIO.SPIAndI2C.pinmap`.

## How It Works

### Pin map

The pin map defines one NI Digital Pattern instrument (`PXI1Slot2`) and one site (site 0). It has the following DUT pins:

| Pin | Channel | Use |
|---|---|---|
| `CS` | 1 | SPI chip select |
| `SDI` | 2 | Serial data into the DUT (source) |
| `SDO` | 3 | Serial data out of the DUT (capture) |
| `SCK` | 4 | Serial clock |
| `VO3` | 5 | Not used by the register IO steps |
| `VIN` | 6 | Not used by the register IO steps |

The pins `CS`, `SCK`, `SDI`, and `SDO` are also grouped as the pin group `DigitalPins`.

### Levels, timing, and specifications

- **Levels**: `DigitalPins` drive 0 V (Vil) and 3.3 V (Vih), with a compare threshold of 0.66 V (Vol) and 2.64 V (Voh), and a termination mode of High-Z.
- **Specifications**: The SPI clock period is `t.LOW + t.HIGH`, where each half is 5 µs, so the clock runs at 100 kHz. The data setup time, chip select setup time, and clock-to-data delay are also defined here.
- **Time sets**: `Idle`, `SourceStartSetup`, `Start`, `Busy`, and `Stop`. They use the clock period from the specifications.

### Patterns

Each protocol has a read pattern and a write pattern (`SPI_read_pattern`, `SPI_write_pattern`, `I2C_read_pattern`, and `I2C_write_pattern`). The patterns are generic. The number of transactions and the bit widths are controlled by sequencer registers rather than being fixed in the pattern, so the same pattern can serve different register sizes:

| Sequencer register | Purpose |
|---|---|
| `reg0` | Number of transactions in the burst (the loop count) |
| `reg1` | Number of address bits |
| `reg2` | Number of value bits |

The SPI source patterns are available as `.digipatsrc` files in `Supporting Materials/DigitalProject/SPI`. The patterns source address and value bits on `SDI` from a source waveform, and the read pattern captures value bits on `SDO` into a capture waveform.

### Code modules

The `DutControl` folder contains the protocol abstraction.

- `IDigitalProtocol` defines the operations: `ReadRegister`, `WriteRegister` (broadcast or site-unique value), `ReadRegisters`, `WriteRegisters` (broadcast or site-unique values), `SetBundle`, and `ReleaseBundle`. It also exposes the settings the implementation uses, such as pattern names, waveform names, sequencer register names, bit widths, and bit order.
- `DigitalProtocol` is the abstract base class. It is where the register IO is implemented:
  1. It converts the address and value into waveform samples (default sample width 8 bits, most significant bit first, 16-bit address and 16-bit value).
  2. It writes `reg0`, `reg1`, and `reg2` on the sequencer.
  3. It writes the source waveform, either to all sites or per site.
  4. It bursts the read or write pattern with a 10-second timeout.
  5. For reads, it fetches the capture waveform and converts the samples back to a value.
  `SetBundle` also creates the serial source and capture waveforms on the instrument. The source waveform is created as site-unique, so it supports both broadcast and site-unique writes.
- `SPI` and `I2C` are singletons derived from `DigitalProtocol`. Each sets its read and write pattern names and the pins it needs (`SPI`: `SDI` and `SDO` by default; `I2C`: `SDI`, `SDO`, and `SCK`).
- `SemiconductorModuleContextExtensions.DutControl(communicationProtocol)` picks the protocol instance, creates a `DigitalSessionsBundle` for its pins from `TSMSessionManager`, calls `SetBundle`, and returns the instance as `IDigitalProtocol`.

Example usage:

```C#
IDigitalProtocol protocol = tsmContext.DutControl(CommunicationProtocol.SPI);
try
{
    protocol.WriteRegister(address: 0x48, value: 4);
    SiteData<long> readBack = protocol.ReadRegister(address: 0x48);
}
finally
{
    protocol.ReleaseBundle();
}
```

> **NOTE**
>
> Call `ReleaseBundle` in a `finally` block after register operations and result publishing. It releases the reference to the current code module's bundle without disposing the instrument sessions.

### Test steps

Open `Code Modules/STLExample.RegisterIO.SPIAndI2C.sln` to review the source code for the test steps in `Code Modules/TestSteps`.

1. `ConfigureDigitalProtocol(communicationProtocol, addressBitWidth, valueBitWidth, writePatternName, readPatternName, sourceWaveformName, captureWaveformName, sampleWidth, readWriteCountSequenceRegister, addressBitWidthSequenceRegister, valueBitWidthSequenceRegister, pinNames)` overrides the default protocol settings. It is only required if you need to change a default. Call it once per test program before any protocol instance is used, and set only the values you need.
2. `WriteValueToRegisterAndCompareReadbackValue(tsmContext, protocol, registerAddress, valueToWrite)` writes one value to every site, reads the register back, compares each site's readback to the written value, publishes `RegisterValueReadback` and `ComparisonResult`, and returns the per-site comparison.
3. `WriteSiteUniqueValueToRegisterAndCompareReadbackValues(tsmContext, protocol, registerAddress, perSiteValuesToWrite)` writes a different value to each site (one per site, in site order), reads the register back, compares, publishes the same two results, and returns the per-site comparison.
4. `WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(tsmContext, protocol, registerAddresses, valuesToWrite)` writes one value per register in a single burst, reads all the registers back, and compares each one. It publishes `Register_0x<address>_Readback` and `Register_0x<address>_Comparison` for every register, and returns the per-site, per-register comparison.

## Using the Example

Complete the following steps to use this example:

1. Open the example solution `Code Modules/STLExample.RegisterIO.SPIAndI2C.sln` in Visual Studio and build it. The code modules must be built before you run the sequences. They are referenced from the sequence files.
2. Open the Digital Pattern project `Supporting Materials/DigitalProject/STLExample.RegisterIO.SPIAndI2C.digiproj` in the Digital Pattern Editor to review the pin levels, timing, specifications, and patterns.
3. Open `STLExample.RegisterIO.SPI.seq` or `STLExample.RegisterIO.I2C.seq` in the TestStand Sequence Editor.
4. Select **Semiconductor Module -> Edit Pin Map File** or click the **Edit Pin Map File** button on the TSM toolbar to open `STLExample.RegisterIO.SPIAndI2C.pinmap` in the Pin Map Editor. Confirm the instrument alias `PXI1Slot2`, the DUT pins, and the site connections.
5. Review the `ProcessSetup`, `MainSequence`, and `ProcessCleanup` sequences:
   - `ProcessSetup`: Initializes the digital instrument and loads the pin levels and timing from the digital project. It also caches the session mapping, which keeps the cached instrument session information consistent between the STL TestStandSteps assembly and the example code modules assembly.
   - `MainSequence`: Configures the communication protocol (SPI or I2C) used for register access, then calls the test steps described above to write registers and compare the readback values.
   - `ProcessCleanup`: Closes and resets the instrument.
6. Confirm the register addresses, values, and bit widths passed to each step match your DUT. The default address and value width is 16 bits.
7. To run the test program, click the **Start/Resume Lot** button on the TSM toolbar.
