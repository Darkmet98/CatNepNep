using Yarhl.FileFormat;

namespace CatNepNep.BinFile
{
    class Bin : Format
    {
        public uint Count { get; set; }
        public uint[] Positions { get; set; }
        public uint[] Id { get; set; }
        public string[] Text { get; set; }
        public string[] TextId { get; set; }

        /*
         * 1 = Id hardcoded
         * 2 = Plain Id
         */
        public int Type { get; set; }
    }
}
