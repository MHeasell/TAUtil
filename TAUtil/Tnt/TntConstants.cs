namespace TAUtil.Tnt
{
    /// <summary>
    /// Contains constants relevant to TNT format files.
    /// </summary>
    public static class TntConstants
    {
        /// <summary>
        /// The byte value in minimap data representing
        /// out-of-bounds areas.
        /// </summary>
        public const byte MinimapVoidByte = 0x64;

        /// <summary>
        /// The maximum width of a minimap in a TNT file.
        /// </summary>
        public const int MaxMinimapWidth = 252;

        /// <summary>
        /// The maximum height of a minimap in a TNT file.
        /// </summary>
        public const int MaxMinimapHeight = 252;

        /// <summary>
        /// The length of a feature name entry in a TNT file.
        /// </summary>
        public const int AnimNameLength = 128;
    }
}
