using System.Collections.Generic;
using System.Linq;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Relay
{
    /// <summary>
    /// Defines methods for relay control.
    /// </summary>
    public static class Control
    {
        /// <summary>
        /// Performs the relay action on the relay.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="relay">The name of the relay to control.</param>
        /// <param name="relayAction">The relay action to perform.</param>
        public static void ControlRelay(ISemiconductorModuleContext tsmContext, string relay, RelayDriverAction relayAction)
        {
            tsmContext.ControlRelay(relay, relayAction);
        }

        /// <summary>
        /// Performs the same relay action on multiple relays.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="relays">The name of the relays to control.</param>
        /// <param name="relayAction">The relay action to perform.</param>
        public static void ControlRelay(ISemiconductorModuleContext tsmContext, string[] relays, RelayDriverAction relayAction)
        {
            tsmContext.ControlRelay(relays, relayAction);
        }

        /// <summary>
        /// Performs the relay actions on the relays.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="relays">The name of the relays to control.</param>
        /// <param name="relayActions">The relay actions to perform.</param>
        public static void ControlRelay(ISemiconductorModuleContext tsmContext, string[] relays, RelayDriverAction[] relayActions)
        {
            tsmContext.ControlRelay(relays, relayActions);
        }

        /// <summary>
        /// Performs the relay actions on the relays.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="relayNameToActionDictionary">A dictionary that maps of the relay name to the relay action to apply on that relay.</param>
        public static void ControlRelay(ISemiconductorModuleContext tsmContext, IDictionary<string, RelayDriverAction> relayNameToActionDictionary)
        {
            tsmContext.ControlRelay(relayNameToActionDictionary.Keys.ToArray(), relayNameToActionDictionary.Values.ToArray());
        }

        /// <summary>
        /// Performs the relay actions on the relays.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="relays">The name of the relays to control.</param>
        /// <param name="perSiteRelayActions">The per-site relay actions to perform.</param>
        public static void ControlRelay(ISemiconductorModuleContext tsmContext, string[] relays, IDictionary<int, RelayDriverAction> perSiteRelayActions)
        {
            foreach (var tsmSiteContext in tsmContext.GetSiteSemiconductorModuleContexts())
            {
                tsmSiteContext.ControlRelay(relays, perSiteRelayActions[tsmSiteContext.SiteNumbers.First()]);
            }
        }

        /// <summary>
        /// Performs the relay actions on the relays in the relay configuration.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="relayConfiguration">The name of the relay configuration to apply.</param>
        public static void ApplyRelayConfiguration(ISemiconductorModuleContext tsmContext, string relayConfiguration)
        {
            tsmContext.ApplyRelayConfiguration(relayConfiguration);
        }
    }
}
