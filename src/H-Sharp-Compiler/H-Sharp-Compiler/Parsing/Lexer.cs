using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using HSharp.IO;

namespace HSharp.Parsing {
    
    public class Lexer {

        static string[] definiteKeywords = new string[] { // words guaranteed to be keywords
            "if",
            "for",
            "foreach",
            "do",
            "while",
            "match",
            "case",
            "type",
            "using",
            "class",
            "struct",
            "object",
            "interface",
            "union",
            "enum",
            "const",
            "static",
            "public",
            "private",
            "protected",
            "external",
            "internal",
            "abstract",
            "override",
            "virtual",
            "final",
            "true",
            "false",
            "null"
        };

        static string[] operators = new string[] {
            @"\+", "-", @"\*", "/", "%", "=", @"\|", "&", ";", ",", @"\.",
            @"\(", @"\)", @"\[", @"\]", @"\{", @"\}", @"\""", @"'", ":",
            "<", ">", "#", @"\?"
        };

        private static string __regpat = null;

        private static string CreatePattern() {
            if (__regpat is null) {
                string regOp = $"(?<op>{string.Join('|', operators)})";
                string regInt = @"(?<ilit>\d+)";
                string regFloat = @"(?<flit>\d+\.\d+f?)"; // floating point number (Single, Double)
                string regId = @"(?<id>[a-zA-Z_][a-zA-Z0-9_]*)";
                __regpat = $"{regInt}|{regFloat}|{regOp}|{regId}";
                return __regpat;
            } else {
                return __regpat;
            }
        }

        public LexToken[] Lex(string sourceFilePath) {

            // Load all text
            string text = File.ReadAllText(sourceFilePath);

            // Parse line data
            Range[] lnRanges = this.GetLineRanges(text);

            // Remove comments
            text = this.RemoveComments(text);

            // Create pattern and match
            string pattern = CreatePattern();
            var matching = Regex.Matches(text, pattern);

            // Create token list
            List<LexToken> tokens = new List<LexToken>();

            // Create tokens from regex match
            foreach (Match match in matching) {

                string type = "0"; // find out what regex group was matched
                foreach (Group g in match.Groups) {
                    if (!string.IsNullOrEmpty(g.Value) && g.Name.CompareTo("0") != 0) {
                        type = g.Name;
                    }
                }

                uint ln = (uint)Array.IndexOf(lnRanges, lnRanges.First(x => x.Start.Value <= match.Index && match.Index <= x.End.Value));
                uint cl = (uint)(match.Index - lnRanges[ln].Start.Value) + 1;
                ln++;

                switch (type) {
                    case "id":
                        if (definiteKeywords.Contains(match.Value)) {
                            if (match.Value.CompareTo("false") == 0 || match.Value.CompareTo("true") == 0) {
                                tokens.Add(new LexToken(LexTokenType.BoolLiteral, match.Value, new SourcePosition(ln, cl)));
                            } else if (match.Value.CompareTo("null") == 0) {
                                tokens.Add(new LexToken(LexTokenType.NullLiteral, match.Value, new SourcePosition(ln, cl)));
                            } else {
                                tokens.Add(new LexToken(LexTokenType.Keyword, match.Value, new SourcePosition(ln, cl)));
                            }
                        } else {
                            tokens.Add(new LexToken(LexTokenType.Identifier, match.Value, new SourcePosition(ln, cl)));
                        }
                        break;
                    case "ilit":
                        tokens.Add(new LexToken(LexTokenType.IntLiteral, match.Value, new SourcePosition(ln, cl)));
                        break;
                    case "flit":

                        break;
                    case "op":
                        if (match.Value.CompareTo(";") == 0 || match.Value.CompareTo(",") == 0) {
                            tokens.Add(new LexToken(LexTokenType.Separator, match.Value, new SourcePosition(ln, cl)));
                        } else if (match.Value.CompareTo("(") == 0) {
                            tokens.Add(new LexToken(LexTokenType.ExpressionStart, match.Value, new SourcePosition(ln, cl)));
                        } else if (match.Value.CompareTo(")") == 0) {
                            tokens.Add(new LexToken(LexTokenType.ExpressionEnd, match.Value, new SourcePosition(ln, cl)));
                        } else if (match.Value.CompareTo("[") == 0) {
                            tokens.Add(new LexToken(LexTokenType.IndexerStart, match.Value, new SourcePosition(ln, cl)));
                        } else if (match.Value.CompareTo("]") == 0) {
                            tokens.Add(new LexToken(LexTokenType.IndexerEnd, match.Value, new SourcePosition(ln, cl)));
                        } else if (match.Value.CompareTo("{") == 0) {
                            tokens.Add(new LexToken(LexTokenType.BlockStart, match.Value, new SourcePosition(ln, cl)));
                        } else if (match.Value.CompareTo("}") == 0) {
                            tokens.Add(new LexToken(LexTokenType.BlockEnd, match.Value, new SourcePosition(ln, cl)));
                        } else {
                            tokens.Add(new LexToken(LexTokenType.Operator, match.Value, new SourcePosition(ln, cl)));
                        }
                        break;
                    default:
                        break;
                }

            }

            return tokens.ToArray();

        }

        private string RemoveComments(string str) => str;

        private Range[] GetLineRanges(string str) {

            string lnendMkr = Environment.NewLine;
            int i = str.IndexOf(Environment.NewLine);
            if (i == -1) {
                lnendMkr = "\n";
                i = str.IndexOf(lnendMkr);
                if (i == -1) {
                    return new Range[] { new Range(0, str.Length) };
                }
            }

            List<int> lnEnds = new List<int>();
            do {

                lnEnds.Add(i);
                i = str.IndexOf(lnendMkr, i + 1);

            } while (i != -1);

            lnEnds.Add(str.Length); // add incase there's no empty last-line

            Range[] lineRanges = new Range[lnEnds.Count];
            for (int j = 0; j < lnEnds.Count; j++) {
                if (j - 1 >= 0) {
                    lineRanges[j] = new Range(lnEnds[j-1] + lnendMkr.Length, lnEnds[j]);
                } else {
                    lineRanges[j] = new Range(0, lnEnds[j]);
                }
            }

            return lineRanges;

        }

    }

}
