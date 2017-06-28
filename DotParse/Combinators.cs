using System;
using System.Collections.Generic;
using System.Linq;

namespace Revn.DotParse
{
    public static class Combinators
    {
        public static Parser<TResult[], TSource> Seq<TResult, TSource>(this Parser<TResult, TSource> first,
            params Parser<TResult, TSource>[] second)
        {
            Parser<TResult, TSource>[] parsers = new[] {first}.Concat(second).ToArray();
            return source =>
            {
                ParseResult<TResult, TSource>[] results = SeqInner(parsers, source).ToArray();
                ParseResult<TResult, TSource> last = results[results.Length - 1];

                return last.IsSuccess 
                    ? last.Source.ToSuccess(results.Select(r => r.Value).ToArray()) 
                    : last.Source.ToFailed<TResult[], TSource>(last.Reason);
            };
        }

        public static Parser<TResult[], TSource> Repeat<TResult, TSource>(this Parser<TResult, TSource> parser,
            int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            Parser<TResult, TSource>[] parsers = count == 1 ? new[] {parser} : Enumerable.Repeat(parser, count - 1).ToArray();

            return parser.Seq(parsers);
        }

        public static Parser<TResult[], TSource> Many<TResult, TSource>(this Parser<TResult, TSource> parser)
            => source =>
            {
                var results = new List<TResult>();

                ParseResult<TResult, TSource> parseResult = parser(source);

                while (parseResult.IsSuccess)
                {
                    results.Add(parseResult.Value);
                    parseResult = parser(parseResult.Source);
                }

                return parseResult.Source.ToSuccess(results.ToArray());
            };

        public static Parser<TResult, TSource> Or<TResult, TSource>(this Parser<TResult, TSource> left, Parser<TResult, TSource> right)
            => source =>
            {
                ParseResult<TResult, TSource> leftResult = left(source);
                return leftResult.IsSuccess ? leftResult : right(source);
            };

        private static IEnumerable<ParseResult<TResult, TSource>> SeqInner<TResult, TSource>(
            Parser<TResult, TSource>[] parsers, ISource<TSource> source)
        {
            ParseResult<TResult, TSource> tmpResult = parsers[0](source);
            yield return tmpResult;

            for (var i = 1; i < parsers.Length; i++)
            {
                if (tmpResult.IsFailed)
                {
                    yield break;
                }

                tmpResult = parsers[i](tmpResult.Source);
                yield return tmpResult;
            }
        }

        public static Parser<(TResultA a, TResultB b), TSource> SeqT<TResultA, TResultB, TSource>(
            this Parser<TResultA, TSource> first, Parser<TResultB, TSource> second)
            => source =>
            {
                // try parse first
                ParseResult<TResultA, TSource> firstResult = first(source);
                if (firstResult.IsFailed)
                {
                    return firstResult.Source.ToFailed<(TResultA a, TResultB b), TSource>(firstResult.Reason);
                }

                // try parse second
                ParseResult<TResultB, TSource> secondResult = second(firstResult.Source);
                if (secondResult.IsFailed)
                {
                    return secondResult.Source.ToFailed<(TResultA a, TResultB b), TSource>(secondResult.Reason);
                }

                return secondResult.Source.ToSuccess((firstResult.Value, secondResult.Value));
            };

        public static Parser<TResultB, TSource> Map<TResultA, TResultB, TSource>(this Parser<TResultA, TSource> parser,
            Func<TResultA, TResultB> mapper)
            => source =>
            {
                ParseResult<TResultA, TSource> result = parser(source);
                if (result.IsFailed)
                {
                    return result.Source.ToFailed<TResultB, TSource>(result.Reason);
                }
                return result.Source.ToSuccess(mapper(result.Value));
            };


        private static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T added)
        {
            foreach (T e in source)
            {
                yield return e;
            }

            yield return added;
        }
    }
}
