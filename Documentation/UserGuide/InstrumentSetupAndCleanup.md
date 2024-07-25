# Instrument Setup and Cleanup

The Semiconductor Test Library expects instrument sessions to be created for and stored via a separate session manager. The library currently leverages the TestStand Semiconductor Module to act its session manager, where the initialization and cleanup of instrument sessions is intended to be invoked from TestStand's ProcessSetup and ProcessCleanup sequences, respectively.

The Semiconductor Test Library provides instrument type specific initialization and cleanup code in the `Initialization` class.

> [!NOTE]
> Class: `Initialization`\
> Namespace: `NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.<instrument type` \
> Assembly: `NationalInstruments.SemiconductorTestLibrary.Abstractions.dll`

Additionally, the TestStandSteps provides the high-level,

> [!NOTE]
> Namespace: `NationalInstruments.SemiconductorTestLibrary.TestStandSteps` \
> Assembly: `NationalInstruments.SemiconductorTestLibrary.TestStandSteps.dll`

## Using the Map Method

As of STS Software 24.5, the NationalInstruments.SemiconductorTestLibrary.TestStandSteps assembly provides method that are available as step types for you to drag-and-drop from the TestStand Insertion Pallet. These provided methods aid quick commonly suers...

The source code for these steps are available on github for reference.

It is expect that users can leverage

It also uses abstractions, they may not be the same memory space. This is not a problem except for init code

nuget inted

Use the `MapInitializedInstrumentSessions` method from the `Initialization` class to ensure the cached instrument session information is consistent between the NationalInstruments.SemiconductorTestLibrary.TestStandSteps assembly and client assembly.

When using the Steps provided by the the NationalInstruments.SemiconductorTestLibrary.TestStandSteps assembly in your test sequence, and writing although it uses the NationalInstruments.SemiconductorTestLibrary.Abstractions assembly is dedent on the the NationalInstruments.SemiconductorTestLibrary.Abstractions.  they may not share the same memory space if the client assembly is using a newer version of the NationalInstruments.SemiconductorTestLibrary.Abstractions.

If the client assembly is using a newer version of the NationalInstruments.SemiconductorTestLibrary.Abstractions, then will not share the same memory space which can cause problems.

## NI DCPower Session Groups

The instrument abstraction for the NI DCPower drive expects that the sessions are configured in one or more groups within the loaded pin map. If the load pin map does not use session groups, the `InitializeAndClose.Initialize` method will throw and exception.

Applicable namespace: `SemiconductorTestLibrary.InstrumentAbstraction.DCPower`

**Related information:**

- LINK
