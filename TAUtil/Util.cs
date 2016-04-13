namespace TAUtil
{
    using System;

    using TAUtil.Tnt;

    internal static class Util
    {
        public static string ConvertChars(byte[] data)
        {
            int i = Array.IndexOf<byte>(data, 0);

            if (i == -1)
            {
                throw new ArgumentException("Data is not null-terminated");
            }

            return System.Text.Encoding.ASCII.GetString(data, 0, i);
        }

        public static Size GetMinimapActualSize(byte[] data, int width, int height)
        {
            int realHeight = 0;
            int realWidth = 0;

            // find the real width by scanning across from the right
            // until we encounter non-void data.
            for (int x = (width - 1); x >= 0; x--)
            {
                if (data[x] != TntConstants.MinimapVoidByte)
                {
                    realWidth = x + 1;
                    break;
                }
            }

            // Find the real height by scanning upwards from the bottom
            // until we encounter non-void data.
            for (int y = (height - 1); y >= 0; y--)
            {
                if (data[y * width] != TntConstants.MinimapVoidByte)
                {
                    realHeight = y + 1;
                    break;
                }
            }

            return new Size(realWidth, realHeight);
        }

        public static byte[] TrimMinimapBytes(byte[] data, int width, int height, int newWidth, int newHeight)
        {
            byte[] newData = new byte[newWidth * newHeight];

            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    newData[(y * newWidth) + x] = data[(y * width) + x];
                }
            }

            return newData;
        }

        internal struct Size
        {
            public Size(int width, int height)
                : this()
            {
                this.Width = width;
                this.Height = height;
            }

            public int Width { get; private set; }

            public int Height { get; private set; }
        }
    }
}
