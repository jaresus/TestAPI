using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.ViewModel
{
    public class WydzialVM
    {
        public int ID { get; set; }
        public string Nazwa { get; set; }
        public int IDParent { get; set; }
        public int Position { get; set; }
        public bool IsBrygada { get; set; }
    }
}
