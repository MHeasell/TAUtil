namespace TAUtil.Gdi.Palette
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// <para>
    /// Indexed color palette interface.
    /// </para>
    /// <para>
    /// Provides methods for indexed color lookups,
    /// reverse lookups
    /// and nearest neighbour searches.
    /// </para>
    /// </summary>
    public interface IPalette : IEnumerable<Color>
    {
        /// <summary>
        /// Gets the number of colors in the palette.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the color at the given index.
        /// </summary>
        /// <param name="index">The index of the color.</param>
        /// <returns>The color at the given index.</returns>
        Color this[int index] { get; }

        /// <summary>
        /// Looks up the index of the specified color.
        /// </summary>
        /// <param name="color">The color to look up.</param>
        /// <returns>The index of the color.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The color is not found in the palette.
        /// </exception>
        int LookUp(Color color);

        /// <summary>
        /// Returns true if the palette contains the given color.
        /// </summary>
        /// <param name="c">The color to look for.</param>
        /// <returns>true if the color is in the palette, false otherwise.</returns>
        bool Contains(Color c);

        /// <summary>
        /// Gets the index of the color nearest to the specified color.
        /// </summary>
        /// <param name="color">The color to find the nearest neighbour of.</param>
        /// <returns>The index of the nearest color.</returns>
        int GetNearest(Color color);
    }
}
