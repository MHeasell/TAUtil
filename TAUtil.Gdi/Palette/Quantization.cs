namespace TAUtil.Gdi.Palette
{
    using System.Drawing;
    using System.Drawing.Imaging;

    public static class Quantization
    {
        public static void ToTAPalette(Bitmap bmp)
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int length = bmp.Width * bmp.Height;
            unsafe
            {
                int* pointer = (int*)data.Scan0;
                for (int i = 0; i < length; i++)
                {
                    var c = Color.FromArgb(pointer[i]);
                    int nearest = PaletteFactory.TAPalette.GetNearest(c);
                    var col = PaletteFactory.TAPalette[nearest];
                    pointer[i] = col.ToArgb();
                }
            }

            bmp.UnlockBits(data);
        }
    }
}
