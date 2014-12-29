namespace TAUtil.Gdi.Bitmap
{
    using System.Drawing;
    using System.IO;

    using TAUtil.Gdi.Palette;

    /// <summary>
    /// Provides methods for converting between <see cref="Bitmap"/> instances
    /// and serialized binary images using the TA indexed color palette.
    /// </summary>
    public static class BitmapConvert
    {
        private static readonly BitmapSerializer Serializer;

        private static readonly BitmapDeserializer Deserializer;

        static BitmapConvert()
        {
            Serializer = new BitmapSerializer(PaletteFactory.TAPalette);
            Deserializer = new BitmapDeserializer(PaletteFactory.TAPalette);
        }

        /// <summary>
        /// Deserializes a bitmap from the given stream.
        /// </summary>
        /// <param name="bytes">The stream to read from.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns>The deserialized bitmap.</returns>
        public static Bitmap ToBitmap(Stream bytes, int width, int height)
        {
            return Deserializer.Deserialize(bytes, width, height);
        }

        /// <summary>
        /// Deserializes a bitmap from the given byte array.
        /// </summary>
        /// <param name="bytes">The array of bytes to read from.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns>The deserialized bitmap.</returns>
        public static Bitmap ToBitmap(byte[] bytes, int width, int height)
        {
            return Deserializer.Deserialize(bytes, width, height);
        }

        /// <summary>
        /// Deserializes a bitmap from the given stream,
        /// using the specified color index as a transparency mask.
        /// </summary>
        /// <param name="bytes">The stream to read from.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="transparencyIndex">The color index used to indicate transparency.</param>
        /// <returns>The deserialized bitmap.</returns>
        public static Bitmap ToBitmap(Stream bytes, int width, int height, int transparencyIndex)
        {
            var transPalette = new TransparencyMaskedPalette(PaletteFactory.TAPalette, transparencyIndex);
            var des = new BitmapDeserializer(transPalette);
            return des.Deserialize(bytes, width, height);
        }

        /// <summary>
        /// Deserializes a bitmap from the given byte array,
        /// using the specified color index as a transparency mask.
        /// </summary>
        /// <param name="bytes">The array of bytes to read from.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="transparencyIndex">The color index used to indicate transparency.</param>
        /// <returns>The deserialized bitmap.</returns>
        public static Bitmap ToBitmap(byte[] bytes, int width, int height, int transparencyIndex)
        {
            var transPalette = new TransparencyMaskedPalette(PaletteFactory.TAPalette, transparencyIndex);
            var des = new BitmapDeserializer(transPalette);
            return des.Deserialize(bytes, width, height);
        }

        /// <summary>
        /// Serializes the given bitmap to an array of bytes.
        /// The bitmap is expected to use only colors present in the TA color palette.
        /// </summary>
        /// <param name="bitmap">The bitmap to serialize.</param>
        /// <returns>The serialized byte array.</returns>
        public static byte[] ToBytes(Bitmap bitmap)
        {
            return Serializer.ToBytes(bitmap);
        }

        /// <summary>
        /// Serializes the given bitmap to an array of bytes,
        /// using the specified color index to indicate transparency.
        /// The bitmap is expected to use only colors present in the TA color palette
        /// and the transparent color.
        /// </summary>
        /// <param name="bitmap">The bitmap to serialize.</param>
        /// <param name="transparencyIndex">The color index to use as transparency.</param>
        /// <returns>The serialized byte array.</returns>
        public static byte[] ToBytes(Bitmap bitmap, int transparencyIndex)
        {
            var transPalette = new TransparencyMaskedPalette(PaletteFactory.TAPalette, transparencyIndex);
            var ser = new BitmapSerializer(transPalette);
            return ser.ToBytes(bitmap);
        }

        /// <summary>
        /// Serializes the given bitmap to the given stream.
        /// The bitmap is expected to use only colors present in the TA color palette.
        /// </summary>
        /// <param name="s">The stream to write to.</param>
        /// <param name="bitmap">The bitmap to serialize.</param>
        public static void ToStream(Stream s, Bitmap bitmap)
        {
            Serializer.Serialize(s, bitmap);
        }

        /// <summary>
        /// Serializes the given bitmap to the given stream,
        /// using the specified color index to indicate transparency.
        /// The bitmap is expected to use only colors present in the TA color palette
        /// and the transparent color.
        /// </summary>
        /// <param name="s">The stream to write to.</param>
        /// <param name="bitmap">The bitmap to serialize.</param>
        /// <param name="transparencyIndex">The color index to use as transparency.</param>
        public static void ToStream(Stream s, Bitmap bitmap, int transparencyIndex)
        {
            var transPalette = new TransparencyMaskedPalette(PaletteFactory.TAPalette, transparencyIndex);
            var ser = new BitmapSerializer(transPalette);
            ser.Serialize(s, bitmap);
        }
    }
}
