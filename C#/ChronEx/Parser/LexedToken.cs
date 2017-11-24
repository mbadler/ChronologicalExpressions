﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ChronEx.Parser
{
    public struct LexedToken
    {
        //many single chars can be easilly converted to token with a simple lookup
        public static Dictionary<char, LexedTokenType> charToToken_Dictionary = new Dictionary<char, LexedTokenType>()
        {
            {' ',LexedTokenType.WHITESPACE },
            {'\t',LexedTokenType.TAB },
            {'!',LexedTokenType.EXCLAMATION},
            {'{',LexedTokenType.OPENCURLY },
            {'}',LexedTokenType.CLOSECURLY },
            {',',LexedTokenType.COMMA }
        };

        public LexedToken(LexedTokenType TokenType,string TokenText,int LineNumber,int Position)
        {
            this.TokenType = TokenType;
            this.TokenText = TokenText;
            this.LineNumber = LineNumber;
            this.Position = Position;
        }

        public LexedToken(LexedTokenType TokenType)
        {
            this.TokenType = TokenType;
            this.TokenText = "";
            this.LineNumber = 0;
            this.Position = 0;

        }

        public LexedTokenType TokenType { get; }
        public string TokenText { get; }
        public int LineNumber { get; }
        public int Position { get; }
    }

    public enum LexedTokenType
    {
        TEXT,
        DELIMITEDTEXT,
        REGEX,
        NEWLINE,
        BOF,
        UNKNOWN,
        WHITESPACE,
        TAB,
        EOF,
        EXCLAMATION,
        STATEMENTEND,
        OPENCURLY,
        CLOSECURLY,
        COMMA,
        NUMBER
    }
}
