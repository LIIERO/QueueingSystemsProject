using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class PatientGenerator : RealTimeObject
    {
        public uint ID { get; private set; }
        public double TimeBetweenAttempts { get; private set; }
        public int PercentageChanceEachAttempt { get; private set; }
        public List<int[][]> AvailablePatientClasses { get; private set; }
        public List<Patient> GeneratedPatients { get; private set; } = new();

        private double attemptTimer = 0.0;

        private Random randomizer = new Random();

        public PatientGenerator(uint id, double timeBetweenAttempts, int percentageChanceEachAttempt, List<int[][]> availablePatientClasses) : base()
        {
            ID = id;
            TimeBetweenAttempts = timeBetweenAttempts;
            PercentageChanceEachAttempt = percentageChanceEachAttempt;
            AvailablePatientClasses = availablePatientClasses;
        }

        protected override void Update(double delta)
        {
            base.Update(delta);

            if (attemptTimer <= 0.0)
            {
                attemptTimer = TimeBetweenAttempts;

                if (randomizer.Next(0, 100) < PercentageChanceEachAttempt)
                {
                    PatientPriority priority = (PatientPriority)randomizer.Next(0, Constants.highestPatientPriority + 1);
                    int[][] patientClass = AvailablePatientClasses[randomizer.Next(0, AvailablePatientClasses.Count)];
                    Patient newPatient = new(priority, patientClass);
                    GeneratedPatients.Add(newPatient);

                    newPatient.MoveToFirstSystem();
                }
            }

            attemptTimer -= delta;
        }
    }
}
