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
        public bool IsGenerating { get; private set; }
        public double TimeBetweenAttempts { get; private set; }
        public int PercentageChanceEachAttempt { get; private set; }
        public List<PatientClass> AvailablePatientClasses { get; private set; }
        public List<Patient> GeneratedPatients { get; private set; } = new();

        private double attemptTimer = 0.0;

        private Random randomizer = new Random();

        public double AverageTimeBetweenPatients => 100 * (TimeBetweenAttempts / PercentageChanceEachAttempt);

        public PatientGenerator(uint id, double timeBetweenAttempts, int percentageChanceEachAttempt, List<PatientClass> availablePatientClasses) : base()
        {
            ID = id;
            TimeBetweenAttempts = timeBetweenAttempts;
            PercentageChanceEachAttempt = percentageChanceEachAttempt;
            AvailablePatientClasses = availablePatientClasses;

            EventManager.StopGeneratingPatients += StopGenerating;

            IsGenerating = true;
        }

        private void StopGenerating()
        {
            IsGenerating = false;
        }

        protected override void Update(double delta)
        {
            base.Update(delta);

            if (!IsGenerating) return;

            if (attemptTimer <= 0.0001)
            {
                attemptTimer = TimeBetweenAttempts;

                if (randomizer.Next(0, 100) < PercentageChanceEachAttempt)
                {
                    Patient newPatient = GetRandomPatient();

                    GeneratedPatients.Add(newPatient);

                    newPatient.MoveToFirstSystem();
                }
            }

            attemptTimer -= delta;
        }

        private Patient GetRandomPatient()
        {
            int[] classProbabilities = new int[AvailablePatientClasses.Count];
            for (int i = 0; i < classProbabilities.Length; i++)
            {
                classProbabilities[i] = AvailablePatientClasses[i].ClassProbability;
            }
            int? whichClass = GlobalUtils.GetIndexByProbability(classProbabilities) ?? throw new WrongProbabilityException();
            PatientClass chosenClass = AvailablePatientClasses[(int)whichClass];

            int? prio = GlobalUtils.GetIndexByProbability(chosenClass.PriorityProbabilities) ?? throw new WrongProbabilityException();
            PatientPriority priority = (PatientPriority)prio;

            return new Patient(chosenClass.ClassID, priority, chosenClass.MovementProbabilities);
        }

        ~PatientGenerator()
        {
            EventManager.StopGeneratingPatients -= StopGenerating;
        }
    }
}
