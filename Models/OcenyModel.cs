using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestAPI.Models
{
    public class Ocena
    {

        public Ocena(Ocena ocena)
        {

            ID              = ocena.ID;
            DataDo          = ocena.DataDo.Value;
            DataOd          = ocena.DataOd;
            OcenaV          = ocena.OcenaV;
            PracownikID     = ocena.PracownikID;
            KwalifikacjaID  = ocena.KwalifikacjaID;
            WprowadzajacyID = ocena.WprowadzajacyID;
            StemelCzasu     = ocena.StemelCzasu;
            Komentarz       = ocena.Komentarz;
        }
        public Ocena()
        {

        }

        public int ID { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy.MM.dd}")]
        [DataType(DataType.Date)]
        public DateTime DataOd { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy.MM.dd}")]
        [DataType(DataType.Date)]
        public DateTime? DataDo { get; set; }
        [Display(Name = "Ocena")]
        public int OcenaV { get; set; }
        [Required]
        [Display(Name = "Pracownik")]
        public int PracownikID { get; set; }
        //[NotMapped]
        public Pracownik Pracownik { get; set; }
        [Required]
        public int KwalifikacjaID { get; set; }
        public Kwalifikacja Kwalifikacja { get; set; }
        public string WprowadzajacyID { get; set; }
        public ApplicationUser Wprowadzajacy { get; set; }
        public DateTime StemelCzasu { get; set; }
        public string Komentarz { get; set; }
    }

    public class Wydzial
    {
        public int ID { get; set; }
        public string Nazwa { get; set; }
        public int IDParent { get; set; }
        public int Position { get; set; }
        public bool IsBrygada { get; set; }
        public ICollection<Kwalifikacja> Kwalifikacje { get; set; }
        [JsonIgnore]
        public ICollection<Pracownik> Pracownicy { get; set; }
        [JsonIgnore]
        public IList<KwalifikacjaWydzial> KwalifikacjaWydzial { get; set; }

    }
    public class Kwalifikacja
    {
        public int ID { get; set; }
        public string Nazwa { get; set; }
        [Display(Name = "Wydział")]
        public IList<KwalifikacjaWydzial> KwalifikacjaWydzial { get; set; }
        public string Opis { get; set; }
        public string Link { get; set; }
        [JsonIgnore]
        public ICollection<Ocena> Oceny { get; set; }

    }
    public class KwalifikacjaWydzial
    {
        public int KwalifikacjaID { get; set; }
        [JsonIgnore]
        public Kwalifikacja Kwalifikacja { get; set; }
        public int WydzialID { get; set; }
        //[JsonIgnore]
        public Wydzial Wydzial { get; set; }
    }
    public class Pracownik
    {
        public int ID { get; set; }
        [Display(Name = "Numer personalny")]
        public string NrPersonalny { get; set; }
        public string Nazwisko { get; set; }
        [Display(Name = "Imię")]
        public string Imie { get; set; }
        [Display(Name = "Wydział")]
        public int WydzialID { get; set; }
        public Wydzial Wydzial { get; set; }
        public string Firma { get; set; } //bowa, randstad
        [Display(Name = "Aktywny")]
        public bool IsActive { get; set; }
        public ICollection<Ocena> Oceny { get; set; }
        [NotMapped]
        [Display(Name = "Pracownik")]
        public string FullName => Nazwisko + ", " + Imie;
        public string GetFullName()
        {
            return Nazwisko + ", " + Imie;
        }

    }
}
