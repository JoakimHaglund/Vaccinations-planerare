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
        private readonly string ProdId = "BatSoupCure";
        public DateTime Stamp;
        private DateTime currentEvent;
        private TimeOnly startOfWorkDay;//StartTid
        private TimeOnly endOfWorkDay;//SlutTid
        private int duration;//Minutes per event
        private int attendants;//Num of events at a time
        public ICalendar(DateOnly? startDate, TimeOnly? startTime, TimeOnly? endTime, int eventTime = 5, int eventsAtATime = 2)
        {
            DateOnly tempDate = (DateOnly)(startDate != null ? startDate : DateOnly.FromDateTime(DateTime.Now).AddDays(7));
            startOfWorkDay = (TimeOnly)(startTime != null ? startTime : new TimeOnly(08, 00));
            currentEvent = tempDate.ToDateTime(startOfWorkDay);

            endOfWorkDay = (TimeOnly)(endTime != null ? endTime : new TimeOnly(20, 00)); 
            duration = eventTime;
            attendants = eventsAtATime;
        }
        public ICalendarEvent CreateEvent(DateTime start, DateTime end, int count, string summary)
        {
            return new ICalendarEvent
            {
                Uid = DateTime.Now.ToString("MMmmddsshh") + count + "@BatIsBack.OnTheMenu",
                Stamp = DateTime.Now,
                EventStart = start,
                EventEnd = end,
                Summary = summary
            };

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
