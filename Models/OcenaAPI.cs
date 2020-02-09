using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class OcenaListaAPI
    {
        public int PracownikID { get; set; }
        public string NrPersonalny { get; set; }
        public string Pracownik { get; set; }
        public IDKompetencjaOcenaAPI[] KompetencjaOcena { get; set; }
        public int Test { get; set; }
    }

    public class IDKompetencjaOcenaAPI
    {
        public int K { get; set; }
        public int O { get; set; }
        [DataType(DataType.Date)]
        public DateTime Od { get; set; }
        [DataType(DataType.Date)]
        public DateTime Do { get; set; }
        public string WID { get; set; } // wprowadzający id
        public DateTime S { get; set; } // stempel czasu
        public string Kom { get; set; } // komentarz
    }
}
