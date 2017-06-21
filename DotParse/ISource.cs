namespace Revn.DotParse
{
    public interface ISource<out T>
    {
        T Peek();
        ISource<T> ToNext();
    }
}