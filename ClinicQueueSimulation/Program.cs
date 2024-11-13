using System;

namespace ClinicQueueSimulation
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            Simulation sim = new(simulationUpdateTime: 0.05, simulationLength: 50.0);

            PatientQueue queue = new(id: 0);

            PatientQueue healedPatientsQueue = new(id: 1);

            PatientGenerator generator1 = new(id: 0, targetQueueID: 0, timeBetweenAttempts: 0.1, percentageChanceEachAttempt: 40);

            Doctor[] doctors = [
                new(id: 0, serviceTime: 1.0, inputQueues: [0], targetQueueID: 1), 
                new(id: 1, serviceTime: 1.5, inputQueues: [0], targetQueueID: 1), 
                new(id: 2, serviceTime: 2.0, inputQueues: [0], targetQueueID: 1), 
                new(id: 3, serviceTime: 2.5, inputQueues: [0], targetQueueID: 1),
                new(id: 4, serviceTime: 3.0, inputQueues: [0], targetQueueID: 1)
            ];

            //InformationDisplay informationDisplay = new(sim, generator1, queue, doctors);
            InformationDisplay informationDisplay = new(sim, generator1, healedPatientsQueue, doctors);

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