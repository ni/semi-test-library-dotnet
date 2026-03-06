# Multiplexed Connection Sequence Example

This example demonstrates site-multiplexed measurement with the Semiconductor Test Library.

The pin map defines one instrument channel routed to one DUT pin across multiple sites. Each `MultiplexedDUTPinRoute.routeName` maps to a relay configuration name used during test execution.

## Prerequisites

1. If you want to use the example you must have the following software installed:
   - STS Software 25.5.0 or later
2. To run the example you must also have:
   - A DMM instrument alias `DMM_4081_C1_S02` and a Relay Module alias `RELAY_2567_C1_S03`, as defined in the pin map.
   - TestStand configured with Semiconductor Module support.

> **NOTE**
>
> You can review the example sequence file in the TestStand Sequence Editor and C# code source files in Visual Studio or any text editor without meeting requirement #2.
> To run the example, the required instruments must be physically installed or simulated using Offline Mode. `STLExample.MultiplexedConnection.offlinecfg` defines the required instruments for this example sequence file.

## Exploring the Example

Explore `MainSequence` and `Code Modules/TestSteps/MultiplexedDMMRead.cs` to see the site-multiplexed workflow.

### MainSequence

1. `MainSequence` step 1 (Semiconductor Action) calls `InitializeGenericMultiplexerSession` to register multiplexer sessions.
2. `MainSequence` step 2 (Semiconductor Action) calls `OneInstrumentChannelToManySitesForOneDutPin`.

### Code Module

1. `InitializeGenericMultiplexerSession(ISemiconductorModuleContext tsmContext, string multiplexerTypeId)` initializes switch sessions for the multiplexer defined by `multiplexerTypeId`.
2. `OneInstrumentChannelToManySitesForOneDutPin(ISemiconductorModuleContext tsmContext, string dutPinName, string endOfTestingRelayConfigurationName = "")` queries per-site routes for `dutPinName`, applies relay configurations, performs STL DMM reads, publishes per-site results, and applies `endOfTestingRelayConfigurationName`.

## Using the Example

Complete the following steps to run this example.

1. Open `STLExample.MultiplexedConnection.seq` in TestStand Sequence Editor.
2. The following are already configured in this example:
   - Setup flow initializes Relay Module and DMM sessions.
   - `MainSequence` step 1 calls `InitializeGenericMultiplexerSession` with `multiplexerTypeId` set to `NIGenericMultiplexer`.
   - `MainSequence` step 2 calls `OneInstrumentChannelToManySitesForOneDutPin` with `dutPinName` set to `A` and `endOfTestingRelayConfigurationName` set to `DisconnectDmmFromPinAOnAllSites`.
3. To run the test program, click the **Start/Resume Lot** button on the TSM toolbar.
