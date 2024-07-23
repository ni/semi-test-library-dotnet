# Instrument Setup and Cleanup

The Semiconductor Test Library expects that instrument sessions can be created for and stored via a separate session manager. The library currently supports the session manager provided via the TestStand Semiconductor Module. This means that the Initialization and Cleanup of instrument sessions is intended to be invoked from TestStand's ProcessSetup and ProcessCleanup sequences, respectively.

## Using the Map Method

## NI DCPower Session Grouping

The instrument abstraction for the NI DCPower drive expects that the sessions are configured in one or more groups within the loaded pin map. If the load pin map does not use session groups, the `InitializeAndClose.Initialize` method will throw and exception.

Applicable namespace: `SemiconductorTestLibrary.InstrumentAbstraction.DCPower`
