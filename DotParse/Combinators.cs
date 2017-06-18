using System;
using System.Collections.Generic;
using System.Linq;

namespace DotParse
{
    public static class Combinators
    {
        public static Parser<TResult[], TSource> Seq<TResult, TSource>(this Parser<TResult, TSource> first,
            Parser<TResult, TSource> second)
            => source =>
            {
                ParseResult<TResult, TSource> firstResult = first(source);

                if (firstResult is Success<TResult, TSource> firstSuccess)
                {
                    ParseResult<TResult, TSource> secondResult = second(firstResult.Source);

                    if (secondResult is Success<TResult, TSource> secondSuccess)
                    {
                        return secondSuccess.Source.ToSuccess(new[] {firstSuccess.Value, secondSuccess.Value});
                    }

                    var secondFailed = secondResult as Failed<TResult, TSource>;
                    return secondFailed.Source.ToFailed<TResult[], TSource>(secondFailed.Reason);
                }

                var firstFailed = firstResult as Failed<TResult, TSource>;
                return firstFailed.Source.ToFailed<TResult[], TSource>(firstFailed.Reason);
            };

        public static Parser<(TResultA a, TResultB b), TSource> SeqT<TResultA, TResultB, TSource>(
            this Parser<TResultA, TSource> first, Parser<TResultB, TSource> second)
            => source =>
            {
                ParseResult<TResultA, TSource> firstResult = first(source);

                if (firstResult is Success<TResultA, TSource> firstSuccess)
                {
                    ParseResult<TResultB, TSource> secondResult = second(firstResult.Source);

                    if (secondResult is Success<TResultB, TSource> secondSuccess)
                    {
                        return secondSuccess.Source.ToSuccess<(TResultA a, TResultB b), TSource>((firstSuccess.Value, secondSuccess.Value));
                    }

                    var secondFailed = secondResult as Failed<TResultB, TSource>;
                    return secondFailed.Source.ToFailed<(TResultA a, TResultB b), TSource>(secondFailed.Reason);
                }

                var firstFailed = firstResult as Failed<TResultA, TSource>;
                return firstFailed.Source.ToFailed<(TResultA a, TResultB b), TSource>(firstFailed.Reason);
            };

        public static Parser<TResult[], TSource> Seq<TResult, TSource>(this Parser<TResult[], TSource> first,
            Parser<TResult, TSource> second)
            => source =>
            {
                ParseResult<TResult[], TSource> firstResult = first(source);

                if (firstResult is Success<TResult[], TSource> firstSuccess)
                {
                    ParseResult<TResult, TSource> secondResult = second(firstResult.Source);

                    if (secondResult is Success<TResult, TSource> secondSuccess)
                    {
                        return secondSuccess.Source.ToSuccess(firstSuccess.Value.Concat(secondSuccess.Value).ToArray());
                    }

                    var secondFailed = secondResult as Failed<TResult, TSource>;
                    return secondFailed.Source.ToFailed<TResult[], TSource>(secondFailed.Reason);
                }

                return firstResult as Failed<TResult[], TSource>;
            };

        public static Parser<TResult[], TSource> Repeat<TResult, TSource>(this Parser<TResult, TSource> parser,
            int count)
            => source =>
            {
                if (count < 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                Parser<TResult[], TSource> repeat = parser.Map(r => new[] {r});

                for (var i = 1; i < count; i++)
                {
                    repeat = repeat.Seq(parser);
                }

                return repeat(source);
            };

        public static Parser<TResultB, TSource> Map<TResultA, TResultB, TSource>(this Parser<TResultA, TSource> parser,
            Func<TResultA, TResultB> mapper)
            => source =>
            {
                ParseResult<TResultA, TSource> result = parser(source);
                if (result is Success<TResultA, TSource> success)
                {
                    return success.Source.ToSuccess(mapper(success.Value));
                }

                var failed = result as Failed<TResultA, TSource>;
                return failed.Source.ToFailed<TResultB, TSource>(failed.Reason);
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
