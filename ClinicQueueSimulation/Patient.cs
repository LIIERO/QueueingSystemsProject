using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool IsHealed { get; set; } = false;
        public PatientPriority Priority { get; set; } // The priority may change during the simulation

        public Patient(PatientPriority priority)
        {
            ID = nextId;
            Priority = priority;
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

        ~Patient() { NoPatients--; }
    }
}
