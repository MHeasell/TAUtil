namespace TAUtil
{
    /// <summary>
    /// An exception thrown when an error occurs during TDF parsing.
    /// </summary>
    public class TdfParseException : ParseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TdfParseException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="lineNumber">The line number on which the error occurred.</param>
        /// <param name="columnNumber">The column number on which the error occurred.</param>
        public TdfParseException(string message, int lineNumber, int columnNumber)
            : base(message)
        {
            this.LineNumber = lineNumber;
            this.ColumnNumber = columnNumber;
        }

        /// <summary>
        /// Gets or sets the line number on which the parsing error occurred.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the column number on which the parsing error occurred.
        /// </summary>
        public int ColumnNumber { get; set; }
    }
}
