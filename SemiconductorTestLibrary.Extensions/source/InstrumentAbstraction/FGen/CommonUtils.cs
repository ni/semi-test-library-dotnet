using System.Linq;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    internal static class CommonUtils
    {
        /// <summary>
        /// Provides channel name from individual channel string.
        /// </summary>
        /// <param name="individualChannelString">IndividualChannelString</param>
        /// <returns>Channel name</returns>
        internal static string GetChannelName(string individualChannelString)
        {
            return individualChannelString.Split('/').Last();
        }
    }
}
