namespace TAUtil.Sct
{
    using System;
    using System.IO;

    using TAUtil.Tnt;

    /// <summary>
    /// Class for writing SCT format files to a stream.
    /// </summary>
    public class SctWriter : IDisposable
    {
        private readonly BinaryWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SctWriter"/> class.
        /// </summary>
        /// <param name="s">The stream to write to.</param>
        public SctWriter(Stream s)
            : this(new BinaryWriter(s))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SctWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        public SctWriter(BinaryWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes a SCT format file to the stream
        /// using data from the provided SCT source adapter.
        /// </summary>
        /// <param name="adapter">The object to write to the stream.</param>
        public void WriteSct(ISctSource adapter)
        {
            SctHeader h = new SctHeader();
            h.Version = 3;
            h.Width = (uint)adapter.DataWidth;
            h.Height = (uint)adapter.DataHeight;

            h.Tiles = (uint)adapter.TileCount;

            int ptrAccumulator = SctHeader.HeaderLength;
            
            h.PtrData = (uint)ptrAccumulator;
            ptrAccumulator += sizeof(ushort) * adapter.DataWidth * adapter.DataHeight;
            
            // height data always comes after tile data
            ptrAccumulator += TileAttr.SctVersion3AttrLength * adapter.DataWidth * 2 * adapter.DataHeight * 2;

            h.PtrTiles = (uint)ptrAccumulator;
            ptrAccumulator += MapConstants.TileDataLength * adapter.TileCount;

            h.PtrMiniMap = (uint)ptrAccumulator;

            h.Write(this.writer);

            this.WriteData(adapter);
            this.WriteHeights(adapter);
            this.WriteTiles(adapter);
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

        private void WriteMinimap(ISctSource adapter)
        {
            this.writer.Write(adapter.GetMinimap());
        }

        private void WriteTiles(ISctSource adapter)
        {
            foreach (var tile in adapter.EnumerateTiles())
            {
                this.writer.Write(tile);
            }
        }

        private void WriteHeights(ISctSource adapter)
        {
            foreach (TileAttr t in adapter.EnumerateAttrs())
            {
                t.WriteToSct(this.writer, 3);
            }
        }

        private void WriteData(ISctSource adapter)
        {
            foreach (int i in adapter.EnumerateData())
            {
                this.writer.Write((ushort)i);
            }
        }
    }
}
