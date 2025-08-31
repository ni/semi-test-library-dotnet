# Merge Pin Group Feature

The DCPower Instrument Abstraction allows you to merge SMU pins together to achieve higher current. This is an advanced, yet common use case for many mixed-signal STS applications. This is particularly useful for applications that require a higher current output beyond what any single independent channel of the SMU can provide.

> [!NOTE]
> Only supported with v25.5 or later of the Semiconductor Test Library NuGet package.

This feature takes advantage of a NI-DCPower driver capability known as Merged Channels, which allows the user to combine multiple channels on a single multi-channel SMU to work in unison. Merging channels requires you to designate a primary channel and programmatically combine it with other compatible merge channels.

- Primary channel: The channel you access when programming merged channels in the  low-level driver session.
- Merge channels: The channels that you specify with the Merged Channels property. The merge channels work in unison with the primary channel.

## Hardware Requirements

### Supported Instruments

The Merged Pin Groups feature is supported when using the following multi-channel SMUs:

- [PXIe-4147](https://www.ni.com/docs/en-US/bundle/pxie-4147/page/merged-channels.html), supports 2 channel and 4 channel merging.
- [PXIe-4162](https://www.ni.com/docs/en-US/bundle/pxie-4162/page/merged-channels.html), supports 2 channel and 4 channel merging.
- [PXIe-4163](https://www.ni.com/docs/en-US/bundle/pxie-4163/page/merged-channels.html), supports 2 channel, 4 channel, and 8 channel merging.

### Physical Connections

The designated primary and merge channels must be physically connected on the application load board, either statically (always merged together) or dynamically using a MUX or relays. 
For remote sensing, only the primary channel sense wire must be connected.

The following image illustrates an example of the relay-based dynamic connections for a 4 channel merge:
![image](SMUMergePinGroupConnection.png)

> [!NOTE]
> Only certain channels on a device can be used as primary channels. Refer to the device specific user manuals linked above for more details, including Choosing a Valid Merge Configuration, Designing Merge Circuitry, and Effect of Merging Channels on Performance Specifications.

## Pin Map Requirements

The designated primary and merge channels must all map to DUT pins in the pin map file, on a per site bases. Channels mapped to a particular site must all be from the same instrument, and that instrument must support Merged Channels (refer to [Hardware Requirements](#hardware-requirements)). It is possible to map multiple sites to the same instrument, depending on the merge configuration (x2, x4, etc.). The pins are then assigned to a dedicated pin group, where the pin mapping to the primary SMU channel is the first element in the pin group and the pin group only contains the pins mapped to the channels being merged.

Use the following procedure to configure the pin map to use a Merged Pin Group:

1. Add DUT pin definitions for each of the channels being merged. For example, "Vcc_0", "Vcc_1", "Vcc_2", etc.
   - Ensure that the channels mapped to any particular are all from the same instrument, and the instrument supports Merged Channels (refer to [Hardware Requirements](#hardware-requirements))
2. Add a new pin group definition. Use a name that is appropriate for the combined pin. For example, "Vcc" or "Vcc_Merged".
3. Assign each of the pins created in step 1 to the pin group created in step 2.
   - Ensure that the first pin the pin group is mapped to the primary channel of the merged channel group and the pin group only contains the pins mapped to the channels being merged.

The following example pin map file illustrates a pin group of two pins being merged for two sites.

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
        <Connection pin="Vcc0" siteNumber="0" instrument="SMU_4147_C1_S05" channel="0" />
        <Connection pin="Vcc1" siteNumber="0" instrument="SMU_4147_C1_S05" channel="1" />
        <Connection pin="Vcc0" siteNumber="1" instrument="SMU_4147_C1_S05" channel="2" />
        <Connection pin="Vcc1" siteNumber="1" instrument="SMU_4147_C1_S05" channel="3" />
    </Connections>
</PinMap>
```

## Code Requirements

The merge operation must be performed within the test program at runtime, once instrument sessions are initialized.

> [!NOTE]
> This by design to allow you the flexibility to handle situations where channels may not be statically merged on application load board, but rather, are be programmatically merged via external relay or MUX during testing for only certain tests that demand higher current. Allowing you to take advantage of the individual channels during other tests, or vice versa.

You can use the `MergePinGroup` method with a `DCPowerSessionsBundle` object that contains the pin group to perform the merge operation with the instrument.
Similarly, you can use the `MergePinGroup` method to un-merge the channels in the in group.
It is recommended to perform the merge operations at the start and end of the test program, unless performing a dynamic merge for specific tests.
Once the merge operation has been performed, all subsequent DCPower Extension methods can be used on the bundle, and will operate on the pin group as if it were one single pin in the bundle.

> [!NOTE]
> The `DCPowerSessionsBundle` must be created using the Pin Group by name. Do not create the `DCPowerSessionsBundle` using the individual pin names.

## Example Usage

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

There is also sequence style example available that showcases a complete working example of this feature.
Refer to the [SMUMergePinGroup Example README](https://github.com/ni/semi-test-library-dotnet/blob/main/Examples/source/Sequence/SMUMergePinGroup/README.md) for more details.
This example is also installed on any system using STS Software 25.5 or later, under the following directory, `C:\Users\Public\Documents\National Instruments\NI_SemiconductorTestLibrary\Examples\Sequence\SMUMergePinGroup`.
