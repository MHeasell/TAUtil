using System;
using System.IO;

namespace TAUtil.Hpi2
{
    internal struct HpiDirectoryEntry
    {
        /// <summary>
        /// Pointer to a null-terminated string containing the entry name.
        /// </summary>
        public uint NameOffset;

        /// <summary>
        /// Pointer to the data for the entry.
        /// The actual data varies depending on whether this entry
        /// is a file or a directory.
        /// </summary>
        public uint DataOffset;

        /// <summary>
        /// 1 if the entry is a directory, 0 if it is a file.
        /// </summary>
        public byte IsDirectory;

        public static void Read(BinaryReader reader, out HpiDirectoryEntry entry)
        {
            entry.NameOffset = reader.ReadUInt32();
            entry.DataOffset = reader.ReadUInt32();
            entry.IsDirectory = reader.ReadByte();
        }
    }
}
