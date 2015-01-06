namespace TAUtil.Gaf
{
    using System;
    using System.IO;

    /// <summary>
    /// Class for reading GAF format files.
    /// </summary>
    public class GafReader : IDisposable
    {
        private readonly BinaryReader reader;

        private readonly IGafReaderAdapter adapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="GafReader"/> class.
        /// </summary>
        /// <param name="filename">The path of the GAF file to read.</param>
        /// <param name="adapter">The adapter to pass read data to.</param>
        public GafReader(string filename, IGafReaderAdapter adapter)
            : this(File.OpenRead(filename), adapter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GafReader"/> class.
        /// </summary>
        /// <param name="s">The stream to read from.</param>
        /// <param name="adapter">The adapter to pass read data to.</param>
        public GafReader(Stream s,  IGafReaderAdapter adapter)
            : this(new BinaryReader(s), adapter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GafReader"/> class.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="adapter">The adapter to pass read data to.</param>
        public GafReader(BinaryReader reader, IGafReaderAdapter adapter)
        {
            this.reader = reader;
            this.adapter = adapter;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="GafReader"/> class.
        /// </summary>
        ~GafReader()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Reads the GAF data from the input stream.
        /// </summary>
        public void Read()
        {
            // read in header
            Structures.GafHeader header = new Structures.GafHeader();
            Structures.GafHeader.Read(this.reader, ref header);

            this.adapter.BeginRead(header.Entries);

            // read in pointers to entries
            int[] pointers = new int[header.Entries];
            for (int i = 0; i < header.Entries; i++)
            {
                pointers[i] = this.reader.ReadInt32();
            }

            // read in the actual entries themselves
            for (int i = 0; i < header.Entries; i++)
            {
                this.reader.BaseStream.Seek(pointers[i], SeekOrigin.Begin);
                this.ReadGafEntry();
            }

            this.adapter.EndRead();
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
        /// Indicates whether to dispose of managed resources.
        /// This should be true when explicitly disposing
        /// and false when being disposed due to garbage collection.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.reader.Dispose();
            }
        }

        private void ReadGafEntry()
        {
            // read the entry header
            Structures.GafEntry entry = new Structures.GafEntry();
            Structures.GafEntry.Read(this.reader, ref entry);

            this.adapter.BeginEntry(entry.Name, entry.Frames);

            // read in all the frame entry pointers
            Structures.GafFrameEntry[] frameEntries = new Structures.GafFrameEntry[entry.Frames];
            for (int i = 0; i < entry.Frames; i++)
            {
                Structures.GafFrameEntry.Read(this.reader, ref frameEntries[i]);
            }

            // read in the corresponding frames
            for (int i = 0; i < entry.Frames; i++)
            {
                this.reader.BaseStream.Seek(frameEntries[i].PtrFrameTable, SeekOrigin.Begin);
                this.LoadFrame();
            }

            this.adapter.EndEntry();
        }

        private void LoadFrame()
        {
            // read in the frame data table
            Structures.GafFrameData d = new Structures.GafFrameData();
            Structures.GafFrameData.Read(this.reader, ref d);

            this.adapter.BeginFrame(d.XPos, d.YPos, d.Width, d.Height, d.TransparencyIndex, d.FramePointers);

            // read the actual frame image
            this.reader.BaseStream.Seek(d.PtrFrameData, SeekOrigin.Begin);

            if (d.FramePointers > 0)
            {
                // read in the pointers
                uint[] framePointers = new uint[d.FramePointers];
                for (int i = 0; i < d.FramePointers; i++)
                {
                    framePointers[i] = this.reader.ReadUInt32();
                }

                // read in each frame
                for (int i = 0; i < d.FramePointers; i++)
                {
                    this.reader.BaseStream.Seek(framePointers[i], SeekOrigin.Begin);
                    this.LoadFrame();
                }
            }
            else
            {
                byte[] data;
                if (d.Compressed)
                {
                    var frameReader = new CompressedFrameReader(this.reader, d.TransparencyIndex);
                    data = frameReader.ReadCompressedImage(d.Width, d.Height);
                }
                else
                {
                    data = this.ReadUncompressedImage(d.Width, d.Height);
                }

                this.adapter.SetFrameData(data);
            }

            this.adapter.EndFrame();
        }

        private byte[] ReadUncompressedImage(int width, int height)
        {
            return this.reader.ReadBytes(width * height);
        }
    }
}
