namespace TAUtil.Tnt
{
    using System;
    using System.IO;

    /// <summary>
    /// Class for writing TNT format files to a stream.
    /// </summary>
    public class TntWriter : IDisposable
    {
        private readonly BinaryWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TntWriter"/> class.
        /// </summary>
        /// <param name="s">The stream to write to.</param>
        public TntWriter(Stream s)
            : this(new BinaryWriter(s))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TntWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        public TntWriter(BinaryWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes a TNT format file to the stream
        /// using data from the provided TNT source adapter.
        /// </summary>
        /// <param name="adapter">The object to write to the stream.</param>
        public void WriteTnt(ITntSource adapter)
        {
            TntHeader h = new TntHeader();
            h.IdVersion = TntHeader.TntMagicNumber;

            h.Width = (uint)(adapter.DataWidth * 2);
            h.Height = (uint)(adapter.DataHeight * 2);
            h.SeaLevel = (uint)adapter.SeaLevel;

            h.TileAnims = (uint)adapter.AnimCount;
            h.Tiles = (uint)adapter.TileCount;

            int ptrAccumulator = TntHeader.HeaderLength;
            h.PtrMapData = (uint)ptrAccumulator;
            ptrAccumulator += sizeof(ushort) * adapter.DataWidth * adapter.DataHeight;
            h.PtrMapAttr = (uint)ptrAccumulator;
            ptrAccumulator += TileAttr.AttrLength * adapter.DataWidth * 2 * adapter.DataHeight * 2;
            h.PtrTileGfx = (uint)ptrAccumulator;
            ptrAccumulator += MapConstants.TileDataLength * adapter.TileCount;
            h.PtrTileAnims = (uint)ptrAccumulator;
            ptrAccumulator += (sizeof(uint) + TntConstants.AnimNameLength) * adapter.AnimCount;
            h.PtrMiniMap = (uint)ptrAccumulator;

            h.Unknown1 = 1; // if this is set to 0, the minimap doesn't show

            h.Write(this.writer);

            this.WriteData(adapter);
            this.WriteAttrs(adapter);
            this.WriteTiles(adapter);
            this.WriteAnims(adapter);
            this.WriteMinimap(adapter);
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
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">
        /// Indicates whether to dispose of managed resources.
        /// This should be true when explicitly disposing
        /// and false when being disposed due to garbage collection.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.writer.Dispose();
            }
        }

        private void WriteMinimap(ITntSource source)
        {
            MinimapInfo info = source.GetMinimap();

            this.writer.Write(TntConstants.MaxMinimapWidth);
            this.writer.Write(TntConstants.MaxMinimapHeight);

            for (int y = 0; y < TntConstants.MaxMinimapHeight; y++)
            {
                for (int x = 0; x < TntConstants.MaxMinimapWidth; x++)
                {
                    if (y >= info.Height || x >= info.Width)
                    {
                        this.writer.Write(TntConstants.MinimapVoidByte);
                        continue;
                    }

                    this.writer.Write(info.Data[(y * info.Width) + x]);
                }
            }
        }

        private void WriteAnims(ITntSource source)
        {
            int count = 0;
            foreach (string anim in source.EnumerateAnims())
            {
                this.WriteAnim(anim, count++);
            }
        }

        private void WriteTiles(ITntSource source)
        {
            foreach (byte[] tile in source.EnumerateTiles())
            {
                this.writer.Write(tile);
            }
        }

        private void WriteAttrs(ITntSource source)
        {
            foreach (TileAttr t in source.EnumerateAttrs())
            {
                t.Write(this.writer);
            }
        }

        private void WriteData(ITntSource source)
        {
            foreach (int i in source.EnumerateData())
            {
                this.writer.Write((ushort)i);
            }
        }

        private void WriteAnim(string name, int index)
        {
            byte[] c = new byte[TntConstants.AnimNameLength];
            System.Text.Encoding.ASCII.GetBytes(name, 0, name.Length, c, 0);
            this.writer.Write(index);
            this.writer.Write(c);
        }
    }
}
