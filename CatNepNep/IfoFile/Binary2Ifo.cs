
using Yarhl.FileFormat;
using Yarhl.IO;

namespace CatNepNep.IfoFile
{
    public class Binary2Ifo : IConverter<BinaryFormat, Ifo>
    {
        private Ifo ifo;
        private DataReader reader;

        public Ifo Convert(BinaryFormat source)
        {
            reader = new DataReader(source.Stream) {Stream = {Position = 0}};
            ifo = new Ifo();

            ReadHeader();
            DumpEntries();

            return ifo;
        }

        private void ReadHeader()
        {
            ifo.HeaderEntryCount = reader.ReadInt32();
            ifo.HeaderEntrySize = reader.ReadInt32();

            ifo.TextEntryCount = reader.ReadInt32();
            ifo.TextEntrySize = reader.ReadInt32();
        }

        private void DumpEntries()
        {
            for (int i = 0; i < ifo.HeaderEntryCount; i++)
            {
                ifo.HeaderEntry.Add(reader.ReadBytes(ifo.HeaderEntrySize));
            }

            for (int i = 0; i < ifo.TextEntryCount; i++)
            {
                ifo.TextEntry.Add(reader.ReadString(ifo.TextEntrySize).Replace("\0", ""));
            }
        }
    }
}
