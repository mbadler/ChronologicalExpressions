using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ChronEx.Parser
{
    /// <summary>
    /// This class takes a string pattern and lexes it into a list of token that the parser can use
    /// </summary>
    public class Lexer
    {
        


        public Lexer(string Pattern)
        {
            LexPattern(Pattern);
        }
        char[] PatternArray = null;

        int CursorPos = 0; //the position in the pattern where the cursor is
        int PeekAheadCount = 0; //how far ahead of the cursor is the peek 
        StringBuilder buffer = new StringBuilder(); //contains the accumulate selected chars, reused to mimize allocations
        LexedTokenType PersumedTokenType = LexedTokenType.BOF;
        public List<LexedToken> returnlist = new List<LexedToken>();
        /// <summary>
        /// The position of the text in the current line
        /// </summary>
        int CurLinePos = 1;
        /// <summary>
        /// The current line the cursor is on
        /// </summary>
        int CurLineNum = 1; // 


        public char Current
        {
            get
            {
                return PatternArray[CursorPos];
            }
        }

        public bool IsEOF
        {
            get
            {
                return CursorPos == PatternArray.Length;
            }
        }

        private void LexPattern(string Pattern)
        {
            
            PatternArray = Pattern.ToCharArray();

            //walk thru each char in the pattern and build the tokens
            while (CursorPos < PatternArray.Length)
            {

                //if we are at the begining of the file or row
                if (isBOL())
                {
                    LexBOL();
                    continue;
                }

                switch (PersumedTokenType)
                {
                    case LexedTokenType.REGEX:
                        {
                            LexRegex();
                            break;
                        }
                    case LexedTokenType.TEXT:
                        {
                            LexText();
                            break;
                        }
                        
                    case LexedTokenType.DELIMITEDTEXT:
                        {
                            LexDelimitedText();
                            break;
                        }
                    case LexedTokenType.NUMBER:
                        {
                            LexNumber();
                            break;
                        }
                    case LexedTokenType.NEWLINE:
                        break;
                    case LexedTokenType.BOF:
                        break;
                    default:
                        break;
                }
            }

            // if we poped out of the loop and we still have a persumed type then 
            // finish off the persumed toke
            if(PersumedTokenType != LexedTokenType.UNKNOWN)
            {
                CaptureToken(PersumedTokenType);
            }
            //Always add a EOF token
            CaptureToken(LexedTokenType.EOF);
        }

        private void LexNumber()
        {
            //if the next char is a ending (line , space tab or special char)
            // then allow this function to lex it and exit
            if (LexForTokenEndingChar(LexedTokenType.NUMBER))
            {
                return;
            }

            //if its a . then we should still accept it for decimal however we need to esure that
            //there is at least 1 digit after that
            if(Current=='.')
            {
                //first check the captured buffer to make sure we dont have a . yet

                if(buffer.ToString().Contains("."))
                {
                    throwParserException("Unexpected Dot");
                }

                //peek ahead and throw if the next char is not a number
                var next = Peek();
                 
                if(!next.HasValue || !Char.IsDigit(next.Value))
                {
                    throwParserException("Uexpected Dot");
                }
            }

            //if we got here then we know its not a special char,breaking char or dot
            //so it needs to be a number
            if(!Char.IsDigit(Current)&& Current != '.')
            {
                throwParserException("Unexpected char In Number");
            }
            //else just add it to the buffer
            Select();
        }

        private void LexDelimitedText()
        {
            // if its another delimiter then close the text
            if(Current=='\'')
            {
                CaptureToken(LexedTokenType.DELIMITEDTEXT);
                Skip();
                PersumedTokenType = LexedTokenType.UNKNOWN;
                return;
            }

            //if its an escape char then peek forward to see if its a ' and captur only that
             
            if(Current == '\\' && Peek()=='\'')
            {
                //don't need the slash cause its only escaping
                Skip();
                    
            }

            if(Current== '\r' || Current == '\n')
            {
                throwParserException("Unterminated Delimited Text");
            }

            Select();
             
        }

        private void LexText()
        {
            

            //if the next char is a ending (line , space tab or special char)
            // then allow this function to lex it and exit
            if(LexForTokenEndingChar(LexedTokenType.TEXT))
            {
                return;
            }

            //else just add it to the buffer
            Select();
        }

        private bool LexForTokenEndingChar(LexedTokenType RecordAsType)
        {
            // if its a new line char - send it to the new line capturer
            if (Current == '\n' || Current == '\r')
            {
                CaptureToken(RecordAsType);
                CaptureNewLine();
                return true;
            }

            // if its a space or tab then captur the current value
            if (Current == ' ' || Current == '\t')
            {
                //capture the buffer but dont' skip the white space - allow it to go back into the loop for processing
                CaptureToken(RecordAsType);
                PersumedTokenType = LexedTokenType.UNKNOWN;
                return true;
            }

            //next we check for any of the special chars - if found then close shop and let the the
            //lexSpecialChar function take care of it
            if (LexedToken.charToToken_Dictionary.ContainsKey(Current))
            {
                CaptureToken(RecordAsType);
                PersumedTokenType = LexedTokenType.UNKNOWN;
                LexSpecialChars();
                return true;
            }

            //its not a token ending char - let teh caller deal with it
            return false;
        }

        private void LexRegex()
        {

            //for regex capture all chars until we reach another /

            // hoever if we find a escaping backslash , then check to see if it is followed by a forward slash
            if (Current == '\\')
            {
                var nextchar = Peek();
                if (nextchar==null)
                {
                    // this is a error
                    //return and allow the loop to throw the exception
                    return;
                }
                if(nextchar=='/')
                {
                    //this is an ecscape char for a forward slash
                    //skip the escape char
                    Skip();
                    //and select the forward slash
                    Select();
                    return;
                }
            }
            // if the char is a forward lsash then we know that the reges is over
            if(Current=='/')
            {
                //capture the buffer as a regex
                CaptureToken(LexedTokenType.REGEX);
                //we don't need to capture the slash
                Skip();
                PersumedTokenType = LexedTokenType.UNKNOWN;
                return;
            }

            //you can have a new line char in a regex
            if(Current=='\n')
            {
                throwParserException("Regex expression must be closed");
            }
            //else its just a char select it into the buffer
            Select();

        }

        private void throwParserException(string v)
        {
            throw new ParserException(v);
        }

        //moves the lookahead pointer -1 backwords
        private char? Poke()
        {
            PeekAheadCount--;
            if(CursorPos+PeekAheadCount < 0)
            {
                return null;
            }
            return PatternArray[CursorPos + PeekAheadCount];
        }

        // moves the lookahead pointer +1 forward
        private char? Peek()
        {
            PeekAheadCount++;
            if (CursorPos + PeekAheadCount >= PatternArray.Length)
            {
                return null;
            }
            return PatternArray[CursorPos + PeekAheadCount];
        }

        private void LexBOL()
        {
             //Line starts with a slash - its a regex
            if(Current == '/')
            {
               
                //we don't need to capture the slash
                Skip();
                PersumedTokenType = LexedTokenType.REGEX;
                return;
            }
            // if its a single qoute then its a delimieted text
            if (Current == '\'')
            {
                //Line starts with a slash - its a regex
                //we don't need to capture the single quote
                Skip();
                PersumedTokenType = LexedTokenType.DELIMITEDTEXT;
                return;
            }

            //if its a newline then capture it as a toke and exit
            if(Current=='\n' || Current == '\r')
            {
                CaptureNewLine();
                return;
            }

            // if its a delimieted Text
            if(Current == '\'')
            {
                Skip();//no need to capture the deimilater
                PersumedTokenType = LexedTokenType.DELIMITEDTEXT;
                return;
            }

            //if its a number
            if(Char.IsDigit(Current))
            {
                Select();
                PersumedTokenType = LexedTokenType.NUMBER;
                return;
            }


            //see if its one of the specila chars we have mappings for
            if(!LexSpecialChars())
            {
                //else we will just assume its string
                Select();
                PersumedTokenType = LexedTokenType.TEXT;
            }

           


        }

        private bool LexSpecialChars()
        {
            // check if the special char mappings has this token if not reurhtn
            if(!LexedToken.charToToken_Dictionary.ContainsKey(Current))
            {
                return false;
            }

            //select and capture the token based onthe mapping and set it back to unknown
            var b = Current;
            Select();
            CaptureToken(LexedToken.charToToken_Dictionary[b]);
            PersumedTokenType = LexedTokenType.UNKNOWN;
            return true;
        }

        private bool isBOL()
        {
            return PersumedTokenType == LexedTokenType.BOF || PersumedTokenType == LexedTokenType.NEWLINE || PersumedTokenType == LexedTokenType.UNKNOWN;
        }

        //skips to the next char without capturing it in the buffer
        // rests the lookahead
        protected void Skip()
        {
            PeekAheadCount = 0;
            CursorPos++;
        }

        //captures the current char into the buffer and moves ahead
        // resets the lookahead
        protected void Select()
        {
            buffer.Append(PatternArray[CursorPos]);
            PeekAheadCount = 0;
            CursorPos++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TokenType"></param>
        //Captures whatever is in the buffer into a new token which is placed on the token stream
        //reset the buffer;
        protected void CaptureToken(LexedTokenType TokenType)
        {
            var tkn = new LexedToken(TokenType, buffer.ToString(), CurLinePos, CurLineNum);
            resetBuffer();
            returnlist.Add(tkn);


        }

        /// <summary>
        /// Captures a new line as a token , willincrment line number and handle \r\n pair
        /// </summary>
        void CaptureNewLine()
        {
            //select whatever char triggerd
            Select();
            //if the next is /n then its a pair and selet the /n
            if(!IsEOF && Current == '\n')
            {
                Select();
            }
            CaptureToken(LexedTokenType.NEWLINE);
            PersumedTokenType = LexedTokenType.UNKNOWN;
            CurLineNum++;
            CurLinePos = 1;
        }
        private void resetBuffer()
        {
            buffer.Clear();
            PeekAheadCount = 0;
        }
    }

    [Serializable]
    public class ParserException : Exception
    {
        public ParserException()
        {
        }

        public ParserException(string message) : base(message)
        {
        }

        public ParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
