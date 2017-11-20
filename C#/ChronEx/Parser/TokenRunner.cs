using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ChronEx.Models.AST;

namespace ChronEx.Parser
{
    public class TokenRunner
    {
        List<LexedToken> tokens = new List<LexedToken>();
        ParsedTree ptree = null;
        int curind = 0;

        public TokenRunner(IEnumerable<LexedToken> Tokens, ParsedTree p)
        {
            tokens = Tokens.ToList();
            ptree = p;


        }

       


    }

   

}
