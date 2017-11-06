using System;
using System.Collections.Generic;
using System.Text;

namespace ChronEx.Models.AST
{
    public abstract class Selector:Element
    {
    }

    public class SpecifiedEventNameSelector:Selector
    {
        public string EventName { get; set; }

        internal override IsMatchResults IsMatch(IChronologicalEvent chronevent)
        {
           return (string.Compare(chronevent.EventName, this.EventName, true) == 0)
                ? IsMatchResults.IsMatch : IsMatchResults.IsNotMatch;
        }

        internal override bool IsPotentialMatch(IChronologicalEvent chronevent)
        {
            //will always be the same as full match
            return IsMatch(chronevent) != IsMatchResults.IsNotMatch;
        }
    }

    public class dotSelector : Selector
    {
        //. matches everything so its always true
        internal override IsMatchResults IsMatch(IChronologicalEvent chronevent)
        {
            return IsMatchResults.IsMatch;
        }

        internal override bool IsPotentialMatch(IChronologicalEvent chronevent)
        {
            //. matches everything so its always true
            return true;
        }
    }

}
