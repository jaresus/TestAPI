using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatrycaKwalifikacji.ViewModels
{
    public class TreeViewNode
    {
        public int Id { get; set; }
        public string Parent { get; set; }
        public string Text { get; set; }
        public string IsBrygada { get; set; }

    }
}
