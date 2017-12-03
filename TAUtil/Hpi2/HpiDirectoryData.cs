namespace TAUtil.Hpi2
{
    using System.IO;

    internal struct HpiDirectoryData
    {
        public uint NumberOfEntries;
        public uint EntryListOffset;

        public static void Read(BinaryReader r, out HpiDirectoryData d)
        {
            d.NumberOfEntries = r.ReadUInt32();
            d.EntryListOffset = r.ReadUInt32();
        }
    }
}
