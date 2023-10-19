using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICalendar
{
    public class ICalendar
    {
        public string ProdId;
        public List<ICalendarEvent> Events = new List<ICalendarEvent>();
        public DateTime Stamp;
        public DateTime CurrentEvent;
        private Random randomNum = new Random();
        public void CreateEvent(DateTime start, DateTime end, string summary)
        {
            Events.Add(new ICalendarEvent
            {
                Uid = DateTime.Now.ToString("MMddhhmmss") + randomNum.Next(0, 100) + "@Bat.Is.Back.On.The.Menu",
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
            output.Add("PRODID:-//hacksw/handcal//NONSGML v1.0//EN");//Ändra till vår id
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
