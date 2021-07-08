namespace CatNepNep.BinFile.Common.Export
{
    internal class Binary2PoQuest : Binary2PoCommon
    {
        public override void DumpText()
        {
            Id = Reader.ReadString(0x14).Replace("\0", "");

            //Names
            Po.Add(GenerateEntry("Name", Reader.ReadString(0x28).Replace("\0", "")));

            //Skip unnecessary data
            Reader.Stream.Position += 0x8;

            //Characters
            Po.Add(GenerateEntry("Character", Reader.ReadString(0x1A).Replace("\0", "")));

            //Skip unnecessary data
            Reader.Stream.Position += 0x9E;

            //Clear Conditions
            Po.Add(GenerateEntry("Clear Conditions", Reader.ReadString(0x44).Replace("\0", "")));

            //Descriptions
            Po.Add(GenerateEntry("Descriptions", Reader.ReadString(0xC8).Replace("\0", "")));

            //Order
            Po.Add(GenerateEntry("Order", Reader.ReadString(0x40).Replace("\0", "")));
        }
    }
}
