using System;

namespace CatNepNep.Exceptions
{
    [Serializable]
    class CatNotSupported : Exception
    {
        public CatNotSupported()
            : base("Error, this cat is not supported.")
        {
        }
    }
}
