# Multiplexed Connection Support

The Semiconductor Test Library (STL) supports multiplexed connection workflows, where a shared instrument channel is routed to the same DUT pin across multiple sites through a multiplexer or relay network on the load board. This workflow is supported in Semiconductor Test Library (STL) because of the existing [Shared Pins](InstrumentAbstraction.md#shared-pins) functionality.

> [!IMPORTANT]
> Semiconductor Test Library (STL) is not responsible for making connections or controlling routes. Your code is responsible for initializing the multiplexer session and invoking TSM APIs to apply the appropriate routes or relay configurations before performing measurements with Semiconductor Test Library (STL).

> [!NOTE]
> Supported in Semiconductor Test Library 25.5 NuGet package or later.

## Pin Map Configuration

Configure the pin map with the multiplexed connection and relay configuration definitions. The route metadata defined in `MultiplexedDUTPinRoute` elements is consumed by TSM APIs at runtime to determine which relay configuration to apply for each site.

### Pin Map Structure

1. Define the measurement instrument (for example, `NIDmmInstrument` or `NIDCPowerInstrument`).
2. Define a `Multiplexer` with a `multiplexerTypeId` that matches the type identifier used in your code when calling `GetSwitchNames` and `SetSwitchSession`.
3. Define `MultiplexedConnection` elements that map instrument channels to DUT pins.
4. Define `MultiplexedDUTPinRoute` elements within each `MultiplexedConnection` to specify the site, multiplexer, and route name for each pin-site combination.

Depending on the multiplexer type, you may also need to define:
- `NIRelayDriverModule` for controlling relay-based routes
- `SystemRelay` entries for each relay used for site routing
- `RelayConfiguration` entries that specify relay states for each route
- `SystemRelayConnection` entries to map relays to relay driver module control lines

> [!NOTE]
> The `multiplexerTypeId` in the pin map must match the type identifier used by your code when calling `GetSwitchNames` and `SetSwitchSession`.

### Example Pin Map: NIGenericMultiplexer with Relay Configurations

The following example illustrates one shared DMM channel routed to pin `A` across four sites using an `NIGenericMultiplexer`. This configuration uses relay configurations to control routing:

```xml
<Instruments>
  <NIDmmInstrument name="DMM_4081_C1_S02" />
  <NIRelayDriverModule name="RELAY_2567_C1_S03" numberOfControlLines="64" />
  <Multiplexer name="MUX1" multiplexerTypeId="NIGenericMultiplexer" />
</Instruments>

<Pins>
  <DUTPin name="A" />
</Pins>

<Relays>
  <SystemRelay name="DmmPinASite0" />
  <SystemRelay name="DmmPinASite1" />
  <SystemRelay name="DmmPinASite2" />
  <SystemRelay name="DmmPinASite3" />
</Relays>

<RelayConfigurations>
  <RelayConfiguration name="ConnectDmmToPinASite0">
    <RelayPosition relay="DmmPinASite0" position="Closed" />
    <RelayPosition relay="DmmPinASite1" position="Open" />
    <RelayPosition relay="DmmPinASite2" position="Open" />
    <RelayPosition relay="DmmPinASite3" position="Open" />
  </RelayConfiguration>
  <!-- Similar configurations for Site1, Site2, Site3 -->
  <RelayConfiguration name="DisconnectDmmFromPinAOnAllSites">
    <RelayPosition relay="DmmPinASite0" position="Open" />
    <RelayPosition relay="DmmPinASite1" position="Open" />
    <RelayPosition relay="DmmPinASite2" position="Open" />
    <RelayPosition relay="DmmPinASite3" position="Open" />
  </RelayConfiguration>
</RelayConfigurations>

<Connections>
  <MultiplexedConnection instrument="DMM_4081_C1_S02" channel="0">
    <MultiplexedDUTPinRoute pin="A" siteNumber="0" multiplexer="MUX1" routeName="ConnectDmmToPinASite0" />
    <MultiplexedDUTPinRoute pin="A" siteNumber="1" multiplexer="MUX1" routeName="ConnectDmmToPinASite1" />
    <MultiplexedDUTPinRoute pin="A" siteNumber="2" multiplexer="MUX1" routeName="ConnectDmmToPinASite2" />
    <MultiplexedDUTPinRoute pin="A" siteNumber="3" multiplexer="MUX1" routeName="ConnectDmmToPinASite3" />
  </MultiplexedConnection>
  <SystemRelayConnection relay="DmmPinASite0" relayDriverModule="RELAY_2567_C1_S03" controlLine="K0" />
  <SystemRelayConnection relay="DmmPinASite1" relayDriverModule="RELAY_2567_C1_S03" controlLine="K1" />
  <SystemRelayConnection relay="DmmPinASite2" relayDriverModule="RELAY_2567_C1_S03" controlLine="K2" />
  <SystemRelayConnection relay="DmmPinASite3" relayDriverModule="RELAY_2567_C1_S03" controlLine="K3" />
</Connections>
```

### Example Pin Map: NIRelayMultiplexer with Multiple Channels

The following example illustrates multiple SMU channels routed to multiple DUT pins across four sites using an `NIRelayMultiplexer`. Each channel is mapped to a specific site:

```xml
<Instruments>
  <NIDCPowerInstrument name="SMU_4147_C1_S11" numberOfChannels="4">
    <ChannelGroup name="CommonDCPowerChannelGroup" />
  </NIDCPowerInstrument>
  <Multiplexer name="Relay_2567_C1_S07" multiplexerTypeId="NIRelayMultiplexer" />
</Instruments>

<Pins>
  <DUTPin name="DUTPin1" />
  <DUTPin name="DUTPin2" />
</Pins>

<Sites>
  <Site siteNumber="0" />
  <Site siteNumber="1" />
  <Site siteNumber="2" />
  <Site siteNumber="3" />
</Sites>

<Connections>
  <MultiplexedConnection instrument="SMU_4147_C1_S11" channel="0">
    <MultiplexedDUTPinRoute pin="DUTPin1" siteNumber="0" multiplexer="Relay_2567_C1_S07" routeName="Site0_DUTPin1" />
    <MultiplexedDUTPinRoute pin="DUTPin2" siteNumber="0" multiplexer="Relay_2567_C1_S07" routeName="Site0_DUTPin2" />
  </MultiplexedConnection>
  <MultiplexedConnection instrument="SMU_4147_C1_S11" channel="1">
    <MultiplexedDUTPinRoute pin="DUTPin1" siteNumber="1" multiplexer="Relay_2567_C1_S07" routeName="Site1_DUTPin1" />
    <MultiplexedDUTPinRoute pin="DUTPin2" siteNumber="1" multiplexer="Relay_2567_C1_S07" routeName="Site1_DUTPin2" />
  </MultiplexedConnection>
  <!-- Similar connections for channels 2 and 3 mapping to sites 2 and 3 -->
</Connections>
```

**Related information**:

- [TestStand Semiconductor Module User Manual - Multiplexed Connections](https://www.ni.com/docs/en-US/bundle/teststand-semiconductor-module/page/specifying-multiplex-in-pinmap.html)

## Implementation Workflow

Perform multiplexed operations at runtime after required instrument sessions are initialized. The workflow follows a serial site-by-site pattern where you apply the relay configuration to route to one site, perform the measurement, then move to the next site.

### Setup Requirements

Before using multiplexed connections in your test code:

1. **Initialize the Relay Driver Module session** (if applicable) using the existing Semiconductor Test Library (STL) `SetupAndCleanupSteps.SetupNIRelayDriverModuleSessions` method (typically in ProcessSetup).
2. **Initialize the measurement instrument sessions** (for example, DMM or DCPower) using the existing Semiconductor Test Library (STL) `SetupAndCleanupSteps` methods.
3. **Initialize the multiplexer session** with the TSM context using `GetSwitchNames` and `SetSwitchSession`. This must be done by your code, typically in ProcessSetup.

### Test Code Workflow

1. **Query per-site route names** for the target DUT pin using `GetSwitchSessions`.
2. **For each site**:
   - Apply the relay configuration for the current site using `ApplyRelayConfiguration`.
   - Create a `TSMSessionManager` with the site-specific context.
   - Query the Semiconductor Test Library (STL) session bundle and perform measurement operations.
3. **Apply an end-of-test relay configuration** to restore relay states if needed.

## Example Usage

The following code demonstrates the typical TSM API + Semiconductor Test Library (STL) workflow for a multiplexed DMM measurement. These examples are based on the [Multiplexed Connection Example](https://github.com/ni/semi-test-library-dotnet/blob/main/Examples/source/Sequence/MultiplexedConnection/).

> [!NOTE]
> This workflow uses the TSM Relay Control API (`ApplyRelayConfiguration`) to control routes. The multiplexer session initialization registers a placeholder session so TSM can map switch context without direct NI Switch driver calls.

### Multiplexer Session Initialization (User Code in ProcessSetup)

Your code must initialize the multiplexer session with the TSM context. This is called from ProcessSetup after initializing the Relay Driver Module (if applicable) and measurement instrument sessions using Semiconductor Test Library (STL) TestStandSteps:

```csharp
public static void InitializeGenericMultiplexerSession(
    ISemiconductorModuleContext tsmContext,
    string multiplexerTypeId)
{
    // Query switch names for the configured multiplexer type.
    var switchNames = tsmContext.GetSwitchNames(multiplexerTypeId);

    foreach (var switchName in switchNames)
    {
        // Register a placeholder session so TSM can map switch context
        // without direct NI Switch driver calls.
        tsmContext.SetSwitchSession(multiplexerTypeId, switchName, new object());
    }
}
```

### Multiplexed Measurement (Test Method)

Perform site-by-site measurements after applying the appropriate relay configuration for each site:

```csharp
public static void OneInstrumentChannelToManySitesForOneDutPin(
    ISemiconductorModuleContext tsmContext,
    string dutPinName,
    string endOfTestingRelayConfigurationName = "",
    string multiplexerTypeId = "NIGenericMultiplexer")
{
    // Retrieve site-specific route names for the requested DUT pin.
    // Route names are defined in the pin map and map to relay configurations.
    tsmContext.GetSwitchSessions(
        dutPinName,
        multiplexerTypeId,
        out ISemiconductorModuleContext[] tsmContexts,
        out string[] routes);

    // Execute one site at a time because this flow shares a single instrument channel.
    for (int i = 0; i < tsmContexts.Length; i++)
    {
        // Select the relay state for the current site using the main context.
        tsmContext.ApplyRelayConfiguration(routes[i]);
        var sessionManager = new TSMSessionManager(tsmContexts[i]);
        var dmm = sessionManager.DMM(dutPinName);

        // Read measurement and publish it for the active site context.
        var measurement = dmm.Read(maximumTimeInMilliseconds: 1000);
        tsmContexts[i].PublishResults(measurement, $"{dutPinName}_Voltage");
    }

    // Restore the configured end-of-test relay state after site loop completes.
    if (!string.IsNullOrEmpty(endOfTestingRelayConfigurationName))
    {
        tsmContext.ApplyRelayConfiguration(endOfTestingRelayConfigurationName);
    }
}
```

### Multiplexer Session Cleanup (User Code in ProcessCleanup)

Your code must clean up any switch sessions during ProcessCleanup, before cleaning up the relay driver module (if applicable) and measurement instrument sessions:

```csharp
public static void CleanupGenericMultiplexerSession(
    ISemiconductorModuleContext tsmContext,
    string multiplexerTypeId)
{
    // Retrieve and close any switch sessions.
    // For NIGenericMultiplexer with placeholder objects, this is a no-op.
    // For multiplexer types that use actual NI Switch sessions, this closes them.
    var sessions = tsmContext.GetAllSwitchSessions(multiplexerTypeId);

    foreach (var session in sessions)
    {
        var switchSession = session as NISwitch;
        switchSession?.Close();
    }
}
```

## Examples

There is a sequence-style example available that demonstrates this workflow end to end.
Refer to the [Multiplexed Connection Example README](https://github.com/ni/semi-test-library-dotnet/blob/main/Examples/source/Sequence/MultiplexedConnection/README.md) for details.

This example is also installed on any system using STS Software 26.0 or later under the following directory:
`C:\Users\Public\Documents\National Instruments\NI_SemiconductorTestLibrary\Examples\Sequence\MultiplexedConnection`.