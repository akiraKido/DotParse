using System.Collections;
using System.Collections.Generic;

namespace Revn.DotParse.Internals
{
    internal class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IList<T> _list;

        public ReadOnlyList(IList<T> list)
        {
            _list = list;
        }

        public IEnumerator<T> GetEnumerator()
            => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        public int Count => _list.Count;

        public T this[int index] => _list[index];
    }

    internal static class ReadOnlyListHelper
    {
        public static ReadOnlyList<T> AsReadOnlyList<T>(this IList<T> list)
            => new ReadOnlyList<T>(list);
    }
}
