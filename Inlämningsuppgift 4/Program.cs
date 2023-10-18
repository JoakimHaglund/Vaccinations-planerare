using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Vaccination
{
    public class ValidateData
    {
        public static int ReadInt(string question)
        {
            while (true)
            {
                Console.Write(question + " ");
                try
                {
                    int value = int.Parse(Console.ReadLine());
                    return value;
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Värdet är utanför intervallet!");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Värdet har fel format, var vänlig ange heltal!");
                }
            }
        }
        public static DateOnly? Date(string date)
        {
            try
            {
                var output = DateOnly.ParseExact(date, "yyyyMMdd");
                return output;
            }
            catch
            {
                return null;
            }
        }
        public static Patient CheckForNull(Patient patient)
        {
            bool isNull = false;

            isNull = string.IsNullOrEmpty(patient.LastFourDigits);
            isNull = string.IsNullOrEmpty(patient.FirstName);
            isNull = string.IsNullOrEmpty(patient.FirstName);

            isNull = patient.HealthcareWorker == null;
            isNull = patient.RiskGroup == null;
            isNull = patient.HasBeenInfected == null;
            isNull = patient.Personnummer == null;

            if (isNull)
            {
                return null;
            }
            else
            {
                return patient;
            }
        }
    }
    public class FileIo
    {
        public static string ReadFilePath(bool checkFileExist = true)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("(Ange \"exit\" för att för att avsluta)");
                Console.Write("Ange filnamn: ");
                string path = Console.ReadLine();

                int last = path.LastIndexOf('\\');
                string directoryPath = "";

                if (last != -1)
                {
                    directoryPath = path.Substring(0, last);
                }
                else
                {
                    Console.WriteLine("Filsökvägen kunde inte hittas!");
                }

                if (Directory.Exists(directoryPath))
                {
                    if (!checkFileExist)
                    {
                        return path;
                    }

                    if (File.Exists(path))
                    {
                        return path;
                    }
                    else
                    {
                        Console.WriteLine("Filen kunde inte hittas!");
                    }
                }
                else
                {
                    Console.WriteLine("Katalogen kunde inte hittas!");
                }
                if (path.ToLower() == "exit")
                {
                    return null;
                }
            }
        }
        public static List<string> ReadFile(string path)
        {
            try
            {
                List<string> list = File.ReadAllLines(path).ToList();
                return list;
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Katalogen kunde inte hittas!");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Filen kunde inte hittas!");
            }
            catch
            {
                Console.WriteLine("Fel vid filläsning!");
            }
            return null;
        }
        public static void WriteFile(string path, string[] lines)
        {
            try
            {
                File.WriteAllLines(path, lines);
                Console.WriteLine("Resultatet har sparats i " + path);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Filsökvägen kunde inte hittas!");
            }
            catch
            {
                Console.WriteLine("Fel vid filläsning!");
            }
        }
    }
    public class Parser
    {
        public static List<string> Personnummer(string date)
        {
            var personnummer = new List<string>();
            int numOfHypens = date.Count(c => (c == '-'));
            if (numOfHypens > 1)
            {
                Console.WriteLine("Fel format!");
                return null;
            }
            if (date.Contains('-'))
            {
                personnummer = date.Split('-').ToList();
            }
            else
            {
                int split = date.Length - 4;
                string birthDate = date.Substring(0, split);
                string lastFourDigits = date.Substring(split);
                personnummer.Add(birthDate);
                personnummer.Add(lastFourDigits);
            }
            if (personnummer[0].Length < 8)
            {
                personnummer[0] = "19" + personnummer[0];
            }
            return personnummer;
        }
        public static bool? ToBool(string input)
        {
            if (input == "1" || input == "0")
            {
                return input == "1";
            }
            return null;
        }
        public static List<string> ToList(string input)
        {

            string[] elements = input.Split(',');

            if (elements.Length == 6)
            {
                return new List<string>(elements);
            }
            else
            {
                return null;
            }
        }
    }

    public class Patient
    {
        public DateOnly? Personnummer;
        public string LastFourDigits;
        public string FirstName;
        public string Lastname;
        public bool? HealthcareWorker;
        public bool? RiskGroup;
        public bool? HasBeenInfected;

        public static Patient AddPerson(string input)
        {
            List<string> elements = Parser.ToList(input);

            if (elements != null && elements.Count == 6)
            {
                var personnummer = Parser.Personnummer(elements[0]);

                return new Patient
                {
                    Personnummer = ValidateData.Date(personnummer[0]),
                    LastFourDigits = personnummer[1],
                    FirstName = elements[2],
                    Lastname = elements[1],
                    HealthcareWorker = Parser.ToBool(elements[3]),
                    RiskGroup = Parser.ToBool(elements[4]),
                    HasBeenInfected = Parser.ToBool(elements[5])
                };
            }
            else
            {
                return null;
            }
        }
        public static List<Patient> AddPersons(string[] input)
        {
            var patients = new List<Patient>();
            bool abort = false;
            for (int i = 0; i < input.Length; i++)
            {
                var patient = ValidateData.CheckForNull(AddPerson(input[i]));

                if (patient != null)
                {
                    patients.Add(patient);
                }
                else
                {
                    Console.WriteLine($"Läsfel på rad {i}");
                    abort = true;
                }
            }
            if (abort)
            {
                return null;
            }
            else
            {
                return patients;
            }
        }
    }
    public class Program
    {
        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            string PathIn = @"C:\Windows\Temp\People.csv";
            string PathOut = @"C:\Windows\Temp\Vaccinations.csv";

            bool VaccinateMinors = false;
            int AvailableVaccineDoses = 0;
            while (true)
            {
                Console.WriteLine("Huvudmeny");
                Console.WriteLine("---------");
                Console.WriteLine("Antal tillgängliga vaccindoser: " + AvailableVaccineDoses);
                Console.WriteLine("Vaccinering under 18 år: " + (VaccinateMinors ? "Ja" : "Nej"));
                Console.WriteLine("Indatafil: " + PathIn);
                Console.WriteLine("Utdatafil: " + PathOut);

                int choice = ShowMenu("Vad vill du göra?", new List<string>
                {
                    "Skapa prioritetsordning",
                    "Ändra antal vaccindoser",
                    "Ändra åldersgräns",
                    "Ändra indatafil",
                    "Ändra utdatafil",
                    "Avsluta"
                });

                if (choice == 0)
                {
                    List<string> input = FileIo.ReadFile(PathIn);
                    string[] output = CreateVaccinationOrder(input.ToArray(), AvailableVaccineDoses, VaccinateMinors);
                    if (output != null)
                    {
                        FileIo.WriteFile(PathOut, output);
                    }
                    else
                    {
                        Console.WriteLine("Resultatet har inte sparats!");
                    }
                    Console.ReadLine();
                }
                else if (choice == 1)
                {
                    AvailableVaccineDoses = ChangeVaccinationDoses();
                }
                else if (choice == 2)
                {
                    VaccinateMinors = ChangeVaccinationSetting();
                }
                else if (choice == 3)
                {
                    string temp = FileIo.ReadFilePath();
                    if (temp != null)
                    {
                        PathIn = temp;
                    }
                }
                else if (choice == 4)
                {
                    string temp = FileIo.ReadFilePath(false);
                    if (temp != null)
                    {
                        PathOut = temp;
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Live long and prosper!");
                    break;
                }
                Console.Clear();
            }
        }
        public static int ChangeVaccinationDoses()
        {
            Console.WriteLine("Ändra antal vaccindoser");
            Console.WriteLine("-----------------------");
            int vaccinationDoses = ValidateData.ReadInt("Ange nytt antal doser: ");
            Console.WriteLine($"Du angav vaccinationDoser: {vaccinationDoses}");
            return vaccinationDoses;
        }
        public static bool ChangeVaccinationSetting()
        {
            bool newSetting = AskForVaccinationSetting();
            Console.WriteLine("Inställningen har ändrats till: " + (newSetting ? "Ja" : "Nej"));
            return newSetting;
        }
        public static bool AskForVaccinationSetting()
        {
            var options = new List<string> { "Ja", "Nej" };
            int input = ShowMenu("Ska personer under 18 år vaccineras? (Ja/Nej)", options);

            return input == 0;
        }
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            var vaccinationOrder = new List<string>();
            var patients = new List<Patient>();
            var dateNow = DateOnly.FromDateTime(DateTime.Now);
            var dateEighteenYearsAgo = dateNow.AddYears(-18);

            patients = Patient.AddPersons(input);

            if (patients == null)
            {
                return null;
            }

            for (int i = 0; i < patients.Count; i++)
            {
                if (patients[i].Personnummer > dateEighteenYearsAgo && !vaccinateChildren)
                {
                    patients.RemoveAt(i);
                }
            }
            patients = patients.OrderByDescending(p => p.HealthcareWorker)
                .ThenBy(p => p.Personnummer >= dateNow.AddYears(-65))
                .ThenByDescending(p => p.RiskGroup)
                .ToList<Patient>();

            foreach (var person in patients)
            {
                int vaccineDoses = 2;

                if ((bool)person.HasBeenInfected)
                {
                    vaccineDoses = 1;
                }
                if (vaccineDoses <= doses)
                {
                    vaccinationOrder.Add(
                        person.Personnummer.Value.ToString("yyyyMMdd") +
                        "-" +
                        person.LastFourDigits +
                        "," +
                        person.Lastname +
                        "," +
                        person.FirstName +
                        "," +
                        vaccineDoses.ToString()
                        );
                    doses -= vaccineDoses;
                }
                else
                {
                    return null;
                }
            }
            return vaccinationOrder.ToArray();
        }
        public static int ShowMenu(string prompt, IEnumerable<string> options)
        {
            if (options == null || !options.Any())
            {
                throw new ArgumentException("Cannot show a menu for an empty list of options.");
            }

            Console.WriteLine(prompt);

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            // Calculate the width of the widest option so we can make them all the same width later.
            int width = options.Max(option => option.Length);

            int selected = 0;
            int top = Console.CursorTop;
            for (int i = 0; i < options.Count(); i++)
            {
                // Start by highlighting the first option.
                if (i == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var option = options.ElementAt(i);
                // Pad every option to make them the same width, so the highlight is equally wide everywhere.
                Console.WriteLine("- " + option.PadRight(width));

                Console.ResetColor();
            }
            Console.CursorLeft = 0;
            Console.CursorTop = top - 1;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true).Key;

                // First restore the previously selected option so it's not highlighted anymore.
                Console.CursorTop = top + selected;
                string oldOption = options.ElementAt(selected);
                Console.Write("- " + oldOption.PadRight(width));
                Console.CursorLeft = 0;
                Console.ResetColor();

                // Then find the new selected option.
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count() - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }

                // Finally highlight the new selected option.
                Console.CursorTop = top + selected;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                string newOption = options.ElementAt(selected);
                Console.Write("- " + newOption.PadRight(width));
                Console.CursorLeft = 0;
                // Place the cursor one step above the new selected option so that we can scroll and also see the option above.
                Console.CursorTop = top + selected - 1;
                Console.ResetColor();
            }

            // Afterwards, place the cursor below the menu so we can see whatever comes next.
            Console.CursorTop = top + options.Count();

            // Show the cursor again and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
    }

    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void VaccinateMinors()
        {
            // Arrange health / risk / infection
            string[] input =
            {
                "20000101-1111,Svennson,Bob,0,0,0",
                "20100101-2222,Svennson,Sven,0,0,0"
            };
            int doses = 10;
            bool vaccinateChildren = true;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(2, output.Length);
            Assert.AreEqual("20000101-1111,Svennson,Bob,2", output[0]);
            Assert.AreEqual("20100101-2222,Svennson,Sven,2", output[1]);
        }
        [TestMethod]
        public void NotEnoughVaccineDoses()
        {
            // Arrange health / risk / infection
            string[] input =
            {
                "20000101-1111,Svennson,Bob,0,0,0",
                "20000101-2222,Svennson,Sven,0,0,0"
            };
            int doses = 3;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert

            Assert.AreEqual(null, output);
        }
        [TestMethod]
        public void SortByAge()
        {
            // Arrange health / risk / infection
            string[] input =
            {
                "20000101-1111,Svennson,Bob,0,0,0",
                "20010101-2222,Svennson,Sven,0,0,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(2, output.Length);
            Assert.AreEqual("20000101-1111,Svennson,Bob,2", output[0]);
            Assert.AreEqual("20010101-2222,Svennson,Sven,2", output[1]);
        }
        [TestMethod]
        public void HealthCareWorkerPrioritized()
        {
            // Arrange health / risk / infection
            string[] input =
            {
                "19500101-1111,Svennson,Bob,0,0,0",
                "20010101-2222,Svennson,Sven,1,0,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(2, output.Length);
            Assert.AreEqual("19500101-1111,Svennson,Bob,2", output[1]);
            Assert.AreEqual("20010101-2222,Svennson,Sven,2", output[0]);
        }
        [TestMethod]
        public void CheckPrioritizationOrder()
        {
            // Arrange health / risk / infection
            string[] input =
            {
                "19500101-2222,Andersson,Mia,1,1,1",
                "19500101-2222,Andersson,Anna,1,0,1",
                "20000101-1111,Svennson,Bob,1,0,1",
                "20010101-2222,Nilsson,Mikael,1,0,0",
                "20010101-2222,Svennson,Sven,1,0,0",
                "20000101-1111,Mikaelsson,Kerstin,0,1,0",
                "19950101-1111,Bobsson,Sven,0,0,1"
            };
            int doses = 100;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(7, output.Length);
            Assert.AreEqual("19500101-2222,Andersson,Mia,1", output[0]);
            Assert.AreEqual("19500101-2222,Andersson,Anna,1", output[1]);
            Assert.AreEqual("20000101-1111,Svennson,Bob,1", output[2]);
            Assert.AreEqual("20010101-2222,Nilsson,Mikael,2", output[3]);
            Assert.AreEqual("20010101-2222,Svennson,Sven,2", output[4]);
            Assert.AreEqual("20000101-1111,Mikaelsson,Kerstin,2", output[5]);
            Assert.AreEqual("19950101-1111,Bobsson,Sven,1", output[6]);
        }
    }
}
