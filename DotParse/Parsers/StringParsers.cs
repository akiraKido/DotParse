using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Revn.DotParse.Parsers
{
    public static class StringParsers
    {
        public static Parser<string, string> Satisfy(Func<string, (bool isSuccess, string result, int count)> func)
            => source =>
            {
                string line = source.Peek();
                if (line == null)
                {
                    return source.ToEndOfSource<string, string>();
                }
                (bool isSuccess, string result, int count) = func(line);

                return isSuccess
                    ? source.ToNext(count).ToSuccess(result)
                    : source.ToNotSatisfy<string, string>();
            };

        /// <summary>
        /// 先頭と一致した場合先頭から指定した文字列を取り出すパーサ
        /// </summary>
        /// <param name="literal"></param>
        /// <returns></returns>
        public static Parser<string, string> Match(string literal)
            => Satisfy(str =>
            {
                bool match = str.StartsWith(literal);

                return (match, (match ? str.Substring(0, literal.Length) : null), (match ? literal.Length : 0));
            });

        /// <summary>
        /// 先頭と一致した場合先頭から指定した文字列リテラルをスキップするパーサ
        /// </summary>
        /// <param name="literal"></param>
        /// <returns></returns>
        public static Parser<string, string> Skip(string literal)
            => Satisfy(str => (str.StartsWith(literal), string.Empty, literal.Length));
        
        /// <summary>
        /// あらゆる文字列をうけつけるパーサ
        /// </summary>
        public static readonly Parser<string, string> AnyString = Satisfy(_ => (true, _, _.Length));

        /// <summary>
        /// 任意の一文字をうけつけるパーサ
        /// </summary>
        public static readonly Parser<string, string> AnyChar = Satisfy(_ => (true, _.Substring(0, 1), 1));

        /// <summary>
        /// 指定された文字に一致する一文字を受け付けるパーサ
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<string, string> Char(char c)
            => Satisfy(str => (str[0] == c, str.Substring(0, 1), 1));

        /// <summary>
        /// 指定された文字に一致する一文字を受け付けるパーサ
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<string, string> Char(params char[] c)
            => Satisfy(str => (c.Contains(str[0]), str.Substring(0, 1), 1));

        /// <summary>
        /// 先頭が正規表現と一致するか判定するパーサ
        /// </summary>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static Parser<string, string> Regex(string regex)
            => Satisfy(str =>
            {
                Regex r = new Regex(ToTopMatchRegex(regex));
                Match match = r.Match(str);
                bool success = match.Success;
                string value = success ? match.Value : null;

                return (success, value, value?.Length ?? 0);
            });


        private static string ToTopMatchRegex(string regex)
            => regex[0] == '^' ? regex : $"^{regex}";
    }
}
