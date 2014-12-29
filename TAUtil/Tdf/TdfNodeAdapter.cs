namespace TAUtil.Tdf
{
    using System.Collections.Generic;

    /// <summary>
    /// <para>
    /// Adapter for the <see cref="TdfParser"/> class.
    /// </para>
    /// <para>
    /// This adapter constructs a tree of <see cref="TdfNode"/> objects
    /// from the parsed TDF file.
    /// </para>
    /// </summary>
    public class TdfNodeAdapter : ITdfNodeAdapter
    {
        private readonly Stack<TdfNode> nodeStack = new Stack<TdfNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TdfNodeAdapter"/> class.
        /// </summary>
        public TdfNodeAdapter()
        {
            this.RootNode = new TdfNode();
            this.nodeStack.Push(this.RootNode);
        }

        /// <summary>
        /// Gets the root node of the <see cref="TdfNode"/> tree.
        /// After parsing is finished, this node will contain all the parsed data.
        /// </summary>
        public TdfNode RootNode { get; private set; }

        /// <summary>
        /// See <see cref="ITdfNodeAdapter.Initialize"/>.
        /// </summary>
        /// <param name="parser">A reference to the parser.</param>
        public void Initialize(TdfParser parser)
        {
        }

        /// <summary>
        /// See <see cref="ITdfNodeAdapter.BeginBlock"/>.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        public void BeginBlock(string name)
        {
            TdfNode n = new TdfNode(name);
            this.nodeStack.Peek().Keys[name] = n;
            this.nodeStack.Push(n);
        }

        /// <summary>
        /// See <see cref="ITdfNodeAdapter.AddProperty"/>.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void AddProperty(string name, string value)
        {
            this.nodeStack.Peek().Entries[name] = value;
        }

        /// <summary>
        /// See <see cref="ITdfNodeAdapter.EndBlock"/>.
        /// </summary>
        public void EndBlock()
        {
            this.nodeStack.Pop();
        }
    }
}
