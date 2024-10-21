using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal class InformationDisplay : RealTimeObject
    {
        private Simulation simulation;
        private PatientQueue mainQueue;
        private Doctor[] doctors;

        private List<int> queueLengthHistory = new();

        public InformationDisplay(Simulation simulation, PatientQueue mainQueue, Doctor[] doctors)
        {
            this.simulation = simulation;
            this.mainQueue = mainQueue;
            this.doctors = doctors;

            EventManager.StopSimulation += DisplaySimulationData;
        }

        ~InformationDisplay()
        {
            EventManager.StopSimulation -= DisplaySimulationData;
        }

        protected override void Update(double delta)
        {
            base.Update(delta);

            Console.SetCursorPosition(0, 0);

            Console.WriteLine("Czas symulacji: {0:0.###}", simulation.CurrentTime);

            int l = mainQueue.GetQueueLength();
            queueLengthHistory.Add(l);
            Console.WriteLine("\nDługość kolejki: {0:00}", l);

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

        private void DisplaySimulationData()
        {
            Console.WriteLine("\nDane symulacji:");

            Console.WriteLine($"Średnia liczba pacjentów w kolejce: {queueLengthHistory.Average()}");
        }
    }
}
