using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class CommonSteps
    {
        /// <summary>
        /// Bursts a pattern and publishes the pass/fail results as well as the fail count.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsOrPinGroups">The pins or pin groups.</param>
        /// <param name="patternName">The name of the pattern to burst.</param>
        public static void BurstPattern(ISemiconductorModuleContext tsmContext, string[] pinsOrPinGroups, string patternName)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                var digital = sessionManager.Digital(pinsOrPinGroups);
                digital.BurstPatternAndPublishResults(patternName, publishedDataId: "Pattern Pass/Fail Result");
                digital.DoAndPublishResults(sessionInfo => sessionInfo.PinSet.GetFailCount().ToDoubleArray(), "Pattern Fail Count");
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }

        private static double[] ToDoubleArray(this long[] failCounts)
        {
            return Array.ConvertAll(failCounts, value => (double)value);
        }
    }
}
