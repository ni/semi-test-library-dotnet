# Scope Extension Methods

The Semiconductor Test Library (STL) provides high-level extension methods for the `ScopeSessionsBundle` class to configure, control, and acquire waveforms from NI-SCOPE instruments.
These methods operate in parallel across all instrument sessions, pins, and sites in the bundle, and they accept and return pin- and site-aware data types, such as `SiteData` and `PinSiteData`.

The extension methods are defined in the `NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope` namespace.

```C#
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
```

> [!NOTE]
> Refer to the [API Reference](../SemiconductorTestLibrary/) for the complete list of method overloads and parameter details.

## Creating a Scope Sessions Bundle

Use the `Scope` method of the `TSMSessionManager` to create a `ScopeSessionsBundle` object for the required pins or pin groups.

```C#
TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);
```

## Configuration Methods

| Method | Settings Type | Description |
| --- | --- | --- |
probe attenuation, channel enable state, and time-interleaved sampling.
| `ConfigureElectricalCharacteristics` | `ElectricalCharacteristics` | Configures the input impedance and maximum input frequency. Overloads accept the same settings for all pins, per-site settings (`SiteData<ElectricalCharacteristics>`), or per-pin, per-site settings (`PinSiteData<ElectricalCharacteristics>`). |
| `ConfigureTiming` | `TimingSettings` | Configures the minimum sample rate, minimum number of points, reference position, number of records, and real-time enforcement. |
| `ConfigureClock` | `ClockSettings` | Configures the input clock source, output clock source, clock synchronization pulse source, and whether the instrument is the master. |
Configures the acquisition type for all sessions in the bundle.

> [!TIP]
> Settings are grouped into class objects before they are passed to the STL method. This ensures the settings are applied as efficiently as possible by the instrument driver.

## Trigger Methods

| Method | Settings Type | Description |
| --- | --- | --- |
| `ConfigureEdgeTrigger` | `EdgeTriggerSettings` | Configures an edge trigger with a trigger level, slope, and coupling. |
| `ConfigureTriggerHysteresis` | `HysteresisTriggerSettings` | Configures a hysteresis trigger with a trigger level, hysteresis, slope, and coupling. |
| `ConfigureTriggerDigital` | `DigitalTriggerSettings` | Configures a digital trigger with a trigger slope. |
| `ConfigureTriggerWindow` | `WindowTriggerSettings` | Configures a window trigger with low and high window levels, window mode, and coupling. |
| `ConfigureTriggerImmediate` | None | Configures the instrument to trigger immediately after it is initiated. |

All trigger settings types inherit the `HoldOff` and `Delay` properties from the `TriggerSettings` base class.

## Control Methods

| Method | Description |
| --- | --- |
| `Initiate` | Initiates a waveform acquisition. |
| `Abort` | Aborts an in-progress acquisition. |
| `Commit` | Commits all configured settings to hardware so that parameter changes are immediately reflected. |
| `AutoSetup` | Automatically configures the instrument settings, such as the vertical range, based on the detected input signal. |

## Acquisition Methods

| Method | Return Type | Description |
| --- | --- | --- |
| `FetchWaveform` | `PinSiteData<AnalogWaveform<double>>` | Fetches the acquired waveform from a previously initiated acquisition. |
| `ReadWaveform` | `PinSiteData<AnalogWaveform<double>>` | Initiates an acquisition, waits for it to complete, and returns the acquired waveform. |
| `Read` | `PinSiteData<double[]>` | Initiates an acquisition, waits for it to complete, and returns the acquired raw samples. |

> [!NOTE]
> If an acquisition fails or does not complete within the specified timeout, the resulting `NISemiconductorTestException` includes the site, pin, and channel details for every channel on the affected card, because a single site losing its trigger signal prevents the entire card from acquiring data.

## Query Methods

The following methods return one value per instrument session in the bundle.

| Method | Return Type | Description |
| --- | --- | --- |
| `GetAcquisitionStatus` | `ScopeAcquisitionStatus[]` | Gets the acquisition status. |
| `GetRecordLength` | `long[]` | Gets the record length. |
| `GetResolution` | `long[]` | Gets the resolution, in bits. |
| `GetSampleMode` | `ScopeSampleMode[]` | Gets the sample mode. |
| `GetSampleRate` | `double[]` | Gets the effective sample rate, in samples per second. |
| `GetNumberOfRecordsToAcquire` | `long[]` | Gets the number of records to acquire. |

## Example

The following example configures the vertical and electrical characteristics, configures an edge trigger, initiates an acquisition, and fetches the acquired waveforms.

```C#
public static void ConfigureEdgeTriggerInitiateAndFetch(ISemiconductorModuleContext tsmContext, string pinOrPinGroup)
{
	TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
	ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);

	scopeBundle.ConfigureVertical(new VerticalSettings
	{
		Range = 1.0,
		Offset = 0.0,
		ProbeAttenuation = 1.0,
	});
	scopeBundle.ConfigureElectricalCharacteristics(new ElectricalCharacteristics
	{
		InputImpedance = 1e6,
	});
	scopeBundle.Commit();

	scopeBundle.ConfigureEdgeTrigger(new EdgeTriggerSettings
	{
		HoldOff = PrecisionTimeSpan.FromSeconds(0.0),
		Delay = PrecisionTimeSpan.FromSeconds(0.0),
		TriggerLevel = 0.0,
		TriggerSlope = ScopeTriggerSlope.Positive,
		TriggerCoupling = ScopeTriggerCoupling.DC,
	});

	scopeBundle.Initiate();
	PinSiteData<AnalogWaveform<double>> waveforms = scopeBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(5.0), 1000);
}
```

> [!NOTE]
> Refer to the `ScopeAcquisition` sequence example for complete examples using edge, hysteresis, digital, window, and immediate triggers.
