using System.IO;
using System.Text;
using CatNepNep.BinFile;
using CatNepNep.Exceptions;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace CatNepNep.CatFile
{
    class Binary2Cat : IConverter<BinaryFormat, Cat>
    {
        private Cat Result { get; set; }
        private DataReader Reader { get; set; }
        public bool Legacy { get; set; }

        public Cat Convert(BinaryFormat source)
        {
            Result = new Cat();
            Reader = new DataReader(source.Stream);

            Result.HeaderType = Reader.ReadUInt32(); //Read the Cat Type
            Result.HasNames = Reader.ReadUInt32(); //Read if has names
            Reader.ReadUInt32(); //Padding
            Result.HeaderTwoSize = Reader.ReadUInt32(); //Read the size of Header Two

            CheckCatType();
            

            return Result;
        }


        private void CheckCatType()
        {
            switch (Result.HeaderType)
            {
                case 1:
                    CheckNames();
                    ReadBlocksInformation(false);
                    break;

                case 3:
                    ReadHeaderTwoType1(false);
                    GetBlocks(false);
                    ReadBlocksInformation(true);
                    break;
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

        private void ReadBlocksInformation(bool type3)
        {
            //Go to the pointers block
            Reader.Stream.Position = Result.HeaderTwoSize + 0x14;

            //Initialize the Cat Blocks
            Result.Positions = new uint[Result.NumberOfEntries];
            Result.Sizes = new uint[Result.NumberOfEntries];
            Result.Blocks = new byte[Result.NumberOfEntries][];
            var count = 2;
            if (type3)
            {
                Result.Names = new string[Result.NumberOfEntries];
                Result.NamesPositions = new uint[Result.NumberOfEntries];
                count = 4;
            }

            //Start the dump
            for (var e = 0; e < count; e++)
            {
                for (var i = 0; i < Result.NumberOfEntries; i++)
                {
                    switch (e)
                    {
                        case 0:
                            Result.Positions[i] = Reader.ReadUInt32() + Result.HeaderTwoSize; //Dump the positions
                            break;
                        case 1:
                            Result.Sizes[i] = Reader.ReadUInt32(); //Dump the sizes
                            break;
                        case 2:
                            Result.NamesPositions[i] = Reader.ReadUInt32() + Result.HeaderTwoSize; //Dump the name position
                            break;
                        //Read the blocks
                        default:
                            Reader.Stream.Position = Result.NamesPositions[i];
                            Result.Names[i] = Reader.ReadString(Binary2Bin.GetLength(Reader));
                            break;
                    }
                }
            }

            //Read the arrays
            for (var i = 0; i < Result.NumberOfEntries; i++)
            {
                Reader.Stream.Position = Result.Positions[i];
                Result.Blocks[i] = Reader.ReadBytes((int)Result.Sizes[i]);
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
                Result.Names[i] = (Legacy)
                    ? $"{Reader.ReadString((int)Result.SizeNameChain).Replace("\0", "")}.bin" 
                    : $"{i}_{Reader.ReadString((int)Result.SizeNameChain).Replace("\0", "")}.bin";
            }
        }

    }
}
