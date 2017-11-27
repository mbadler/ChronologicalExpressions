using ChronEx.Models;
using ChronEx.Models.AST;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ChronEx.Processor
{
    
    public class Runner
    {
        protected ParsedTree tree;

        protected IEnumerable<IChronologicalEvent> EventList { get; }

        protected List<Tracker> trackList = new List<Tracker>();
        /// <summary>
        /// Trackers that were found to have results and the captures
        /// </summary>
        protected List<Tracker> CapturedtrackList = new List<Tracker>();

        // for eventual streaming implementation
        public Runner()
        {

        }

        public Runner(ParsedTree tree,IEnumerable<IChronologicalEvent> EventList)
        {
            this.tree = tree;
            this.EventList = EventList;
        }

        //runs thru the events and returns a bool if the pattern matches the events
        //Currently "Event Directed Engine" (conceptually a Text directed Engine in regex world)
        //For the future we can look at creating a (Pattern Directed Engine)
        public virtual bool IsMatch()
        {
            return PerformMatch(true,false).Item2 > 0;
        }

        //runs thru the events and returns the number of matches found
        public virtual int MatchCount()
        {
            return PerformMatch(false,false).Item2 ;
        }

        /// <summary>
        /// Returns a list containing the list of matches found
        /// </summary>
        /// <returns></returns>
        public virtual List<ChronExMatch> Matches()
        {
            return PerformMatch(false, true).Item1;
        }

        /// <summary>
        /// Performs pattern matching
        /// </summary>
        /// <param name="ShortCircuit">Indicates to return as soon as a match is found</param>
        /// <returns>if short circuit 1 for match 0 for no matches , if not short circuit will return the number of matches found</returns>
        protected (List<ChronExMatch>,int) PerformMatch(bool ShortCircuit,bool Store)
        {
            // get the first element this is the lowest level filter to launching a tracker
            var getlemes = tree.GetElements();
            if (!getlemes.Any())
            {
                return (null,0);
            }
            var firstElem = getlemes.First();

            //if there are no elements then its of course not a mathc
            if (firstElem == null)
            {
                return (null, 0);
            }

            var matchcount = 0;

            //Loop thru the event list and for each event do 2 things
            // 1. Determine if we can set up a tracker for this event
            // 2. Broadcast this event to all trackers to allow them to match themselves
            foreach (var even in EventList)
            {
                //check if we should create a new tracker
                if (firstElem.IsPotentialMatch(even))
                {
                    StartNewTracker(tree);
                }
                //loop thru all the trackers and send them the event and see if any of them match
                //if any do then immediatly exit with a true

                //the remove list will maintain a list of trackers that have detrmine that they cannot match
                //and they will be removed from the tracker list after the foreach
                List<Tracker> _removeList = new List<Tracker>();
                foreach (var trk in trackList)
                {
                    // submit to the tracker for processing
                    // since this is only the match function
                    // if any of the trackers have a positive result - return right away
                    var m = trk.ProcessEvent(even,Store);
                    if (m == Processor.IsMatchResult.IsMatch)
                    {
                        if (ShortCircuit)
                        {
                            return (null,1);
                        }
                        else
                        {
                            //increment the match count and add it to the remove list
                            
                            matchcount++;
                            _removeList.Add(trk);
                            if (Store)
                                CapturedtrackList.Add(trk);
                        }
                        
                    }
                    //if no match then add it to the remove list
                    if (m == Processor.IsMatchResult.IsNotMatch)
                    {
                        _removeList.Add(trk);
                    }
                }

                //remove all trackers that are in the remove list
                foreach (var trk in _removeList)
                {
                    trackList.Remove(trk);
                }

            }

            // at this point we have run out of events and non of the trackers have claimed sucess so return false
            if(Store)
            {
                var lst = new List<ChronExMatch>();
                foreach (var item in CapturedtrackList)
                {
                    lst.Add(new ChronExMatch()
                    {
                        CapturedEvents = item.StoredList
                    });

                }
                return (lst, matchcount);
            }
            return (null,matchcount);
        }

        private void StartNewTracker(ParsedTree tree)
        {
            // for now just adds a new genric tracker to the list
            trackList.Add(new Tracker(tree));
        }

        
    }
}
