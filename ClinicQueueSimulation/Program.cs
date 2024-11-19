using System;

namespace ClinicQueueSimulation
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            Simulation sim = new(simulationUpdateTime: 0.05, simulationLength: 50.0);

            PatientQueue[] systems = [
                new(id: 0, isPriorityBased: true), // System startowy
                new(id: 1, isPriorityBased: false),
                new(id: 2, isPriorityBased: false),
                new(id: 3, isPriorityBased: true)
            ];

            List<int[][]> patientClasses = new List<int[][]>() // Ścieżki (możliwe klasy) pacjenta (macierze prawdopodobieństwa przejścia do danego systemu)
            { // indeks rzędu - z którego systemu idziemy, indeks kolumny - do którego systemu idziemy, wartość - prawdopodobieństwo przejścia do danego systemu
                // Prawdopodobieństwo w procentach, nie mogą być ułamki
                new int [][] { 
                    [ 0, 100, 0, 0 ],
                    [ 0, 0, 100, 0 ],
                    [ 0, 0, 0, 100 ],
                    [ 0, 0, 0, 0 ], // Jeżeli są same zera to jest to system końcowy dla danej klasy
                }
                //new int [,] { { 2, 0 } },
                //new int [,] { { 0, 3, 1, 0, 2 } }
            };
                
            PatientGenerator generator1 = new(id: 0, timeBetweenAttempts: 0.1, percentageChanceEachAttempt: 40, availablePatientClasses: patientClasses);

            Doctor[] doctors = [
                new(id: 0, serviceTime: 1.0, inputQueueID: 0),
                new(id: 1, serviceTime: 1.5, inputQueueID: 1), 
                new(id: 2, serviceTime: 2.0, inputQueueID: 2),
                new(id: 3, serviceTime: 2.5, inputQueueID: 3),
                new(id: 4, serviceTime: 1.5, inputQueueID: 0) // Drugi lekarz, który bierze z tej samej kolejki = kolejny kanał obsługi tej kolejki
            ];

            InformationDisplay informationDisplay = new(sim, generator1, systems, doctors);

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