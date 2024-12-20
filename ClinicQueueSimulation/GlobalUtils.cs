﻿using System;
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
        public const double simulationUpdateTime = 0.1; // To musi być na 0,1; można zmienić pod warunkiem zmian w walidacji średniego czasu między zgłoszeniami

        public const int highestPatientPriority = (int)PatientPriority.High;

        public const int maxQueueLength = 99;

        public const int SimulationSecondsToRealTimeMinutes = 15;
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

            if (probSum != 0 && probSum != 100)
            {
                Console.WriteLine(probSum);
                throw new Exception();
            }

            return null;
        }

        public static ConsoleColor PatientClassToColor(PatientClassID ClassID)
        {
            return ClassID switch
            {
                PatientClassID.Child => ConsoleColor.Green,
                PatientClassID.Adult => ConsoleColor.Cyan,
                PatientClassID.Elder => ConsoleColor.Magenta,
                _ => ConsoleColor.White,
            };
        }

        /*public static T GetInputFromKeyboard<T>()
        {
            T.Parse
        }*/
    }
}
