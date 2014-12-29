namespace TAUtil.Hpi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class for reading HPI files.
    /// </summary>
    public sealed class HpiReader : IDisposable
    {
        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="HpiReader"/> class.
        /// </summary>
        /// <param name="filename">The path of the file to read from.</param>
        public HpiReader(string filename)
        {
            this.FileName = filename;
            this.handle = NativeMethods.HPIOpen(filename);
            if (this.handle == IntPtr.Zero)
            {
                throw new IOException("failed to read " + filename);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="HpiReader"/> class.
        /// </summary>
        ~HpiReader()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the path of the file this reader is reading.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Closes the file handle.
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

        /// <summary>
        /// Enumerates all the files in the HPI archive,
        /// starting from the root directory.
        /// See <seealso cref="GetFilesRecursive(string)"/> for more information.
        /// </summary>
        /// <returns>An enumeration of the files inside the HPI archive.</returns>
        public IEnumerable<HpiEntry> GetFilesRecursive()
        {
            return this.GetFilesRecursive(string.Empty);
        }

        /// <summary>
        /// <para>
        /// Enumerates all the files in the HPI file under the given directory.
        /// The scan is recursive,
        /// so files in subdirectories will be included in the enumeration.
        /// </para>
        /// <para>
        /// Beware that paths inside HPIs can contain special characters such as ">",
        /// which will cause methods in <see cref="System.IO.Path"/> to throw an exception.
        /// To manipulate paths originating from HPI archives,
        /// use <see cref="HpiPath"/>.
        /// </para>
        /// </summary>
        /// <param name="directory">The directory to enumerate.</param>
        /// <returns>
        /// An enumeration of all files inside the given directory.
        /// The full path of each file is given relative to HPI root.
        /// </returns>
        public IEnumerable<HpiEntry> GetFilesRecursive(string directory)
        {
            IEnumerable<HpiEntry> en = this.GetFilesAndDirectories(directory);

            foreach (HpiEntry e in en)
            {
                if (e.Type == HpiEntry.FileType.File)
                {
                    yield return new HpiEntry(HpiPath.Combine(directory, e.Name), e.Type, e.Size);
                }
                else
                {
                    var recEn = this.GetFilesRecursive(HpiPath.Combine(directory, e.Name));
                    foreach (HpiEntry f in recEn)
                    {
                        yield return f;
                    }
                }
            }
        }

        /// <summary>
        /// <para>
        /// Enumerates the files at the root of the HPI archive.
        /// </para>
        /// <para>
        /// This enumeration is not recursive,
        /// so files inside directories will not be included.
        /// </para>
        /// <para>
        /// Beware that paths inside HPIs can contain special characters such as ">",
        /// which will cause methods in <see cref="System.IO.Path"/> to throw an exception.
        /// To manipulate paths originating from HPI archives,
        /// use <see cref="HpiPath"/>.
        /// </para>
        /// </summary>
        /// <returns>
        /// An enumeration of the files at the root of the HPI archive.
        /// The path to each file is relative to the HPI root.
        /// </returns>
        public IEnumerable<HpiEntry> GetFiles()
        {
            return this.GetFiles(string.Empty);
        }

        /// <summary>
        /// <para>
        /// Enumerates the files in the given directory
        /// of the HPI archive.
        /// </para>
        /// <para>
        /// This enumeration is not recursive,
        /// so files in subdirectories will not be included.
        /// </para>
        /// <para>
        /// Beware that paths inside HPIs can contain special characters such as ">",
        /// which will cause methods in <see cref="System.IO.Path"/> to throw an exception.
        /// To manipulate paths originating from HPI archives,
        /// use <see cref="HpiPath"/>.
        /// </para>
        /// </summary>
        /// <param name="directory">
        /// The directory to enumerate, relative to the HPI root.
        /// </param>
        /// <returns>
        /// An enumeration of all the files in the given directory inside the HPI file,
        /// relative to that directory.
        /// </returns>
        public IEnumerable<HpiEntry> GetFiles(string directory)
        {
            return this.GetFilesAndDirectories(directory).Where(x => x.Type == HpiEntry.FileType.File);
        }

        /// <summary>
        /// <para>
        /// Enumerates the directories within the given directory
        /// of the HPI archive.
        /// </para>
        /// <para>
        /// Beware that paths inside HPIs can contain special characters such as ">",
        /// which will cause methods in <see cref="System.IO.Path"/> to throw an exception.
        /// To manipulate paths originating from HPI archives,
        /// use <see cref="HpiPath"/>.
        /// </para>
        /// </summary>
        /// <param name="directory">
        /// The directory to enumerate, relative to the HPI root.
        /// </param>
        /// <returns>
        /// An enumeration of all the directories inside the given directory of of the HPI archive.
        /// The path of each directory is given relative to to the given directory.
        /// </returns>
        public IEnumerable<HpiEntry> GetDirectories(string directory)
        {
            return this.GetFilesAndDirectories(directory).Where(x => x.Type == HpiEntry.FileType.Directory);
        }

        /// <summary>
        /// <para>
        /// Enumerates the files and directories within the given directory
        /// of the HPI archive.
        /// </para>
        /// <para>
        /// This enumeration is not recursive,
        /// so only files and directories immediately within the given directory
        /// will be enumerated.
        /// </para>
        /// <para>
        /// Beware that paths inside HPIs can contain special characters such as ">",
        /// which will cause methods in <see cref="System.IO.Path"/> to throw an exception.
        /// To manipulate paths originating from HPI archives,
        /// use <see cref="HpiPath"/>.
        /// </para>
        /// </summary>
        /// <param name="directory">
        /// The directory to enumerate, relative to the HPI root.
        /// </param>
        /// <returns>
        /// An enumeration of the files and directories
        /// inside the given directory of of the HPI archive.
        /// The path of each directory is given relative to to the given directory.
        /// </returns>
        public IEnumerable<HpiEntry> GetFilesAndDirectories(string directory)
        {
            int next = 0;
            for (;;)
            {
                StringBuilder s = new StringBuilder();
                int type;
                int size;
                next = NativeMethods.HPIDir(this.handle, next, directory, s, out type, out size);

                if (next == 0)
                {
                    break;
                }

                yield return new HpiEntry(
                    s.ToString(),
                    type == 0 ? HpiEntry.FileType.File : HpiEntry.FileType.Directory,
                    size);
            }
        }

        /// <summary>
        /// <para>
        /// Reads the given file inside the HPI archive as a stream.
        /// </para>
        /// <para>
        /// Beware that the length of the stream is unbounded.
        /// It is up to the reader to ensure that they do not read
        /// past the end of the file and into unknown memory.
        /// If you are reading a text file, use <see cref="ReadTextFile"/> instead.
        /// </para>
        /// </summary>
        /// <param name="filename">The path of the file to read.</param>
        /// <returns>A stream containing the contents of the file to be read.</returns>
        public Stream ReadFile(string filename)
        {
            IntPtr ptr = NativeMethods.HPIOpenFile(this.handle, filename);

            if (ptr == IntPtr.Zero)
            {
                throw new IOException("failed to read " + filename);
            }

            return new HpiStream(ptr);
        }

        /// <summary>
        /// <para>
        /// Reads the given file inside the HPI archive as a stream.
        /// </para>
        /// <para>
        /// The stream will terminate upon encountering a null byte.
        /// Therefore, this method is only suitable for reading text files
        /// where the null byte will not appear within the file.
        /// For reading binary files, use <see cref="ReadFile"/>.
        /// </para>
        /// </summary>
        /// <param name="filename">The path of the file to read.</param>
        /// <returns>A stream containing the contents of the file to be read.</returns>
        public Stream ReadTextFile(string filename)
        {
            IntPtr ptr = NativeMethods.HPIOpenFile(this.handle, filename);

            if (ptr == IntPtr.Zero)
            {
                throw new IOException("failed to read " + filename);
            }

            HpiStream s = new HpiStream(ptr);
            s.StopAtNull = true;
            return s;
        }

        /// <summary>
        /// Extracts the specified file from the HPI archive
        /// to the given location on disk.
        /// </summary>
        /// <param name="filename">The path of the file to extract.</param>
        /// <param name="destname">The path to save the extracted file to.</param>
        public void ExtractFile(string filename, string destname)
        {
            if (NativeMethods.HPIExtractFile(this.handle, filename, destname) == 0)
            {
                throw new IOException(string.Format("failed to extract {0} to {1}", filename, destname));
            }
        }

        private void Dispose(bool disposing)
        {
            if (this.handle != IntPtr.Zero)
            {
                NativeMethods.HPIClose(this.handle);
                this.handle = IntPtr.Zero;
            }
        }
    }
}
