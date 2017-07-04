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
        private readonly IReadOnlyList<string> _lines;

        private readonly int _line;
        private readonly int _pos;

        public StringLineSource(string text)
        {
            _lines = text.SplitByLine().ToList().AsReadOnlyList();
            _line = 0;
            _pos = 0;
        }

        private StringLineSource(IReadOnlyList<string> lines, int line, int pos)
        {
            _lines = lines;
            _line = line;
            _pos = pos;
        }

        public string Peek()
        {
            return _lines.Count < _line ? _lines[_line].Substring(_pos) : null;
        }

        public ISource<string> ToNext()
        {
            return new StringLineSource(_lines, _line + 1, 0);
        }

        /// <summary>
        /// 指定した文字数分先にすすめます
        /// </summary>
        /// <param name="count">なん文字消費したか</param>
        /// <returns></returns>
        public ISource<string> ToNext(int count)
        {
            return _pos + count < (_lines[_line].Length - 1)
                ? new StringLineSource(_lines, _line, _pos + count)
                : new StringLineSource(_lines, _line + 1, 0);
        }
    }
}
