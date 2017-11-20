using System;
using System.Collections.Generic;
using System.Text;
using ChronEx.Models;
using ChronEx.Models.AST;

namespace ChronEx.Parser
{
    public class Tracker
    {
        private ParsedTree tree;
        IEnumerator<Element> _evntEnum = null;
        private List<IChronologicalEvent> StoredList = null;

        public Tracker(ParsedTree tree)
        {
            this.tree = tree;
            _evntEnum = tree.GetElements().GetEnumerator();
            //element trees are guarunteed to have at lest one element 
            //so we can sfly move next
            _evntEnum.MoveNext();
        }

        internal TrackerProcessResult ProcessEvent(IChronologicalEvent even,bool Store)
        {
            //check the match
            var res = _evntEnum.Current.IsMatch(even);
            // if it does not match then we can termiante this tracker
            //return nomatch and caller will terminte
            if (res == IsMatchResults.IsNotMatch)
            {
                return TrackerProcessResult.IsNotMatch;
            }
            //continue will not be implemented yet , no need ot hadle now

            //if the element matched then:
           //if we should store then store it
           if(Store && StoredList == null)
            {
                StoredList = new List<IChronologicalEvent>();
            }
           if(Store)
            {
                StoredList.Add(even);
            }
                //if this is the last element in the tree
                //then return that we sucessfully matched the whole tree
                if(!_evntEnum.MoveNext())
                {
                    return TrackerProcessResult.IsMatch;
                }
                else
                {
                    //allow the process to contineu
                    return TrackerProcessResult.NoMatchYet;
                }
            
        }
    }

    public enum TrackerProcessResult
    {
        /// <summary>
        /// This event filled the pattern and there is a match
        /// </summary>
        IsMatch,
        /// <summary>
        ///this element was valid but the entire tree pattern has yet to be checked
        ///
        NoMatchYet,
        /// <summary>
        /// This tracker will not result in a match
        /// </summary>
        IsNotMatch


    }
}
