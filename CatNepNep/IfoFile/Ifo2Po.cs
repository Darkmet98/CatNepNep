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
            po.Header.Extensions.Add("Type", "4");

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
