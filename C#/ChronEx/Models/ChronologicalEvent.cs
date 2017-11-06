using System;
using System.Collections.Generic;
using System.Text;

namespace ChronEx.Models
{
    public class ChronologicalEvent : IChronologicalEvent
    {
        public string EventName { get; set; }
        public DateTime EventDateTime { get; set; }
    }
}
