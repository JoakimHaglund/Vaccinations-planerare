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
