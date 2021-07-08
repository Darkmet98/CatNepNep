using System.Text;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace CatNepNep.CatFile
{
    class Cat2NodeContainer : IConverter<Cat, NodeContainerFormat>
    {
        public NodeContainerFormat Convert(Cat source)
        {
            NodeContainerFormat container = new NodeContainerFormat();
            Node child;
            string info = source.HasNames + "\n" + //Has Names
                          source.NumberOfEntries + "\n" + //Number Of Files
                          source.HeaderType + "\n" + //Header Type
                          source.HeaderTwoSize+ "\n"; //Header 2 Size


            for (int i = 0; i < source.NumberOfEntries; i++)
            {
                var name = ((source.Names == null) ? i.ToString().PadLeft(5, '0') + ".bin" : source.Names[i]);
                child = NodeFactory.FromMemory(name);
                child.Stream.Write(source.Blocks[i], 0, source.Blocks[i].Length);
                container.Root.Add(child);
                info += name + "\n";
            }

            //Add the header
            child = NodeFactory.FromMemory("HEADER.catBin");
            child.Stream.Write(source.HeaderBlock, 0, source.HeaderBlock.Length);
            container.Root.Add(child);

            //Check the Names
            if (source.HasNames == 2)
            {
                child = NodeFactory.FromMemory("NAMES.catBin");
                child.Stream.Write(source.NamesBlock, 0, source.NamesBlock.Length);
                container.Root.Add(child);
            }

            //Write the info
            byte[] infoBytes = Encoding.UTF8.GetBytes(info);
            child = NodeFactory.FromMemory("INFO.catBin");
            child.Stream.Write(infoBytes, 0, infoBytes.Length);
            container.Root.Add(child);

            return container;
        }
    }
}
