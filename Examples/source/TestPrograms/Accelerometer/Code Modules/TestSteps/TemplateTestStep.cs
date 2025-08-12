/*************************************************************
Boilerplate  template for creating a new custom test module.
Feel free to paste this when starting a new test method,
but make sure to update and rename the method signature,
as well as the documentation. The method signature should
match the file name.
*************************************************************/

using NationalInstruments.SemiconductorTestLibrary.Common;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Call this method from the test executive to execute the test code.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object reference.</param>
        public static void TemplateTestStep(ISemiconductorModuleContext semiconductorModuleContext)
        {
            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            // Write your test specific logic here...
        }
    }
}