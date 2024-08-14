# Configuring Instrument Sessions

The Semiconductor Test Library can help you efficiently configure instruments by consolidating the most commonly configured driver properties into a class. You can create and configure an instance of this class with only the properties you intend to update. The resulting object can then be passed as an input parameter to the appropriate extension method(s) in the library, which will operate on the object to update the targeted driver properties in one go. Other properties in the object not explicitly set will be ignored. This minimizes the number of parallel for loops, thus reducing overhead and the overall code execution time.

> [!NOTE]
> Extension methods that configure driver properties will always abort the driver session before updating a property.
>
> Extension methods that configure driver properties do not re-initiate or preform a commit of the driver session. Such operations are expected to be applied in proceeding code. If you want to force the driver properties to be applied immediately, ensure that you subsequently call the high-level `Commit` or `Initiate` Extension method.

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
