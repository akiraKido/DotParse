using System;

namespace Revn.DotParse.Parsers
{
    public static class CharParsers
    {
        /// <summary>
        /// 任意の一文字を読むパーサ
        /// </summary>
        public static readonly Parser<char, char?> AnyChar = Satisfy(_ => true);

        /// <summary>
        /// 条件付きで一文字読むパーサ
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Parser<char, char?> Satisfy(Func<char, bool> predicate)
            => source =>
                {
                    char? next = source.Peek();

                    return next.HasValue
                        ? predicate(next.Value)
                            ? source.ToNext().ToSuccess(next.Value)
                            : source.ToNotSatisfy<char, char?>()
                        : source.ToEndOfSource<char, char?>();
                };

        /// <summary>
        /// 指定した文字に一致するか判定するパーサ
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char, char?> Char(char ch)
            => Satisfy(c => c == ch);

        public static bool IsAlpha(char ch)
            => char.IsLower(ch) || char.IsUpper(ch);

        public static bool IsAlphaNum(char ch)
            => IsAlpha(ch) || char.IsNumber(ch);

        public static Parser<char, char?> Digit = Satisfy(char.IsDigit);
        public static Parser<char, char?> Upper = Satisfy(char.IsUpper);
        public static Parser<char, char?> Lower = Satisfy(char.IsLower);
        public static Parser<char, char?> Alpha = Satisfy(IsAlpha);
        public static Parser<char, char?> AlphaNum = Satisfy(IsAlphaNum);
        public static Parser<char, char?> Letter = Satisfy(char.IsLetter);
    }
}
