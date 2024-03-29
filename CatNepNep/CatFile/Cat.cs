﻿using Yarhl.FileFormat;

namespace CatNepNep.CatFile
{
    class Cat : Format
    {

        /* HEADER 1 */
        
        /*
         * 1 = Indexed CAT
         * 2 = ???
         * 3 = CAT with names on the block
         */
        public uint HeaderType { get; set; }
        
        /*
         * 1 = No
         * 2 = Yes
         */
        public uint HasNames { get; set; }
        public uint HeaderTwoSize { get; set; }


        /* HEADER 2 */

        /*
         * CAT Type 1 = Size without Header 1
         * CAT Type 2 = ???
         * CAT Type 3 = Size with Header 1
         */
        public uint FileSize { get; set; }

        /*
         * CAT Type 1 = Null
         * CAT Type 2 = ???
         * CAT Type 3 = Size without Header 1
         */
        public uint FileSizeTwo { get; set; }

        //If HasNames=2, read this for get the size
        public uint NamesSize { get; set; }

        //The number of blocks on the file
        public uint NumberOfEntries { get; set; }

        /* NAMES BLOCK */
        public uint SizeNameChain { get; set; }
        

        /* CAT ARRAYS */
        public uint[] Positions { get; set; } //Position+HeaderTwoSize
        public uint[] Sizes { get; set; }
        public byte[][] Blocks { get; set; }
        public string[] Names { get; set; }
        //Only for Cat Type 3
        public uint[] NamesPositions { get; set; }
        public byte[] HeaderBlock { get; set; }
        public byte[] NamesBlock { get; set; }
    }
}
