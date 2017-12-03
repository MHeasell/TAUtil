namespace TAUtil.Hpi2
{
    using System.IO;

    internal struct HpiChunk
    {
        public const uint MagicNumber = 0x48535153;

        public uint Marker;
        public byte Version;
        public byte CompressionScheme;
        public byte Encrypted;
        public uint CompressedSize;
        public uint DecompressedSize;
        public uint Checksum;

        public static void Read(BinaryReader r, out HpiChunk h)
        {
            h.Marker = r.ReadUInt32();
            h.Version = r.ReadByte();
            h.CompressionScheme = r.ReadByte();
            h.Encrypted = r.ReadByte();
            h.CompressedSize = r.ReadUInt32();
            h.DecompressedSize = r.ReadUInt32();
            h.Checksum = r.ReadUInt32();
        }
    }
}
