using System;
using System.IO;
using CatNepNep.Exceptions;

namespace CatNepNep.CatFile
{
    class Directory2Cat
    {
        private string Directory { get; }
        private Cat CatNode { get; }

        public Directory2Cat(string directory)
        {
            Directory = directory;
            CatNode = new Cat();
        }

        public Cat GenerateCat()
        {
            if (!File.Exists(Directory + Path.DirectorySeparatorChar + "INFO.catBin")) throw new CatFolderNotRecognized();
            
            GetInfo();
            GetSizes();
            
            //If have names block
            if(CatNode.HasNames == 2) CatNode.NamesBlock = File.ReadAllBytes(Directory + Path.DirectorySeparatorChar + "NAMES.catBin");

            CatNode.HeaderBlock = File.ReadAllBytes(Directory + Path.DirectorySeparatorChar + "HEADER.catBin");

            return CatNode;
        }

        private void GetInfo()
        {
            var info = File.ReadAllLines(Directory + Path.DirectorySeparatorChar + "INFO.catBin");
            
            CatNode.HasNames = Convert.ToUInt32(info[0]);
            CatNode.NumberOfEntries = Convert.ToUInt32(info[1]);
            CatNode.HeaderType = Convert.ToUInt32(info[2]);
            CatNode.HeaderTwoSize = Convert.ToUInt32(info[3]);

            CatNode.Blocks = new byte[CatNode.NumberOfEntries][];

            if (CatNode.HeaderType == 3) CatNode.Names = new string[CatNode.NumberOfEntries];

            for(var i = 0; i < CatNode.NumberOfEntries; i++)
            {
                var name = info[i + 4];

                if (!File.Exists(Directory + Path.DirectorySeparatorChar + name)) throw new CatFolderDamaged();
                if (CatNode.HeaderType == 3) CatNode.Names[i] = name;
                var nameNew = Path.GetFileNameWithoutExtension(name) + "_new" + Path.GetExtension(name);
                CatNode.Blocks[i] = File.Exists(Directory + Path.DirectorySeparatorChar + nameNew)
                    ? File.ReadAllBytes(Directory + Path.DirectorySeparatorChar + nameNew)
                    : File.ReadAllBytes(Directory + Path.DirectorySeparatorChar + name);
            }
        }

        private void GetSizes()
        {
            CatNode.Sizes = new uint[CatNode.NumberOfEntries];

            for (var i = 0; i < CatNode.NumberOfEntries; i++)
            {
                CatNode.Sizes[i] = (uint)CatNode.Blocks[i].Length;
            }
        }

    }
}
