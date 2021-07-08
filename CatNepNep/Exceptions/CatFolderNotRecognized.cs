using System;

namespace CatNepNep.Exceptions
{
    [Serializable]
    class CatFolderNotRecognized : Exception
    {
        public CatFolderNotRecognized()
            : base("Error, this folder is not a Exported Cat.")
        {
        }
    }
}
