namespace TAUtil.Hpi2
{
    using System;
    using System.Collections.Generic;
    using System.IO;

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

        public HpiArchive(string filename)
            : this(new BinaryReader(File.OpenRead(filename)))
        {
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
    }
}
