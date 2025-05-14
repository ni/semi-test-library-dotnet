using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.MultiplexedConnections.Common.MyDMM
{
    /// <summary>
    /// Defines <see cref="NIDmm"/> sessions initialize and close APIs.
    /// </summary>
    public static class InitializeAndClose
    {
        /// <summary>
        /// Initializes <see cref="NIDmm"/> sessions in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        public static void Initialize(ISemiconductorModuleContext tsmContext, bool resetDevice = false)
        {
            var instrumentNames = tsmContext.GetNIDmmInstrumentNames();
            foreach (var instrumentName in instrumentNames)
            {
                var session = new NIDmm(instrumentName, idQuery: true, resetDevice);
                tsmContext.SetNIDmmSession(instrumentName, session);
            }

            MyDMMSessionInformation.GenerateResourceDescriptorToSitePinDictionary(tsmContext);
        }
    }
}
