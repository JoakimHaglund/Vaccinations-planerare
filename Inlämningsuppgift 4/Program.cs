using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Vaccination
{
    public class ValidateInput
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
                    Console.WriteLine("Värde utanför intervallet!");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Värdet har fel format, var vänlig ange heltal!");
                }
                catch
                {
                    Console.WriteLine("Felaktigt värde!");
                }
            }
        }
        public static string ReadString(string question)
        {
            while (true)
            {
                Console.Write(question + " ");
                try
                {
                    string value = Console.ReadLine();
                    return value;
                }
                catch
                {
                    Console.WriteLine("Felaktigt värde!");
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
                Console.WriteLine("Felaktigt datum");
                return null;
            }
        }
    }
    public class FileIo
    {
        public static string ReadFilePath(bool checkFileExist = true)
        {
            while (true)
            {
                Console.Write("Ange filnamn: ");
                string path = Console.ReadLine();
                int last = path.LastIndexOf('\\');
                string dirPath = "";
                if (last != -1)
                {
                    dirPath = path.Substring(0, last);
                }
                else
                {
                    Console.WriteLine("Felaktig filsökväg!");
                }
                if (Directory.Exists(dirPath))
                {
                    if (checkFileExist)
                    {
                        if (File.Exists(path))
                        {
                            return path;
                        }
                        else
                        {
                            Console.WriteLine("Filen finns inte!");
                        }
                    }
                    else
                    {
                        return path;
                    }
                }
                else
                {
                    Console.WriteLine("Katalogen finns inte");
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
                Console.WriteLine("FEL: Katalogen finns inte!");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("FEL: Kunde inte hitta filen!");
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
                Console.WriteLine("FEL: Filsökväg kunde inte hittas!");
            }
            catch
            {
                Console.WriteLine("Fel vid filläsning!");
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
            List<string> elements = ParseToList(input);

            if (elements != null && elements.Count == 6)
            {
                var personnummer = ParseDate(elements[0]);

                return new Patient
                {
                    Personnummer = ValidateInput.Date(personnummer[0]),
                    LastFourDigits = personnummer[1],
                    FirstName = elements[2],
                    Lastname = elements[1],
                    HealthcareWorker = ParseToBool(elements[3]),
                    RiskGroup = ParseToBool(elements[4]),
                    HasBeenInfected = ParseToBool(elements[5])
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
                var patient = CheckForNull(AddPerson(input[i]));

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
        public static List<string> ParseDate(string date)
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
        public static bool? ParseToBool(string input)
        {
            if (input == "1" || input == "0")
            {
                return input == "1";
            }
            return null;
        }
        public static List<string> ParseToList(string input)
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
        public static Patient CheckForNull(Patient patient)
        {
            if (patient.HealthcareWorker == null)
            {
                return null;
            }
            else if (patient.RiskGroup == null)
            {
                return null;
            }
            else if (patient.HasBeenInfected == null)
            {
                return null;
            }
            else if (patient.Personnummer == null)
            {
                return null;
            }
            else
            {
                return patient;
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
                        Console.WriteLine("Reslutatet har inte sparats!");
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
                    PathIn = FileIo.ReadFilePath();
                }

                else if (choice == 4)
                {
                    PathOut = FileIo.ReadFilePath(false);
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
            int vaccinationDoses = ValidateInput.ReadInt("Ange nytt antal doser: ");
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
            var DateEighteenYearsAgo = DateOnly.FromDateTime(DateTime.Now).AddYears(-18);

            patients = Patient.AddPersons(input);

            if (patients == null)
            {
                return null;
            }

            for (int i = 0; i < patients.Count; i++)
            {
                if (patients[i].Personnummer > DateEighteenYearsAgo && vaccinateChildren)
                {
                    patients.RemoveAt(i);
                }
            }
            patients = patients.OrderByDescending(p => p.HealthcareWorker)
                .ThenByDescending(p => p.Personnummer)
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
                        person.Personnummer.ToString() +
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
                    break;
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
        public void ExampleTest()
        {
            // Arrange
            var patients = new List<Patient>
    {
        new Patient("Alice", "Andersson", 15, false, false),
        new Patient("Bob", "Bengtsson", 20, false, false),
        new Patient("Charlie", "Carlsson", 17, true, false),
    };

            int doses = 10;
            bool vaccinateMinors = false;

            // Act
            string[] vaccinationOrder = Program.CreateVaccinationOrder(patients, doses, vaccinateMinors);

            // Assert
            // Verify that minors (under 18) are excluded from the vaccination order
            Assert.IsFalse(vaccinationOrder.Any(order => order.Contains("Andersson, Alice")));
            Assert.IsFalse(vaccinationOrder.Any(order => order.Contains("Carlsson, Charlie")));

            // Verify that adults are included in the vaccination order
            Assert.IsTrue(vaccinationOrder.Any(order => order.Contains("Bengtsson, Bob")));

            // Make sure there are no exceptions
            Assert.AreEqual(patients.Count(p => p.Age < 18 && !p.IsHealthcareWorker && !p.IsRiskGroup), 0);
        }
    }
}
