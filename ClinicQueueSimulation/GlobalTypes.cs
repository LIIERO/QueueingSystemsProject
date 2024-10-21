using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    public enum PatientPriority { Low = 0, Medium = 1, High = 2 };

    public static class Constants
    {
        public const int highestPatientPriority = (int)PatientPriority.High;

        public const int maxQueueLength = 99;

        public const double bufferDelay = 0.05;
    }

}
