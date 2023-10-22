using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaccination;

namespace ICalendar
{
    public class ICalendar
    {
        private string ProdId = "BatSoupCure";
        public List<ICalendarEvent> Events = new List<ICalendarEvent>();
        public DateTime Stamp;
        private DateTime currentEvent;
        private Random randomNum = new Random();
        private TimeOnly startOfWorkDay;//StartTid
        private TimeOnly endOfWorkDay;//SlutTid
        private int timePerEvent;//Minutes per event
        private int simultaneousEvents;//Num of events at a time
        public ICalendar(DateTime? startDateTime = null, TimeOnly endTime, int eventTime = 5, int eventsAtATime = 2)
        {
            currentEvent = (DateTime)(startDateTime != null ? startDateTime : DateTime.Now);
            startOfWorkDay = TimeOnly.FromDateTime(currentEvent);
            endOfWorkDay = endTime;
            timePerEvent = eventTime;
            simultaneousEvents = eventsAtATime;
        }
        public void CreateEvent(DateTime start, DateTime end, string summary)
        {
            Events.Add(new ICalendarEvent
            {
                Uid = DateTime.Now.ToString("MMmmddsshh") + randomNum.Next(100, 999) + "@BatIsBack.OnTheMenu",
                Stamp = DateTime.Now,
                EventStart = start,
                EventEnd = end,
                Summary = summary
            });

        }
        public List<ICalendarEvent> CreateEvents(string[] input)
        {
            // skapa event
            return null;
        }
        public string[] CreateCalendarOutput()
        {
            var output = new List<string>();
            string DateTimeFormat = "yyyyMMddThhmmss";
            output.Add("BEGIN:VCALENDAR");
            output.Add("VERSION:2.0");
            output.Add(ProdId);
            foreach (var evnt in Events)
            {//Datum hantering behöves
                output.Add("BEGIN:VEVENT");
                output.Add("UID:" + evnt.Uid);
                output.Add("DTSTAMP:" + evnt.Stamp.ToString(DateTimeFormat));
                output.Add("DTSTART:" + evnt.EventStart.ToString(DateTimeFormat));
                output.Add("DTEND:" + evnt.EventEnd.ToString(DateTimeFormat));
                output.Add("SUMMARY:" + evnt.Summary);
                output.Add("END:VEVENT");
            }
            output.Add("END:VCALENDAR");
            return output.ToArray();
        }
        public void GetUserInput()
        {
            DateOnly? startDate = ValidateData.ReadDate("tartdatum(YYYY-MM-DD) :");
            TimeOnly? startTime = ValidateData.ReadTime("Starttid");
            TimeOnly? endTime = ValidateData.ReadTime("Sluttid");
            ValidateData.ReadInt("Antal samtidiga vaccinationer:");
            ValidateData.ReadInt("Minuter per vaccination:");
            Console.WriteLine("Kalenderfil:");
            FileIo.ReadFilePath(true);
        }

    }
    public struct ICalendarEvent
    {
        public string Uid;
        public DateTime Stamp;
        public DateTime EventStart;
        public DateTime EventEnd;
        public string Summary;
    }
   
}
