namespace CatNepNep.BinFile.Common.Import
{
    internal class Po2BinaryQuest : Po2BinaryCommon
    {
        public override void InsertText()
        {
            //Skip the header
            Writer.Stream.Position += 0x4;

            for (int i = 0; i < Po.Entries.Count/5; i++)
            {
                //Skip Id
                Writer.Stream.Position += 0x14;

                //Write Name
                WriteText((5 * i), 0x28);

                //Skip unnecessary data
                Writer.Stream.Position += 0x8;

                //Write Character
                WriteText((5 * i) + 1, 0x1A);

                //Skip unnecessary data
                Writer.Stream.Position += 0x9E;

                //Write Clear Conditions
                WriteText((5 * i) + 2, 0x44);

                //Write Descriptions
                WriteText((5 * i) + 3, 0xC8);

                //Write Order
                WriteText((5 * i) + 4, 0x40);
            }
        }
    }
}
