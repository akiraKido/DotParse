namespace Revn.DotParse
{
    /// <summary>
    /// Parser
    /// </summary>
    public delegate ParseResult<TSource, TResult> Parser<TSource, TResult>(ISource<TSource> source);
}
