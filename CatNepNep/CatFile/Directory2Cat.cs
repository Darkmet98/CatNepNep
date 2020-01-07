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

            for(var i = 0; i < CatNode.NumberOfEntries; i++)
            {
                CatNode.Blocks[i] = File.ReadAllBytes(Directory + Path.DirectorySeparatorChar + info[i + 4]);
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
