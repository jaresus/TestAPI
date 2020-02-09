using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatrycaKwalifikacji.ViewModels
{
    public class TreeViewNode
    {
        public int id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string IsBrygada { get; set; }

    }
}
