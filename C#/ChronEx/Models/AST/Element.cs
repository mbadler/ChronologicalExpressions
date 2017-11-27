using ChronEx.Parser;
using ChronEx.Processor;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChronEx.Models.AST
{
    public abstract class Element
    {
        /// <summary>
        /// matches the event against the full element and determines if the event 
        /// satifies the element criteria,
        /// </summary>
        /// <param name="chronevent"></param>
        /// <returns></returns>
        internal abstract IsMatchResult IsMatch(IChronologicalEvent chronevent, Tracker Tracker);

        /// <summary>
        /// does a quick top level test to determine match to decide wether to implement
        /// a tracker for this event
        /// </summary>
        /// <param name="chronevent"></param>
        /// <returns></returns>
        internal abstract bool IsPotentialMatch(IChronologicalEvent chronevent);

        /// <summary>
        /// This method is called on newly contrcusted AST classes
        /// The class will examine who is in the staement and decide if it should be the root or the child
        /// </summary>
        /// <param name="ExisitngElement"></param>
        /// <returns></returns>
        public abstract Element ReturnParseTreeFromExistingElement(Element ExisitngElement);

    }

   

}
