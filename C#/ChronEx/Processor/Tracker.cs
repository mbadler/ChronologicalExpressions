using System;
using System.Collections.Generic;
using System.Text;
using ChronEx.Models;
using ChronEx.Models.AST;

namespace ChronEx.Processor
{
    public class Tracker
    {
        private ParsedTree tree;
        IEnumerator<Element> _evntEnum = null;
        public List<IChronologicalEvent> StoredList = null;

        /// <summary>
        /// A state back passed around to elements so that we can keep the AST objects stateless
        /// </summary>
        internal Dictionary<Element,object> StateBag = new Dictionary<Element,object>();
        
        internal int StackDepth = 0;
        public Tracker(ParsedTree tree)
        {
            this.tree = tree;
            _evntEnum = tree.GetElements().GetEnumerator();
            //element trees are guarunteed to have at lest one element 
            //so we can sfly move next
            _evntEnum.MoveNext();
        }

        internal IsMatchResult ProcessEvent(IChronologicalEvent even,bool Store)
        {
            //check the match
            var res = _evntEnum.Current.IsMatch(even,this);
            // if it does not match then we can termiante this tracker
            //return nomatch and caller will terminte
            if (res == IsMatchResult.IsNotMatch)
            {
                return IsMatchResult.IsNotMatch;
            }
            //continue will not be implemented yet , no need ot hadle now


            //if this is not a forwarded event (which dot not capture)
            //then store it in the capture list
            if (res != IsMatchResult.ForwardToNext)
            {
                if (Store && StoredList == null)
                {
                    StoredList = new List<IChronologicalEvent>();
                }
                if (Store)
                {
                    StoredList.Add(even);
                }
            }
           // if the result was a staight out match then move the element pointer forward and report the results 
           if(res == IsMatchResult.IsMatch)
            {
                if(!_evntEnum.MoveNext())
                {
                    return IsMatchResult.IsMatch;
                }
                else
                {
                    //allow the process to contineu
                    return IsMatchResult.Continue;
                }
            }
           //if its a continue - then just continue
           if(res == IsMatchResult.Continue)
            {
                return IsMatchResult.Continue;
            }

           //if its a forward then rerun this event against the next item in the list
           if(res == IsMatchResult.ForwardToNext)
            {
                //try to movenext
                //if this was the last one on the last then it would be a match and just call it that
                if (!_evntEnum.MoveNext())
                {
                    return IsMatchResult.IsMatch;
                }
                else
                {
                    //call myself again using the same 
                    return ProcessEvent(even, Store);
                }
            }

            return IsMatchResult.IsNotMatch;
        }

        public T GetMyStateBag<T>(Element Eleme) 
            {
            if(StateBag.ContainsKey(Eleme))
            {
                return (T)StateBag[Eleme];
            }
            else
            {
                return default(T);
            }
        }

        public void SetMyStateBag(Element Eleme,Object StateBagObj)
        {
            StateBag[Eleme] = StateBagObj;
        }

        public void RemoveMyStateBag(Element Eleme)
        {
            StateBag.Remove(Eleme);
        }



    }

    public enum IsMatchResult
    {
        /// <summary>
        /// This event filled the pattern and there is a match
        /// </summary>
        IsMatch,
        /// <summary>
        ///So far we match - but cannot give a definitive answer yet (for quatifiers , and and clauses)
        ///
        Continue,
        /// <summary>
        /// This tracker will not result in a match
        /// </summary>
        IsNotMatch,
        
        /// <summary>
        /// This event is the end of my matching , forward the same event to the next element
        /// </summary>
        ForwardToNext,
        
    }
}
