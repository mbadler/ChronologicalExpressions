using System;
using System.Collections.Generic;
using System.Text;

namespace ChronEx.Models.AST
{
    public class NegatedElement: ContainerElement
    {
       

        internal override IsMatchResults IsMatch(IChronologicalEvent chronevent)
        {
            switch (ContainedElement.IsMatch(chronevent))
            {
                case IsMatchResults.IsMatch:
                    {
                        return IsMatchResults.IsNotMatch;
                         
                    }
                case IsMatchResults.IsNotMatch:
                    {
                        return IsMatchResults.IsMatch;
                    }

                case IsMatchResults.Continue:
                    {
                        return IsMatchResults.Continue;
                    }
                default:
                    return IsMatchResults.Continue;
            }
        }

        internal override bool IsPotentialMatch(IChronologicalEvent chronevent)
        {
            return !ContainedElement.IsPotentialMatch(chronevent);
        }
    }
}
