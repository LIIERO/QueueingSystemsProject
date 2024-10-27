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
        public List<Patient> HealedPatients { get; private set; } = new();

        public Doctor(uint id, double serviceTime) : base()
        {
            ID = id;
            ServiceTime = serviceTime;
        }

        protected override void Update(double delta)
        {
            base.Update(delta);

            if (IsMissingPatients)
                EventManager.InvokeRequestPatientEvent(this);
        }

        private void CurePatient()
        {
            if (!IsWorking) return;
            IsWorking = false;
            
            if (CurrentPatient != null)
            {
                PatientsCured++;
                CurrentPatient.IsHealed = true;
                HealedPatients.Add(CurrentPatient);
            }    
            else throw new NullException();

            CurrentPatient = null;
            IsMissingPatients = true;
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
    }
}
