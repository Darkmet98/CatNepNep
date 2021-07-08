using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;
using TextWriter = Yarhl.IO.TextWriter;

namespace CatNepNep.BinFile
{
    class Po2BinaryFormat : IConverter<Po, BinaryFormat>
    {
        private DataWriter Writer { get; set; }
        private Po Po { get; set; }
        public static Dictionary<string, string> Dictionary { get; set; }
        public BinaryFormat Convert(Po source)
        {
            // Make sure that the shift-jis encoding is initialized in
            // .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Po = source;
            
            if (Po.Header.Extensions["Type"] == "3")
            {
                return new BinaryFormat(WriteTxt());
            }

            Writer = new DataWriter(new DataStream())
            {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
                Endianness = EndiannessMode.LittleEndian,
            };

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
            var result = Po.Entries[position].Text;

            if (result == "<!null>")
                return " ";

            return (Dictionary == null ? result : ReplaceText(result)).Replace("\\\\p", "\\p");
        }

        public static string ReplaceText(string line)
        {
            string result = line;
            foreach (var replace in Dictionary)
            {
                result = result.Replace(replace.Key, replace.Value);
            }

            return result;
        }

        private DataStream WriteTxt()
        {
            TextWriter writer = new TextWriter(new DataStream(), Encoding.GetEncoding("shift_jis"))
            {
                NewLine = "\r\n"
            };
            foreach (var entry in Po.Entries)
            {
                writer.WriteLine("{" + (!string.IsNullOrEmpty(entry.Translated)?
                                     (Dictionary != null)?ReplaceText(entry.Translated):
                                        entry.Translated
                                     :entry.Original) + "}\r\n");
            }
            return writer.Stream;
        }

        public static void GenerateDictionary()
        {
            var file = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                      Path.DirectorySeparatorChar + "Dictionary.map";
            if (!File.Exists(file))
                throw new Exception("The dictionary file doesn't exist.");

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
