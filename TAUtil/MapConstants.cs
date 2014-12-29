namespace TAUtil
{
    /// <summary>
    /// Constants relevant to SCT and TNT map files.
    /// </summary>
    public static class MapConstants
    {
        /// <summary>
        /// The width of a single tile in pixels (and bytes).
        /// </summary>
        public const int TileWidth = 32;

        /// <summary>
        /// The height of a single tile in pixels (and bytes).
        /// </summary>
        public const int TileHeight = 32;

        /// <summary>
        /// The length of the data for one single tile in bytes.
        /// </summary>
        public const int TileDataLength = TileWidth * TileHeight;
    }
}
