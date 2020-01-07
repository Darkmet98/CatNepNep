// Copyright (C) 2019 Pedro Garau Martínez
//
// This file is part of CatNepNep.
//
// CatNepNep is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CatNepNep is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with CatNepNep. If not, see <http://www.gnu.org/licenses/>.
//

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
                Result.Text[i] = Reader.ReadString(GetLength()); //Read the string

                if (Result.Type == 2)
                {
                    Reader.Stream.Position = Result.Id[i]; //Go to the string id position
                    Result.TextId[i] = Reader.ReadString(GetLength()); //Read the string id
                }

            }
        }



        /// <summary>
        /// Return the length of the string
        /// </summary>
        private int GetLength()
        {
            //Initialize the length 
            int length = 0;
            //Save the current position from the reader
            Reader.Stream.PushCurrentPosition();
            //Read the current byte
            byte bit = Reader.ReadByte();
            //Read the length of the string
            while (bit != 0)
            {
                bit = Reader.ReadByte();
                length++;
            }

            //Return to the previous position
            Reader.Stream.PopPosition();
            //Return the length
            return length;
        }
    }
}
