# Supported Instrument Types

The Semiconductor Test Library supports the core set of modular instruments commonly used within the NI [Semiconductor Test System (STS)](https://www.ni.com/sts). Refer to the following table for the instrument types currently supported by the library.

> [!TIP]
> When Extensions are not supported by an instrument type, but a Abstractions support is provided, you can refer to the [Making Low-Level Driver Calls](advanced/MakingLowLevelDriverCalls.md) topic the to learn how to interact with the lower-level driver APIs at the Abstractions layer.
>
> If a instrument type is not listed in the table below, you will need to use the [TestStand Semiconductor Module Code Module API](https://www.ni.com/docs/en-US/bundle/teststand-semiconductor-module/page/tsm-code-module-api.html) in combination with the low-level instrument driver API and proceed with instrument channel centric programming. If you need any assistance with this, contact NI for support.

**Table 1:** Supported Instrument Types

| Instrument Type                           | Driver    | Abstractions | Extensions           |
| :--------------------------------------   | :-------- | :----------- | :------------------- |
| NI Source Measurement Unit (SMU)          | niDCPower | :heavy_check_mark: | :heavy_check_mark: |
| NI Programmable Power Supply (PPS)        | niDCPower | :heavy_check_mark: | :heavy_check_mark: |
| NI Multifunction I/O (DAQ) \*             | niDAQmx   | :warning: | :warning:   |
| NI Sound and Vibration Module (DSA) \* \* | niDAQmx   | :warning: | :warning: |
| NI Digital Pattern Instrument (DPI/HSD)   | niDigital | :heavy_check_mark: | :heavy_check_mark: |
| NI Digital Multimeter (DMM)               | niDmm     | :heavy_check_mark: | :heavy_check_mark: |
| NI Relay Module (RELAY)                   | niSwitch  | :heavy_check_mark: | :heavy_check_mark: |
| NI Function Generator (FGEN)              | niFgen    | :heavy_check_mark: | :x: |
| NI Digitizer/Oscilloscope (SCOPE)         | niScope   | :heavy_check_mark: | :x: |
| NI Timing Synchronization Module (SYNC)   | niSync    | :heavy_check_mark: | :x: |

> [!NOTE]
> :heavy_check_mark: Supported
>
> :warning: Only a Subset of Devices/Functionality Supported
>
> :x: Not Yet Supported
>
> \* \
> Supported Devices: PXIe-636x \
> Supported Functionality: Analog Input, Analog Output, Digital Input, Digital Output
>
> \* \* \
> Supported Devices: PXIe-4467/8 \
> Supported Functionality: Analog Input, Analog Output, Analog Output: Function Generation
