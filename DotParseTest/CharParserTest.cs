using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotParse;
using static DotParse.Parsers.CharParsers;

namespace DotParseTest
{
    [TestClass]
    public class CharParserTest
    {
        [TestMethod]
        public void AnyCharTest()
        {
            var source = "abc";
            AssertSuccess(AnyChar, source, 'a');
        }

        [TestMethod]
        public void SatisfyTest()
        {
            var source = "abc";

            AssertSuccess(Satisfy(c => c == 'a'), source, 'a');
            AssertFailed(Satisfy(c => c == 'b'), source);
        }

        [TestMethod]
        public void CharTest()
        {
            var parser = Char('a');
            
            AssertSuccess(parser, "abc", 'a');
            AssertFailed(parser, "123");
        }

        [TestMethod]
        public void DigitTest()
        {
            AssertFailed(Digit, "abc");
            AssertSuccess(Digit, "123", '1');
        }

        [TestMethod]
        public void UpperTest()
        {
            AssertSuccess(Upper, "Abc", 'A');
            AssertFailed(Upper, "abc");
        }

        [TestMethod]
        public void LowerTest()
        {
            AssertSuccess(Lower, "abc", 'a');
            AssertFailed(Lower, "Abc");
        }

        [TestMethod]
        public void AlphaTest()
        {
            AssertSuccess(Alpha, "abc", 'a');
            AssertSuccess(Alpha, "ABC", 'A');
            AssertFailed(Alpha, "123");
        }

        [TestMethod]
        public void AlphaNumTest()
        {
            AssertSuccess(AlphaNum, "abc", 'a');
            AssertSuccess(AlphaNum, "ABC", 'A');
            AssertSuccess(AlphaNum, "123", '1');
            AssertFailed(AlphaNum, "?22");
        }

        [TestMethod]
        public void LetterTest()
        {
            AssertSuccess(Letter, "abc", 'a');
            AssertSuccess(Letter, "ABC", 'A');
            AssertFailed(Letter, "123");
            AssertFailed(Letter, "?22");
        }

        [TestMethod]
        public void EmptySourceTest()
        {
            AssertFailed(AnyChar, "");
        }

        private static void AssertSuccess(Parser<char, char?> parser, string source, char expected)
        {
            var result = parser(new CharSource(source));

            result.IsSuccess.Is(true);
            (result as Success<char, char?>).Value.Is(expected);
        }

        private static void AssertFailed(Parser<char, char?> parser, string source)
        {
            parser(new CharSource(source)).IsFailed.Is(true);
        }
    }
}
