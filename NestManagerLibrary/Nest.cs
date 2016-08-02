using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NestManagerLibrary
{
    public class Nest
    {
        public string CutNumber { get; set; }
        public string JobNumber { get; set; }
        public string Sequence { get; set; }
        public string BatchNumber { get; set; }
        public string Thickness { get; set; }
        public string FileName { get; set; }
        public bool Nested { get; set; }
        public bool Partial { get; set; }
        public bool Completed { get; set; }
        public int OrderID { get; set; }
        public string completedate;
        public string partslist;
    }
}
