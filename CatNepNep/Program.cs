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
using System.Text;
using CatNepNep.BinFile;
using CatNepNep.BinFile.Common.Export;
using CatNepNep.BinFile.Common.Import;
using CatNepNep.CatFile;
using CatNepNep.Exceptions;
using CatNepNep.IfoFile;
using CatNepNep.TxtFile;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace CatNepNep
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"CatNepNep - A Neptunia spin offs toolkit for fan translations by Darkmet98. Version: 1.0");

            // Make sure that the shift-jis encoding is initialized in
            // .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (args.Length == 1 && !string.IsNullOrEmpty(args[0]))
            {
                // get the file attributes for file or directory
                FileAttributes attr = File.GetAttributes(@args[0]);

                if (attr.HasFlag(FileAttributes.Directory)) Folders(args[0]);
                else Files(args[0]);
            }
            else if (args.Length == 2)
            {
                switch (args[1])
                {
                    case "-common":
                        BinCommons(args[0]);
                        break;
                    case "-dlc":
                        BinDlc(args[0]);
                        break;
                    default:
                        Info();
                        break;
                }
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
                    nod.Transform<Po2Binary, BinaryFormat, Po>();

                    var type = Convert.ToInt32(nod.GetFormatAs<Po>().Header.Extensions["Type"]);
                    if (type == 3)
                        nod.Transform(new Po2Ifo()).Transform(new Ifo2Binary()).Stream.WriteTo(name + ".ifo");
                    else 
                        nod.Transform(new Po2BinaryFormat()).Stream.WriteTo(name.Contains("_txt") ? name.Replace("_txt", "")+"_new.TXT":name+"_new.bin");
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
                case ".IFO":
                    nod.Transform(new Binary2Ifo()).Transform(new Ifo2Po()).Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(name + ".po");
                    break;
                default:
                    throw new FileNotSupported();

            }
        }

        private static void BinDlc(string file)
        {
            var node = NodeFactory.FromFile(file);
            var name = Path.GetFileNameWithoutExtension(file);

            switch (Path.GetExtension(file).ToUpper())
            {
                case ".BIN":
                    Console.WriteLine(@"Exporting " + file + @"...");
                    node.Transform(new BinFile.Dlc.Binary2Po()).Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(name + ".po");
                    break;
                case ".PO":
                    Console.WriteLine(@"Importing " + file + @"...");
                    node.Transform<Po2Binary, BinaryFormat, Po>().Transform(new BinFile.Dlc.Po2Binary()).Stream.WriteTo(name + ".bin");
                    break;
            }
        }

        private static void BinCommons(string file)
        {
            var nodo = NodeFactory.FromFile(file); //BinaryFormat
            var name = Path.GetFileNameWithoutExtension(file);

            switch (Path.GetExtension(file).ToUpper())
            {
                case ".BIN":
                    
                    Node nodPo;
                    Console.WriteLine(@"Exporting " + file + @"...");
                    IConverter<BinaryFormat, Po> nodeConverter = null;
                    switch (name)
                    {
                        //HyperDimension Neptunia U
                        case "QUEST":
                            nodeConverter = new Binary2PoQuest();
                            break;
                        default:
                            throw new FileNotSupported();
                    }
                    var nodoPo = nodo.Transform(nodeConverter);
                    nodoPo?.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(name + ".po");
                    break;
                case ".PO":

                    Node nodoOut;
                    IConverter<Po, BinaryFormat> importer = null;
                    Console.WriteLine(@"Importing " + name + @"...");
                    switch (name)
                    {
                        //HyperDimension Neptunia U
                        case "QUEST":
                            importer = new Po2BinaryQuest()
                            {
                                OriginalFile = new DataReader(new DataStream(name + ".bin", FileOpenMode.Read))
                                {
                                    DefaultEncoding = Encoding.GetEncoding("shift_jis"),
                                    Endianness = EndiannessMode.LittleEndian,
                                }
                            };
                            nodo.Transform<Po2Binary, BinaryFormat, Po>();
                            nodoOut = nodo.Transform(importer);
                            nodoOut.Stream.WriteTo(name+"_new.bin");
                    break;
                        default:
                            throw new FileNotSupported();
                    }
                    break;
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
