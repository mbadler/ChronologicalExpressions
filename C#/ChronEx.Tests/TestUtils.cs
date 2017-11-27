using ChronEx.Models;
using ChronEx.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChronEx.Tests
{
    public static class TestUtils
    {
        public static IEnumerable<ChronologicalEvent> SplitLogsStringsIntoChronEventList(string s)
        {
            return s.Split('\n').Select(x => x.Split(','))
                            .Select(y => new ChronologicalEvent()
                            {
                                EventName = y[0],
                                EventDateTime = DateTime.Parse(y[1])
                            });
        }

        public static void AssertMatchesAreEqual(this List<IChronologicalEvent> MatchList,string AssertedList)
        {
            if (MatchList == null)
            {
                throw new ArgumentNullException(nameof(MatchList));
            }

            if (string.IsNullOrWhiteSpace(AssertedList))
            {
                throw new ArgumentException("Asserted List", nameof(AssertedList));
            }

            var AssertAsList = AssertedList.Split(",");
            if(MatchList.Count != AssertAsList.Count())
            {
                throw new Exception("Match List count does not match AssertList");
            }

            for (int i = 0; i < MatchList.Count(); i++)
            {
                if(AssertAsList[i] != MatchList[i].EventName)
                {
                    throw new Exception($"For index {i} {MatchList[i].EventName} does not equal {AssertAsList[i]} ");
                }
            }
        }
    }
}
