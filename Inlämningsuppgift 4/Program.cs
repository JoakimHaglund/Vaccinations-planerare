﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
                    Console.WriteLine("Value out of range!");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Value has the wrong format!");
                }
                catch
                {
                    Console.WriteLine("Invalid value!");
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
                    Console.WriteLine("Invalid value!");
                }
            }
        }
    }
    public class FileIo
    {
        public static string ReadFilePath(bool checkFileExist = true)
        {
            while (true)
            {
                Console.Write("Enter fileName: ");
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
                    Console.WriteLine("Mappen finns inte");
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
                Console.WriteLine("ERROR: Directory not found!");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: File not found!");
            }
            catch
            {
                Console.WriteLine("Error in file read");
            }
            return null;
        }
        public static void WriteFile(string path, List<string> lines)
        {
            try
            {
                File.WriteAllLines(path, lines);
                Console.WriteLine("Files Saved to: " + path);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("ERROR: Directory not found!");
            }
            catch
            {
                Console.WriteLine("Error in file write");
            }
        }

    }
    public class Patient
    {
        public string FirstName;
        public string Lastname;
        public int Personnummer;
        public bool HealthcareWorker;
        public bool RiskGroup;
        public bool HasBeenInfected;

        public Patient AddPerson(string input)
        {
            List<string> elements = ParseToList(input);

            if (elements != null && elements.Count == 6)
            {
                string firstName = elements[0];
                string lastName = elements[1];
                string personnummer = elements[2];
                string healthCareWorker = elements[3];
                string riskGroup = elements[4];
                string hasBeenInfected = elements[5];

                return new Patient
                {
                    FirstName = firstName,
                    Lastname = lastName,
                    Personnummer = personnummer,
                    HealthcareWorker = healthcareWorker,
                    RiskGroup = riskGroup,
                    HasBeenInfected = hasBeenInfected
                };
            }
            else
            {
                return null;
            }
            //Konventera till Patient objekt
            //parse to list - ta in en string och spotta ut en lista
            //ParseDate() - på första elementert av listan
            //ParseToBool() - på 3 sista elementen av listan
            //skapa sedan ett nytt Patient
        }
        public int ParseDate(string date)
        {
            //Needs to check year string lenght - if 10 chars -> add year
            //if there's no '-' for last four digits then we need to handle that
            //perhaps return as list so we can seperate the birth date and last 4 digits
        }
        public bool ParseToBool(string input)
        {

        }
        public List<string> ParseToList(string input)
        {

            string[] elements = input.Split(',');

            if (elements.Length >= 6)
            {
                return new List<string>(elements);
            }
            else
            {
                return null;
            }
        }
    }
    public class Program
    {
        private static List<Patient> patients = new List<Patient>();
        private static string csvPath = @"C:\Windows\Temp\Contacts.csv";
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
                //Skapa prioritetsordning
                if (choice == 0)
                {
                    List<string> k = FileIo.ReadFile(PathIn);
                    FileIo.WriteFile(PathOut, k);
                }
                //Ändra antal vaccindoser
                else if (choice == 1)
                {
                    AvailableVaccineDoses = ChangeVaccinationDoses();
                }
                //Ändra åldersgräns
                else if (choice == 2)
                {
                    VaccinateMinors = ChangeVaccinationSetting();
                }
                //Ändra indatafil
                else if (choice == 3)
                {
                    PathIn = FileIo.ReadFilePath();
                }
                //Ändra utdatafil
                else if (choice == 4)
                {
                    PathOut = FileIo.ReadFilePath(false);
                }
                //Avsluta
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Hejdå!");
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
            List<string> options = new List<string> { "Ja", "Nej" };
            int input = ShowMenu("Ska personer under 18 år vaccineras? (Ja/Nej)", options);

            return input == 0;

        }
        // Create the lines that should be saved to a CSV file after creating the vaccination order.
        //
        // Parameters:
        //
        // input: the lines from a CSV file containing population information
        // doses: the number of vaccine doses available
        // vaccinateChildren: whether to vaccinate people younger than 18
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            // Replace with your own code.
            //create list of patient objects
            //use patient list to create vaccine order
            // turn it into a list of csv strings

            return new string[0];
        }

        public static int ShowMenu(string prompt, IEnumerable<string> options)
        {
            if (options == null || options.Count() == 0)
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
            string[] input =
            {
                "19720906-1111,Elba,Idris,0,0,1",
                "8102032222,Efternamnsson,Eva,1,1,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(output.Length, 2);
            Assert.AreEqual("19810203-2222,Efternamnsson,Eva,2", output[0]);
            Assert.AreEqual("19720906-1111,Elba,Idris,1", output[1]);
        }
    }
}
