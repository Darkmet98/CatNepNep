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


namespace CatNepNep.BinFile.Common.Import
{
    internal class Po2BinaryQuest : Po2BinaryCommon
    {
        public override void InsertText()
        {
            //Skip the header
            Writer.Stream.Position += 0x4;

            for (int i = 0; i < Po.Entries.Count/5; i++)
            {
                //Skip Id
                Writer.Stream.Position += 0x14;

                //Write Name
                WriteText((5 * i), 0x28);

                //Skip unnecessary data
                Writer.Stream.Position += 0x8;

                //Write Character
                WriteText((5 * i) + 1, 0x1A);

                //Skip unnecessary data
                Writer.Stream.Position += 0x9E;

                //Write Clear Conditions
                WriteText((5 * i) + 2, 0x44);

                //Write Descriptions
                WriteText((5 * i) + 3, 0xC8);

                //Write Order
                WriteText((5 * i) + 4, 0x40);
            }
        }
    }
}
