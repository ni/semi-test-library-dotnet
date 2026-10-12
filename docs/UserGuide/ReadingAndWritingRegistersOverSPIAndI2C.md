# Reading and Writing Registers over SPI and I2C

The Semiconductor Test Library (STL) provides digital instrument abstraction for NI Digital Pattern instruments that you can use to read and write DUT registers over serial protocols, such as SPI and I2C. A register access consists of burst of a pattern that sources the address and value bits from a source waveform and, for reads, captures the value bits into a capture waveform.

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
4. **Code modules**: Convert the address and value to waveform samples, configure the sequencer registers, burst the pattern, and convert any captured samples back to a value.

### Sequencer Registers

The patterns in the example use the following sequencer registers. The code modules write these registers before every burst.

| Sequencer register | Purpose |
|---|---|
| `reg0` | Number of transactions in the burst (the loop count). |
| `reg1` | Number of address bits. |
| `reg2` | Number of value bits. |

### Source and Capture Waveforms

The patterns source the address and value bits on the serial data in (`SDI`) pin from a source waveform. The read patterns capture the value bits on the serial data out (`SDO`) pin into a capture waveform. The code converts each address and value into samples using a configurable sample width (default 8 bits) with the most significant bit first.

The source waveform is created as site-unique. This supports writing both a broadcast value, where all sites receive the same value, and site-unique values, where each site receives a different value.

## Protocol Abstraction

The example separates the protocol-independent register IO logic from the protocol-specific settings.

| Type | Description |
|---|---|
| `IDigitalProtocol` | The protocol-agnostic contract for reading and writing registers. |
| `DigitalProtocol` | Abstract base class that implements the register read and write logic. |
| `SPI` and `I2C` | Singleton protocol implementations. Each sets its pattern names and the pins it requires. |
| `CommunicationProtocol` | Enumeration used to select `SPI` or `I2C`. |
| `SemiconductorModuleContextExtensions.DutControl` | Extension method that creates a protocol instance bound to the current context. |

`IDigitalProtocol` exposes the following operations:

- `ReadRegister` and `ReadRegisters`
- `WriteRegister` and `WriteRegisters`, each with an overload for a broadcast value and an overload for site-unique values
- `SetBundle` and `ReleaseBundle`

It also exposes the settings used by the implementation, such as the pattern names, waveform names, sequencer register names, bit widths, and bit order.

### Default Settings

| Setting | Default |
|---|---|
| Sample width | 8 bits |
| Bit order | Most significant bit first |
| Address bit width | 16 bits |
| Value bit width | 16 bits |
| Burst timeout | 10 seconds |

You can override the defaults for your DUT, for example by using the `ConfigureDigitalProtocol` test step from the example. Call it once per test program, before any protocol instance is used.

## Using the Protocol in Test Code

Use the `DutControl` extension method on the `ISemiconductorModuleContext` object to get an `IDigitalProtocol` instance for the selected protocol. The method creates a `DigitalSessionsBundle` for the pins used by the protocol with `TSMSessionManager` and passes it to `SetBundle`.

```csharp
IDigitalProtocol protocol = tsmContext.DutControl(CommunicationProtocol.SPI);
try
{
    // Write the same value to the register on all sites.
    protocol.WriteRegister(address: 0x48, value: 4);

    // Read the register back from all sites.
    SiteData<long> readBack = protocol.ReadRegister(address: 0x48);
}
finally
{
    protocol.ReleaseBundle();
}
```

> [!NOTE]
> Call `ReleaseBundle` in a `finally` block after the register operations and result publishing. It releases the reference to the bundle of the current code module without disposing the instrument sessions.

### Writing Site-Unique Values

Pass a `SiteData<long>` object to `WriteRegister` to write a different value to each site.

```csharp
IDigitalProtocol protocol = tsmContext.DutControl(CommunicationProtocol.I2C);
try
{
    var valuesPerSite = new SiteData<long>(new long[] { 1, 2, 3, 4 });
    protocol.WriteRegister(address: 0x48, value: valuesPerSite);
    SiteData<long> readBack = protocol.ReadRegister(address: 0x48);
}
finally
{
    protocol.ReleaseBundle();
}
```

### Accessing Multiple Registers in One Burst

Use `WriteRegisters` and `ReadRegisters` to access several registers with a single pattern burst. The addresses and values are interleaved in the source waveform, and the loop count sequencer register is set to the number of registers.

```csharp
IDigitalProtocol protocol = tsmContext.DutControl(CommunicationProtocol.SPI);
try
{
    uint[] addresses = { 0x10, 0x11, 0x12 };
    long[] values = { 0xA, 0xB, 0xC };

    protocol.WriteRegisters(addresses, values);
    SiteData<long[]> readBack = protocol.ReadRegisters(addresses);
}
finally
{
    protocol.ReleaseBundle();
}
```

## Pin Map and Digital Project Requirements

The pins in the pin map must include all of the pins used within the patterns. The pins for each protocol implementation are defined as follows:

| Protocol | Default pins | Source pin | Capture pin |
|---|---|---|---|
| SPI | `SDI`, `SDO` | `SDI` | `SDO` |
| I2C | `SDI`, `SDO`, `SCK` | `SDI` | `SDO` |

> [!NOTE]
> The pins used by the bundle must include all of the pins that the pattern drives or compares. If your patterns use additional pins, such as chip select (`CS`) or clock (`SCK`), add them to the pin names for the protocol with `ConfigureDigitalProtocol`. You can also use this step to change the pin names to match your pin map.

Load the pin levels, time sets, and specifications that apply to the protocol pins in ProcessSetup, with the same steps you use for any other digital pattern test program.

## Example Test Steps

The example provides the following test steps that demonstrate three register access scenarios. Each step creates the protocol with `DutControl`, performs the register operations, publishes the results, and releases the bundle.

| Test step | Description | Published data IDs |
|---|---|---|
| `WriteValueToRegisterAndCompareReadbackValue` | Writes one value to a register on every site, reads it back, and compares each site's readback to the written value. | `RegisterValueReadback`, `ComparisonResult` |
| `WriteSiteUniqueValueToRegisterAndCompareReadbackValues` | Writes a different value to each site, reads the register back, and compares the readback values. | `RegisterValueReadback`, `ComparisonResult` |
| `WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues` | Writes one value per register in a single burst, reads all of the registers back, and compares each one. | `Register_0x<address>_Readback`, `Register_0x<address>_Comparison` for every register |

## Examples

For the complete sequence-style example that demonstrates this workflow end-to-end, refer to the [Register IO (SPI and I2C) Example README](https://github.com/ni/semi-test-library-dotnet/blob/main/Examples/source/Sequence/RegisterIO.SPI-I2C/README.md).
