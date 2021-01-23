using System;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace CatNepNep.BinFile.Dlc
{
    public class Binary2Po : IConverter<BinaryFormat, Po>
    {
        private Po po;
        private DataReader reader;

        public Po Convert(BinaryFormat source)
        {
            // Read the language used by the user' OS, this way the editor can spellchecker the translation. - Thanks Liquid_S por the code
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            po = new Po
            {
                Header = new PoHeader("Hyperdimension Neptunia", "dummy@dummy.com", currentCulture.Name)
                {
                    Extensions = { { "Type", "4" } }
                },
            };
            reader = new DataReader(source.Stream)
            {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
            };

            ReadData();

            return po;
        }

        private void ReadData()
        {
            var bytes = reader.ReadBytes(0xC);
            
            if (bytes[4] != 0xC)
                throw new Exception("This not a binary dlc text entry!");

            var text = reader.ReadString().Replace("\0", "");

            po.Add(new PoEntry(text)
            {
                Context = System.Convert.ToBase64String(bytes)
            });
        }
    }
}
