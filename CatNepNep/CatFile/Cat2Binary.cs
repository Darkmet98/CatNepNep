using System;
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
            //Number of repetitions
            var count = 2;

            //Initialize the array
            Cat.Positions = new uint[Cat.NumberOfEntries];
            if (Cat.HeaderType == 3)
            {
                count++;
                Cat.NamesPositions = new uint[Cat.NumberOfEntries];
            }

            //Generate the pointer zone

            //Padding
            Writer.WriteTimes(0, 4);
            //Content
            Writer.WriteTimes(0, (4*count)*Cat.NumberOfEntries);
            //Padding
            Writer.WriteTimes(0, 4);

            //Write the files and get the positions
            for (var i = 0; i < Cat.NumberOfEntries; i++)
            {
                Cat.Positions[i] = (uint)Writer.Stream.Position - Cat.HeaderTwoSize;
                Writer.Write(Cat.Blocks[i]);
                Writer.WritePadding(0x00, 0x10);
                if (Cat.HeaderType == 3)
                {
                    Cat.NamesPositions[i] = (uint)Writer.Stream.Position - Cat.HeaderTwoSize;
                    Writer.Write(Cat.Names[i]);
                    Writer.WritePadding(0x00, 0x10);
                }
            }

            //Go again to the pointer zone and write the info
            Writer.Stream.Position = Cat.HeaderTwoSize + 0x14;
            for (var e = 0; e < count; e++)
            {
                for (var i = 0; i < Cat.NumberOfEntries; i++)
                {
                    switch (e)
                    {
                        case 0:
                            Writer.Write(Cat.Positions[i]);
                            break;
                        case 1:
                            Writer.Write(Cat.Sizes[i]);
                            break;
                        default:
                            Writer.Write(Cat.NamesPositions[i]);
                            break;
                    }
                    
                }
            }
        }

        private void UpdateHeader()
        {
            switch (Cat.HeaderType)
            {
                case 1:
                case 3:
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
                    throw new NotImplementedException();
            }
        }
    }
}
