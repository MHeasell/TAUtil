namespace TAUtil.Sct
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using TAUtil.Tnt;

    /// <summary>
    /// Class for reading SCT format files.
    /// </summary>
    public class SctReader : IDisposable, ISctSource
    {
        /// <summary>
        /// The width of a SCT minimap.
        /// </summary>
        public const int MinimapWidth = 128;

        /// <summary>
        /// The height of a SCT minimap.
        /// </summary>
        public const int MinimapHeight = 128;

        private readonly BinaryReader reader;

        private SctHeader header;

        /// <summary>
        /// Initializes a new instance of the <see cref="SctReader"/> class.
        /// </summary>
        /// <param name="filename">The path of the file to read from.</param>
        public SctReader(string filename)
            : this(File.OpenRead(filename))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SctReader"/> class.
        /// </summary>
        /// <param name="s">The stream to read from.</param>
        public SctReader(Stream s)
            : this(new BinaryReader(s))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SctReader"/> class.
        /// </summary>
        /// <param name="r">The reader to read from.</param>
        public SctReader(BinaryReader r)
        {
            this.reader = r;
            SctHeader.Read(this.reader, ref this.header);
        }

        /// <summary>
        /// Gets the data width. See <see cref="ISctSource.DataWidth"/>.
        /// </summary>
        public int DataWidth
        {
            get { return (int)this.header.Width; }
        }

        /// <summary>
        /// Gets the data height. See <see cref="ISctSource.DataHeight"/>.
        /// </summary>
        public int DataHeight
        {
            get { return (int)this.header.Height; }
        }

        /// <summary>
        /// Gets the width of the section in attribute cells.
        /// </summary>
        public int WidthInAttrs
        {
            get
            {
                return (int)this.header.Width * 2;
            }
        }

        /// <summary>
        /// Gets the height of the section in attribute cells.
        /// </summary>
        public int HeightInAttrs
        {
            get
            {
                return (int)this.header.Height * 2;
            }
        }

        /// <summary>
        /// Gets the tile count. See <see cref="ISctSource.TileCount"/>.
        /// </summary>
        public int TileCount
        {
            get
            {
                return (int)this.header.Tiles;
            }
        }

        /// <summary>
        /// See <see cref="ISctSource.GetMinimap"/>.
        /// </summary>
        /// <returns>The minimap data.</returns>
        public byte[] GetMinimap()
        {
            this.reader.BaseStream.Seek(this.header.PtrMiniMap, SeekOrigin.Begin);
            return this.reader.ReadBytes(MinimapWidth * MinimapHeight);
        }

        /// <summary>
        /// See <see cref="ISctSource.EnumerateAttrs"/>.
        /// </summary>
        /// <returns>An enumeration of the attributes.</returns>
        public IEnumerable<TileAttr> EnumerateAttrs()
        {
            this.reader.BaseStream.Seek(this.header.PtrHeightData, SeekOrigin.Begin);
            for (int y = 0; y < this.HeightInAttrs; y++)
            {
                for (int x = 0; x < this.WidthInAttrs; x++)
                {
                    yield return TileAttr.ReadFromSct(this.reader, (int)this.header.Version);
                }
            }
        }

        /// <summary>
        /// See <see cref="ISctSource.EnumerateData"/>.
        /// </summary>
        /// <returns>An enumeration of the data.</returns>
        public IEnumerable<int> EnumerateData() 
        {
            this.reader.BaseStream.Seek(this.header.PtrData, SeekOrigin.Begin);
            for (int y = 0; y < this.DataHeight; y++)
            {
                for (int x = 0; x < this.DataWidth; x++)
                {
                    yield return this.reader.ReadInt16();
                }
            }
        }

        /// <summary>
        /// See <see cref="ISctSource.EnumerateTiles"/>.
        /// </summary>
        /// <returns>An enumeration of the tile data.</returns>
        public IEnumerable<byte[]> EnumerateTiles()
        {
            this.reader.BaseStream.Seek(this.header.PtrTiles, SeekOrigin.Begin);
            for (int i = 0; i < this.TileCount; i++)
            {
                yield return this.reader.ReadBytes(MapConstants.TileDataLength);
            }
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
        /// or false if disposing implicitly due to garbage collection.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.reader.Dispose();
            }
        }
    }
}
