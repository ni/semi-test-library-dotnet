# Supported Instrument Types

The Semiconductor Test Library supports the core set of modular instruments commonly used within the NI [Semiconductor Test System (STS)](https://www.ni.com/sts). The following table provides a list which instrument types are currently supported by the library.

**Table 1:** List of Supported Instrument Types

| Instrument Type                           | Driver    | Abstractions | Extensions           |
| :--------------------------------------   | :-------- | :----------- | :------------------- |
| NI Source Measurement Unit (SMU)          | niDCPower | Supported    | Core Support         |
| NI Programmable Power Supply (PPS)        | niDCPower | Supported    | Core Support         |
| NI Multifunction I/O (DAQ) \*             | niDAQmx   | Supported    | Limited Support \+   |
| NI Sound and Vibration Module (DSA) \* \* | niDAQmx   | Supported    | Limited Support \+\+ |
| NI Digital Pattern Instrument (DPI/HSD)   | niDigital | Supported    | Core Support         |
| NI Digital Multimeter (DMM)               | niDmm     | Supported    | Core Support         |
| NI Relay Module (RELAY)                   | niSwitch  | Supported    | Core Support         |
| NI Function Generator (FGEN)              | niFgen    | Supported    | Not Yet Supported    |
| NI Digitizer/Oscilloscope (SCOPE)         | niScope   | Supported    | Not Yet Supported    |
| NI Timing Synchronization Module (SYNC)   | niSync    | Supported    | Not Yet Supported    |

> [!NOTE]
> Core Support: the core functionality to use the instrument is supported, but some advanced features are not yet exposed with a high-level Extension method. Refer to the documentation regarding how to interact with the lower-level driver APIs using the provided Abstraction methods.
>
> Limited Support: Only the most common use cases are implemented.
>
> \* Only PXIe-6368 devices are currently supported.
>
> \* \* Only PXIe-4467/8 devices are currently supported.
>
> \+ Only Analog Input, Analog Output, Digital Input, and Digital Output task types are currently supported.
>
> \+\+ Only Analog Input, Analog Output, and Analog Output: Function Generation task types are supported.
