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

using System.Collections.Generic;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace CatNepNep.BinFile
{
    class Bin2Po : IConverter<Bin, Po>
    {
        protected Po Po { get; set; }
        public Bin2Po()
        {
            //Read the language used by the user' OS, this way the editor can spellcheck the translation. - Thanks Liquid_S por the code
            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            Po = new Po
            {
                Header = new PoHeader("Hyperdimension Neptunia U: Action Unleashed", "dummy@dummy.com", currentCulture.Name),
            };
        }

        public Po Convert(BinFile.Bin source)
        {
            Po.Header.Extensions.Add("Type", source.Type.ToString());
            for (int i = 0; i < source.Count; i++)
            {
                PoEntry entry = new PoEntry(); //Generate the entry on the po file
                entry.Original = !string.IsNullOrWhiteSpace(source.Text[i]) ? source.Text[i] : "<!null>"; //Check if the string is not null
                entry.Context = i.ToString();
                entry.Reference = (source.Type == 1)?source.Id[i].ToString():source.TextId[i];
                Po.Add(entry);
            }

            return Po;
        }

    }
}
