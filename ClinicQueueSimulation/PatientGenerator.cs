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

        private Random randomizer = new Random();

        public PatientGenerator(uint id) : base()
        {
            ID = id;
        }

        protected override void Update(double delta)
        {
            base.Update(delta);

            // TODO: Make some proper generation
            if (randomizer.Next(0, 4) == 0) // for now 25% for a patient to come each iteration without considering delta time
            {
                PatientPriority priority = (PatientPriority)randomizer.Next(0, Constants.highestPatientPriority + 1);
                Patient newPatient = new(priority);
                EventManager.InvokeAddPatientToQueueEvent(newPatient);
            }
        }
    }
}
