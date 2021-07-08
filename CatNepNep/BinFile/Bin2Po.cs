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
