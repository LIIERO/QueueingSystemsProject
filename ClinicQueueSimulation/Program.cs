using System;

namespace ClinicQueueSimulation
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            Simulation sim = new(0.05, 50.0);
            PatientQueue queue = new();

            PatientGenerator generator1 = new(0, 0.1, 40);
            Doctor[] doctors = [new(0, 1.0), new(1, 1.5), new(2, 2.0), new(3, 2.5), new(4, 3.0)];

            InformationDisplay informationDisplay = new(sim, generator1, queue, doctors);

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