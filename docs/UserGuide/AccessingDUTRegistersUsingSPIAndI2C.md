# Accessing DUT Registers Using SPI and I2C

Many DUTs expose configuration and status registers over serial protocols such as SPI and I2C. You can access these registers with an NI Digital Pattern instrument by combining the Semiconductor Test Library (STL) Digital Pattern Instrument Abstraction methods with generic template patterns. The template patterns use sequencer registers, source waveforms, and capture waveforms so that a single pattern can serve any register address, register value, and number of registers.

The [Register IO (SPI and I2C) Sequence Example](https://github.com/ni/semi-test-library-dotnet/blob/main/Examples/source/Sequence/RegisterIO.SPI-I2C/README.md) provides a reusable, protocol-agnostic implementation of this approach. The example encapsulates the following protocol-specific details behind a single `IDigitalProtocol` interface, so the same test code works with either protocol:

- Pattern bursting
- Source and capture waveforms
- Bit packing

> [!NOTE]
>
> - The `IDigitalProtocol` interface and the `DigitalProtocol`, `SPI`, and `I2C` classes are part of the example code, not the STL NuGet package. Copy the `DutControl` folder of the example into your test program and adapt it to your DUT.
> - The example is supported from Semiconductor Test Library 24.5.1 NuGet package or later.
> - The register addresses, values, patterns, and timing in the example sequences target a specific DUT. Adapt them to your DUT as described in [Adapting the Example to Your DUT](#adapting-the-example-to-your-dut).

## Architecture

The example implementation is organized into the following layers:

| Layer                       | Type                                              | Responsibility                                                                                                                                                 |
| --------------------------- | ------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Test steps                  | `TestStep` methods                                | Call the register access operations and publish the results. The `CommunicationProtocol` parameter selects SPI or I2C at runtime.                              |
| Protocol selection          | `SemiconductorModuleContextExtensions.DutControl` | Gets the protocol instance for the selected `CommunicationProtocol`, creates a `DigitalSessionsBundle` for the protocol pins, and binds it to the instance.    |
| Protocol-agnostic interface | `IDigitalProtocol`                                | Defines the register access operations and the protocol settings.                                                                                              |
| Core register access logic  | `DigitalProtocol` (abstract base class)           | Converts addresses and values into waveform samples, writes the sequencer registers and source waveform, bursts the pattern, and decodes the capture waveform. |
| Protocol-specific settings  | `SPI` and `I2C` (singleton classes)               | Define the default pattern names, waveform names, pins, and bit widths of each protocol.                                                                       |
| Instrument Abstraction      | `DigitalSessionsBundle` STL extension methods     | Program the NI Digital Pattern instrument.                                                                                                                     |

`IDigitalProtocol` defines the following register access operations:

| Method                                                      | Description                                                                           |
| ----------------------------------------------------------- | ------------------------------------------------------------------------------------- |
| `ReadRegister(uint address)`                                | Reads a single register and returns the value of each site as `SiteData<long>`.       |
| `WriteRegister(uint address, long value)`                   | Writes the same value to a single register on all sites.                              |
| `WriteRegister(uint address, SiteData<long> value)`         | Writes a site-unique value to a single register.                                      |
| `ReadRegisters(uint[] addresses)`                           | Reads multiple registers in a single burst and returns `SiteData<long[]>`.            |
| `WriteRegisters(uint[] addresses, long[] values)`           | Writes register-specific values to multiple registers in a single burst.              |
| `WriteRegisters(uint[] addresses, SiteData<long>[] values)` | Writes register-specific, site-unique values to multiple registers in a single burst. |

The `DigitalProtocol` class also provides overloads of each method that accept `addressBitWidth` and `valueBitWidth` parameters to override the default bit widths for a single operation.

### STL Methods Used

The example uses the following STL methods to program the instrument:

| STL Method                                            | Purpose                                                                                      |
| ----------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| `TSMSessionManager.Digital`                           | Creates a `DigitalSessionsBundle` for the protocol pins.                                     |
| `DigitalSessionsBundle.WriteSequencerRegister`        | Writes the transaction count, address bit width, and value bit width to sequencer registers. |
| `DigitalSessionsBundle.WriteSourceWaveformSiteUnique` | Writes the address and value samples to the source waveform for each site.                   |
| `DigitalSessionsBundle.BurstPattern`                  | Bursts the read or write template pattern and waits until it is done.                        |
| `DigitalSessionsBundle.FetchCaptureWaveform`          | Fetches the captured register value samples for each site.                                   |
| `DigitalSessionsBundle.Do`                            | Creates the serial source and capture waveforms with low-level NI-Digital driver calls.      |

**Related Information**:

- [Instrument Abstraction](InstrumentAbstraction.md)
- [Making Low-Level Driver Calls](advanced/MakingLowLevelDriverCalls.md)

## Digital Project Requirements

The template patterns, waveforms, and pin map must match the settings of the protocol instance in the code modules.

### Pin Map

The protocol pins must be DUT pins in the pin map, and must be connected to an NI Digital Pattern instrument for each site. The example uses the following pins:

| Pin   | SPI Use                    | I2C Use                                                                                      |
| ----- | -------------------------- | -------------------------------------------------------------------------------------------- |
| `CS`  | Chip select (active low)   | Not part of the I2C bus. The patterns can hold it at a static level if your DUT requires it. |
| `SDI` | Serial data into the DUT   | Serial data line (SDA) for both writing and reading                                          |
| `SDO` | Serial data out of the DUT | Not part of the I2C bus. The patterns can hold it at a static level if your DUT requires it. |
| `SCK` | Serial clock               | Serial clock line (SCL)                                                                      |

> [!NOTE]
>
> - The bundle that `DutControl` creates must contain all the pins in `PinNames`, as well as the source pin and the capture pin. Otherwise, `SetBundle` throws an exception.
> - For I2C, the SDA pin requires a pull-up resistor, unless your DUT board already provides one, because the patterns release the pin to check the ACK and NACK bits and to capture the register data.

### Template Patterns

Each protocol requires a read pattern and a write pattern. The patterns are generic: sequencer registers control the number of transactions and the bit widths, instead of fixed values in the patterns.

| Sequencer Register | Purpose                                          |
| ------------------ | ------------------------------------------------ |
| `reg0`             | Number of transactions in the burst (loop count) |
| `reg1`             | Number of address bits                           |
| `reg2`             | Number of value bits                             |

Each pattern repeats the following transaction `reg0` times:

| Pattern              | Transaction                                                                                                                               |
| -------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| `SPI_write_template` | `CS` low, write bit, register address, value, `CS` high.                                                                                  |
| `SPI_read_template`  | `CS` low, read bit, register address, captured value, `CS` high.                                                                          |
| `I2C_write_template` | START, device address with the write bit, register address, value, STOP. The DUT acknowledges each byte.                                  |
| `I2C_read_template`  | START, device address with the write bit, register address, repeated START, device address with the read bit, captured value, NACK, STOP. |

The patterns generate or check the protocol framing bits, such as the SPI read or write bit, the I2C device address, and the I2C ACK and NACK bits. Therefore, the source waveform contains only the register address and value bits. Because the I2C device address is part of the pattern, update the I2C patterns to match the device address of your DUT.

### Source and Capture Waveforms

The write patterns source the register address and the value from the source waveform. The read patterns source only the register address and capture the value into the capture waveform.

| Protocol | Source Waveform (Pin)       | Capture Waveform (Pin)       |
| -------- | --------------------------- | ---------------------------- |
| SPI      | `SPI_source_buffer` (`SDI`) | `SPI_capture_buffer` (`SDO`) |
| I2C      | `I2C_source_buffer` (`SDI`) | `I2C_capture_buffer` (`SDI`) |

The waveforms must meet the following requirements:

- **Serial waveforms**: The waveforms are serial waveforms with the sample width and bit order configured on the protocol instance. The example uses a sample width of 1 bit and the most significant bit first.
- **Site-unique source data mapping**: The source waveform must always be site-unique so that the same waveform supports both broadcast and site-unique writes.
- **Unique waveform names per protocol**: Each protocol uses its own waveform names because both protocols share the same instrument session.
- **Do not apply the waveform file data**: The NI-Digital driver fixes the size of a source waveform on its first write in a session. `DigitalProtocol` pads every write to the same size, so the instrument setup must not apply the source waveform data from the waveform files. Keep the `applySourceWaveformData` parameter of the `SetupNIDigitalPatternInstrumentation` step at its default value, `False`.

> [!NOTE]
> Waveform settings cannot change after the waveform is created on a session. If you change the sample width, bit order, or pin of a protocol after its waveforms are created, use different waveform names.

### Address Bit Width

The address bit width is the number of register address bits that the source waveform supplies. It does not include the framing bits that the patterns send, so set it based on how your DUT frames each transaction:

- **SPI**: If the DUT combines the read or write bit and the register address in the same command word, the pattern sends the read or write bit, and the address bit width is the number of remaining address bits in that command word.
- **I2C**: The read or write bit is part of the device address byte, which the patterns send. Therefore, the address bit width is the full width of the register address.

Pass only the address bits that the source waveform supplies to the register access methods.

> [!NOTE]
> The example I2C patterns expect an ACK after every 8 bits, so use an 8-bit address and an 8-bit value with these patterns.

## Theory of Operation

Each register access operation performs a single pattern burst. For example, the `WriteRegister` method performs the following actions:

1. Converts the address and value into waveform samples, based on the bit widths, sample width, and bit order.
2. Writes the transaction count (`reg0`), address bit width (`reg1`), and value bit width (`reg2`) to the sequencer registers.
3. Writes the samples to the site-unique source waveform. For a broadcast write, every site receives the same samples.
4. Bursts the write pattern with a 10-second timeout.

The `ReadRegister` method sources only the address samples, bursts the read pattern, and then fetches the capture waveform and converts the samples into a single value for each site.

The multiple-register methods set the transaction count to the number of registers so that the pattern loops once per register within a single burst:

- `WriteRegisters` interleaves the address and value samples of each register in the source waveform.
- `ReadRegisters` places the address samples of each register back to back in the source waveform, and then splits the captured samples into one value per register.

## Implementation Workflow

### Setup Requirements

Complete the following steps in the `ProcessSetup` sequence of the test program:

1. **Initialize the NI Digital Pattern instrument sessions** with the `SetupAndCleanupSteps.SetupNIDigitalPatternInstrumentation` step. This step loads the files in the digital project and applies the levels and timing sheets that you specify. In the example sequences, the **Setup NI Digital Instrumentation** step applies the `STLExample.RegisterIO.SPIAndI2C` levels and timing sheets.
2. **Configure the protocol** with the `ConfigureDigitalProtocol` test step. Call it once per test program before you use any protocol instance. This step is required only if you need to change a default setting of the protocol, such as the bit widths, sample width, or pin names.

> [!NOTE]
> The default sample width of the `DigitalProtocol` class is 8 bits, which does not match the 1-bit sample width of the waveform files in the example digital project. Therefore, the example sequences call `ConfigureDigitalProtocol` to set the sample width to 1 bit.

### Test Code Workflow

1. **Get the protocol instance** with the `DutControl` extension method in each code module that accesses the DUT registers.
2. **Perform the register access operations**, such as `WriteRegister` and `ReadRegister`.
3. **Compare and publish the results** with the STL `Compare` and `PublishResults` methods.

> [!NOTE]
>
> - Do not store the returned `IDigitalProtocol` instance for reuse in a subsequent code module. The instance is bound to the bundle of the code module that called `DutControl`.
> - The protocol instances are process-wide singletons and are not thread-safe. Do not invoke them in parallel. Refer to [Concurrent Code Execution](advanced/ConcurrentCodeExecution.md) for more details.

### Cleanup Requirements

Complete the following steps in the `ProcessCleanup` sequence of the test program:

1. **Clear the bundle reference** of each protocol instance that you used with the `CleanupDigitalProtocol` test step. Without this step, TestStand reports unreleased object references when it shuts down.
2. **Close the instrument sessions** with the **Cleanup Instrumentation** step.

## Example Usage

The following code demonstrates the typical workflow. These examples are based on the [Register IO (SPI and I2C) Sequence Example](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/Sequence/RegisterIO.SPI-I2C).

### Configuring the Protocol (User Code in ProcessSetup)

Override the default settings of the protocol once per test program. Any parameter that you leave at its default value (an empty string, `-1`, or an empty pin list) uses the default setting of the protocol. Set the bit widths to match the registers of your DUT, as described in [Address Bit Width](#address-bit-width).

```csharp
TestStep.ConfigureDigitalProtocol(
    CommunicationProtocol.SPI,
    addressBitWidth: 7,
    valueBitWidth: 8,
    sampleWidth: 1,
    pinNames: new[] { "CS", "SCK", "SDI", "SDO" });

TestStep.ConfigureDigitalProtocol(
    CommunicationProtocol.I2C,
    addressBitWidth: 8,
    valueBitWidth: 8,
    sampleWidth: 1,
    pinNames: new[] { "CS", "SCK", "SDI", "SDO" });
```

### Writing and Reading a Single Register (Test Method)

Write the same value to a single register on all sites, and then read it back:

```csharp
public static void WriteValueToRegisterAndCompareReadbackValue(
    ISemiconductorModuleContext tsmContext,
    CommunicationProtocol protocol,
    uint registerAddress,
    long valueToWrite)
{
    IDigitalProtocol digitalProtocol = tsmContext.DutControl(protocol);

    digitalProtocol.WriteRegister(registerAddress, valueToWrite);
    SiteData<long> regValueReadBack = digitalProtocol.ReadRegister(registerAddress);

    SiteData<bool> comparisonResults = regValueReadBack.Compare(ComparisonType.EqualTo, valueToWrite);
    tsmContext.PublishResults(regValueReadBack, "RegisterValueReadback");
    tsmContext.PublishResults(comparisonResults, "ComparisonResult");
}
```

### Writing Site-Unique Values (Test Method)

Write a unique value to a single register on each site by passing `SiteData<long>`:

```csharp
public static void WriteSiteUniqueValueToRegisterAndCompareReadbackValues(
    ISemiconductorModuleContext tsmContext,
    CommunicationProtocol protocol,
    uint registerAddress,
    long[] perSiteValuesToWrite)
{
    SiteData<long> regValues = tsmContext.NewSiteData(perSiteValuesToWrite);
    IDigitalProtocol digitalProtocol = tsmContext.DutControl(protocol);

    digitalProtocol.WriteRegister(registerAddress, regValues);
    SiteData<long> regValueReadBack = digitalProtocol.ReadRegister(registerAddress);

    SiteData<bool> comparisonResults = regValueReadBack.Compare(ComparisonType.EqualTo, regValues);
    tsmContext.PublishResults(regValueReadBack, "RegisterValueReadback");
    tsmContext.PublishResults(comparisonResults, "ComparisonResult");
}
```

### Writing and Reading Multiple Registers (Test Method)

Write register-specific values to multiple registers in a single burst, and then read all the registers back in a second burst:

```csharp
public static void WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(
    ISemiconductorModuleContext tsmContext,
    CommunicationProtocol protocol,
    uint[] registerAddresses,
    long[] valuesToWrite)
{
    IDigitalProtocol digitalProtocol = tsmContext.DutControl(protocol);

    digitalProtocol.WriteRegisters(registerAddresses, valuesToWrite);
    SiteData<long[]> regValuesReadBack = digitalProtocol.ReadRegisters(registerAddresses);

    for (int i = 0; i < registerAddresses.Length; i++)
    {
        int index = i;
        SiteData<long> singleRegReadBack = regValuesReadBack.Select(x => x[index]);
        SiteData<bool> comparisonResult = singleRegReadBack.Compare(ComparisonType.EqualTo, valuesToWrite[i]);
        tsmContext.PublishResults(singleRegReadBack, $"Register_0x{registerAddresses[i]:X2}_Readback");
        tsmContext.PublishResults(comparisonResult, $"Register_0x{registerAddresses[i]:X2}_Comparison");
    }
}
```

> [!NOTE]
> A single burst can access up to 32 registers (`DigitalProtocol.MaxRegistersPerBurst`).

### Overriding the Bit Widths for a Single Operation (Test Method)

Use the `DigitalProtocol` overloads to override the default bit widths for a single operation, such as reading a 16-bit register over SPI:

```csharp
var digitalProtocol = (DigitalProtocol)tsmContext.DutControl(CommunicationProtocol.SPI);
SiteData<long> value = digitalProtocol.ReadRegister(address: 0x10, addressBitWidth: 7, valueBitWidth: 16);
```

### Cleaning Up the Protocol (User Code in ProcessCleanup)

Clear the bundle reference of each protocol instance before the instrument sessions are closed:

```csharp
TestStep.CleanupDigitalProtocol(CommunicationProtocol.SPI);
TestStep.CleanupDigitalProtocol(CommunicationProtocol.I2C);
```

## Adapting the Example to Your DUT

Review the following items when you adapt the example to your DUT:

- **Pins**: Update the pin map and the `pinNames` parameter of `ConfigureDigitalProtocol` to match the pins of your DUT.
- **Patterns**: Update the template patterns for the framing that your DUT requires, such as the I2C device address or the SPI chip select polarity. Keep the sequencer register loop and the source and capture opcodes so that the code modules continue to work.
- **Levels and timing**: Update the specifications, levels, and time sets to match the voltage levels and clock rate of your DUT.
- **Bit widths**: Set the address and value bit widths to match the registers of your DUT. The address and value bit widths must be between 1 and 64, and each address and value must fit in its bit width.
- **Register addresses and test limits**: Update the register addresses, the values to write, and the expected readback values that the sequence passes to each step. If you change the register addresses of a multiple-register step, also update the measurement IDs of its test evaluations because the published data IDs contain the register addresses.

> [!NOTE]
> A simulated instrument has no DUT to respond, so you cannot verify the register readback values in Offline Mode.

## Examples

For the complete sequence-style example that demonstrates this workflow end-to-end, including the digital project, pin map, and sequence files for both protocols, refer to the [Register IO (SPI and I2C) Sequence Example README](https://github.com/ni/semi-test-library-dotnet/blob/main/Examples/source/Sequence/RegisterIO.SPI-I2C/README.md).

When the example is installed with STS Software, it is located under the following directory:
`C:\Users\Public\Documents\National Instruments\NI_SemiconductorTestLibrary\Examples\Sequence\RegisterIO.SPI-I2C`.
