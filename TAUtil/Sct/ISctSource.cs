namespace TAUtil.Sct
{
    using System.Collections.Generic;

    using TAUtil.Tnt;

    /// <summary>
    /// <para>
    /// Adapter for the <see cref="SctWriter"/> class.
    /// This interface provides properties and methods
    /// for retrieving data in order to write it to a SCT file.
    /// </para>
    /// <para>
    /// Users should implement this interface
    /// to allow SctWriter to extract the necessary data for writing.
    /// </para>
    /// </summary>
    public interface ISctSource
    {
        /// <summary>
        /// Gets the width of the section in graphic tiles.
        /// </summary>
        int DataWidth { get; }

        /// <summary>
        /// Gets the height of the section in graphic tiles.
        /// </summary>
        int DataHeight { get; }

        /// <summary>
        /// Gets the number of tiles in the tile list.
        /// </summary>
        int TileCount { get; }

        /// <summary>
        /// Enumerates the tile list.
        /// Each tile is an array of binary data.
        /// </summary>
        /// <returns>An enumeration of the tiles.</returns>
        IEnumerable<byte[]> EnumerateTiles();

        /// <summary>
        /// Enumerates the tile data.
        /// Each cell contains an index indicating which tile in the tile list
        /// should be displayed at this location.
        /// The data is enumerated row by row, starting from the top left.
        /// The length of the data is <see cref="DataWidth"/> * <see cref="DataHeight"/>.
        /// </summary>
        /// <returns>An enumeration of the tile data.</returns>
        IEnumerable<int> EnumerateData();

        /// <summary>
        /// <para>
        /// Enumerates the attribute data.
        /// Each cell contains a <see cref="TileAttr"/> structure
        /// indicating the attributes for the cell.
        /// The attributes are enumerated row by row, starting from the top left.
        /// The length of the data is <see cref="DataWidth"/> * <see cref="DataHeight"/> * 4.
        /// (There are four attribute cells for each data cell.)
        /// </para>
        /// <para>
        /// In the case of SCT files,
        /// only the height data from each attribute is written.
        /// </para>
        /// </summary>
        /// <returns>An enumeration of the attributes.</returns>
        IEnumerable<TileAttr> EnumerateAttrs();

        /// <summary>
        /// Gets the section's minimap.
        /// This is a 128x128 indexed color image.
        /// The length of this data in bytes is 128*128.
        /// </summary>
        /// <returns>The minimap data.</returns>
        byte[] GetMinimap();
    }
}