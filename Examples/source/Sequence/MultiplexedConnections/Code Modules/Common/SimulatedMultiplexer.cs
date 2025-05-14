namespace SemiconductorTestLibrary.Examples.MultiplexedConnections.Common
{
    public class SimulatedMultiplexer
    {
        public readonly string ResourceName;
        
        public SimulatedMultiplexer(string resourceName) 
        {
            ResourceName = resourceName;
        }

        public void ConnectRoute(string routeName) { }
        public void DisconnectRoute(string routeName) { }

        public void Close() { }
    }
}
