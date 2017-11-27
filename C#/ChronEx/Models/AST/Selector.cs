using ChronEx.Parser;
using ChronEx.Processor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ChronEx.Models.AST
{
    /// <summary>
    /// Selector is the base class for the simple top level selectors
    /// </summary>
    public abstract class Selector:Element
    {
        //Simple selectors are always the final step in any tree
        //so just add my self to the exiting and return the exisitng
        public override Element ReturnParseTreeFromExistingElement(Element ExistingElement)
        {
            //if exising is null then i don't have a parent and just return myself
            if(ExistingElement==null)
            {
                return this;
            }

            //if the exisitng is a simple selector then thats a error , a simple selector cannoth conatin another selecotr
            if(ExistingElement is Selector)
            {
                throw new Exception($"Selector of type {this.GetType().Name} cannot contain another selector , requested selector type is {ExistingElement.GetType().Name}");
            }

            //the containg type needs to be a container , if its not then throw an exceptiona
            var contype = ExistingElement as ContainerElement;
            if(contype==null)
            {
                throw new Exception($"A container for a selector must be a ContainerElement , however  type {ExistingElement.GetType().Name} is not a container ");
            }

            //add my self to the container and return the existing
            contype.AddContainedElement(this);
            return ExistingElement;
        }
    }

    public class SpecifiedEventNameSelector:Selector
    {
        public string EventName { get; set; }

        internal override IsMatchResult IsMatch(IChronologicalEvent chronevent, Tracker Tracker)
        {
           return (string.Compare(chronevent.EventName, this.EventName, true) == 0)
                ? Processor.IsMatchResult.IsMatch : Processor.IsMatchResult.IsNotMatch;
        }

        internal override bool IsPotentialMatch(IChronologicalEvent chronevent)
        {
            //will always be the same as full match
            return IsMatch(chronevent, null) != Processor.IsMatchResult.IsNotMatch;
        }
    }

    public class DotSelector : Selector
    {
        //. matches everything so its always true
        internal override IsMatchResult IsMatch(IChronologicalEvent chronevent, Tracker Tracker)
        {
            return Processor.IsMatchResult.IsMatch;
        }

        internal override bool IsPotentialMatch(IChronologicalEvent chronevent)
        {
            //. matches everything so its always true
            return true;
        }
    }

    public class RegexSelector : Selector
    {

        public string MatchPattern { get; set; }

        Regex rgx = null;
        internal override IsMatchResult IsMatch(IChronologicalEvent chronevent, Tracker Tracker)
        {
            if(IsRegexMatch(chronevent.EventName))
            {
                return Processor.IsMatchResult.IsMatch;
            }
            return Processor.IsMatchResult.IsNotMatch;
        }

        internal override bool IsPotentialMatch(IChronologicalEvent chronevent)
        {
            return IsRegexMatch(chronevent.EventName);
        }

        bool IsRegexMatch(string Text)
        {
            if(rgx == null)
            {
                rgx = new Regex(MatchPattern);
            }
            return rgx.IsMatch(Text);
        }
    }


}
