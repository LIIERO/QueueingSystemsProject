using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class Simulation
    {
        public double SimulationUpdateTime { get; private set; }
        public double SimulationLength { get; private set; }
        public double CurrentTime { get; private set; } = 0.0;
        public bool Running { get; private set; } = false;

        private PeriodicTimer? periodicTimer = null;

        public Simulation(double simulationUpdateTime, double simulationLength)
        {
            SimulationUpdateTime = simulationUpdateTime;
            SimulationLength = simulationLength;

            EventManager.StartSimulation += Start;
            EventManager.StopSimulation += Stop;
        }

        ~Simulation()
        {
            EventManager.StartSimulation -= Start;
            EventManager.StopSimulation -= Stop;
        }

        public void Start()
        {
            Running = true;
            RunInBackground(TimeSpan.FromSeconds(SimulationUpdateTime), RunUpdate);
        }

        public void Stop()
        {
            Running = false;
            periodicTimer?.Dispose();
        }

        async Task RunInBackground(TimeSpan timeSpan, Action action)
        {
            periodicTimer = new(timeSpan);
            while (await periodicTimer.WaitForNextTickAsync())
            {
                if (!Running) break;
                action();
            }
        }

        private void RunUpdate()
        {
            if (!Running) return;

            if (CurrentTime >= SimulationLength)
            {
                EventManager.InvokeStopSimulationEvent();
                return;
            }

            CurrentTime += SimulationUpdateTime;
            EventManager.InvokeUpdateRealTimeObjectsEvent(SimulationUpdateTime);
        }
    }
}
