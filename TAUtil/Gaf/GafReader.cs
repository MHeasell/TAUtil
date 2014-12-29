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

        /// <summary>
        /// Initializes a new instance of the <see cref="GafReader"/> class.
        /// </summary>
        /// <param name="filename">The path of the GAF file to read.</param>
        public GafReader(string filename)
            : this(File.OpenRead(filename))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GafReader"/> class.
        /// </summary>
        /// <param name="s">The stream to read from.</param>
        public GafReader(Stream s)
            : this(new BinaryReader(s))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GafReader"/> class.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        public GafReader(BinaryReader reader)
        {
            this.reader = reader;
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
        /// <returns>The read GAF data.</returns>
        public GafEntry[] Read()
        {
            // read in header
            Structures.GafHeader header = new Structures.GafHeader();
            Structures.GafHeader.Read(this.reader, ref header);

            // read in pointers to entries
            int[] pointers = new int[header.Entries];
            for (int i = 0; i < header.Entries; i++)
            {
                pointers[i] = this.reader.ReadInt32();
            }

            // read in the actual entries themselves
            GafEntry[] entries = new GafEntry[header.Entries];
            for (int i = 0; i < header.Entries; i++)
            {
                this.reader.BaseStream.Seek(pointers[i], SeekOrigin.Begin);
                entries[i] = this.ReadGafEntry();
            }

            return entries;
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

        private GafEntry ReadGafEntry()
        {
            // read the entry header
            Structures.GafEntry entry = new Structures.GafEntry();
            Structures.GafEntry.Read(this.reader, ref entry);

            // read in all the frame entry pointers
            Structures.GafFrameEntry[] frameEntries = new Structures.GafFrameEntry[entry.Frames];
            for (int i = 0; i < entry.Frames; i++)
            {
                Structures.GafFrameEntry.Read(this.reader, ref frameEntries[i]);
            }

            // read in the corresponding frames
            GafFrame[] frames = new GafFrame[entry.Frames];

            for (int i = 0; i < entry.Frames; i++)
            {
                this.reader.BaseStream.Seek(frameEntries[i].PtrFrameTable, SeekOrigin.Begin);
                frames[i] = this.LoadFrame();
            }

            // fill in and return our output structure
            GafEntry outEntry = new GafEntry();
            outEntry.Name = entry.Name;
            outEntry.Frames = frames;
            return outEntry;
        }

        private GafFrame LoadFrame()
        {
            // read in the frame data table
            Structures.GafFrameData d = new Structures.GafFrameData();
            Structures.GafFrameData.Read(this.reader, ref d);

            GafFrame frame = new GafFrame();
            frame.OffsetX = d.XPos;
            frame.OffsetY = d.YPos;

            frame.Width = d.Width;
            frame.Height = d.Height;

            frame.TransparencyIndex = d.TransparencyIndex;

            // read the actual frame image
            this.reader.BaseStream.Seek(d.PtrFrameData, SeekOrigin.Begin);

            if (d.FramePointers > 0)
            {
                // TODO: implement support for subframes
                frame.Width = 0;
                frame.Height = 0;
                frame.Data = new byte[0];
            }
            else
            {
                if (d.Compressed)
                {
                    var frameReader = new CompressedFrameReader(this.reader, d.TransparencyIndex);
                    frame.Data = frameReader.ReadCompressedImage(d.Width, d.Height);
                }
                else
                {
                    frame.Data = this.ReadUncompressedImage(d.Width, d.Height);
                }
            }

            return frame;
        }

        private byte[] ReadUncompressedImage(int width, int height)
        {
            return this.reader.ReadBytes(width * height);
        }
    }
}
