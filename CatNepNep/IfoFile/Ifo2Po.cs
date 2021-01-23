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
using Yarhl.Media.Text;

namespace CatNepNep.IfoFile
{
    public class Ifo2Po : IConverter<Ifo, Po>
    {
        public Po Convert(Ifo source)
        {
            // Read the language used by the user' OS, this way the editor can spellcheck the translation. - Thanks Liquid_S por the code
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var po = new Po
            {
                Header = new PoHeader("Hyperdimension Neptunia U: Action Unleashed", "dummy@dummy.com", currentCulture.Name),
            };


            // Save the current info to the header
            po.Header.Extensions.Add("HeaderEntryCount", source.HeaderEntryCount.ToString());
            po.Header.Extensions.Add("HeaderEntrySize", source.HeaderEntrySize.ToString());
            po.Header.Extensions.Add("TextEntryCount", source.TextEntryCount.ToString());
            po.Header.Extensions.Add("TextEntrySize", source.TextEntrySize.ToString());
            po.Header.Extensions.Add("Type", "3");

            for (int i = 0; i < source.HeaderEntryCount; i++)
            {
                po.Header.Extensions.Add($"HeaderEntry_{i}", System.Convert.ToBase64String(source.HeaderEntry[i]));
            }

            // Save the text files
            for (int i = 0; i < source.TextEntryCount; i++)
            {
                po.Add(new PoEntry(source.TextEntry[i])
                {
                    Context = i.ToString()
                });
            }

            return po;
        }
    }
}
