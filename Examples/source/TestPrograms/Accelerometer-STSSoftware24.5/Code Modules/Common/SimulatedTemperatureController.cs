using System;
using System.Timers;

namespace Accelerometer.Common
{
    internal sealed class SimulatedTemperatureController : IDisposable
    {
        private const int RoomTemperature = 23;
        private readonly Timer mTemperatureAdjustTimer;

        public SimulatedTemperatureController()
        {
            CurrentTemperature = RoomTemperature;
            RequestedTemperature = RoomTemperature;

            mTemperatureAdjustTimer = new Timer(25);
            mTemperatureAdjustTimer.Elapsed += AdjustTemperatureTowardRequestedTemperature;
        }

        private void AdjustTemperatureTowardRequestedTemperature(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            const double epsilon = 1;
            if (Math.Abs(CurrentTemperature - RequestedTemperature) < epsilon)
            {
                CurrentTemperature = RequestedTemperature;
                mTemperatureAdjustTimer.Stop();
                return;
            }
            if (CurrentTemperature < RequestedTemperature)
            {
                CurrentTemperature++;
            }
            else
            {
                CurrentTemperature--;
            }
        }

        public double CurrentTemperature { get; private set; }

        public double RequestedTemperature { get; private set; }

        public void SetTemperature(double requestedTemperature)
        {
            RequestedTemperature = requestedTemperature;
            mTemperatureAdjustTimer.Start();
        }

        public void ResetToRoomTemperature()
        {
            SetTemperature(RoomTemperature);
        }

        public void Shutdown()
        {
            mTemperatureAdjustTimer.Dispose();
            // The simulated temperature controller does not require any shutdown code.
            // Shutdown code for actual hardware would otherwise be inserted here.
        }

        public void Dispose()
        {
            mTemperatureAdjustTimer?.Dispose();
        }
    }
}