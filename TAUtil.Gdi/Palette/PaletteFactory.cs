namespace TAUtil.Gdi.Palette
{
    using System.IO;

    /// <summary>
    /// Provides methods for creating palette instances from files.
    /// </summary>
    public static class PaletteFactory
    {
        /// <summary>
        /// Palette instance containing the colors
        /// used in the Total Annihilation game palette.
        /// </summary>
        public static readonly IPalette TAPalette;

        private const int TAPaletteColorCount = 256;

        static PaletteFactory()
        {
            using (var r = new MemoryStream(TAUtil.Gdi.Properties.Resources.PALETTE))
            {
                TAPalette = PaletteFactory.FromBinaryPal(r);
            }
        }

        /// <summary>
        /// Creates an IPalette instance
        /// from the given stream.
        /// The stream data is expected to have the format
        /// of a Total Annihilation palette file (.PAL).
        /// </summary>
        /// <param name="file">The stream to read from.</param>
        /// <returns>The created palette instance.</returns>
        public static IPalette FromBinaryPal(Stream file)
        {
            var p = new ArrayPalette(TAPaletteColorCount);
            var r = new BinaryPaletteReader(new BinaryReader(file));

            for (int i = 0; i < TAPaletteColorCount; i++)
            {
                p[i] = r.ReadColor();
            }

            return p;
        }
    }
}
