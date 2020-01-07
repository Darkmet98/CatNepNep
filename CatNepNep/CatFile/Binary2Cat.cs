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

using System.IO;
using System.Text;
using CatNepNep.Exceptions;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace CatNepNep.CatFile
{
    class Binary2Cat : IConverter<BinaryFormat, Cat>
    {
        private Cat Result { get; set; }
        private DataReader Reader { get; set; }

        public Cat Convert(BinaryFormat source)
        {
            Result = new Cat();
            Reader = new DataReader(source.Stream)
            {
                Endianness = EndiannessMode.LittleEndian
            };

            Result.HeaderType = Reader.ReadUInt32(); //Read the Cat Type
            Result.HasNames = Reader.ReadUInt32(); //Read if has names
            Reader.ReadUInt32(); //Padding
            Result.HeaderTwoSize = Reader.ReadUInt32(); //Read the size of Header Two

            CheckCatType();
            ReadBlocksInformation();

            return Result;
        }


        private void CheckCatType()
        {
            switch (Result.HeaderType)
            {
                case 1:
                    CheckNames();
                    break;

                case 3:
                case 2:
                default:
                    throw new CatNotSupported();
            }
        }

        private void CheckNames()
        {
            switch (Result.HasNames)
            {
                case 1: //False
                    ReadHeaderTwoType1(false);
                    GetBlocks(false);
                    break;

                case 2: //True
                    ReadHeaderTwoType1(true);
                    GetBlocks(true);
                    ReadNames();
                    break;
                default:
                    throw new CatNotSupported();
            }
        }

        private void ReadHeaderTwoType1(bool hasNames)
        {
            Result.FileSize = Reader.ReadUInt32();
            
            if (hasNames)
            {
                Result.FileSizeTwo = Reader.ReadUInt32();
                Result.NamesSize = Reader.ReadUInt32();
            }
            else Reader.Stream.Position += 8;
            
            Reader.Stream.Position += 8;

            Result.NumberOfEntries = Reader.ReadUInt32();
        }

        private void ReadBlocksInformation()
        {
            //Go to the pointers block
            Reader.Stream.Position = Result.HeaderTwoSize + 0x14;

            //Initialize the Cat Blocks
            Result.Positions = new uint[Result.NumberOfEntries];
            Result.Sizes = new uint[Result.NumberOfEntries];
            Result.Blocks = new byte[Result.NumberOfEntries][];

            //Start the dump
            for (var e = 0; e < 3; e++)
            {
                for (var i = 0; i < Result.NumberOfEntries; i++)
                {
                    if (e == 0) Result.Positions[i] = Reader.ReadUInt32() + Result.HeaderTwoSize; //Dump the positions
                    else if (e == 1) Result.Sizes[i] = Reader.ReadUInt32(); //Dump the sizes
                    else //Read the blocks
                    {
                        Reader.Stream.Position = Result.Positions[i];
                        Result.Blocks[i] = Reader.ReadBytes((int)Result.Sizes[i]);
                    }
                }
            }
        }

        private void GetBlocks(bool hasNames)
        {
            //Go to the first position
            Reader.Stream.Position = 0;

            //Dump the header block
            Result.HeaderBlock = Reader.ReadBytes((int)Result.HeaderTwoSize + 0x10);

            if (hasNames)
            {
                //Go to the Names position
                Reader.Stream.Position = Result.FileSize;

                Result.NamesBlock = Reader.ReadBytes((int) Result.NamesSize);
            }

        }

        private void ReadNames()
        {
            Reader.Stream.Position = Result.FileSize + 0x14; //Skip the header info because it's not necessary to dump the required info
            Result.Names = new string[Result.NumberOfEntries];
            Result.SizeNameChain = Reader.ReadUInt32();
            Reader.Stream.Position = Result.FileSize + Result.HeaderTwoSize;

            for (var i = 0; i < Result.NumberOfEntries; i++)
            {
                Result.Names[i] = Reader.ReadString((int)Result.SizeNameChain, Encoding.UTF8).Replace("\0", "");
            }
        }

    }
}
