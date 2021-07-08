using System;

namespace CatNepNep.Exceptions
{
    [Serializable]
    class FileNotSupported : Exception
    {
        public FileNotSupported()
            : base("Error, CatNepNep don't work for now with this filetype, please check the compatibility list.")
        {
        }
    }
}
