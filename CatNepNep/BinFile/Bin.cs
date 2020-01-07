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

namespace CatNepNep.BinFile
{
    class Bin : Format
    {
        public uint Count { get; set; }
        public uint[] Positions { get; set; }
        public uint[] Id { get; set; }
        public string[] Text { get; set; }
        public string[] TextId { get; set; }

        /*
         * 1 = Id hardcoded
         * 2 = Plain Id
         */
        public int Type { get; set; }
    }
}
