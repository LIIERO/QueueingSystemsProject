using System;
using System.Runtime.InteropServices;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;

namespace ClinicQueueSimulation
{
    internal class Program
    {
        private static List<PatientClass> patientClasses = new List<PatientClass>() // Ścieżki (możliwe klasy) pacjenta (macierze prawdopodobieństwa przejścia do danego systemu)
        { // indeks rzędu - z którego systemu idziemy, indeks kolumny - do którego systemu idziemy, wartość - prawdopodobieństwo przejścia do danego systemu
            // Prawdopodobieństwo w procentach, nie mogą być ułamki

            new PatientClass() // Dziecko
            {
                ClassID = PatientClassID.Child,

                ClassProbability = 25,

                MovementProbabilities = [
                [ 0, 100, 0, 0, 0, 0, 0, 0 ], // Rejestracja 0
                [ 0, 0, 0, 8, 8, 8, 1, 75 ], // Pediatra 1
                [ 0, 0, 0, 0, 0, 0, 0, 0 ], // Internista 2
                [ 0, 20, 0, 0, 0, 0, 1, 79 ], // Kardiolog 3
                [ 0, 20, 0, 0, 0, 0, 0, 80 ], // Dermatolog 4
                [ 0, 20, 0, 0, 0, 0, 0, 80 ], // Laryngolog 5
                [ 0, 0, 0, 0, 0, 0, 0, 100 ], // Chirurg 6
                [ 0, 0, 0, 0, 0, 0, 0, 0 ] // Kasa 7
                ],

                PriorityProbabilities = [0, 50, 50]
            },

            new PatientClass() // Dorosły
            {
                ClassID = PatientClassID.Adult,

                ClassProbability = 40,

                MovementProbabilities = [
                [ 0, 0, 55, 15, 15, 15, 0, 0 ], // Rejestracja 0
                [ 0, 0, 0, 0, 0, 0, 0, 0 ], // Pediatra 1
                [ 0, 0, 0, 10, 5, 10, 3, 72 ], // Internista 2
                [ 0, 0, 20, 0, 0, 0, 5, 75 ], // Kardiolog 3
                [ 0, 0, 10, 0, 0, 0, 0, 90 ], // Dermatolog 4
                [ 0, 0, 10, 0, 0, 0, 1, 89 ], // Laryngolog 5
                [ 0, 0, 0, 0, 0, 0, 0, 100 ], // Chirurg 6
                [ 0, 0, 0, 0, 0, 0, 0, 0 ] // Kasa 7
                ],

                PriorityProbabilities = [60, 25, 15]
            },

            new PatientClass() // Starszy
            {
                ClassID = PatientClassID.Elder,

                ClassProbability = 35,

                MovementProbabilities = [
                [ 0, 0, 45, 25, 5, 25, 0, 0 ], // Rejestracja 0
                [ 0, 0, 0, 0, 0, 0, 0, 0 ], // Pediatra 1
                [ 0, 0, 0, 15, 0, 10, 5, 70 ], // Internista 2
                [ 0, 0, 17, 0, 0, 0, 8, 75 ], // Kardiolog 3
                [ 0, 0, 0, 0, 0, 0, 0, 100 ], // Dermatolog 4
                [ 0, 0, 10, 0, 0, 0, 1, 89 ], // Laryngolog 5
                [ 0, 0, 0, 0, 0, 0, 0, 100 ], // Chirurg 6
                [ 0, 0, 0, 0, 0, 0, 0, 0 ] // Kasa 7
                ],

                PriorityProbabilities = [40, 40, 20]
            },

            /*new PatientClass() // Alergik
            {
                ClassID = PatientClassID.Allergic,

                ClassProbability = 15,

                MovementProbabilities = [
                [ 0, 0, 40, 10, 25, 25, 0, 0 ], // Rejestracja 0
                [ 0, 0, 0, 0, 0, 0, 0, 0 ], // Pediatra 1
                [ 0, 0, 0, 5, 25, 25, 1, 44 ], // Internista 2
                [ 0, 0, 20, 0, 0, 0, 5, 75 ], // Kardiolog 3
                [ 0, 0, 10, 0, 0, 0, 0, 90 ], // Dermatolog 4
                [ 0, 0, 10, 0, 0, 0, 1, 89 ], // Laryngolog 5
                [ 0, 0, 0, 0, 0, 0, 0, 100 ], // Chirurg 6
                [ 0, 0, 0, 0, 0, 0, 0, 0 ] // Kasa 7
                ],

                PriorityProbabilities = [60, 25, 15]
            }*/
        };


        static void LoadClassDataFromExcel()
        {
            Console.WriteLine("Wczytywanie danych klas pacjentów z pliku Excel...");
            bool validationSuccessful = true;

            string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dataPath = Path.GetFullPath(Path.Combine(sCurrentDirectory, @"..\..\..\..\SimulationData\patientClasses.xlsx"));

            const int matrixStartRow = 8, matrixStartCol = 3, matrixHeight = 8, matrixWidth = 8;
            const int otherDataCol = 2;
            const int classProbRow = 2;
            const int prioProbRowStart = 3;

            int GetIntValueFromCell(Excel.Range xlRange, int row, int col)
            {
                int outputVal;
                if (xlRange.Cells[row, col] == null || xlRange.Cells[row, col].Value2 == null)
                {
                    outputVal = 0;
                    Console.WriteLine($"W komórce ({row}, {col}) wykryto pustą wartość.");
                    validationSuccessful = false;
                } 
                else
                {
                    //bool success = int.TryParse(xlRange.Cells[row, col].Value2, out int output);
                    var output = xlRange.Cells[row, col].Value2; // To w excelu powinien być System.Double
                    //Console.WriteLine(output.GetType());
                    if (output.GetType() != typeof(double))
                    {
                        outputVal = 0;
                        Console.WriteLine($"W komórce ({row}, {col}) wykryto wartość, której nie można zrzutować do liczby całkowitej.");
                        validationSuccessful = false;
                    }
                    else outputVal = (int)output;
                }
                    
                return outputVal;
            }

            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(dataPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int classProbSum = 0;
            for (int noClass = 0; noClass < patientClasses.Count; noClass++)
            {
                xlWorksheet = xlWorkbook.Sheets[noClass + 1];
                xlRange = xlWorksheet.UsedRange;

                //Console.WriteLine(patientClasses[noClass].ClassID);
                int classProb = GetIntValueFromCell(xlRange, classProbRow, otherDataCol);
                classProbSum += classProb;
                patientClasses[noClass].ClassProbability = classProb;
                //Console.WriteLine(patientClasses[noClass].ClassProbability);
                //Console.WriteLine();

                int prioSum = 0;
                for (int p = 0; p <= Constants.highestPatientPriority; p++)
                {
                    int prioProb = GetIntValueFromCell(xlRange, p + prioProbRowStart, otherDataCol);
                    prioSum += prioProb;
                    patientClasses[noClass].PriorityProbabilities[p] = prioProb;
                    //Console.WriteLine(patientClasses[noClass].PriorityProbabilities[p]);
                }
                //Console.WriteLine();

                if (prioSum != 100)
                {
                    validationSuccessful = false;
                    Console.WriteLine($"Prawdopodobieństwa priorytetów klasy {patientClasses[noClass].ClassID} nie sumują się do 100.");
                }


                for (int i = 0; i < matrixHeight; i++)
                {
                    int rowProbSum = 0;
                    for (int j = 0; j < matrixWidth; j++)
                    {
                        int movementProb = GetIntValueFromCell(xlRange, i + matrixStartRow, j + matrixStartCol);
                        rowProbSum += movementProb;
                        patientClasses[noClass].MovementProbabilities[i][j] = movementProb;
                        //Console.WriteLine(patientClasses[noClass].MovementProbabilities[i][j]);
                    }
                    //Console.WriteLine();

                    if (rowProbSum != 100 && rowProbSum != 0)
                    {
                        validationSuccessful = false;
                        Console.WriteLine($"Prawdopodobieństwa przejścia klasy {patientClasses[noClass].ClassID} w rzędzie o indeksie {i} nie sumują się do 100 (ani do 0).");
                    }
                }
            }

            if (classProbSum != 100)
            {
                Console.WriteLine($"Prawdopodobieństwa wystąpienia klas nie sumują się do 100.");
                validationSuccessful = false;
            }

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            if (!validationSuccessful)
                Environment.Exit(0);
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Czy użyć danych domyślnych klas pacjentów? [t/n]");
            var inKey = Console.ReadKey().Key;
            Console.WriteLine();
            if (inKey != ConsoleKey.T && inKey != ConsoleKey.Y)
            {
                Console.Clear();
                LoadClassDataFromExcel();
            }
            Console.Clear();

            Simulation sim = new(simulationUpdateTime: Constants.simulationUpdateTime, simulationLength: 26.0); // 26 sekund = 6.5 godzin

            Doctor[] doctors = [ // 1 sekunda - 15 minut
                new(id: 0, serviceTime: 0.1, inputQueueID: 0),
                new(id: 10, serviceTime: 1.1, inputQueueID: 1),
                new(id: 11, serviceTime: 1.1, inputQueueID: 1), // Drugi lekarz, który bierze z tej samej kolejki = kolejny kanał obsługi tej kolejki
                new(id: 20, serviceTime: 1.0, inputQueueID: 2),
                new(id: 21, serviceTime: 1.0, inputQueueID: 2),
                new(id: 22, serviceTime: 1.0, inputQueueID: 2),
                new(id: 30, serviceTime: 1.0, inputQueueID: 3),
                new(id: 40, serviceTime: 1.2, inputQueueID: 4),
                new(id: 50, serviceTime: 1.0, inputQueueID: 5),
                new(id: 60, serviceTime: 5.0, inputQueueID: 6),
                new(id: 70, serviceTime: 0.1, inputQueueID: 7),
            ];

            //string[] systemNames = ["Rejestracja", "Pediatra", "Internista", ]
            PatientQueue[] systems = [
                new(id: 0, "Rejestracja", isPriorityBased: false, assignedDoctors: [doctors[0]]), // system startowy
                new(id: 1, "Pediatra", isPriorityBased: false, assignedDoctors: [doctors[1], doctors[2]]),
                new(id: 2, "Internista", isPriorityBased: true, assignedDoctors: [doctors[3], doctors[4], doctors[5]]),
                new(id: 3, "Kardiolog", isPriorityBased: true, assignedDoctors: [doctors[6]]),
                new(id: 4, "Dermatolog", isPriorityBased: true, assignedDoctors: [doctors[7]]),
                new(id: 5, "Laryngolog", isPriorityBased: true, assignedDoctors: [doctors[8]]),
                new(id: 6, "Chirurg", isPriorityBased: false, assignedDoctors: [doctors[9]]),
                new(id: 7, "Kasa", isPriorityBased: false, assignedDoctors: [doctors[10]]),
            ];

            int probEachAttempt = 50; // Wartość domyślna lambdy to 0.1/0.5 = 0.2
            Console.Clear();
            Console.WriteLine("Czy użyć danych domyślnych systemów? [t/n]");
            inKey = Console.ReadKey().Key;
            Console.WriteLine();
            if (inKey != ConsoleKey.T && inKey != ConsoleKey.Y)
            {
                Console.Clear();

                Console.Write("Podaj średni czas przybywania zgłoszeń - minimum 0,1 - maksimum 10 (liczba zmiennoprzecinkowa, użyj przecinka jako separatora): ");
                double lambda = double.Parse(Console.ReadLine());
                probEachAttempt = (int)(100 * (Constants.simulationUpdateTime / lambda));
                if (probEachAttempt < 1) probEachAttempt = 1;   
                else if (probEachAttempt > 100) probEachAttempt = 100;

                List<Doctor> newDoctors = new();

                foreach (PatientQueue system in systems)
                {
                    Console.Clear();
                    Console.WriteLine($"System {system.ID}: {system.Name}");
                    Console.Write("Podaj liczbę kanałów obsługi (liczba całkowita): ");
                    int noChannels = int.Parse(Console.ReadLine());
                    Console.Write("Podaj czas obsługi (liczba zmiennoprzecinkowa, użyj przecinka jako separatora): ");
                    double serviceTime = double.Parse(Console.ReadLine());

                    List<Doctor> systemDoctors = new();
                    for (int i = 0; i < noChannels; i++)
                    {
                        string newID = system.ID.ToString() + i.ToString();
                        Doctor newDoc = new(int.Parse(newID), serviceTime, system.ID);
                        newDoctors.Add(newDoc);
                        systemDoctors.Add(newDoc);
                    }
                    system.ReassignDoctors(systemDoctors.ToArray());
                }

                foreach (Doctor d in doctors)
                    d.Dispose();
                
                doctors = newDoctors.ToArray();
            }

            Console.Clear();

            /*foreach (Doctor doctor in doctors)
            {
                Console.WriteLine($"{doctor.ID}; {doctor.ServiceTime}; {doctor.InputQueueID}");
            }
            foreach (PatientQueue q in systems)
            {
                Console.WriteLine(q.AssignedDoctors.Length);
            }
            Console.ReadKey();
            Console.Clear();*/
            

            PatientGenerator generator1 = new(id: 0, timeBetweenAttempts: Constants.simulationUpdateTime, percentageChanceEachAttempt: probEachAttempt, availablePatientClasses: patientClasses);

            InformationDisplay informationDisplay = new(sim, generator1, systems, doctors);

            EventManager.InvokeStartSimulationEvent();

            Console.ReadKey();

            if (sim.Generating)
            {
                EventManager.InvokeStopGeneratingPatientsEvent();
                Console.ReadKey();
            }

            EventManager.InvokeStopSimulationEvent();

            Console.ReadKey();
        }
    }
}