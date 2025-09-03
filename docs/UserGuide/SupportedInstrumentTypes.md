# Supported Instrument Types

The Semiconductor Test Library supports the core set of modular instruments commonly used within the NI [Semiconductor Test System (STS)](https://www.ni.com/sts). The table below shows the specific instrument types currently supported by the library.

For unsupported instrument types, it is possible to implement low-level programming base on instrument channels in your test code. Use [Custom Instrument Support](CustomInstrumentSupport.md) to write your own extension methods that control the desired instrument type. Contact NI support for assistance.

**Table 1:** Supported Instrument Types

| Instrument Type                           | Driver    | Abstractions | Extensions           |
| :--------------------------------------   | :-------- | :----------- | :------------------- |
| NI Source Measurement Unit (SMU)          | niDCPower | :heavy_check_mark: | :heavy_check_mark: |
| NI Programmable Power Supply (PPS)        | niDCPower | :heavy_check_mark: | :heavy_check_mark: |
| NI Multifunction I/O (DAQ) \*             | niDAQmx   | :warning: | :warning: |
| NI Sound and Vibration Module (DSA) \* \* | niDAQmx   | :warning: | :warning: |
| NI Analog Input Module (DAQ) \* \* \*     | niDAQmx   | :warning: | :warning: |
| NI Digital Pattern Instrument (DPI/HSD)   | niDigital | :heavy_check_mark: | :heavy_check_mark: |
| NI Digital Multimeter (DMM)               | niDmm     | :heavy_check_mark: | :heavy_check_mark: |
| NI Relay Module (RELAY)                   | niSwitch  | :heavy_check_mark: | :heavy_check_mark: |
| NI Function Generator (FGEN)              | niFgen    | :heavy_check_mark: | :x: |
| NI Digitizer/Oscilloscope (SCOPE)         | niScope   | :heavy_check_mark: | :x: |
| NI Timing Synchronization Module (SYNC)   | niSync    | :heavy_check_mark: | :x: |

> [!NOTE]
> :heavy_check_mark: Supported
>
> :warning: Subset of Devices and/or Device Functionality Supported
>
> :x: Not Yet Supported
>
> \* \
> Supported Devices: PXIe-636x \
> Supported Functionality: Analog Input, Analog Output, Digital Input, Digital Output
>
> \* \* \
> Supported Devices: PXIe-4467, PXIe-4468 \
> Supported Functionality: Analog Input, Analog Output, Analog Output: Function Generation
>
> \* \* \* \
> Supported Devices: PXIe-4309, PXIe-4310 \
> Supported Functionality: Analog Input

> [!TIP]
> When Extensions are not supported by an instrument type, but Abstractions support is provided, you can refer to the [Making Low-Level Driver Calls](advanced/MakingLowLevelDriverCalls.md) topic to learn how to interact with the low-level driver APIs at the Abstractions layer.
