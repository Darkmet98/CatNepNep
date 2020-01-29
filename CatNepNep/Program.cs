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
using System.IO;
using CatNepNep.BinFile;
using CatNepNep.CatFile;
using CatNepNep.Exceptions;
using CatNepNep.TxtFile;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.Media.Text;

namespace CatNepNep
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"CatNepNep - A Neptunia spin offs toolkit for fan translations by Darkmet98. Version: 1.0");
            if (args.Length == 1 && !string.IsNullOrEmpty(args[0]))
            {
                // get the file attributes for file or directory
                FileAttributes attr = File.GetAttributes(@args[0]);

                if (attr.HasFlag(FileAttributes.Directory)) Folders(args[0]);
                else Files(args[0]);
            }
            else Info();
        }

        private static void Folders(string folder)
        {
            var generate = new Directory2Cat(folder);
            var cat = generate.GenerateCat();
            cat.ConvertWith<Cat2Binary, Cat, BinaryFormat>().Stream.WriteTo(folder + "_new.cat");


        }

        private static void Files(string file)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var nod = NodeFactory.FromFile(file); //BinaryFormat
            Node nodPo;
            Console.WriteLine(@"Exporting " + file + @"...");

            switch (Path.GetExtension(file).ToUpper())
            {
                case ".BIN":
                    
                    IConverter<BinaryFormat, Bin> sldConverter = new Binary2Bin();
                    var nodoScript = nod.Transform(sldConverter);

                    IConverter<Bin, Po> poConv = new Bin2Po();
                    nodPo = nodoScript.Transform(poConv);
                    nodPo?.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(name + ".po");
                    break;

                case ".PO":
                    var po = new Po2BinaryFormat();
                    nod.Transform<Po2Binary, BinaryFormat, Po>();
                    nodPo = nod.Transform(po);
                    nodPo.Stream.WriteTo(name.Contains("_txt") ? name.Replace("_txt", "")+"_new.TXT":name+"_new.bin");
                    break;
                case ".CAT":
                    var cat = new Binary2Cat();
                    var nodCat = nod.Transform(cat);
                    
                    var containerConverter = new Cat2NodeContainer();
                    var nodContainer = nodCat.Transform(containerConverter);
                    if (!Directory.Exists(name)) Directory.CreateDirectory(name ?? throw new Exception("That's not supposed to throw a exception lol, please make a issue if you read this line."));

                    foreach (var child in Navigator.IterateNodes(nodContainer))
                    {
                        if (child.Stream == null) continue;

                        var output = Path.Combine(name + Path.DirectorySeparatorChar + child.Name);
                        Console.WriteLine(@"Exporting " +output + @"...");
                        child.Stream.WriteTo(output);
                    }
                    break;
                case ".TXT":
                    var convertTxt = new Binary2Po();
                    nodoScript = nod.Transform(convertTxt);
                    nodoScript?.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(name + "_txt.po");
                    break;
                default:
                    throw new FileNotSupported();

            }
        }

        private static void Info()
        {
            Console.WriteLine(@"Usage: CatNepNep ""File/Folder""");

            Console.WriteLine(@"CAT Files");
            Console.WriteLine(@"Export text.cat: CatNepNep ""text.cat""");
            Console.WriteLine(@"Import Po to TALK.DAT: CatNepNep ""text""");

            Console.WriteLine(@"Bin Files");
            Console.WriteLine(@"Export bin files to Po: CatNepNep ""NAMES.bin""");
            Console.WriteLine(@"Import the Po to the bin: CatNepNep ""NAMES.po""");
        }
    }
}
