using ChronEx.Models.AST;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ChronEx.Parser
{
    public class ChronExParser
    {
        // for release 0.3 , create a lexer and tokenize the inppt 
        // split it by statements
        // directly create th AST from the statements
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
            //split the current token into individual statements
            var statements = SplitTokensIntoStatements(tokens);


            //if its a empty tree - just exit
            if(tokens.Count==2
                && tokens[0].TokenType == LexedTokenType.BOF
                && tokens[1].TokenType == LexedTokenType.EOF)
            {
                return p;
            }
            //run thru the individual statemnets
            foreach (var item in statements)
            {
                //this is the element that will be added to the tree
                //we will create this based ont he values we find in the stream and wrap /unwrap as needed based on what we find
                Element CurrentElement = null;
                //we will keep track of the token index for validation
                var tokenindex = 0;

                foreach (var token in item)
                {
                    tokenindex++;
                    Element tempElem = null;
                    switch (token.TokenType)
                    {
                        case LexedTokenType.DELIMITEDTEXT:
                        case LexedTokenType.TEXT:
                            {
                                if (tempElem != null)
                                {
                                    throwParserException(token, "Unexpected Text");
                                }
                                if (token.TokenText == ".")
                                {
                                    tempElem = new DotSelector();
                                }
                                else
                                {
                                    tempElem = new SpecifiedEventNameSelector()
                                    {
                                        EventName = token.TokenText
                                    };
                                }

                                
                                break;
                            }


                        case LexedTokenType.REGEX:
                            {
                                if (tempElem != null)
                                {
                                    throwParserException(token, "Unexpected Regex");
                                }

                                tempElem = new RegexSelector()
                                {
                                    MatchPattern = token.TokenText
                                };
                               
                                break;
                            }


                        case LexedTokenType.EXCLAMATION:
                            {
                                if (tokenindex > 1)
                                {
                                    throwParserException(token, "Negation must be the first symbol on a line");
                                }
                                tempElem = new NegatedElement();
                                break;
                            }
                        case LexedTokenType.PLUS:
                        case LexedTokenType.QUESTIONMARK:
                        case LexedTokenType.STAR:
                            {
                                tempElem = new SymbolQuantifier()
                                {
                                    QuantifierSymbol = token.TokenText[0]
                                };

                                break;
                            }
                        case LexedTokenType.STATEMENTEND:

                            {
                                if(CurrentElement == null)
                                {
                                    throwParserException(token, "Expecting Selector");
                                }
                                p.AddElement(CurrentElement);
                                CurrentElement = null;
                                tempElem = null;
                                break;

                            }
                        default:
                            break;
                    }

                    if(tempElem != null)
                    {
                        CurrentElement = tempElem.ReturnParseTreeFromExistingElement(CurrentElement);
                    }
                }
                
            }
            return p;
        }

        private Element AddTempElemToCurrentElement(Element ParentElement, Element tempElem,LexedToken t)
        {
            switch (ParentElement)
            {
                case ContainerElement c:
                    {
                        c.ContainedElement = tempElem;
                        return c;
                    }
                case null:
                    {
                        return tempElem;
                    }
            }

            throwParserException(t, "Unexpected token");
            return null;
        }

        private void throwParserException(LexedToken token, string v)
        {
            throw new ParseException($"{v} Token: {token.TokenType}. At Row: {token.LineNumber} , Position: {token.Position}");
        }

        /// <summary>
        /// returns a list of lists of tokens that make up statements in this group
        /// for now will split on new lines
        /// </summary>
        /// <returns></returns>
        public static List<LexedStatment> SplitTokensIntoStatements(IEnumerable<LexedToken> Splittables)
        {
            var result = new List<LexedStatment>();
            var currentStatemnt = new LexedStatment();

            //loop along allong token and split into statements on new lines
            foreach (var item in Splittables)
            {
                //if(item.TokenType == LexedTokenType.)
                if (item.TokenType == LexedTokenType.NEWLINE)
                {
                    currentStatemnt.Add(new LexedToken(LexedTokenType.STATEMENTEND, "", item.LineNumber, item.Position));
                    result.Add(currentStatemnt);
                    currentStatemnt = new LexedStatment();
                }
                else if(item.TokenType != LexedTokenType.EOF)
                {
                    
                    currentStatemnt.Add(item);
                }
            }

            //if the last item has statement add it anyways
            if (currentStatemnt.Count > 0)
            {
                currentStatemnt.Add(new LexedToken(LexedTokenType.STATEMENTEND, "", 0, 0));
                result.Add(currentStatemnt);
            }

            return result;
        }
    }

    [Serializable]
    internal class ParseException : Exception
    {
        public ParseException()
        {
        }

        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class LexedStatment : List<LexedToken>
    {
        public bool HasNegation { get; set; }
    }
}
