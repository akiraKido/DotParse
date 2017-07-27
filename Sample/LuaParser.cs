using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revn.DotParse;
using static Revn.DotParse.Parsers.StringParsers;

namespace Sample
{

    [TestClass]
    public class LuaParserTest
    {
        [TestMethod]
        public void LuaParseSample()
        {
            var str = LuaParser.Parse("function hoge(test)\n    X = 10\nend");
            Assert.AreEqual(str?[0].Value, "hoge");
        }
    }

    internal static class LuaParser
    {
        private static readonly Parser<string, string> Identifier
            = Regex("[a-zA-Z][a-zA-Z0-9]*");

        private static readonly Parser<string, LuaObject> LuaNumber
            = Regex("[0-9|.]+")
                .Map(n => new LuaNumber(decimal.Parse(n)) as LuaObject);

        private static readonly Parser<string, LuaObject> LuaString
            = Regex("\".*\"")
                .Map(s => new LuaString(s.Substring(1, s.Length - 2)) as LuaObject);

        private static readonly Parser<string, LuaObject[]> Literal
            = LuaNumber.Or(LuaString)
                .Map(item => new[] {item});


        private static readonly Parser<string, string> WhiteSpace
            = Satisfy(s => (char.IsWhiteSpace(s[0]), string.Empty, char.IsWhiteSpace(s[0]) ? 1 : 0));

        private static TA RemoveWhiteSpace<TA, TB>((TA _a, TB _b) ab) => ab._a;

        private static (TA a, TB b, TC c) Flatten<TA, TB, TC>(((TA _a, TB _b) ab, TC _c) tuple) =>
            (tuple.ab._a, tuple.ab._b, tuple._c);


        private static readonly Parser<string, LuaObject[]> LuaAssignmentExpression
            = Identifier
                .SeqT(Char('='))
                .SeqT(Literal)
                .Map(Flatten)
                .Map(item => new[] {new LuaAssignmentExpression(item.a, item.c[0]) as LuaObject});

        private static readonly Parser<string, LuaObject[]> Expression
            = LuaAssignmentExpression;

        private static readonly Parser<string, LuaFunction[]> LuaFunction
            = Skip("function")
                .SeqT(Identifier)        .Map(item => item.b)
                .SeqT(Char('('))         .Map(item => item.a)
                .SeqT(Identifier)        .Map(item => new {Name = item.a, Identifier = item.b})
                .SeqT(Char(')'))         .Map(item => item.a)
                .SeqT(Expression.Many()) .Map(item => new {item.a.Name, item.a.Identifier, Expressions = item.b})
                .SeqT(Match("end"))      .Map(item => item.a)
                .Map(item => new[] {new LuaFunction(item.Name, new [] {item.Identifier}, item.Expressions.First())});

        internal static LuaFunction[] Parse(string s)
        {
            return ( LuaFunction(new LuaCodeSource(s)) as Success<string, LuaFunction[]> )?.Value;
        }
    }

    internal class LuaCodeSource : StringLineSource
    {
        public LuaCodeSource(string text) : base(text) { }

        private LuaCodeSource(IReadOnlyList<string> lines, int line, int i) : base(lines, line, i) { }

        public override ISource<string> ToNext(int count)
        {
            int lineIndex = Line;
            string line = Lines[Line];
            int index = Position + count;

            while (true)
            {
                if (index >= line.Length)
                {
                    index = 0;
                    if (++lineIndex >= Lines.Count) break;
                    line = Lines[lineIndex];
                }
                if (!char.IsWhiteSpace(line[index])) break;
                index++;
            }
            return new LuaCodeSource(Lines, lineIndex, index);
        }
    }

    internal abstract class LuaObject
    {
        protected object _value;
        internal object Value => _value;

        public override string ToString()
        {
            return _value.ToString();
        }
    }

    internal class LuaCompilerHelper
    {
        internal object Value { get; }

        internal LuaCompilerHelper(object value)
        {
            Value = value;
        }
    }

    internal class LuaString : LuaObject
    {
        internal LuaString(string value)
        {
            _value = value;
        }
    }

    internal class LuaNumber : LuaObject
    {
        internal LuaNumber(decimal value)
        {
            _value = value;
        }
    }

    internal class LuaAssignmentExpression : LuaObject
    {
        internal LuaAssignmentExpression(string name, LuaObject assignment)
        {
            Name = name;
            AssignmentObject = assignment;
            _value = $"{name} = {assignment.ToString()}";
        }

        internal string Name { get; }
        internal LuaObject AssignmentObject { get; }
    }

    internal class LuaFunction : LuaObject
    {
        internal LuaFunction(string name, IEnumerable<string> parameters, IEnumerable<LuaObject> expressions)
        {
            Name = name;
            Parameters = parameters;
            Expressions = expressions;
            _value = name;
        }

        internal string Name { get; }
        internal IEnumerable<string> Parameters { get; }
        internal IEnumerable<LuaObject> Expressions { get; }
    }

}