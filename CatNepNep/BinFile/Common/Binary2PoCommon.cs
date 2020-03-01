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


using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace CatNepNep.BinFile.Common
{
    class Binary2PoCommon : IConverter<BinaryFormat, Po>
    {
        //Common Data
        protected string Id { get; set; }


        public int Count { get; set; }
        public int BlockSize { get; set; }
        protected DataReader Reader { get; set; }
        protected Po Po { get; set; }
        protected string Comment { get; set; }

        public Binary2PoCommon() 
        {
            //Read the language used by the user' OS, this way the editor can spellchecker the translation. - Thanks Liquid_S por the code
            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            Po = new Po
            {
                Header = new PoHeader("Hyperdimension Neptunia", "dummy@dummy.com", currentCulture.Name)
            };
            Comment = "";
        }


        public Po Convert(BinaryFormat source)
        {

            // Make sure that the shift-jis encoding is initialized in
            // .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Reader = new DataReader(source.Stream)
            {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
                Endianness = EndiannessMode.LittleEndian,
            };

            //Read the number of blocks on the file
            Count = Reader.ReadInt32();
            //Generate the block size
            BlockSize = GenerateBlockInfo();

            Reader.Stream.Position = 0x4;
            for (var i = 0; i < Count; i++)
            {
                DumpText();
            }


            return Po;
        }


        public virtual void DumpText()
        { }

        public virtual PoEntry GenerateEntry(string type, string text)
        {
            return new PoEntry //Generate the entry on the po file
            {
                Original = (!string.IsNullOrEmpty(text))?text : "<!null>", //Original text
                Reference = Id, //Id
                Context = Id + "|" + type, //Context type
                ExtractedComments = type + "\n#. (ASCII Char = 1 char, Special char = 2 char)"
            }; 
        }

        private int GenerateBlockInfo()
        {
            return (int) (Reader.Stream.Length - 0x4) / Count;
        }
    }
}
