// Copyright (C) 2021 Pedro Garau Martínez
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

using System.Collections.Generic;
using Yarhl.FileFormat;

namespace CatNepNep.IfoFile
{
    public class Ifo : Format
    {
        public int HeaderEntryCount { get; set; }
        public int HeaderEntrySize { get; set; }

        public int TextEntryCount { get; set; }
        public int TextEntrySize { get; set; }

        public List<byte[]> HeaderEntry { get; }
        public List<string> TextEntry { get; }

        public Ifo()
        {
            HeaderEntry = new List<byte[]>();
            TextEntry = new List<string>();
        }
    }
}
