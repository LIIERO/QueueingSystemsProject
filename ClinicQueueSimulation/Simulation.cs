using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class Simulation(double simulationUpdateTime)
    {
        public double SimulationUpdateTime { get; private set; } = simulationUpdateTime;
        public double CurrentTime { get; private set; } = 0.0;
        public double DeltaTime { get; private set; } = 0.0;

        public void Start()
        {
            Stopwatch simulationTimer = new();
            double previousFrameTime = 0.0;
            double nextTickTimer = 0.0;

            simulationTimer.Start();

            while (true)
            {
                previousFrameTime = CurrentTime;
                CurrentTime = simulationTimer.Elapsed.TotalSeconds;
                DeltaTime = CurrentTime - previousFrameTime;

                nextTickTimer += DeltaTime;

                if (nextTickTimer >= SimulationUpdateTime)
                {
                    // Invoke update event every time simulation update time elapses
                    EventManager.InvokeUpdateRealTimeObjectsEvent(nextTickTimer);

                    nextTickTimer = 0.0;
                }
            }
        }
    }
}
