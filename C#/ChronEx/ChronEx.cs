using ChronEx.Models;
using ChronEx.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChronEx
{
    public class ChronEx
    {
        public ChronEx()
        {
        }

        public static bool IsMatch(string ChronExStatment,IEnumerable<IChronologicalEvent> Events)
        {
            //get a prser and generate the parsed tree
            var prs = new ChronExParser();
            var tree = prs.ParsePattern(ChronExStatment);
            //create a runner and run the statment and events
            var runner = new Runner(tree, Events);
            return runner.IsMatch();
        }

        public static int MatchCount(string ChronExStatment, IEnumerable<IChronologicalEvent> Events)
        {
            //get a prser and generate the parsed tree
            var prs = new ChronExParser();
            var tree = prs.ParsePattern(ChronExStatment);
            //create a runner and run the statment and events
            var runner = new Runner(tree, Events);
            return runner.MatchCount();
        }

        
    }
}
