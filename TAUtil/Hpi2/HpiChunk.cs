namespace TAUtil.Hpi2
{
    internal struct HpiChunk
    {
        public uint Marker;
        public byte Version;
        public byte CompressionScheme;
        public byte Encrypted;
        public uint compressedSize;
        public uint decompressedSize;
        public uint checksum;
    }
}
