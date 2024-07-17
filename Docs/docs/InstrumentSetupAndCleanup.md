# Instrument Setup and Cleanup

## Using the Map Method

## NI DCPower Session Grouping

The instrument abstraction for the NI DCPower drive expects that the sessions are configured in one or more groups within the loaded pin map. If the load pin map does not use session groups, the `InitializeAndClose.Initialize` method will throw and exception.

Applicable namespace: `SemiconductorTestLibrary.InstrumentAbstraction.DCPower`
