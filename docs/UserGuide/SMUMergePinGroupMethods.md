# Merge Pin Group Feature

The Merge Pin Group Feature introduced in STL 25.5 version of STS. For existing STL customers starting from Version 24.5 can preview this feature by donwloading the software code from Github.

## Requirements

To make use of this feature in STL there are few pre-requisites that needs to be filled.

- Hardware connection requirement
- PinMap grouping requirement
- MergePinGroup and UnmergePinGroup API calls

### Hardware connection requirement

- Merging is a low level driver feature supported by Multichannel SMU hardware. 
- Supported Multichannel SMUs are PXIe-4147, PXIe-4162 and PXIe-4163.
- The [PXIe-4147](https://www.ni.com/docs/en-US/bundle/pxie-4147/page/merged-channels.html) and [PXIe-4162](https://www.ni.com/docs/en-US/bundle/pxie-4162/page/merged-channels.html) supports 2 channel and 4 channel merging.
- [PXIe-4163](https://www.ni.com/docs/en-US/bundle/pxie-4163/page/merged-channels.html
) supports 2, 4 and 8 channel merging.
- For remote sensing only the primary channel's sense wire needs to be connected.
- The load board can have relays that can be used dynammic merging or it can be connected permentely on the load board

The below diagram shows the relay based dyncmic connections for 4 channel merging.
![image](SMUMergePinGroupConnection.png)

### PinMap Grouping requirement

- In the pinmap file the smu channels that are to be merged needs to be mapped to certain pins and a pingroup needs to be defined using those pins only.
- The first pin the pingroup should be mapped to the primary channel of the merge group.
- For Multisite ensure that all sites are mapped to mergable channels and first pin in each site is mapped to primary of channel group.
- The below pinmap file shows the pingroup of 4 pins that can be merged.

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

### MergePinGroup and UnmergePinGroup API calls

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
