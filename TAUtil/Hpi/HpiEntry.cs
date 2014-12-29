namespace TAUtil.Hpi
{
    /// <summary>
    /// Data structure representing a directory entry
    /// inside a HPI archive.
    /// </summary>
    public struct HpiEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HpiEntry"/> struct.
        /// </summary>
        /// <param name="name">The name of the file or directory.</param>
        /// <param name="type">The type of the entry.</param>
        /// <param name="size">The size of the entry if it is a file.</param>
        public HpiEntry(string name, FileType type, int size)
            : this()
        {
            this.Name = name;
            this.Type = type;
            this.Size = size;
        }

        /// <summary>
        /// Indicates the type of a HPI entry.
        /// </summary>
        public enum FileType
        {
            /// <summary>
            /// The entry is a file.
            /// </summary>
            File,

            /// <summary>
            /// The entry is a directory.
            /// </summary>
            Directory
        }

        /// <summary>
        /// Gets or sets the name of the file or directory.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the HPI entry.
        /// </summary>
        public FileType Type { get; set; }

        /// <summary>
        /// Gets or sets the size of the entry if it is a file.
        /// </summary>
        public int Size { get; set; }
    }
}
