using ChronEx.Models;
using ChronEx.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChronEx.Tests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void Lex_Text_OneLineOfTextReturnsTEXTToken()
        {
            var n = new Lexer("abc");
            var res = n.returnlist;
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(LexedTokenType.TEXT, res[0].TokenType);
            Assert.AreEqual("abc", res[0].TokenText);
            Assert.AreEqual(LexedTokenType.EOF, res[1].TokenType);
        }

        [TestMethod]
        public void Lex_Text_TwoLinesOfTextReturn2TextsWithLineBreak()
        {
            var n = new Lexer( @"abc
def");
           
            var res = n.returnlist;
            Assert.AreEqual(4, res.Count);
            Assert.AreEqual(LexedTokenType.TEXT, res[0].TokenType);
            Assert.AreEqual("abc", res[0].TokenText);
            Assert.AreEqual(LexedTokenType.NEWLINE, res[1].TokenType);
            Assert.AreEqual(LexedTokenType.TEXT, res[2].TokenType);
            Assert.AreEqual("def", res[2].TokenText);
            Assert.AreEqual(LexedTokenType.EOF, res[3].TokenType);

        }

        [TestMethod]
        public void Lex_DelimText_TwoLinesOfDelimtextReturn2LinesPlusBreak()
        {

            var s = "'abc'\r\n'def'";
            var n = new Lexer(s);
           
            var res = n.returnlist;
            Assert.AreEqual(4, res.Count);
            Assert.AreEqual(LexedTokenType.DELIMITEDTEXT, res[0].TokenType);
            Assert.AreEqual("abc", res[0].TokenText);
            Assert.AreEqual(LexedTokenType.NEWLINE, res[1].TokenType);
            Assert.AreEqual(LexedTokenType.DELIMITEDTEXT, res[2].TokenType);
            Assert.AreEqual("def", res[2].TokenText);
            Assert.AreEqual(LexedTokenType.EOF, res[3].TokenType);

        }

        [TestMethod]
        public void Lex_DelimText_WithEmbededEscapeChars()
        {
           
            var s = "'abc\\'\\r\\n\\'def'";
            var n = new Lexer(s);

           
            var res = n.returnlist;
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(LexedTokenType.DELIMITEDTEXT, res[0].TokenType);
            Assert.AreEqual("abc'\\r\\n'def", res[0].TokenText);
           

        }

        [TestMethod]
        public void Lex_Regex_ReturnRegex()
        {
            var s = "/thisisaregex/\n";
            var n = new Lexer(s);
            

         
            var res = n.returnlist;
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(LexedTokenType.REGEX, res[0].TokenType);
            Assert.AreEqual("thisisaregex", res[0].TokenText);


        }

        [TestMethod]
        public void Lex_Regex_EmbeddedRegexEscapeDoesNotGetChanged()
        {
           
            var s = "/thisis\\[aregex/";
            var n = new Lexer(s);

            var res = n.returnlist;
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(LexedTokenType.REGEX, res[0].TokenType);
            Assert.AreEqual("thisis\\[aregex", res[0].TokenText);


        }

        [TestMethod]
        public void Lex_Regex_EmbeddedEscapedSlashDoesNotEndRegex()
        {
            
            var s = "/thisis\\/aregex/";
            var n = new Lexer(s);
           
            var res = n.returnlist;
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(LexedTokenType.REGEX, res[0].TokenType);
            Assert.AreEqual("thisis/aregex", res[0].TokenText);


        }

        [TestMethod]
        public void Lex_Number_ReturnsNumber()
        {
            var s = "1234567 ABC";
            var n = new Lexer(s);

            var res = n.returnlist;
            Assert.AreEqual(4, res.Count);
            Assert.AreEqual(LexedTokenType.NUMBER, res[0].TokenType);
            Assert.AreEqual("1234567", res[0].TokenText);

        }

        [TestMethod]
        public void Lex_Number_SpecialCharBreaksNumberEvenIfAttached()
        {
            var s = "1234567{}ABC";
            var n = new Lexer(s);



            var res = n.returnlist;
            Assert.AreEqual(5, res.Count);
            Assert.AreEqual(LexedTokenType.NUMBER, res[0].TokenType);
            Assert.AreEqual("1234567", res[0].TokenText);
            Assert.AreEqual(LexedTokenType.OPENCURLY, res[1].TokenType);


        }

        [TestMethod]
        public void Lex_Number_DecimalNumbersCaptureOK()
        {
            var s = "12345.67 ABC";
            var n = new Lexer(s);



            var res = n.returnlist;
            Assert.AreEqual(4, res.Count);
            Assert.AreEqual(LexedTokenType.NUMBER, res[0].TokenType);
            Assert.AreEqual("12345.67", res[0].TokenText);


        }

        [TestMethod]
        public void Lex_Number_TrailingDecimalFailsWithException()
        {
            var s = "1234567. ABC";
            Assert.ThrowsException<ParserException>(() =>
            {
                 var n = new Lexer(s);
            });

        }

        [TestMethod]
        public void Lex_Number_2DotsInNumberFail()
        {
            var s = "12345.67.4 ABC";
           
            Assert.ThrowsException<ParserException>(() =>
            {
                var n = new Lexer(s);
            });

        }

        [TestMethod]
        public void Lex_Number_CharInNumberFails()
        {
            var s = "12345.67ABC";
            Assert.ThrowsException<ParserException>(() =>
            {
                var n = new Lexer(s);
            });

        }
    }
}
