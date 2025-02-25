using System;
using System.Collections.Generic;
using System.Text;

namespace CatUI.Utils
{
    public static class TextUtils
    {
        /// <summary>
        /// Will remove the soft hyphens (U+00ad) if any, returning true if there were hyphens, false otherwise.
        /// The text without the hyphens will be returned in clearText if it had hyphens, otherwise clearText is an empty string.
        /// </summary>
        /// <param name="textWithHyphens">The text that has hyphens.</param>
        /// <param name="clearText">
        /// The text cleared from any hyphens. Is an empty string when there were no hyphens (to avoid useless allocations).
        /// </param>
        /// <returns>True if there were hyphens, false otherwise.</returns>
        public static bool RemoveSoftHyphens(string textWithHyphens, out string clearText)
        {
            bool hasHyphens = false;
            var sb = new StringBuilder();
            foreach (char chr in textWithHyphens)
            {
                if (chr != '\u00ad')
                {
                    sb.Append(chr);
                }
                else
                {
                    hasHyphens = true;
                }
            }

            if (hasHyphens)
            {
                clearText = sb.ToString();
                return true;
            }
            else
            {
                clearText = "";
                return false;
            }
        }

        public static List<int> GetSoftHyphensPositions(string text)
        {
            List<int> shyPositions = new();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r' && i == text.Length - 1)
                {
                    throw new ArgumentException("Invalid text: found CR (\\r) without LF (\\n)", text);
                }

                if (text[i] == '\u00ad')
                {
                    shyPositions.Add(i);
                }
            }

            return shyPositions;
        }

        /// <summary>
        /// Returns true if the character is a character considered as a newline by Unicode, false otherwise.
        /// </summary>
        /// <remarks>
        /// True only for LINE FEED (U+000A), CARRIAGE RETURN (U+000D), FORM FEED (U+000C), VERTICAL TABULATION (U+000B),
        /// NEXT LINE (U+0085), LINE SEPARATOR (U+2028) and PARAGRAPH SEPARATOR (U+2029). CRLF must be treated as a single
        /// entity, but this method does not take 2 characters at once, so you must create a separate case for CR+LF.
        /// </remarks>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is a character considered as a newline by Unicode, false otherwise.</returns>
        public static bool IsUnicodeNewline(char c)
        {
            //in order: LINE FEED, CARRIAGE RETURN, FORM FEED, VERTICAL TABULATION, NEXT LINE, LINE SEPARATOR,
            //PARAGRAPH SEPARATOR
            return
                c == '\n' ||
                c == '\r' ||
                c == '\f' ||
                c == '\v' ||
                c == '\u0085' ||
                c == '\u2028' ||
                c == '\u2029';
        }

        /// <summary>
        /// Returns true if the character is a whitespace and the text can be "broken" at this position, meaning the
        /// rest of the characters can be placed on a new line if necessary (when using word wrap for example).
        /// Warning! If the character is not a white space, this method returns false.
        /// </summary>
        /// <remarks>
        /// This returns false for any non-whitespace and for NO-BREAK SPACE (U+00A0), FIGURE SPACE (U+2007),
        /// NARROW NO-BREAK SPACE (U+202F), WORD JOINER (U+2060) and ZERO WIDTH NON-BREAKING SPACE (U+FEFF). 
        /// </remarks>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the text can be broken on this character as of Unicode standard, false otherwise.</returns>
        public static bool CanWhitespaceBreakText(char c)
        {
            return
                char.IsWhiteSpace(c) &&
                c != '\u00a0' &&
                c != '\u2007' &&
                c != '\u2027' &&
                c != '\u2060' &&
                c != '\ufeff';
        }
    }
}
