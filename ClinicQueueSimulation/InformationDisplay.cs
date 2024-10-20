using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class InformationDisplay : RealTimeObject
    {
        private PatientQueue mainQueue;
        private Doctor[] doctors;

        public InformationDisplay(PatientQueue mainQueue, Doctor[] doctors)
        {
            this.mainQueue = mainQueue;
            this.doctors = doctors;
        }

        protected override void Update(double delta)
        {
            base.Update(delta);

            Console.SetCursorPosition(0, 0);

            Console.WriteLine("Długość kolejki: {0:00}", mainQueue.GetQueueLength());

            Console.WriteLine("\nWizualizacja kolejki:");
            Console.WriteLine(mainQueue.GetVisualRepresentation());

            Console.WriteLine("\nLekarze:");
            foreach (Doctor doctor in doctors)
            {
                string doctorState;
                if (doctor.CurrentPatient != null) doctorState = "Leczy pacjenta o priorytecie " + (int)doctor.CurrentPatient.Priority;
                else doctorState = "Czeka...                                 ";
                Console.WriteLine($"Lekarz {doctor.ID}, czas obsługi: {doctor.ServiceTime}s, stan: {doctorState}");
            }
        }
    }
}
