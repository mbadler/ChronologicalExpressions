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
    }
}
