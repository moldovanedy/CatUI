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
            var hasHyphens = false;
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

            for (var i = 0; i < text.Length; i++)
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
    }
}
