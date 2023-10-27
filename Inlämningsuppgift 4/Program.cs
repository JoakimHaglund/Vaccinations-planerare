using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Icalendar;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Runtime.ConstrainedExecution;

namespace Vaccination
{
    public class ValidateData
    {
        public static int? ReadInt(string question, bool acceptNull = false)
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
                catch 
                {
                    if (acceptNull)
                    {
                        return null;
                    }

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

            isNull = string.IsNullOrEmpty(patient.Personnummer);
            isNull = string.IsNullOrEmpty(patient.FirstName);
            isNull = string.IsNullOrEmpty(patient.LastName);

            isNull = patient.HealthcareWorker == null;
            isNull = patient.RiskGroup == null;
            isNull = patient.HasBeenInfected == null;
            isNull = patient.BirthDate == null;

            if (isNull)
            {
                return null;
            }
            else
            {
                return patient;
            }
        }
        public static TimeOnly? ReadTime(string question, TimeOnly? CanNotBeLessThan = null)
        {
            while (true)
            {
                Console.Write(question);
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) return null;

                try
                {
                    var output = TimeOnly.ParseExact(input, "HH:mm");
                    if (CanNotBeLessThan != null && output < CanNotBeLessThan)
                    {
                        throw new ArgumentException("Input value is less than required value");
                    }
                    return output;
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Sluttiden är lägre än starttiden!");
                }
                catch 
                {
                    Console.WriteLine("Felaktigt format! (HH:MM)");
                } 
            }
        }
        public static DateOnly? ReadDate(string question)
        {
            while (true)
            {
                Console.Write(question);
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) return null;

                try
                {
                    var output = DateOnly.ParseExact(input, "yyyy-MM-dd");
                    return output;
                }
                catch
                {
                    Console.WriteLine("Felaktigt format! (År - Månad - Dag)");
                }
            }
        }
        public static string TrimAtLastChar(string input, char chr)
        {
            int last = input.LastIndexOf(chr);
            if (last != -1)
            {
                return input.Substring(0, last);
            }
            else
            {
                return input;
            }
        }
    }
    public class FileIo
    {
        public static string ReadFilePath(string fileFormat, bool checkFileExist = true, string prompt = "Ange filnamn: ")
        {
            while (true)
            {
                
                Console.Write(prompt);
                string path = Console.ReadLine();
                string directoryPath = "";

                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }
                
                path = ValidateData.TrimAtLastChar(path, '.') + "." + fileFormat;
                
                int lastSlash = path.LastIndexOf('\\');
                if (path.Contains("\\"))
                {
                    directoryPath = ValidateData.TrimAtLastChar(path, '\\');
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
        public static void WriteFile(string path, string[] input)
        {
            bool shouldWrite = true;
            if (input != null && File.Exists(path))
            {
                shouldWrite = Program.AskYesNoQuestion($"{path} finns redan! Vill du skriva över filen?");
            }
            if (input == null || !shouldWrite)
            {
                Console.WriteLine("Resultatet har inte sparats!");
            }
            else
            {
                try
                {
                    File.WriteAllLines(path, input);
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
    }
    public class Parse
    {
        public static List<string> Personnummer(string date)
        {
            var personnummer = new List<string>();
            int numOfHypens = date.Count(c => (c == '-'));

            if (numOfHypens > 1)
            {
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

            if (personnummer[0].Length == 6)
            {
                personnummer[0] = "19" + personnummer[0];
            }
            else if(personnummer[0].Length < 6 || personnummer[0].Length > 8)
            {
                return null;
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
        public DateOnly? BirthDate;
        public string Personnummer;
        public string FirstName;
        public string LastName;
        public bool? HealthcareWorker;
        public bool? RiskGroup;
        public bool? HasBeenInfected;

        public static Patient AddPatient(string input)
        {
            List<string> elements = Parse.ToList(input);

            if (elements != null && elements.Count == 6)
            {
                var personnummer = Parse.Personnummer(elements[0]);

                return new Patient
                {
                    BirthDate = ValidateData.Date(personnummer[0]),
                    Personnummer = personnummer[0] + "-" + personnummer[1],
                    FirstName = elements[2],
                    LastName = elements[1],
                    HealthcareWorker = Parse.ToBool(elements[3]),
                    RiskGroup = Parse.ToBool(elements[4]),
                    HasBeenInfected = Parse.ToBool(elements[5])
                };
            }
            else
            {
                return null;
            }
        }
        public static List<Patient> AddPatients(string[] input)
        {
            var patients = new List<Patient>();
            bool abort = false;

            for (int i = 0; i < input.Length; i++)
            {
                var patient = ValidateData.CheckForNull(AddPatient(input[i]));

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
                Console.Clear();
                Console.WriteLine("Huvudmeny");
                Console.WriteLine("---------");
                Console.WriteLine("Antal tillgängliga vaccindoser: " + AvailableVaccineDoses);
                Console.WriteLine("Vaccinering under 18 år: " + (VaccinateMinors ? "Ja" : "Nej"));
                Console.WriteLine("Indatafil: " + PathIn);
                Console.WriteLine("Utdatafil: " + PathOut);

                int choice = ShowMenu("Vad vill du göra?", new List<string>
                {
                    "Skapa prioritetsordning",
                    "Schemalägg vaccinationer",
                    "Ändra antal vaccindoser",
                    "Ändra åldersgräns",
                    "Ändra indatafil",
                    "Ändra utdatafil",
                    "Avsluta"
                });
                
                Console.WriteLine();

                if (choice == 0)
                {
                    CreateAndSaveVaccinationOrder(PathIn, PathOut, AvailableVaccineDoses, VaccinateMinors);
                    Console.ReadLine();
                }
                else if (choice ==  1)
                {
                    CreateAndSaveVaccinationCalendar(PathIn, AvailableVaccineDoses, VaccinateMinors);
                }
                else if (choice == 2)
                {
                    AvailableVaccineDoses = ChangeVaccinationDoses();
                }
                else if (choice == 3)
                {
                    VaccinateMinors = ChangeVaccinationSetting();
                }
                else if (choice == 4)
                {
                    string temp = FileIo.ReadFilePath("csv");
                    if (temp != null)
                    {
                        PathIn = temp;
                    }
                }
                else if (choice == 5)
                {
                    string temp = FileIo.ReadFilePath("csv",false);
                    if (temp != null)
                    {
                        PathOut = temp;
                    }
                }
                else
                {
                    Console.WriteLine("Live long and prosper!");
                    break;
                }
            }
        }
        public static void CreateAndSaveVaccinationCalendar(string pathIn, int vaccineDoses, bool vaccinateMinors)
        {
            List<string> input = FileIo.ReadFile(pathIn);
            if (input != null)
            {
                string[] ordredInput = CreateVaccinationOrder(input.ToArray(), vaccineDoses, vaccinateMinors);

                var calendar = ICalendar.GetUserInput();
                string path = FileIo.ReadFilePath("ics", false, "Kalenderfil: ");

                var calendarEvents = calendar.CreateEvents(ordredInput);
                var output = calendar.CreateCalendarOutput(calendarEvents);
                FileIo.WriteFile(path, output);
            }
        }
        public static void CreateAndSaveVaccinationOrder(string pathIn, string pathOut, int vaccineDoses, bool vaccinateMinors)
        {
            List<string> input = FileIo.ReadFile(pathIn);
            if(input != null)
            {
                string[] output = CreateVaccinationOrder(input.ToArray(), vaccineDoses, vaccinateMinors);
                FileIo.WriteFile(pathOut, output);
            }
        }
        public static int ChangeVaccinationDoses()
        {
            Console.WriteLine("Ändra antal vaccindoser");
            Console.WriteLine("-----------------------");
            int vaccinationDoses = (int)ValidateData.ReadInt("Ange nytt antal doser: ");
            return vaccinationDoses;
        }
        public static bool ChangeVaccinationSetting()
        {
            bool newSetting = AskYesNoQuestion("Ska personer under 18 år vaccineras?");
            return newSetting;
        }
        public static bool AskYesNoQuestion(string question)
        {
            var options = new List<string> { "Ja", "Nej" };
            int input = ShowMenu(question, options);

            return input == 0;
        }
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            var vaccinationOrder = new List<string>();
            var patients = new List<Patient>();

            var dateNow = DateOnly.FromDateTime(DateTime.Now);
            var dateEighteenYearsAgo = dateNow.AddYears(-18);

            patients = Patient.AddPatients(input);

            if (patients == null)
            {
                return null;
            }

            for (int i = 0; i < patients.Count; i++)
            {
                if (patients[i].BirthDate > dateEighteenYearsAgo && !vaccinateChildren)
                {
                    patients.RemoveAt(i);
                }
            }

            patients = patients.OrderByDescending(p => p.HealthcareWorker)
                .ThenBy(p => p.BirthDate >= dateNow.AddYears(-65))
                .ThenByDescending(p => p.RiskGroup)
                .ThenBy(p => p.BirthDate)
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
                        person.Personnummer +
                        "," +
                        person.LastName +
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
        public void SortByAge()
        {
            // Arrange health / risk / infection
            string[] input =
            {
                "19580101-1111,Svennson,Tor,0,0,0",
                "19590101-1111,Svennson,Erik,0,0,0",
                "19500101-1111,Svennson,Bob,0,0,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(3, output.Length);
            Assert.AreEqual("19500101-1111,Svennson,Bob,2", output[0]);
            Assert.AreEqual("19580101-1111,Svennson,Tor,2", output[1]);
            Assert.AreEqual("19590101-1111,Svennson,Erik,2", output[2]);
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
        public void SortByAgeOver65ThenByRiskGroup()
        {
            // Arrange health / risk / infection
            string[] input =
            {
                "19650101-1111,Svennson,Tor,0,1,0",
                "19590101-1111,Svennson,Erik,0,0,0",
                "19500101-1111,Svennson,Bob,0,0,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(3, output.Length);
            Assert.AreEqual("19500101-1111,Svennson,Bob,2", output[0]);
            Assert.AreEqual("19650101-1111,Svennson,Tor,2", output[1]);
            Assert.AreEqual("19590101-1111,Svennson,Erik,2", output[2]);

        }
        [TestMethod]
        public void SortHealtcareWorkerByAge()
        {
            // Arrange health / risk / infection
            string[] input =
            {
                "19520101-1111,Svennson,Tor,1,0,0",
                "19540101-1111,Svennson,Erik,0,0,0",
                "19560101-1111,Svennson,Bob,1,0,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(3, output.Length);
            Assert.AreEqual("19520101-1111,Svennson,Tor,2", output[0]);
            Assert.AreEqual("19560101-1111,Svennson,Bob,2", output[1]);
            Assert.AreEqual("19540101-1111,Svennson,Erik,2", output[2]);
        }
        [TestMethod]
        public void NotEnoughVaccineDoses()
        {
            // Arrange health / risk / infection
            string[] input =
            {
                "19650101-1111,Svennson,Tor,0,0,0",
                "19660101-1111,Svennson,Erik,0,0,1",
                "19670101-1111,Svennson,Bob,0,0,0"
            };
            int doses = 4;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(2, output.Length);
            Assert.AreEqual("19650101-1111,Svennson,Tor,2", output[0]);
            Assert.AreEqual("19660101-1111,Svennson,Erik,1", output[1]);
        }
        [TestMethod]
        public void CheckPrioritizationOrder()
        {
            // Arrange health / risk / infection
            string[] input =
            {
                "19500101-2222,Andersson,Mia,0,0,0", //Pension
                "20000101-1111,Svennson,Bob,0,0,0", //nothing special 
                "20010101-2222,Nilsson,Mikael,1,0,0", //healtcare worker
                "20010101-1111,Mikaelsson,Kerstin,0,1,0", //risk
                "20150101-1111,Kalin,Jakob,1,1,0", //child worker
            };
            int doses = 100;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(4, output.Length);
            Assert.AreEqual("20010101-2222,Nilsson,Mikael,2", output[0]);
            Assert.AreEqual("19500101-2222,Andersson,Mia,2", output[1]);
            Assert.AreEqual("20010101-1111,Mikaelsson,Kerstin,2", output[2]);
            Assert.AreEqual("20000101-1111,Svennson,Bob,2", output[3]);
        }
    }
}
