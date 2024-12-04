using System;

namespace ClinicQueueSimulation
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            Simulation sim = new(simulationUpdateTime: 0.05, simulationLength: 10.0);

            Doctor[] doctors = [ // 1 sekunda - 15 minut
                new(id: 0, serviceTime: 0.1, inputQueueID: 0),
                new(id: 1, serviceTime: 1.0, inputQueueID: 1),
                new(id: 2, serviceTime: 1.0, inputQueueID: 2),
                new(id: 3, serviceTime: 1.0, inputQueueID: 2), // Drugi lekarz, który bierze z tej samej kolejki = kolejny kanał obsługi tej kolejki
                new(id: 4, serviceTime: 1.0, inputQueueID: 2),
                new(id: 5, serviceTime: 1.0, inputQueueID: 3),
                new(id: 6, serviceTime: 1.2, inputQueueID: 4),
                new(id: 7, serviceTime: 1.0, inputQueueID: 5),
                new(id: 8, serviceTime: 5.0, inputQueueID: 6),
                new(id: 9, serviceTime: 0.1, inputQueueID: 7),
            ];

            PatientQueue[] systems = [
                new(id: 0, "Rejestracja", isPriorityBased: false) {AssignedDoctors = [doctors[0]]}, // system startowy
                new(id: 1, "Pediatra", isPriorityBased: false) {AssignedDoctors = [doctors[1]]},
                new(id: 2, "Internista", isPriorityBased: true) {AssignedDoctors = [doctors[2], doctors[3], doctors[4]]},
                new(id: 3, "Kardiolog", isPriorityBased: true) {AssignedDoctors = [doctors[5]]},
                new(id: 4, "Dermatolog", isPriorityBased: true) {AssignedDoctors = [doctors[6]]},
                new(id: 5, "Laryngolog", isPriorityBased: true) {AssignedDoctors = [doctors[7]]},
                new(id: 6, "Chirurg", isPriorityBased: false) {AssignedDoctors = [doctors[8]]},
                new(id: 7, "Kasa", isPriorityBased: false) {AssignedDoctors = [doctors[9]]},
            ];

            List<PatientClass> patientClasses = new List<PatientClass>() // Ścieżki (możliwe klasy) pacjenta (macierze prawdopodobieństwa przejścia do danego systemu)
            { // indeks rzędu - z którego systemu idziemy, indeks kolumny - do którego systemu idziemy, wartość - prawdopodobieństwo przejścia do danego systemu
                // Prawdopodobieństwo w procentach, nie mogą być ułamki

                new PatientClass() // Dziecko
                {
                    ClassID = PatientClassID.Child,

                    ClassProbability = 15,

                    MovementProbabilities = [
                    [ 0, 100, 0, 0, 0, 0, 0, 0 ], // Rejestracja 0
                    [ 0, 0, 0, 8, 8, 8, 1, 75 ], // Pediatra 1
                    [ 0, 0, 0, 0, 0, 0, 0, 0 ], // Internista 2
                    [ 0, 20, 0, 0, 0, 0, 1, 79 ], // Kardiolog 3
                    [ 0, 20, 0, 0, 0, 0, 0, 80 ], // Dermatolog 4
                    [ 0, 20, 0, 0, 0, 0, 0, 80 ], // Laryngolog 5
                    [ 0, 0, 0, 0, 0, 0, 0, 100 ], // Chirurg 6
                    [ 0, 0, 0, 0, 0, 0, 0, 0 ] // Kasa 7
                    ],

                    PriorityProbabilities = [0, 0, 100]
                },

                new PatientClass() // Dorosły
                {
                    ClassID = PatientClassID.Adult,

                    ClassProbability = 35,

                    MovementProbabilities = [
                    [ 0, 0, 55, 15, 15, 15, 0, 0 ], // Rejestracja 0
                    [ 0, 0, 0, 0, 0, 0, 0, 0 ], // Pediatra 1
                    [ 0, 0, 0, 10, 5, 10, 3, 72 ], // Internista 2
                    [ 0, 0, 20, 0, 0, 0, 5, 75 ], // Kardiolog 3
                    [ 0, 0, 10, 0, 0, 0, 0, 90 ], // Dermatolog 4
                    [ 0, 0, 10, 0, 0, 0, 1, 89 ], // Laryngolog 5
                    [ 0, 0, 0, 0, 0, 0, 0, 100 ], // Chirurg 6
                    [ 0, 0, 0, 0, 0, 0, 0, 0 ] // Kasa 7
                    ],

                    PriorityProbabilities = [60, 25, 15]
                },

                new PatientClass() // Starszy
                {
                    ClassID = PatientClassID.Elder,

                    ClassProbability = 35,

                    MovementProbabilities = [
                    [ 0, 0, 45, 25, 5, 25, 0, 0 ], // Rejestracja 0
                    [ 0, 0, 0, 0, 0, 0, 0, 0 ], // Pediatra 1
                    [ 0, 0, 0, 15, 0, 10, 5, 70 ], // Internista 2
                    [ 0, 0, 17, 0, 0, 0, 8, 75 ], // Kardiolog 3
                    [ 0, 0, 0, 0, 0, 0, 0, 100 ], // Dermatolog 4
                    [ 0, 0, 10, 0, 0, 0, 1, 89 ], // Laryngolog 5
                    [ 0, 0, 0, 0, 0, 0, 0, 100 ], // Chirurg 6
                    [ 0, 0, 0, 0, 0, 0, 0, 0 ] // Kasa 7
                    ],

                    PriorityProbabilities = [40, 40, 20]
                },

                new PatientClass() // Alergik
                {
                    ClassID = PatientClassID.Allergic,

                    ClassProbability = 15,

                    MovementProbabilities = [
                    [ 0, 0, 40, 10, 25, 25, 0, 0 ], // Rejestracja 0
                    [ 0, 0, 0, 0, 0, 0, 0, 0 ], // Pediatra 1
                    [ 0, 0, 0, 5, 25, 25, 1, 44 ], // Internista 2
                    [ 0, 0, 20, 0, 0, 0, 5, 75 ], // Kardiolog 3
                    [ 0, 0, 10, 0, 0, 0, 0, 90 ], // Dermatolog 4
                    [ 0, 0, 10, 0, 0, 0, 1, 89 ], // Laryngolog 5
                    [ 0, 0, 0, 0, 0, 0, 0, 100 ], // Chirurg 6
                    [ 0, 0, 0, 0, 0, 0, 0, 0 ] // Kasa 7
                    ],

                    PriorityProbabilities = [60, 25, 15]
                }
            };
            
            PatientGenerator generator1 = new(id: 0, timeBetweenAttempts: 0.05, percentageChanceEachAttempt: 50, availablePatientClasses: patientClasses);

            InformationDisplay informationDisplay = new(sim, generator1, systems, doctors);

            EventManager.InvokeStartSimulationEvent();

            Console.ReadKey();

            if (sim.Generating)
            {
                EventManager.InvokeStopGeneratingPatientsEvent();
                Console.ReadKey();
            }

            EventManager.InvokeStopSimulationEvent();
        }
    }
}