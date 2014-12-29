namespace TAUtil._3do
{
    using System.IO;

    /// <summary>
    /// A vector in three dimensional space.
    /// Used by 3DO models.
    /// </summary>
    public struct Vector
    {
        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public int Y;

        /// <summary>
        /// The Z component of the vector.
        /// </summary>
        public int Z;

        internal static void Read(Stream s, ref Vector v)
        {
            Read(new BinaryReader(s), ref v);
        }

        internal static void Read(BinaryReader b, ref Vector v)
        {
            v.X = b.ReadInt32();
            v.Y = b.ReadInt32();
            v.Z = b.ReadInt32();
        }
    }
}
