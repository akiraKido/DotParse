namespace DotParse
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
    }
}
