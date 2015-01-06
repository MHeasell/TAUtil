namespace TAUtil.Gaf.Structures
{
    using System.IO;

    internal struct GafFrameData
    {
        public ushort Width;
        public ushort Height;
        public short XPos;
        public short YPos;
        public byte TransparencyIndex;
        public bool Compressed;
        public ushort FramePointers;
        public uint Unknown2;
        public uint PtrFrameData;
        public uint Unknown3;

        public static void Read(BinaryReader b, ref GafFrameData e)
        {
            e.Width = b.ReadUInt16();
            e.Height = b.ReadUInt16();
            e.XPos = b.ReadInt16();
            e.YPos = b.ReadInt16();
            e.TransparencyIndex = b.ReadByte();
            e.Compressed = b.ReadBoolean();
            e.FramePointers = b.ReadUInt16();
            e.Unknown2 = b.ReadUInt32();
            e.PtrFrameData = b.ReadUInt32();
            e.Unknown3 = b.ReadUInt32();
        }
    }
}
