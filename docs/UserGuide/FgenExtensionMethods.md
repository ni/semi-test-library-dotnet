# FGen Extension Methods

The Semiconductor Test Library (STL) provides high-level extension methods for the `FgenSessionsBundle` class to configure, control, and generate standard waveforms on NI-FGEN (function generator) instruments.
These methods operate across all instrument sessions, pins, and sites in the bundle. Configuration methods apply settings to each channel mapped in the bundle, while control methods operate on the instrument session.

Standard waveform generation supports the following periodic waveforms:

- Sine
- Square
- Triangle
- Ramp Up
- Ramp Down
- DC
- Noise

> [!NOTE]
> User waveform and arbitrary waveform support is not yet available in STL.

The extension methods are defined in the `NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen` namespace.

```C#
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
```

## Creating an FGen Sessions Bundle

Use the `Fgen` method of the `TSMSessionManager` to create an `FgenSessionsBundle` object for the required pins or pin groups.

```C#
TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
FgenSessionsBundle fgenBundle = sessionManager.Fgen(pinsOrPinGroups);
```

> [!IMPORTANT]
> Channels that belong to the same FGen instrument share one driver session. Configure all pins of an instrument in the same bundle. Creating multiple bundles that share a channel, or a bundle that contains only some of the channels of an instrument, is not supported and throws a `NISemiconductorTestException`. For the supported PinMap and bundle configurations, refer to the FGen Standard Waveform Generation feature specification.

### Limitations

The current STL abstraction creates a single session per FGen instrument. Consequently, all control operations are applied at the instrument level and affect every channel associated with the session.

While channel-specific configuration is supported, channel-specific control is not. Therefore, use cases that require independent generation, triggering, initiation, abortion, or runtime control of individual channels within the same FGen instrument are not supported.

#### Supported PinMap and Bundle Configurations

|Case | Scenario                                                                           | SessionsBundle Contains                   | Number of Instruments                                  | Number of Sites | Support Status                              | Supported Multisite Option in TestStand|
|-----|------------------------------------------------------------------------------------|-------------------------------------------|--------------------------------------------------------|-----------------|---------------------------------------------|----------------------------------------|
|1    | Only one channel is mapped in the PinMap                                           | N/A                                       | Single or Multiple Instruments                         | Multiple Sites  |  <font color="Green">**Supported**</font>   | Any of the option                      |
|2    | Multiple channels are mapped to the same pin across sites                          | All channels of a given instrument        | Single or Multiple Instruments                         | Multiple Sites  |  <font color="Green">**Supported**</font>   | **One-thread only**                    |
|3    | Multiple channels are mapped to the same pin across sites                          | Subset of channels (split across bundles) | Single or Multiple Instruments                         | Multiple Sites  |  <font color="Red">**Not Supported**</font> | NA                                     |
|4    | Multiple channels are mapped to different pins                                     | All channels of a given instrument        | Single Instrument                                      | Single Site     |  <font color="Green">**Supported**</font>   | Any of the option                      |
|5    | Multiple channels are mapped to different pins                                     | Subset of channels (split across bundles) | Single Instrument                                      | Single Site     |  <font color="Red">**Not Supported**</font> | NA                                     |
|6    | Multiple channels are mapped to different pins                                     | All channels of a given instrument        | Multiple Instruments (one per site)                    | Multiple Sites  |  <font color="Green">**Supported**</font>   | Any of the option                      |
|7    | Multiple channels are mapped to different pins                                     | Subset of channels (split across bundles) | Multiple Instruments (one per site)                    | Multiple Sites  |  <font color="Red">**Not Supported**</font> | NA                                     |
|8    | Multiple channels are mapped to different pins and different sites across a module | All channels of a given instrument        | Multiple Instruments (channels straddled across sites) | Multiple Sites  |  <font color="Green">**Supported**</font>   | **One-thread only**                    |
|9    | Multiple channels are mapped to different pins and different sites across a module | Subset of channels (split across bundles) | Multiple Instruments (channels straddled across sites) | Multiple Sites  |  <font color="Red">**Not Supported**</font> | NA                                     |

> **NOTE:**
> When the scenarios involved channels from same instrument being shared across different sites, then the code has to be executed in single thread. So, if code module is called from an TestStand sequence, we need to ensure that Multisite Option in TestStand step is set to **One-thread only**.

### User Recommendations

- When creating the PinMap
  - Do not include channel-pin/site connection that are not being used.
  - Spread channels from same instrument to different pins that are likely to have common operations performed on it.
  - If you are only using one channel of a multi-channel FGen instrument, ensure the 2nd channel of the instrument is not mistakenly mapped to unused pins in connection list.
  - Avoid using the second channel whenever possible. If additional FGen instruments are available and currently unused, prefer using the first channel of an unused instrument rather than the second channel of an instrument that is already in use.

- When creating the bundle
  - Avoid creating multiple separate bundles for different sites, this case is not supported.
  - Avoid calling `FilterBySite` when operating on an `FgenSessionsBundle` object, unless absolutely certain that none of the pins in the bundle map their respective sites across channels of the same FGEN instrument. For example, if the site0 connection for PinA maps to channel0, and the site1 connection for PinA maps to channel1 of the same FGEN instrument, then this is not allowed and the `FilterBySite` method will throw an exception.
  - Avoid calling `ISemiconductorModuleContext.GetSiteSemiconductorModuleContexts()` or `ISemiconductorModuleContext.GetSemiconductorModuleContextWithSites(int[])` in your code module to obtain per-site or subset-site instances of `ISemiconductorModuleContext`. Creating an `FgenSessionsBundle` object from these derived tsmContext instances can result in unsupported bundle configurations and may cause exceptions during FGen bundle creation.
  - Ensure pins that are connected to the channels of same instrument are part of the same bundle.
  - Avoid creating multiple bundles with shared pins.

## Extension Methods

### Output Methods

| Method | Settings Type | Description |
| --- | --- | --- |
| `ConfigureOutputEnabled` | `bool`, `SiteData<bool>`, or `PinSiteData<bool>` | Enables or disables the output relay of each channel. Overloads accept the same value for all pins, per-site values, or per-pin, per-site values. |
| `ConfigureOutputMode` | `OutputMode` | Configures the output mode, such as function generation. This is a session-level setting and applies to all channels of the instrument. |

### Standard Waveform Methods

| Method | Settings Type | Description |
| --- | --- | --- |
| `ConfigureStandardWaveform` | `StandardWaveformSettings`, `SiteData<StandardWaveformSettings>`, or `PinSiteData<StandardWaveformSettings>` | Configures the standard waveform type, frequency, amplitude, DC offset, and start phase of each channel. |

`StandardWaveformSettings` has the following constructor. The `dcOffset` and `startPhase` parameters are optional and default to `0`.

```C#
new StandardWaveformSettings(StandardWaveform waveformFunctionType, double frequency, double amplitude, double dcOffset = 0, double startPhase = 0)
```

| Property | Unit | Description |
| --- | --- | --- |
| `WaveformFunctionType` | None | The standard waveform to generate, such as Sine, Square, Triangle, Ramp Up, Ramp Down, DC, or Noise. The `User` waveform type is not supported. |
| `Frequency` | Hertz | The frequency of the waveform. |
| `Amplitude` | Volts (peak-to-peak) | The peak-to-peak amplitude of the waveform. |
| `DcOffset` | Volts | The DC offset of the waveform. |
| `StartPhase` | Degrees | The horizontal offset of the waveform, in degrees of one cycle. |

> [!TIP]
> Settings are grouped into a `StandardWaveformSettings` object before they are passed to the STL method. Properties that are not set by the user keep their previously configured values in the driver.

### Clock Methods

| Method | Settings Type | Description |
| --- | --- | --- |
| `ConfigureClockMode` | `ClockMode` | Configures the clock mode of the device. This is a device-level setting. |
| `ConfigureSampleSource` | `string` | Configures the sample clock source. This is a device-level setting. |
| `ConfigureReferenceClock` | `string` source, `double` frequency | Configures the reference clock source and frequency. This is a device-level setting. |

### Trigger and Event Methods

| Method | Settings Type | Description |
| --- | --- | --- |
| `ConfigureStartTriggerDigitalEdge` | `string` source, `DigitalEdge` | Configures a digital edge start trigger from the specified source. |
| `ConfigureStartTriggerSoftwareEdge` | None | Configures a software edge start trigger. |
| `SendSoftwareEdgeTrigger` | None | Sends a software edge trigger to start generation on all channels that are waiting for the trigger. |
| `ConfigureTriggerMode` | `TriggerMode` | Configures the trigger mode of the device. |
| `DisableStartTrigger` | None | Disables the start trigger. |
| `ExportSignal` | `SignalSource` signalSource, `string` outputTerminal | Exports a signal from the instrument to the specified output terminal. |

### Control Methods

| Method | Return Type | Description |
| --- | --- | --- |
| `Commit` | `void` | Commits all configured settings to hardware so that parameter changes are immediately reflected. |
| `Initiate` | `void` | Starts waveform generation. This method does not wait for generation to complete. |
| `IsDone` | `bool[]` | Gets whether generation is complete, for each session in the bundle. |
| `WaitUntilDone` | `void` | Waits until generation is complete or the timeout expires. The default timeout is 10000 ms. |
| `Abort` | `void` | Aborts an in-progress generation. |

> [!NOTE]
> Control methods operate on the instrument session. Their effect applies to all enabled channels of that session, so they cannot start, stop, or wait on an individual channel.

### Utility Methods

| Method | Description |
| --- | --- |
| `Reset` | Resets the session to its default state. |
| `ResetDevice` | Resets the device, including the hardware state of all its channels. |

## Example

The following example configures a standard sine waveform on all channels of the bundle, enables the output, generates the waveform for 10 seconds, and then aborts generation.

```C#
public static void GenerateSineWaveform(ISemiconductorModuleContext tsmContext, string[] pinsOrPinGroups)
{
	TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
	FgenSessionsBundle fgenBundle = sessionManager.Fgen(pinsOrPinGroups);

	fgenBundle.ConfigureOutputMode(OutputMode.Function);
	fgenBundle.ConfigureStandardWaveform(new StandardWaveformSettings(
		waveformFunctionType: StandardWaveform.Sine,
		frequency: 1e3,
		amplitude: 1.0));
	fgenBundle.ConfigureOutputEnabled(true);

	fgenBundle.Initiate();
	PreciseWait(timeInSeconds: 10);
	fgenBundle.Abort();
}
```

> [!NOTE]
> Refer to the `FGenStandardWaveformGeneration` sequence example for a complete example that uses the `GenerateStandardWaveform` test step.
