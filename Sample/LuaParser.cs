using System;
using System.Linq;
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
            var str = LuaParser.Parse( "X = 10" );
            Assert.AreEqual(str[0].Value, "X = 10");
        }
    }
    
    internal static class LuaParser
    {
        private static readonly Parser<string, LuaObject> LuaNumber
            = Regex( "0-9" )
                .Map( n => new LuaNumber( decimal.Parse( n ) ) as LuaObject);
        
        private static readonly Parser<string, LuaObject> LuaString
            = Regex("\".*\"")
                .Map(s => new LuaString(s.Substring(1, s.Length - 2) ) as LuaObject);

        private static (TA a, TB b, TC c) Flatten<TA, TB, TC>(((TA _a, TB _b) ab, TC _c) tuple) =>
            (tuple.ab._a, tuple.ab._b, tuple._c);

        private static readonly Parser<string, LuaObject[]> LuaAssignmentExpression
            = Regex("[a-zA-Z][a-zA-Z0-9]?")
                .SkipWhiteSpace()
                .SeqT(Regex("="))
                .SeqT(Literal)
                .Map(Flatten)
                .Map(item => new[] { new LuaAssignmentExpression(item.a, item.c[0]) as LuaObject });

        private static readonly Parser<string, LuaObject[]> Literal
            = LuaNumber.Or(LuaString)
                .Map(item => new[] { item });
        
        internal static LuaObject[] Parse( string s )
        {
            return ( LuaAssignmentExpression( new StringLineSource( s ) ) as Success<string, LuaObject[]> )?.Value;
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
    
    internal class LuaString : LuaObject
    {
        internal LuaString( string value )
        {
            _value = value;
        }
    }

    internal class LuaNumber : LuaObject
    {
        internal LuaNumber( decimal value )
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
    
    
}