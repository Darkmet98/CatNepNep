using System.Collections.Generic;
using Yarhl.FileFormat;

namespace CatNepNep.IfoFile
{
    public class Ifo : Format
    {
        public int HeaderEntryCount { get; set; }
        public int HeaderEntrySize { get; set; }

        public int TextEntryCount { get; set; }
        public int TextEntrySize { get; set; }

        public List<byte[]> HeaderEntry { get; }
        public List<string> TextEntry { get; }

        public Ifo()
        {
            HeaderEntry = new List<byte[]>();
            TextEntry = new List<string>();
        }
    }
}
