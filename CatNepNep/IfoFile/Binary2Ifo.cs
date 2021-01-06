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
using Yarhl.FileFormat;
using Yarhl.IO;

namespace CatNepNep.IfoFile
{
    public class Binary2Ifo : IConverter<BinaryFormat, Ifo>
    {
        private Ifo ifo;
        private DataReader reader;

        public Ifo Convert(BinaryFormat source)
        {
            reader = new DataReader(source.Stream) {Stream = {Position = 0}};
            ifo = new Ifo();

            ReadHeader();
            DumpEntries();

            return ifo;
        }

        private void ReadHeader()
        {
            ifo.HeaderEntryCount = reader.ReadInt32();
            ifo.HeaderEntrySize = reader.ReadInt32();

            ifo.TextEntryCount = reader.ReadInt32();
            ifo.TextEntrySize = reader.ReadInt32();
        }

        private void DumpEntries()
        {
            for (int i = 0; i < ifo.HeaderEntryCount; i++)
            {
                ifo.HeaderEntry.Add(reader.ReadBytes(ifo.HeaderEntrySize));
            }

            for (int i = 0; i < ifo.TextEntryCount; i++)
            {
                ifo.TextEntry.Add(reader.ReadString(ifo.TextEntrySize).Replace("\0", ""));
            }
        }
    }
}
