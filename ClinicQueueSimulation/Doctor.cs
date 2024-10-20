using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class Doctor : RealTimeObject
    {
        public uint ID { get; private set; }
        public double ServiceTime { get; private set; }
        public int PatientsCured { get; private set; } = 0;
        public bool IsWorking { get; private set; } = false;
        public bool IsMissingPatients { get; private set; } = true; // Nie ma pacjentów w kolejce do wzięcia
        public Patient? CurrentPatient { get; private set; } = null; // TODO: Do something with the patient after curing?

        private double serviceTimer = 0.0;

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

        protected override void Update(double delta)
        {
            base.Update(delta);

            if (!IsWorking) return;

            serviceTimer += delta;
            //Console.WriteLine($"---{serviceTimer}");

            if (serviceTimer > ServiceTime) // patient has been cured
            {
                IsWorking = false;

                PatientsCured++;

                CurrentPatient = null;

                //Console.WriteLine("CURED");

                EventManager.InvokeRequestPatientEvent(this);
            }
        }

        public void RoutePatient(Patient? patient) // Method used by PatientQueue class
        {
            if (IsWorking) return;
            if (patient == null)
            {
                IsMissingPatients = true;
                //Console.WriteLine("No patient routed");
                return;
            }

            //Console.WriteLine($"ROUTED id: {patient.ID}, priority: {patient.Priority}");
            IsMissingPatients = false;
            IsWorking = true;
            CurrentPatient = patient;
            serviceTimer = 0.0;
        }

        private void RequestPatientOnArrival(Patient _)
        {
            if (IsMissingPatients)
            {
                IsMissingPatients = false;
                //await EventManager.UpdateRealTimeObjects;
                EventManager.InvokeRequestPatientEvent(this);
            }
        }
    }
}
