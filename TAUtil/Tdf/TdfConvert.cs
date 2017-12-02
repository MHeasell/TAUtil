namespace TAUtil.Tdf
{
    using System;

    /// <summary>
    /// Contains utility methods for converting to and from
    /// TDF string values.
    /// </summary>
    public static class TdfConvert
    {
        /// <summary>
        /// Converts the given integer to a string.
        /// </summary>
        /// <param name="i">The integer to convert.</param>
        /// <returns>The string representation of the integer.</returns>
        public static string ToString(int i)
        {
            return Convert.ToString(i);
        }

        /// <summary>
        /// Converts the given boolean value to a string.
        /// </summary>
        /// <param name="b">
        /// The boolean value to convert.
        /// </param>
        /// <returns>
        /// The string representation of the boolean value.
        /// </returns>
        public static string ToString(bool b)
        {
            return b ? "1" : "0";
        }

        /// <summary>
        /// Converts the given double-precision floating-point value to a string.
        /// </summary>
        /// <param name="d">
        /// The double to convert.
        /// </param>
        /// <returns>
        /// The string representation of the double.
        /// </returns>
        public static string ToString(double d)
        {
            return Convert.ToString(d);
        }

        /// <summary>
        /// Converts the given string representation of a boolean value
        /// to a raw boolean value.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>The boolean value represented by the string.</returns>
        public static bool ToBool(string s)
        {
            return !(string.IsNullOrEmpty(s) || s == "0");
        }

        /// <summary>
        /// Converts the given string representation of an integer value
        /// to an integer.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>The integer value represented by the string.</returns>
        public static int ToInt32(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return 0;
            }

            return Convert.ToInt32(s);
        }

        /// <summary>
        /// Attempts to convert the given string to an integer.
        /// If parsing succeeds, result will contain the converted value.
        /// Otherwise, output will be set to 0.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="result">The variable into which the parsed int will be placed.</param>
        /// <returns>true if conversion succeeded, otherwise false.</returns>
        public static bool TryToInt32(string s, out int result)
        {
            return int.TryParse(s, out result);
        }

        /// <summary>
        /// Converts the given string representation of a decimal number
        /// to a double-precision floating-point value.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>The double value represented by the string.</returns>
        public static double ToDouble(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return 0.0;
            }

            return Convert.ToDouble(s);
        }
    }
}
