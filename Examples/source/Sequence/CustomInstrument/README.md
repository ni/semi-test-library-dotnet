# Custom Instrument Example

Custom Instrument example demonstrates using non-TSM native instruments such as Third-Party Instruments and NI RIO instruments through **Custom Instrument** Support provided in STL.
This example serves as template code, user can start with this example and replace dummy driver calls with actul calls to their instrument driver. User can also write additional extension methods and TestSteps according to their need.

CustomInstrumentExample TestStand sequence contains 3 sample tests to do static validation of DAC (Digital to Analog Converter) DUT by providing different digital inputs and measuring analog outputs.
Sample Tests:
1. ZeroScale Test
1. FullScale Test
1. Functional Test

**ZeroScale Test** 
Provide '0' input to all digital pins (A, B, C, D) and measure offset voltage on analog pins (E, F). Then compare the measured values with expected offset voltage.

**FullScale Test**
Provide '1' input to all digital pins (A, B, C, D) and measure voltage on analog pins (E, F). Then compare the measured values with expected FullScale voltage (VCC).

**Functional Test**
Provide different test inputs on digital pins (A, B, C, D) for e.g  provide {0,1,0,1} and measure analog  pins (E, F). Then compare the measured values with equivalent analong values.

## Contents:
- CustomInstrumentExample.seq - Example TestStand sequence demonstrates running sample validation tests on DUT.
- CustomInstrumentExample.pinmap - Dummy pinmap file containing devices and pin information.
- Code Modules/ExampleTestStep.cs - Contains sample TestSteps.
- Code Modules/SetupAndCleanupSteps.cs - Demonstarte creating methods for Setup and Cleanup operations.
- Code Modules/MyCustomInstrument/MyCustomInstrument.cs - Demonstarte writing concreate implementations for ICustomInstrument
- Code Modules/MyCustomInstrument/MyCustomInstrumentFactory.cs - Demonstarte writing concreate implementations for ICustomInstrumentFactory
- Code Modules/MyCustomInstrument/HighLevelDriverOperations.cs - Demonstarte writing different extension methods for driver operations. E.g. performing write operations, measure operations, etc.
- Code Modules/bin/SemiconductorTestLibrary.Examples.CustomerInstrument.dll - CustomInstrumentExample assembly built with above example code.
- Code Modules/Imports/MyCustomInstrumentDriverAPI - Dummy customer instrument driver API assembly.
