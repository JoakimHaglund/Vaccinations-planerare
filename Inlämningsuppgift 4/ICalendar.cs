using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaccination;

namespace Icalendar
{
    public class ICalendar
    {
        private readonly string ProdId = "BatSoupCure";
        private DateTime currentEvent;
        private TimeOnly startOfWorkDay;//StartTid
        private TimeOnly endOfWorkDay;//SlutTid
        private int duration;//Minutes per event
        private int attendants;//Num of events at a time
        public ICalendar(DateOnly? startDate, TimeOnly? startOfDay, TimeOnly? endOfday, int eventDuration = 5, int eventAttendants = 2)
        {
            startOfWorkDay = (TimeOnly)(startOfDay ?? new TimeOnly(08, 00));
            endOfWorkDay = (TimeOnly)(endOfday ?? new TimeOnly(20, 00));

            var defaultDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);
            var tempDate = (DateOnly)(startDate ?? defaultDate);
            currentEvent = tempDate.ToDateTime(startOfWorkDay);

            duration = eventDuration;
            attendants = eventAttendants;
        }
        private ICalendarEvent CreateEvent(DateTime start, DateTime end, int count, string summary)
        {
            return new ICalendarEvent
            {
                Uid = DateTime.Now.ToString("MMmmddsshh") + count + "@BatIsBack.OnTheMenu",
                EventStart = start,
                EventEnd = end,
                Summary = summary
            };
        }
        public List<ICalendarEvent> CreateEvents(string[] input)
        {
            int count = 0;
            var output = new List<ICalendarEvent>();
            var strings = new List<string>();
            string summary = "";
            for (int i = 0; i < input.Length; i++)
            {

                string[] stringBuilder = input[i].Split(',');
                //Format personnummer for better readabillity
                string personnummer = stringBuilder[0].Substring(0, 4) + " " + stringBuilder[0].Substring(4, 2) + " " + stringBuilder[0].Substring(6);
                summary += "Namn: " + stringBuilder[2] + " " + stringBuilder[1] + ". Prn: " + personnummer;

                if ((i + 1) % attendants == 0 || i == input.Length - 1)
                {
                    strings.Add(summary);
                    summary = "";
                }
                else
                {
                    summary += "\\n";
                }
            }
            foreach (string inputItem in strings)
            {
                var endOfEvent = currentEvent.AddMinutes(duration);
                if (endOfEvent.Hour > endOfWorkDay.Hour || endOfEvent.Hour == endOfWorkDay.Hour && endOfEvent.Minute > endOfWorkDay.Minute)
                {
                    //Get hours left of day
                    int HoursLeftInDay = 24 - currentEvent.Hour;
                    //Set starting hour and minute of next day
                    currentEvent = currentEvent.AddHours(HoursLeftInDay + startOfWorkDay.Hour);
                    currentEvent = currentEvent.AddMinutes(-currentEvent.Minute + startOfWorkDay.Minute);

                    endOfEvent = currentEvent.AddMinutes(duration);
                }
                output.Add(CreateEvent(currentEvent, endOfEvent, count, inputItem));
                currentEvent = endOfEvent;
                count++;
            }

            return output;
        }
        public string[] CreateCalendarOutput(List<ICalendarEvent> Events)
        {
            var output = new List<string>();
            string DateTimeFormat = "yyyyMMddTHHmmss";
            output.Add("BEGIN:VCALENDAR");
            output.Add("VERSION:2.0");
            output.Add("PRODID:" + ProdId);
            foreach (var evnt in Events)
            {
                output.Add("BEGIN:VEVENT");
                output.Add("UID:" + evnt.Uid.Trim());
                output.Add("DTSTAMP:" + DateTime.Now.ToString(DateTimeFormat));
                output.Add("DTSTART:" + evnt.EventStart.ToString(DateTimeFormat));
                output.Add("DTEND:" + evnt.EventEnd.ToString(DateTimeFormat));
                output.Add("SUMMARY: Cure chink bat disease");
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
            TimeOnly? endTime = ValidateData.ReadTime("Sluttid: ");
            int attendents = ValidateData.ReadInt("Antal samtidiga vaccinationer: ");
            int duration = ValidateData.ReadInt("Minuter per vaccination: ");
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
        public void test()
        {
            var myEventPlanner = new ICalendar(null, null, null, 60, 10);
            // Arrange health / risk / infection
            string[] input =
            {
                "20100101-2222,lolson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,Sven,2",
                "20100101-2222,Svennson,henry,2"
            };


            List<ICalendarEvent> myEvents = myEventPlanner.CreateEvents(input);

            // Act
            string[] output = myEventPlanner.CreateCalendarOutput(myEvents);
            FileIo.WriteFile("E:\\ffswindows\\Desktop\\testttt.ics", output);
            foreach (string ou in output)
            {
                Console.WriteLine(ou);
            }
            // Assert
            Assert.AreEqual(2, output.Length);
            Assert.AreEqual("20000101-1111,Svennson,Bob,2", output[0]);
            Assert.AreEqual("20100101-2222,Svennson,Sven,2", output[1]);
        }

    }
}
