namespace DotParse
{
    /// <summary>
    /// Parser
    /// </summary>
    public delegate ParseResult<TResult, TSource> Parser<TResult, TSource>(ISource<TSource> source);
}
