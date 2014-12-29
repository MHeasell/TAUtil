namespace TAUtil.Hpi
{
    using System;
    using System.IO;

    /// <summary>
    /// Class for writing HPI files.
    /// </summary>
    public sealed class HpiWriter : IDisposable
    {
        private readonly CompressionMethod compression;

        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="HpiWriter"/> class.
        /// </summary>
        /// <param name="filename">
        /// The path to write the file to.
        /// If this file exists, it will be overwritten.
        /// </param>
        /// <param name="compression">
        /// The compression method to use.
        /// </param>
        /// <param name="callback">
        /// This callback function will be called periodically
        /// during the pack process.
        /// </param>
        public HpiWriter(
            string filename,
            CompressionMethod compression = CompressionMethod.LZ77,
            HpiCallback callback = null)
        {
            this.handle = NativeMethods.HPICreate(filename, callback);
            if (this.handle == IntPtr.Zero)
            {
                throw new IOException("failed to create " + filename);
            }

            this.compression = compression;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="HpiWriter"/> class.
        /// </summary>
        ~HpiWriter()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Callback function used during HPI packing
        /// to provide progress information.
        /// </summary>
        /// <param name="filename">The name of the file currently being packed into the HPI.</param>
        /// <param name="hpiName">The name of the HPI file being written.</param>
        /// <param name="fileCount">The number of the file currently being packed.</param>
        /// <param name="fileCountTotal">The total number of files to pack.</param>
        /// <param name="fileBytes">The number of bytes of the file that have been packed.</param>
        /// <param name="fileBytesTotal">The total size of the file being packed.</param>
        /// <param name="totalBytes">The number of bytes that has been packed so far.</param>
        /// <param name="totalBytesTotal">The total number of bytes to pack.</param>
        /// <returns>
        /// A non-zero value if the pack process should be cancelled.
        /// Otherwise 0.
        /// </returns>
        public delegate int HpiCallback(
            string filename,
            string hpiName,
            int fileCount,
            int fileCountTotal,
            int fileBytes,
            int fileBytesTotal,
            int totalBytes,
            int totalBytesTotal);

        /// <summary>
        /// Enumerates the possible HPI compression methods.
        /// </summary>
        public enum CompressionMethod
        {
            /// <summary>
            /// No compression.
            /// </summary>
            None = 0,

            /// <summary>
            /// LZ77 compression.
            /// </summary>
            LZ77 = 1,

            /// <summary>
            /// ZLib compression.
            /// </summary>
            ZLib = 2
        }

        /// <summary>
        /// Creates a directory within the HPI archive.
        /// </summary>
        /// <param name="dirName">The complete path of the directory to create.</param>
        public void CreateDirectory(string dirName)
        {
            int success = NativeMethods.HPICreateDirectory(this.handle, dirName);

            if (success == 0)
            {
                throw new IOException("failed to create directory " + dirName);
            }
        }

        /// <summary>
        /// Adds a file to the HPI archive.
        /// </summary>
        /// <param name="hpiName">
        /// The complete path of the file inside the HPI archive,
        /// including the file name.
        /// </param>
        /// <param name="fileName">
        /// The path to the file on disk to add.
        /// </param>
        public void AddFile(string hpiName, string fileName)
        {
            int success = NativeMethods.HPIAddFile(this.handle, hpiName, fileName);

            if (success == 0)
            {
                throw new IOException("failed to add file " + fileName);
            }
        }

        /// <summary>
        /// Closes the HPI file handle.
        /// This is the same as calling <see cref="Dispose()"/>.
        /// </summary>
        public void Close()
        {
            this.Dispose();
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

        private void Dispose(bool disposing)
        {
            if (this.handle != IntPtr.Zero)
            {
                int success = NativeMethods.HPIPackArchive(this.handle, this.compression);
                this.handle = IntPtr.Zero;
                if (success == 0)
                {
                    throw new IOException("failed to pack archive");
                }
            }
        }
    }
}
