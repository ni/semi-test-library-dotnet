using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class CommonSteps
    {
        // TODO: Testcase:5 Removing Method

        private static IDictionary<string, DCPowerSourceSettings> BuildDCPowerSourceSettings(IList<string[]> pins, IList<int> pinIndexes, double[] voltageLevels, double[] currentLimits)
        {
            var settings = new Dictionary<string, DCPowerSourceSettings>();
            for (int i = 0; i < pins.Count; i++)
            {
                int translatedIndex = pinIndexes[i];
                foreach (var pin in pins[i])
                {
                    settings.Add(pin, new DCPowerSourceSettings()
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        Level = voltageLevels[translatedIndex],
                        Limit = currentLimits[translatedIndex],
                    });
                }
            }
            return settings;
        }

        private static IDictionary<string, PPMUSettings> BuildPPMUSettings(IList<string[]> pins, IList<int> pinIndexes, double[] voltageLevels, double[] currentLimits)
        {
            var settings = new Dictionary<string, PPMUSettings>();
            for (int i = 0; i < pins.Count; i++)
            {
                int translatedIndex = pinIndexes[i];
                foreach (var pin in pins[i])
                {
                    settings.Add(pin, new PPMUSettings()
                    {
                        OutputFunction = PpmuOutputFunction.DCVoltage,
                        VoltageLevel = voltageLevels[translatedIndex],
                        CurrentLimitRange = currentLimits[translatedIndex],
                    });
                }
            }
            return settings;
        }
    }
}
