using System;

namespace ClinicQueueSimulation
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            Simulation sim = new(simulationUpdateTime: 0.05, simulationLength: 50.0);

            PatientQueue[] queues = [
                new(id: 0), // Kolejka startowa
                new(id: 1),
                new(id: 2),
                new(id: 3)
            ];

            int[][] patientClasses = [[0, 1, 2, 3], [2, 0], [1], [0, 3, 1, 0, 2]]; // Ścieżki (możliwe klasy) pacjenta (po kolei ID kolejek przez które musi przejść)
            PatientGenerator generator1 = new(id: 0, timeBetweenAttempts: 0.1, percentageChanceEachAttempt: 40, availablePatientClasses: patientClasses);

            Doctor[] doctors = [
                new(id: 0, serviceTime: 1.0, inputQueueID: 0),
                new(id: 1, serviceTime: 1.5, inputQueueID: 1), 
                new(id: 2, serviceTime: 2.0, inputQueueID: 2),
                new(id: 3, serviceTime: 2.5, inputQueueID: 3),
                new(id: 4, serviceTime: 1.5, inputQueueID: 0) // Drugi lekarz, który bierze z tej samej kolejki = kolejny kanał obsługi tej kolejki
            ];

            InformationDisplay informationDisplay = new(sim, generator1, queues, doctors);

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