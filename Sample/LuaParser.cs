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
            LuaString str = LuaParser.Parse( "\"hoge\"" );
            Assert.AreEqual(str.Value, "hoge");
        }
    }
    
    internal static class LuaParser
    {
        private static readonly Func<char[], string> CharsToString =
            cs => cs.Aggregate( string.Empty, ( current, c ) => current + c );

        private static readonly Parser<string, LuaString> LuaString
            = Regex("\".*\"")
                .Map(s => new LuaString(s.Substring(1, s.Length - 2) ));

        internal static LuaString Parse( string s )
        {
            return ( LuaString( new StringLineSource( s ) ) as Success<string, LuaString> )?.Value;
        }
    }

    internal class LuaString
    {
        internal LuaString( string value )
        {
            this.Value = value;
        }
        
        internal string Value { get; }
    }
    
}