namespace Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        // Constant values shared across multiple methods within the TestSteps class.
        private const string TemperatureControllerDataId = "TempCtrl";
        private const string ReadPartNumberPatternName = "ReadPartNumber";
        private const string SetVrefValuePatternName = "SetVrefValue";
        private const string VccTypicalSpecSymbol = "DC.Vcc_Typical";
        private const string DcVccTypicalMaxCurrentSpecSymbol = "DC.Vcc_Typical_Max_Current";
        private const string SetVrefValueWaveformName = "SetVrefValueWaveform";
        private const string GetVrefValueWaveformName = "GetVrefValueWaveform";
    }
}
