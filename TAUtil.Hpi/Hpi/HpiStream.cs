namespace TAUtil.Hpi
{
    using System;
    using System.IO;

    /// <summary>
    /// This class represents an unmanaged memory stream.
    /// It is used when reading files from a HPI archive.
    /// </summary>
    /// <remarks>
    /// If no length is passed during construction,
    /// this stream will have unbounded length.
    /// In this case, it is possible to read past the end of the stream
    /// and into unknown memory.
    /// </remarks>
    internal unsafe sealed class HpiStream : Stream
    {
        private readonly IntPtr ptr;
        private readonly byte* hpiFileHandle;
        private readonly long length;

        public HpiStream(IntPtr ptr, long length = -1)
        {
            this.ptr = ptr;
            this.hpiFileHandle = (byte*)ptr.ToPointer();
            this.length = length;
        }

        public bool StopAtNull
        {
            get;
            set;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get
            {
                if (this.length == -1)
                {
                    throw new NotSupportedException();
                }

                return this.length;
            }
        }

        public override long Position
        {
            get;
            set;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                // If we have a length, don't overrun it.
                if (this.length != -1 && this.Position >= this.length)
                {
                    return i;
                }

                byte b = this.hpiFileHandle[this.Position];

                // If we're a text buffer, don't go past null.
                if (this.StopAtNull && b == 0)
                {
                    return i;
                }

                buffer[offset + i] = b;
                this.Position++;
            }

            return count;
        }

        public override int ReadByte()
        {
            // If we have a length, don't overrun it.
            if (this.length != -1 && this.Position >= this.length)
            {
                return -1;
            }

            byte b = this.hpiFileHandle[this.Position];

            // if we're a text buffer, don't go past null.
            if (this.StopAtNull && b == 0)
            {
                return -1;
            }

            this.Position++;
            return b;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
                case SeekOrigin.End:
                    if (this.length == -1)
                    {
                        throw new NotSupportedException("cannot seek from end of stream");
                    }

                    this.Position = this.length - offset;
                    break;
                default:
                    throw new ArgumentException("invalid seek origin");
            }

            return this.Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            NativeMethods.HPICloseFile(this.ptr);
        }
    }
}
