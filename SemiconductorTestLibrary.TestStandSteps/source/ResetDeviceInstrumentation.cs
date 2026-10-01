using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class CommonSteps
    {
        /// <summary>
        /// touches, including sites outside the calling step context.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void GetApertureTime(
            ISemiconductorModuleContext tsmContext)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);

                // Configure a distinct aperture time on the sibling before the reset so its restoration can be verified.
                var sessionsBundleVCC = sessionManager.DCPower("VCC");

                var sessionsBundleVDD = sessionManager.DCPower("VDD");

                sessionsBundleVDD.GetApertureTimeInSeconds(out var maximumApertureTime1);

                sessionsBundleVCC.GetApertureTimeInSeconds(out var maximumApertureTime2);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
