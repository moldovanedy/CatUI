using System;
using System.Collections.Generic;
using System.Text;

namespace CatUI.Shared
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
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < textWithHyphens.Length; i++)
            {
                if (textWithHyphens[i] != '\u00ad')
                {
                    sb.Append(textWithHyphens[i]);
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
            List<int> shyPositions = new List<int>();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r' && i == text.Length - 1)
                {
                    throw new ArgumentException("Invalid text: found CR (\\r) without LF (\\n)", text);
                }

                if (text[i] == '\u00ad')
                {
                    shyPositions.Add(i);
                    continue;
                }
            }

            return shyPositions;
        }
    }
}