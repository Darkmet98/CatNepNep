using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace CatNepNep.IfoFile
{
    public class Po2Ifo : IConverter<Po, Ifo>
    {
        public Ifo Convert(Po source)
        {
            var ifo = new Ifo()
            {
                HeaderEntryCount = System.Convert.ToInt32(source.Header.Extensions["HeaderEntryCount"]),
                HeaderEntrySize = System.Convert.ToInt32(source.Header.Extensions["HeaderEntrySize"]),
                TextEntryCount = System.Convert.ToInt32(source.Header.Extensions["TextEntryCount"]),
                TextEntrySize = System.Convert.ToInt32(source.Header.Extensions["TextEntrySize"]),
            };

            for (int i = 0; i < ifo.HeaderEntryCount; i++)
            {
                ifo.HeaderEntry.Add(System.Convert.FromBase64String(source.Header.Extensions[$"HeaderEntry_{i}"]));
            }

            foreach (var entry in source.Entries)
            {
                ifo.TextEntry.Add(entry.Text);
            }

            return ifo;
        }
    }
}
