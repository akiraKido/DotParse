﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Revn.DotParse
{
    public static class Combinators
    {
        public static Parser<TSource, TResult[]> Seq<TSource, TResult>(this Parser<TSource, TResult> first, params Parser<TSource, TResult>[] second)
        {
            Parser<TSource, TResult>[] parsers = new[] {first}.Concat(second).ToArray();
            return source =>
            {
                ParseResult<TSource, TResult>[] results = SeqInner(parsers, source).ToArray();
                ParseResult<TSource, TResult> last = results[results.Length - 1];

                return last.IsSuccess 
                    ? last.Source.ToSuccess(results.Select(r => r.Value).ToArray()) 
                    : last.Source.ToFailed<TSource, TResult[]>(last.Reason);
            };
        }

        public static Parser<TSource, TResult[]> Repeat<TSource, TResult>(this Parser<TSource, TResult> parser, int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            Parser<TSource, TResult>[] parsers =
                count == 1 ? new[] { parser } : Enumerable.Repeat(parser, count - 1).ToArray();

            return parser.Seq(parsers);
        }

        public static Parser<TSource, TResult[]> Any<TSource, TResult>(this Parser<TSource, TResult> parser)
            => source =>
            {
                ParseResult<TSource, TResult> result = parser(source);
                if (result.IsFailed)
                {
                    return result.Source.ToSuccess(new TResult[] { });
                }

                var results = new List<TResult>();

                while (result.IsSuccess)
                {
                    while (result.IsSuccess)
                    {
                        results.Add(result.Value);
                        result = parser(result.Source);
                    }
                }

                return result.Source.ToSuccess(results.ToArray());
            };

        public static Parser<TSource, TResult[]> Many<TSource, TResult>(this Parser<TSource, TResult> parser)
        {
            return source =>
                        {
                            var results = new List<TResult>();

                            ParseResult<TSource, TResult> parseResult = parser(source);
                            if (parseResult.IsFailed)
                            {
                                return parseResult.Source.ToNotSatisfy<TSource, TResult[]>();
                            }

                while (true)
                {
                    results.Add(parseResult.Value);
                    var nextResult = parser(parseResult.Source);
                    if (!nextResult.IsSuccess) break;
                    parseResult = nextResult;
                }

                            return parseResult.Source.ToSuccess(results.ToArray());
                        };
        }

        public static Parser<TSource, TResult> Skip<TSource, TResult>(
            this Parser<TSource, TResult> parser, Parser<TSource, TResult> predicate)
        {
            return source =>
            {
                var result1 = parser(source);
                if (result1.IsFailed) return result1.Source.ToFailed<TSource, TResult>(result1.Reason);

                var predicateResult = predicate(source);
                return predicateResult.IsSuccess
                    ? result1.Source.ToSuccess(predicateResult.Value)
                    : predicateResult.Source.ToFailed<TSource, TResult>(predicateResult.Reason);
            };
        }
        
        
        public static Parser<TSource, TResult> SkipAll<TSource, TResult>(this Parser<TSource, TResult> parser)
        {
            return source =>
            {
                var result = parser(source);
                while (result.IsSuccess)
                {
                    result = parser(result.Source);
                }

                return result.Source.ToSuccess(default(TResult));
            };
        }

        public static Parser<TSource, TResult> Or<TSource, TResult>(this Parser<TSource, TResult> left, Parser<TSource, TResult> right)
        {
            return source =>
                        {
                            ParseResult<TSource, TResult> leftResult = left(source);
                            return leftResult.IsSuccess ? leftResult : right(source);
                        };
        }

        private static IEnumerable<ParseResult<TSource, TResult>> SeqInner<TSource, TResult>(
            Parser<TSource, TResult>[] parsers, ISource<TSource> source)
        {
            ParseResult<TSource, TResult> tmpResult = parsers[0](source);
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

        public static Parser<TSource, (TResultA a, TResultB b)> SeqT<TSource, TResultA, TResultB>(
            this Parser<TSource, TResultA> first, Parser<TSource, TResultB> second)
        {
            return source =>
                        {
                // try parse first
                ParseResult<TSource, TResultA> firstResult = first(source);
                            if (firstResult.IsFailed)
                            {
                                return firstResult.Source.ToFailed<TSource, (TResultA a, TResultB b)>(firstResult.Reason);
                            }

                // try parse second
                ParseResult<TSource, TResultB> secondResult = second(firstResult.Source);
                            if (secondResult.IsFailed)
                            {
                                return secondResult.Source.ToFailed<TSource, (TResultA a, TResultB b)>(secondResult.Reason);
                            }

                            return secondResult.Source.ToSuccess((firstResult.Value, secondResult.Value));
                        };
        }

        public static Parser<TSource, TResultB> Map<TSource, TResultA, TResultB>(this Parser<TSource, TResultA> parser, Func<TResultA, TResultB> mapper)
        {
            return source =>
                        {
                            ParseResult<TSource, TResultA> result = parser(source);
                            if (result.IsFailed)
                            {
                                return result.Source.ToFailed<TSource, TResultB>(result.Reason);
                            }
                            return result.Source.ToSuccess(mapper(result.Value));
                        };
        }

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
