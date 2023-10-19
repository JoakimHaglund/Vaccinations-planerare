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
