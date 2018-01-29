# Chronological Expressions
A Event Pattern Matching language

[ChronEx Pattern Language Specification](https://github.com/mbadler/ChronologicalExpressions/wiki/ChronEx-Pattern-Specification)

## Reference Implementations

There are 2 reference implementations

__C#__ [https://github.com/mbadler/ChronEx-CSharp]

Last Release: 0.0.6 Numeric Quantifiers

__Python__ [https://github.com/mbadler/ChronEx-Python]

Last Release: None Yet


### ChronExQuery  - _Pattern Expression Querying Editor_

There is a small simple pattern expression query editor application included in the C# repository based on the c# reference implementation. It allows you to paste a dataset and then query patterns against the data. [Direct link](https://github.com/mbadler/ChronEx-CSharp/tree/master/ChronExQuery) to the application.

Currently included with query editor is a dataset featuring a lifecycle events log of daily actions of a person. The log has been adapted from https://doi.org/10.4121/uuid:01eaba9f-d3ed-4e04-9945-b8b302764176

## The Problem
When trying to extract events from a log, its easy to pinpoint a single entry , but it is much harder to relate this single entry to a scope that would encompass a entire sequence of events. There is no real way to express a pattern of sequences of log entries that would signify that a significant event happened. 

As an example: Webserver log all requests to the server, however by looking at a single log entry you would not be able to tell what that browsing session accomplished, its only by looking at all of the entries for that session that you see a pattern emerge that will tell you the nature of the session

## The Solution
ChronologicalExpressions (ChronEx is short) is proposed as an analytical approach to this problem. This approach is loosely patterned after RegEx (Regular Expressions). In which you express a pattern of how you expect a certain scenario to chronologically occur and then you apply that expression against a set of data and Match/Extract the events

To understand this in regex terms: Searching text is an inherently ordered operation, first you match the first letter , if it matches then you move on the next letter etc..

Regex allows us to define more complicated searching sequences, so for example a RegEx of "^A{2}$" would tell us to start at the beginning of the string and then move forward and match 2 A's and then move forward and make sure that its the end of the line

## Is this a Machine Learning algorithim?

No, ChronEx would be considered a "Expert System" algorithim, wherin a domain expert knows exactly the pattern of events they are looking for. ChronEx allows the expert to express his search pattern in a clear and cocise manner. Instead of having to write mutli join sql statments or implement a state machine in code, chronex allows the expert to map out what he knows the patteren should look like.

## Example
Suppose we had a issue , we know that many customers are adding stuff to shopping carts but they are not checking out, we need to find out why. We want to query the logs (in this case per session) and find which sequence of logs per session meet the criteria of a dropped sale. Using ChronEx we would be able to express what we are looking for something like this

~~~
ShoppingCart.Load  //Event signifies that the user viewed the shopping cart
!Confirm.Load* // zero or more no Confirmation load events
$ // end of session
~~~

This expression will find any session where the user went to the shopping cart page - but by the time the session timed out there were no additional visits to the confirm page again


