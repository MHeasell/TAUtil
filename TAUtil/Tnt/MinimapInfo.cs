namespace TAUtil.Tnt
{
    /// <summary>
    /// Data structure containing information about the minimap
    /// for a map or section.
    /// </summary>
    public class MinimapInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinimapInfo"/> class.
        /// </summary>
        /// <param name="width">The width of the minimap.</param>
        /// <param name="height">The height of the minimap.</param>
        /// <param name="data">
        /// An array containing the minimap data.
        /// The length of the data should be as long as
        /// the width multiplied by the height.
        /// </param>
        public MinimapInfo(int width, int height, byte[] data)
        {
            this.Width = width;
            this.Height = height;
            this.Data = data;
        }

        /// <summary>
        /// Gets the width of the minimap.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the minimap.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the minimap data.
        /// </summary>
        public byte[] Data { get; private set; }
    }
}