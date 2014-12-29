namespace TAUtil.Tdf
{
    /// <summary>
    /// Adapter for the <see cref="TdfParser"/> class.
    /// Users of <see cref="TdfParser"/> should implement this interface
    /// to control how TDF records are processed during parsing.
    /// </summary>
    public interface ITdfNodeAdapter
    {
        /// <summary>
        /// Creates a new TDF block as a child of the current block.
        /// The created block becomes the new current block.
        /// </summary>
        /// <param name="name">The name of the block to create.</param>
        void BeginBlock(string name);

        /// <summary>
        /// Adds a property entry to the current block.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        void AddProperty(string name, string value);

        /// <summary>
        /// Indicates that the current block has finished
        /// and sets the current block to be the parent of the current block.
        /// </summary>
        void EndBlock();

        /// <summary>
        /// Indicates that parsing is about to begin.
        /// This allows the implementation to obtain a reference to the parser
        /// and perform any required initialization logic.
        /// </summary>
        /// <param name="parser">The parser object.</param>
        void Initialize(TdfParser parser);
    }
}
