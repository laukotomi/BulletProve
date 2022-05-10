using System;

namespace LTest.ExtensionMethods
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class StringExtensionMethods
    {
        /// <summary>
        /// If a string ends with the specified value then removes it.
        /// </summary>
        /// <param name="input">The string that should be trimmed.</param>
        /// <param name="suffixToRemove">String that should be removed.</param>
        /// <param name="comparisonType"><see cref="StringComparison"/>.</param>
        public static string TrimEnd(this string input, string suffixToRemove, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            if (!string.IsNullOrEmpty(suffixToRemove) && input.EndsWith(suffixToRemove, comparisonType))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }

            return input;
        }

        /// <summary>
        /// Truncates string if too long.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <param name="maxChars">Maximum length.</param>
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }
    }
}