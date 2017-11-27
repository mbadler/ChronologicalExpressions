using ChronEx.Models;
using ChronEx.Models.AST;
using ChronEx.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChronEx.Tests
{
    [TestClass]
    public class QuantifierElementTests
    {
        [TestMethod]
        public void Quant_Symbol_CorrectASTCreatedWithNegate()
        {
            var n = new ChronExParser().ParsePattern("!abc+").GetElements().ToList();

            Assert.IsTrue(n[0] is SymbolQuantifier);
            var b = (SymbolQuantifier)n[0];
            Assert.IsTrue(b.ContainedElement is NegatedElement);
            var neg = ((NegatedElement)b.ContainedElement);
            Assert.IsTrue(neg.ContainedElement is SpecifiedEventNameSelector);

        }

        [TestMethod]
        public void Quant_Symbol_CorrectSymbolAssigned()
        {
            var n = new ChronExParser().ParsePattern("abc+").GetElements().ToList();

            Assert.IsTrue(n[0] is SymbolQuantifier);
            var b = (SymbolQuantifier)n[0];
            Assert.AreEqual('+', b.QuantifierSymbol);

        }



        [TestMethod]
        public void Quant_Symbol_PlusExists()
        {
            var script =
@"a
b+
c";
            var events = GetQuanTestSet();
            var matches = ChronEx.Matches(script, events);
            matches[0].CapturedEvents.AssertMatchesAreEqual("a,b,b,b,b,c");
            matches[1].CapturedEvents.AssertMatchesAreEqual("a,b,b,c");
            matches[2].CapturedEvents.AssertMatchesAreEqual("a,b,c");
            Assert.AreEqual(3, matches.Count);
        }

        [TestMethod]
        public void Quant_Symbol_PlusWithRegexOr()
        {
            var script =
@"a
/b|c/+
d";
            var events = GetQuanTestSet();
            var matches = ChronEx.Matches(script, events);
            matches[0].CapturedEvents.AssertMatchesAreEqual("a,b,b,b,b,c,c,c,c,d");
            matches[1].CapturedEvents.AssertMatchesAreEqual("a,b,b,c,c,d");
            matches[2].CapturedEvents.AssertMatchesAreEqual("a,b,c,d");
            Assert.AreEqual(3, matches.Count);
        }

        [TestMethod]
        public void Quant_Symbol_Star()
        {
            var script =
@"b
a*
d";
            var events = GetQuanTestSet();
            var matches = ChronEx.Matches(script, events);
            matches[0].CapturedEvents.AssertMatchesAreEqual("b,a,d");
            matches[1].CapturedEvents.AssertMatchesAreEqual("b,d");
            matches[2].CapturedEvents.AssertMatchesAreEqual("b,a,a,d");
            Assert.AreEqual(3, matches.Count);
        }

        [TestMethod]
        public void Quant_Symbol_QuestionMark()
        {
            var script =
@"b
a?
d";
            var events = GetQuanTestSet();
            var matches = ChronEx.Matches(script, events);
            matches[0].CapturedEvents.AssertMatchesAreEqual("b,a,d");
            matches[1].CapturedEvents.AssertMatchesAreEqual("b,d");
            Assert.AreEqual(2, matches.Count);
        }


        public IEnumerable<ChronologicalEvent> GetQuanTestSet()
        {
            var TestSet =
        @"a,10/17/2017 0:01
a,10/17/2017 0:02
a,10/17/2017 0:03
a,10/17/2017 0:04
b,10/17/2017 0:05
b,10/17/2017 0:06
b,10/17/2017 0:07
b,10/17/2017 0:08
c,10/17/2017 0:09
c,10/17/2017 0:10
c,10/17/2017 0:11
c,10/17/2017 0:12
d,10/17/2017 0:13
d,10/17/2017 0:14
d,10/17/2017 0:15
d,10/17/2017 0:16
a,10/17/2017 0:17
a,10/17/2017 0:18
b,10/17/2017 0:19
b,10/17/2017 0:20
c,10/17/2017 0:21
c,10/17/2017 0:22
d,10/17/2017 0:23
d,10/17/2017 0:24
a,10/17/2017 0:25
b,10/17/2017 0:26
c,10/17/2017 0:27
d,10/17/2017 0:28
c,10/17/2017 0:29
b,10/17/2017 0:30
a,10/17/2017 0:31
d,10/17/2017 0:32
c,10/17/2017 0:33
b,10/17/2007 0:34
d,10/17/2017 0:35
b,10/17/2017 0:36
b,10/17/2017 0:37
a,10/17/2017 0:38
a,10/17/2017 0:39
d,10/17/2017 0:40"
;
            return TestUtils.SplitLogsStringsIntoChronEventList(TestSet);
        }
    }
}