using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class ArchiwumRekordAPI
    {
        public int RowsCount { get; set; }
        public List<OcenaArchiwum> Items { get; set; }
    }
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
    public class OcenaAPI
    {
        public int ID { get; set; }
        public DateTime DataOd { get; set; }
        public DateTime? DataDo { get; set; }
        [Display(Name = "Ocena")]
        public int OcenaV { get; set; }
        public int PracownikID { get; set; }
        public string Pracownik { get; set; }
        public int KwalifikacjaID { get; set; }
        public string Kwalifikacja { get; set; }
        public string WprowadzajacyID { get; set; }
        public string Wprowadzajacy { get; set; }
        public DateTime StempelCzasu { get; set; }
        public string Komentarz { get; set; }
    }
    public class IKwalifikacjaAPI
    {
        public int ID { get; set; }
        public string Nazwa { get; set; }
        public IList<Wydzial> Wydzial { get; set; }
        public string Opis { get; set; }
        public string Link { get; set; }
        public IList<Stanowisko> Stanowisko { get; set; }
    }
    public class IKwalSzkolCel
    {
        public int KwalifikacjaID { get; set; }
        public string Kwalifikacja { get; set; }
        public int? Cel { get; set; }
        public int Wartosc { get; set; }
    }
    public class ISzkoleniaAPI
    {
        public Wydzial Wydzial { get; set; }
        public IList<IKwalSzkolCel> Items { get; set; }
    }
}
