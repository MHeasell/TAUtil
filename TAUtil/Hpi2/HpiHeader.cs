namespace TAUtil.Hpi2
{
    using System.IO;

    internal struct HpiHeader
    {
        /// <summary>
        /// The size of the directory in bytes, including the directory header.
        /// </summary>
        public uint DirectorySize;

        /// <summary>
        /// The decryption key.
        /// </summary>
        public uint HeaderKey;

        /// <summary>
        /// Offset to the start of the directory.
        /// </summary>
        public uint Start;

        public static void Read(BinaryReader r, out HpiHeader h)
        {
            h.DirectorySize = r.ReadUInt32();
            h.HeaderKey = r.ReadUInt32();
            h.Start = r.ReadUInt32();
        }
    }
}
