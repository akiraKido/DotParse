using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revn.DotParse;
using static Revn.DotParse.Parsers.CharParsers;

namespace Sample
{
    [TestClass]
    public class PostalCodeParserTest
    {
        [TestMethod]
        public void PostalCodeSample()
        {
            PostalCode result = PostalCodeParser.Parse("123-4567");
            Assert.AreEqual(result.Region, 123);
            Assert.AreEqual(result.Office, 4567);
        }
    }

    internal static class PostalCodeParser
    {
        private static readonly Func<char[], int> CharsToInt = chars => int.Parse(new string(chars));

        private static readonly Parser<char?, int> RegionParser = Digit.Repeat(3).Map(CharsToInt);
        private static readonly Parser<char?, int> OfficeParser = Digit.Repeat(4).Map(CharsToInt);

        private static (TA a, TB b, TC c) Flatten<TA, TB, TC>(((TA _a, TB _b) ab, TC _c) tuple) =>
            (tuple.ab._a, tuple.ab._b, tuple._c);


        private static Parser<char?, PostalCode> CodeParser =
            RegionParser
            .SeqT(Char('-'))
            .SeqT(OfficeParser)
            .Map(Flatten)
            .Map(t => new PostalCode(t.a, t.c));

        public static PostalCode Parse(string str)
            => (CodeParser(new CharSource(str)) as Success<char?, PostalCode>)?.Value;
    }

    internal class PostalCode
    {
        public int Region { get; }
        public int Office { get; }

        public PostalCode(int region, int office)
        {
            this.Region = region;
            this.Office = office;
        }

        public override string ToString() => $"{this.Region}-{this.Office}";
    }
}
