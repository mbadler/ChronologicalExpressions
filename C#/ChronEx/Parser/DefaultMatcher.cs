using System;
using System.Collections.Generic;
using System.Text;
using ChronEx.Models;
using ChronEx.Models.AST;

namespace ChronEx.Parser
{
    public class DefaultMatcher : MatcherBase
    {
        public override bool IsMatch(IChronologicalEvent Chronevent, Element ElementToMatch)
        {
            return ElementToMatch.IsMatch(Chronevent);
        }
    }
}
