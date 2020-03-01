// Copyright (C) 2020 Pedro Garau Martínez
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
namespace CatNepNep.BinFile.Common.Export
{
    internal class Binary2PoQuest : Binary2PoCommon
    {
        public override void DumpText()
        {
            Id = Reader.ReadString(0x14).Replace("\0", "");

            //Names
            Po.Add(GenerateEntry("Name", Reader.ReadString(0x28).Replace("\0", "")));

            //Skip unnecessary data
            Reader.Stream.Position += 0x8;

            //Characters
            Po.Add(GenerateEntry("Character", Reader.ReadString(0x1A).Replace("\0", "")));

            //Skip unnecessary data
            Reader.Stream.Position += 0x9E;

            //Clear Conditions
            Po.Add(GenerateEntry("Clear Conditions", Reader.ReadString(0x44).Replace("\0", "")));

            //Descriptions
            Po.Add(GenerateEntry("Descriptions", Reader.ReadString(0xC8).Replace("\0", "")));

            //Order
            Po.Add(GenerateEntry("Order", Reader.ReadString(0x40).Replace("\0", "")));
        }
    }
}
