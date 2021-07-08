using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace CatNepNep.TxtFile
{
    class Binary2Po : IConverter<BinaryFormat, Po>
    {
        private TextReader Reader { get; set; }
        protected Po Po { get; set; }

        public Binary2Po()
        {
            //Read the language used by the user' OS, this way the editor can spellcheck the translation. - Thanks Liquid_S por the code
            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            Po = new Po
            {
                Header = new PoHeader("Hyperdimension Neptunia U: Action Unleashed", "dummy@dummy.com", currentCulture.Name),
            };
            Po.Header.Extensions.Add("Type", "3"); //3 is txt file
        }

        public Po Convert(BinaryFormat source)
        {
            // Make sure that the shift-jis encoding is initialized in
            // .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //Initialize the reader with SJIS encoding
            Reader = new TextReader(source.Stream, Encoding.GetEncoding("shift_jis"));

            //Read the txt
            SeekTxt();

            return Po;
        }

        private void SeekTxt()
        {
            var i = 0;
            do
            {
                Reader.ReadToToken("{");
                var text = Reader.ReadToToken("}");
                if (!string.IsNullOrEmpty(text))
                {
                    var entry = new PoEntry(); //Generate the entry on the po file
                    entry.Original = text;
                    entry.Context = i.ToString();
                    i++;
                    Po.Add(entry);
                }
            } while (!Reader.Stream.EndOfStream);
        }
    }
}
