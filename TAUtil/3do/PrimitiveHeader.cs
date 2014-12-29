namespace TAUtil._3do
{
    using System.IO;

    internal struct PrimitiveHeader
    {
        public const int Length = 32;

        /// <summary>
        /// The color of the primitive.
        /// The index of a color in the TA color palette.
        /// </summary>
        public int ColorIndex;

        /// <summary>
        /// The number of vertices used by the primitive.
        /// A primitive with one vertex is a point,
        /// two a line, three a triangle, four a quad, and so on.
        /// </summary>
        public int VertexCount;

        /// <summary>
        /// Always set to 0. Purpose unknown.
        /// </summary>
        public int AlwaysZero;

        /// <summary>
        /// The offset to a an array of shorts
        /// which are indices into the object's vertex array.
        /// This allows multiple primitives to share the same vertices.
        /// </summary>
        public int PtrVertexIndexArray;

        /// <summary>
        /// The offset to a null terminated string
        /// which indicates which texture to use for this primitive.
        /// A value of 0 probably means no texture.
        /// </summary>
        public int PtrTextureName;

        /// <summary>
        /// Used by Cavedog for their editor, not needed.
        /// Always set to 0 or ignore.
        /// </summary>
        public int Unknown1;

        /// <summary>
        /// Used by Cavedog for their editor, not needed.
        /// Always set to 0 or ignore.
        /// </summary>
        public int Unknown2;

        /// <summary>
        /// Used by Cavedog for their editor, not needed.
        /// Always set to 0 or ignore.
        /// </summary>
        public int Unknown3;

        public static void Read(Stream s, ref PrimitiveHeader header)
        {
            Read(new BinaryReader(s), ref header);
        }

        public static void Read(BinaryReader b, ref PrimitiveHeader header)
        {
            header.ColorIndex = b.ReadInt32();
            header.VertexCount = b.ReadInt32();

            header.AlwaysZero = b.ReadInt32();

            header.PtrVertexIndexArray = b.ReadInt32();
            header.PtrTextureName = b.ReadInt32();

            header.Unknown1 = b.ReadInt32();
            header.Unknown2 = b.ReadInt32();
            header.Unknown3 = b.ReadInt32();
        }
    }
}
