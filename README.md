# Chronological Expressions
A Event Pattern Matching language

## The Problem
When trying to extract events from a log, its easy to pinpoint a single entry , but it is much harder to relate this single entry to a scope that would encompass a entire sequence of events. There is no real way to express a pattern of sequences of log entries that would signify that a significant event happend. 

As an example: Webserver log all requests to the server, however by looking at a single log entry you would not be able to tell what that browsing session accomplished, its only by looking at all of the entries for that session that you see a pattern emerge that will tell you the nature of the session

## The Solution
ChronologicalExpressions (ChronEx is short) is propsed as an analytical approach to this problem. This approach is loosly patterend after RegEx (Regular Expressions). In which you express a pattern of how you expect a certain scenerio to chronologicly occour and then you apply that expression against a set of data and Match/Extract the events

To understand this in regex terms: Searching text is an inherintaly ordered operation, first you match the first letter , if it matches then you move on the next letter etc..

Regex allows us to define more complicated searching sequences, so for example a RegEx of "^A{2}$" would tell us to start at the beginning of the stirng and then move forward and match 2 A's and then move forward and make sure that its the end of the line

## Example
Suppose we had a issue , we know that many customers are adding stuff to shopping carts but they are not checking out, we need to find out why. We want to query the logs (in this case per session) and find which sequence of logs per session meet the criteria of a dropped sale. Using ChronEx we would be able to express what we are looking for something like this

~~~
ShoppingCart.Load  //Event signifies that the user viewed the shopping cart
^Confirm.Load* // zero or more no Confirmation load events
$ // end of session
~~~

This expression will find any session where the user went to the shopping cart page - but by the time the session timed out there were no aditional visits to the confirm page again

# Proposed Spec

**Elements**

* Patterns are composed of elements seperated by line breaks
* An element can be a Selector or a Group
* Selectors are positioned at the begining of the group indentation level
* Groups start with a grouping mark and end with a grouping mark (for example the ( ) marks )
* after each element you can place a quantfier (similiar to the regex quantifiers)
* Additionally you can add filter directives to a selector - this will modify the selector so that the selector will only match if the filter matches


**Direct Selector**

A direct selector is a selector which appears in the pattern directly

For example this selector will match the Page_Loaded event as is
~~~
Page_loaded
~~~


**Wildcard Selector**

A wildcard selector is denoted with a . and will match any event

Example: This pattern will match a Page_loaded event followed by any single event followed by a Page_unloaded event
~~~
Page_Loaded
.
Page_Unloaded
~~~

**Regex Selector**

A selector can be a RegEx expression that is matched against selector names
regex selectors are defined similar to javascript regex litertals by sourounding it with a // marks

Example: This pattern will match a page load event followed by any event named click followed a page unloaded event

~~~
Page_Loaded
/Page_.*Click/
Page_Unloaded
~~~

**Selector Quantifiers**

Similar to regex , a selector can have a quantifier that will match only if the event match the quantity specified

Example: This pattern will only match a linkclicked event that happens at least 3 but not more than 5 times in a row
~~~
Link_Clicked{3,5}
~~~

**Negated Selector**

A negated selector will match any event next in the list if it is not the specified events. it is denoted with a ^ 

Example: This pattern will match any link click that is not immedialty followed with a page load (unless the seesion ended)
~~~
Link_Clicked
^Page_Load
~~~


