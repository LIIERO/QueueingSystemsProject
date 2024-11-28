using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class PatientQueue : RealTimeObject
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public bool IsPriorityBased { get; private set; }
        public List<Patient> PatientList { get; private set; } = new(); // List of patients in order of appearence

        private Dictionary<PatientPriority, List<Patient>> patients = new(); // Patients sorted by priority

        private List<Doctor> requestingDoctors = new();
        private Random random = new Random();

        public Doctor[] AssignedDoctors { get; set; } // For display only

        public PatientQueue(int id, string systemName, bool isPriorityBased)
        {
            ID = id;
            Name = systemName;
            IsPriorityBased = isPriorityBased;
            EventManager.AddPatientToQueue += AddPatient;
            EventManager.RemovePatientFromQueue += RemovePatient;
            EventManager.RequestPatient += AddDoctor;
            IsPriorityBased = isPriorityBased;
        }

        ~PatientQueue()
        {
            EventManager.AddPatientToQueue -= AddPatient;
            EventManager.RemovePatientFromQueue -= RemovePatient;
            EventManager.RequestPatient -= AddDoctor;
        }

        protected override void Update(double delta)
        {
            base.Update(delta);

            if (requestingDoctors.Count <= 0) return;
            
            int r = random.Next(requestingDoctors.Count);
            requestingDoctors[r].RoutePatient(RemoveTopPatient());
            requestingDoctors.RemoveAt(r);
        }

        private void AddPatient(Patient patient, int queueID)
        {
            if (ID != queueID) return;

            patient.IsInQueue = true;
            PatientList.Add(patient);
            PatientPriority priority = patient.Priority;

            if (!patients.ContainsKey(priority))
            {
                patients.Add(priority, new List<Patient>());
            }
            patients[priority].Add(patient);
        }

        public Patient? RemoveTopPatient() // Removes the top patient that wants to go to specified doctor
        {
            // If queue is not priority based we remove the earliest patient
            if (!IsPriorityBased)
            {
                if (PatientList.Count <= 0) return null;

                Patient nextPatient = PatientList[0];
                PatientList.RemoveAt(0);
                patients[nextPatient.Priority].Remove(nextPatient);

                return nextPatient;
            }

            // If it is priority based, we are going from highest priority to lowest
            for (int prio = Constants.highestPatientPriority; prio >= 0; prio--)
            {
                PatientPriority priority = (PatientPriority)prio;

                if (!patients.ContainsKey(priority)) continue;

                if (patients[priority].Count <= 0) continue;

                Patient nextPatient = patients[priority][0]; // Earliest patient of the priority
                patients[priority].RemoveAt(0);
                PatientList.Remove(nextPatient);
                nextPatient.IsInQueue = false;

                return nextPatient;
            }

            return null;
        }

        public void RemovePatient(Patient patient, int queueID)
        {
            if (ID != queueID) return;

            if (!patients.ContainsKey(patient.Priority)) return;

            if (patients[patient.Priority].Count == 0) return;

            patients[patient.Priority].Remove(patient);

            PatientList.Remove(patient);

            patient.IsInQueue = false;
        }

        public void AddDoctor(Doctor doctor, int queueID)
        {
            if (queueID != ID) return;

            if (!requestingDoctors.Contains(doctor)) requestingDoctors.Add(doctor);
        }

        public int GetQueueLength()
        {
            return PatientList.Count;
        }

        public void PrintVisualRepresentation()
        {
            //StringBuilder sb = new();
            int l = GetQueueLength();
            for (int i = 0; i < Constants.maxQueueLength; i++)
            {
                if (i < l)
                {
                    Console.ForegroundColor = GlobalUtils.PatientClassToColor(PatientList[i].ClassID);
                    //sb.Append((int)PatientList[i].Priority);
                    Console.Write((int)PatientList[i].Priority);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    //sb.Append('_');
                    Console.Write('_');
                } 
            }
            Console.ForegroundColor = ConsoleColor.White;
            //return sb.ToString();
        }
    }
}
