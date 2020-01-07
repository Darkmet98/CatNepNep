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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace CatNepNep.BinFile
{
    class Po2BinaryFormat : IConverter<Po, BinaryFormat>
    {
        private DataWriter Writer { get; set; }
        private Po Po { get; set; }
        private Dictionary<string, string> Dictionary { get; set; }
        public BinaryFormat Convert(Po source)
        {
            // Make sure that the shift-jis encoding is initialized in
            // .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if(File.Exists("Dictionary.map")) GenerateDictionary("Dictionary.map");

            Writer = new DataWriter(new DataStream())
            {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
                Endianness = EndiannessMode.LittleEndian,
            };
            Po = source;
            //Write a blank header
            Writer.WriteUntilLength(0, (8 * source.Entries.Count) + 4);
            Writer.Stream.Position = 0;
            //Write the length of strings
            Writer.Write((int)source.Entries.Count);
            //Write the content
            WriteContent();

            return new BinaryFormat(Writer.Stream);
        }

        private void WriteContent()
        {
            Writer.Stream.Position = 4;
            if (Po.Header.Extensions["Type"] == "1")
            {
                for (var i = 0; i < Po.Entries.Count; i++)
                {
                    Writer.Stream.PushCurrentPosition();

                    Writer.Stream.Position = Writer.Stream.Length;

                    var position = (uint)Writer.Stream.Position;
                    Writer.Write(GetString(i));

                    Writer.Stream.PopPosition();
                    Writer.Write(position);
                    Writer.Write(System.Convert.ToUInt32(Po.Entries[i].Reference));
                }
            }
            else
            {
                for (var i = 0; i < Po.Entries.Count; i++)
                {
                    for (var e = 0; e < 2; e++)
                    {
                        Writer.Stream.PushCurrentPosition();

                        Writer.Stream.Position = Writer.Stream.Length;

                        var position = (uint)Writer.Stream.Position;
                        Writer.Write(e == 0 ? Po.Entries[i].Reference : GetString(i));

                        Writer.Stream.PopPosition();
                        Writer.Write(position);
                    }
                   
                }
            }
            
        }

        private string GetString(int position)
        {
            var result = string.IsNullOrEmpty(Po.Entries[position].Translated) ?
                Po.Entries[position].Original : Po.Entries[position].Translated;

            if (Dictionary == null) return result != "<!null>" ? result : " ";
            
            foreach (var replace in Dictionary)
            {
                result = result.Replace(replace.Key, replace.Value);
            }

            return result;
        }

        public void GenerateDictionary(string file)
        {
            try
            {
                Dictionary = new Dictionary<string, string>();
                string[] dictionary = System.IO.File.ReadAllLines(file);
                foreach (string line in dictionary)
                {
                    string[] lineFields = line.Split('=');
                    Dictionary.Add(lineFields[0], lineFields[1]);
                }
            }
            catch (Exception e)
            {
                Console.Beep();
                Console.WriteLine(@"The dictionary is wrong, please, check the readme and fix it. Press any Key to continue.");
                Console.WriteLine(e);
                Console.ReadKey();
                System.Environment.Exit(-1);
            }
        }
    }
}
