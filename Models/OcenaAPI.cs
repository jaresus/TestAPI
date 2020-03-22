﻿using System;
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
        public IDKwalifikacjaOcenaAPI[] KwalifikacjaOcena { get; set; }
        public int Test { get; set; }
    }

    public class IDKwalifikacjaOcenaAPI
    {
        public int ID { get; set; }
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
    public class IWydzialKwalifikacjaAPI
    {
        public int IDw { get; set; }
        public string Nazwa { get; set; }
    }

    public class IKwalifikacjaOcenyAPI 
    { 
        public int ID { get; set; }
        public string Nazwa { get; set; }
        public IWydzialKwalifikacjaAPI[] Wydzialy { get; set; }
        public IDKwalifikacjaOcenaAPI[] Oceny { get; set; }
    }
}
