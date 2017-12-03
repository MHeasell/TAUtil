namespace TAUtil.Hpi2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    public class HpiArchive
    {
        public enum CompressionScheme
        {
            None = 0,
            LZ77,
            ZLib
        }

        public abstract class DirectoryEntry
        {
            public string Name;

            public DirectoryEntry(string name)
            {
                Name = name;
            }
        }

        public class DirectoryInfo : DirectoryEntry
        {
            public List<DirectoryEntry> Entries;

            public DirectoryInfo(string name, List<DirectoryEntry> entries) : base(name)
            {
                Entries = entries;
            }
        }

        public class FileInfo : DirectoryEntry
        {
            public int Size;
            public int Offset;
            public CompressionScheme CompressionScheme;

            public FileInfo(string name, int size, int offset, CompressionScheme compressionScheme) : base(name)
            {
                Size = size;
                Offset = offset;
                CompressionScheme = compressionScheme;
            }
        }

        private byte decryptionKey;

        private DirectoryInfo root;

        private BinaryReader reader;

        public HpiArchive(BinaryReader r)
        {
            HpiVersion v;
            HpiVersion.Read(r, out v);

            if (v.Marker != HpiVersion.MagicNumber)
            {
                throw new Exception("Invalid HPI magic number");
            }

            if (v.Version != HpiVersion.VersionNumber)
            {
                throw new Exception("Unsupported HPI version");
            }

            HpiHeader h;
            HpiHeader.Read(r, out h);

            this.decryptionKey = TransformKey((byte)h.HeaderKey);

            var data = new byte[h.DirectorySize];

            r.BaseStream.Seek(h.Start, SeekOrigin.Begin);

            ReadAndDecrypt(r, decryptionKey, data, (int)h.Start, (int)(h.DirectorySize - h.Start));

            var dataReader = new BinaryReader(new MemoryStream(data));
            dataReader.BaseStream.Seek(h.Start, SeekOrigin.Begin);

            HpiDirectoryData directory;
            HpiDirectoryData.Read(dataReader, out directory);

            this.root = new HpiArchive.DirectoryInfo(string.Empty, ConvertDirectoryEntries(directory, dataReader));
            this.reader = r;
        }

        public HpiArchive(string filename)
            : this(new BinaryReader(File.OpenRead(filename)))
        {
        }

        /// <summary>
        /// Returns the info about the file at the given path,
        /// or null if no such file exists.
        /// </summary>
        public FileInfo FindFile(string path)
        {
            var components = path.Split(new[] { HpiPath.DirectorySeparatorChar, HpiPath.AltDirectorySeparatorChar });

            var currentDir = root;

            for (int i = 0; i < components.Length - 1; ++i)
            {
                var c = components[i];
                var entry = currentDir.Entries.Find(x => string.Equals(x.Name, c, StringComparison.OrdinalIgnoreCase));
                if (entry == null)
                {
                    return null;
                }

                var d = entry as DirectoryInfo;
                if (d == null)
                {
                    return null;
                }

                currentDir = d;
            }

            return FindFileInner(currentDir, components[components.Length - 1]);
        }

        public void Extract(FileInfo file, byte[] buffer)
        {
            var chunkCount = (file.Size / 65536) + (file.Size % 65536 == 0 ? 0 : 1);
            reader.BaseStream.Seek(file.Offset, SeekOrigin.Begin);

            // skip chunk sizes
            reader.BaseStream.Seek(chunkCount * sizeof(UInt32), SeekOrigin.Current);

            int bufferOffset = 0;
            for (int i = 0; i < chunkCount; ++i)
            {
                HpiChunk chunkHeader;
                HpiChunk.Read(reader, out chunkHeader);
                if (chunkHeader.Marker != HpiChunk.MagicNumber)
                {
                    throw new Exception("Invalid chunk header");
                }

                var chunkBuffer = new byte[chunkHeader.CompressedSize];
                ReadAndDecrypt(reader, decryptionKey, chunkBuffer, 0, (int)chunkHeader.CompressedSize);

                var checksum = ComputeChecksum(chunkBuffer, 0, (int)chunkHeader.CompressedSize);
                if (checksum != chunkHeader.Checksum)
                {
                    throw new Exception("Invalid chunk checksum");
                }

                if (chunkHeader.Encrypted != 0)
                {
                    DecryptInner(chunkBuffer, 0, (int)chunkHeader.CompressedSize);
                }

                switch (chunkHeader.CompressionScheme)
                {
                    case 0: // no compression
                        if (chunkHeader.CompressedSize != chunkHeader.DecompressedSize)
                        {
                            throw new Exception("Uncompressed chunk has different decompressed and compressed sizes");
                        }
                        Array.Copy(chunkBuffer, 0, buffer, bufferOffset, chunkHeader.CompressedSize);
                        bufferOffset += (int)chunkHeader.DecompressedSize;
                        break;

                    case 1: // LZ77 compression
                        DecompressLZ77(chunkBuffer, 0, (int)chunkHeader.CompressedSize, buffer, bufferOffset, (int)chunkHeader.DecompressedSize);
                        bufferOffset += (int)chunkHeader.DecompressedSize;
                        break;

                    case 2: // ZLib compression
                        DecompressZlib(chunkBuffer, 0, (int)chunkHeader.CompressedSize, buffer, bufferOffset, (int)chunkHeader.DecompressedSize);
                        bufferOffset += (int)chunkHeader.DecompressedSize;
                        break;

                    default:
                        throw new Exception("Invalid compression scheme");
                }
            }
        }

        private static void DecompressZlib(byte[] input, int inputOffset, int inputSize, byte[] output, int outputOffset, int outputSize)
        {
            var inputStream = new DeflateStream(new MemoryStream(input, inputOffset, inputSize), CompressionMode.Decompress);
            var outputStream = new MemoryStream(output, outputOffset, outputSize);
            inputStream.CopyTo(outputStream);
        }

        private static void DecompressLZ77(byte[] input, int inputOffset, int inputSize, byte[] output, int outputOffset, int outputSize)
        {
            var window = new byte[4096];

            int windowPos = 1;

            var reader = new BinaryReader(new MemoryStream(input, inputOffset, inputSize));
            var writer = new BinaryWriter(new MemoryStream(output, outputOffset, outputSize));

            while (true)
            {
                var tag = reader.ReadByte();

                for (int i = 0; i < 8; ++i)
                {
                    if ((tag & 1) == 0) // next byte is a literal byte
                    {
                        var value = reader.ReadByte();
                        writer.Write(value);
                        window[windowPos] = value;
                        windowPos = (windowPos + 1) & 0xFFF;
                    }
                    else // next byte points into the sliding window
                    {
                        var packedData = reader.ReadUInt16();
                        var windowOffset = packedData >> 4;
                        if (windowOffset == 0)
                        {
                            return;
                        }

                        var count = (packedData & 0x0F) + 2;
                        for (int x = 0; x < count; ++x)
                        {
                            writer.Write(window[windowOffset]);
                            window[windowPos] = window[windowOffset];
                            windowOffset = (windowOffset + 1) & 0xFFF;
                        }
                    }

                    tag >>= 1;
                }
            }
        }

        private static void DecryptInner(byte[] buffer, int offset, int size)
        {
            for (int i = 0; i < size; ++i)
            {
                var pos = (byte)i;
                buffer[offset + i] = (byte)((buffer[offset + i] - pos) ^ pos);
            }
        }

        private static uint ComputeChecksum(byte[] buffer, int offset, int size)
        {
            uint sum = 0;
            for (int i = 0; i < size; ++i)
            {
                sum += buffer[offset + i];
            }

            return sum;
        }

        private static byte TransformKey(byte key)
        {
            return (byte)((key << 2) | (key >> 6));
        }

        static private void ReadAndDecrypt(BinaryReader reader, byte key, byte[] buffer, int offset, int size)
        {
            var seed = (byte)reader.BaseStream.Position;
            var bytesRead = reader.Read(buffer, offset, size);
            Decrypt(key, seed, buffer, offset, bytesRead);
        }

        private static void Decrypt(byte key, byte seed, byte[] buffer, int offset, int size)
        {
            if (key == 0)
            {
                return;
            }

            for (int i = 0; i < size; ++i)
            {
                var pos = seed + i;
                buffer[offset + i] = (byte)((pos ^ key) ^ buffer[offset + i]);
            }
        }

        private FileInfo FindFileInner(DirectoryInfo currentDir, string v)
        {
            var entry = currentDir.Entries.Find(x => string.Equals(x.Name, v, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                return null;
            }

            return entry as FileInfo;
        }

        private List<DirectoryEntry> ConvertDirectoryEntries(HpiDirectoryData directory, BinaryReader reader)
        {
            var v = new List<DirectoryEntry>((int)directory.NumberOfEntries);
            for (int i = 0; i < directory.NumberOfEntries; ++i)
            {
                var seekOffset = directory.EntryListOffset + (i * HpiDirectoryEntry.StructureSizeInBytes);
                reader.BaseStream.Seek(seekOffset, SeekOrigin.Begin);

                HpiDirectoryEntry entry;
                HpiDirectoryEntry.Read(reader, out entry);
                v.Add(ConvertDirectoryEntry(entry, reader));
            }

            return v;
        }

        private DirectoryEntry ConvertDirectoryEntry(HpiDirectoryEntry entry, BinaryReader reader)
        {
            reader.BaseStream.Seek(entry.NameOffset, SeekOrigin.Begin);
            var name = Util.ReadNullTerminatedString(reader);

            if (entry.IsDirectory != 0)
            {
                reader.BaseStream.Seek(entry.DataOffset, SeekOrigin.Begin);
                HpiDirectoryData d;
                HpiDirectoryData.Read(reader, out d);
                var subEntries = ConvertDirectoryEntries(d, reader);
                return new DirectoryInfo(name, subEntries);
            }
            else
            {
                reader.BaseStream.Seek(entry.DataOffset, SeekOrigin.Begin);
                HpiFileData f;
                HpiFileData.Read(reader, out f);
                return ConvertFile(name, f);
            }
        }

        private DirectoryEntry ConvertFile(string name, HpiFileData f)
        {
            return new FileInfo(name, (int)f.FileSize, (int)f.DataOffset, (CompressionScheme)f.CompressionScheme);
        }
    }
}
