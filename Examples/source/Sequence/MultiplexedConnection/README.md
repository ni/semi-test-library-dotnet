# Multiplexed Connection Sequence Example

This example demonstrates how to use site-multiplexed measurements with the Semiconductor Test Library (STL).

The pin map defines one instrument channel routed to one DUT pin across multiple sites. Each `MultiplexedDUTPinRoute.routeName` maps to a relay configuration name used during test execution.
This sequence demonstrates routing the same DUT pin (`A`) across four sites, one site at a time. Then, the sequence performs per-site measurements through TSM-managed sessions.

## Key Files

The example contains the following key files:

- `STLExample.MultiplexedConnection.seq`: Example TestStand sequence file.
- `STLExample.MultiplexedConnection.pinmap`: Pin map used by the sequence.
- `STLExample.MultiplexedConnection.offlinecfg`: Offline configuration file for instrument simulation in Offline Mode.
- `Code Modules/SetupAndCleanupSteps/SetupAndCleanupSteps.cs`: Setup and cleanup step methods.
- `Code Modules/TestSteps/TestStep.cs`: Operational test step method.
- `Code Modules/STLExample.MultiplexedConnection.csproj`: Example code module project file.

## Prerequisites

1. To use this example, your system must also meet the following requirements:
   - STS Software 24.5.0 or later
   - Semiconductor Test Library (STL) 25.5.0 or later
2. To run the example you must also have:
   - One NI-DMM PXIe-4081 instrument with the `DMM_4081_C1_S02` alias.
   - One NI-SWITCH PXIe-2567 module with the `RELAY_2567_C1_S03` alias.

### Offline Mode

Complete the following steps to simulate the instruments in Offline Mode:

1. Open `STLExample.MultiplexedConnection.seq` in the TestStand Sequence Editor.
2. Enable Offline Mode from the TSM toolbar.
3. Load `STLExample.MultiplexedConnection.offlinecfg`.
4. Verify that the simulated instrument aliases match the instrument aliases defined in `STLExample.MultiplexedConnection.pinmap`.

> **NOTE**
>
> If you cannot meet the second set of requirements, you can review the example sequence file in the TestStand Sequence Editor. You can also review the C# code source files in Visual Studio or any text editor.
> To run the example, the required instruments must be physically installed or simulated using Offline Mode. `STLExample.MultiplexedConnection.offlinecfg` defines the required instruments for this example sequence file.
> Ensure that the instrument aliases configured in NI MAX (or the Hardware Configuration Tool) match the aliases defined in `STLExample.MultiplexedConnection.pinmap`.

## Exploring the Example

Explore `MainSequence` and the files under `Code Modules/TestSteps` to see the site-multiplexed workflow.

### MainSequence

1. `MainSequence` calls `OneInstrumentChannelToManySitesForOneDutPin` to perform DMM reads for each site and publish per-site results.

### Code Modules

Open `Code Modules/STLExample.MultiplexedConnection.sln` to review the source code for the methods described below.

1. `SetupAndCleanupSteps.InitializeGenericMultiplexerSession(ISemiconductorModuleContext tsmContext, string multiplexerTypeId)` gets switch names from `tsmContext` for `multiplexerTypeId` and initializes a session for each switch.
2. `SetupAndCleanupSteps.CleanupGenericMultiplexerSession(ISemiconductorModuleContext tsmContext, string multiplexerTypeId)` gets all switch sessions from `tsmContext` for `multiplexerTypeId` and closes all switch sessions for `multiplexerTypeId`.
3. `TestStep.OneInstrumentChannelToManySitesForOneDutPin(ISemiconductorModuleContext tsmContext, string dutPinName, string endOfTestingRelayConfigurationName = "", string multiplexerTypeId = "NIGenericMultiplexer")` queries per-site routes for `dutPinName`, applies relay configurations, performs per-site measurements, publishes results, and applies `endOfTestingRelayConfigurationName`. By default, it uses `NIGenericMultiplexer`. Callers can override `multiplexerTypeId` when needed.

## Using the Example

Complete the following steps to run this example:

1. Open `STLExample.MultiplexedConnection.seq` in TestStand Sequence Editor.
2. Review the pin map by selecting **Semiconductor Module -> Edit Pin Map** from TestStand.
   - Confirm instrument definitions for `DMM_4081_C1_S02` and `RELAY_2567_C1_S03`.
   - Review DUT pin `A`, site definitions, and per-site multiplexed routes.
   - Review relay configurations that correspond to each `routeName`.
   - Review site connections (pin, site, instrument, channel).
3. Review the following sequence callbacks to understand workflow ownership:
   - `ProcessSetup`: Initializes Relay Module and DMM sessions using standard STL TestStandSteps, then calls `InitializeGenericMultiplexerSession`.
   - `MainSequence`: Calls `OneInstrumentChannelToManySitesForOneDutPin` to execute per-site routing/measurement.
   - `ProcessCleanup`: Calls `CleanupGenericMultiplexerSession` to close all switch sessions for `multiplexerTypeId`, then cleans up initialized instrument references using the standard STL TestStandSteps cleanup step.
4. The following callbacks are already configured in the sequence file:
   - Setup flow initializes Relay Module and DMM sessions.
   - `ProcessSetup` calls `InitializeGenericMultiplexerSession` with `multiplexerTypeId` set to `NIGenericMultiplexer`.
   - `MainSequence` calls `OneInstrumentChannelToManySitesForOneDutPin` with `dutPinName` set to `A` and `endOfTestingRelayConfigurationName` set to `DisconnectDmmFromPinAOnAllSites`.
   - `ProcessCleanup` calls `CleanupGenericMultiplexerSession` with `multiplexerTypeId` set to `NIGenericMultiplexer`, then calls standard cleanup instrumentation.
   - To use a different multiplexer type, pass `multiplexerTypeId` with the matching type ID from the pin map.
5. To run the test program, click the **Start/Resume Lot** button on the TSM toolbar.
