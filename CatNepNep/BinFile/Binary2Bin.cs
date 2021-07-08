using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace CatNepNep.BinFile
{
    class Binary2Bin : IConverter<BinaryFormat, Bin>
    {
        private Bin Result { get; set; }
        protected DataReader Reader { get; set; }

        /// <summary>
        /// Convert the binary file to Cat format
        /// </summary>
        public BinFile.Bin Convert(BinaryFormat source)
        {

            // Make sure that the shift-jis encoding is initialized in
            // .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //Initialize the format
            Result = new Bin();

            //Initialize the reader
            Reader = new DataReader(source.Stream)
            {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
                Endianness = EndiannessMode.LittleEndian
            };

            DumpHeader();
            DumpText();

            return Result;
        }

        /// <summary>
        /// Dump the header
        /// </summary>
        private void DumpHeader()
        {
            //Read the number of strings of the file
            Result.Count = Reader.ReadUInt32();

            //Check the bin type
            Reader.Stream.Position += 4;
            uint check = Reader.ReadUInt32();
            Result.Type = 0;
            Result.Type = check > 0x10000000 ? 1 : 2;
            Reader.Stream.Position -= 8;

            //Initialize the arrays
            Result.Positions = new uint[Result.Count];
            Result.Text = new string[Result.Count];
            Result.TextId = new string[Result.Count];
            Result.Id = new uint[Result.Count];

            //Dump the header
            for (var i = 0; i < Result.Count; i++)
            {
                if (Result.Type == 1)
                {
                    Result.Positions[i] = Reader.ReadUInt32(); //Read the position
                    Result.Id[i] = Reader.ReadUInt32(); //Read the id value
                }
                else
                {
                    Result.Id[i] = Reader.ReadUInt32(); //Read the id value
                    Result.Positions[i] = Reader.ReadUInt32(); //Read the position
                }
                
            }
        }

        /// <summary>
        /// Dump the text from the binary file
        /// </summary>
        private void DumpText()
        {
            for (var i = 0; i < Result.Count; i++)
            {
                Reader.Stream.Position = Result.Positions[i]; //Go to the string position
                Result.Text[i] = Reader.ReadString(GetLength(Reader)); //Read the string

                if (Result.Type == 2)
                {
                    Reader.Stream.Position = Result.Id[i]; //Go to the string id position
                    Result.TextId[i] = Reader.ReadString(GetLength(Reader)); //Read the string id
                }

            }
        }



        /// <summary>
        /// Return the length of the string
        /// </summary>
        public static int GetLength(DataReader reader)
        {
            //Initialize the length 
            int length = 0;
            //Save the current position from the reader
            reader.Stream.PushCurrentPosition();
            //Read the current byte
            byte bit = reader.ReadByte();
            //Read the length of the string
            while (bit != 0)
            {
                bit = reader.ReadByte();
                length++;
            }

            //Return to the previous position
            reader.Stream.PopPosition();
            //Return the length
            return length;
        }
    }
}
