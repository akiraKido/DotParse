using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotParse;

namespace DotParseTest
{
    internal static class MSTestEx
    {
        public static void Is<T>(this T actual, T expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void AssertSuccess<TResult, TSource>(this ParseResult<TResult, TSource> result, TResult expected)
        {
            result.IsSuccess.Is(true);
            (result as Success<TResult, TSource>).Value.Is(expected);
        }

        public static void AssertFailed<TResult, TSource>(this ParseResult<TResult, TSource> result, string reason)
        {
            result.IsFailed.Is(true);
            (result as Failed<TResult, TSource>).Reason.Is(reason);
        }
    }
}
