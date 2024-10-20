using System;

namespace ClinicQueueSimulation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PatientQueue queue = new();

            PatientGenerator generator1 = new(0);
            Doctor[] doctors = [new(0, 1), new(1, 1)];

            InformationDisplay informationDisplay = new(queue, doctors);

            Simulation sim = new(0.1);
            sim.Start();

            
            
        }
    }
}