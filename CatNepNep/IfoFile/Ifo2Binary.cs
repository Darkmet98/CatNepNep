using System.IO;
using System.Text;
using CatNepNep.BinFile;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace CatNepNep.IfoFile
{
    public class Ifo2Binary : IConverter<Ifo, BinaryFormat>
    {
        private DataWriter writer;
        private Ifo ifo;

        public BinaryFormat Convert(Ifo source)
        {
            writer = new DataWriter(new DataStream())
            {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
            };
            ifo = source;

            if (File.Exists("Dictionary.map"))
                Po2BinaryFormat.GenerateDictionary("Dictionary.map");

            WriteHeader();
            WriteContent();

            return new BinaryFormat(writer.Stream);
        }

        private void WriteHeader()
        {
            writer.Write(ifo.HeaderEntryCount);
            writer.Write(ifo.HeaderEntrySize);

            writer.Write(ifo.TextEntryCount);
            writer.Write(ifo.TextEntrySize);
        }

        private void WriteContent()
        {
            foreach (var entry in ifo.HeaderEntry)
            {
                writer.Write(entry);
            }

            foreach (var entry in ifo.TextEntry)
            {
                // Write the text
                writer.Write((Po2BinaryFormat.Dictionary != null) ? Po2BinaryFormat.ReplaceText(entry) : entry, ifo.TextEntrySize);
            }
        }
    }
}
