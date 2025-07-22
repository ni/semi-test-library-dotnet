Custom Instrument Example

This example can also be used as a template starting point for implementing a new Custom Instrument. First remove the dummy driver assembly from the imports directory, replacing it with your actual custom instrument driver assembly. Then replace the dummy driver calls with calls to the actual custom instrument driver, and write additional extension methods and TestSteps as needed

The example TestStand sequence contains 3 sample steps to test a hypothetical Device Under Test (DUT) using a custom instrument. The DUT is considered to be some kind of Digital to Analog Converter (DAC) and the Custom Instrument to be some kind of multi-functional Data Acquisition (DAQ) device capable of sourcing digital signals and acquiring analog signals. The DUT is connected to the CustomInstrument so that it can receive digital signals from the CustomInstrument and output an analog signal back to the CustomInstrument to be acquired."
Sample Tests:
1. ZeroScale Test
2. FullScale Test
3. Functional Test

ZeroScale Test
- Provide '0' input to all digital pins (A, B, C, D) and measure offset voltage on analog pins (E, F). Then compare the measured values with expected offset voltage.

FullScale Test
- Provide '1' input to all digital pins (A, B, C, D) and measure voltage on analog pins (E, F). Then compare the measured values with expected FullScale voltage (VCC).

Functional Test
- Provide different test inputs on digital pins (A, B, C, D) for e.g  provide {0,1,0,1} and measure analog  pins (E, F). Then compare the measured values with equivalent analog values.

Prerequisites: You must have the following software installed if you want
to use the example with real instruments or in OfflineMode:
- STS Software 24.5.0 or later

Contents:
- CustomInstrumentExample.seq - Example TestStand sequence demonstrates running sample validation tests on DUT.
- CustomInstrumentExample.pinmap - Dummy pinmap file containing devices and pin information.
- Code Modules/ExampleTestStep.cs - Contains sample TestSteps.
- Code Modules/SetupAndCleanupSteps.cs - Demonstarte creating methods for Setup and Cleanup operations.
- Code Modules/MyCustomInstrument/MyCustomInstrument.cs - Demonstarte writing concreate implementations for ICustomInstrument
- Code Modules/MyCustomInstrument/MyCustomInstrumentFactory.cs - Demonstarte writing concreate implementations for ICustomInstrumentFactory
- Code Modules/MyCustomInstrument/HighLevelDriverOperations.cs - Demonstarte writing different extension methods for driver operations. E.g. performing write operations, measure operations, etc.
- Code Modules/bin/SemiconductorTestLibrary.Examples.CustomInstrument.dll - CustomInstrumentExample assembly built with above example code.
- Code Modules/Imports/MyCustomInstrumentDriverAPI - Dummy custom instrument driver API assembly.
