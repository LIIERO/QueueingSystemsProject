using System;

namespace ClinicQueueSimulation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Simulation sim = new(0.1, 50.0);
            PatientQueue queue = new();

            PatientGenerator generator1 = new(0);
            Doctor[] doctors = [new(0, 0.9), new(1, 1.1)];

            InformationDisplay informationDisplay = new(sim, queue, doctors);

            EventManager.InvokeStartSimulationEvent();

            Console.ReadKey();

            if (sim.Running)
            { 
                EventManager.InvokeStopSimulationEvent();
                Console.ReadKey();
            }
        }
    }
}