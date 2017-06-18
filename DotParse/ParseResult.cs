namespace DotParse
{
    public abstract class ParseResult<TResult, TSource>
    {
        public ISource<TSource> Source { get; }

        public abstract bool IsSuccess { get; }
        public abstract bool IsFailed { get; }

        protected ParseResult(ISource<TSource> source)
        {
            this.Source = source;
        }
    }

    public class Success<TResult, TSource> : ParseResult<TResult, TSource>
    {
        public TResult Value { get; }

        public Success(TResult value, ISource<TSource> source)
            : base(source)
        {
            this.Value = value;
        }

        public override bool IsSuccess => true;
        public override bool IsFailed => false;
    }

    public class Failed<TResult, TSource> : ParseResult<TResult, TSource>
    {
        public Failed(ISource<TSource> source, string reason)
            : base(source)
        {
            this.Reason = reason;
        }

        public string Reason { get; }

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
        public static ParseResult<TResult, TSource> ToFailed<TResult, TSource>(this ISource<TSource> source, string reason) => new Failed<TResult, TSource>(source, reason);

        public static ParseResult<TResult, TSource> ToEndOfSource<TResult, TSource>(this ISource<TSource> source) =>
            source.ToFailed< TResult, TSource>(FailedReason.EndOfSource);

        public static ParseResult<TResult, TSource> ToNotSatisfy<TResult, TSource>(this ISource<TSource> source) =>
            source.ToFailed<TResult, TSource>(FailedReason.NotSatisfy);

        public static ParseResult<TResult, TSource> ToSuccess<TResult, TSource>(this ISource<TSource> source, TResult value) => new Success<TResult, TSource>(value, source);
    }
}
