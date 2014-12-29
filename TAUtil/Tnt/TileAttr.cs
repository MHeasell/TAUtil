namespace TAUtil.Tnt
{
    using System.IO;

    /// <summary>
    /// Data structure holding attribute information
    /// for a single tile attribute cell.
    /// </summary>
    public struct TileAttr
    {
        /// <summary>
        /// The feature value representing no feature.
        /// </summary>
        public const ushort FeatureNone = 0xFFFF;

        /// <summary>
        /// The feature value representing a void cell.
        /// </summary>
        public const ushort FeatureVoid = 0xFFFC;

        /// <summary>
        /// Feature value present in some maps.
        /// Its purpose is unknown.
        /// </summary>
        public const ushort FeatureUnknown = 0xFFFE;

        /// <summary>
        /// The length of an attribute structure in bytes in a TNT file.
        /// </summary>
        public const int AttrLength = 4;

        /// <summary>
        /// The length of an attribute structure in bytes in a version 2 SCT file.
        /// </summary>
        public const int SctVersion2AttrLength = 8;

        /// <summary>
        /// The length of an attribute structure in bytes in a version 3 SCT file.
        /// </summary>
        public const int SctVersion3AttrLength = 4;

        /// <summary>
        /// The height at this point.
        /// </summary>
        public byte Height;

        /// <summary>
        /// <para>
        /// Offset in the feature array
        /// of the feature located in this square.
        /// </para>
        /// <para>
        /// 0xFFFF if none,
        /// 0xFFFC (-4) if void.
        /// I've also seen 0xFFFE (-2) on some of the early Cavedog maps
        /// such as Lava Run and AC02,
        /// but have no idea what it means.
        /// Please contact me if you have any information.
        /// </para>
        /// </summary>
        public ushort Feature;

        /// <summary>
        /// No known purpose
        /// (my personal guess is that it is padding to get to 4 bytes).
        /// </summary>
        public byte Pad1;

        internal static TileAttr Read(BinaryReader b)
        {
            TileAttr attr;
            attr.Height = b.ReadByte();
            attr.Feature = b.ReadUInt16();
            attr.Pad1 = b.ReadByte();
            return attr;
        }

        internal static TileAttr ReadFromSct(Stream file, int version)
        {
            return ReadFromSct(new BinaryReader(file), version);
        }

        /// <summary>Reads a tile attribute from a SCT format source.</summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="version">The SCT format version. Valid versions are 2 and 3.</param>
        /// <returns>The attribute data that was read.</returns>
        internal static TileAttr ReadFromSct(BinaryReader reader, int version)
        {
            TileAttr a;
            a.Height = reader.ReadByte();
            reader.ReadInt16(); // padding
            reader.ReadByte(); // more padding

            if (version == 2)
            {
                // skip some more padding present in V2
                reader.ReadInt32();
            }

            a.Feature = 0;
            a.Pad1 = 0;
            return a;
        }

        internal void Write(BinaryWriter b)
        {
            b.Write(this.Height);
            b.Write(this.Feature);
            b.Write(this.Pad1);
        }

        internal void WriteToSct(BinaryWriter b, int version)
        {
            b.Write(this.Height);
            b.Write((short)0); // padding
            b.Write((byte)0); // more padding

            if (version == 2)
            {
                // write some more padding present in V2
                b.Write(0);
            }
        }
    }
}
