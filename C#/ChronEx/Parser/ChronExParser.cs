using ChronEx.Models.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChronEx.Parser
{
    public class ChronExParser
    {
        // for release 1 , extreemly simple parser
        // split by line breaks then create different type based on 
        // text
        public ParsedTree ParsePattern(string Pattern)
        {
            var elements = Pattern.Split('\n');
            var ptree = new ParsedTree();
            foreach(var elem in elements)
            {
                var elmname = elem.Trim();
                if (elmname == "")
                {
                    continue;
                }
                if(elmname.StartsWith("."))
                {
                    ptree.AddElement(new dotSelector());
                }
                else
                {
                    ptree.AddElement(new SpecifiedEventNameSelector()
                    {
                        EventName = elmname
                    });

                }
            }
            return ptree;
        }
    }
}
