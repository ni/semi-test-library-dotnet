using NationalInstruments.DAQmx;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// The class is used to configure the settings for an Analog Output Function Generation DAQmx task.
    /// </summary>
    /// <remarks>
    /// Using this class is advantageous as it allows the user to set one or more properties in a single call.
    /// </remarks>
    public class AOFunctionGenerationSettings
    {
        /// <summary>
        /// Specifies the kind of the waveform to generate (sine, square, triangle, or sawtooth).
        /// </summary>
        /// <remarks>
        /// This value is used to get/set the value of the NationalInstruments.DAQmx.AOChannel.FunctionGenerationType.
        /// </remarks>
        public AOFunctionGenerationType? FunctionType { get; set; }
        /// <summary>
        /// Specifies the frequency of the waveform to generate.
        /// </summary>
        /// <remarks>
        /// This value is used to get/set the value of the NationalInstruments.DAQmx.AOChannel.FunctionGenerationFrequency.
        /// </remarks>
        public double? Frequency { get; set; }
        /// <summary>
        /// Specifies the amplitude of the waveform to generate.
        /// </summary>
        /// <remarks>
        /// This value is used to get/set the value of the NationalInstruments.DAQmx.AOChannel.FunctionGenerationAmplitude.
        /// </remarks>
        public double? Amplitude { get; set; }
        /// <summary>
        /// Specifies the offset of the waveform to generate.
        /// </summary>
        /// <remarks>
        /// This value is used to get/set the value of the NationalInstruments.DAQmx.AOChannel.FunctionGenerationOffset.
        /// </remarks>
        public double? Offset { get; set; }
        /// <summary>
        /// Specifies the start phase of the waveform to generate.
        /// </summary>
        /// <remarks>
        /// This value is used to get/set the value of the NationalInstruments.DAQmx.AOChannel.FunctionGenerationStartPhase.
        /// </remarks>
        public double? StartPhase { get; set; }
        /// <summary>
        /// Specifies the duty cycle of the waveform.
        /// Only applicable when the FunctionType is set to AOFunctionGenerationType.Square.
        /// </summary>
        /// <remarks>
        /// This value is used to get/set the value of the NationalInstruments.DAQmx.AOChannel.FunctionGenerationDutyCycle.
        /// </remarks>
        public double? DutyCycle { get; set; }

        /// <summary>
        /// Creates an object to define and set Analog Output Function Generation specific settings.
        /// This object is used as a parameter for configuring an Analog Output Function Generation DAQmx task <see cref="AnalogOutputFunctionGeneration.ConfigureAOFunctionGeneration(DAQmxTasksBundle, AOFunctionGenerationSettings)"/>
        /// </summary>
        public AOFunctionGenerationSettings()
        {
        }
    }

    /// <summary>
    /// The class is used to configure the timing settings for a DAQmx task.
    /// </summary>
    /// <remarks>
    /// Using this class is advantageous as it allows the user to set one or more properties in a single call.
    /// </remarks>
    public class DAQmxTimingSampleClockSettings
    {
        /// <summary>
        /// Returns the NationalInstruments.DAQmx.SampleTimingType.SampleClock sample timing type.
        /// </summary>
        /// <remarks>
        /// This value is used to set the value of the NationalInstruments.DAQmx.Timing.SampleTimingType driver property.
        /// </remarks>
        public SampleTimingType SampleTimingType { get { return SampleTimingType.SampleClock; } }
        /// <summary>
        /// Specifies the terminal of the signal to use as the Sample Clock.
        /// </summary>
        /// <remarks>
        /// Use this property to specify an external source to use as the Sample Clock.
        /// This value is used to get/set the value of the NationalInstruments.DAQmx.Timing.SampleClockSource driver property.
        /// </remarks>
        public string SampleClockSource { get; set; }
        /// <summary>
        /// Specifies on which edge of a clock pulse sampling takes place.
        /// </summary>
        /// <remarks>
        /// This value is used to get/set the value of the NationalInstruments.DAQmx.Timing.SampleClockActiveEdge driver property.
        /// This property is useful primarily when the signal you use as the Sample Clock is not a periodic clock.
        /// </remarks>
        public SampleClockActiveEdge? SampleClockActiveEdge { get; set; }
        /// <summary>
        /// Specifies the sampling rate in samples per channel per second.
        /// </summary>
        /// <remarks>
        /// If you use an external source for the Sample Clock, set the value to the maximum expected rate of that clock.
        /// This value is used to get/set the value of the NationalInstruments.DAQmx.Timing.SampleClockRate driver property.
        /// </remarks>
        public double? SampleClockRate { get; set; }
        /// <summary>
        /// Specifies if a task acquires or generates a finite number of samples or if it
        /// continuously acquires or generates samples.
        /// </summary>
        /// <remarks>
        /// This value is used to get/set the value of the NationalInstruments.DAQmx.Timing.SampleQuantityMode driver property.
        /// </remarks>
        public SampleQuantityMode? SampleQuantityMode { get; set; }
        /// <summary>
        /// Specifies the number of samples to acquire or generate for each channel in the task.
        /// </summary>
        /// <remarks>
        /// This value is used to get/set the value of the NationalInstruments.DAQmx.Timing.SampleClockSource driver property.
        /// </remarks>
        public int? SamplesPerChannel { get; set; }

        /// <summary>
        /// Creates an object to define and set DAQmx Timing Sample Clock specific settings.
        /// This object is used as a parameter for configuring a sample clock timing type for a DAQmx task. <see cref="Configure.ConfigureTiming(DAQmxTasksBundle, DAQmxTimingSampleClockSettings)"/>
        /// </summary>
        public DAQmxTimingSampleClockSettings()
        {
        }
    }
}
