using System.IO;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace CatNepNep.BinFile.Dlc
{
    public class Po2Binary : IConverter<Po, BinaryFormat>
    {
        public BinaryFormat Convert(Po source)
        {
            var writer = new DataWriter(new DataStream())
            {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
            };

            if (File.Exists("Dictionary.map"))
                Po2BinaryFormat.GenerateDictionary("Dictionary.map");

            var entry = source.Entries[0];
            writer.Write(System.Convert.FromBase64String(entry.Context));
            writer.Write((Po2BinaryFormat.Dictionary != null) ? Po2BinaryFormat.ReplaceText(entry.Text) : entry.Text);

            return new BinaryFormat(writer.Stream);
        }
    }
}
