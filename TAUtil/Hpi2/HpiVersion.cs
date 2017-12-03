namespace TAUtil.Hpi2
{
    using System.IO;

    internal struct HpiVersion
    {
        /// <summary>
        /// The magic number at the start of the HPI header ("HAPI"). 
        /// </summary>
        public const uint MagicNumber = 0x49504148;

        /// <summary>
        /// The version number indicating a standard HPI file.
        /// </summary>
        public const uint VersionNumber = 0x00010000;

        /// <summary>
        /// Set to "HAPI".
        /// </summary>
        public uint Marker;

        /// <summary>
        /// Set to "BANK" if the file is a saved game.
        /// </summary>
        public uint Version;

        public static void Read(BinaryReader r, out HpiVersion v)
        {
            v.Marker = r.ReadUInt32();
            v.Version = r.ReadUInt32();
        }
    }
}
