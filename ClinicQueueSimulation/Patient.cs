using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClinicQueueSimulation
{
    internal class Patient : RealTimeObject
    {
        public static int NoPatients { get; private set; } = 0;
        private static int nextId = 0;
        private const int firstSystem = 0;

        public int ID { get; private set; }
        public int CurrentSystemID { get; private set; } // Every patient starts with the same one - firstQueue
        public double TimeSpentInSystem { get; private set; } = 0.0;
        public double TimeSpentInQueue { get; private set; } = 0.0;
        public bool IsInQueue { get; set; } = true;
        public bool IsHealed { get; private set; } = false;
        public PatientClassID ClassID { get; private set; }
        public PatientPriority Priority { get; set; } // The priority may change during the simulation
        public int[][] ClassMatrix { get; private set; } // The path of systems and probabilities
        public int NoSystemsGoneThrough { get; private set; } = 0;
        public int LatestDoctorID { get; set; }


        public Patient(PatientClassID classID, PatientPriority priority, int[][] classMatrix)
        {
            CurrentSystemID = firstSystem;
            ID = nextId;
            ClassID = classID;
            Priority = priority;
            ClassMatrix = classMatrix;
            NoPatients++;
            nextId++;
        }

        protected override void Update(double delta)
        {
            base.Update(delta);

            if (IsHealed) return;

            TimeSpentInSystem += delta;
            if (IsInQueue) TimeSpentInQueue += delta;
        }

        private int? GetNextRequiredSystem()
        {
            int[] probabilities = ClassMatrix[CurrentSystemID];
            return GlobalUtils.GetIndexByProbability(probabilities);
        }

        public void MoveToFirstSystem()
        {
            NoSystemsGoneThrough++;
            EventManager.InvokeAddPatientToQueueEvent(this, firstSystem);
        }

        public void MoveToNextSystem()
        {
            int? nextSystem = GetNextRequiredSystem();
            if (nextSystem == null)
            {
                IsHealed = true;
            }
            else
            {
                NoSystemsGoneThrough++;
                CurrentSystemID = (int)nextSystem;
                EventManager.InvokeAddPatientToQueueEvent(this, CurrentSystemID);
            }
        }

        /*public void Write()
        {

            (int)doctor.CurrentPatient.Priority
        }*/

        ~Patient() { NoPatients--; }
    }
}
