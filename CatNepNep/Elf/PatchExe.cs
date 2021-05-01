using System.Collections.Generic;
using System.IO;
using ElfManipulator.Data;

namespace CatNepNep.Elf
{
    class PatchExe
    {
        private Config config;
        public PatchExe(string exePath)
        {
            var dirPath = Path.GetDirectoryName(exePath);
            if (!string.IsNullOrWhiteSpace(dirPath))
                dirPath += Path.DirectorySeparatorChar;

            InstanceSettingsConfig(exePath, dirPath);

            var apply = new CustomApplyTranslations(config);
            apply.GenerateElfPatched();
        }

        private void InstanceSettingsConfig(string exePath, string dirPath)
        {
            config = new Config()
            {
                ContainsFixedEntries = false,
                ElfPath = exePath,
                NewSize = 0x00100000,
                PoConfigs = new List<PoConfig>()
                {
                    new PoConfig()
                    {
                        EncodingId = 932,
                        SectionName = ".rdata",
                        PoPath = $"{dirPath}Executable.po",
                        CustomDictionary = false
                    }
                }
            };
        }
    }
}
