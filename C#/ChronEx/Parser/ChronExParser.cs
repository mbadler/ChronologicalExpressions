using ChronEx.Models.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChronEx.Parser
{
    public class ChronExParser
    {
        // for release 0.2 , create a lexer and tokenize the inppt 
        // pass it to a parser to create the syntax tree
        // split by line breaks then create different type based on 
        // text
        public ParsedTree ParsePattern(string Pattern)
        {
            var Lex = new Lexer(Pattern);

            var tokens = Lex.returnlist;

            // pass it to the parser to get an ast
            return CreateAST(tokens);
            
        }

        public ParsedTree CreateAST(List<LexedToken> tokens)
        {
            var p = new ParsedTree();
            foreach (var item in tokens)
            {
                switch (item.TokenType)
                {
                    case LexedTokenType.TEXT:
                    case LexedTokenType.DELIMITEDTEXT:
                        {
                            if (item.TokenText == ".")
                            {
                                p.AddElement(new dotSelector());
                            }
                            else
                            {
                                p.AddElement(new SpecifiedEventNameSelector()
                                {
                                    EventName = item.TokenText
                                });
                            }

                            break;
                        }
                    
                       
                    case LexedTokenType.REGEX:
                        {
                            p.AddElement(new RegexSelector()
                            {
                                MatchPattern = item.TokenText
                            });

                            break;
                        }
                        
                    // Safly ignore these for now
                    case LexedTokenType.NEWLINE:
                        break;
                    case LexedTokenType.BOF:
                        break;
                    case LexedTokenType.UNKNOWN:
                        break;
                    case LexedTokenType.WHITESPACE:
                        break;
                    case LexedTokenType.TAB:
                        break;
                    case LexedTokenType.EOF:
                        break;
                    default:
                        break;
                }
            }
            return p;
        }
    }
}
