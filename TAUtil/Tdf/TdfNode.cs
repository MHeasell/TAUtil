namespace TAUtil.Tdf
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Tree structure representing a hierarchy of keys and values
    /// as in a TDF format file.
    /// </summary>
    public class TdfNode
    {
        private const int IndentationLevel = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="TdfNode"/> class.
        /// The created node has a name of null.
        /// </summary>
        public TdfNode()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TdfNode"/> class.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        public TdfNode(string name)
        {
            this.Name = name;
            this.Keys = new Dictionary<string, TdfNode>(StringComparer.OrdinalIgnoreCase);
            this.Entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets or sets the name of the node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the mapping of keys in this node.
        /// Each key maps to a TdfNode with the same name representing a child block.
        /// </summary>
        public Dictionary<string, TdfNode> Keys { get; private set; }

        /// <summary>
        /// Gets the mapping of entries in this node.
        /// Each item is a property with a name and associated value.
        /// </summary>
        public Dictionary<string, string> Entries { get; private set; }

        /// <summary>
        /// Reads a TDF file from the given stream into a TdfNode.
        /// </summary>
        /// <param name="s">The stream to read from.</param>
        /// <returns>A TdfNode containing the read data.</returns>
        public static TdfNode LoadTdf(Stream s)
        {
            return LoadTdf(new StreamReader(s));
        }

        /// <summary>
        /// Reads a TDF file from the given reader into a TdfNode.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <returns>A TdfNode containing the read data.</returns>
        public static TdfNode LoadTdf(TextReader reader)
        {
            var adapter = new TdfNodeAdapter();
            var parser = new TdfParser(reader, adapter);
            parser.Load();
            return adapter.RootNode;
        }

        /// <summary>
        /// Writes the contents of this TdfNode to the given stream
        /// in TDF format.
        /// </summary>
        /// <param name="s">The stream to write to.</param>
        public void WriteTdf(Stream s)
        {
            StreamWriter wr = new StreamWriter(s);
            this.WriteTdf(wr, 0);
            wr.Flush();
        }
        public bool ContentsEqual(TdfNode other)
        {
            if (!string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (this.Keys.Count != other.Keys.Count)
            {
                return false;
            }

            if (this.Entries.Count != other.Entries.Count)
            {
                return false;
            }

            foreach (var entry in this.Entries)
            {
                if (!other.Entries.TryGetValue(entry.Key, out var value) || entry.Value != value)
                {
                    return false;
                }
            }

            foreach (var entry in this.Keys)
            {
                if (!other.Keys.TryGetValue(entry.Key, out var value) || !entry.Value.ContentsEqual(value))
                {
                    return false;
                }
            }

            return true;
        }

        private void WriteTdf(StreamWriter writer, int depth)
        {
            string indent = new string(' ', depth * TdfNode.IndentationLevel);
            string indent2 = new string(' ', (depth + 1) * TdfNode.IndentationLevel);

            // write out the header
            writer.Write(indent);
            writer.WriteLine("[{0}]", this.Name);

            // open the body
            writer.Write(indent2);
            writer.WriteLine("{");

            // write the body
            // first, variables and their values
            foreach (var e in this.Entries)
            {
                writer.Write(indent2);
                writer.WriteLine("{0}={1};", e.Key, e.Value);
            }

            // then subkeys
            foreach (var e in this.Keys)
            {
                e.Value.WriteTdf(writer, depth + 1);
            }

            // close the body
            writer.Write(indent2);
            writer.WriteLine("}");
        }
    }
}
