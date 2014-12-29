namespace TAUtil.Gdi.Palette
{
    using System;
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// Wrapper for reading TA palette files.
    /// The files themselves are just 256 colors
    /// packed in binary, 4 bytes for each color: r, g, b, a.
    /// </summary>
    public class BinaryPaletteReader : IDisposable
    {
        private readonly BinaryReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryPaletteReader"/> class.
        /// </summary>
        /// <param name="b">The reader to read from.</param>
        public BinaryPaletteReader(BinaryReader b)
        {
            this.reader = b;
        }

        /// <summary>
        /// Reads a color from the underlying stream.
        /// </summary>
        /// <returns>The color that was read.</returns>
        public Color ReadColor()
        {
            int r = this.reader.ReadByte();
            int g = this.reader.ReadByte();
            int b = this.reader.ReadByte();

            // alpha, ignored
            this.reader.ReadByte();

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Disposes the object.
        /// See <see cref="IDisposable.Dispose"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">
        /// Indicates whether to dispose of managed objects.
        /// This should be true if disposing explicitly
        /// or false if disposing implicitly due to garbage collection.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.reader.Dispose();
            }
        }
    }
}
