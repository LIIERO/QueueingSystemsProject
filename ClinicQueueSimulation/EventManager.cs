using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal static class EventManager
    {
        public delegate void SimulationEvent();
        public static event SimulationEvent? StartSimulation;
        public static event SimulationEvent? StopSimulation;

        public delegate void TimedEvent(double time);
        public static event TimedEvent? UpdateRealTimeObjects;

        public delegate void PatientEvent(Patient patient);
        public static event PatientEvent? AddPatientToQueue;
        public static event PatientEvent? RemovePatientFromQueue;

        public delegate void DoctorEvent(Doctor doctor);
        public static event DoctorEvent? RequestPatient;

        public static void InvokeStartSimulationEvent() { StartSimulation?.Invoke(); }
        public static void InvokeStopSimulationEvent() { StopSimulation?.Invoke(); }
        public static void InvokeUpdateRealTimeObjectsEvent(double delta) { UpdateRealTimeObjects?.Invoke(delta); }
        public static void InvokeAddPatientToQueueEvent(Patient patient) { AddPatientToQueue?.Invoke(patient); }
        public static void InvokeRemovePatientFromQueueEvent(Patient patient) { RemovePatientFromQueue?.Invoke(patient); }
        public static void InvokeRequestPatientEvent(Doctor doctor) { RequestPatient?.Invoke(doctor); }
    }
}
