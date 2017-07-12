using Microsoft.VisualStudio.TestTools.UnitTesting;
using Revn.DotParse;

namespace DotParseTest
{
    internal static class MSTestEx
    {
        public static void Is<T>(this T actual, T expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void AssertSuccess<TSource, TResult>(this ParseResult<TSource, TResult> result, TResult expected)
        {
            result.IsSuccess.Is(true);
            (result as Success<TSource, TResult>).Value.Is(expected);
        }

        public static void AssertFailed<TSource, TResult>(this ParseResult<TSource, TResult> result, string reason)
        {
            result.IsFailed.Is(true);
            (result as Failed<TSource, TResult>).Reason.Is(reason);
        }
    }
}
