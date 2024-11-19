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
        private const int firstSystem = 0;

        public int ID { get; private set; }
        public int CurrentSystemID { get; private set; } // Every patient starts with the same one - firstQueue
        public double TimeSpentInSystem { get; private set; } = 0.0;
        public double TimeSpentInQueue { get; private set; } = 0.0;
        public bool IsInQueue { get; set; } = true;
        public bool IsHealed { get; private set; } = false;
        public PatientPriority Priority { get; set; } // The priority may change during the simulation
        public int[][] ClassMatrix { get; private set; } // The path of systems and probabilities

        private Random randomizer = new Random();

        public Patient(PatientPriority priority, int[][] classMatrix)
        {
            CurrentSystemID = firstSystem;
            ID = nextId;
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
            /*if (ClassMatrix.Length == 0) return null;
            return ClassMatrix[0];*/

            int r = randomizer.Next(0, 100);

            int[] probabilities = ClassMatrix[CurrentSystemID];
            int probSum = 0;

            for (int nextQueue = 0; nextQueue < probabilities.Length; nextQueue++)
            {
                probSum += probabilities[nextQueue];
                if (r < probSum)
                    return nextQueue;
            }
            return null;
        }

        /*private void RemoveNextRequiredQueue()
        {
            var classPathList = new List<int>(ClassMatrix);
            classPathList.RemoveAt(0);
            ClassMatrix = classPathList.ToArray();
        }*/

        public void MoveToFirstSystem()
        {
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
                CurrentSystemID = (int)nextSystem;
                EventManager.InvokeAddPatientToQueueEvent(this, (int)nextSystem);
            }
        }

        ~Patient() { NoPatients--; }
    }
}
