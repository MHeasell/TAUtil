namespace TAUtil.HpiUtil
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a file a directory entry
    /// inside a HPI archive.
    /// </summary>
    public class HpiEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HpiEntry"/> class.
        /// </summary>
        /// <param name="reader">The HPI archive the entry belongs to.</param>
        /// <param name="name">The name of the file or directory.</param>
        /// <param name="type">The type of the entry.</param>
        /// <param name="size">The size of the entry if it is a file.</param>
        internal HpiEntry(HpiReader reader, string name, FileType type, int size)
        {
            this.Reader = reader;
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
        /// Gets the HPI archive the entry is in.
        /// </summary>
        public HpiReader Reader { get; private set; }

        /// <summary>
        /// Gets the name of the file or directory.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the HPI entry.
        /// </summary>
        public FileType Type { get; private set; }

        /// <summary>
        /// Gets the size of the entry if it is a file.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Opens the entry for reading.
        /// </summary>
        /// <returns>A stream of the file data.</returns>
        public Stream Open()
        {
            if (this.Type != FileType.File)
            {
                throw new InvalidOperationException("This entry is not a file.");
            }

            return this.Reader.ReadFile(this.Name, this.Size);
        }
    }
}
