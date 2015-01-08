namespace TAUtil.Gdi.Palette
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Wrapper for an IPalette instance
    /// that provides support for using one of the index values
    /// as a transparency mask.
    /// </summary>
    public class TransparencyMaskedPalette : IPalette
    {
        private readonly IPalette palette;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransparencyMaskedPalette"/> class.
        /// </summary>
        /// <param name="palette">The underlying palette to wrap.</param>
        /// <param name="transparencyIndex">The index to use for transparency.</param>
        public TransparencyMaskedPalette(IPalette palette, int transparencyIndex = -1)
        {
            this.palette = palette;
            this.TransparencyIndex = transparencyIndex;
        }

        /// <summary>
        /// Gets or sets the index to use for transparency.
        /// </summary>
        public int TransparencyIndex { get; set; }

        /// <summary>
        /// Gets the color count. See <see cref="IPalette.Count"/>.
        /// </summary>
        public int Count
        {
            get
            {
                return this.palette.Count;
            }
        }

        /// <summary>
        /// Gets the color at the given index. See <see cref="IPalette.this"/>.
        /// </summary>
        /// <param name="index">The index to get.</param>
        /// <returns>The color at the given index.</returns>
        public Color this[int index]
        {
            get
            {
                return index == this.TransparencyIndex ? Color.Transparent : this.palette[index];
            }
        }

        /// <summary>
        /// Looks up the index for the color. See <see cref="IPalette.LookUp"/>.
        /// </summary>
        /// <param name="color">The color to look up.</param>
        /// <returns>The index of the given color.</returns>
        public int LookUp(Color color)
        {
            return color == Color.Transparent ? this.TransparencyIndex : this.palette.LookUp(color);
        }

        /// <summary>
        /// Returns true if the palette contains the given color.
        /// See <see cref="IPalette.Contains"/>.
        /// </summary>
        /// <param name="c">The color to look for.</param>
        /// <returns>true if the color is in the palette, false otherwise.</returns>
        public bool Contains(Color c)
        {
            return c == Color.Transparent || this.palette.Contains(c);
        }

        /// <summary>
        /// Gets the index of the color nearest to the input. See <see cref="IPalette.GetNearest"/>.
        /// </summary>
        /// <param name="color">The color to find the nearest neighbour of.</param>
        /// <returns>The index of the nearest color.</returns>
        public int GetNearest(Color color)
        {
            return color == Color.Transparent ? this.TransparencyIndex : this.palette.GetNearest(color);
        }

        /// <summary>
        /// Gets the enumerator. See <see cref="IEnumerable{T}.GetEnumerator"/>.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Color> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Gets the enumerator. See <see cref="IEnumerable{T}.GetEnumerator"/>.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
