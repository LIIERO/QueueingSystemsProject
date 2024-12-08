﻿using System;
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
        private const string simDataPath = "E:\\SzkolaProgramowanie\\SK\\QueueingSystemsProject\\SimulationResults\\simulationData.csv";
        private const string patientDataPath = "E:\\SzkolaProgramowanie\\SK\\QueueingSystemsProject\\SimulationResults\\patientData.csv";

        private Simulation simulation;
        private PatientGenerator generator;
        private PatientQueue[] queues;
        private PatientQueue mainQueue;
        private Doctor[] doctors;

        private List<int> mainQueueLengthHistory = new();
        private StringBuilder simCSV = new();

        private Thread consoleUpdateThread;

        public InformationDisplay(Simulation simulation, PatientGenerator generator, PatientQueue[] queues, Doctor[] doctors)
        {
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
            simCSV.AppendLine(headerLine.ToString());
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
            queueLine.Append(simulation.CurrentTime.ToString(CultureInfo.InvariantCulture));
            foreach (PatientQueue q in queues)
            {
                queueLine.Append(',');
                queueLine.Append(q.GetQueueLength().ToString());
            }
            simCSV.AppendLine(queueLine.ToString());

            consoleUpdateThread.Join();
        }

        private void UpdateConsole()
        {
            Console.SetCursorPosition(0, 0);

            TimeSpan span = new(0, (int)(simulation.CurrentTime * Constants.SimulationSecondsToRealTimeMinutes), 0);
            Console.WriteLine("Czas symulacji: {0:0.00}       Rzeczywisty czas: {1:00} h {2:00} min", simulation.CurrentTime, span.Hours, span.Minutes);

            //mainQueueLengthHistory.Add(mainQueue.GetQueueLength());
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
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        private void DisplaySimulationData()
        {
            consoleUpdateThread.Join();

            Console.Clear();
            Console.SetCursorPosition(0, 0);

            Console.WriteLine("\nDane symulacji:");

            //Console.WriteLine($"Średnia liczba pacjentów w kolejce {mainQueue.ID}: {mainQueueLengthHistory.Average()}");

            Console.WriteLine("\nStatystyki lekarzy:");
            foreach (Doctor doctor in doctors)
            {
                Console.WriteLine($"Lekarz {doctor.ID} - {doctor.InputSystemName}, obsłużono {doctor.HealedPatients.Count} pacjentów");
            }

            foreach (PatientQueue q in queues) Console.WriteLine(q.GetQueueLength());
            Console.WriteLine();
            foreach (PatientQueue q in queues) Console.WriteLine(q.HowManyAdded);
            Console.WriteLine();
            Console.WriteLine(EventManager.NoPatientAddedToSevenQueue);

            // Writing to csv
            File.WriteAllText(simDataPath, simCSV.ToString());

            StringBuilder patientCSV = new();
            string headerLine = "ID,Class name,Priority,Time spent in system [s],Time spent in queue [s],Wyleczony,Był w tylu systemach";
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
