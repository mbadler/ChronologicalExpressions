using ChronEx.Models;
using ChronEx.Models.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChronEx.Parser
{
    public abstract class MatcherBase
    {
        public abstract bool IsMatch(IChronologicalEvent Chronevent,Element Matcher);
    }
}
