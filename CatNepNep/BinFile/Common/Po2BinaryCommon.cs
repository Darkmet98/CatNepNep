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
using System.IO;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace CatNepNep.BinFile.Common
{
    public class Po2BinaryCommon : IConverter<Po, BinaryFormat>
    {

        public DataReader OriginalFile { get; set; }
        protected DataWriter Writer;
        private BinaryFormat _binary;
        protected Po Po;

        public BinaryFormat Convert(Po source)
        {

            //Generate the new dat
            GenerateFile();

            //Dump the Po
            Po = source;

            //Insert the translated text
            InsertText();

            //Return the stream from the new file
            return new BinaryFormat(_binary.Stream);
        }

        public virtual void InsertText()
        { }


        protected void WriteText(int index, int size)
        {
            //Get the text
            var result = string.IsNullOrEmpty(Po.Entries[index].Translated) ?
                Po.Entries[index].Original : Po.Entries[index].Translated;

            //Skip if is null
            if (result == "<!null>")
            {
                Writer.Stream.Position += size;
                return;
            }
            
            //Write the text
            Writer.Write((Po2BinaryFormat.Dictionary != null)?Po2BinaryFormat.ReplaceText(result):result, size);
        }


        private void GenerateFile()
        {
            //Generate the exported file
            _binary = new BinaryFormat();
            Writer = new DataWriter(_binary.Stream) {DefaultEncoding = Encoding.GetEncoding("shift_jis")};

            //Dump the original executable to the stream
            OriginalFile.Stream.WriteTo(Writer.Stream);

            //Go to the initial index
            Writer.Stream.Position = 0;
        }
    }
}
