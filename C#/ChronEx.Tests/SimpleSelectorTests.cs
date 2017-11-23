using ChronEx.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChronEx.Tests
{
    [TestClass]
    public class SimpleSelectorTests
    {

        [TestMethod]
        public void EmptyPatternReturnsFalse()
        {
            var events = GetBasicCheckoutEvents();
            var script = "";

            Assert.IsFalse(ChronEx.IsMatch(script, events));
        }

        [TestMethod]
        public void SimpleSelectorSequenceMatch()
        {
            var events = GetBasicCheckoutEvents();
            var script =
            @"ItemPage.Item.AddToCart.Clicked
ItemPage.ShoppingCartLink.Clicked";
            Assert.IsTrue(ChronEx.IsMatch(script, events));
        }

        [TestMethod]
        public void SimpleSelectorSequenceDoesNotMatch()
        {
            var events = GetBasicCheckoutEvents();
            var script =
            @"ItemPage.Item.AddToCart.Clicked
Catalog.Render
ItemPage.ShoppingCartLink.Clicked";
            Assert.IsFalse(ChronEx.IsMatch(script, events));
        }

        [TestMethod]
        public void CountOfASingleSelector()
        {
            var events = GetBasicCheckoutEvents();
            var script =
            @"Catalog.Data.Render";
            Assert.AreEqual(3, ChronEx.MatchCount(script, events));
        }

        [TestMethod]
        public void dotMatchesEveryEvent()
        {
            var events = GetBasicCheckoutEvents();
            var script =
            @".";
            Assert.AreEqual(36, ChronEx.MatchCount(script, events));
        }

        [TestMethod]
        public void SelectorAndDotAndSelectorMatch()
        {
            var events = GetBasicCheckoutEvents();
            //this pattern will only have one match
            var script =
            @"ItemPage.Load
ItemPage.Render
ItemPage.Accessories.MoreDetailLink.Clicked";
            // this will now match 2 (including the session.end
            Assert.AreEqual(1, ChronEx.MatchCount(script, events));
            script =
            @"ItemPage.Load
ItemPage.Render
.";
            Assert.AreEqual(2, ChronEx.MatchCount(script, events));


        }

        [TestMethod]
        public void NegationNegatesAMatch()
        {
            var events = GetBasicCheckoutEvents();
            var script =
@"Homepage.Link.Clicked
!Catalog.Load";

            Assert.AreEqual(1, ChronEx.MatchCount(script, events));
        }

        [TestMethod]
        public void RegexSelectorMatches()
        {
            var events = GetBasicCheckoutEvents();
            var script =
            @"/(Catalog|HomePage)\.Load/";
            Assert.AreEqual(5, ChronEx.MatchCount(script, events));
        }

        [TestMethod]
        public void MetchesMethodReturnsMatches()
        {
            var events = GetBasicCheckoutEvents();
            var script =
            @"/(Catalog|HomePage)\.Load/
.";
            var matches = ChronEx.Matches(script, events);

            Assert.AreEqual(5, matches.Count);
            Assert.AreEqual(2, matches[0].CapturedEvents.Count);
        }

        public IEnumerable<ChronologicalEvent> GetBasicCheckoutEvents()
        {
            var s =
           @"Session.Start,10/17/2017 0:00
HomePage.Load,10/17/2017 0:00
HomePage.Render,10/17/2017 0:01
HomePage.Link.Clicked,10/17/2017 0:02
Catalog.Load,10/17/2017 0:03
Catalog.Render,10/17/2017 0:04
Catalog.Next.Clicked,10/17/2017 0:05
Catalog.Data.Load,10/17/2017 0:06
Catalog.Data.Render,10/17/2017 0:07
Catalog.ItemLink.Clicked,10/17/2017 0:08
ItemPage.Load,10/17/2017 0:09
ItemPage.Render,10/17/2017 0:10
ItemPage.Accessories.MoreDetailLink.Clicked,10/17/2017 0:11
ItemPage.Accessories.Data.Load,10/17/2017 0:12
ItemPage.Item.AddToCart.Clicked,10/17/2017 0:13
ItemPage.ShoppingCartLink.Clicked,10/17/2017 0:14
ShoppingCart.Load,10/17/2017 0:15
ShoppingCart.Render,10/17/2017 0:16
ShoppingCart.CheckoutNow.Clicked,10/17/2017 0:17
HomePage.Load,10/17/2017 0:18
HomePage.Render,10/17/2017 0:19
HomePage.Link.Clicked,10/17/2017 0:20
Catalog.Load,10/17/2017 0:21
Catalog.Render,10/17/2017 0:22
Catalog.Next.Clicked,10/17/2017 0:23
Catalog.Data.Load,10/17/2017 0:24
Catalog.Data.Render,10/17/2017 0:25
Catalog.Next.Clicked,10/17/2017 0:26
Catalog.Data.Load,10/17/2017 0:27
Catalog.Data.Render,10/17/2017 0:28
HomePage.Load,10/17/2017 0:29
HomePage.Render,10/17/2017 0:30
Homepage.Link.Clicked,10/17/2017 0:31
ItemPage.Load,10/17/2017 0:32
ItemPage.Render,10/17/2017 0:33
Session.End,10/12/2017 0:34"
;
            return s.Split('\n').Select(x => x.Split(','))
                .Select(y => new ChronologicalEvent()
            {
                EventName = y[0],
                EventDateTime = DateTime.Parse(y[1])
            });

        }
    }
}



