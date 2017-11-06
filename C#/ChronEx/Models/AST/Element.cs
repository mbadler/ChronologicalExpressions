using System;
using System.Collections.Generic;
using System.Text;

namespace ChronEx.Models.AST
{
    public abstract class Element
    {
        /// <summary>
        /// matches the event against the full element and determines if the event 
        /// satifies the element criteria, for quantifiers it may return Contine 
        /// which means that it matches so far - but we need more elements to be
        /// certain
        /// </summary>
        /// <param name="chronevent"></param>
        /// <returns></returns>
        internal abstract IsMatchResults IsMatch(IChronologicalEvent chronevent);

        /// <summary>
        /// does a quick top level test to determine match to decide wether to implement
        /// a tracker for this event
        /// </summary>
        /// <param name="chronevent"></param>
        /// <returns></returns>
        internal abstract bool IsPotentialMatch(IChronologicalEvent chronevent);


    }

    /// <summary>
    /// Results from a is match 
    /// </summary>
    public enum IsMatchResults
    {
        //was a match
        IsMatch,
        //was not a match
        IsNotMatch,
        //need more events to determine
        Continue
    }

}
