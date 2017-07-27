using System;
using System.Collections.Generic;
using System.Linq;
using Revn.DotParse.Internals;

namespace Revn.DotParse
{
    public class CharSource : ISource<char?>
    {
        private readonly string _str;

        private readonly int _pos;

        public CharSource(string s)
            : this(s, 0)
        {
        }

        private CharSource(string s, int pos)
        {
            _str = s;
            _pos = pos;
        }

        public char? Peek()
            => _pos < _str.Length ? (char?)_str[_pos] : null;

        public ISource<char?> ToNext() => new CharSource(_str, _pos + 1);
        public ISource<char?> ToNext(int pos) => throw new NotImplementedException();
    }

    public class StringLineSource : ISource<string>
    {
        protected readonly IReadOnlyList<string> Lines;

        protected readonly int Line;
        protected readonly int Position;

        public StringLineSource(string text)
        {
            Lines = text.SplitByLine().ToList().AsReadOnlyList();
            Line = 0;
            Position = 0;
        }

        protected StringLineSource(IReadOnlyList<string> lines, int line, int position)
        {
            Lines = lines;
            Line = line;
            Position = position;
        }

        public virtual string Peek()
        {
            return Line < Lines.Count ? Lines[Line].Substring(Position) : null;
        }

        /// <summary>
        /// 次の行に進みます
        /// </summary>
        /// <returns></returns>
        public virtual ISource<string> ToNext()
        {
            return new StringLineSource(Lines, Line + 1, 0);
        }

        /// <summary>
        /// 指定した文字数分先にすすめます
        /// </summary>
        /// <param name="count">なん文字消費したか</param>
        /// <returns></returns>
        public virtual ISource<string> ToNext(int count)
        {
            return Position + count < (Lines[Line].Length - 1)
                ? new StringLineSource(Lines, Line, Position + count)
                : new StringLineSource(Lines, Line + 1, 0);
        }
    }
}
