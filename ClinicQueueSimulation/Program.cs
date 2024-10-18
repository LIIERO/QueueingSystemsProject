using System;

namespace ClinicQueueSimulation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PatientQueue queue = new();
            PatientGenerator generator1 = new(0);
            Doctor doctor1 = new(0, 1);
            Doctor doctor2 = new(1, 1);

            Simulation sim = new(0.1);
            sim.Start();
            
        }
    }
}