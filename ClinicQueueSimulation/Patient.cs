using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class Patient
    {
        public static int NoPatients { get; private set; } = 0;
        private static int nextId = 0;

        public int ID { get; private set; }
        public PatientPriority Priority { get; set; } // The priority may change during the simulation

        public Patient(PatientPriority priority)
        {
            ID = nextId;
            Priority = priority;
            NoPatients++;
            nextId++;
        }

        ~Patient() { NoPatients--; }

        // TODO: Patient gets bored after some time and they leave queue (invoke RemovePatientFromQueue on themselves)
    }
}
