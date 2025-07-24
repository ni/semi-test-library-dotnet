# Custom Instrument Example
The example TestStand sequence contains 3 sample steps to test a hypothetical Device Under Test (DUT) using a custom instrument.
The DUT is considered to be some kind of Digital to Analog Converter (DAC) and the Custom Instrument to be some kind of multi-functional Data Acquisition (DAQ) device capable of sourcing digital signals and acquiring analog signals.
The DUT is connected to the CustomInstrument so that it can receive digital signals from the CustomInstrument and output an analog signal back to the CustomInstrument to be acquired.
## Sample Tests
### ZeroScale Test
1. Provides '0' input to all digital pins (A, B, C, D).
2. Measures offset voltage on analog pins (E, F).
3. Compares the measured values with expected offset voltage.
### FullScale Test
1. Provides '1' input to all digital pins (A, B, C, D).
2. Measures voltage on analog pins (E, F).
3. Compares the measured values with expected FullScale voltage (VCC).
### Functional Test
1. Provides different test inputs on digital pins (A, B, C, D), for example: {0,1,0,1}.
2. Measures analog pins (E, F).
3. Compares the measured values with equivalent analog values.
## Prerequisites
You must have the following software installed if you want
to use the example:
- STS Software 25.5.0 or later
> Note that this example does not required any physical hardware to run.
## Exploring the Example
### Key Files
- STLExample.CustomInstrument.seq - Example TestStand sequence demonstrates running sample validation tests on DUT.
- STLExample.CustomInstrument.pinmap - Dummy pin map file containing devices and pin information.
- Code Modules/ExampleTestStep.cs - Contains sample TestSteps.
- Code Modules/SetupAndCleanupSteps.cs - Demonstrates creating methods for Setup and Cleanup operations.
- Code Modules/MyCustomInstrument/MyCustomInstrument.cs - Demonstrates writing concrete implementations for ICustomInstrument
- Code Modules/MyCustomInstrument/MyCustomInstrumentFactory.cs - Demonstrates writing concrete implementations for ICustomInstrumentFactory
- Code Modules/MyCustomInstrument/HighLevelDriverOperations.cs - Demonstrates writing different extension methods for driver operations. For example, performing write operations, measure operations, etc.
- Code Modules/bin/NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.dll - CustomInstrumentExample assembly built with above example code.
- Code Modules/Imports/MyCustomInstrumentDriverAPI - Dummy custom instrument driver API assembly
## Using this Example as a Template
This example can also be used as a template starting point for implementing a new Custom Instrument.
1. First remove the dummy driver assembly from the imports directory, replacing it with your actual custom instrument driver assembly.
2. Replace the dummy driver calls with calls to the actual custom instrument driver.
3. Write additional extension methods and TestSteps as needed.