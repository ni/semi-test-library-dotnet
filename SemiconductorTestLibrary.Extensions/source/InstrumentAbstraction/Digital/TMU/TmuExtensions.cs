using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.TMU;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital
{
    /// <summary>
    /// Contains implementation of STL extension methods for TMU operations.
    /// </summary>
    public static class TmuExtensions
    {
        /// <summary>
        /// Initiates the TMU measurement for the assigned TMU resource of each pin within the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <remarks>
        /// Before initiating, the <see cref="SelectedFunction"/> is set to <see cref="SelectedFunction.Digital"/> and the <see cref="TerminationMode"/> is set to <see cref="TerminationMode.HighZ"/> for the associated pin(s).<br/>
        /// This function validates TMU configuration, clears the measurement buffers for the specified TMU(s), and prepares the hardware for making the a TMU measurement.<br/>
        /// All TMU configuration attributes must be set before calling this function.<br/>
        /// The driver validates and commits TMU configuration parameters when this function is called, not when Commit() is called.<br/>
        /// This function returns immediately, it does not wait for the measurement to complete.<br/>
        /// Once a measurement is initiated, any attempt to modify TMU configuration will fail until the measurement completes or is aborted.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pinNames">Specific pins to initiate the TMU measurement on. When <c>null</c>, all pins are targeted.</param>
        public static void TMUInitiate(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    SetDigitalHighZState(sessionInfo);
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.Initiate();
                }
            });
        }

        /// <summary>
        /// Enables the assigned TMU resource of each pin within the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <remarks>
        /// Invoking this method creates a clear separation between TMU configuration and resource reservation.<br/>
        /// The assigned TMU resource is reserved when this method is called.<br/>
        /// Enabling a TMU with invalid configuration will result in an error when <see cref="TMUInitiate" /> is called.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pinNames">Specific pins for which to enable the assigned TMU resource. When <c>null</c>, all pins are targeted.</param>
        public static void EnableTMU(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.Enabled = true;
                }
            });
        }

        /// <summary>
        /// Disables the assigned TMU resource of each pin within the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <remarks>
        /// Invoking this method creates a clear separation between TMU configuration and resource reservation.<br/>
        /// The assigned TMU resource is unreserved when this method is called.<br/>
        /// The TMU configuration (start/stop sources, event polarities, etc.) remain intact when this method is called.<br/>
        /// This method does not validate the TMU configuration.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pinNames">Specific pins for which to disable the assigned TMU resource. When <c>null</c>, all pins are targeted.</param>
        public static void DisableTMU(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.Enabled = false;
                }
            });
        }

        /// <summary>
        /// Aborts any in-progress TMU measurement(s) on TMU resource of each pin within the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <remarks>
        /// If no measurement is in progress on the TMU resource associated with any of the pin within the <see cref="DigitalSessionsBundle"/>,
        /// then this function has no effect for that TMU resource.
        /// This function does not affect TMU configuration or reservation state.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pinNames">Specific pins for which to abort the operation on the associated TMU resource. When <c>null</c>, all pins are targeted.</param>
        public static void TMUAbort(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.Abort();
                }
            });
        }

        /// <summary>
        /// Checks for available TMU resources and attempts to perform a soft assignment for each session.<br/>
        /// If enough TMU resources are available, a TMU resource will automatically be assigned
        /// and the TMU context name of that resource stored within each <see cref="DigitalSessionInformation" /> of the <see cref="DigitalSessionsBundle" />.
        /// Otherwise, an exception will be thrown.
        /// </summary>
        /// <remarks>
        /// TMU resource assignment is virtual.<br/>
        /// This method does not reserve TMU resources at the hardware level.<br/>
        /// TMU resources are not enabled or disabled by this method.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pinNames">The pins to assign a TMU resource to. When <c>null</c>, all pins are targeted.</param>
        public static void AssignTMUResources(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            try
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AssignTMUContexts(pinNames);
                });
            }
            catch
            {
                // Clear partially assigned TMU resources in case of exception
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.ClearAssignedTMUContexts(pinNames);
                });

                throw; // rethrow the original exception.
            }
        }

        /// <summary>
        /// Clears any assigned TMU contexts stored within each <see cref="DigitalSessionInformation" /> of the <see cref="DigitalSessionsBundle" />.
        /// </summary>
        /// <remarks>
        /// TMU resource assignment is virtual.<br/>
        /// This method does not unreserve TMU resources at the hardware level.<br/>
        /// TMU resources are not enabled or disabled by this method.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pinNames">Pins for which the assigned TMU should be cleared. When <c>null</c>, all pins are targeted.</param>
        public static void ClearTMUAssignment(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.ClearAssignedTMUContexts(pinNames);
            });
        }

        /// <summary>
        /// Configures the TMU to perform a period measurement for pins in the sessions bundle.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will set the necessary attributes to configure period measurement
        /// for each of the assigned TMU resources.<br/>
        /// This method will also enable the assigned TMU resources.
        /// The specific attributes values set depends on the value of the <paramref name="edgeType"/> parameter.<br/>
        /// </para>
        /// <para>
        /// For rising edge period (<see cref="TmuPolarity.RisingEdge"/>):<br/>
        /// - <see cref="TmuAttributes.TmuStartSource"/> = the associated pin<br/>
        /// - <see cref="TmuAttributes.TmuStartSourceEvent"/> = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="TmuAttributes.TmuStartSourceEventPolarity"/> = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// - <see cref="TmuAttributes.TmuStopSource"/> = same pin a start source<br/>
        /// - <see cref="TmuAttributes.TmuStopSourceEvent"/> = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="TmuAttributes.TmuStopSourceEventPolarity"/> = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// - <see cref="TmuAttributes.TmuSamplesToAcquire"/> = value of <paramref name="samplesToAcquire"/> parameter.<br/>
        /// - <see cref="TmuAttributes.TmuArmType"/> = value of <paramref name="armType"/> parameter.<br/>
        /// - <see cref="TmuAttributes.TmuEnabled"/> = <c>true</c>
        /// </para>
        /// <para>
        /// For falling edge period (<see cref="TmuPolarity.FallingEdge"/>):<br/>
        /// - <see cref="TmuAttributes.TmuStartSource"/> = the associated pin<br/>
        /// - <see cref="TmuAttributes.TmuStartSourceEvent"/> = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="TmuAttributes.TmuStartSourceEventPolarity"/> = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// - <see cref="TmuAttributes.TmuStopSource"/> = same pin a start source<br/>
        /// - <see cref="TmuAttributes.TmuStopSourceEvent"/> = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="TmuAttributes.TmuStopSourceEventPolarity"/> = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// - <see cref="TmuAttributes.TmuSamplesToAcquire"/> = value of <paramref name="samplesToAcquire"/> parameter.<br/>
        /// - <see cref="TmuAttributes.TmuArmType"/> = value of <paramref name="armType"/> parameter.<br/>
        /// - <see cref="TmuAttributes.TmuEnabled"/> = <c>true</c>
        /// </para>
        /// If the <paramref name="edgeType"/> parameter is set to<see cref="TmuPolarity.EitherEdge"/>, an exception will be thrown.<br/>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="edgeType">The type of edge to detect. Only accepts <see cref="TmuPolarity.RisingEdge"/> or <see cref="TmuPolarity.FallingEdge"/>.</param>
        /// <param name="samplesToAcquire">The number of samples to acquire for the TMU measurement.</param>
        /// <param name="armType">
        /// The type of signal used to arm the TMU measurement.<br/>
        /// The TMU's arm input is used to frame, or select, the start and stop events of interest for each TMU sample.
        /// </param>
        /// <param name="pinNames">The specific pins to configure the TMU for. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigurePeriodMeasurement(this DigitalSessionsBundle sessionsBundle, TmuPolarity edgeType, long samplesToAcquire, TmuArmType armType = TmuArmType.Immediate, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    ConfigurePeriodMeasurementForSitePin(sessionInfo, sitePinInfo, edgeType, samplesToAcquire, armType);
                }
            });
        }

        /// <summary>
        /// Fetches the averaged TMU measurement for pins in the sessions bundle.
        /// </summary>
        /// <remarks>
        /// This method will wait for the measurement to complete,
        /// up to the amount of seconds defined by the <paramref name="timeoutInSeconds"/> parameter.<br/>
        /// An exception will be thrown if a timeout occurs during the measurement,
        /// or if the measurement was aborted before it completed.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="timeoutInSeconds">Maximum time (in seconds) to wait for the measurement to complete.</param>
        /// <param name="pinNames">The specific pins to fetch the TMU measurement for. When <c>null</c>, all pins are targeted.</param>
        /// <returns>The averaged measurement value fetched from the TMU resource, for each pin and site.</returns>
        public static PinSiteData<double> FetchAveragedTMUMeasurement(this DigitalSessionsBundle sessionsBundle, double timeoutInSeconds = 5, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    return tmu.FetchAveragedMeasurement(timeoutInSeconds);
                }
                return double.NaN;
            });
        }

        #region Configure TMU Start Source

        /// <summary>
        /// Configures the TMU start source for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUStartSource(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.Start.Source = sitePinInfo.IndividualChannelString;
                }
            });
        }

        #endregion

        #region Configure TMU Stop Source

        /// <summary>
        /// Configures the TMU stop source for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUStopSource(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.Stop.Source = sitePinInfo.IndividualChannelString;
                }
            });
        }

        #endregion

        #region Configure TMU Start Source Event

        /// <summary>
        /// Configures the TMU start source event for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="sourceEvent">The source event type.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUStartSourceEvent(this DigitalSessionsBundle sessionsBundle, TmuSourceEvent sourceEvent, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.Start.SourceEvent = sourceEvent;
                }
            });
        }

        #endregion

        #region Configure TMU Stop Source Event

        /// <summary>
        /// Configures the TMU stop source event for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="sourceEvent">The source event type.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUStopSourceEvent(this DigitalSessionsBundle sessionsBundle, TmuSourceEvent sourceEvent, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.Stop.SourceEvent = sourceEvent;
                }
            });
        }

        #endregion

        #region Configure TMU Start Source Event Polarity

        /// <summary>
        /// Configures the TMU start source event polarity for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="polarity">The source event polarity.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUStartSourceEventPolarity(this DigitalSessionsBundle sessionsBundle, TmuPolarity polarity, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.Start.SourceEventPolarity = polarity;
                }
            });
        }

        #endregion

        #region Configure TMU Stop Source Event Polarity

        /// <summary>
        /// Configures the TMU stop source event polarity for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="polarity">The edge polarity.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUStopSourceEventPolarity(this DigitalSessionsBundle sessionsBundle, TmuPolarity polarity, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.Stop.SourceEventPolarity = polarity;
                }
            });
        }

        #endregion

        #region Configure TMU Arm Type

        /// <summary>
        /// Configures the TMU arm type for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="armType">The arm type.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUArmType(this DigitalSessionsBundle sessionsBundle, TmuArmType armType, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.ArmType = armType;
                }
            });
        }

        #endregion

        #region Configure TMU Edge Arm Source

        /// <summary>
        /// Configures the TMU edge arm source for pins in the sessions bundle.
        /// Applicable when arm type is set to Edge.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUEdgeArmSource(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.EdgeArm.Source = sitePinInfo.IndividualChannelString;
                }
            });
        }

        #endregion

        #region Configure TMU Edge Arm Source Event

        /// <summary>
        /// Configures the TMU edge arm source event for pins in the sessions bundle.
        /// Applicable when arm type is set to Edge and arm source is a digital pin or channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="sourceEvent">The source event type.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUEdgeArmSourceEvent(this DigitalSessionsBundle sessionsBundle, TmuSourceEvent sourceEvent, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.EdgeArm.SourceEvent = sourceEvent;
                }
            });
        }

        #endregion

        #region Configure TMU Edge Arm Polarity

        /// <summary>
        /// Configures the TMU edge arm polarity for pins in the sessions bundle.
        /// Applicable when arm type is set to Edge.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="polarity">The edge polarity.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUEdgeArmPolarity(this DigitalSessionsBundle sessionsBundle, TmuPolarity polarity, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.EdgeArm.Polarity = polarity;
                }
            });
        }

        #endregion

        #region Configure TMU Samples To Acquire

        /// <summary>
        /// Configures the number of TMU samples to acquire for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="samplesToAcquire">The number of samples to acquire.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUSamplesToAcquire(this DigitalSessionsBundle sessionsBundle, long samplesToAcquire, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.SamplesToAcquire = samplesToAcquire;
                }
            });
        }

        #endregion

        #region Configure TMU Sample Timeout

        /// <summary>
        /// Configures the TMU sample timeout for pins in the sessions bundle.
        /// Specifies the maximum time (in seconds) the TMU will wait for both the start and stop events.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="timeoutInSeconds">The timeout in seconds (must be greater than 0).</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        public static void ConfigureTMUSampleTimeout(this DigitalSessionsBundle sessionsBundle, double timeoutInSeconds, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
                    DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
                    tmu.SampleTimeout = timeoutInSeconds;
                }
            });
        }

        #endregion

        private static void AssignTMUContexts(this DigitalSessionInformation digitalSessionInformation, string[] pins = null)
        {
            // Filter sitePinInfo based on specified pins.
            var sitePinInfos = (pins != null && pins.Any())
                ? digitalSessionInformation.AssociatedSitePinList.Where(sp => pins.Contains(sp.PinName))
                : digitalSessionInformation.AssociatedSitePinList;

            // Initialize the TMUAssignmentManager with the available TMU resources for the devices within the current session.
            List<string> availableTMUContexts = GetDigitalTmus(digitalSessionInformation.Session).GetDisabledTmuContexts();
            TMUContextManager.AddAvailableTMUs(string.Join(", ", availableTMUContexts));

            // Assign TMU resources to each target pin/site pair within the session.
            foreach (SitePinInfo sitePinInfo in sitePinInfos)
            {
                var digitalSitePinInfo = sitePinInfo as DigitalSitePinInfo;

                // Assign TMU only if it is not already assigned.
                // It may already be assigned if:
                // - AssignTMUResources() is invoked twice on the same bundle object.
                // - AssignTMUResources() is invoked after having already invoked AssignTMUResources(pinNames) on the same the bundle object for a subset of pins.
                // - AssignTMUResources(pinNames1) is invoked after having already invoked AssignTMUResources(pinNames2) on the same the bundle object,
                // where pinNames1 and pinNames2 contain overlapping pins.
                if (string.IsNullOrEmpty(digitalSitePinInfo.AssignedTmuContext))
                {
                    string deviceName = digitalSitePinInfo.InstrumentName;
                    var success = TMUContextManager.TryCheckOutTMU(deviceName, out string tmuName);
                    if (!success)
                    {
                        throw new NISemiconductorTestException(
                            string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUNotEnoughResources, deviceName, sitePinInfo.SitePinString));
                    }
                    digitalSitePinInfo.AssignedTmuContext = tmuName;
                }
            }
        }

        private static void ClearAssignedTMUContexts(this DigitalSessionInformation digitalSessionInformation, string[] pins = null)
        {
            // Filter sitePinInfo based on specified pins.
            var sitePinInfos = (pins != null && pins.Any())
                ? digitalSessionInformation.AssociatedSitePinList.Where(sp => pins.Contains(sp.PinName))
                : digitalSessionInformation.AssociatedSitePinList;

            List<string> availableTMUList = GetDigitalTmus(digitalSessionInformation.Session).GetDisabledTmuContexts();

            // Check if all the assigned TMUs of site/pin pair are safe to release.
            if (!IsSafeToReleaseAllTMUs(sitePinInfos, availableTMUList))
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUResourcesInUse));
            }

            // Clear assigned TMU and release it back to TMU resource pool.
            foreach (SitePinInfo sitePinInfo in sitePinInfos)
            {
                var digitalSitePinInfo = sitePinInfo as DigitalSitePinInfo;

                // Clear only if TMU resource is assigned for a site/pin pair.
                // This can happen when:
                // - 'ClearTMUAssignment' is invoked twice on the same bundle object.
                // - 'ClearTMUAssignment' is invoked before invoking 'AssignTMUResources'.
                // - 'ClearTMUAssignment(pinNames)' is invoked, targeting only a subset of pins within the bundle object, and then the 'ClearTMUAssignment()' is invoked on whole bundle object.
                if (!string.IsNullOrEmpty(digitalSitePinInfo.AssignedTmuContext))
                {
                    string deviceName = digitalSitePinInfo.InstrumentName;
                    string tmuName = digitalSitePinInfo.AssignedTmuContext;
                    digitalSitePinInfo.AssignedTmuContext = string.Empty;
                    TMUContextManager.TryCheckInTMU(deviceName, tmuName);
                }
            }
        }

        private static bool IsSafeToReleaseAllTMUs(IEnumerable<SitePinInfo> sitePinInfos, List<string> availableTMUList)
        {
            foreach (var sitePinInfo in sitePinInfos)
            {
                string tmuName = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;

                // Break the loop when the TMUname is not in the 'availableTMUList', TMU resource is reserved at the driver level.
                if (!string.IsNullOrEmpty(tmuName) && !availableTMUList.Contains(tmuName))
                {
                    return false;
                }
            }
            return true; // It is safe to release only when all the assigned TMUs are free, resources not reservered at driver level.
        }

        private static void ConfigurePeriodMeasurementForSitePin(DigitalSessionInformation sessionInfo, SitePinInfo sitePinInfo, TmuPolarity edgeType, long samplesToAcquire, TmuArmType armSourcetype)
        {
            string tmuContext = (sitePinInfo as DigitalSitePinInfo).AssignedTmuContext;
            TmuSourceEvent sourceEvent;
            switch (edgeType)
            {
                case TmuPolarity.RisingEdge:
                    sourceEvent = TmuSourceEvent.Voh;
                    break;
                case TmuPolarity.FallingEdge:
                    sourceEvent = TmuSourceEvent.Vol;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(edgeType), edgeType, string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUUnsupportedPolarity));
            }

            // Configure the TMU Start Source, TMU Start Source Event, TMU Start Source Event Polarity,
            // TMU Stop Source, TMU Stop Source Event, TMU Stop Source Event Polarity, number of samples to acquire, and arm source.
            DigitalTmu tmu = GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
            tmu.Start.Source = sitePinInfo.IndividualChannelString;
            tmu.Start.SourceEvent = sourceEvent;
            tmu.Start.SourceEventPolarity = edgeType;
            tmu.Stop.Source = sitePinInfo.IndividualChannelString;
            tmu.Stop.SourceEvent = sourceEvent;
            tmu.Stop.SourceEventPolarity = edgeType;
            tmu.SamplesToAcquire = samplesToAcquire;
            tmu.ArmType = armSourcetype;

            // Enable the TMU (reserve it)
            tmu.Enabled = true;
        }

        private static void ValidatePinsOfTMU(IEnumerable<string> bundlePins, string[] requestedPins)
        {
            if (requestedPins == null || requestedPins.Length == 0)
            {
                return;
            }
            var invalidPins = requestedPins.Except(bundlePins);
            if (invalidPins.Any())
            {
                throw new NISemiconductorTestException(
                    string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUPinsNotInBundle, string.Join(", ", invalidPins.Select(pin => $"\"{pin}\""))));
            }
        }

        private static void SetDigitalHighZState(DigitalSessionInformation digitalSessionInformation)
        {
            // Fix for timeout issue when fetching TMU measurement.
            // Set the pin function to Digital and Termination mode to HighZ.
            digitalSessionInformation.PinSet.SelectedFunction = SelectedFunction.Digital;
            digitalSessionInformation.PinSet.DigitalLevels.TerminationMode = TerminationMode.HighZ;
        }

        private static bool DoForThisPin(string[] pinNames, string currentPin)
        {
            return pinNames == null
                || pinNames.Length == 0
                || pinNames.Contains(currentPin);
        }

        private static DigitalTmuCollection GetDigitalTmus(NIDigital session)
        {
            return new DigitalTmuCollection(session);
        }
    }
}
