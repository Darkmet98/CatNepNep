using System;

namespace CatNepNep.Exceptions
{
    [Serializable]
    class CatFolderDamaged : Exception
    {
        public CatFolderDamaged()
            : base("Error, this Cat folder is damaged.")
        {
        }
    }
}
