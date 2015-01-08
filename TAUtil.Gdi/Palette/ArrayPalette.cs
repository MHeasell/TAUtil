namespace TAUtil.Gdi.Palette
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Indexed color palette backed by a fixed size array.
    /// </summary>
    public class ArrayPalette : IPalette
    {
        private readonly Color[] palette;

        private readonly Dictionary<Color, int> reversePalette;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayPalette"/> class.
        /// </summary>
        /// <param name="size">The number of colors in the palette.</param>
        public ArrayPalette(int size)
        {
            this.palette = new Color[size];
            this.reversePalette = new Dictionary<Color, int>();
        }

        /// <summary>
        /// Gets the color count. See <see cref="IPalette.Count"/>.
        /// </summary>
        public int Count
        {
            get
            {
                return this.palette.Length;
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
                return this.palette[index];
            }

            set
            {
                this.palette[index] = value;
                this.reversePalette[value] = index;
            }
        }

        /// <summary>
        /// Looks up the index for the color. See <see cref="IPalette.LookUp"/>.
        /// </summary>
        /// <param name="color">The color to look up.</param>
        /// <returns>The index of the given color.</returns>
        public int LookUp(Color color)
        {
            return this.reversePalette[color];
        }

        /// <summary>
        /// Determines whether the palette contains the given color.
        /// See <see cref="IPalette.Contains"/>.
        /// </summary>
        /// <param name="c">The color to look for.</param>
        /// <returns>true if the palette contains the color, false otherwise.</returns>
        public bool Contains(Color c)
        {
            return this.reversePalette.ContainsKey(c);
        }

        /// <summary>
        /// Gets the index of the color nearest to the input. See <see cref="IPalette.GetNearest"/>.
        /// </summary>
        /// <param name="color">The color to find the nearest neighbour of.</param>
        /// <returns>The index of the nearest color.</returns>
        public int GetNearest(Color color)
        {
            int winIndex = -1;
            int winDistance = int.MaxValue;
            for (var i = 0; i < this.Count; i++)
            {
                var dist = DistanceSquared(this[i], color);
                if (dist == 0)
                {
                    return i;
                }

                if (dist < winDistance)
                {
                    winIndex = i;
                    winDistance = dist;
                }
            }

            return winIndex;
        }

        /// <summary>
        /// Gets the enumerator. See <see cref="IEnumerable{T}.GetEnumerator"/>.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Color> GetEnumerator()
        {
            foreach (var c in this.palette)
            {
                yield return c;
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

        private static int DistanceSquared(Color c1, Color c2)
        {
            int dr = c2.R - c1.R;
            int dg = c2.G - c1.G;
            int db = c2.B - c1.B;
            return (dr * dr) + (dg * dg) + (db * db);
        }
    }
}
