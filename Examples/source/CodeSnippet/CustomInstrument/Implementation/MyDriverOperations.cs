using System.Linq;
using NationalInstruments;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.CustomInstrument.Examples
{
    /// <summary>
    /// This class contains examples methods to perform DriverOperations using CustomInstrument support provided in STL.
    /// </summary>
    public static partial class MyDriverOperations
    {
        /// <summary>
        /// Sample method to Perform specific driver operation .
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">TSM Context</param>
        /// <param name="parameter1">TSM Context</param>
        /// <param name="parameter2">TSM Context</param>
        public static void PerformDriverOperation1(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle, double parameter1, double parameter2)
        {
            myCustomInstrumentSessionsBundle.Do(customInstrumentSessionInfo =>
            {
                // customInstrumentSessionInfo.Session.<driver call for specific driver operations defined in extension class>
            });
        }

        /// <summary>
        /// Sample method to apply device configurations.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">TSM Context</param>
        /// <param name="parameter1">TSM Context</param>
        /// <param name="parameter2">TSM Context</param>
        public static void ConfigureMyCustomInstrument(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle, double parameter1, double parameter2)
        {
            myCustomInstrumentSessionsBundle.Do(customInstrumentSessionInfo =>
            {
                // customInstrumentSessionInfo.Session.<driver call for configuring the device defined in extension class>
            });
        }

        /// <summary>
        /// Sample method to clear configurations.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">TSM Context</param>

        public static void ClearConfiguration(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle)
        {
            myCustomInstrumentSessionsBundle.Do(customInstrumentSessionInfo =>
            {
                // customInstrumentSessionInfo.Session.<driver call clearing configurations defined in extension class>
            });
        }
    }
}
