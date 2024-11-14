using System;
using System.Collections.Generic;
using System.Linq;
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

        public int ID { get; private set; }
        public double TimeSpentInSystem { get; private set; } = 0.0;
        public double TimeSpentInQueue { get; private set; } = 0.0;
        public bool IsInQueue { get; set; } = true;
        public bool IsHealed { get; private set; } = false;
        public PatientPriority Priority { get; set; } // The priority may change during the simulation
        public int[] ClassPath { get; private set; } // The path of queues the patient must take

        public Patient(PatientPriority priority, int[] classPath)
        {
            ID = nextId;
            Priority = priority;
            ClassPath = classPath;
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

        public int? GetNextRequiredQueue()
        {
            if (ClassPath.Length == 0) return null;
            return ClassPath[0];
        }

        private void RemoveNextRequiredQueue()
        {
            var classPathList = new List<int>(ClassPath);
            classPathList.RemoveAt(0);
            ClassPath = classPathList.ToArray();
        }

        public void MoveToNextQueue()
        {
            int? nextQueue = GetNextRequiredQueue();
            if (nextQueue == null)
            {
                IsHealed = true;
            }
            else
            {
                RemoveNextRequiredQueue();
                EventManager.InvokeAddPatientToQueueEvent(this, (int)nextQueue);
            }
        }

        ~Patient() { NoPatients--; }
    }
}
