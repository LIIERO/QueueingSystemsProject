using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class Doctor : RealTimeObject
    {
        public int ID { get; private set; }
        public double ServiceTime { get; private set; }
        public int InputQueueID { get; private set; } // From which queue does the doctor take patients from?
        //public int TargetQueueID { get; private set; } // Which queue does the doctor put "healed" patients to?
        public int NOPatientsCured { get; private set; } = 0;
        public bool IsWorking { get; private set; } = false;
        public bool IsMissingPatients { get; private set; } = true; // Nie ma pacjentów w kolejce do wzięcia
        public Patient? CurrentPatient { get; private set; } = null; // TODO: Do something with the patient after curing?
        public List<Patient> HealedPatients { get; private set; } = new();

        public Doctor(int id, double serviceTime, int inputQueueID) : base()
        {
            ID = id;
            ServiceTime = serviceTime;
            InputQueueID = inputQueueID;
            //TargetQueueID = targetQueueID;
        }

        protected override void Update(double delta)
        {
            base.Update(delta);

            if (IsMissingPatients)
                EventManager.InvokeRequestPatientEvent(this, InputQueueID);
        }

        private void CurePatient()
        {
            if (!IsWorking) return;
            IsWorking = false;
            
            if (CurrentPatient != null)
            {
                NOPatientsCured++;
                HealedPatients.Add(CurrentPatient);

                CurrentPatient.MoveToNextSystem();
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
