namespace TAUtil
{
    using System;

    /// <summary>
    /// The exception that is thrown when a reader is unable to parse input,
    /// usually because of errors in the input.
    /// </summary>
    [Serializable]
    public class ParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        public ParseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="message">
        /// A message explaining why the exception occurred.
        /// </param>
        public ParseException(string message)
            : base(message)
        {
        }
    }
}
