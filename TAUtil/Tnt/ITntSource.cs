namespace TAUtil.Tnt
{
    using System.Collections.Generic;

    /// <summary>
    /// <para>
    /// Adapter for the <see cref="TntWriter"/> class.
    /// This interface provides properties and methods
    /// for retrieving data in order to write it to a TNT file.
    /// </para>
    /// <para>
    /// Users should implement this interface
    /// to allow TntWriter to extract the necessary data for writing.
    /// </para>
    /// </summary>
    public interface ITntSource
    {
        /// <summary>
        /// Gets the width of the map in graphic tiles.
        /// </summary>
        int DataWidth { get; }

        /// <summary>
        /// Gets the height of the map in graphic tiles.
        /// </summary>
        int DataHeight { get; }

        /// <summary>
        /// Gets the map's sealevel.
        /// </summary>
        int SeaLevel { get; }

        /// <summary>
        /// Gets the number of tiles in the map's tile list.
        /// </summary>
        int TileCount { get; }

        /// <summary>
        /// Gets the number of feature names in the map's feature list.
        /// </summary>
        int AnimCount { get; }

        /// <summary>
        /// Enumerates the map's tile list.
        /// Each tile is an array of binary data.
        /// </summary>
        /// <returns>An enumeration of the tiles.</returns>
        IEnumerable<byte[]> EnumerateTiles();

        /// <summary>
        /// Enumerates the map's feature/animation list.
        /// Each entry is the name of the feature.
        /// </summary>
        /// <returns>An enumeration of the features.</returns>
        IEnumerable<string> EnumerateAnims();

        /// <summary>
        /// Enumerates the map tile data.
        /// Each cell contains an index indicating which tile in the tile list
        /// should be displayed at this location.
        /// The data is enumerated row by row, starting from the top left.
        /// The length of the data is <see cref="DataWidth"/> * <see cref="DataHeight"/>.
        /// </summary>
        /// <returns>An enumeration of the map tile data.</returns>
        IEnumerable<int> EnumerateData();

        /// <summary>
        /// Enumerates the map attribute data.
        /// Each cell contains a <see cref="TileAttr"/> structure
        /// indicating the attributes for the cell.
        /// The attributes are enumerated row by row, starting from the top left.
        /// The length of the data is <see cref="DataWidth"/> * <see cref="DataHeight"/> * 4.
        /// There are four attribute cells for each data cell.
        /// </summary>
        /// <returns>An enumeration of the map attributes.</returns>
        IEnumerable<TileAttr> EnumerateAttrs();

        /// <summary>
        /// Gets information about the map's minimap.
        /// </summary>
        /// <returns>The map's minimap info.</returns>
        MinimapInfo GetMinimap();
    }
}