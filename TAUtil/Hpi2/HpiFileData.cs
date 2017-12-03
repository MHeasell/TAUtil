using System;
using System.IO;

namespace TAUtil.Hpi2
{
    internal struct HpiFileData
    {
        /// <summary>
        /// Pointer to the start of the file data.
        /// </summary>
        public uint DataOffset;

        /// <summary>
        /// Size of the compressed file in bytes.
        /// </summary>
        public uint FileSize;

        /// <summary>
        /// 0 for no compression, 1 for LZ77 compression, 2 for ZLib compression.
        /// </summary>
        public uint CompressionScheme;

        public static void Read(BinaryReader reader, out HpiFileData f)
        {
            f.DataOffset = reader.ReadUInt32();
            f.FileSize = reader.ReadUInt32();
            f.CompressionScheme = reader.ReadUInt32();
        }
    }
}
