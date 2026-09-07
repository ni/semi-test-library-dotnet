using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.TestSteps
{
    public static partial class DUTPowerUpAndDown
    {
        public static void PowerUpDUT(ISemiconductorModuleContext tsmContext, double voltageLevel = 3.3, double currentLimitRange = 0.002)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            // Drive CSB low FIRST so the BME280 latches SPI mode as the rail comes up.
            sessionManager.Digital("CSB").WriteStatic(PinState._0);

            // Power the DUT: PPMU-force VIN to 3.3 V (2 mA range is ample for this board).
            sessionManager.Digital("VIN").ForceVoltage(voltageLevel: voltageLevel, currentLimitRange: currentLimitRange);
        }

        public static void PowerDownDut(ISemiconductorModuleContext tsmContext)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            sessionManager.Digital("VIN").ForceVoltage(voltageLevel: 0.0, currentLimitRange: 0.002); // remove power
            sessionManager.Digital("CSB").WriteStatic(PinState.X);                                    // stop driving CSB
        }
    }
}
