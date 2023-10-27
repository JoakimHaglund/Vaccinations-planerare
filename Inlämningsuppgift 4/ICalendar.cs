using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaccination;

namespace Icalendar
{
    public class ICalendar
    {
        private readonly string ProdId = "VaccinationBooking";
        public DateTime CurrentEvent { get; private set; }
        public TimeOnly StartOfWorkDay { get; private set; }
        public TimeOnly EndOfWorkDay { get; private set; }
        public int Duration { get; private set; }
        public int Attendants { get; private set; }
        public ICalendar(DateOnly? startDate, TimeOnly? startOfDay, TimeOnly? endOfday, int? eventDuration, int? eventAttendants)
        {
            StartOfWorkDay = (TimeOnly)(startOfDay ?? new TimeOnly(08, 00));
            EndOfWorkDay = (TimeOnly)(endOfday ?? new TimeOnly(20, 00));

            var defaultDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);
            var tempDate = (DateOnly)(startDate ?? defaultDate);
            CurrentEvent = tempDate.ToDateTime(StartOfWorkDay);

            //if the value is nonsens it's set to minimum viable value
            //and if the value is null it's set to the default value
            Duration = (eventDuration < 0) ? 0 : eventDuration ?? 5;
            Attendants = (eventAttendants < 1) ? 1 : eventAttendants ?? 2;
        }
        private ICalendarEvent CreateEvent(DateTime start, DateTime end, int count, string summary)
        {
            return new ICalendarEvent
            {
                Uid = DateTime.Now.ToString("MMmmddsshh") + count + "@BestVaccinationBooking.Program",
                EventStart = start,
                EventEnd = end,
                Summary = summary
            };
        }
        public List<ICalendarEvent> CreateEvents(string[] input)
        {
            int count = 0;
            var output = new List<ICalendarEvent>();
            var summaries = new List<string>();
            string summary = "";

            if (input == null)
            {
                return null;
            }

            for (int i = 0; i < input.Length; i++)
            {
                string[] values = input[i].Split(',');

                if (values.Length != 4)
                {
                    return null;
                }

                try
                {
                    //Format personnummer for better readabillity (Format: YYYY MM DD XXXX)
                    string personnummer = values[0].Substring(0, 4) + " " + 
                        values[0].Substring(4, 2) + " " + values[0].Substring(6);

                    summary += values[2] + " " + values[1] + " - " + personnummer;
                }
                catch 
                {
                    return null;
                }

                if ((i + 1) % Attendants == 0 || i == input.Length - 1)
                {
                    summaries.Add(summary);
                    summary = "";
                }
                else
                {
                    summary += "\\n";
                }
            }
            foreach (string inputItem in summaries)
            {
                var endOfEvent = CurrentEvent.AddMinutes(Duration);
                bool overEndOfWorkMinute = endOfEvent.Hour == EndOfWorkDay.Hour && endOfEvent.Minute > EndOfWorkDay.Minute;
                if (endOfEvent.Hour > EndOfWorkDay.Hour || overEndOfWorkMinute)
                {
                    //Get hours left of day
                    int HoursLeftInDay = 24 - CurrentEvent.Hour;
                    //Set starting hour and minute of next day
                    CurrentEvent = CurrentEvent.AddHours(HoursLeftInDay + StartOfWorkDay.Hour);
                    CurrentEvent = CurrentEvent.AddMinutes(-CurrentEvent.Minute + StartOfWorkDay.Minute);

                    endOfEvent = CurrentEvent.AddMinutes(Duration);
                }
                output.Add(CreateEvent(CurrentEvent, endOfEvent, count, inputItem));
                CurrentEvent = endOfEvent;
                count++;
            }

            return output;
        }
        public string[] CreateCalendarOutput(List<ICalendarEvent> Events)
        {
            var output = new List<string>();
            string DateTimeFormat = "yyyyMMddTHHmmss";

            if (Events == null)
            {
                return null;
            }

            output.Add("BEGIN:VCALENDAR");
            output.Add("VERSION:2.0");
            output.Add("PRODID:" + ProdId);
            foreach (var evnt in Events)
            {
                output.Add("BEGIN:VEVENT");
                output.Add("UID:" + evnt.Uid);
                output.Add("DTSTAMP:" + DateTime.Now.ToString(DateTimeFormat));
                output.Add("DTSTART:" + evnt.EventStart.ToString(DateTimeFormat));
                output.Add("DTEND:" + evnt.EventEnd.ToString(DateTimeFormat));
                output.Add("SUMMARY:Vaccination");
                output.Add("DESCRIPTION:" + evnt.Summary.Trim());
                output.Add("END:VEVENT");
            }
            output.Add("END:VCALENDAR");
            return output.ToArray();
        }
        public static ICalendar GetUserInput()
        {
            Console.WriteLine("Schemalägg vaccinationer");
            Console.WriteLine("------------------------");
            Console.WriteLine("Mata in blankrad för att välja standardvärde.\n");

            DateOnly? startDate = ValidateData.ReadDate("Startdatum (YYYY-MM-DD): ");
            TimeOnly? startTime = ValidateData.ReadTime("Starttid: ");
            TimeOnly? endTime = ValidateData.ReadTime("Sluttid: ", startTime);
            int? attendents = ValidateData.ReadInt("Antal samtidiga vaccinationer: ", true);
            int? duration = ValidateData.ReadInt("Minuter per vaccination: ", true);

            //Get valid TimeOnlys to be able to compare lenght of workshift with an event duration
            var tempStartTime = (TimeOnly)(startTime ?? new TimeOnly(08, 00));
            var tempStopTime = (TimeOnly)(startTime ?? new TimeOnly(20, 00));

            int possibleLenght = (int)(tempStopTime - tempStartTime).TotalMinutes;
            if (duration != null && duration > possibleLenght)
            {
                Console.WriteLine("Vaccinationen tar längre tid än arbetstiden!");
                duration = ValidateData.ReadInt("Minuter per vaccination: ", true);
            }

            return new ICalendar(startDate, startTime, endTime, duration, attendents);
        }

    }
    public struct ICalendarEvent
    {
        public string Uid;
        public DateTime EventStart;
        public DateTime EventEnd;
        public string Summary;
    }

    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void NullInput()
        {
            var myEventPlanner = new ICalendar(null, null, null, null, null);
            string[] input = null;
            
            List<ICalendarEvent> output = myEventPlanner.CreateEvents(input);

            // Assert
            Assert.AreEqual(null, output);
        }

        [TestMethod]
        public void ExpectedUsage()
        {
            var date = new DateOnly(2020, 10, 10);
            var now = DateTime.Now;
            var myEventPlanner = new ICalendar(date, null, null, 60, 1);

            string[] input =
            {
                "20100101-2222,Exempelson,Sven,2",
                "20100101-2222,Exempelson,Sven,2"
            };
            var expectedDateTime = new DateTime(2020, 10, 10, 8, 0, 0);
            List<ICalendarEvent> expectedOutput = new List<ICalendarEvent>
            {
                new ICalendarEvent
                {
                    Uid = now.ToString("MMmmddsshh") + 0 + "@BestVaccinationBooking.Program",
                    EventStart = expectedDateTime,
                    EventEnd = expectedDateTime.AddMinutes(60),
                    Summary = "Patient: Sven Exempelson - 2010 01 01-2222"
                },
                new ICalendarEvent
                {
                    Uid = now.ToString("MMmmddsshh") + 1 + "@BestVaccinationBooking.Program",
                    EventStart = expectedDateTime.AddMinutes(60),
                    EventEnd = expectedDateTime.AddMinutes(120),
                    Summary = "Patient: Sven Exempelson - 2010 01 01-2222"
                }
            };
            List<ICalendarEvent> output = myEventPlanner.CreateEvents(input);

            // Assert
            Assert.AreEqual(2, output.Count);
            for (int i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(expectedOutput[i].EventStart, output[i].EventStart);
                Assert.AreEqual(expectedOutput[i].EventEnd, output[i].EventEnd);
                Assert.AreEqual(expectedOutput[i].Uid, output[i].Uid);
                Assert.AreEqual(expectedOutput[i].Summary, output[i].Summary);
            }
        }

        [TestMethod]
        public void MissingInputValue()
        {
            var date = new DateOnly(2020, 10, 10);
            var now = DateTime.Now;
            var myEventPlanner = new ICalendar(date, null, null, 60, 1);

            string[] input =
            {
                ",Exempelson,Sven,2",
                "20100101-2222,,Sven,2"
            };
            var expectedDateTime = new DateTime(2020, 10, 10, 8, 0, 0);
           
            List<ICalendarEvent> output = myEventPlanner.CreateEvents(input);

            // Assert
            Assert.AreEqual(null, output);
        }
        [TestMethod]
        public void WrongInputLenght()
        {
            var date = new DateOnly(2020, 10, 10);
            var now = DateTime.Now;
            var myEventPlanner = new ICalendar(date, null, null, 60, 1);

            string[] input =
            {
                "Exempelson,Sven,2"
            };

            List<ICalendarEvent> output = myEventPlanner.CreateEvents(input);

            // Assert
            Assert.AreEqual(null, output);
        }

        [TestMethod]
        public void DefaultValues()
        {
            var testEventPlanner = new ICalendar(null, null, null, null, null);

            var dateTimeNow = DateTime.Now;
            var expectedDateTime = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 8, 0, 0);
            expectedDateTime = expectedDateTime.AddDays(7);

            Assert.AreEqual(expectedDateTime, testEventPlanner.CurrentEvent);
            Assert.AreEqual(new TimeOnly(8, 0), testEventPlanner.StartOfWorkDay);
            Assert.AreEqual(new TimeOnly(20, 0), testEventPlanner.EndOfWorkDay);
            Assert.AreEqual(5 , testEventPlanner.Duration);
            Assert.AreEqual(2, testEventPlanner.Attendants);
        }

        [TestMethod]
        public void InvalidIntValues()
        {
            //Note: This does not check invalid dates as they will crash before you can give it to the class 
            var testEventPlanner = new ICalendar(null, null, null, -1, 0);

            var dateTimeNow = DateTime.Now;
            var expectedDateTime = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, 8, 0, 0);
            expectedDateTime = expectedDateTime.AddDays(7);

            Assert.AreEqual(expectedDateTime, testEventPlanner.CurrentEvent);
            Assert.AreEqual(new TimeOnly(8, 0), testEventPlanner.StartOfWorkDay);
            Assert.AreEqual(new TimeOnly(20, 0), testEventPlanner.EndOfWorkDay);
            Assert.AreEqual(0, testEventPlanner.Duration);
            Assert.AreEqual(1, testEventPlanner.Attendants);
        }

    }
}
