namespace TAUtil.Tnt
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Class for reading from TNT format files.
    /// </summary>
    public class TntReader : IDisposable, ITntSource
    {
        private readonly BinaryReader reader;

        private TntHeader header;

        /// <summary>
        /// Initializes a new instance of the <see cref="TntReader"/> class.
        /// </summary>
        /// <param name="filename">The path of the file to read from.</param>
        public TntReader(string filename)
            : this(File.OpenRead(filename))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TntReader"/> class.
        /// </summary>
        /// <param name="s">The stream to read from.</param>
        public TntReader(Stream s)
            : this(new BinaryReader(s))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TntReader"/> class.
        /// </summary>
        /// <param name="r">The reader to read from.</param>
        public TntReader(BinaryReader r)
        {
            this.reader = r;
            TntHeader.Read(this.reader, ref this.header);
        }

        /// <summary>
        /// Gets the width of the map in attribute cells.
        /// </summary>
        public int Width
        {
            get
            {
                return (int)this.header.Width;
            }
        }

        /// <summary>
        /// Gets the height of the map in attribute cells.
        /// </summary>
        public int Height
        {
            get
            {
                return (int)this.header.Height;
            }
        }

        /// <summary>
        /// Gets the data width. See <see cref="ITntSource.DataWidth"/>.
        /// </summary>
        public int DataWidth
        {
            get
            {
                return this.Width / 2;
            }
        }

        /// <summary>
        /// Gets the data height. See <see cref="ITntSource.DataHeight"/>.
        /// </summary>
        public int DataHeight
        {
            get
            {
                return this.Height / 2;
            }
        }

        /// <summary>
        /// Gets the sealevel. See <see cref="ITntSource.SeaLevel"/>.
        /// </summary>
        public int SeaLevel
        {
            get
            {
                // FIXME: unsafe cast
                return (int)this.header.SeaLevel;
            }
        }

        /// <summary>
        /// Gets the tile count. See <see cref="ITntSource.TileCount"/>.
        /// </summary>
        public int TileCount
        {
            get
            {
                return (int)this.header.Tiles;
            }
        }

        /// <summary>
        /// Gets the animation count. See <see cref="ITntSource.AnimCount"/>.
        /// </summary>
        public int AnimCount
        {
            get
            {
                return (int)this.header.TileAnims;
            }
        }

        /// <summary>
        /// See <see cref="ITntSource.EnumerateAttrs"/>.
        /// </summary>
        /// <returns>An enumeration of the attributes.</returns>
        public IEnumerable<TileAttr> EnumerateAttrs()
        {
            this.reader.BaseStream.Seek(this.header.PtrMapAttr, SeekOrigin.Begin);
            for (int y = 0; y < this.header.Height; y++)
            {
                for (int x = 0; x < this.header.Width; x++)
                {
                    yield return TileAttr.Read(this.reader);
                }
            }
        }

        /// <summary>
        /// See <see cref="ITntSource.EnumerateData"/>.
        /// </summary>
        /// <returns>An enumeration of the data.</returns>
        public IEnumerable<int> EnumerateData()
        {
            this.reader.BaseStream.Seek(this.header.PtrMapData, SeekOrigin.Begin);
            int length = this.DataWidth * this.DataHeight;
            for (int i = 0; i < length; i++)
            {
                yield return this.reader.ReadUInt16();
            }
        }

        /// <summary>
        /// See <see cref="ITntSource.EnumerateTiles"/>.
        /// </summary>
        /// <returns>An enumeration of the tiles.</returns>
        public IEnumerable<byte[]> EnumerateTiles()
        {
            this.reader.BaseStream.Seek(this.header.PtrTileGfx, SeekOrigin.Begin);
            for (int i = 0; i < this.TileCount; i++)
            {
                yield return this.reader.ReadBytes(MapConstants.TileDataLength);
            }
        }

        /// <summary>
        /// See <see cref="ITntSource.EnumerateAnims"/>.
        /// </summary>
        /// <returns>An enumeration of the animations.</returns>
        public IEnumerable<string> EnumerateAnims()
        {
            this.reader.BaseStream.Seek(this.header.PtrTileAnims, SeekOrigin.Begin);
            for (int i = 0; i < this.AnimCount; i++)
            {
                this.reader.ReadUInt32(); // skip feature index
                byte[] chars = this.reader.ReadBytes(TntConstants.AnimNameLength);
                string s = TAUtil.Util.ConvertChars(chars);
                yield return s;
            }
        }

        /// <summary>
        /// See <see cref="ITntSource.GetMinimap"/>.
        /// </summary>
        /// <returns>The minimap info.</returns>
        public MinimapInfo GetMinimap()
        {
            this.reader.BaseStream.Seek(this.header.PtrMiniMap, SeekOrigin.Begin);
            int width = this.reader.ReadInt32();
            int height = this.reader.ReadInt32();
            byte[] data = this.reader.ReadBytes(width * height);
            Util.Size trimmedSize = Util.GetMinimapActualSize(data, width, height);
            data = Util.TrimMinimapBytes(
                data,
                width,
                height,
                trimmedSize.Width,
                trimmedSize.Height);
            MinimapInfo minimap = new MinimapInfo(trimmedSize.Width, trimmedSize.Height, data);

            return minimap;
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
