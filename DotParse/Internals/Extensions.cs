using System;
using System.Collections.Generic;

namespace Revn.DotParse.Internals
{
    internal static class Extensions
    {
        public static IEnumerable<string> SplitByLine(this string text)
        {
            return text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
