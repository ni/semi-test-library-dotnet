# Working With Multiplexed Connections

Semiconductor Test Library (STL) supports multiplexed connections to enable routing a shared instrument channel to the same DUT pin across multiple sites. This is achieved by defining a multiplexed connection in the pin map, specifying relay configurations for the routes, and then using a combination of the existing [Shared Pins](InstrumentAbstraction.md#shared-pins) functionality with the TestStand Semiconductor Module (TSM) Code Module API within a code module. Physical multiplexed connections must be made externally using a multiplexer or a relay network on the load board.

> [!IMPORTANT]
> STL does not make connections or control routes. These core responsibilities are retained by the TSM Code Module API. Before you perform measurements with STL, your code must initialize the multiplexer session and invoke TSM APIs to apply the appropriate routes or relay configurations.

> [!NOTE]
> Supported in Semiconductor Test Library 25.5 NuGet package or later.

## Pin Map Configuration

Configure the pin map with the multiplexed connection and relay configuration definitions. The route metadata is consumed by TSM APIs at runtime to determine which relay configuration to apply for each site.

### Configuring Multiplexed Connections

Use the Pin Map Editor to configure the following:

1. **Add the measurement instrument** (for example, a DMM or a DC Power instrument).
2. **Add a multiplexer instrument**.
3. **Set the Multiplexer Type field** to match the type identifier used in your code when it calls `GetSwitchNames` and `SetSwitchSession`.
4. **Define the DUT pins** used in the multiplexed connections.
5. **Define the System Relays** used for routing.
6. **Define the Relay Configurations** that specify which relays to open and close for each route.
7. **Add connections in the Connections table** to map each instrument channel to a DUT pin across sites. For each connection, specify the following:
   - **Pin**: The DUT pin that is connected through the multiplexer.
   - **Site**: The site number for this connection.
   - **Instrument** and **Channel**: The instrument channel that is routed.
   - **Multiplexer**: The multiplexer used for routing.
   - **Route**: The relay configuration or route name to apply for this pin-site combination.

> [!IMPORTANT]
> Only DUT Pins and SystemRelays are supported for multiplexed connection workflows. Defining SystemPins or SiteRelays is not supported for this workflow.

### Example Pin Map: NIGenericMultiplexer with Relay Configurations

The following example illustrates one shared DMM channel that is routed to the `VCC` pin across two sites using an `NIGenericMultiplexer`. This configuration uses relay configurations to control routing.

```xml
<Instruments>
  <NIDmmInstrument name="DMM_4081_C1_S02" />
  <NIRelayDriverModule name="RELAY_2567_C1_S03" numberOfControlLines="64" />
  <Multiplexer name="MUX1" multiplexerTypeId="NIGenericMultiplexer" />
</Instruments>

<Pins>
  <DUTPin name="VCC" />
</Pins>

<Sites>
  <Site siteNumber="0" />
  <Site siteNumber="1" />
</Sites>

<Relays>
  <SystemRelay name="DmmVCCSite0" />
  <SystemRelay name="DmmVCCSite1" />
</Relays>

<RelayConfigurations>
  <RelayConfiguration name="ConnectDmmToVCCSite0">
    <RelayPosition relay="DmmVCCSite0" position="Closed" />
    <RelayPosition relay="DmmVCCSite1" position="Open" />
  </RelayConfiguration>
  <RelayConfiguration name="ConnectDmmToVCCSite1">
    <RelayPosition relay="DmmVCCSite0" position="Open" />
    <RelayPosition relay="DmmVCCSite1" position="Closed" />
  </RelayConfiguration>
  <RelayConfiguration name="DisconnectDmmFromVCCOnAllSites">
    <RelayPosition relay="DmmVCCSite0" position="Open" />
    <RelayPosition relay="DmmVCCSite1" position="Open" />
  </RelayConfiguration>
</RelayConfigurations>

<Connections>
  <MultiplexedConnection instrument="DMM_4081_C1_S02" channel="0">
    <MultiplexedDUTPinRoute pin="VCC" siteNumber="0" multiplexer="MUX1" routeName="ConnectDmmToVCCSite0" />
    <MultiplexedDUTPinRoute pin="VCC" siteNumber="1" multiplexer="MUX1" routeName="ConnectDmmToVCCSite1" />
  </MultiplexedConnection>
  <SystemRelayConnection relay="DmmVCCSite0" relayDriverModule="RELAY_2567_C1_S03" controlLine="K0" />
  <SystemRelayConnection relay="DmmVCCSite1" relayDriverModule="RELAY_2567_C1_S03" controlLine="K1" />
</Connections>
```

### Example Pin Map: NIRelayMultiplexer with Multiple Channels

The following example illustrates multiple SMU channels that are routed to multiple DUT pins across two sites using an `NIRelayMultiplexer`. Each channel is mapped to a specific site.

```xml
<Instruments>
  <NIDCPowerInstrument name="SMU_4147_C1_S11" numberOfChannels="2">
    <ChannelGroup name="CommonDCPowerChannelGroup" />
  </NIDCPowerInstrument>
  <Multiplexer name="Relay_2567_C1_S07" multiplexerTypeId="NIRelayMultiplexer" />
</Instruments>

<Pins>
  <DUTPin name="VCC" />
  <DUTPin name="VDD" />
</Pins>

<Sites>
  <Site siteNumber="0" />
  <Site siteNumber="1" />
</Sites>

<Relays>
  <SystemRelay name="SMUVCCSite0" />
  <SystemRelay name="SMUVCCSite1" />
  <SystemRelay name="SMUVDDSite0" />
  <SystemRelay name="SMUVDDSite1" />
</Relays>

<RelayConfigurations>
  <RelayConfiguration name="ConnectSMUToVCCSite0">
    <RelayPosition relay="SMUVCCSite0" position="Closed" />
    <RelayPosition relay="SMUVCCSite1" position="Open" />
  </RelayConfiguration>
  <RelayConfiguration name="ConnectSMUToVCCSite1">
    <RelayPosition relay="SMUVCCSite0" position="Open" />
    <RelayPosition relay="SMUVCCSite1" position="Closed" />
  </RelayConfiguration>
  <RelayConfiguration name="ConnectSMUToVDDSite0">
    <RelayPosition relay="SMUVDDSite0" position="Closed" />
    <RelayPosition relay="SMUVDDSite1" position="Open" />
  </RelayConfiguration>
  <RelayConfiguration name="ConnectSMUToVDDSite1">
    <RelayPosition relay="SMUVDDSite0" position="Open" />
    <RelayPosition relay="SMUVDDSite1" position="Closed" />
  </RelayConfiguration>
</RelayConfigurations>

<Connections>
  <MultiplexedConnection instrument="SMU_4147_C1_S11" channel="0">
    <MultiplexedDUTPinRoute pin="VCC" siteNumber="0" multiplexer="Relay_2567_C1_S07" routeName="ConnectSMUToVCCSite0" />
    <MultiplexedDUTPinRoute pin="VDD" siteNumber="0" multiplexer="Relay_2567_C1_S07" routeName="ConnectSMUToVDDSite0" />
  </MultiplexedConnection>
  <MultiplexedConnection instrument="SMU_4147_C1_S11" channel="1">
    <MultiplexedDUTPinRoute pin="VCC" siteNumber="1" multiplexer="Relay_2567_C1_S07" routeName="ConnectSMUToVCCSite1" />
    <MultiplexedDUTPinRoute pin="VDD" siteNumber="1" multiplexer="Relay_2567_C1_S07" routeName="ConnectSMUToVDDSite1" />
  </MultiplexedConnection>
</Connections>
```

**Related Information**:

- [TestStand Semiconductor Module User Manual - Specifying Multiplexers and Multiplexed Connections in a Pin Map (TSM)](https://www.ni.com/docs/en-US/bundle/teststand-semiconductor-module/page/specifying-multiplex-in-pinmap.html)

## Implementation Workflow

Perform multiplexed operations at runtime after the required instrument sessions are initialized. The workflow follows a serial site-by-site pattern, where you perform the following actions:

- Apply the relay configuration to route to one site.
- Perform the measurement.
- Move to the next site.

### Setup Requirements

Before using multiplexed connections in your test code:

1. **Initialize the Relay Driver Module session** with the existing STL `SetupAndCleanupSteps.SetupNIRelayDriverModuleSessions` method.
2. **Initialize the measurement instrument sessions** (for example, DMM or DCPower) with the existing STL `SetupAndCleanupSteps` methods.
3. **Initialize the multiplexer session** with the TSM context, using the `GetSwitchNames` and `SetSwitchSession` methods. You must implement code to perform this action. Typically, your code for this will be invoked within ProcessSetup of the test program's sequence file.

> [!NOTE]
> Steps 1 and 2 are likely already covered within the ProcessSetup sequence of the test program's sequence file if that test program was created by the STS Project Creation Tool using the NI Default - C#/.NET template or a derivation thereof.

### Test Code Workflow

1. **Query per-site route names** for the target DUT pin with the `GetSwitchSessions` method.
2. **Do the following for each site**:
   1. Apply the relay configuration of the current site with the `ApplyRelayConfiguration` method.
   2. Create a `TSMSessionManager` object with the site-specific context.
   3. Query the appropriate STL session bundle and perform measurement operations.
3. **If needed, apply an end-of-test relay configuration** to restore relay states.

## Example Usage

The following code demonstrates the typical TSM API + STL workflow for a multiplexed DMM measurement. These examples are based on the [Multiplexed Connection Example](https://github.com/ni/semi-test-library-dotnet/blob/main/Examples/source/Sequence/MultiplexedConnection/).

### Multiplexer Session Initialization (User Code in ProcessSetup)

Your code must initialize the multiplexer session with the TSM context. The initialization is called from ProcessSetup after initializing the Relay Driver Module and measurement instrument sessions with STL TestStandSteps:

> [!NOTE]
> This workflow uses the TSM Relay Control API (`ApplyRelayConfiguration`) to control routes. The initialization of the multiplexer session registers a placeholder session object. This placeholder session object does not get used directly but allows TSM to map the multiplexed connections. This is required as TSM's API expects the user to provide an NI-Switch driver session object that can then later be used with low-level driver calls to connect each route, but that use case is not applicable to STL. Furthermore, if a valid object is not passed to the TSM API, it will throw an exception.

```csharp
public static void InitializeGenericMultiplexerSession(
    ISemiconductorModuleContext tsmContext,
    string multiplexerTypeId)
{
  try
  {
    // Query switch names for the configured multiplexer type.
    var switchNames = tsmContext.GetSwitchNames(multiplexerTypeId);

    foreach (var switchName in switchNames)
    {
        // For TSM, you must set a session object for the associated multiplexer.
        // This allows TSM to later retrieve the multiplexer routes in the test program with the TSM GetSwitchSessions method.
        // TSM expects that an NI-Switch driver session is used to operate the multiplexer routes during the program.
        // However, this example does not use the NI-Switch driver directly.
        // Instead, this example uses the TSM Control Relay methods to operate the multiplexer routes that the TSM GetSwitchSessions method retrieves.
        // Therefore, you must provide a dummy object to the TSM SetSwitchSession method to enable this use case.
        tsmContext.SetSwitchSession(multiplexerTypeId, switchName, new object());
    }
  }
  catch (Exception e)
  {
      NISemiconductorTestException.Throw(e);
  }
}
```

### Multiplexed Measurement (Test Method)

Perform site-by-site measurements after applying the appropriate relay configuration for each site with the following example:

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

    // Execute one site at a time because this pin shares the same instrument channel for all sites.
    for (int i = 0; i < tsmContexts.Length; i++)
    {
        // Apply the relay configuration of the current site.
        var currentContext = tsmContexts[i];
        currentContext.ApplyRelayConfiguration(routes[i]);
        var sessionManager = new TSMSessionManager(currentContext);
        var dmm = sessionManager.DMM(dutPinName);

        // Read the measurement and publish it for the active site context.
        var measurement = dmm.Read(maximumTimeInMilliseconds: 1000);
        currentContext.PublishResults(measurement, $"{dutPinName}_Voltage");
    }

    // Restore the configured end-of-test relay state after the site loop completes.
    if (!string.IsNullOrEmpty(endOfTestingRelayConfigurationName))
    {
        tsmContext.ApplyRelayConfiguration(endOfTestingRelayConfigurationName);
    }
}
```

### Multiplexer Session Cleanup (User Code in ProcessCleanup)

Your code must first clean up all switch sessions during ProcessCleanup. Then, your code must also clean up the relay driver module (if applicable) and the measurement instrument sessions in the following way:

```csharp
public static void CleanupGenericMultiplexerSession(
    ISemiconductorModuleContext tsmContext,
    string multiplexerTypeId)
{
    try
    {
      var sessions = tsmContext.GetAllSwitchSessions(multiplexerTypeId);

      // Note: Since the multiplexer session was initialized with a placeholder object,
      // the cast to NISwitch will return null and Close() will not be called.
      // This cleanup code is included for completeness, but is not strictly necessary
      // when using placeholder objects.
      foreach (var session in sessions)
      {
          var switchSession = session as NISwitch;
          switchSession?.Close();
      }
    }
    catch (Exception e)
    {
        NISemiconductorTestException.Throw(e);
    }
}
```

## Examples

For the complete sequence-style example that demonstrates this workflow end-to-end, refer to the [Multiplexed Connection Example README](https://github.com/ni/semi-test-library-dotnet/blob/main/Examples/source/Sequence/MultiplexedConnection/README.md).

This example is also installed on all systems that use STS Software 26.0 or later under the following directory:
`C:\Users\Public\Documents\National Instruments\NI_SemiconductorTestLibrary\Examples\Sequence\MultiplexedConnection`.