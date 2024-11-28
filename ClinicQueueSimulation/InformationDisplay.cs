using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ClinicQueueSimulation
{
    internal class InformationDisplay : RealTimeObject
    {
        private const string simDataPath = "E:\\SzkolaProgramowanie\\SK\\QueueingSystemsProject\\SimulationResults\\simulationData.csv";
        private const string patientDataPath = "E:\\SzkolaProgramowanie\\SK\\QueueingSystemsProject\\SimulationResults\\patientData.csv";

        private Simulation simulation;
        private PatientGenerator generator;
        private PatientQueue[] queues;
        private PatientQueue mainQueue;
        private Doctor[] doctors;

        private List<int> mainQueueLengthHistory = new();
        private StringBuilder simCSV = new();

        public InformationDisplay(Simulation simulation, PatientGenerator generator, PatientQueue[] queues, Doctor[] doctors)
        {
            this.simulation = simulation;
            this.generator = generator;
            this.queues = queues;
            this.doctors = doctors;
            this.mainQueue = queues[0];

            EventManager.StopSimulation += DisplaySimulationData;

            string headerLine = $"Time [s],Queue {mainQueue.ID} length";
            simCSV.AppendLine(headerLine);
        }

        ~InformationDisplay()
        {
            EventManager.StopSimulation -= DisplaySimulationData;
        }

        protected override void Update(double delta)
        {
            base.Update(delta);

            Console.SetCursorPosition(0, 0);

            Console.WriteLine("Czas symulacji: {0:0.###}", simulation.CurrentTime);

            int l = mainQueue.GetQueueLength();
            mainQueueLengthHistory.Add(l);
            //Console.WriteLine($"\nDługość kolejki {mainQueue.ID}: {l:00}");

            Console.Write("\nSystemy kolejkowe:");
            foreach (PatientQueue queue in queues)
            {
                Console.Write($"\nKolejka {queue.Name}: ");
                queue.PrintVisualRepresentation();
                Console.WriteLine();
                foreach (Doctor doctor in queue.AssignedDoctors)
                {
                    string doctorState;
                    if (doctor.CurrentPatient != null)
                    {
                        Console.ForegroundColor = GlobalUtils.PatientClassToColor(doctor.CurrentPatient.ClassID);
                        doctorState = "Leczy pacjenta o priorytecie " + (int)doctor.CurrentPatient.Priority;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        doctorState = "Czeka...                                 ";
                    }
                    
                    Console.WriteLine($"Lekarz {doctor.ID}, czas obsługi: {doctor.ServiceTime}s, stan: {doctorState}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            // Writing data to CSV       
            var newLine = string.Format("{0},{1}", simulation.CurrentTime.ToString(CultureInfo.InvariantCulture), mainQueue.GetQueueLength().ToString());
            simCSV.AppendLine(newLine);
        }

        private void DisplaySimulationData()
        {
            Console.WriteLine("\nDane symulacji:");

            Console.WriteLine($"Średnia liczba pacjentów w kolejce {mainQueue.ID}: {mainQueueLengthHistory.Average()}");

            Console.WriteLine("\nStatystyki lekarzy:");
            foreach (Doctor doctor in doctors)
            {
                Console.WriteLine($"Lekarz {doctor.ID}, obsłużono {doctor.HealedPatients.Count} pacjentów");
            }

            // Writing to csv
            File.WriteAllText(simDataPath, simCSV.ToString());

            StringBuilder patientCSV = new();
            string headerLine = "ID,Priority,Time spent in system [s],Time spent in queue [s]";
            patientCSV.AppendLine(headerLine);
            foreach (Patient patient in generator.GeneratedPatients)
            {
                var newLine = string.Format("{0},{1},{2},{3}", patient.ID.ToString(), patient.Priority.ToString(), patient.TimeSpentInSystem.ToString(CultureInfo.InvariantCulture), patient.TimeSpentInQueue.ToString(CultureInfo.InvariantCulture));
                patientCSV.AppendLine(newLine);
            }
            File.WriteAllText(patientDataPath, patientCSV.ToString());
        }
    }
}
