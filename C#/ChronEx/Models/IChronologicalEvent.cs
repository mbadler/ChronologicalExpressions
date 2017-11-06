using System;
using System.Collections.Generic;
using System.Text;

namespace ChronEx.Models
{
    public interface IChronologicalEvent
    {
        string EventName { get;  }
        DateTime EventDateTime { get; }
    }
}
