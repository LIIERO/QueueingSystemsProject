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
        private PatientQueue mainQueue;
        private Doctor[] doctors;

        private List<int> queueLengthHistory = new();
        private StringBuilder simCSV = new();

        public InformationDisplay(Simulation simulation, PatientGenerator generator, PatientQueue mainQueue, Doctor[] doctors)
        {
            this.simulation = simulation;
            this.generator = generator;
            this.mainQueue = mainQueue;
            this.doctors = doctors;

            EventManager.StopSimulation += DisplaySimulationData;

            string headerLine = "Time [s],Queue length";
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
            queueLengthHistory.Add(l);
            Console.WriteLine("\nDługość kolejki: {0:00}", l);

            Console.WriteLine("\nWizualizacja kolejki:");
            Console.WriteLine(mainQueue.GetVisualRepresentation());

            Console.WriteLine("\nLekarze:");
            foreach (Doctor doctor in doctors)
            {
                string doctorState;
                if (doctor.CurrentPatient != null) doctorState = "Leczy pacjenta o priorytecie " + (int)doctor.CurrentPatient.Priority;
                else doctorState = "Czeka...                                 ";
                Console.WriteLine($"Lekarz {doctor.ID}, czas obsługi: {doctor.ServiceTime}s, stan: {doctorState}");
            }

            // Writing data to CSV       
            var newLine = string.Format("{0},{1}", simulation.CurrentTime.ToString(CultureInfo.InvariantCulture), mainQueue.GetQueueLength().ToString());
            simCSV.AppendLine(newLine);
        }

        private void DisplaySimulationData()
        {
            Console.WriteLine("\nDane symulacji:");

            Console.WriteLine($"\nŚrednia liczba pacjentów w kolejce: {queueLengthHistory.Average()}");

            Console.WriteLine("\nStatystyki lekarzy:");
            foreach (Doctor doctor in doctors)
            {
                Console.WriteLine($"Lekarz {doctor.ID}, wyleczono {doctor.HealedPatients.Count} pacjentów");
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
