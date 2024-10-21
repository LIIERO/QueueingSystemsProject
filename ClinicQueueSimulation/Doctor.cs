using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class Doctor
    {
        public uint ID { get; private set; }
        public double ServiceTime { get; private set; }
        public int PatientsCured { get; private set; } = 0;
        public bool IsWorking { get; private set; } = false;
        public bool IsMissingPatients { get; private set; } = true; // Nie ma pacjentów w kolejce do wzięcia
        public Patient? CurrentPatient { get; private set; } = null; // TODO: Do something with the patient after curing?

        public Doctor(uint id, double serviceTime) : base()
        {
            EventManager.AddPatientToQueue += RequestPatientOnArrival;

            ID = id;
            ServiceTime = serviceTime;
        }

        ~Doctor()
        {
            EventManager.AddPatientToQueue -= RequestPatientOnArrival;
        }

        private void CurePatient()
        {
            if (!IsWorking) return;
            IsWorking = false;
            PatientsCured++;
            CurrentPatient = null;
            EventManager.InvokeRequestPatientEvent(this);
        }

        public void RoutePatient(Patient? patient) // Method used by PatientQueue class
        {
            if (IsWorking) return;
            if (patient == null)
            {
                IsMissingPatients = true;
                return;
            }

            IsMissingPatients = false;
            IsWorking = true;
            CurrentPatient = patient;

            Task.Delay((int)(1000 * ServiceTime)).ContinueWith(t => CurePatient());
        }

        private void RequestPatientOnArrival(Patient _)
        {
            if (IsMissingPatients)
            {
                IsMissingPatients = false;
                Task.Delay((int)(1000 * Constants.bufferDelay)).ContinueWith(t => EventManager.InvokeRequestPatientEvent(this));
            }
        }
    }
}
