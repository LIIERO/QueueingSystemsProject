using System;
using System.Collections;
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
        //private const string simDataPath = "E:\\SzkolaProgramowanie\\SK\\QueueingSystemsProject\\SimulationResults\\simulationData.csv";
        //private const string patientDataPath = "E:\\SzkolaProgramowanie\\SK\\QueueingSystemsProject\\SimulationResults\\patientData.csv";
        private string queueDataPath;
        private string systemDataPath;
        private string patientDataPath;

        private Simulation simulation;
        private PatientGenerator generator;
        private PatientQueue[] queues;
        private PatientQueue mainQueue;
        private Doctor[] doctors;

        private List<int> mainQueueLengthHistory = new();
        private StringBuilder queueCSV = new();
        private StringBuilder systemCSV = new();

        private Thread consoleUpdateThread;

        public InformationDisplay(Simulation simulation, PatientGenerator generator, PatientQueue[] queues, Doctor[] doctors)
        {
            //Console.SetBufferSize(200, 30);

            string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            queueDataPath = Path.GetFullPath(Path.Combine(sCurrentDirectory, @"..\..\..\..\SimulationResults\queueData.csv"));
            systemDataPath = Path.GetFullPath(Path.Combine(sCurrentDirectory, @"..\..\..\..\SimulationResults\systemData.csv"));
            patientDataPath = Path.GetFullPath(Path.Combine(sCurrentDirectory, @"..\..\..\..\SimulationResults\patientData.csv"));

            this.simulation = simulation;
            this.generator = generator;
            this.queues = queues;
            this.doctors = doctors;
            this.mainQueue = queues[0];

            EventManager.StopSimulation += DisplaySimulationData;

            StringBuilder headerLine = new();
            headerLine.Append("Time [s]");
            foreach (PatientQueue q in queues)
            {
                headerLine.Append(',');
                headerLine.Append(q.Name);
            }
            queueCSV.AppendLine(headerLine.ToString());
            systemCSV.AppendLine(headerLine.ToString());
        }

        ~InformationDisplay()
        {
            EventManager.StopSimulation -= DisplaySimulationData;
        }

        protected override void Update(double delta)
        {
            base.Update(delta);

            consoleUpdateThread = new Thread(new ThreadStart(UpdateConsole));
            consoleUpdateThread.Start();

            // Writing data to CSV
            StringBuilder queueLine = new();
            StringBuilder systemLine = new();
            string currentTime = simulation.CurrentTime.ToString(CultureInfo.InvariantCulture);
            queueLine.Append(currentTime);
            systemLine.Append(currentTime);
            foreach (PatientQueue q in queues)
            {
                int queueLen = q.GetQueueLength();

                queueLine.Append(',');
                queueLine.Append(queueLen.ToString());

                foreach (Doctor d in q.AssignedDoctors)
                {
                    if (d.IsWorking) queueLen++;
                }
                systemLine.Append(',');
                systemLine.Append(queueLen.ToString());
            }
            queueCSV.AppendLine(queueLine.ToString());
            systemCSV.AppendLine(systemLine.ToString());

            consoleUpdateThread.Join();
        }

        private void UpdateConsole()
        {
            Console.SetCursorPosition(0, 0);
            //Console.SetWindowPosition(0, 0);

            TimeSpan span = new(0, (int)(simulation.CurrentTime * Constants.SimulationSecondsToRealTimeMinutes), 0);
            string genStatus = simulation.Generating ? "Włączona" : "Wyłączona";
            Console.WriteLine("Czas symulacji: {0:0.00} --- Rzeczywisty czas: {1:00} h {2:00} min --- Generacja zgłoszeń: {3}, średni czas zgłoszeń = {4:0.00}", simulation.CurrentTime, span.Hours, span.Minutes, genStatus, generator.AverageTimeBetweenPatients);

            //mainQueueLengthHistory.Add(mainQueue.GetQueueLength());
            //Console.WriteLine($"\nDługość kolejki {mainQueue.ID}: {l:00}");

            foreach (PatientQueue queue in queues)
            {
                Console.Write($"\nKolejka {queue.Name}: ");
                queue.PrintVisualRepresentation();
                Console.WriteLine();
                //Console.SetWindowPosition(0, 0);

                foreach (Doctor doctor in queue.AssignedDoctors)
                {
                    string doctorState;
                    Patient? currentPatient = doctor.CurrentPatient;

                    if (currentPatient == null)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        doctorState = "Czeka...                                 ";
                            
                    }
                    else// lock(currentPatient)
                    {
                        Console.ForegroundColor = GlobalUtils.PatientClassToColor(currentPatient.ClassID);
                        doctorState = "Leczy pacjenta o priorytecie " + (int)currentPatient.Priority;
                    }
                    
                    Console.WriteLine($"Lekarz {doctor.ID}, czas obsługi: {doctor.ServiceTime}s, stan: {doctorState}");
                    //Console.SetWindowPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            //Console.SetWindowPosition(0, 0);
        }

        private void DisplaySimulationData()
        {
            consoleUpdateThread.Join();

            Console.Clear();
            Console.SetCursorPosition(0, 0);

            //Console.WriteLine("\nDane symulacji:");

            //Console.WriteLine($"Średnia liczba pacjentów w kolejce {mainQueue.ID}: {mainQueueLengthHistory.Average()}");

            Console.WriteLine("Statystyki lekarzy:\n");
            foreach (Doctor doctor in doctors)
            {
                Dictionary<PatientClassID, int> healedPatientsByClass = new() { { PatientClassID.Child, 0 }, { PatientClassID.Adult, 0 }, { PatientClassID.Elder, 0 } };
                foreach (Patient patient in doctor.HealedPatients)
                {
                    healedPatientsByClass[patient.ClassID]++;
                }

                Console.WriteLine($"Lekarz {doctor.ID} - {doctor.InputSystemName}, obsłużono {doctor.HealedPatients.Count} pacjentów:");
                Console.WriteLine($"{healedPatientsByClass[PatientClassID.Child]} dzieci, {healedPatientsByClass[PatientClassID.Adult]} dorosłych i {healedPatientsByClass[PatientClassID.Elder]} starszych.\n");
            }

            //foreach (PatientQueue q in queues) Console.WriteLine(q.GetQueueLength());
            //Console.WriteLine();
            //foreach (PatientQueue q in queues) Console.WriteLine(q.HowManyAdded);
            //Console.WriteLine();
            //foreach (PatientQueue q in queues) Console.WriteLine(q.HowManyRemoved);
            //Console.WriteLine();
            //Console.WriteLine(EventManager.NoPatientAddedToSevenQueue);

            // Writing to csv
            File.WriteAllText(queueDataPath, queueCSV.ToString());
            File.WriteAllText(systemDataPath, systemCSV.ToString());

            StringBuilder patientCSV = new();
            string headerLine = "ID,Class name,Priority,Time spent in system [s],Time spent in queue [s],Wyleczony,Był w tylu systemach,Ostatni lekarz ID,Obecny system ID";
            patientCSV.AppendLine(headerLine);
            foreach (Patient patient in generator.GeneratedPatients)
            {
                var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", patient.ID.ToString(), patient.ClassID, patient.Priority.ToString(), patient.TimeSpentInSystem.ToString(CultureInfo.InvariantCulture), patient.TimeSpentInQueue.ToString(CultureInfo.InvariantCulture), patient.IsHealed, patient.NoSystemsGoneThrough, patient.LatestDoctorID, patient.CurrentSystemID);
                patientCSV.AppendLine(newLine);
            }
            File.WriteAllText(patientDataPath, patientCSV.ToString());
        }
    }
}
