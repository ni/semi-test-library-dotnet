# Merge Pin Group Feature

Introduced in Semiconductor Test Library 25.5. Customers with version 24.5 or later can preview this feature by donwloading the software code from Github.

## Requirements

This feature requires specific hardware connections, pinmap groupings, and MergePinGroup and UnmergePinGroup API calls, which are described in the following sections.

### Hardware Connection

Merging is a low-level driver feature supported by multichannel SMU hardware. Merged pin groups are supported for the following multichannel SMUs:
- [PXIe-4147](https://www.ni.com/docs/en-US/bundle/pxie-4147/page/merged-channels.html), supports 2 channel and 4 channel merging.
- [PXIe-4162](https://www.ni.com/docs/en-US/bundle/pxie-4162/page/merged-channels.html), supports 2 channel and 4 channel merging.
- [PXIe-4163](https://www.ni.com/docs/en-US/bundle/pxie-4163/page/merged-channels.html), supports 2 channel, 4 channel, and 8 channel merging.

For remote sensing, only the primary channel sense wire must be connected.

The load board may use permanent connections or relays for dynammic merging. The following image illustrates the relay-based dynamic connections for 4 channel merging.
![image](SMUMergePinGroupConnection.png)

### PinMap Grouping

Verify the pinmap has the required pingroup:
- The SMU channels to be merged must be mapped to specific pins.
- A pingroup must be defined using only the pins mapped for merged channels.
- The first pin the pingroup should be mapped to the primary channel of the merged channel group.

For multi-site scenarios: 
 - Ensure that all sites are mapped to mergable channels.
 - Ensure the first pin in each site is mapped to the primary channel of the channel group.

The following pinmap file illustrates a pingroup of 4 pins that can be merged.

```<?xml version="1.0" encoding="utf-8"?>
<PinMap schemaVersion="1.6" xmlns="http://www.ni.com/TestStand/SemiconductorModule/PinMap.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <Instruments>
        <NIDCPowerInstrument name="SMU_4147_C1_S05" numberOfChannels="4">
            <ChannelGroup name="CommonDCPowerChannelGroup" />
        </NIDCPowerInstrument>
    </Instruments>
    <Pins>
        <DUTPin name="Vcc0" />
        <DUTPin name="Vcc1" />
        <DUTPin name="Vcc2" />
        <DUTPin name="Vcc3" />
    </Pins>
    <PinGroups>
        <PinGroup name="Vcc">
            <PinReference pin="Vcc0" />
            <PinReference pin="Vcc1" />
            <PinReference pin="Vcc2" />
            <PinReference pin="Vcc3" />
        </PinGroup>
    </PinGroups>
    <Sites>
        <Site siteNumber="0" />
    </Sites>
    <Connections>
        <Connection pin="Vcc0" siteNumber="0" instrument="SMU_4147_C1_S05" channel="0" />
        <Connection pin="Vcc1" siteNumber="0" instrument="SMU_4147_C1_S05" channel="1" />
        <Connection pin="Vcc2" siteNumber="0" instrument="SMU_4147_C1_S05" channel="2" />
        <Connection pin="Vcc3" siteNumber="0" instrument="SMU_4147_C1_S05" channel="3" />
    </Connections>
</PinMap>
```

### MergePinGroup and UnmergePinGroup API Calls

The following .Net/C# code snippet shows how to use `MergePinGroup()` and `UnmergePinGroup()` API calls to perform Merging and Unmerging operation on the `Vcc` PinGroup defined in the above pinmap file.

``` C#
var sessionManager = new TSMSessionManager(tsmContext);
var smuBundle = sessionManager.DCPower("Vcc");

// Perform merge operation on the pin group.
smuBundle.MergePinGroup("Vcc");

// Source and/or measure the signals.
smuBundle.ForceCurrent(currentLevel, voltageLimit, waitForSourceCompletion: true);
smuBundle.MeasureAndPublishCurrent(publishedDataId: "MergedCurrent");

// Use the SMU Bundle object to perform unmerge operation on the pin group.
smuBundle.UnmergePinGroup("Vcc");
```

Please refer to the STL [shipping example](file:///C:\Users\Public\Documents\National Instruments\NI_SemiconductorTestLibrary\Examples\Sequence\SMUMergePinGroup) or in the [Github Repo](https://github.com/ni/semi-test-library-dotnet/tree/main/Examples/source/Sequence/SMUMergePinGroup) for the complete working example of this feature.
