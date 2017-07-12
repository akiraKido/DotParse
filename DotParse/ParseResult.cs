using System;

namespace Revn.DotParse
{
    public abstract class ParseResult<TSource, TResult>
    {
        public ISource<TSource> Source { get; }

        public virtual TResult Value => throw new InvalidOperationException();

        public abstract bool IsSuccess { get; }
        public abstract bool IsFailed { get; }
        public virtual string Reason => throw new InvalidOperationException();

        protected ParseResult(ISource<TSource> source)
        {
            this.Source = source;
        }
    }

    public class Success<TSource, TResult> : ParseResult<TSource, TResult>
    {
        public Success(TResult value, ISource<TSource> source)
            : base(source)
        {
            this.Value = value;
        }

        public override TResult Value { get; }

        public override bool IsSuccess => true;
        public override bool IsFailed => false;
    }

    public class Failed<TSource, TResult> : ParseResult<TSource, TResult>
    {
        public Failed(ISource<TSource> source, string reason)
            : base(source)
        {
            this.Reason = reason;
        }

        public override string Reason { get; }

        public override bool IsSuccess => false;
        public override bool IsFailed => true;
    }

    public static class FailedReason
    {
        public const string EndOfSource = "end of source";
        public const string NotSatisfy = "not satisfy";
    }

    internal static class ParseResultExtensions
    {
        public static ParseResult<TSource, TResult> ToFailed<TSource, TResult>(this ISource<TSource> source, string reason) => new Failed<TSource, TResult>(source, reason);

        public static ParseResult<TSource, TResult> ToEndOfSource<TSource, TResult>(this ISource<TSource> source) =>
            source.ToFailed< TSource, TResult>(FailedReason.EndOfSource);

        public static ParseResult<TSource, TResult> ToNotSatisfy<TSource, TResult>(this ISource<TSource> source) =>
            source.ToFailed<TSource, TResult>(FailedReason.NotSatisfy);

        public static ParseResult<TSource, TResult> ToSuccess<TSource, TResult>(this ISource<TSource> source, TResult value) => new Success<TSource, TResult>(value, source);
    }
}
