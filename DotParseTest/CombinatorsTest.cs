using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Revn.DotParse.Parsers.CharParsers;
using Revn.DotParse;

namespace DotParseTest
{
    [TestClass]
    public class CombinatorsTest
    {
        [TestMethod]
        public void SeqTest()
        {
            Parser<string, char?> testParser = Letter.Seq(Digit, Digit).Map(chars => new string(chars));
            
            testParser(new CharSource("abc")).AssertFailed(FailedReason.NotSatisfy);
            testParser(new CharSource("123")).AssertFailed(FailedReason.NotSatisfy);

            testParser(new CharSource("a23")).AssertSuccess("a23");
            testParser(new CharSource("a234")).AssertSuccess("a23");
        }

        [TestMethod]
        public void RepeatTest()
        {
            Parser<string, char?> testParser = Letter.Repeat(3).Map(chars => new string(chars));
            testParser(new CharSource("abc")).AssertSuccess("abc");
            testParser(new CharSource("123")).AssertFailed(FailedReason.NotSatisfy);
        }
    }
}
