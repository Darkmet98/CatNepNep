using System.Text;
using CatNepNep.BinFile;
using Yarhl.Media.Text;

namespace CatNepNep.Elf
{
    class CustomMapping : ElfManipulator.Functions.GenerateMapping
    {
        public CustomMapping(byte[] elfOri, Po poPassed, Encoding encodingPassed, int memDiffPassed, bool containsFixedLength, string dictionaryPathPassed, bool customDictionary = false) : base(elfOri, poPassed, encodingPassed, memDiffPassed, containsFixedLength, dictionaryPathPassed, customDictionary)
        {
            Po2BinaryFormat.GenerateDictionary();
        }

        public override string UseDictionary(string text)
        {
            return Po2BinaryFormat.ReplaceText(text);
        }
    }
}
