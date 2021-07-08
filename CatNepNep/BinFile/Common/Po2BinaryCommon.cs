using System.IO;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace CatNepNep.BinFile.Common
{
    public class Po2BinaryCommon : IConverter<Po, BinaryFormat>
    {

        public DataReader OriginalFile { get; set; }
        protected DataWriter Writer;
        private BinaryFormat _binary;
        protected Po Po;

        public BinaryFormat Convert(Po source)
        {

            //Generate the new dat
            GenerateFile();

            //Dump the Po
            Po = source;

            //Insert the translated text
            InsertText();

            //Return the stream from the new file
            return new BinaryFormat(_binary.Stream);
        }

        public virtual void InsertText()
        { }


        protected void WriteText(int index, int size)
        {
            //Get the text
            var result = string.IsNullOrEmpty(Po.Entries[index].Translated) ?
                Po.Entries[index].Original : Po.Entries[index].Translated;

            //Skip if is null
            if (result == "<!null>")
            {
                Writer.Stream.Position += size;
                return;
            }
            
            //Write the text
            Writer.Write((Po2BinaryFormat.Dictionary != null)?Po2BinaryFormat.ReplaceText(result):result, size);
        }


        private void GenerateFile()
        {
            //Generate the exported file
            _binary = new BinaryFormat();
            Writer = new DataWriter(_binary.Stream) {DefaultEncoding = Encoding.GetEncoding("shift_jis")};

            //Dump the original executable to the stream
            OriginalFile.Stream.WriteTo(Writer.Stream);

            //Go to the initial index
            Writer.Stream.Position = 0;
        }
    }
}
