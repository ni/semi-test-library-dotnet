# Scope Configured Acquisition Sequence Example

## Introduction

This example demonstrates how to use the Semiconductor Test Library specifically the Scope extension methods  for configured waveform acquisition. This example demonstrates the multi-site capability of scope extension methods using 2 sites in the pinmap file. This example can be modified for more sites by adding connection and site details only to the pinmap file.

### Highlighted Features

- Pin Map
- Multi Site
- Offline Mode

### Major APIs

- TSM Code Module API
- Semiconductor Test Library (Scope extension methods)

### Prerequisites

1. If you want to use the example you must have the following software installed:
   - STS Software 26.5.0 or later (see note below for using with STS Software 24.5.3)
2. To run the test program you must also have:
   - Two NI-Scope instruments named SCP_5172_C1_S02 and SCP_5172_C1_S03, respectively.
   - TestStand configured to use the Batch process model.

## Using the Example

Complete the steps in the following sections to learn about the test program components. You can also run this example in offline mode to see it in action.

1. Select **Semiconductor Module -> Edit Pin Map** File or click the **Edit Pin Map File** button on the TSM toolbar to open the Accelerometer pin map file in the Pin Map Editor.
The pin map file defines the following information:
   - One NI-SCOPE instrument named `SCOPE_5105_C1_S08`.
   - Ten DUT pins named `Vcc`, `Gnd`, and `Vref_OScope`. The `Vsc0` DUT pin used to connect the pin NI-SCOPE.
   - Two sites on the tester.
   - A series of connections for each site, in which each connection specifies a DUT pin, a site number, an instrument, and an instrument channel.

1. Complete the following steps to review the Configurations that this test program uses.
2. Each step is calling the Scope extension methods for configuring, Initiating and fetching waveforms.

