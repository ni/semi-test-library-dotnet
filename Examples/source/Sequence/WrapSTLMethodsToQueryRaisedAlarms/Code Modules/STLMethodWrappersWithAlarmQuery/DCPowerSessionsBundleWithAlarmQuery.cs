using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.Examples.SemiconductorTestLibrary.WrapSTLMethodsToQueryRaisedAlarms.STLMethodWrappersWithAlarmQuery.QueryForRaisedAlarmsWrappers;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.WrapSTLMethodsToQueryRaisedAlarms.STLMethodWrappersWithAlarmQuery
{
    /// <summary>
    /// Extension methods to wrap measure calls to query instrument alarms when taking measurements on a DCPower instrument.
    /// </summary>
    public class DCPowerSessionsBundleWithAlarmQuery : DCPowerSessionsBundle
    {
        /// <summary>
        /// This constructor is used to create a new instance of the <see cref="DCPowerSessionsBundleWithAlarmQuery"/> class.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="allSessionInformation">The collection of all <see cref="DCPowerSessionInformation"/> objects.</param>
        public DCPowerSessionsBundleWithAlarmQuery(ISemiconductorModuleContext semiconductorModuleContext, IEnumerable<DCPowerSessionInformation> allSessionInformation) : base(semiconductorModuleContext, allSessionInformation)
        {
        }

        /// <param name="clearAlarm">If true, clears any raised alarms before taking the measurement.</param>
        /// <inheritdoc cref="Measure.MeasureVoltage(DCPowerSessionsBundle)"/>
        public PinSiteData<double> MeasureVoltage(bool clearAlarm = true)
        {
            return WrapMethodWithQueryForRaisedAlarms(TSMContext, clearAlarm, () => Measure.MeasureVoltage(this));
        }

        /// <inheritdoc cref="Measure.MeasureCurrent(DCPowerSessionsBundle)"/>
        public PinSiteData<double> MeasureCurrent(bool clearAlarm = true)
        {
            return WrapMethodWithQueryForRaisedAlarms(TSMContext, clearAlarm, () => Measure.MeasureCurrent(this));
        }

        /// <inheritdoc cref="Measure.MeasureAndPublishVoltage(DCPowerSessionsBundle, string, out double[][])"/>
        /// <param name="publishedDataId"/>
        /// <param name="voltageMeasurements"/>
        /// <param name="clearAlarm">If true, clears any raised alarms before taking the measurement.</param>
        public void MeasureAndPublishVoltage(string publishedDataId, out double[][] voltageMeasurements, bool clearAlarm = true)
        {
            double[][] localVoltageMeasurements = new double[InstrumentSessions.Count()][];
            WrapMethodWithQueryForRaisedAlarms(TSMContext, clearAlarm, () => Measure.MeasureAndPublishVoltage(this, publishedDataId, out localVoltageMeasurements));
            voltageMeasurements = localVoltageMeasurements;
        }

        /// <inheritdoc cref="Measure.MeasureAndPublishVoltage(DCPowerSessionsBundle, string)"/>
        /// <param name="publishedDataId"/>
        public PinSiteData<double> MeasureAndPublishVoltage(string publishedDataId)
        {
            return WrapMethodWithQueryForRaisedAlarms(TSMContext, false, () => Measure.MeasureAndPublishVoltage(this, publishedDataId));
        }

        /// <inheritdoc cref="Measure.MeasureAndPublishCurrent(DCPowerSessionsBundle, string, out double[][])"/>
        /// <param name="publishedDataId"/>
        /// <param name="currentMeasurements"/>
        /// <param name="clearAlarm">If true, clears any raised alarms before taking the measurement.</param>
        public void MeasureAndPublishCurrent(string publishedDataId, out double[][] currentMeasurements, bool clearAlarm = true)
        {
            double[][] localCurrentMeasurements = new double[InstrumentSessions.Count()][];
            WrapMethodWithQueryForRaisedAlarms(TSMContext, clearAlarm, () => Measure.MeasureAndPublishCurrent(this, publishedDataId, out localCurrentMeasurements));
            currentMeasurements = localCurrentMeasurements;
        }

        /// <inheritdoc cref="Measure.MeasureAndPublishCurrent(DCPowerSessionsBundle, string)"/>
        /// <param name="publishedDataId"/>
        /// <param name="clearAlarm">If true, clears any raised alarms before taking the measurement.</param>
        public PinSiteData<double> MeasureAndPublishCurrent(string publishedDataId, bool clearAlarm = true)
        {
            return WrapMethodWithQueryForRaisedAlarms(TSMContext, clearAlarm, () => Measure.MeasureAndPublishCurrent(this, publishedDataId));
        }
    }
}
