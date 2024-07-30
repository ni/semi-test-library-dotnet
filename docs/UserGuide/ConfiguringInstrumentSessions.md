# Configuring Instrument Sessions

The Semiconductor Test Library attempts to configure instruments in the most efficient way possible by consolidating the most commonly set driver properties into a class. An instance of this class can be created and configured with only the properties intending to be updated. The resulting object can then be passed as an input parameter the appropriate extension method(s) in the library, which will them operate on that object to update the targeted driver properties in one go. All other properties not explicitly set will be ignored. This minimizes the use of parallel for loops that get called under-the-hood and thus reduces the overall test time.

> [!NOTE]
> Extension methods that configure driver properties the driver session will always be aborted before the property is updated.
>
> Extension methods that configure driver properties do not re-initiate or commit the sessions, and expect such operations to be applied happen in proceeding code. If you want to force the driver properties are applied immediately, ensure that you subsequently call the high-level `Commit` or `Initiate` Extension method for the driver.

**Related concepts:**

- [Making Low-level Driver Calls](advanced/MakingLowLevelDriverCalls.md)
- [NI TSM: Parallel For Loops](https://www.ni.com/docs/en-US/bundle/teststand-semiconductor-module/page/parallel-for-loops.html)
- [NI DCPower: Programming Flow](https://www.ni.com/docs/en-US/bundle/ni-dcpower/page/programming-flow.html)
- [NI DCPower: Programming States](https://www.ni.com/docs/en-US/bundle/ni-dcpower/page/programming-states.html)
- [NI Digital: Session State Model](https://www.ni.com/docs/en-US/bundle/ni-digital-pattern/page/programming-states.html)

## Configuring DCPower Settings

The DCPower Extensions provide two main classes to configure DCPower settings: `DCPowerSourceSettings` & `DCPowerMeasureSettings`.

### Source Settings

To configure common settings relating to sourcing constant voltage or current in Single Point mode with NI DCPower instruments use the `DCPowerSourceSettings` class.

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

To configure common measurement settings when operating in either constant voltage or current mode with NI DCPower instruments use the `DCPowerMeasureSettings` class.

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

To configure common PPMU related setting with NI Digital Pattern instruments use the `PPMUSettings` class.

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
