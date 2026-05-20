# SMU Gang Pin Group

The DCPower Instrument Abstraction allows you to gang SMU pins together to achieve higher current output than a single channel of any SMU can provide.

STL supports this functionality by programatically tying all the channels of a ganged pin group together, sharing equal current levels and limits across the channels and synchronizing them to act together.

> [!NOTE]
> Supported in Semiconductor Test Library 26.0 NuGet package or later.

## Hardware Requirements

### Supported Instruments

The following SMUs modules have been fully tested to validate they support STL's ganging feature:

- [PXIe-4137](https://www.ni.com/docs/en-US/bundle/pxie-4137/page/user-manual-welcome.html)
- [PXIe-4139](https://www.ni.com/docs/en-US/bundle/pxie-4139/page/user-manual-welcome.html)
- [PXIe-4150](https://www.ni.com/docs/en-US/bundle/pxie-4150/page/user-manual-welcome.html)
- [PXIe-4147](https://www.ni.com/docs/en-US/bundle/pxie-4147/page/user-manual-welcome.html)
- [PXIe-4162](https://www.ni.com/docs/en-US/bundle/pxie-4162/page/user-manual-welcome.html)
- [PXIe-4163](https://www.ni.com/docs/en-US/bundle/pxie-4163/page/user-manual-welcome.html)

> [!NOTE]
> - This is not a compressive list. Any channel from an SMU module that supports source and measure triggers can be part of a ganged pin group. However, sequence mode operations require all channels of the ganged pin group to also support start and sequence advance triggers.
> - Channels from different single or multi-channel SMUs can be ganged together. In such cases, the current shared by each individual channel cannot exceed the current rating of lowest rated SMU channel.
> - There is no restriction on the number of channels ganged.
> - Channels can be ganged in any order.
> - Basic voltage and current sequence operations can be preformed with ganged pin groups, but more advance synchronized sequence operations are not currently supported.

### Physical Connections

All the channels of ganged pin group must be physically connected on the application load board, either statically (always ganged together) or dynamically using a MUX or relays.
For remote sensing, sense wires of all the ganged channels must be connected.

The following image illustrates an example of the relay-based dynamic connections for a 2 channel gang:
![SMUGangPinGroupSetup](../images/SMUGangPinGroup/SMUGangPinGroupSetup.png)

## Theory of Operations

 There are two types of channels in each ganged pin group:

Leader channel: The lead channel responsible for driving source and measurement operations to which all other channels of the group are synchronized. Typically, associated with the first pin in the ganged pin group.
Follower channels: The channels synchronized with the leader channel for source and measurement operations.
The Current Level and Current Limit values set by the user are split equally across all channels.

STL sets the source and measure triggers for follower channels to synchronize them with source and measurement operations preformed by the leader channel.

When preforming voltage or current sequencing operations with a ganged pin group, the start trigger and sequence advance trigger are also set for follower channels.

> [!NOTE]
> Unlike [SMU Merge Pin Group feature](SMUMergePinGroup.md), where the complexity of operating ganged channels is handled by the driver, STL manages all of the necessary triggering, current level/limit splitting, and current measurement combining required to ensure a ganged pin group operates as a single synchronized unit per site.

## Pin Map Requirements

The ganged channels must all map to DUT pins in the pin map file on a per-site basis. The pins are then assigned to a dedicated pin group and the pin group only contains the pins mapped to the channels being ganged.

Use the following procedure to configure the pin map to use a ganged pin group:

1. Add DUT pin definitions for each of the channels being ganged. For example, "Vcc0", "Vcc1", "Vcc2" and so on.
2. Add a new pin group definition. Use a name that is appropriate for the combined pin. For example, "Vcc" or "Vcc_Ganged".
3. Assign each of the pins created in step 1 to the pin group created in step 2.

> [!NOTE]
> Unlike the pin map requirements for [SMU Merge Pin Group](SMUMergePinGroup.md#pin-map-requirements), the order of the pins in the pin group used to gang does not matter to the end user. However, note that the channel mapping to the first pin of the pin group is designated as the Leader channel when the gang operation is preformed.

The following example pin map file illustrates a pin group of two pins being ganged for two sites.

```<?xml version="1.0" encoding="utf-8"?>
<PinMap schemaVersion="1.6" xmlns="http://www.ni.com/TestStand/SemiconductorModule/PinMap.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <Instruments>
        <NIDCPowerInstrument name="SMU_4137_C1_S05" numberOfChannels="1">
            <ChannelGroup name="CommonDCPowerChannelGroup" />
        </NIDCPowerInstrument>
        <NIDCPowerInstrument name="SMU_4137_C1_S06" numberOfChannels="1">
            <ChannelGroup name="CommonDCPowerChannelGroup" />
        </NIDCPowerInstrument>
        <NIDCPowerInstrument name="SMU_4137_C1_S07" numberOfChannels="1">
            <ChannelGroup name="CommonDCPowerChannelGroup" />
        </NIDCPowerInstrument>
        <NIDCPowerInstrument name="SMU_4137_C1_S08" numberOfChannels="1">
            <ChannelGroup name="CommonDCPowerChannelGroup" />
        </NIDCPowerInstrument>
    </Instruments>
    <Pins>
        <DUTPin name="Vcc0" />
        <DUTPin name="Vcc1" />
    </Pins>
    <PinGroups>
        <PinGroup name="Vcc">
            <PinReference pin="Vcc0" />
            <PinReference pin="Vcc1" />
        </PinGroup>
    </PinGroups>
    <Sites>
        <Site siteNumber="0" />
        <Site siteNumber="1" />
    </Sites>
    <Connections>
        <Connection pin="Vcc0" siteNumber="0" instrument="SMU_4137_C1_S05" channel="0" />
        <Connection pin="Vcc1" siteNumber="0" instrument="SMU_4137_C1_S06" channel="0" />
        <Connection pin="Vcc0" siteNumber="1" instrument="SMU_4137_C1_S07" channel="0" />
        <Connection pin="Vcc1" siteNumber="1" instrument="SMU_4137_C1_S08" channel="0" />
    </Connections>
</PinMap>
```
> [!NOTE]
> The Ganged Pin Group feature is supported regardless of whether DCPower session groups are configured as a common session for all instrument, separate sessions for each instrument, or a separate session for each channel.

## Code Requirements

The gang operation must be performed within the test program at run-time, once instrument sessions are initialized.

> [!NOTE]
> This flexible design preserves access to individual channels for situations where channels are programmatically ganged with external relays or MUX only for certain tests that demand higher current. Allowing you to take advantage of the individual channels during other tests, or vice versa.

You can use the `GangPinGroup` method with a `DCPowerSessionsBundle` object that contains the pin group to perform the gang operation with the instrument.
Similarly, you can use the `UngangPinGroup` method to un-gang the channels in the pin group.
As a best practice, perform the gang operations at the start and end of the test program, unless performing a dynamic gang for specific tests.
Once the gang operation has been performed, all subsequent DCPower Extension methods can be used on the bundle, and will operate on the pin group as if it were one single pin in the bundle.

> [!NOTE]
> Once a pin group is ganged, low level driver operations must not be performed to configure the ganged channels, as that will override the configuration set by STL for ganging and may have adverse effects.

## Example Usage

The following C#/.NET code snippet shows how to use `GangPinGroup()` and `UngangPinGroup()` API calls to perform Ganging and Unganging operation on the `Vcc` PinGroup defined in the above pin map file.

``` C#
var sessionManager = new TSMSessionManager(tsmContext);
var smuBundle = sessionManager.DCPower("Vcc");

// Perform gang operation on the pin group.
smuBundle.GangPinGroup("Vcc");

// Source and/or measure the signals.
smuBundle.ForceCurrent(currentLevel, voltageLimit, waitForSourceCompletion: true);
smuBundle.MeasureAndPublishCurrent(publishedDataId: "GangedCurrent");

// Use the SMU Bundle object to perform ungang operation on the pin group.
smuBundle.UngangPinGroup("Vcc"); 
```

There is also a sequence style example available that showcases a complete working example of ganging SMU pin groups.
Refer to the [SMUGangPinGroup Example README](https://github.com/ni/semi-test-library-dotnet/blob/main/Examples/source/Sequence/SMUGangPinGroup/README.md) for more details.
This example is also installed on any system using STS Software 26.0 or later, under the following directory, `C:\Users\Public\Documents\National Instruments\NI_SemiconductorTestLibrary\Examples\Sequence\SMUGangPinGroup`.

## Measurement Data

When a ganged pin group is present within a `DCPowerSessionsBundle` object, the `MeasureCurrent` and `MeasureVoltage` methods will return a `PinSiteData` containing data associated with the pin group name. If there are non-ganged pins or pin groups contained and measured as part of the same bundle object, their measurement data will be associated with their respective individual pin names. Refer to the screenshot below as an example.

![PinSiteData](../images/SMUGangPinGroup/PinSiteData.png)

The measured current value of a ganged pin group will reflect the total combined current across all ganged channels that map to the pin group. Whereas, the measured voltage value will reflect a common voltage for all of the ganged channels mapped to the pin group.

> [!NOTE]
> If the `MeasureWhen` property is set to `AutomaticallyAfterSourceComplete`, only the first measurement taken will return valid data. To generate a subsequent measurements you must must re-initiate the output.
> The `MeasureWhen` and `MeasureTrigger` properties should not be configured for individual ganged pins. Doing so will result in an exception being thrown.
>
> ```cs
> var sessionManager = Initialize(pinmap);
> var dcPower = sessionManager.DCPower(new[] { "PowerPins" });
> var dcpowerMeasureSettings = new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete, ApertureTime = 0.001 };
> dcPower.GangPinGroup("PowerPins");
> dcPower.ConfigureMeasureSettings(dcpowerMeasureSettings);
> dcPower.Initiate();
> dcPower.MeasureVoltage();
> dcPower.MeasureVoltage() // Will throw fetch time out exception;
> ```

The `MeasureAndPublishCurrent` and `MeasureAndPublishVoltage`, and `PublishResults` methods will publish the measurement results using the name of the first pin in the ganged pin group, which is the pin associated with the leader channel. It is recommended that you specify the leader pin name in the Pin field of related tests in the Test tab of the calling TestStand step when working with ganged pin groups.

> [!NOTE]
> While the TestStand Semiconductor Module (TSM) allows values to be published by pin group name, it requires separate values for each of the pins within the pin group. When working with ganged pins, since the results are returned by pin group name and represent the combined value across all ganged pins, it typically does not make sense to publish the individual pin results. Instead, we only ever want to publish the combined result across the ganged pins.
> If users want to publish results for specific pin, they can do so by extracting the pin data from the `PinSiteData` object by the ganged pin group name using the `ExtractPin` method and then publish the resulting `SiteData` object.

> [!TIP]
> If you do not want to associate the published data with a pin, you can extract the data from the `PinSiteData` object by the ganged pin group name, using the `ExtractPin` method, and then only publish the returned `SiteData` object without associating it with any pin(s) by passing it to the `PublishResults` method.
>
> ```cs
> var results = dcPower.MeasureCurrent();
> tsmContext.PublishResults(results.ExtractPin("GangedPinGroupName"), publishedDataId: "Current");
> ```

The following images show a code module, which invokes the `MeasureAndPublishCurrent` method and is called from a step in a TestStand sequence, and how the Test tab of the calling step appears both at edit-time and at run-time. Note that at edit-time, the test item in the Tests tab of calling step has the Pin field configured with the leader pin name, and the Published Data ID field matches the value, "Current", used by the code module.

![MeasureAndPublishMethodCall](../images/SMUGangPinGroup/MeasureAndPublishMethodCall.png)
![TestsTabLeaderPinEdittime](../images/SMUGangPinGroup/TestsTabLeaderPinEdittime.png)
![TestsTabLeaderPinRuntime](../images/SMUGangPinGroup/TestsTabLeaderPinRuntime.png)
