using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class PatientClass
    {
        public PatientClassID ClassID { get; set; }
        public int ClassProbability { get; set; } // Ta wartość musi się dodawać do 100 dla wszystkich klas
        public int[][] MovementProbabilities { get; set; }
        public int[] PriorityProbabilities { get; set; }

    }
}
