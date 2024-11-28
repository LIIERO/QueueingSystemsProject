using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    public enum PatientPriority { Low = 0, Medium = 1, High = 2 };
    public enum PatientClassID { Child = 0, Adult = 1, Elder = 2, Allergic = 3 };

    public static class Constants
    {
        public const int highestPatientPriority = (int)PatientPriority.High;

        public const int maxQueueLength = 99;
    }

    public class NullException : Exception { }
    public class WrongProbabilityException : Exception { }

    public static class GlobalUtils
    {
        private static Random randomizer = new Random();

        public static int? GetIndexByProbability(int[] probabilityList)
        {
            int r = randomizer.Next(0, 100);
            int probSum = 0;

            for (int nextID = 0; nextID < probabilityList.Length; nextID++)
            {
                probSum += probabilityList[nextID];
                if (r < probSum)
                    return nextID;
            }
            return null;
        }

        public static ConsoleColor PatientClassToColor(PatientClassID ClassID)
        {
            return ClassID switch
            {
                PatientClassID.Child => ConsoleColor.Green,
                PatientClassID.Adult => ConsoleColor.Cyan,
                PatientClassID.Elder => ConsoleColor.Yellow,
                PatientClassID.Allergic => ConsoleColor.Magenta,
                _ => ConsoleColor.White,
            };
        }
    }
}
