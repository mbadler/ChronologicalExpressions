using System;
using System.Collections.Generic;
using System.Text;
using ChronEx.Parser;
using ChronEx.Processor;

namespace ChronEx.Models.AST
{
    public class SymbolQuantifier : QuantifierElement
    {

        public char QuantifierSymbol { get; set; }

        public override Element ReturnParseTreeFromExistingElement(Element ExisitngElement)
        {
            //for now I am the top element inthe chain so add it to me and return myself
            AddContainedElement(ExisitngElement);
            return this;
        }

        internal override IsMatchResult IsMatch(IChronologicalEvent chronevent, Tracker Tracker)
        {
            //first lets check the statebag to see if we are in there yet
            var sbag = Tracker.GetMyStateBag<int?>(this);
            if (!sbag.HasValue)
            {
                sbag = new Nullable<int>(0);
                Tracker.SetMyStateBag(this,sbag);
            }

            //Pass the event along to the contained element and get its result
            var subRes = ContainedElement.IsMatch(chronevent, Tracker);
            IsMatchResult tres = IsMatchResult.IsNotMatch;
            if(QuantifierSymbol=='+')
            {
                tres = HandlePlus(sbag, subRes);
            }

            if (QuantifierSymbol == '*')
            {
                tres = HandleStar(sbag, subRes);
            }

            if (QuantifierSymbol == '?')
            {
                tres = HandleQuestionMark(sbag, subRes);
            }

            //if its the end (either a match or not) , then remove our bag state
            if (tres == IsMatchResult.IsNotMatch || tres == IsMatchResult.ForwardToNext || tres == IsMatchResult.IsMatch)
            {
                Tracker.RemoveMyStateBag(this);
            }
            else
            {
                //if its a continue then increment the bagstate
                sbag = sbag.Value+1;
                Tracker.SetMyStateBag(this, sbag);
            }

            //return to the tracker the result
            return tres;

        }

        private IsMatchResult HandleQuestionMark(int? sbag, IsMatchResult subRes)
        {
            //rules for star
            //Matches 0 or more events, if there are any events then they are captured if there aren't any then nothing s captured

            //lets first handle the simpilest scenerio - no match at all
            //return a forward
            if (sbag.Value == 0 && subRes == Processor.IsMatchResult.IsNotMatch)
            {
                return Processor.IsMatchResult.ForwardToNext;
            }

            //next scenerio - this is  the first match then declare it a continue , we don't match becasue the next round might not be a match
            //and then we would need to cancel it out
            if (sbag.Value == 0 && subRes == Processor.IsMatchResult.IsMatch)
            {
                return Processor.IsMatchResult.Continue;
            }

            //if we already matched once and this event does not match then its ok and forward
            if (sbag.Value == 1 && subRes != Processor.IsMatchResult.IsMatch)
            {
                return Processor.IsMatchResult.ForwardToNext;
            }

            //any other scenrio is a not match
            return IsMatchResult.IsNotMatch;

            //for now should not be any other possible scenrio
            throw new Exception("Unexpected");

        }

        private IsMatchResult HandleStar(int? sbag, IsMatchResult subRes)
        {
            //rules for star
            //Matches 0 or more events, if there are any events then they are captured if there aren't any then nothing s captured

            //lets first handle the simpilest scenerio - no match at all
            //return a forward
            if (sbag.Value == 0 && subRes == Processor.IsMatchResult.IsNotMatch)
            {
                return Processor.IsMatchResult.ForwardToNext;
            }

            //next scenerio - this is either the first or subsequent got arounds and we match
            if (subRes == Processor.IsMatchResult.IsMatch)
            {
                return Processor.IsMatchResult.Continue;
            }

            //next scenerio - we matched previously but we dont match now
            // we mark ourself as sucessful but we don't capture this element we pass it on to the next
            if (sbag.Value > 0 && subRes == Processor.IsMatchResult.IsNotMatch)
            {
                return Processor.IsMatchResult.ForwardToNext;
            }

            //for now should not be any other possible scenrio
            throw new Exception("Unexpected");
        }

        private IsMatchResult HandlePlus(int? sbag, IsMatchResult subRes)
        {
            //rules for plus
            //Matches at least 1 event but will capture all matching events

            //lets first handle the simpilest scenerio - nothing found yet and this is not a match
            //return a no match
            if(sbag.Value == 0 && subRes == Processor.IsMatchResult.IsNotMatch)
            {
                return Processor.IsMatchResult.IsNotMatch;
            }

            //next scenerio - this is either the first or subsequent got arounds and we match
            if(subRes == Processor.IsMatchResult.IsMatch)
            {
                return Processor.IsMatchResult.Continue;
            }

            //next scenerio - we matched previously but we dont match now
            // we mark ourself as sucessful but we don't capture this element we pass it on to the next
            if(sbag.Value > 0 && subRes == Processor.IsMatchResult.IsNotMatch)
            {
                return Processor.IsMatchResult.ForwardToNext;
            }

            //for now should not be any other possible scenrio
            throw new Exception("Unexpected");
        }



        //for now return true;
        internal override bool IsPotentialMatch(IChronologicalEvent chronevent)
        {
            return true;
        }
    }
}
