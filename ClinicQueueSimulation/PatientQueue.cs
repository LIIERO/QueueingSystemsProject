﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class PatientQueue// : RealTimeObject
    {
        public List<Patient> PatientList { get; private set; } = new(); // List of patients in order of appearence

        private Dictionary<PatientPriority, List<Patient>> patients = new(); // Patients sorted by priority

        public PatientQueue()
        {
            EventManager.AddPatientToQueue += AddPatient;
            EventManager.RemovePatientFromQueue += RemovePatient;
            EventManager.RequestPatient += RoutePatientToDoctor;
        }

        ~PatientQueue()
        {
            EventManager.AddPatientToQueue -= AddPatient;
            EventManager.RemovePatientFromQueue -= RemovePatient;
            EventManager.RequestPatient -= RoutePatientToDoctor;
        }

        /*protected override void Update(double delta)
        {
            base.Update(delta);

            Console.WriteLine(GetQueueLength());
        }*/

        private void AddPatient(Patient patient)
        {
            PatientList.Add(patient);

            PatientPriority priority = patient.Priority;

            if (!patients.ContainsKey(priority))
            {
                patients.Add(priority, new List<Patient>());
            }
            patients[priority].Add(patient);
        }

        public Patient? RemoveTopPatient()
        {
            // Going from highest priority to lowest
            for (int p = Constants.highestPatientPriority; p >= 0; p--)
            {
                PatientPriority priority = (PatientPriority)p;

                if (!patients.ContainsKey(priority)) continue;

                if (patients[priority].Count == 0) continue;

                Patient firstPatient = patients[priority][0];
                patients[priority].RemoveAt(0);

                PatientList.Remove(firstPatient);

                return firstPatient;
            }

            return null;
        }

        public void RemovePatient(Patient patient)
        {
            if (!patients.ContainsKey(patient.Priority)) return;

            if (patients[patient.Priority].Count == 0) return;

            patients[patient.Priority].Remove(patient);

            PatientList.Remove(patient);
        }

        public void RoutePatientToDoctor(Doctor doctor)
        {
            doctor.RoutePatient(RemoveTopPatient());
        }

        public int GetQueueLength()
        {
            return PatientList.Count;
        }

        public string GetVisualRepresentation()
        {
            StringBuilder sb = new();
            int l = GetQueueLength();
            for (int i = 0; i < Constants.maxQueueLength; i++)
            {
                if (i < l) sb.Append((int)PatientList[i].Priority);
                else sb.Append('_');
            }
            return sb.ToString();
        }
    }
}
