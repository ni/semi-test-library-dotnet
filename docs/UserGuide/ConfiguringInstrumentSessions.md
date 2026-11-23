# Configuring Instrument Sessions

The Semiconductor Test Library can help you efficiently configure instruments by consolidating the most commonly configured driver properties into a class. You can create and configure an instance of this class with only the properties you intend to update. The resulting object can then be passed as an input parameter to the appropriate extension method(s) in the library, which will operate on the object to update the targeted driver properties in one go. Other properties in the object not explicitly set will be ignored. This minimizes the number of parallel for loops, thus reducing overhead and the overall code execution time.

> [!NOTE]
> Extension methods that configure driver properties will always abort the driver session before updating a property.
>
> By default, extension methods that configure driver properties do not re-initiate or perform a commit of the driver session. Such operations are expected to be applied in proceeding code. If you want to force the driver properties to be applied immediately, either pass the optional `updateMode` parameter to the configuration method, or call the high-level `Commit` or `Initiate` extension method. Refer to [Controlling When Settings Are Applied](#controlling-when-settings-are-applied) for more details.

**Related Concepts:**

- [Making Low-Level Driver Calls](advanced/MakingLowLevelDriverCalls.md)
- [NI TSM: Parallel For Loops](https://www.ni.com/docs/en-US/bundle/teststand-semiconductor-module/page/parallel-for-loops.html)
- [NI DCPower: Programming Flow](https://www.ni.com/docs/en-US/bundle/ni-dcpower/page/programming-flow.html)
- [NI DCPower: Programming States](https://www.ni.com/docs/en-US/bundle/ni-dcpower/page/programming-states.html)
- [NI Digital: Session State Model](https://www.ni.com/docs/en-US/bundle/ni-digital-pattern/page/programming-states.html)

## Configuring DCPower Settings

The DCPower Extensions provide two main classes to configure DCPower settings: `DCPowerSourceSettings` and `DCPowerMeasureSettings`.

### Source Settings

Use the `DCPowerSourceSettings` class to configure common settings related to sourcing constant voltage or current in Single Point mode with NI-DCPower instruments.

Example Usage:

```C#
var pinNames = new string[] { "PinA", "PinB" };
var sessionManager = new TSMSessionManager(semiconductorModuleContext);
var dcPowerPins = sessionManager.DCPower(pinNames);

var sourceSettings = new DCPowerSourceSettings
{
    Level = 0.01,
    Limit = 1,
    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
};

dcPowerPins.ConfigureSourceSettings(sourceSettings);
dcPowerPins.Initiate();

// OR Simply
// dcPowerPins.ForceCurrent(sourceSettings);
```

> [!NOTE]
> Refer to the API Reference for more details regarding the `DCPowerSourceSettings` class.
>
> Class: `DCPowerSourceSettings` \
> Namespace: `NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower` \
> Assembly: `NationalInstruments.SemiconductorTestLibrary.Extensions.dll`

### Measure Settings

Use the `DCPowerMeasureSettings` class to configure common measurement settings when operating in constant voltage or current mode with NI-DCPower instruments.

Example Usage:

```C#
var sessionManager = new TSMSessionManager(semiconductorModuleContext);
var dcPowerPins = sessionManager.DCPower("DutPinA");
var measureSettings = new DCPowerMeasureSettings()
{
    ApertureTime = 0.01,
    ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.Seconds,
    Sense = DCPowerMeasurementSense.Remote,
    MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete,
};

dcPowerPins.ConfigureMeasureSettings(measureSettings);
var currentMeasurements = dcPowerPins.MeasureCurrent()
```

> [!NOTE]
> Refer to the API Reference for more details regarding the `DCPowerMeasureSettings` class.
>
> Class: `DCPowerMeasureSettings` \
> Namespace: `NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower` \
> Assembly: `NationalInstruments.SemiconductorTestLibrary.Extensions.dll`

## Configuring Digital Settings

Use the `PPMUSettings` class to configure common PPMU-related settings with NI Digital Pattern instruments.

Example Usage:

```C#
var pinNames = new string[] { "PinA", "PinB" };
var sessionManager = new TSMSessionManager(tsmContext);
var ppmuPins = sessionManager.Digital(pinNames);
var pinASettings = new PPMUSettings
{
    VoltageLevel = 0.01,
    CurrentLimitRange = 1,
    OutputFunction = PpmuOutputFunction.DCCurrent
};
var pinBSettings = new PPMUSettings
{
    VoltageLevel = 3.3,
    CurrentLimitRange = 0.01,
    OutputFunction = PpmuOutputFunction.DCVoltage,
};
var ppmuSettings = new Dictionary<string, PPMUSettings>()
{
    [pinNames[0]] = pinASettings,
    [pinNames[1]] = pinBSettings,
};

ppmuPins.ForceVoltage(ppmuSettings);
```

> [!NOTE]
> Refer to the API Reference for more details regarding the `PPMUSettings` class.
>
> Class: `PPMUSettings` \
> Namespace: `NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital` \
> Assembly: `NationalInstruments.SemiconductorTestLibrary.Extensions.dll`

## Controlling When Settings Are Applied

Most NI instrument drivers cache property values and only apply them to the hardware when the session is committed or initiated. This can make it difficult to debug hardware configuration changes, and invalid configurations may only raise an exception at a later stage in the test code.

To address this, many DCPower and Digital configuration extension methods accept an optional `updateMode` parameter that specifies when the configured settings take effect:

| Value | Behavior |
| -- | -- |
| `UpdateMode.Deferred` (default) | The session is aborted and the driver property is set. The update is deferred until the next commit or initiate operation. This preserves the existing behavior when the parameter is omitted. |
| `UpdateMode.Commit` | The session is aborted, the driver property is set, and the update is committed to the instrument. The result may not be reflected in the output of the device until the session has been initiated. |
| `UpdateMode.Immediate` | The session is aborted, the driver property is set, and the session is then initiated. |

Example Usage:

```C#
var sessionManager = new TSMSessionManager(semiconductorModuleContext);
var dcPowerPins = sessionManager.DCPower("DutPinA");
var digitalPins = sessionManager.Digital("DutPinB");

// Commit the measure settings to the hardware right away to validate them.
dcPowerPins.ConfigureMeasureSettings(measureSettings, UpdateMode.Commit);

// Apply the source settings and initiate the SMU.
dcPowerPins.ConfigureSourceSettings(sourceSettings, UpdateMode.Immediate);

// Commit the digital voltage levels without bursting a pattern.
digitalPins.ConfigureVoltageLevels(vil: 0, vih: 3.3, vol: 1.6, voh: 1.7, vterm: 2.0, updateMode: UpdateMode.Commit);
```

> [!NOTE]
> When used with ganged DCPower pins, the commit or initiate operation is performed in a way that preserves the ganging configuration.
>
> The `UpdateMode` enum is defined separately in the DCPower and Digital namespaces: \
> `NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower.UpdateMode` \
> `NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.UpdateMode`

The following **DCPower** methods support the `UpdateMode` parameter:

- `ConfigureSourceSettings`
- `ConfigureCurrentLimit`
- `ConfigureCurrentLimits`
- `ConfigureOutputConnected`
- `ConfigureOutputEnabled`
- `ConfigureSourceDelay`
- `ConfigureMeasureSettings`
- `ConfigureMeasureWhen`
- `ConfigureMeasurementSense`
- `ConfigureCurrentLevel`
- `ConfigureCurrentLevelRange`
- `ConfigureCurrentLimitHigh`
- `ConfigureCurrentLimitLow`
- `ConfigureCurrentLimitRange`
- `ConfigureLimitSymmetry`
- `ConfigureVoltageLevel`
- `ConfigureVoltageLevelRange`
- `ConfigureVoltageLimit`
- `ConfigureVoltageLimitHigh`
- `ConfigureVoltageLimitLow`
- `ConfigureVoltageLimitRange`
- `ConfigureApertureTimeInSeconds`

The following **Digital** methods support the `UpdateMode` parameter:

- `ConfigureSingleLevel`
- `ConfigureVoltageLevels`
- `ConfigureTerminationMode`
- `ConfigureTimeSetPeriod`
- `ConfigureTimeSetDriveEdges`
- `ConfigureTimeSetEdge`
- `ConfigureTimeSetCompareEdgesStrobe`

### Digital Commit, Initiate, and Pattern Start Label

The Digital extensions also provide bundle-level methods that mirror the equivalent DCPower methods:

- `Commit()`: Applies all previously configured pin levels, termination modes, clocks, triggers, and pattern timing, moving the session to the committed state.
- `Initiate()`: Commits the properties, moves the session to the running state, and bursts the currently configured pattern.
- `ConfigurePattern(string startLabel)`: Sets the pattern start label and configures the pattern burst sites for the sites represented by the bundle. To target a subset of sites, filter the bundle by site before calling this method.
- `GetPatternStartLabel()`: Returns the currently configured pattern start label for each session.

```C#
var digitalPins = sessionManager.Digital(digitalPinNames);
digitalPins.ConfigurePattern("StartLabel");
digitalPins.Commit();
var startLabels = digitalPins.GetPatternStartLabel();
digitalPins.Initiate();
```

> [!NOTE]
> The `ConfigurePattern` method does not contain the sites to configure. By default, it operates on all the sites in the bundle. To operate only on specific sites, filter the bundle with the desired sites and then call this method on the resulting bundle. 
>
> ```C#
> var digitalPins = sessionManager.Digital(digitalPinNames);
> var filteredDigitalPins = digitalPins.FilterBySites(new int[] { 1, 2 });
> filteredDigitalPins.ConfigurePattern("StartLabel");
> filteredDigitalPins.Commit();
> var startLabels = filteredDigitalPins.GetPatternStartLabel();
> filteredDigitalPins.Initiate();
> ```
