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
using Yarhl.FileFormat;
using Yarhl.IO;

namespace CatNepNep.CatFile
{
    class Cat2Binary : IConverter<Cat, BinaryFormat>
    {
        private DataWriter Writer { get; set; }
        private Cat Cat { get; set; }

        public BinaryFormat Convert(Cat source)
        {
            Writer = new DataWriter(new DataStream())
            {
              Endianness  = EndiannessMode.LittleEndian
            };
            Cat = source;

            //First, write the header
            Writer.Write(Cat.HeaderBlock);
            GeneratePointerZone();
            if (Cat.HasNames == 2)
            {
                Writer.Stream.Position = Writer.Stream.Length;
                Writer.Write(Cat.NamesBlock);
            }
            UpdateHeader();

            return new BinaryFormat(Writer.Stream);
        }

        private void GeneratePointerZone()
        {
            //Generate the pointer zone
            Writer.WriteTimes(0, 8*(Cat.NumberOfEntries+1));

            //Initialize the array
            Cat.Positions = new uint[Cat.NumberOfEntries];

            //Write the files and get the positions
            for (var i = 0; i < Cat.NumberOfEntries; i++)
            {
                Cat.Positions[i] = (uint)Writer.Stream.Position - Cat.HeaderTwoSize;
                Writer.Write(Cat.Blocks[i]);
                Writer.WritePadding(0x00, 0x10);
            }

            //Go again to the pointer zone and write the info
            Writer.Stream.Position = Cat.HeaderTwoSize + 0x14;
            for (var e = 0; e < 2; e++)
            {
                for (var i = 0; i < Cat.NumberOfEntries; i++)
                {
                    Writer.Write(e == 0 ? Cat.Positions[i] : Cat.Sizes[i]);
                }
            }
        }

        private void UpdateHeader()
        {
            switch (Cat.HeaderType)
            {
                case 1:
                    int length = (int)Writer.Stream.Length;
                    if (Cat.HasNames == 2)
                    {
                        Writer.Stream.Position = 0x10;
                        Writer.Write((uint)(length - Cat.NamesBlock.Length)); //Write the File length without the name block
                        Writer.Write((uint)(length - Cat.NamesBlock.Length - Cat.HeaderTwoSize)); //Write the File length without the name block and header 2
                    }
                    else
                    {
                        Writer.Stream.Position = 0x10;
                        Writer.Write((uint)(length - Cat.HeaderTwoSize)); //Write the File length without the header 2
                    }
                    break;

                case 2:
                    //???
                    break;

                case 3:
                    //TODO
                    break;
            }
        }
    }
}
