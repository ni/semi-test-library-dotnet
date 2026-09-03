using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

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
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void TMUInitiate(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    SetDigitalHighZState(sessionInfo);
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.Initiate();
                }
            });
        }

        /// <inheritdoc cref="TMUInitiate(DigitalSessionsBundle, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="pinName">The specific pin to initiate the TMU measurement on.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void TMUInitiate(this DigitalSessionsBundle sessionsBundle, string pinName)
        {
            sessionsBundle.TMUInitiate(new string[] { pinName });
        }

        /// <summary>
        /// Enables the assigned TMU resource of each pin within the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <remarks>
        /// Invoking this method creates a clear separation between TMU configuration and resource reservation.<br/>
        /// The assigned TMU resource is reserved when this method is called.<br/>
        /// Enabling a TMU with invalid configuration will result in an error when <see cref="TMUInitiate(DigitalSessionsBundle, string[])" /> is called.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pinNames">Specific pins for which to enable the assigned TMU resource. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void EnableTMU(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.Enabled = true;
                }
            });
        }

        /// <inheritdoc cref="EnableTMU(DigitalSessionsBundle, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="pinName">The specific pin for which to enable the assigned TMU resource.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void EnableTMU(this DigitalSessionsBundle sessionsBundle, string pinName)
        {
            sessionsBundle.EnableTMU(new string[] { pinName });
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
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void DisableTMU(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.Enabled = false;
                }
            });
        }

        /// <inheritdoc cref="DisableTMU(DigitalSessionsBundle, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="pinName">The specific pin for which to disable the assigned TMU resource.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void DisableTMU(this DigitalSessionsBundle sessionsBundle, string pinName)
        {
            sessionsBundle.DisableTMU(new string[] { pinName });
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
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void TMUAbort(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.Abort();
                }
            });
        }

        /// <inheritdoc cref="TMUAbort(DigitalSessionsBundle, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="pinName">The specific pin for which to abort the operation on the associated TMU resource.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void TMUAbort(this DigitalSessionsBundle sessionsBundle, string pinName)
        {
            sessionsBundle.TMUAbort(new string[] { pinName });
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
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle,
        /// or when there are not enough TMU resources available to assign to one or more of the targeted pins.
        /// </exception>
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
                    sessionInfo.ClearAssignedTMUContexts(pinNames, doTMUReleaseCheck: false);
                });

                throw; // rethrow the original exception.
            }
        }

        /// <inheritdoc cref="AssignTMUResources(DigitalSessionsBundle, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="pinName">The pin to assign a TMU resource to.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.,
        /// or when there are not enough TMU resources available to assign to the targeted pin.
        /// </exception>
        public static void AssignTMUResources(this DigitalSessionsBundle sessionsBundle, string pinName)
        {
            sessionsBundle.AssignTMUResources(new string[] { pinName });
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
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle,
        /// or when one or more of the assigned TMU resources are still reserved at the driver level.
        /// </exception>
        public static void ClearTMUAssignment(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.ClearAssignedTMUContexts(pinNames);
            });
        }

        /// <inheritdoc cref="ClearTMUAssignment(DigitalSessionsBundle, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="pinName">The pin for which the assigned TMU should be cleared.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.,
        /// or when the assigned TMU resource is still reserved at the driver level.
        /// </exception>
        public static void ClearTMUAssignment(this DigitalSessionsBundle sessionsBundle, string pinName)
        {
            sessionsBundle.ClearTMUAssignment(new string[] { pinName });
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
        /// - <see cref="DigitalTmuSource.Source"/> (Start) = the associated pin<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Start) = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Start) = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Stop) = same pin as start source<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Stop) = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Stop) = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// - <see cref="DigitalTmu.SamplesToAcquire"/> = value of <paramref name="samplesToAcquire"/> parameter.<br/>
        /// - <see cref="DigitalTmu.ArmType"/> = derived from the value of the <paramref name="armSetting"/> parameter.<br/>
        /// - <see cref="DigitalTmu.Enabled"/> = <c>true</c>
        /// </para>
        /// <para>
        /// For falling edge period (<see cref="TmuPolarity.FallingEdge"/>):<br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Start) = the associated pin<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Start) = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Start) = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Stop) = same pin as start source<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Stop) = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Stop) = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// - <see cref="DigitalTmu.SamplesToAcquire"/> = value of <paramref name="samplesToAcquire"/> parameter.<br/>
        /// - <see cref="DigitalTmu.ArmType"/> = derived from the value of the <paramref name="armSetting"/> parameter.<br/>
        /// - <see cref="DigitalTmu.Enabled"/> = <c>true</c>
        /// </para>
        /// If the <paramref name="edgeType"/> parameter is set to<see cref="TmuPolarity.EitherEdge"/>, an exception will be thrown.<br/>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="edgeType">The type of edge to detect. Only accepts <see cref="TmuPolarity.RisingEdge"/> or <see cref="TmuPolarity.FallingEdge"/>.</param>
        /// <param name="samplesToAcquire">The number of samples to acquire for the TMU measurement.</param>
        /// <param name="armSetting">
        /// The arm setting used to arm each sample of the TMU measurement.<br/>
        /// The TMU's arm input is used to frame, or select, the start and stop events of interest for each TMU sample.<br/>
        /// When <see cref="TmuArmSetting.StartEdge"/> or <see cref="TmuArmSetting.StopEdge"/> is specified,
        /// the edge arm source, event, and polarity are configured to match the corresponding start or stop source.
        /// </param>
        /// <param name="pinNames">The specific pins to configure the TMU for. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are not present in the sessions bundle,
        /// when <paramref name="armSetting"/> is an unsupported value,
        /// or when the <paramref name="edgeType"/> is <see cref="TmuPolarity.EitherEdge"/> (an unsupported polarity).
        /// </exception>
        public static void ConfigurePeriodMeasurement(this DigitalSessionsBundle sessionsBundle, TmuPolarity edgeType, long samplesToAcquire, TmuArmSetting armSetting = TmuArmSetting.Immediate, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            ValidateTmuArmSetting(armSetting);
            TmuSourceEvent sourceEvent = ValidateAndGetSourceEventForEdge(edgeType);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    string channel = sitePinInfo.IndividualChannelString;
                    ConfigureAndEnableTmu(
                        tmu: tmu,
                        startSource: channel,
                        startEvent: sourceEvent,
                        startPolarity: edgeType,
                        stopSource: channel,
                        stopEvent: sourceEvent,
                        stopPolarity: edgeType,
                        samplesToAcquire: samplesToAcquire,
                        armSetting: armSetting);
                }
            });
        }

        /// <inheritdoc cref="ConfigurePeriodMeasurement(DigitalSessionsBundle, TmuPolarity, long, TmuArmSetting, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="edgeType"/>
        /// <param name="samplesToAcquire"/>
        /// <param name="pinName">The specific pin to configure the TMU for.</param>
        /// <param name="armSetting"/>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.,
        /// when <paramref name="armSetting"/> is an unsupported value,
        /// or when the <paramref name="edgeType"/> is <see cref="TmuPolarity.EitherEdge"/> (an unsupported polarity).
        /// </exception>
        public static void ConfigurePeriodMeasurement(this DigitalSessionsBundle sessionsBundle, TmuPolarity edgeType, long samplesToAcquire, string pinName, TmuArmSetting armSetting = TmuArmSetting.Immediate)
        {
            sessionsBundle.ConfigurePeriodMeasurement(edgeType, samplesToAcquire, armSetting, new string[] { pinName });
        }

        /// <summary>
        /// Configures the TMU for skew measurement between a reference pin and a target pin.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Skew is defined as the time difference between a specific edge type on the reference channel
        /// and the same edge type on the target channel.<br/>
        /// A positive result indicates the target edge occurs after the reference edge.<br/>
        /// A negative result indicates the target edge occurs before the reference edge.
        /// </para>
        /// <para>
        /// This method sets the necessary attributes to configure skew measurement for the assigned TMU resource.<br/>
        /// </para>
        /// <para>
        /// For rising edge skew (<see cref="TmuPolarity.RisingEdge"/>):<br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Start) = Reference channel<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Start) = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Start) = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Stop) = Target channel<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Stop) = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Stop) = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// - <see cref="DigitalTmu.SamplesToAcquire"/> = value of <paramref name="samplesToAcquire"/> parameter.<br/>
        /// - <see cref="DigitalTmu.ArmType"/> = derived from the value of the <paramref name="armSetting"/> parameter.<br/>
        /// - <see cref="DigitalTmu.Enabled"/> = <c>true</c>
        /// </para>
        /// <para>
        /// For falling edge skew (<see cref="TmuPolarity.FallingEdge"/>):<br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Start) = Reference channel<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Start) = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Start) = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Stop) = Target channel<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Stop) = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Stop) = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// - <see cref="DigitalTmu.SamplesToAcquire"/> = value of <paramref name="samplesToAcquire"/> parameter.<br/>
        /// - <see cref="DigitalTmu.ArmType"/> = derived from the value of the <paramref name="armSetting"/> parameter.<br/>
        /// - <see cref="DigitalTmu.Enabled"/> = <c>true</c>
        /// </para>
        /// If the <paramref name="edgeType"/> parameter is set to <see cref="TmuPolarity.EitherEdge"/>, an exception will be thrown.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="referencePinNames">The pins to use as the reference (start) source for the skew measurement.</param>
        /// <param name="targetPinNames">The pins to use as the target (stop) source for the skew measurement.</param>
        /// <param name="edgeType">The type of edge to detect. Only accepts <see cref="TmuPolarity.RisingEdge"/> or <see cref="TmuPolarity.FallingEdge"/>.</param>
        /// <param name="samplesToAcquire">The number of samples to acquire for the TMU measurement.</param>
        /// <param name="armSetting">
        /// The arm setting used to arm each sample of the TMU measurement.<br/>
        /// The TMU's arm input is used to frame, or select, the start and stop events of interest for each TMU sample.<br/>
        /// When <see cref="TmuArmSetting.StartEdge"/> or <see cref="TmuArmSetting.StopEdge"/> is specified,
        /// the edge arm source, event, and polarity are configured to match the corresponding start or stop source.
        /// </param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when <paramref name="referencePinNames"/> or <paramref name="targetPinNames"/> is <c>null</c> or empty,
        /// when the two pin arrays have different lengths, when a reference pin is also used as a target pin,
        /// when one or more of the requested pins are not present in the sessions bundle,
        /// when <paramref name="armSetting"/> is an unsupported value, when the <paramref name="edgeType"/> is
        /// <see cref="TmuPolarity.EitherEdge"/> (an unsupported polarity), or when a target pin cannot be found on the same site as its reference pin.
        /// </exception>
        public static void ConfigureTMUSkewMeasurement(
            this DigitalSessionsBundle sessionsBundle,
            string[] referencePinNames,
            string[] targetPinNames,
            TmuPolarity edgeType,
            long samplesToAcquire,
            TmuArmSetting armSetting = TmuArmSetting.Immediate)
        {
            ValidateSkewParameters(referencePinNames, targetPinNames, armSetting, sessionsBundle.Pins);
            TmuSourceEvent sourceEvent = ValidateAndGetSourceEventForEdge(edgeType);

            // Create a mapping from reference pin to target pin
            var referenceToTargetMap = new Dictionary<string, string>();
            for (int i = 0; i < referencePinNames.Length; i++)
            {
                referenceToTargetMap[referencePinNames[i]] = targetPinNames[i];
            }

            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                // Configure only for reference pins (which have the TMU assigned)
                if (referenceToTargetMap.TryGetValue(sitePinInfo.PinName, out string targetPinName))
                {
                    // Find the target pin's sitePinInfo in the same site.
                    var targetSitePinInfo = sessionInfo.AssociatedSitePinList
                        .FirstOrDefault(sp => sp.PinName == targetPinName && sp.SiteNumber == sitePinInfo.SiteNumber);
                    if (targetSitePinInfo == null)
                    {
                        throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUSkewTargetPinNotFound, targetPinName, sitePinInfo.SiteNumber));
                    }
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    ConfigureAndEnableTmu(
                        tmu: tmu,
                        startSource: sitePinInfo.IndividualChannelString,
                        startEvent: sourceEvent,
                        startPolarity: edgeType,
                        stopSource: targetSitePinInfo.IndividualChannelString,
                        stopEvent: sourceEvent,
                        stopPolarity: edgeType,
                        samplesToAcquire: samplesToAcquire,
                        armSetting: armSetting);
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUSkewMeasurement(DigitalSessionsBundle, string[], string[], TmuPolarity, long, TmuArmSetting)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="referencePinName">The pin to use as the reference (start) source for the skew measurement.</param>
        /// <param name="targetPinName">The pin to use as the target (stop) source for the skew measurement.</param>
        /// <param name="edgeType"/>
        /// <param name="samplesToAcquire"/>
        /// <param name="armSetting"/>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when <paramref name="referencePinName"/> or <paramref name="targetPinName"/> is <c>null</c> or empty,
        /// when the reference pin is also used as the target pin,
        /// when one or more of the requested pins are not present in the sessions bundle,
        /// when <paramref name="armSetting"/> is an unsupported value, when the <paramref name="edgeType"/> is
        /// <see cref="TmuPolarity.EitherEdge"/> (an unsupported polarity), or when the target pin cannot be found on the same site as its reference pin.
        /// </exception>
        public static void ConfigureTMUSkewMeasurement(
            this DigitalSessionsBundle sessionsBundle,
            string referencePinName,
            string targetPinName,
            TmuPolarity edgeType,
            long samplesToAcquire,
            TmuArmSetting armSetting = TmuArmSetting.Immediate)
        {
            sessionsBundle.ConfigureTMUSkewMeasurement(new string[] { referencePinName }, new string[] { targetPinName }, edgeType, samplesToAcquire, armSetting);
        }

        /// <summary>
        /// Configures the TMU to perform a rise time measurement for pins in the sessions bundle.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Rise time is defined as the time for a signal to transition from the low voltage threshold to the high voltage threshold.<br/>
        /// This method sets the following attributes for the assigned TMU resource:
        /// </para>
        /// <para>
        /// - <see cref="DigitalTmuSource.Source"/> (Start) = the associated pin<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Start) = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Start) = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Stop) = same pin as start source<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Stop) = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Stop) = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// - <see cref="DigitalTmu.SamplesToAcquire"/> = value of <paramref name="samplesToAcquire"/> parameter.<br/>
        /// - <see cref="DigitalTmu.ArmType"/> = derived from the value of the <paramref name="armSetting"/> parameter.<br/>
        /// - <see cref="DigitalTmu.Enabled"/> = <c>true</c>
        /// </para>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="samplesToAcquire">The number of samples to acquire for the TMU measurement.</param>
        /// <param name="armSetting">
        /// The arm setting used to arm each sample of the TMU measurement.<br/>
        /// The TMU's arm input is used to frame, or select, the start and stop events of interest for each TMU sample.<br/>
        /// When <see cref="TmuArmSetting.StartEdge"/> or <see cref="TmuArmSetting.StopEdge"/> is specified,
        /// the edge arm source, event, and polarity are configured to match the corresponding start or stop source.
        /// </param>
        /// <param name="pinNames">The specific pins to configure the TMU for. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are not present in the sessions bundle,
        /// or when <paramref name="armSetting"/> is an unsupported value.
        /// </exception>
        public static void ConfigureTMURiseTimeMeasurement(this DigitalSessionsBundle sessionsBundle, long samplesToAcquire, TmuArmSetting armSetting = TmuArmSetting.Immediate, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            ValidateTmuArmSetting(armSetting);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    string channel = sitePinInfo.IndividualChannelString;
                    ConfigureAndEnableTmu(
                        tmu: tmu,
                        startSource: channel,
                        startEvent: TmuSourceEvent.Vol,
                        startPolarity: TmuPolarity.RisingEdge,
                        stopSource: channel,
                        stopEvent: TmuSourceEvent.Voh,
                        stopPolarity: TmuPolarity.RisingEdge,
                        samplesToAcquire: samplesToAcquire,
                        armSetting: armSetting);
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMURiseTimeMeasurement(DigitalSessionsBundle, long, TmuArmSetting, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="samplesToAcquire"/>
        /// <param name="pinName">The specific pin to configure the TMU for.</param>
        /// <param name="armSetting"/>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.,
        /// or when <paramref name="armSetting"/> is an unsupported value.
        /// </exception>
        public static void ConfigureTMURiseTimeMeasurement(this DigitalSessionsBundle sessionsBundle, long samplesToAcquire, string pinName, TmuArmSetting armSetting = TmuArmSetting.Immediate)
        {
            sessionsBundle.ConfigureTMURiseTimeMeasurement(samplesToAcquire, armSetting, new string[] { pinName });
        }

        /// <summary>
        /// Configures the TMU to perform a fall time measurement for pins in the sessions bundle.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Fall time is defined as the time for a signal to transition from the high voltage threshold to the low voltage threshold.<br/>
        /// This method sets the following attributes for the assigned TMU resource:
        /// </para>
        /// <para>
        /// - <see cref="DigitalTmuSource.Source"/> (Start) = the associated pin<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Start) = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Start) = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Stop) = same pin as start source<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Stop) = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Stop) = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// - <see cref="DigitalTmu.SamplesToAcquire"/> = value of <paramref name="samplesToAcquire"/> parameter.<br/>
        /// - <see cref="DigitalTmu.ArmType"/> = derived from the value of the <paramref name="armSetting"/> parameter.<br/>
        /// - <see cref="DigitalTmu.Enabled"/> = <c>true</c>
        /// </para>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="samplesToAcquire">The number of samples to acquire for the TMU measurement.</param>
        /// <param name="armSetting">
        /// The arm setting used to arm each sample of the TMU measurement.<br/>
        /// The TMU's arm input is used to frame, or select, the start and stop events of interest for each TMU sample.<br/>
        /// When <see cref="TmuArmSetting.StartEdge"/> or <see cref="TmuArmSetting.StopEdge"/> is specified,
        /// the edge arm source, event, and polarity are configured to match the corresponding start or stop source.
        /// </param>
        /// <param name="pinNames">The specific pins to configure the TMU for. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are not present in the sessions bundle,
        /// or when <paramref name="armSetting"/> is an unsupported value.
        /// </exception>
        public static void ConfigureTMUFallTimeMeasurement(this DigitalSessionsBundle sessionsBundle, long samplesToAcquire, TmuArmSetting armSetting = TmuArmSetting.Immediate, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            ValidateTmuArmSetting(armSetting);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    string channel = sitePinInfo.IndividualChannelString;
                    ConfigureAndEnableTmu(
                        tmu: tmu,
                        startSource: channel,
                        startEvent: TmuSourceEvent.Voh,
                        startPolarity: TmuPolarity.FallingEdge,
                        stopSource: channel,
                        stopEvent: TmuSourceEvent.Vol,
                        stopPolarity: TmuPolarity.FallingEdge,
                        samplesToAcquire: samplesToAcquire,
                        armSetting: armSetting);
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUFallTimeMeasurement(DigitalSessionsBundle, long, TmuArmSetting, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="samplesToAcquire"/>
        /// <param name="pinName">The specific pin to configure the TMU for.</param>
        /// <param name="armSetting"/>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.,
        /// or when <paramref name="armSetting"/> is an unsupported value.
        /// </exception>
        public static void ConfigureTMUFallTimeMeasurement(this DigitalSessionsBundle sessionsBundle, long samplesToAcquire, string pinName, TmuArmSetting armSetting = TmuArmSetting.Immediate)
        {
            sessionsBundle.ConfigureTMUFallTimeMeasurement(samplesToAcquire, armSetting, new string[] { pinName });
        }

        /// <summary>
        /// Configures the TMU to perform a duty cycle measurement for pins in the sessions bundle.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method sets the following attributes for the assigned TMU resource based on the <paramref name="dutyCycleType"/> parameter:
        /// </para>
        /// <para>
        /// For duty cycle high (<see cref="TmuDutyCycle.High"/>):<br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Start) = the associated pin<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Start) = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Start) = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Stop) = same pin as start source<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Stop) = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Stop) = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// </para>
        /// <para>
        /// For duty cycle low (<see cref="TmuDutyCycle.Low"/>):<br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Start) = the associated pin<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Start) = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Start) = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Stop) = same pin as start source<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Stop) = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Stop) = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// </para>
        /// <para>
        /// - <see cref="DigitalTmu.SamplesToAcquire"/> = value of <paramref name="samplesToAcquire"/> parameter.<br/>
        /// - <see cref="DigitalTmu.ArmType"/> = derived from the value of the <paramref name="armSetting"/> parameter.<br/>
        /// - <see cref="DigitalTmu.Enabled"/> = <c>true</c>
        /// </para>
        /// <para>
        /// TMU samples are signed time intervals, so the measurement result can be negative.<br/>
        /// With <see cref="TmuArmSetting.Immediate"/>, the TMU looks for the start and stop events as soon as the
        /// measurement is initiated, so on a free-running signal the stop event can be detected before the start event.<br/>
        /// Where a positive time interval is desired, use <see cref="TmuArmSetting.StartEdge"/> for the
        /// <paramref name="armSetting"/> parameter to establish the event ordering.
        /// </para>
        /// <para>
        /// The value returned by <see cref="FetchAveragedTMUMeasurement(DigitalSessionsBundle, double, string[])"/> is the measured
        /// time duration, in seconds, and not a percentage.<br/>
        /// To express the result as a percentage duty cycle, divide it by the period of the signal.
        /// </para>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="dutyCycleType">The duty cycle measurement type. Accepts <see cref="TmuDutyCycle.High"/> or <see cref="TmuDutyCycle.Low"/>.</param>
        /// <param name="samplesToAcquire">The number of samples to acquire for the TMU measurement.</param>
        /// <param name="armSetting">
        /// The arm setting used to arm each sample of the TMU measurement.<br/>
        /// The TMU's arm input is used to frame, or select, the start and stop events of interest for each TMU sample.<br/>
        /// When <see cref="TmuArmSetting.StartEdge"/> or <see cref="TmuArmSetting.StopEdge"/> is specified,
        /// the edge arm source, event, and polarity are configured to match the corresponding start or stop source.
        /// </param>
        /// <param name="pinNames">The specific pins to configure the TMU for. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">Thrown when one or more of the requested <paramref name="pinNames"/> are not present in the sessions bundle, when <paramref name="armSetting"/> is an unsupported value, or when <paramref name="dutyCycleType"/> is not <see cref="TmuDutyCycle.High"/> or <see cref="TmuDutyCycle.Low"/>.
        /// </exception>
        public static void ConfigureTMUDutyCycleMeasurement(this DigitalSessionsBundle sessionsBundle, TmuDutyCycle dutyCycleType, long samplesToAcquire, TmuArmSetting armSetting = TmuArmSetting.Immediate, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            ValidateTmuArmSetting(armSetting);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    string channel = sitePinInfo.IndividualChannelString;
                    switch (dutyCycleType)
                    {
                        case TmuDutyCycle.High:
                            ConfigureAndEnableTmu(
                                tmu: tmu,
                                startSource: channel,
                                startEvent: TmuSourceEvent.Voh,
                                startPolarity: TmuPolarity.RisingEdge,
                                stopSource: channel,
                                stopEvent: TmuSourceEvent.Voh,
                                stopPolarity: TmuPolarity.FallingEdge,
                                samplesToAcquire: samplesToAcquire,
                                armSetting: armSetting);
                            break;
                        case TmuDutyCycle.Low:
                            ConfigureAndEnableTmu(
                                tmu: tmu,
                                startSource: channel,
                                startEvent: TmuSourceEvent.Vol,
                                startPolarity: TmuPolarity.FallingEdge,
                                stopSource: channel,
                                stopEvent: TmuSourceEvent.Vol,
                                stopPolarity: TmuPolarity.RisingEdge,
                                samplesToAcquire: samplesToAcquire,
                                armSetting: armSetting);
                            break;
                        default:
                            throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUUnsupportedDuty, dutyCycleType.ToString()));
                    }
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUDutyCycleMeasurement(DigitalSessionsBundle, TmuDutyCycle, long, TmuArmSetting, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="dutyCycleType"/>
        /// <param name="samplesToAcquire"/>
        /// <param name="pinName">The specific pin to configure the TMU for.</param>
        /// <param name="armSetting"/>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.,
        /// when <paramref name="armSetting"/> is an unsupported value,
        /// or when <paramref name="dutyCycleType"/> is not <see cref="TmuDutyCycle.High"/> or <see cref="TmuDutyCycle.Low"/>.
        /// </exception>
        public static void ConfigureTMUDutyCycleMeasurement(this DigitalSessionsBundle sessionsBundle, TmuDutyCycle dutyCycleType, long samplesToAcquire, string pinName, TmuArmSetting armSetting = TmuArmSetting.Immediate)
        {
            sessionsBundle.ConfigureTMUDutyCycleMeasurement(dutyCycleType, samplesToAcquire, armSetting, new string[] { pinName });
        }

        /// <summary>
        /// Configures the TMU to perform a pulse width measurement for pins in the sessions bundle.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method sets the following attributes for the assigned TMU resource based on the <paramref name="pulseWidthType"/> parameter:
        /// </para>
        /// <para>
        /// For pulse width high (<see cref="TmuPulseWidth.High"/>):<br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Start) = the associated pin<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Start) = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Start) = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Stop) = same pin as start source<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Stop) = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Stop) = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// </para>
        /// <para>
        /// For pulse width low (<see cref="TmuPulseWidth.Low"/>):<br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Start) = the associated pin<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Start) = <see cref="TmuSourceEvent.Vol"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Start) = <see cref="TmuPolarity.FallingEdge"/><br/>
        /// - <see cref="DigitalTmuSource.Source"/> (Stop) = same pin as start source<br/>
        /// - <see cref="DigitalTmuSource.SourceEvent"/> (Stop) = <see cref="TmuSourceEvent.Voh"/><br/>
        /// - <see cref="DigitalTmuSource.SourceEventPolarity"/> (Stop) = <see cref="TmuPolarity.RisingEdge"/><br/>
        /// </para>
        /// <para>
        /// - <see cref="DigitalTmu.SamplesToAcquire"/> = value of <paramref name="samplesToAcquire"/> parameter.<br/>
        /// - <see cref="DigitalTmu.ArmType"/> = derived from the value of the <paramref name="armSetting"/> parameter.<br/>
        /// - <see cref="DigitalTmu.Enabled"/> = <c>true</c>
        /// </para>
        /// <para>
        /// TMU samples are signed time intervals, so the measurement result can be negative.<br/>
        /// With <see cref="TmuArmSetting.Immediate"/>, the TMU looks for the start and stop events as soon as the
        /// measurement is initiated, so on a free-running signal the stop event can be detected before the start event.<br/>
        /// Where a positive time interval is desired, use <see cref="TmuArmSetting.StartEdge"/> for the
        /// <paramref name="armSetting"/> parameter to establish the event ordering.
        /// </para>
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/> object.</param>
        /// <param name="pulseWidthType">The pulse width measurement type. Accepts <see cref="TmuPulseWidth.High"/> or <see cref="TmuPulseWidth.Low"/>.</param>
        /// <param name="samplesToAcquire">The number of samples to acquire for the TMU measurement.</param>
        /// <param name="armSetting">
        /// The arm setting used to arm each sample of the TMU measurement.<br/>
        /// The TMU's arm input is used to frame, or select, the start and stop events of interest for each TMU sample.<br/>
        /// When <see cref="TmuArmSetting.StartEdge"/> or <see cref="TmuArmSetting.StopEdge"/> is specified,
        /// the edge arm source, event, and polarity are configured to match the corresponding start or stop source.
        /// </param>
        /// <param name="pinNames">The specific pins to configure the TMU for. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">Thrown when one or more of the requested <paramref name="pinNames"/> are not present in the sessions bundle, when <paramref name="armSetting"/> is an unsupported value, or when <paramref name="pulseWidthType"/> is not <see cref="TmuPulseWidth.High"/> or <see cref="TmuPulseWidth.Low"/>.
        /// </exception>
        public static void ConfigureTMUPulseWidthMeasurement(this DigitalSessionsBundle sessionsBundle, TmuPulseWidth pulseWidthType, long samplesToAcquire, TmuArmSetting armSetting = TmuArmSetting.Immediate, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            ValidateTmuArmSetting(armSetting);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    string channel = sitePinInfo.IndividualChannelString;
                    switch (pulseWidthType)
                    {
                        case TmuPulseWidth.High:
                            ConfigureAndEnableTmu(
                                tmu: tmu,
                                startSource: channel,
                                startEvent: TmuSourceEvent.Voh,
                                startPolarity: TmuPolarity.RisingEdge,
                                stopSource: channel,
                                stopEvent: TmuSourceEvent.Vol,
                                stopPolarity: TmuPolarity.FallingEdge,
                                samplesToAcquire: samplesToAcquire,
                                armSetting: armSetting);
                            break;
                        case TmuPulseWidth.Low:
                            ConfigureAndEnableTmu(
                                tmu: tmu,
                                startSource: channel,
                                startEvent: TmuSourceEvent.Vol,
                                startPolarity: TmuPolarity.FallingEdge,
                                stopSource: channel,
                                stopEvent: TmuSourceEvent.Voh,
                                stopPolarity: TmuPolarity.RisingEdge,
                                samplesToAcquire: samplesToAcquire,
                                armSetting: armSetting);
                            break;
                        default:
                            throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUUnsupportedPulseWidth, pulseWidthType.ToString()));
                    }
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUPulseWidthMeasurement(DigitalSessionsBundle, TmuPulseWidth, long, TmuArmSetting, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="pulseWidthType"/>
        /// <param name="samplesToAcquire"/>
        /// <param name="pinName">The specific pin to configure the TMU for.</param>
        /// <param name="armSetting"/>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.,
        /// when <paramref name="armSetting"/> is an unsupported value,
        /// or when <paramref name="pulseWidthType"/> is not <see cref="TmuPulseWidth.High"/> or <see cref="TmuPulseWidth.Low"/>.
        /// </exception>
        public static void ConfigureTMUPulseWidthMeasurement(this DigitalSessionsBundle sessionsBundle, TmuPulseWidth pulseWidthType, long samplesToAcquire, string pinName, TmuArmSetting armSetting = TmuArmSetting.Immediate)
        {
            sessionsBundle.ConfigureTMUPulseWidthMeasurement(pulseWidthType, samplesToAcquire, armSetting, new string[] { pinName });
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
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static PinSiteData<double> FetchAveragedTMUMeasurement(this DigitalSessionsBundle sessionsBundle, double timeoutInSeconds = 5, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    return tmu.FetchAveragedMeasurement(timeoutInSeconds);
                }
                return double.NaN;
            });
        }

        /// <inheritdoc cref="FetchAveragedTMUMeasurement(DigitalSessionsBundle, double, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="pinName">The specific pin to fetch the TMU measurement for.</param>
        /// <param name="timeoutInSeconds"/>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static PinSiteData<double> FetchAveragedTMUMeasurement(this DigitalSessionsBundle sessionsBundle, string pinName, double timeoutInSeconds = 5)
        {
            return sessionsBundle.FetchAveragedTMUMeasurement(timeoutInSeconds, new string[] { pinName });
        }

        #region Configure TMU Start Source

        /// <summary>
        /// Configures the TMU start source for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStartSource(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.Start.Source = sitePinInfo.IndividualChannelString;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUStartSource(DigitalSessionsBundle, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStartSource(this DigitalSessionsBundle sessionsBundle, string pinName)
        {
            sessionsBundle.ConfigureTMUStartSource(new string[] { pinName });
        }

        #endregion

        #region Configure TMU Stop Source

        /// <summary>
        /// Configures the TMU stop source for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStopSource(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.Stop.Source = sitePinInfo.IndividualChannelString;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUStopSource(DigitalSessionsBundle, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStopSource(this DigitalSessionsBundle sessionsBundle, string pinName)
        {
            sessionsBundle.ConfigureTMUStopSource(new string[] { pinName });
        }

        #endregion

        #region Configure TMU Start Source Event

        /// <summary>
        /// Configures the TMU start source event for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="sourceEvent">The source event type.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStartSourceEvent(this DigitalSessionsBundle sessionsBundle, TmuSourceEvent sourceEvent, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.Start.SourceEvent = sourceEvent;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUStartSourceEvent(DigitalSessionsBundle, TmuSourceEvent, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="sourceEvent"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStartSourceEvent(this DigitalSessionsBundle sessionsBundle, TmuSourceEvent sourceEvent, string pinName)
        {
            sessionsBundle.ConfigureTMUStartSourceEvent(sourceEvent, new string[] { pinName });
        }

        #endregion

        #region Configure TMU Stop Source Event

        /// <summary>
        /// Configures the TMU stop source event for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="sourceEvent">The source event type.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStopSourceEvent(this DigitalSessionsBundle sessionsBundle, TmuSourceEvent sourceEvent, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.Stop.SourceEvent = sourceEvent;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUStopSourceEvent(DigitalSessionsBundle, TmuSourceEvent, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="sourceEvent"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStopSourceEvent(this DigitalSessionsBundle sessionsBundle, TmuSourceEvent sourceEvent, string pinName)
        {
            sessionsBundle.ConfigureTMUStopSourceEvent(sourceEvent, new string[] { pinName });
        }

        #endregion

        #region Configure TMU Start Source Event Polarity

        /// <summary>
        /// Configures the TMU start source event polarity for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="polarity">The source event polarity.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStartSourceEventPolarity(this DigitalSessionsBundle sessionsBundle, TmuPolarity polarity, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.Start.SourceEventPolarity = polarity;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUStartSourceEventPolarity(DigitalSessionsBundle, TmuPolarity, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="polarity"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStartSourceEventPolarity(this DigitalSessionsBundle sessionsBundle, TmuPolarity polarity, string pinName)
        {
            sessionsBundle.ConfigureTMUStartSourceEventPolarity(polarity, new string[] { pinName });
        }

        #endregion

        #region Configure TMU Stop Source Event Polarity

        /// <summary>
        /// Configures the TMU stop source event polarity for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="polarity">The edge polarity.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStopSourceEventPolarity(this DigitalSessionsBundle sessionsBundle, TmuPolarity polarity, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.Stop.SourceEventPolarity = polarity;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUStopSourceEventPolarity(DigitalSessionsBundle, TmuPolarity, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="polarity"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUStopSourceEventPolarity(this DigitalSessionsBundle sessionsBundle, TmuPolarity polarity, string pinName)
        {
            sessionsBundle.ConfigureTMUStopSourceEventPolarity(polarity, new string[] { pinName });
        }

        #endregion

        #region Configure TMU Arm Type

        /// <summary>
        /// Configures the TMU arm type for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="armType">The arm type.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are not present in the sessions bundle,
        /// or when <paramref name="armType"/> is an unsupported value.
        /// </exception>
        public static void ConfigureTMUArmType(this DigitalSessionsBundle sessionsBundle, TmuArmType armType, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            ValidateTmuArmType(armType);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.ArmType = armType;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUArmType(DigitalSessionsBundle, TmuArmType, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="armType"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.,
        /// or when <paramref name="armType"/> is an unsupported value.
        /// </exception>
        public static void ConfigureTMUArmType(this DigitalSessionsBundle sessionsBundle, TmuArmType armType, string pinName)
        {
            sessionsBundle.ConfigureTMUArmType(armType, new string[] { pinName });
        }

        #endregion

        #region Configure TMU Edge Arm Source

        /// <summary>
        /// Configures the TMU edge arm source for pins in the sessions bundle.
        /// Applicable when arm type is set to Edge.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUEdgeArmSource(this DigitalSessionsBundle sessionsBundle, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.EdgeArm.Source = sitePinInfo.IndividualChannelString;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUEdgeArmSource(DigitalSessionsBundle, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUEdgeArmSource(this DigitalSessionsBundle sessionsBundle, string pinName)
        {
            sessionsBundle.ConfigureTMUEdgeArmSource(new string[] { pinName });
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
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUEdgeArmSourceEvent(this DigitalSessionsBundle sessionsBundle, TmuSourceEvent sourceEvent, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.EdgeArm.SourceEvent = sourceEvent;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUEdgeArmSourceEvent(DigitalSessionsBundle, TmuSourceEvent, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="sourceEvent"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUEdgeArmSourceEvent(this DigitalSessionsBundle sessionsBundle, TmuSourceEvent sourceEvent, string pinName)
        {
            sessionsBundle.ConfigureTMUEdgeArmSourceEvent(sourceEvent, new string[] { pinName });
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
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUEdgeArmPolarity(this DigitalSessionsBundle sessionsBundle, TmuPolarity polarity, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.EdgeArm.Polarity = polarity;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUEdgeArmPolarity(DigitalSessionsBundle, TmuPolarity, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="polarity"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUEdgeArmPolarity(this DigitalSessionsBundle sessionsBundle, TmuPolarity polarity, string pinName)
        {
            sessionsBundle.ConfigureTMUEdgeArmPolarity(polarity, new string[] { pinName });
        }

        #endregion

        #region Configure TMU Samples To Acquire

        /// <summary>
        /// Configures the number of TMU samples to acquire for pins in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <param name="samplesToAcquire">The number of samples to acquire.</param>
        /// <param name="pinNames">The pin names to configure. When <c>null</c>, all pins are targeted.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUSamplesToAcquire(this DigitalSessionsBundle sessionsBundle, long samplesToAcquire, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.SamplesToAcquire = samplesToAcquire;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUSamplesToAcquire(DigitalSessionsBundle, long, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="samplesToAcquire"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUSamplesToAcquire(this DigitalSessionsBundle sessionsBundle, long samplesToAcquire, string pinName)
        {
            sessionsBundle.ConfigureTMUSamplesToAcquire(samplesToAcquire, new string[] { pinName });
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
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when one or more of the requested <paramref name="pinNames"/> are <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUSampleTimeout(this DigitalSessionsBundle sessionsBundle, double timeoutInSeconds, string[] pinNames = null)
        {
            ValidatePinsOfTMU(sessionsBundle.Pins, pinNames);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (DoForThisPin(pinNames, sitePinInfo.PinName))
                {
                    DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                    tmu.SampleTimeout = timeoutInSeconds;
                }
            });
        }

        /// <inheritdoc cref="ConfigureTMUSampleTimeout(DigitalSessionsBundle, double, string[])"/>
        /// <param name="sessionsBundle"/>
        /// <param name="timeoutInSeconds"/>
        /// <param name="pinName">The pin name to configure.</param>
        /// <exception cref="NISemiconductorTestException">
        /// Thrown when the requested <paramref name="pinName"/> is <c>null</c>, empty, or not present in the sessions bundle.
        /// </exception>
        public static void ConfigureTMUSampleTimeout(this DigitalSessionsBundle sessionsBundle, double timeoutInSeconds, string pinName)
        {
            sessionsBundle.ConfigureTMUSampleTimeout(timeoutInSeconds, new string[] { pinName });
        }

        #endregion

        #region Get TMU Start Source

        /// <summary>
        /// Gets the TMU start source channel string for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The start source channel string for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<string> GetTMUStartSource(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.Start.Source;
            });
        }

        #endregion

        #region Get TMU Stop Source

        /// <summary>
        /// Gets the TMU stop source channel string for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The stop source channel string for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<string> GetTMUStopSource(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.Stop.Source;
            });
        }

        #endregion

        #region Get TMU Start Source Event

        /// <summary>
        /// Gets the TMU start source event for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The start source event for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<TmuSourceEvent> GetTMUStartSourceEvent(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.Start.SourceEvent;
            });
        }

        #endregion

        #region Get TMU Stop Source Event

        /// <summary>
        /// Gets the TMU stop source event for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The stop source event for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<TmuSourceEvent> GetTMUStopSourceEvent(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.Stop.SourceEvent;
            });
        }

        #endregion

        #region Get TMU Start Source Event Polarity

        /// <summary>
        /// Gets the TMU start source event polarity for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The start source event polarity for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<TmuPolarity> GetTMUStartSourceEventPolarity(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.Start.SourceEventPolarity;
            });
        }

        #endregion

        #region Get TMU Stop Source Event Polarity

        /// <summary>
        /// Gets the TMU stop source event polarity for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The stop source event polarity for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<TmuPolarity> GetTMUStopSourceEventPolarity(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.Stop.SourceEventPolarity;
            });
        }

        #endregion

        #region Get TMU Enabled

        /// <summary>
        /// Gets a value indicating whether the assigned TMU resource is enabled for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>A value indicating whether the TMU is enabled for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<bool> GetTMUEnabled(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.Enabled;
            });
        }

        #endregion

        #region Get TMU Arm Type

        /// <summary>
        /// Gets the TMU arm type for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The arm type for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<TmuArmType> GetTMUArmType(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.ArmType;
            });
        }

        #endregion

        #region Get TMU Edge Arm Source

        /// <summary>
        /// Gets the TMU edge arm source channel string for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// Applicable when arm type is set to <see cref="TmuArmType.Edge"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The edge arm source channel string for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<string> GetTMUEdgeArmSource(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.EdgeArm.Source;
            });
        }

        #endregion

        #region Get TMU Edge Arm Source Event

        /// <summary>
        /// Gets the TMU edge arm source event for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// Applicable when arm type is set to <see cref="TmuArmType.Edge"/> and arm source is a digital pin or channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The edge arm source event for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<TmuSourceEvent> GetTMUEdgeArmSourceEvent(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.EdgeArm.SourceEvent;
            });
        }

        #endregion

        #region Get TMU Edge Arm Polarity

        /// <summary>
        /// Gets the TMU edge arm polarity for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// Applicable when arm type is set to <see cref="TmuArmType.Edge"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The edge arm polarity for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<TmuPolarity> GetTMUEdgeArmPolarity(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.EdgeArm.Polarity;
            });
        }

        #endregion

        #region Get TMU Samples To Acquire

        /// <summary>
        /// Gets the number of TMU samples to acquire for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The number of samples to acquire for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<long> GetTMUSamplesToAcquire(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.SamplesToAcquire;
            });
        }

        #endregion

        #region Get TMU Sample Timeout

        /// <summary>
        /// Gets the TMU sample timeout for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The sample timeout in seconds for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<double> GetTMUSampleTimeout(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.SampleTimeout;
            });
        }

        #endregion

        #region Get TMU Start Input Debounce Time

        /// <summary>
        /// Gets the TMU start input debounce time for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The start input debounce time in seconds for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<double> GetTMUStartInputDebounceTime(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.Start.InputDebounceTime;
            });
        }

        #endregion

        #region Get TMU Stop Input Debounce Time

        /// <summary>
        /// Gets the TMU stop input debounce time for each pin in the <see cref="DigitalSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DigitalSessionsBundle"/>.</param>
        /// <returns>The stop input debounce time in seconds for each pin and site as <see cref="PinSiteData{T}"/>.</returns>
        /// <exception cref="NISemiconductorTestException">Thrown when a TMU resource has not been assigned to one or more pins. Call <see cref="AssignTMUResources(DigitalSessionsBundle, string[])"/> before invoking this method.</exception>
        public static PinSiteData<double> GetTMUStopInputDebounceTime(this DigitalSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                DigitalTmu tmu = GetAssignedTmu(sessionInfo, sitePinInfo);
                return tmu.Stop.InputDebounceTime;
            });
        }

        #endregion

        // #region Get TMU Count

        // NOTE (POC): GetTMUCount() is temporarily disabled. The driver-native DigitalTmuCollection
        // (NIDigital.Tmu) only exposes GetDisabledTmuContexts() and GetTmu(string) -- there is no
        // native API to query the total number of TMU resources available per instrument session.
        // This needs a follow-up decision on how (or whether) to support this API going forward.

        // /// <summary>
        // /// Gets the total number of TMU resources available for each instrument session in the<see cref = "DigitalSessionsBundle" />.
        // /// </ summary >
        // /// < remarks >
        // /// This value is session-level and reflects the total TMU count across all modules in each instrument session.
        // /// The returned array contains one value per instrument session, in the same order as <see cref = "ISessionsBundle{TSessionInformation}.InstrumentSessions" />.
        // /// </ remarks >
        // /// < param name="sessionsBundle">The<see cref = "DigitalSessionsBundle" />.</ param >
        // ///< returns > An array containing the total number of TMU resources available, one value per instrument session.</returns>
        // public static int[] GetTMUCount(this DigitalSessionsBundle sessionsBundle)
        // {
        //    return sessionsBundle.InstrumentSessions
        //        .Select(sessionInfo => GetDigitalTmus(sessionInfo.Session).GetTmuCount())
        //        .ToArray();
        // }

        // #endregion

        private static void AssignTMUContexts(this DigitalSessionInformation digitalSessionInformation, string[] pins = null)
        {
            // Filter sitePinInfo based on specified pins.
            var sitePinInfos = (pins != null && pins.Any())
                ? digitalSessionInformation.AssociatedSitePinList.Where(sp => pins.Contains(sp.PinName))
                : digitalSessionInformation.AssociatedSitePinList;

            // Initialize the TMUAssignmentManager with the available TMU resources for the devices within the current session.
            List<string> availableTMUContexts = GetDigitalTmus(digitalSessionInformation.Session).GetDisabledTmuContexts();
            Dictionary<string, Queue<string>> tmuContextsPerInstrument = CategorizeTMUContextsByInstrument(availableTMUContexts);

            // Assign TMU resources to each target pin/site pair within the session.
            foreach (SitePinInfo sitePinInfo in sitePinInfos)
            {
                var digitalSitePinInfo = sitePinInfo as DigitalSitePinInfo;
                var assignedTmuContext = digitalSitePinInfo?.AssignedTmuContext;
                // Assign TMU only if it is not already assigned.
                // It may already be assigned if:
                // - AssignTMUResources() is invoked twice on the same bundle object.
                // - AssignTMUResources() is invoked after having already invoked AssignTMUResources(pinNames) on the same the bundle object for a subset of pins.
                // - AssignTMUResources(pinNames1) is invoked after having already invoked AssignTMUResources(pinNames2) on the same the bundle object,
                // where pinNames1 and pinNames2 contain overlapping pins.
                if (string.IsNullOrEmpty(assignedTmuContext))
                {
                    string deviceName = digitalSitePinInfo.InstrumentName;
                    if (!TryGetTMUContext(tmuContextsPerInstrument, deviceName, out string tmuContext))
                    {
                        throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUNotEnoughResources, deviceName, sitePinInfo.PinName));
                    }
                    digitalSitePinInfo.AssignedTmuContext = tmuContext;
                }
            }
        }

        private static void ClearAssignedTMUContexts(this DigitalSessionInformation digitalSessionInformation, string[] pins = null, bool doTMUReleaseCheck = true)
        {
            // Filter sitePinInfo based on specified pins.
            var sitePinInfos = (pins != null && pins.Any())
                ? digitalSessionInformation.AssociatedSitePinList.Where(sp => pins.Contains(sp.PinName))
                : digitalSessionInformation.AssociatedSitePinList;
            // Check if all the assigned TMUs of site/pin pair are safe to release.
            if (doTMUReleaseCheck && !IsSafeToReleaseAllTMUs(digitalSessionInformation.Session, sitePinInfos))
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUResourcesInUse));
            }

            // Clear assigned TMU and release it back to TMU resource pool.
            foreach (SitePinInfo sitePinInfo in sitePinInfos)
            {
                var digitalSitePinInfo = sitePinInfo as DigitalSitePinInfo;
                var assignedTmuContext = digitalSitePinInfo?.AssignedTmuContext;
                // Clear only if TMU resource is assigned for a site/pin pair.
                // This can happen when:
                // - 'ClearTMUAssignment' is invoked twice on the same bundle object.
                // - 'ClearTMUAssignment' is invoked before invoking 'AssignTMUResources'.
                // - 'ClearTMUAssignment(pinNames)' is invoked, targeting only a subset of pins within the bundle object, and then the 'ClearTMUAssignment()' is invoked on whole bundle object.
                if (!string.IsNullOrEmpty(assignedTmuContext))
                {
                    digitalSitePinInfo.AssignedTmuContext = string.Empty;
                    TMUContextManager.Instance.UnAssignTMUContext(digitalSitePinInfo.InstrumentName, assignedTmuContext);
                }
            }
        }

        private static bool IsSafeToReleaseAllTMUs(NIDigital session, IEnumerable<SitePinInfo> sitePinInfos)
        {
            List<string> availableTMUContexts = GetDigitalTmus(session).GetDisabledTmuContexts();
            foreach (var sitePinInfo in sitePinInfos)
            {
                string tmuContext = (sitePinInfo as DigitalSitePinInfo)?.AssignedTmuContext;

                // Break the loop when the TMU context is not in the 'availableTMUContexts', TMU resource is reserved at the driver level.
                if (!string.IsNullOrEmpty(tmuContext) && !availableTMUContexts.Contains(tmuContext))
                {
                    return false;
                }
            }
            return true; // It is safe to release only when all the assigned TMUs are free, resources not reserved at driver level.
        }

        private static void ConfigureAndEnableTmu(
            DigitalTmu tmu,
            string startSource,
            TmuSourceEvent startEvent,
            TmuPolarity startPolarity,
            string stopSource,
            TmuSourceEvent stopEvent,
            TmuPolarity stopPolarity,
            long samplesToAcquire,
            TmuArmSetting armSetting)
        {
            // Configure the TMU Start Source, Start Source Event, and Start Source Event Polarity.
            tmu.Start.Source = startSource;
            tmu.Start.SourceEvent = startEvent;
            tmu.Start.SourceEventPolarity = startPolarity;

            // Configure the TMU Stop Source, Stop Source Event, and Stop Source Event Polarity.
            tmu.Stop.Source = stopSource;
            tmu.Stop.SourceEvent = stopEvent;
            tmu.Stop.SourceEventPolarity = stopPolarity;

            // Configure samples to acquire.
            tmu.SamplesToAcquire = samplesToAcquire;

            // Configure the TMU Arm Type, and, when edge arming is requested,
            // the Edge Arm Source, Source Event, and Polarity to match the requested source.

            switch (armSetting)
            {
                case TmuArmSetting.Immediate:
                    tmu.ArmType = TmuArmType.Immediate;
                    break;
                case TmuArmSetting.StartEdge:
                    tmu.ArmType = TmuArmType.Edge;
                    tmu.EdgeArm.Source = startSource;
                    tmu.EdgeArm.SourceEvent = startEvent;
                    tmu.EdgeArm.Polarity = startPolarity;
                    break;
                case TmuArmSetting.StopEdge:
                    tmu.ArmType = TmuArmType.Edge;
                    tmu.EdgeArm.Source = stopSource;
                    tmu.EdgeArm.SourceEvent = stopEvent;
                    tmu.EdgeArm.Polarity = stopPolarity;
                    break;
                default:
                    throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUUnsupportedArmSetting, armSetting.ToString()));
            }

            // Enable the TMU (reserve it).
            tmu.Enabled = true;
        }

        private static bool TryGetTMUContext(Dictionary<string, Queue<string>> tmuContextsPerInstrument, string deviceName, out string tmuContext)
        {
            tmuContext = null;
            if (tmuContextsPerInstrument.TryGetValue(deviceName, out var tmuContexts))
            {
                while (tmuContexts.Any())
                {
                    var availableTMUContext = tmuContexts.Dequeue();
                    if (TMUContextManager.Instance.TryAssignTMUContext(deviceName, availableTMUContext))
                    {
                        tmuContext = availableTMUContext;
                        return true;
                    }
                }
            }

            return false;
        }

        private static Dictionary<string, Queue<string>> CategorizeTMUContextsByInstrument(List<string> availableTMUContexts)
        {
            // A null or empty list yields an empty dictionary, so downstream TryGetTMUContext simply
            // reports that no TMU resources are available rather than faulting here.
            if (availableTMUContexts == null || availableTMUContexts.Count == 0)
            {
                return new Dictionary<string, Queue<string>>();
            }
            // Build a dictionary with device name as key and queue of available TMU contexts as value.
            return availableTMUContexts.GroupBy(tmuContext => tmuContext.Split('/')[0])
                .ToDictionary(g => g.Key, g => new Queue<string>(g));
        }

        private static void ValidateSkewPins(string[] referencePinNames, string[] targetPinNames)
        {
            // Check that no target pin appears in the reference pins array.
            var overlappingPins = targetPinNames.Intersect(referencePinNames, StringComparer.OrdinalIgnoreCase).ToArray();

            if (overlappingPins.Any())
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUSkewOverlappingPins, string.Join(", ", overlappingPins.Select(p => $"\"{p}\""))));
            }
        }

        private static void ValidatePinsOfTMU(IEnumerable<string> bundlePins, string[] requestedPins)
        {
            if (requestedPins == null || requestedPins.Length == 0)
            {
                return;
            }
            if (requestedPins.Any(pin => string.IsNullOrEmpty(pin)))
            {
                throw new NISemiconductorTestException(
                    string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUPinsNullOrEmpty));
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

        private static DigitalTmu GetAssignedTmu(DigitalSessionInformation sessionInfo, SitePinInfo sitePinInfo)
        {
            string tmuContext = (sitePinInfo as DigitalSitePinInfo)?.AssignedTmuContext;
            return GetDigitalTmus(sessionInfo.Session).GetTmu(tmuContext);
        }

        private static DigitalTmuCollection GetDigitalTmus(NIDigital session)
        {
            return session.Tmu;
        }

        private static void ValidateTmuArmType(TmuArmType armType)
        {
            if (armType != TmuArmType.Immediate && armType != TmuArmType.Edge)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUUnsupportedArmType, armType.ToString()));
            }
        }

        private static void ValidateTmuArmSetting(TmuArmSetting armSetting)
        {
            if (armSetting != TmuArmSetting.Immediate && armSetting != TmuArmSetting.StartEdge && armSetting != TmuArmSetting.StopEdge)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUUnsupportedArmSetting, armSetting.ToString()));
            }
        }

        private static TmuSourceEvent ValidateAndGetSourceEventForEdge(TmuPolarity edgeType)
        {
            switch (edgeType)
            {
                case TmuPolarity.RisingEdge:
                    return TmuSourceEvent.Voh;
                case TmuPolarity.FallingEdge:
                    return TmuSourceEvent.Vol;
                default:
                    throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUUnsupportedPolarity, edgeType.ToString()));
            }
        }

        private static void ValidateSkewParameters(string[] referencePinNames, string[] targetPinNames, TmuArmSetting armSetting, IEnumerable<string> bundlePins)
        {
            if (referencePinNames == null)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUSkewMeasurementNullReferencePinsOrTargetPins, nameof(referencePinNames)));
            }
            if (targetPinNames == null)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUSkewMeasurementNullReferencePinsOrTargetPins, nameof(targetPinNames)));
            }
            if (referencePinNames.Length == 0)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUSkewEmptyReferenceOrTargetPins, nameof(referencePinNames)));
            }
            if (targetPinNames.Length == 0)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUSkewEmptyReferenceOrTargetPins, nameof(targetPinNames)));
            }
            // Validate array lengths match
            if (referencePinNames.Length != targetPinNames.Length)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Digital_TMUSkewPinCountMismatch, referencePinNames.Length, targetPinNames.Length));
            }

            ValidateTmuArmSetting(armSetting);
            // Validate all pins exist in the bundle
            var allPins = referencePinNames.Concat(targetPinNames).Distinct().ToArray();
            ValidatePinsOfTMU(bundlePins, allPins);

            // Validate reference and target pins are not the same
            ValidateSkewPins(referencePinNames, targetPinNames);
        }
    }
}