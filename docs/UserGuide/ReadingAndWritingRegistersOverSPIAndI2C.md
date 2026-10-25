# Reading and Writing Registers over SPI and I2C

The Semiconductor Test Library (STL) provides digital instrument abstraction for NI Digital Pattern instruments that you can use to read and write DUT registers over serial protocols, such as SPI and I2C. A register access consists of a burst of a pattern that sources the address and value bits from a source waveform and, for reads, captures the value bits into a capture waveform.

The Register IO example wraps the protocol-specific details (pattern bursting, source and capture waveforms, and bit packing) behind a single protocol-agnostic interface. The same test steps work for either protocol, and a parameter selects which protocol implementation is used at runtime.

> [!NOTE]
>
> - The protocol abstraction (`IDigitalProtocol`, `DigitalProtocol`, `SPI`, and `I2C`) is example code that you can copy into your own project and adapt. It is not part of the STL NuGet packages.
> - The example builds on STL digital extension methods for writing sequencer registers, writing source waveforms, bursting patterns, and fetching capture waveforms.

## Overview

A register IO implementation requires the following items:

1. **Pin map**: Defines the digital instrument and the DUT pins used by the protocol, such as chip select, clock, serial data in, and serial data out.
2. **Pin levels, time sets, and specifications**: Define the voltage levels and timing used to drive and compare the protocol signals.
3. **Read and write patterns**: Generic patterns for each protocol. The number of transactions and the bit widths are controlled by sequencer registers rather than being fixed in the pattern, so the same pattern can be used for different register sizes.
4. **Source and capture waveforms**: Serial waveforms that the patterns use to shift the address and value bits out to the DUT and to capture the value bits that the DUT returns.
5. **Code modules**: Convert the address and value to waveform samples, configure the sequencer registers, burst the pattern, and convert any captured samples back to a value.

### Sequencer Registers

The patterns in the example use the following sequencer registers. The code modules write these registers before every burst.

| Sequencer register | Purpose |
|---|---|
| `reg0` | Number of transactions in the burst (the loop count). |
| `reg1` | Number of address bits. |
| `reg2` | Number of value bits. |

### Protocol Framing

The patterns implement the protocol framing, so the code modules only provide the register address and value bits. In the example, the SPI patterns drive the chip select and send a read or write bit before the register address. The I2C patterns generate the START and STOP conditions, send the device address with the read or write bit, and check the acknowledge bits. Therefore, the address bit width counts only the register address bits. The I2C patterns expect an acknowledge after every 8 bits, so use 8-bit addresses and values with them.

### Source and Capture Waveforms

The patterns source the address and value bits on the serial data in (`SDI`) pin from a source waveform. The read patterns capture the value bits into a capture waveform. For SPI, the capture pin is the serial data out (`SDO`) pin. For I2C, the data line (SDA) is bidirectional, so the example connects it to `SDI` and uses `SDI` for both the source and the capture waveforms. The code converts each address and value into samples using a configurable sample width (default 8 bits) with the most significant bit first.

The source waveform is created as site-unique. This supports writing both a broadcast value, where all sites receive the same value, and site-unique values, where each site receives a different value.

The driver fixes the size of a source waveform on its first write in a session. Therefore, the example pads every source waveform write to the same size, which is large enough for the largest supported burst.

## Protocol Abstraction

The example separates the protocol-independent register IO logic from the protocol-specific settings.

| Type | Description |
|---|---|
| `IDigitalProtocol` | The protocol-agnostic contract for reading and writing registers. |
| `DigitalProtocol` | Abstract base class that implements the register read and write logic. |
| `SPI` and `I2C` | Singleton protocol implementations. Each sets its pattern names, waveform names, and the pins it requires. |
| `CommunicationProtocol` | Enumeration used to select `SPI` or `I2C`. |
| `SemiconductorModuleContextExtensions.DutControl` | Extension method that returns the selected protocol instance bound to the current context. |

`IDigitalProtocol` exposes the following operations:

- `ReadRegister` and `ReadRegisters`
- `WriteRegister` and `WriteRegisters`, each with an overload for a broadcast value and an overload for site-unique values
- `SetBundle` and `ClearBundle`

It also exposes the settings used by the implementation, such as the pattern names, waveform names, source and capture pins, sequencer register names, bit widths, sample width, and bit order.

`DigitalProtocol` also provides an overload of each register operation that takes the address and value bit widths, so you can use different bit widths for a single call. These overloads are not part of `IDigitalProtocol`.

### Default Settings

| Setting | Default |
|---|---|
| Sample width | 8 bits |
| Bit order | Most significant bit first |
| Address bit width | 16 bits |
| Value bit width | 16 bits |
| Sequencer registers | `reg0`, `reg1`, and `reg2` |
| Burst and capture timeout | 10 seconds |

The protocol-specific defaults are as follows:

| Setting | `SPI` | `I2C` |
|---|---|---|
| Write and read patterns | `SPI_write_template`, `SPI_read_template` | `I2C_write_template`, `I2C_read_template` |
| Source and capture waveforms | `spi_source_buffer`, `spi_capture_buffer` | `i2c_source_buffer`, `i2c_capture_buffer` |

You can override the defaults for your DUT, for example by using the `ConfigureDigitalProtocol` test step from the example. Call it once per test program, before any protocol instance is used. If you omit the pattern names, waveform names, or pin names, the protocol keeps its current values. The bit widths, sample width, and sequencer registers are always set, so any that you omit revert to their defaults.

> [!NOTE]
> The example sequences override the defaults to match the example DUT and digital project: a sample width of 1 bit, an 8-bit value, and a 7-bit address for SPI or an 8-bit address for I2C.

## Using the Protocol in Test Code

Use the `DutControl` extension method on the `ISemiconductorModuleContext` object to get an `IDigitalProtocol` instance for the selected protocol. The method creates a `DigitalSessionsBundle` for the pins used by the protocol with `TSMSessionManager` and passes it to `SetBundle`.

```csharp
IDigitalProtocol protocol = tsmContext.DutControl(CommunicationProtocol.SPI);

// Write the same value to the register on all sites.
protocol.WriteRegister(address: 0x48, value: 4);

// Read the register back from all sites.
SiteData<long> readBack = protocol.ReadRegister(address: 0x48);
```

> [!NOTE]
>
> - Call `DutControl` in each code module that accesses the DUT registers. Do not store the returned `IDigitalProtocol` instance and reuse it in a subsequent code module, because the instance is bound to the bundle of the code module that called `DutControl`.
> - Each protocol instance is a process-wide singleton that keeps a reference to the bundle of the most recent code module. Call `ClearBundle` once per test program after all register operations are complete, typically from the ProcessCleanup callback before the instrument sessions are closed. The example provides the `CleanupDigitalProtocol` test step for this purpose. Without this step, TestStand reports unreleased object references when it shuts down.
> - The protocol instances are not thread-safe. Do not use the same protocol instance from code modules that run in parallel.

### Writing Site-Unique Values

Pass a `SiteData<long>` object to `WriteRegister` to write a different value to each site.

```csharp
IDigitalProtocol protocol = tsmContext.DutControl(CommunicationProtocol.I2C);

// Provide one value per site, in site order.
SiteData<long> valuesPerSite = tsmContext.NewSiteData(new long[] { 1, 2, 3, 4 });
protocol.WriteRegister(address: 0x48, value: valuesPerSite);
SiteData<long> readBack = protocol.ReadRegister(address: 0x48);
```

### Accessing Multiple Registers in One Burst

Use `WriteRegisters` and `ReadRegisters` to access several registers with a single pattern burst. For writes, the addresses and values are interleaved in the source waveform. For reads, the source waveform contains the addresses, and the values are captured in the same order. The loop count sequencer register is set to the number of registers. A single burst can access up to 32 registers (`DigitalProtocol.MaxRegistersPerBurst`).

```csharp
IDigitalProtocol protocol = tsmContext.DutControl(CommunicationProtocol.SPI);

uint[] addresses = { 0x10, 0x11, 0x12 };
long[] values = { 0xA, 0xB, 0xC };

protocol.WriteRegisters(addresses, values);
SiteData<long[]> readBack = protocol.ReadRegisters(addresses);
```

To write a different value to each site, pass one `SiteData<long>` object per register to `WriteRegisters`.

## Pin Map and Digital Project Requirements

The pins in the pin map must include all of the pins used within the patterns. The pins for each protocol implementation are defined as follows:

| Protocol | Default pins | Source pin | Capture pin |
|---|---|---|---|
| SPI | `SDI`, `SDO` | `SDI` | `SDO` |
| I2C | `CS`, `SDI`, `SDO`, `SCK` | `SDI` | `SDI` |

> [!NOTE]
> The pins used by the bundle must include all of the pins that the pattern drives or compares. If your patterns use additional pins, such as chip select (`CS`) or clock (`SCK`), add them to the pin names for the protocol with `ConfigureDigitalProtocol`. You can also use this step to change the pin names to match your pin map. The example sequences set the pin names to `CS`, `SCK`, `SDI`, and `SDO` for both protocols.

The source and capture waveforms in the digital project must match the protocol settings. `SetBundle` creates the waveforms on each instrument session with the waveform names, pins, sample width, and bit order of the protocol, and creates the source waveform with site-unique data mapping. If the digital project already loaded a waveform with the same name but different settings, the driver reports an error. When more than one protocol uses the same instrument session, give each protocol its own waveform names, as the example does.

Load the pin levels, time sets, and specifications that apply to the protocol pins in ProcessSetup, with the same steps you use for any other digital pattern test program.

## Example Test Steps

The example provides the following test steps that demonstrate three register access scenarios. Each step gets the protocol with `DutControl`, performs the register operations, and publishes the results.

| Test step | Description | Published data IDs |
|---|---|---|
| `WriteValueToRegisterAndCompareReadbackValue` | Writes one value to a register on every site, reads it back, and compares each site's readback to the written value. | `RegisterValueReadback`, `ComparisonResult` |
| `WriteSiteUniqueValueToRegisterAndCompareReadbackValues` | Writes a different value to each site, reads the register back, and compares the readback values. | `RegisterValueReadback`, `ComparisonResult` |
| `WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues` | Writes one value per register in a single burst, reads all of the registers back in a single burst, and compares each one. | `Register_0x<address>_Readback`, `Register_0x<address>_Comparison` for every register |

The example also provides the following setup and cleanup steps:

| Test step | Description |
|---|---|
| `ConfigureDigitalProtocol` | Overrides the default settings of the selected protocol. Call it once per test program, before the protocol is used. |
| `CleanupDigitalProtocol` | Clears the reference that the selected protocol holds to the bundle, without closing the instrument sessions. Call it once from ProcessCleanup, before the instrument sessions are closed. |

## Examples

For the complete sequence-style example that demonstrates this workflow end-to-end, refer to the [Register IO (SPI and I2C) Example README](https://github.com/ni/semi-test-library-dotnet/blob/main/Examples/source/Sequence/RegisterIO.SPI-I2C/README.md).
