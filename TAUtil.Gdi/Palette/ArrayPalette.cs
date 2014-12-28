namespace TAUtil.Gdi.Palette
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public class ArrayPalette : IPalette
    {
        private readonly Color[] palette;

        private readonly Dictionary<Color, int> reversePalette;

        public ArrayPalette(int size)
        {
            this.palette = new Color[size];
            this.reversePalette = new Dictionary<Color, int>();
        }

        public int Count
        {
            get
            {
                return this.palette.Length;
            }
        }

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

        public int LookUp(Color color)
        {
            return this.reversePalette[color];
        }

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

        public IEnumerator<Color> GetEnumerator()
        {
            foreach (var c in this.palette)
            {
                yield return c;
            }
        }

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
