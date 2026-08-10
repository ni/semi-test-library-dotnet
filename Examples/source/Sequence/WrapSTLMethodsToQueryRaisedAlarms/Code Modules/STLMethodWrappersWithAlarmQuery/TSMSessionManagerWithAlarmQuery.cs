using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.WrapSTLMethodsToQueryRaisedAlarms.STLMethodWrappersWithAlarmQuery
{
    /// <summary>
    /// Wraps the <see cref="TSMSessionManager"/> class to provide additional functionality for querying raised alarms when creating a new <see cref="DCPowerSessionsBundleWithAlarmQuery"/>.
    /// </summary>
    public class TSMSessionManagerWithAlarmQuery : TSMSessionManager
    {
        private readonly ISemiconductorModuleContext _semiconductorModuleContext;

        /// <inheritdoc cref="TSMSessionManager(ISemiconductorModuleContext)"/>
        /// <param name="semiconductorModuleContext"/>
        public TSMSessionManagerWithAlarmQuery(ISemiconductorModuleContext semiconductorModuleContext) : base(semiconductorModuleContext)
        {
            _semiconductorModuleContext = semiconductorModuleContext;
        }

        /// <inheritdoc cref="TSMSessionManager.DCPower(string)"/>>
        /// <returns>
        /// A new <see cref="DCPowerSessionsBundleWithAlarmQuery"/> object associated with the specified pins.
        /// </returns>
        public new DCPowerSessionsBundleWithAlarmQuery DCPower(string pin)
        {
            return DCPower(new string[] { pin });
        }

        /// <inheritdoc cref="TSMSessionManager.DCPower(string[])"/>>
        /// <returns>
        /// A new <see cref="DCPowerSessionsBundleWithAlarmQuery"/> object associated with the specified pins.
        /// </returns>
        public new DCPowerSessionsBundleWithAlarmQuery DCPower(string[] pins)
        {
            // This is the same code implementation as the STL native DigitalSessionsBundle class.
            _semiconductorModuleContext.GetNIDCPowerSessions(pins, out var sessions, out var pinSetStrings);
            var pinRange = _semiconductorModuleContext.GetPinsInPinGroups(pins);
            var siteRange = _semiconductorModuleContext.SiteNumbers.Append(SitePinInfo.SiteNumberNone).ToArray();
            var allSessionInfo = new List<DCPowerSessionInformation>();
            for (int i = 0; i < sessions.Length; i++)
            {
                allSessionInfo.Add(new DCPowerSessionInformation(sessions[i], pinSetStrings[i], pinRange, siteRange));
            }
            return new DCPowerSessionsBundleWithAlarmQuery(_semiconductorModuleContext, allSessionInfo);
        }
    }
}